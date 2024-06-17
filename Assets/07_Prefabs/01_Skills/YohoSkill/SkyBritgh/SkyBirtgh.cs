using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;


[CreateAssetMenu(menuName = "Skills/Yoho/하늘 가르기")]
public class SkyBirtgh : AttackBase
{
	ColliderCast _cols = null;
	[Description("3타짜리 스킬")]
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
	    GameManager.instance.DisableCtrl(true);
		(self.anim as PlayerAnim).AnimAct.PlayerAfterImage(0.2f, 1.4f, 0.66f);
	}

    public override void OnAnimationMove(Actor self, AnimationEvent evt)
    {
	    
    }


    public override void OnAnimationEvent(Actor self, AnimationEvent evt)
    {
	    string[] tt = evt.stringParameter.Split("$");
	    
	    switch (tt[0])
	    {
		    case "1":
			    {
				    //GameManager.instance.audioPlayer.PlayPoint("Craw", self.transform.position);
				    
				    GameObject obj1 = PoolManager.GetObject("YusungSmithright", self.transform);
				    if (obj1.TryGetComponent<EffectObject>(out EffectObject eff1))
				    {
					    eff1.Begin();
					    self.StartCoroutine(DeleteObj(obj1));
				    }


				    //Debug.LogError("스카이브릿지");
				    GameObject obj = PoolManager.GetObject("SkyBritghCollider", self.transform);
				    if (obj.TryGetComponent<ColliderCast>(out _cols))
				    {
					    _cols.Now(self.transform, (_life) =>
					    {

						    _life.GetActor().move.forceDir = self.transform.forward * 2 + new Vector3(0, 2.8f, 0);
						    //GameManager.instance.TimeFreeze(0, 0.1f);
						    Actor to = _life.GetActor();
						    Actor by = self;

							Debug.Log($"AAA : {obj} | {to} | {by}");
						    DoDamage(to,by, _dmgs[0], obj.transform.position);
						    
					    }, (transform, module) =>
					    {
						    CameraManager.instance.ShakeCamFor(0.18f, 12, 12);
						    GameManager.instance.TimeFreeze(0.3f, 0.08f);
					    }, default, default ,0.1f);
				    }

					GameManager.instance.audioPlayer.PlayPoint("HitSound", self.transform.position);
					Vector3 dir = self.transform.forward;
					self.move.forceDir = dir + new Vector3(0, 5, 0);
				}
			    break;
		    case "2":
			    {
					GameObject obj = null;

						obj = PoolManager.GetObject("SkyBritghCollider", self.transform);
					if (obj.TryGetComponent<ColliderCast>(out _cols))
				    {
					    _cols.Now(self.transform, (_life) =>
					    {
						    _life.GetActor().move.forceDir += new Vector3(0, 0.7f, 0);   
						    
						    Actor to = _life.GetActor();
						    Actor by = self;
						    DoDamage(to,by, _dmgs[1], obj.transform.position);
					    }, (transform, module) =>
					    {
							self.move.forceDir += new Vector3(0, 1f, 0);
							CameraManager.instance.ShakeCamFor(0.1f, 2, 2);
							GameManager.instance.TimeFreeze(0.1f, 0.01f);
							GameObject obj1 = PoolManager.GetObject("SlashMiddle", self.transform);
							if (obj1.TryGetComponent<EffectObject>(out EffectObject eff1))
						    {
							    eff1.Begin();
							    self.StartCoroutine(DeleteObj(obj1));
						    }
							//
							GameManager.instance.audioPlayer.PlayPoint("HitSound", self.transform.position);
					    }, default, default, 0.1f);
				    }
				}
			    break;
		    case "3":
			    {
				    
				    //GameManager.instance.audioPlayer.PlayPoint("CrawLow", self.transform.position);
				    GameObject obj1 = PoolManager.GetObject("YusungSmithleft", self.transform);
				    if (obj1.TryGetComponent<EffectObject>(out EffectObject eff1))
				    {
					    eff1.Begin();
					    self.StartCoroutine(DeleteObj(obj1));
				    }

				    GameObject obj2 = PoolManager.GetObject("YusungSmithright", self.transform);
				    if (obj2.TryGetComponent<EffectObject>(out EffectObject eff2))
				    {
					    eff2.Begin();
					    self.StartCoroutine(DeleteObj(obj2));
				    }
				    GameManager.instance.audioPlayer.PlayPoint("HitSound", self.transform.position);
				    GameObject obj = PoolManager.GetObject("SkyBritghCollider", self.transform);
				    if (obj.TryGetComponent<ColliderCast>(out _cols))
				    {
						Debug.LogError("스카이 브릿지 3");
					    _cols.Now(self.transform, (_life) =>
					    {
						    //CameraManager.instance.ShakeCamFor(0.08f, 2, 2);
						    //GameManager.instance.TimeFreeze(0.1f, 0.1f);
						    _life.GetActor().move.forceDir = self.transform.forward * 18 + new Vector3(0, -24, 0);
						    Actor to = _life.GetActor();
						    Actor by = self;
						    DoDamage(to,by, _dmgs[2], obj.transform.position);
						    CameraManager.instance.ShakeCamFor(0.18f, 12, 12);
						    //GameManager.instance.TimeFreeze(0.3f, 0.08f);
					    }, default, default, 0.1f);
				    }
			    }
			    break;
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
    }
	
    public override void OnAnimationStop(Actor self, AnimationEvent evt)
    {
	    GameManager.instance.EnableCtrl();
    }

	public override int ListValue()
	{
		return 3;
	}
}
