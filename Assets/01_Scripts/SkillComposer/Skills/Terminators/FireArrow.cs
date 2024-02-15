using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[CreateAssetMenu(menuName = "Skills/Infos/FireArrow")]
public class FireArrow : AttackBase
{
	public string arrowPrefabName;
	public float angleY;

	internal override void MyDisoperation(Actor self)
	{

	}

	internal override void MyOperation(Actor self)
	{
		//eff.Play();
		//Debug.Log($"화살발사, {shootPos.position} : {shootPos.forward}");
		Arrow r = PoolManager.GetObject(arrowPrefabName, relatedTransform.position, relatedTransform.forward).GetComponent<Arrow>();
		Vector3 localRot = r.transform.localEulerAngles;
		localRot.y += angleY;
		r.transform.localEulerAngles = localRot;
		//UnityEditor.EditorApplication.isPaused = true;
		r.SetInfo(damage);
		r.SetOwner(self);
		r.SetHitEff((self.atk as PlayerAttack).onNextHits?.Invoke(r.gameObject, self, null, this));
		for (int i = 0; i < statEff.Count; i++)
		{
			r.AddStatusEffect(statEff[i]);
		}

		r.Shoot();
		GameManager.instance.audioPlayer.PlayPoint(audioClipName, self.transform.position);
		//self.anim.SetAttackTrigger();
	}

	public override void UpdateStatus()
	{
		
	}
}
