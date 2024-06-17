using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "Skills/Yoho/두번째공격스")]
public class TwoPunchGrabSkill : AttackBase
{
	protected ColliderCast _cols = null;

	private int value = 0;
	internal override void MyOperation(Actor self)
	{
		
	}
	internal override void MyDisoperation(Actor self)
	{
		
	}
	public override void UpdateStatus()
	{
		
	}

	public override void OnAnimationStart(Actor self, AnimationEvent evt)
	{
		GameManager.instance.DisableCtrl();
		(self.anim as PlayerAnim).AnimAct.PlayerAfterImage(0.2f, 1.3f, 0.66f);

		PlayerAttack tt = self.atk as PlayerAttack;
		if(tt._grabCO != null)
			tt.StopCoroutine(tt._grabCO);
		value = tt.BleedValue;
		tt._grabedEnemy.GetComponent<Actor>().move._isCanMove = true;
		tt._grabedEnemy.transform.parent = tt._grabPos;
		tt._grabedEnemy.transform.position = tt._grabPos.position;
		tt._grabedEnemy.GetComponent<Actor>().move.gravity = false;
		tt.BleedValue = 0;
		if(tt._grabedEnemy.TryGetComponent<CharacterController>(out CharacterController c))
		{
			c.enabled = false;
		}

		if (tt._grabedEnemy.TryGetComponent<NavMeshAgent>(out NavMeshAgent n))
		{
			n.enabled = false;
		}
		//Debug.LogError(tt._grabedEnemy);
	}

	public override void OnAnimationMove(Actor self, AnimationEvent evt)
	{
		
		self.move.forceDir = -self.transform.forward * 11 + new Vector3(0, 7, 0);
	}

	public override void OnAnimationEvent(Actor self, AnimationEvent evt)
	{
		GameObject obj = PoolManager.GetObject("YohoGrab", self.transform);
		PlayerAttack tt = self.atk as PlayerAttack;
		tt._grabedEnemy.GetComponent<Actor>().move._isCanMove = false;
		tt._grabedEnemy.gameObject.transform.parent = null;
		tt._grabedEnemy.GetComponent<Actor>().move.gravity = true;
		tt._grabedEnemy.GetComponent<CharacterController>().enabled = true;
		//tt._grabedEnemy.GetComponent<Actor>().move.forceDir = new Vector3(0, 1, 0);

					int f = tt.BleedValue;
		tt.BleedValue = 0;
		if (obj.TryGetComponent<ColliderCast>(out _cols))
		{
			_cols.Now(self.transform, (_life) =>
			{


				StatusEffects.ApplyStat(_life.GetActor(), self, StatEffID.Stun, 4f);
				
			}, (tls, _life) =>
			{
				CameraManager.instance.ShakeCamFor(0.24f, 20, 20);

				DoDamage(_life.GetActor(), self, _dmgs[0], default, f);

				_life.GetActor().move.forceDir= self.transform.forward * 30 + new Vector3(0,-10, 0);
				GameObject objs = PoolManager.GetObject("YohoGrab_Down", _life.transform);
				if(objs.TryGetComponent<ColliderCast>(out ColliderCast _cols2))
				{
					_cols2.Now(_life.transform, (_lifes) =>
					{
						DoDamage(_lifes.GetActor(), self, _dmgs[1], default, f * 0.1f);
						StatusEffects.ApplyStat(_lifes.GetActor(), self, StatEffID.Stun, 4f);
						_lifes.GetActor().move.forceDir += self.transform.forward * 12f;
					}, default, default, default, 1.2f);
				}
			}, default, default, 0.2f);
		}
	}


	public override void OnAnimationEnd(Actor self, AnimationEvent evt)
	{
	}

	public override void OnAnimationStop(Actor self, AnimationEvent evt)
	{
		GameManager.instance.EnableCtrl();
		
	}

	public override int ListValue()
	{
		return 2;
	}
}
