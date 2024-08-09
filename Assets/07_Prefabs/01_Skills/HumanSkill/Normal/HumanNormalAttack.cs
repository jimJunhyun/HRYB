using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName ="Skills/Human/Normal")]
public class HumanNormalAttack : AttackBase
{


	int posValue2 = -1;
	List<YGArrow> _arrow = new();

	public override void UpdateStatus()
	{
		//throw new System.NotImplementedException();
	}

	internal override void MyDisoperation(Actor self)
	{
		//throw new System.NotImplementedException();
	}

	internal override void MyOperation(Actor self)
	{
		//throw new System.NotImplementedException();
	}

	public override void OnAnimationStart(Actor self, AnimationEvent evt)
	{
		GameManager.instance.DisableCtrl(false);
		_arrow.Clear();
		posValue2 = -1;
	}

	public override void OnAnimationEvent(Actor self, AnimationEvent evt)
	{

		

		string[] tt = evt.stringParameter.Split("$");
		switch (evt.intParameter)
		{
			case 0:
			case 1:
				{
					self.move.forceDir = new Vector3(0, 0, 0);
					GameObject obj = PoolManager.GetObject("HumanBullet", self.transform);
					if (obj.TryGetComponent<YGArrow>(out YGArrow _yg))
					{
						_yg.Ready(self, self.transform.position + self.transform.forward.normalized * 1 + new Vector3(0, 1.2f, 0),
						(_pos, enemy, time) =>
						{
							obj.transform.position = Vector3.Lerp(_pos, enemy, time / 0.3f);
							if(time/0.7f >= 1f)
							{
								PoolManager.ReturnObject(_yg.gameObject);
							}
						},
						(_life) =>
						{
							Debug.LogError($"{_life.name} 맞음");
							DoDamage(_life.GetActor(), self, _dmgs[0], obj.transform.position);
						},
						(_trm, _life) =>
						{
							// null
						});

						_yg.transform.parent = null;
						_yg.Fire();
					}
				}
				break;
			case 2:
				{
					self.move.forceDir = new Vector3(0, 0, 0);
					if (tt[0] == "Fire")
					{
						_arrow.ForEach((g) => { g.Fire(); });
						_arrow.Clear();
						self.move.forceDir = self.transform.forward * -7f;
					}
					else
					{
						GameObject obj = PoolManager.GetObject("HumanBullet", self.transform);
						if (obj.TryGetComponent<YGArrow>(out YGArrow _yg))
						{
							int f = posValue2 > 0 || posValue2 == -1 ? 0 : 1;

							_yg.Ready(self, self.transform.position + self.transform.forward.normalized * f + self.transform.right * posValue2 +new Vector3(0, 1.2f, 0),
							(_pos, enemy, time) =>
							{
								obj.transform.position = Vector3.Lerp(_pos, enemy, time / 0.3f);
								if (time / 0.7f >= 1f)
								{
									PoolManager.ReturnObject(_yg.gameObject);
								}
							},
							(_life) =>
							{
								DoDamage(_life.GetActor(), self, _dmgs[0], obj.transform.position);
							},
							(_trm, _life) =>
							{
								// null
							});
							_yg.transform.parent = null;

							posValue2++;
							_arrow.Add(_yg);
						}
					}
				}
				break;
			case 3:
				{
					GameObject obj = PoolManager.GetObject("HumanBullet", self.transform);
					if (obj.TryGetComponent<YGArrow>(out YGArrow _yg))
					{
						_yg.Ready(self, self.transform.position + self.transform.forward.normalized * 1 + new Vector3(0, 1.2f, 0),
						(_pos, enemy, time) =>
						{
							obj.transform.position = Vector3.Lerp(_pos, enemy, time / 0.3f);
							if (time / 0.7f >= 1f)
							{
								PoolManager.ReturnObject(_yg.gameObject);
							}
						},
						(_life) =>
						{
							DoDamage(_life.GetActor(), self, _dmgs[0], obj.transform.position);
							_life.GetActor().move.forceDir = (obj.transform.position - _life.transform.position).normalized * 5;
						},
						(_trm, _life) =>
						{
							// null
						});

						_yg.transform.parent = null;
						_yg.Fire();
					}
				}
				break;
		}
	}


	public override void OnAnimationMove(Actor self, AnimationEvent evt)
	{
		switch (evt.intParameter)
		{
			case 0:
			case 1:
				self.move.forceDir = self.transform.forward * 4f;
				break;
			case 3:
				self.move.forceDir = self.transform.forward * -8 + new Vector3(0, 2f, 0);
				break;
		}
	}

	public override void OnAnimationEnd(Actor self, AnimationEvent evt)
	{
		switch (evt.intParameter)
		{
			case 0:
			case 1:
				self.move.forceDir = new Vector3(0, 0, 0);
				break;
			case 3:
				self.move.forceDir = new Vector3(0, 0, 0);
				break;
		}
	}

	public override void OnAnimationStop(Actor self, AnimationEvent evt)
	{
		GameManager.instance.EnableCtrl();
	}

	public override void OnAnimationHit(Actor self, AnimationEvent evt)
	{
		_arrow.ForEach((g) => { g.Fire(); });
		_arrow.Clear();
	}

	public override int ListValue()
	{
		return 1;
	}
}
