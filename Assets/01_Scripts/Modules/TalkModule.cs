using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public enum DiaChangeMode
{
	Questing,
	Completed
}

public class TalkModule : Module, IInterable
{
    public Character charInfo;

	public Image qMark;

	public float yOffset = 1.5f;

	public const string QUESTCANVASNAME = "QuestMarkIndicator";


	private readonly int talkingHash = Animator.StringToHash("Talking");

	public string Name => charInfo.baseName;

	public bool IsInterable { get;set; }

	public bool AltInterable { get;set;} = false;

	public float InterTime { get; set; } = 0;
	public InterType interType { get; set; } = InterType.Talk;
	public AltInterType altInterType { get; set; }

	public UnityEvent onNextTalk;
	public UnityEvent onNextTalkChunkComplete;

	GameObject canv;

	protected virtual void Awake()
	{
		charInfo.self = GetActor();
		charInfo.latestQuest = null;
	}

	protected virtual void Start()
	{
		GameObject obj = PoolManager.GetObject(QUESTCANVASNAME, transform);
		obj.transform.localPosition = Vector3.up * yOffset * 1.5f;
		qMark = obj.transform.Find("QMark").GetComponent<Image>();

		canv = PoolManager.GetObject(FarmingPoint.CANVASNAME, transform);
		canv.transform.localPosition = Vector3.up * yOffset;
	}

	protected virtual void Update()
	{
		if (charInfo.Questing && charInfo.latestQuest == null) //받지않은 퀘스트 있음.
		{
			qMark.enabled = true;
			qMark.sprite = GameManager.instance.uiManager.questingMark;
		}
		else if (charInfo.latestQuest != null && charInfo.latestQuest.IsPendingCompletion) //퀘스트 완성 대기중
		{
			qMark.enabled = true;
			qMark.sprite = GameManager.instance.uiManager.questedMark;
		}
		else // ?
		{
			qMark.enabled = false;

		}

		if ((transform.position - GameManager.instance.player.transform.position).sqrMagnitude <= GameManager.instance.pActor.sight.GetSightRange() * GameManager.instance.pActor.sight.GetSightRange())
		{
			canv.SetActive(true);
		}
		else
		{
			canv.SetActive(false);
		}
	}

	public void AltInter()
	{
		
	}

	public void AltInterWith()
	{
		
	}

	public void GlowOff()
	{
		
	}

	public void GlowOn()
	{
		//사람이 빛나진 않지않을까..
	}

	public void Inter()
	{
		GameManager.instance.qManager.InvokeOnChanged(CompletionAct.InteractWith, self.actorName);
		charInfo.OnTalk();
		self.anim.Animators.SetBool(talkingHash, true);
	}

	public void InteractWith()
	{
		Inter();
	}

	public void SetDialogue(Dialogue dia)
	{
		charInfo.SetDialogue(dia);
	}

	public override void ResetStatus()
	{
		base.ResetStatus();
		charInfo.ResetDialogue();
	}
}
