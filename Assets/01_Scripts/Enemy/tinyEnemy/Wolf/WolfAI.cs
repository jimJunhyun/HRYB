using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WolfAI : AISetter
{

	[Header("공격 시작 범위")]
	[SerializeField] public float _attackRange = 2f;

	public float Attackrange()
	{
		return _attackRange;
	}
	
	private const string NormalAttack = "normallAtt";

	
	public void DieEvent()
	{
		self.anim.ResetStatus();
		StopExamine();
	}
	
    protected override void StartInvoke()
    {
	    self.life._dieEvent = DieEvent;


	    #region 평타

	    Waiter _normalAtt = new Waiter(3f);
	    IsInRange noramlRange = new IsInRange(self, player.transform, Attackrange, null, () =>
	    {
			// 공격 셋팅
	    });
	    Attacker normalAttack = new Attacker(self, () =>
	    {

	    });

	    Sequencer normalATK = new Sequencer();
	    
	    normalATK.connecteds.Add((noramlRange));
	    normalATK.connecteds.Add((_normalAtt));
		normalATK.connecteds.Add(normalAttack);
	    #endregion



    }
    

    protected override void UpdateInvoke()
    {
	    
    }
}
