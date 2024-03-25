using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Skills/Yoho/하늘 가르기")]
public class SkyBirtgh : AttackBase
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
	    
    }


    public override void OnAnimationEvent(Actor self, AnimationEvent evt)
    {
	    string[] tt = evt.stringParameter.Split("$");
	    
	    
	    if (_cols != null)
	    {
		    _cols.End();
		    _cols = null;
	    }
	    
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
				    
				    Vector3 dir = self.transform.forward;
				    self.move.forceDir = dir + new Vector3(0, 20, 0);

				    Debug.LogError("스카이브릿지");
				    GameObject obj = PoolManager.GetObject("SkyBritghCollider", self.transform);
				    if (obj.TryGetComponent<ColliderCast>(out _cols))
				    {
					    _cols.Now(self.transform, (_life) =>
					    {
						    CameraManager.instance.ShakeCamFor(0.08f, 2, 2);
						    _life.GetActor().move.forceDir = self.transform.forward * 3 + new Vector3(0, 12, 0);
						    
						    Actor to = _life.GetActor();
						    Actor by = self;
						    DoDamage(to,by, 0.1f);
						    
					    });
				    }
			    }
			    break;
		    case "2":
			    {
				    
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
				    
				    GameObject obj = PoolManager.GetObject("SkyBritghCollider", self.transform);
				    self.move.forceDir += new Vector3(0, 25, 0);
				    if (obj.TryGetComponent<ColliderCast>(out _cols))
				    {
					    _cols.Now(self.transform, (_life) =>
					    {
						    CameraManager.instance.ShakeCamFor(0.08f, 2, 2);
						    
						    
						    Actor to = _life.GetActor();
						    Actor by = self;
						    DoDamage(to,by, 0.4f);
					    });
				    }
			    }
			    break;
		    case "3":
			    {
				    
				    
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
				    
				    GameObject obj = PoolManager.GetObject("SkyBritghCollider", self.transform);
				    if (obj.TryGetComponent<ColliderCast>(out _cols))
				    {
					    _cols.Now(self.transform, (_life) =>
					    {
						    CameraManager.instance.ShakeCamFor(0.08f, 2, 2);

						    _life.GetActor().move.forceDir = self.transform.forward * 18 + new Vector3(0, -24, 0);
						    Actor to = _life.GetActor();
						    Actor by = self;
						    DoDamage(to,by, 0.8f);
					    });
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
	    self.move.moveDir = Vector3.zero;
	    if (_cols != null)
	    {
		    _cols.End();
		    //_cols = null;
	    }
    }
	
	
    public override void OnAnimationStop(Actor self, AnimationEvent evt)
    {
	    GameManager.instance.EnableCtrl();
    }
}
