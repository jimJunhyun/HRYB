using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Skills/Yoho/유성강타")]
public class YusungSmith : AttackBase
{
	internal override void MyOperation(Actor self)
    {

    }

    internal override void MyDisoperation(Actor self)
    {
    }

    public override void UpdateStatus()
    {
	    
    }

    ColliderCast _cols = null;
    public override void OnAnimationStart(Actor self, AnimationEvent evt )
    {
	    // 이팩트 기타등등셋팅
	    GameManager.instance.DisableCtrl(ControlModuleMode.Animated);
    }
    public override void OnAnimationMove(Actor self, AnimationEvent evt)
    {

		    Vector3 dir = self.transform.forward;
		    self.move.forceDir = dir * 48 + new Vector3(0, 24, 0);
		    Debug.Log($"LOGG {dir}");


    }

    
    public override void OnAnimationEvent(Actor self, AnimationEvent evt)
    {

	    if (evt.stringParameter == "ATK")
	    {
		    Vector3 dir = self.transform.forward;
		    self.move.forceDir = dir + new Vector3(0, -30, 0);
	    
		    if (_cols != null)
		    {
			    _cols.End();
			    _cols = null;
		    }
		    CameraManager.instance.ShakeCamFor(0.2f, 5, 5);
		    GameObject obj = PoolManager.GetObject("YusungSmith", self.transform);

		    if (obj.TryGetComponent<ColliderCast>(out _cols))
		    {
			    _cols.Now(self.transform, (_life) =>
			    {
				    DoDamage(_life.GetActor(), self);
				    
			    });
		    }
	    }
	    else if (evt.stringParameter == "EffectOne")
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
	    else if (evt.stringParameter == "EffectTwo")
	    {
		    GameObject obj = PoolManager.GetObject("YusungSmithEnd", self.transform);
		    self.StartCoroutine(DeleteObj(obj, 9));
		    obj.transform.parent = null;
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
		    _cols = null;
	    }
    }

    public override void OnAnimationStop(Actor self, AnimationEvent evt)
    {
	    
	    GameManager.instance.EnableCtrl(ControlModuleMode.Animated);
    }
}
