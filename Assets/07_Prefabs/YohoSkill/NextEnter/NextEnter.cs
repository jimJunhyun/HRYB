using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Skills/Yoho/넘어가기")]
public class NextEnter : AttackBase
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
	
	public override void OnAnimationStart(Actor self, AnimationEvent evt)
	{
		GameManager.instance.DisableCtrl();
	}

	public override void OnAnimationMove(Actor self, AnimationEvent evt)
	{
		Vector3 vec = self.transform.forward;
		self.move.forceDir = vec * 20 + new Vector3(0, 1, 0) * 24;
	}
	
	public override void OnAnimationEvent(Actor self, AnimationEvent evt)
	{

		
			if (_cols != null)
			{
				_cols.End();
				_cols = null;
			}
		
			GameObject obj = PoolManager.GetObject("YohoNextAttack", self.transform);
			if (obj.TryGetComponent<ColliderCast>(out _cols))
			{
				_cols.Now(self.transform, (_life) =>
				{
					DoDamage(_life.GetActor(), self);
				
				
				}, (tls, _life) =>
				{
					Vector3 vec = self.transform.forward;
					CameraManager.instance.ShakeCamFor(0.16f, 2, 2);
					self.move.forceDir += vec * 12 + new Vector3(0, 1, 0) * 2;

					GameObject obj1 = PoolManager.GetObject("NextEnterEff", self.transform);
					if (obj1.TryGetComponent<EffectObject>(out EffectObject eff1))
					{
						eff1.Begin();
						obj1.transform.parent = null;
						obj1.transform.position = _life.transform.position;
						self.StartCoroutine(DeleteObj(obj1));
					}
				
				
					ColliderCast _cols2 = null;
					GameObject obj22 = PoolManager.GetObject("YohoNextHoldAttack", _cols.transform);
					obj22.transform.parent = null;
					if (obj22.TryGetComponent<ColliderCast>(out _cols2))
					{
						_cols2.Now(_cols.transform,(_life) =>
						{
							Vector3 dir = self.transform.position - _life.transform.position;
							dir.y = 0;
							dir.Normalize();

							_life.GetActor().move.forceDir = dir * 6 + new Vector3(0, 3, 0);
						},null, -1, -1, 0.2f, 0.3f);
					}

				});
			}
		
		
		
		
	}
	
	IEnumerator DeleteObj(GameObject obj, float t = 1.4f)
	{
		yield return new WaitForSeconds(t);
		PoolManager.ReturnObject(obj);
	}
	
	
	public override void OnAnimationEnd(Actor self, AnimationEvent evt)
	{
		
		//if (_cols.IsDamaged)
		//{
		//	Vector3 vec = _cols.transform.position;
		//	
		//	GameObject obj22 = PoolManager.GetObject("YohoNextEnd", _cols.transform);
		//
		//	if (obj22.TryGetComponent<ColliderCast>(out ColliderCast _cols2))
		//	{
		//		//_cols2.Now( );
		//	}
		//}
		
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
