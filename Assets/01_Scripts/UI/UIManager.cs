using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
//using GSpawn;
using System.Linq;









/// <summary>
/// 끄고킬수있는 UI의 인터페이스 등 구조를 만들자.
/// </summary>

 public class UIManager : MonoBehaviour
{
	Canvas canvas;
	Canvas comboCanv;

	public YYCtrl yinYangUI;
	public AimPointCtrl aimUI;
	//public InfoCtrl infoUI;
	//public SimpleCrafter crafterUI;
	public QuestUI questUI;
	public InterPrevUI preInterUI;
	public InterProcessUI interingUI;
    public GameObject invenPanel;
    public GameObject optionPanel;
	public DialogueUI dialogueUI;

	public NodeDetailUI detailer;
	

	public GameObject basicUIGroup;

	public MedicineButtonsUI medicineButton;
	public MedicineDetailUI medicineDetail;

	public CollectionButtonUI collectionButton;
	public YinyangItemDetailUI yinyangitemDetail;

	public MedicineTutorialManager mediTutorial;

	public ToolBarManager toolbarUIShower;

	public FocusUI focus;

	public AnimationCurve dropDownCurve;

	public RectTransform CursorPos;
    public Image Cursor;

	public TMPro.TextMeshProUGUI debugText;

	public Transform GetitemUITransform;

    public int DragPoint;
    public int DropPoint;

	public List<GetItemBack> getItemList = new List<GetItemBack>();

	public Transform getUIDefault;

	public Sprite questingMark;
	public Sprite questedMark;
	float getUITime = 0.3f;

	public bool isWholeScreenUIOn
	{
		get=>isOn || isOptionOn;
	}

    bool isOn = false;
    bool isOptionOn = false;

	bool tutorialAppended = false;

	List<SlotUI> uis = new List<SlotUI>();
	List<QuickSlot> quickSlot = new List<QuickSlot>();

	public List<RectTransform> getItemUiSlot = new List<RectTransform>();

	private void Awake()
	{
		getUIDefault = GameObject.Find("GetItemTransform").transform;
		GetitemUITransform = GameObject.Find("GetItemGroup").transform;
		canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
		comboCanv = GameObject.Find("UICombo").GetComponent<Canvas>();
		invenPanel = canvas.transform.Find("ToolPanel").gameObject;
		optionPanel = canvas.transform.Find("OptionUI").gameObject;
		yinYangUI = canvas.GetComponentInChildren<YYCtrl>();
		aimUI = canvas.GetComponentInChildren<AimPointCtrl>();
		//infoUI = canvas.GetComponentInChildren<InfoCtrl>();
		//crafterUI = canvas.GetComponentInChildren<SimpleCrafter>();
		questUI = canvas.GetComponentInChildren<QuestUI>();
		preInterUI = canvas.GetComponentInChildren<InterPrevUI>();
		interingUI = canvas.GetComponentInChildren<InterProcessUI>();
		basicUIGroup = canvas.transform.Find("Group").gameObject;
		detailer = canvas.transform.Find("ToolPanel/Node/NodeDetail").GetComponent<NodeDetailUI>();
		dialogueUI = canvas.transform.Find("DialogueUI/Dialogue").GetComponent<DialogueUI>();
		uis.AddRange(GameObject.Find("Canvas/ToolPanel/Inventory").GetComponentsInChildren<SlotUI>());

		medicineButton = canvas.transform.Find("ToolPanel/Medicine/DrugStore").GetComponent<MedicineButtonsUI>();
		medicineDetail = canvas.transform.Find("ToolPanel/Fusion/MedicineDetail").GetComponent<MedicineDetailUI>();

		yinyangitemDetail = canvas.transform.Find("ToolPanel/Collection").GetComponent<YinyangItemDetailUI>();

		toolbarUIShower = GameObject.Find("ToolPanel").GetComponent<ToolBarManager>();

		invenPanel.SetActive(true);
		optionPanel.SetActive(false);

		getItemUiSlot = GetitemUITransform.GetComponentsInChildren<RectTransform>().ToList();
		getItemUiSlot.RemoveAt(0); //GetitemUITransform 제거

		focus = GameObject.Find("FocusCanv").GetComponent<FocusUI>();
		mediTutorial = GameObject.Find("MedicineTutorialManager").GetComponent<MedicineTutorialManager>();
	}

	private void Start()
	{
		
		quickSlot.AddRange(GameObject.FindObjectsOfType<QuickSlot>());
		
		interingUI.SetGaugeValue(0);
		interingUI.Off();
		preInterUI.Off();
		//infoUI.Off();
		//crafterUI.On();

		OffInven();

		
	}

	public void OnInventory(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			if (!isOn)
			{
				OnInven();
				if (tutorialAppended)
				{
					mediTutorial.StartTutorial();
				}
			}
			else
			{
				OffInven();
			}
		}
		
	}

	public void OnHelp(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			OnOffOption();
		}

	}

	public void UpdateInvenUI()
	{
		for (int i = 0; i < uis.Count; i++)
		{
			uis[i].UpdateItem();
		}
		toolbarUIShower.RefreshWindows();
		medicineButton.SetStatuses(GameManager.instance.pinven.CurHoldingItem.info);
		medicineDetail.RefreshInfo();
	}

	public RectTransform GetInvenSlotUIRect(int idx)
	{
		Debug.Log("NAME : " + uis[idx].transform.parent.name);
		return (uis[idx].transform.parent.Find("Frame") as RectTransform);
	}

	public void UpdateQuestUI()
	{
		questUI.Refresh();
	}

	public void OnInven()
	{
		invenPanel.SetActive(true);
		isOn = true;
		GameManager.instance.UnLockCursor();
		Time.timeScale = 0;
	}

	public void OffInven()
	{
		invenPanel.SetActive(false);
		toolbarUIShower.ChangeStatus(ToolState.Inventory);
		isOn = false;
		GameManager.instance.LockCursor();
		Time.timeScale = 1;
	}

	public void OffOption()
	{
		if (isOptionOn)
		{
			isOptionOn = false;
			optionPanel.SetActive(false);
			GameManager.instance.LockCursor();
		}		
	}

	public void OnOption()
	{
		if (!isOptionOn)
		{
			optionPanel.SetActive(true);
			GameManager.instance.UnLockCursor();
			isOptionOn = true;
		}
	}

	public void OnOffOption()
	{
		if (isOptionOn)
		{
			OffOption();
		}
		else
		{
			OnOption();
		}
	}


	public void OffCanvas()
	{
		canvas.gameObject.SetActive(false);
		comboCanv.gameObject.SetActive(false);
	}

	public void OnCanvas()
	{
		canvas.gameObject.SetActive(true);
		comboCanv.gameObject.SetActive(true);
	}

	public void AppendTutorial()
	{
		tutorialAppended = true;
	}


	int ListCnt = 3;


	public void RefreshGetItemQ(GetItemBack obj)
	{
		//TO DO by 정유철
		/*
		 * 1. 아이템을 먹으면 UI를 소환!
		 * 2. 받은 아이템 정보를 표기해주고 3초뒤에 사라지게 한다.
		 * 3. 만약에 새로운 아이템을 획득하여 또 UI가 소환된다면 전의 UI는 한칸 내린다.
		 * 4. 또 만약에 위에 3개가 있다면 즉시 사라지게 한다.
		*/


		for (int i = getItemList.Count-1; i >= 0; i--) // 뒤로 쌓임을 이용
		{
			getItemList[i].Move();
		}

		getItemList.Add(obj);

	}
}
