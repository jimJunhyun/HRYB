using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Yoho/첫번째공격스")]
public class OnePunchGrabSkill : YGComboAttackBase
{

	public override void OnAnimationStart(Actor self, AnimationEvent evt)
	{
		GameManager.instance.DisableCtrl();
	}

	public override void OnAnimationMove(Actor self, AnimationEvent evt)
	{
		self.move.forceDir += self.transform.forward * 20;
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
				
				
				_life.GetActor().move.forceDir += self.transform.forward * 32 + new Vector3(0,7,0);
				
				
			}, (trm, _life) =>
			{
				CameraManager.instance.ShakeCamFor(0.12f, 3, 3);
				self.move.forceDir = Vector3.zero;
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
				
				Debug.LogError(tt._grabedEnemy);

				DoDamage(_life.GetActor(), self, t);

				if (t >= 10)
				{
					_nextTo?.Invoke();
					PlayerAttack pl = self.atk as PlayerAttack;
					pl.BleedValue = t;
				}
				else
				{
					_nextTo?.Invoke();
					PlayerAttack pl = self.atk as PlayerAttack;
					pl.BleedValue = t;
					
					//_life.RemoveAllStatEff(StatEffID.Bleeding);
				}

			});
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
	


	
}
