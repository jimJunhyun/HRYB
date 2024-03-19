using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Infos/FireArrowAtOnce")]
public class FireArrowAtOnce : AttackBase
{
	public string arrowPrefabName;

	public float circularRad;
	public float circularAngle;

	public ArrowMode aMode;

	Arrow arrow;

	internal override void MyDisoperation(Actor self)
	{

		//UnityEditor.EditorApplication.isPaused = true;
		if(arrow != null)
		{
			//Debug.Log($"화살발사, {arrow.transform.position} : {arrow.transform.forward}");
			if(aMode == ArrowMode.Homing)
			{
				arrow.SetTarget(self.atk.target);
			}
			GameManager.instance.audioPlayer.PlayPoint(audioClipName, self.transform.position);
			arrow.Shoot(aMode);
			//arrow.SetDisappearTimer();
			arrow.ResumeCheck();
			arrow = null;
		}
	}

	internal override void MyOperation(Actor self)
	{
		Vector3 pos = GameManager.instance.player.transform.position + Vector3.up;
		pos.x += Mathf.Cos(circularAngle * Mathf.Deg2Rad) * circularRad;
		pos.y += Mathf.Sin(circularAngle * Mathf.Deg2Rad) * circularRad;
		arrow = PoolManager.GetObject(arrowPrefabName, pos, relatedTransform.forward).GetComponent<Arrow>();
		arrow.SetInfo(self.atk.initDamage * damageMult);
		(self.atk as PlayerAttack).onNextUse?.Invoke(arrow.gameObject);
		(self.atk as PlayerAttack).onNextSkill?.Invoke(self, this);
		arrow.SetHitEff((self.atk as PlayerAttack).onNextHit);
		for (int i = 0; i < statEff.Count; i++)
		{
			arrow.AddStatusEffect(statEff[i]);
		}
		arrow.StopDisappearTimer();
		arrow.StopCheck();
		arrow.SetOwner(self);

	}

	public override void UpdateStatus()
	{
		if(arrow != null)
		{
			Vector3 pos = GameManager.instance.player.transform.position + Vector3.up;
			Vector3 posDiff = (relatedTransform.right * (Mathf.Cos(circularAngle * Mathf.Deg2Rad) * circularRad)) + (relatedTransform.up * (Mathf.Sin(circularAngle * Mathf.Deg2Rad) * circularRad));
			pos += posDiff;

			arrow.transform.position = pos;
			arrow.transform.forward = relatedTransform.forward;
		}
		
	}
}
