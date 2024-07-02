using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonAttackChickenAI : AISetter
{

	[Header("초기화 범위")][SerializeField] public float _section2Range = 15f;
	public float OutSectionRanged()
	{
		return _section2Range;
	}


	public override void DieEvent(float delay = 0, float time = 3)
	{
		//self.anim.ResetStatus();
		StopExamine();
		EscaperMove _moveModule = self.move as EscaperMove;
		GetComponent<BoxCollider>().enabled = false;
		_moveModule.StopMove();
		base.DieEvent(0.7f);
	}


	public override void StartInvoke()
	{
		head = new Selecter();

		EscaperMove _moveModule = self.move as EscaperMove;
		self.life._dieEvent += () => { DieEvent(); };


		IsInRange SectionRange = new IsInRange(self, player.transform, OutSectionRanged, null, () =>
		{
			_moveModule.SetTarget(player.transform);

		});
		Mover move = new Mover(self);

		Sequencer Moved = new Sequencer();
		Moved.connecteds.Add(SectionRange);
		Moved.connecteds.Add(move);

		IsOutRange LongaRange = new IsOutRange(self, player.transform, OutSectionRanged, null, () =>
		{
			_moveModule.StopMove();
		});

		Idler idles = new Idler(self);

		Sequencer Faridler = new Sequencer();
		Faridler.connecteds.Add(LongaRange);
		Faridler.connecteds.Add(idles);

		head.connecteds.Add(Moved);
		head.connecteds.Add(Faridler);

		StartExamine();
	}

	protected override void UpdateInvoke()
	{

	}
}
