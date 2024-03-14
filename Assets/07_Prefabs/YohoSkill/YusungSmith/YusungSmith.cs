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
    private int eventCount = 0;
    private int moveCount = 0;
    public override void OnAnimationStart(Actor self, AnimationEvent evt )
    {
	    // 이팩트 기타등등셋팅
	    GameManager.instance.DisableCtrl(ControlModuleMode.Animated);
	    eventCount = 0;
	    moveCount = 0;
    }
    public override void OnAnimationMove(Actor self, AnimationEvent evt)
    {
	    if (moveCount == 0)
	    {
		    // 이동
		    Vector3 dir = self.transform.forward;
		    self.move.forceDir = dir * 48 + new Vector3(0, 24, 0);
		    Debug.Log($"LOGG {dir}");
	    }
	    else if (moveCount == 1)
	    {

	    }

	    moveCount++;

    }

    
    public override void OnAnimationEvent(Actor self, AnimationEvent evt)
    {
	    Debug.Log(eventCount);
	    
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
	    eventCount++;
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
