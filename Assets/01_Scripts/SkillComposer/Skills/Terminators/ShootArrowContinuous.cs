using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Infos/ShootArrowJitter")]
public class ShootArrowContinuous : AttackBase
{
	public string arrowPrefabName;
	public ArrowMode aMode;
	public float angleJitterAmt;
	public override void UpdateStatus()
	{
		
	}

	internal override void MyDisoperation(Actor self)
	{
		
	}

	public override void Operate(Actor self)
	{
		base.Operate(self);
		MyOperation(self);
	}

	internal override void MyOperation(Actor self)
	{
		//Debug.Log($"화살발사");
		GameObject g = PoolManager.GetObject(arrowPrefabName, relatedTransform.position, relatedTransform.forward);
		if (!g.TryGetComponent<Arrow>(out Arrow r))
		{

			throw new System.Exception("Trying to shoot strange things");
		}
		Vector3 localRot = r.transform.localEulerAngles;
		localRot.y += Random.Range(-angleJitterAmt, angleJitterAmt);
		r.transform.localEulerAngles = localRot;
		//UnityEditor.EditorApplication.isPaused = true;
		r.SetInfo(self.atk.Damage * damageMult);
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
	}
}
