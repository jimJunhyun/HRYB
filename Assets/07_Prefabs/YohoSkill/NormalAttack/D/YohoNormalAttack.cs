using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Yoho/기본공격")]
public class YohoNormalAttack : AttackBase
{
	ColliderCast _cols = null;

	
	
	internal override void MyOperation(Actor self)
	{
		
	}

	internal override void MyDisoperation(Actor self)
	{
		
	}

	public override void UpdateStatus()
	{
		
	}

	void NormalAttackOne(Actor self, AnimationEvent evt)
	{
		Vector3 dir = self.transform.forward;
		self.move.forceDir = dir * 2;
		
	}
	
	void NormalAttackTwo(Actor self, AnimationEvent evt)
	{
		Vector3 dir = self.transform.forward;
		self.move.forceDir = dir * 3.4f;
		
	}
	

	public override void OnAnimationStart(Actor self, AnimationEvent evt)
	{
		GameManager.instance.DisableCtrl(ControlModuleMode.Animated);
		
		
	}

	public override void OnAnimationMove(Actor self, AnimationEvent evt)
	{
		string[] tt = evt.stringParameter.Split("$");
		
		switch (tt[0])
		{
			case "1":
				NormalAttackOne(self,evt);
				break;
			
			case "2":
				NormalAttackTwo(self,evt);
				break;
		}
	}
	

	public override void OnAnimationEvent(Actor self, AnimationEvent evt)
	{
		//self.move.forceDir = new Vector3(0, 0, 0);
		
		if (_cols != null)
		{
			_cols.End();
			_cols = null;
		}
		
		string[] tt = evt.stringParameter.Split("$");
		Debug.LogError(tt[0]);
		switch (tt[0])
		{
			case "1":
				{
					GameObject obj1 = PoolManager.GetObject("YusungSmithleft", self.transform);
					if (obj1.TryGetComponent<EffectObject>(out EffectObject eff1))
					{
						eff1.Begin();
						self.StartCoroutine(DeleteObj(obj1));
					}
				}
				break;

			case "2":
				{
					GameObject obj2 = PoolManager.GetObject("YusungSmithright", self.transform);
					if (obj2.TryGetComponent<EffectObject>(out EffectObject eff2))
					{
						eff2.Begin();
						self.StartCoroutine(DeleteObj(obj2));
					}
				}
				break;
		}
		

		GameObject obj = PoolManager.GetObject("YohoNormalAttack", self.transform);
		if (obj.TryGetComponent<ColliderCast>(out _cols))
		{
			_cols.Now(self.transform, (_life) =>
			{
				CameraManager.instance.ShakeCamFor(0.08f, 2, 2);
				DoDamage(_life.GetActor(), self);
				    
			});
		}
		
		
	}
	
	IEnumerator DeleteObj(GameObject obj, float t = 1.0f)
	{
		yield return new WaitForSeconds(t);
		PoolManager.ReturnObject(obj);
	}
	
	public override void OnAnimationEnd(Actor self, AnimationEvent evt)
	{
		self.move.forceDir= Vector3.zero;
		self.move.moveDir = Vector3.zero;
		if (_cols != null)
		{
			_cols.End();
			//_cols = null;
		}
	}
	
	
	public override void OnAnimationStop(Actor self, AnimationEvent evt)
	{
		GameManager.instance.EnableCtrl(ControlModuleMode.Animated);
	}
}
