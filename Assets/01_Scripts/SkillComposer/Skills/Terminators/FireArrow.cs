using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[CreateAssetMenu(menuName = "Skills/Infos/FireArrow")]
public class FireArrow : AttackBase
{
	public string arrowPrefabName;
	public float angleY;

	public ArrowMode aMode;

	internal override void MyDisoperation(Actor self)
	{

	}

	internal override void MyOperation(Actor self)
	{
		//eff.Play();
		Debug.Log($"화살발사");
		GameObject g = PoolManager.GetObject(arrowPrefabName, relatedTransform.position, relatedTransform.forward);
		if(!g.TryGetComponent<Arrow>(out Arrow r))
		{

			throw new System.Exception("Trying to shoot strange things");
		}
		Vector3 localRot = r.transform.localEulerAngles;
		localRot.y += angleY;
		r.transform.localEulerAngles = localRot;
		//UnityEditor.EditorApplication.isPaused = true;
		r.SetInfo(self.atk.initDamage * damageMult);
		r.SetOwner(self);
		(self.atk as PlayerAttack).onNextUse?.Invoke(r.gameObject);
		(self.atk as PlayerAttack).onNextSkill?.Invoke(self, this);
		r.SetHitEff((self.atk as PlayerAttack).onNextHit);
		for (int i = 0; i < statEff.Count; i++)
		{
			r.AddStatusEffect(statEff[i]);
		}

		if (aMode == ArrowMode.Homing)
		{
			r.SetTarget(self.atk.target);
		}
		r.Shoot(aMode);
		GameManager.instance.audioPlayer.PlayPoint(audioClipName, self.transform.position);
		//self.anim.SetAttackTrigger();
	}

	public override void UpdateStatus()
	{
		
	}
}
