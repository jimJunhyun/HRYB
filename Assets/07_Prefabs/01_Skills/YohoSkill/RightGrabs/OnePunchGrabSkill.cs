using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Yoho/첫번째공격스")]
public class OnePunchGrabSkill : YGComboAttackBase
{
	[Header("SkillIcon")]
	public Sprite spi1;
	public Sprite spi2;
	public override void OnAnimationStart(Actor self, AnimationEvent evt)
	{
		GameManager.instance.DisableCtrl();
		(self.anim as PlayerAnim).AnimAct.PlayerAfterImage(0.2f, 0.6f, 0.66f);
	}

	public override void OnAnimationMove(Actor self, AnimationEvent evt)
	{
		self.move.forceDir += self.transform.forward * 16;
	}

	public override void OnAnimationEvent(Actor self, AnimationEvent evt)
	{
		self.move.forceDir = new Vector3(0, 0, 0);
		if (_cols != null)
		{
			_cols.End();
			_cols = null;
		}

		
		GameObject obj = PoolManager.GetObject("YohoGrab", self.transform);
		if (obj.TryGetComponent<ColliderCast>(out _cols))
		{
			
			
			
			_cols.Now(self.transform, (_life) =>
			{
				
				
				//_life.GetActor().move.forceDir += self.transform.forward * 5 + new Vector3(0,7,0);
				StatusEffects.ApplyStat(_life.GetActor(), self, StatEffID.Stun, 3f);
				
				
			}, (trm, _life) =>
			{

				StatusEffects.ApplyStat(_life.GetActor(), self, StatEffID.Bleeding, 4f);

				CameraManager.instance.ShakeCamFor(0.12f, 12, 12);
				int t = 0;
				foreach (var value in _life.appliedDebuff)
				{
					if (value.Value.eff.Equals(
						    (StatusEffect)GameManager.instance.statEff.idStatEffPairs[(int)StatEffID.Bleeding]))
					{
						t++;
					}
				}


				PlayerAttack tt = self.atk as PlayerAttack;

				tt._grabedEnemy = _life.gameObject;
				
				//Debug.LogError(tt._grabedEnemy);

				DoDamage(_life.GetActor(), self, _dmgs[0], default, t * 0.5f);




				if (t >= 0 && _life.tag != "Jansung")
				{
					if((_life.yy.white.Value > 0))
					{

						_nextTo?.Invoke();
						PlayerAttack pl = self.atk as PlayerAttack;
						pl.BleedValue = t;
						self.StartCoroutine(YeildTime(self));
					}
					else
					{

						GameObject obj2 = PoolManager.GetObject("MasterSparkCols", self.transform);

						EffectObject effFailed = PoolManager.GetEffect("Softbluebuff", self.transform);
						effFailed.Begin();

						if (obj2.TryGetComponent<ColliderCast>(out ColliderCast _cols2))
						{
							_cols2.Now(self.transform, (_life2) =>
							{
								DoDamage(_life2.GetActor(), self, _dmgs[1], default, t * 0.25f);
							}, (tls, tl2) =>
							{

								CameraManager.instance.ShakeCamFor(0.12f, 24, 24);
							}, default,0.5f,0.7f);
						}
					}

				}
				else
				{
					//_nextTo?.Invoke();

					if (_life.tag == "Jansung")
					{
						GameObject obj2 = PoolManager.GetObject("MasterSparkCols", self.transform);

						EffectObject effFailed = PoolManager.GetEffect("Softbluebuff", self.transform);
						effFailed.Begin();

						if (obj2.TryGetComponent<ColliderCast>(out ColliderCast _cols2))
						{
							_cols2.Now(self.transform, (_life2) =>
							{
								DoDamage(_life2.GetActor(), self, _dmgs[1], default, t * 0.25f);
							}, (tls, tl2) =>
							{

								CameraManager.instance.ShakeCamFor(0.12f, 24, 24);
							}, default, 0.5f, 0.7f);
						}
					}


					//(self.cast as PlayerCast).SetCooldownTo(SkillSlotInfo.RClick, 0.2f);
					//_life.RemoveAllStatEff(StatEffID.Bleeding);
				}

			}, default, default, 0.4f);
		}
		
	}

	public override void OnAnimationEnd(Actor self, AnimationEvent evt)
	{
		if (_cols != null)
		{
			_cols.End();
			_cols = null;
		}

	}

	public override void OnAnimationStop(Actor self, AnimationEvent evt)
	{
		GameManager.instance.EnableCtrl();
	}

	public override int ListValue()
	{
		return 2;
	}

	IEnumerator YeildTime(Actor self)
	{
		//Debug.LogError("되긴함");

		(self.cast as PlayerCast).GetSkillRoot(SkillSlotInfo.RClick).skillIcon = spi1;
		(self.cast as PlayerCast).SetCooldownTo(SkillSlotInfo.RClick, 0.2f);
		yield return new WaitForSeconds(0.6f);
		(self.cast as PlayerCast).GetSkillRoot(SkillSlotInfo.RClick).skillIcon = spi2;
		(self.cast as PlayerCast).SetCooldownSet(SkillSlotInfo.RClick, 0);
	}
}
