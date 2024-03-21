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
	    GameManager.instance.DisableCtrl(ControlModuleMode.Animated);
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
				    self.move.forceDir += self.transform.forward * 2 + new Vector3(0, 20, 0);

				    
				    GameObject obj = PoolManager.GetObject("YohoNormalAttack", self.transform);
				    if (obj.TryGetComponent<ColliderCast>(out _cols))
				    {
					    _cols.Now(self.transform, (_life) =>
					    {
						    CameraManager.instance.ShakeCamFor(0.08f, 2, 2);
						    _life.GetActor().move.forceDir += self.transform.forward * 2 + new Vector3(0, 20, 0);
						    
						    Actor to = _life.GetActor();
						    Actor by = self;
						    to.life.DamageYY(by.atk.Damage * damageMult * 0.4f, DamageType.DirectHit, 0, 0, by);
						    Debug.Log($"[데미지] {to.gameObject.name} 에게 데미지 : {by.atk.Damage} * {damageMult} * 0.4f = {(by.atk.Damage * damageMult* 0.4f)}");
						    for (int i = 0; i < statEff.Count; i++)
						    {
							    StatusEffects.ApplyStat(to, by, statEff[i].id, statEff[i].duration, statEff[i].power);
			
						    }
						    
					    });
				    }
			    }
			    break;
		    case "2":
			    {
				    GameObject obj = PoolManager.GetObject("YohoNormalAttack", self.transform);
				    self.move.forceDir += new Vector3(0, 5, 0);
				    if (obj.TryGetComponent<ColliderCast>(out _cols))
				    {
					    _cols.Now(self.transform, (_life) =>
					    {
						    CameraManager.instance.ShakeCamFor(0.08f, 2, 2);
						    
						    
						    Actor to = _life.GetActor();
						    Actor by = self;
						    to.life.DamageYY(by.atk.Damage * damageMult * 0.8f, DamageType.DirectHit, 0, 0, by);
						    Debug.Log($"[데미지] {to.gameObject.name} 에게 데미지 : {by.atk.Damage} * {damageMult} * 0.8f = {(by.atk.Damage * damageMult *  0.8f)}");
						    for (int i = 0; i < statEff.Count; i++)
						    {
							    StatusEffects.ApplyStat(to, by, statEff[i].id, statEff[i].duration, statEff[i].power);
			
						    }
					    });
				    }
			    }
			    break;
		    case "3":
			    {
				    GameObject obj = PoolManager.GetObject("YohoNormalAttack", self.transform);
				    if (obj.TryGetComponent<ColliderCast>(out _cols))
				    {
					    _cols.Now(self.transform, (_life) =>
					    {
						    CameraManager.instance.ShakeCamFor(0.08f, 2, 2);
						    
						    
						    Actor to = _life.GetActor();
						    Actor by = self;
						    to.life.DamageYY(by.atk.Damage * damageMult * 1.5f, DamageType.DirectHit, 0, 0, by);
						    Debug.Log($"[데미지] {to.gameObject.name} 에게 데미지 : {by.atk.Damage} * {damageMult}  * 1.5f= {(by.atk.Damage * damageMult * 1.5f)}");
						    for (int i = 0; i < statEff.Count; i++)
						    {
							    StatusEffects.ApplyStat(to, by, statEff[i].id, statEff[i].duration, statEff[i].power);
			
						    }
					    });
				    }
			    }
			    break;
	    }
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
