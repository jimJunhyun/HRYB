using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Skills/Yoho/유성강타")]
public class YusungSmith : AttackBase
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

    public override void OnAnimationStart(Actor self, AnimationEvent evt )
    {
	    // 이팩트 기타등등셋팅
	    GameManager.instance.DisableCtrl();
    }
    public override void OnAnimationMove(Actor self, AnimationEvent evt)
    {
	    string[] tt = evt.stringParameter.Split("$");
	    if (tt[0] == "1")
	    {
		    Vector3 dir = self.transform.forward;
		    self.move.forceDir = dir * 8 + new Vector3(0, 12, 0);
		    Debug.Log($"LOGG {dir}");
	    }
	    else if (tt[0] == "2")
	    {
		    Vector3 dir = self.transform.forward;
		    self.move.forceDir += dir * 12 + new Vector3(0, 3, 0);
		    Debug.Log($"LOGG {dir}");
	    }
	    else if (tt[0] == "3")
	    {
		    Vector3 dir = self.transform.forward;
		    self.move.forceDir = dir + new Vector3(0, -80, 0);
	    }



    }

    
    public override void OnAnimationEvent(Actor self, AnimationEvent evt)
    {
	    string[] tt = evt.stringParameter.Split("$");
	    if (tt[0] == "ATK")
	    {

	    
		    if (_cols != null)
		    {
			    _cols.End();
			    _cols = null;
		    }
		    GameObject obj = PoolManager.GetObject("YusungSmith", self.transform);

		    if (obj.TryGetComponent<ColliderCast>(out _cols))
		    {
			    _cols.Now(self.transform, (_life) =>
			    {
				    DoDamage(_life.GetActor(), self, obj.transform.position);
				    Vector3 dir = _life.transform.position-self.transform.position;
				    dir.y = 0;
				    dir.Normalize();
					
				    _life.GetActor().move.forceDir += dir*6 + new Vector3(0, 6, 0);
				    

			    }, (me, enemy) =>
			    {
				    RaycastHit ray;
				    CameraManager.instance.ShakeCamFor(0.2f, 18, 18);
				    if (Physics.Raycast(self.transform.position, Vector3.down, out ray, 100, 1 << 11))
				    {
					    GameObject obj = PoolManager.GetObject("YusungSmithEnd", self.transform);
					    self.StartCoroutine(DeleteObj(obj, 9));
					    obj.transform.parent = null;
					    obj.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
					    obj.transform.position = ray.point;
					    GameManager.instance.TimeFreeze(0.3f, 0.08f);
				    }
				    

			    });
		    }
	    }
	    else if (tt[0] == "EffectOne")
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

    }

    public override void OnAnimationStop(Actor self, AnimationEvent evt)
    {
	    if (_cols != null)
	    {
		    _cols.End();
		    _cols = null;
	    }	    
	    GameManager.instance.EnableCtrl();
    }
}
