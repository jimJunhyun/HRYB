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
		GameManager.instance.DisableCtrl(ControlModuleMode.Animated);
		
		
		PlayerAttack tt = self.atk as PlayerAttack;
		if(tt._grabCO != null)
			tt.StopCoroutine(tt._grabCO);
		value = tt.BleedValue;
		tt._grabedEnemy.GetComponent<Actor>().move._isCanMove = true;
		tt._grabedEnemy.transform.parent = tt._grabPos;
		tt._grabedEnemy.transform.position = tt._grabPos.position;
		tt._grabedEnemy.GetComponent<Actor>().move.gravity = false;
		tt.BleedValue = 0;
		tt._grabedEnemy.GetComponent<CharacterController>().enabled = false;
		tt._grabedEnemy.GetComponent<NavMeshAgent>().enabled = false;
		Debug.LogError(tt._grabedEnemy);
	}

	public override void OnAnimationMove(Actor self, AnimationEvent evt)
	{
		
		self.move.forceDir = self.transform.forward * 16 + new Vector3(0, 24, 0);
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
		if (obj.TryGetComponent<ColliderCast>(out _cols))
		{
			_cols.Now(self.transform, (_life) =>
			{
				DoDamage(_life.GetActor(), self);

				CameraManager.instance.ShakeCamFor(0.12f, 8, 8);
				
				_life.GetActor().move.forceDir += self.transform.forward * 30 + new Vector3(0, -20, 0);
			});
		}
	}


	public override void OnAnimationEnd(Actor self, AnimationEvent evt)
	{
		PlayerAttack tt = self.atk as PlayerAttack;
		if (_cols != null)
		{
			_cols.End();
			_cols = null;
		}
		
	}

	public override void OnAnimationStop(Actor self, AnimationEvent evt)
	{
		GameManager.instance.EnableCtrl(ControlModuleMode.Animated);
		
	}


	protected override void DoDamage(Actor to, Actor by)
	{
		if (value <= 0)
		{

			value = 1;
		}
		
		to.life.DamageYY(by.atk.initDamage * damageMult * value * 0.8f, DamageType.DirectHit, 0, 0, by);
		Debug.Log($"[데미지] {to.gameObject.name} 에게 데미지 : {by.atk.initDamage} * {damageMult} * {value} = {(by.atk.initDamage * damageMult * value * 0.8f)}");
		for (int i = 0; i < statEff.Count; i++)
		{
			StatusEffects.ApplyStat(to, by, statEff[i].id, statEff[i].duration, statEff[i].power);
		}
	}
	
}
