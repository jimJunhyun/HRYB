using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public enum BtnState
{
	On,
	Off,
	Reset,
	Focused
}

public class ToolBtn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	private Image img;
	public Sprite BtnUI;
	public Sprite OnBtnUI;

	public bool isSubButton;

	const float DROPDOWNTIME = 0.5f;
	const float DROPDOWNLENGTH = 65;
	List<ToolBtn> subButtons = new List<ToolBtn>();
	HashSet<ToolState> subButtonIndicators = new HashSet<ToolState>();
	Image subButtonPanel;
	bool droppingdown = false;

	private TMP_Text text;

	public BtnState state;

	public ToolState indicating;

	Coroutine ongoing;

	private void Awake()
	{
		state = BtnState.Reset;
		img = GetComponent<Image>();
		text = GetComponentInChildren<TMP_Text>();
		subButtons = new List<ToolBtn>(GetComponentsInChildren<ToolBtn>(true));
		subButtons.Remove(this);
		for (int i = 0; i < subButtons.Count; i++)
		{
			subButtonIndicators.Add(subButtons[i].indicating);
		}
		subButtonPanel = transform.Find("SubPanel")?.GetComponent<Image>();
	}

	private void Start()
	{
		Darken();
		for (int i = 0; i < subButtons.Count; i++)
		{
			subButtons[i].gameObject.SetActive(false);
		}
		if (subButtonPanel)
		{
			subButtonPanel.enabled = false;
		}
	}

	public void Enter()
	{
		if(state != BtnState.Focused)
		{
			state = BtnState.On;
			Lighten();
		}
	}

	public void Exit()
	{
		if(state != BtnState.Focused)
		{
			state = BtnState.Off;
			Darken();
		}
	}

	public void Focus()
	{
		state = BtnState.Focused;
		Lighten();
	}

	public void Click()
	{
		if(indicating == ToolState.None && subButtons.Count > 0)
		{
			subButtons[0].Click();
		}
		else
		{
			GameManager.instance.uiManager.toolbarUIShower.ChangeStatus(indicating);
		}
	}

	private void Update()
	{
		if (state == BtnState.Reset) //이건 어디서 되는지를 모르겠어서 일단 여기서 부름.
		{
			ResetButton();
		}
	}

	public void ParentButtonRefresh()
	{
		for (int i = 0; i < subButtons.Count; i++)
		{
			if (subButtons[i].state == BtnState.Focused)
			{
				Focus();
			}
		}
	}

	public void ResetButton()
	{
		state = BtnState.Off;
		Darken();
		if (subButtons != null && subButtons.Count > 0)
		{
			for (int i = 0; i < subButtons.Count; i++)
			{
				subButtons[i].ResetButton();
				if(subButtons[i].state == BtnState.Focused)
				{
					
				}
			}
			if (droppingdown)
			{
				HideDropDown();
			}
		}
	}

	public void Lighten()
	{
		img.sprite = OnBtnUI;
		text.color = Color.black;
	}

	public void Darken()
	{
		img.sprite = BtnUI;
		text.color = Color.white;
	}

	public void ShowDropDown()
	{
		Focus();

		droppingdown = true;
		if (subButtonPanel)
		{
			subButtonPanel.enabled = true;
		}
		if (ongoing != null)
			StopCoroutine(ongoing);
		ongoing = StartCoroutine(DelDropdownCtrl(true));
		
	}

	public void HideDropDown()
	{
		droppingdown = false;
		if (subButtonPanel)
		{
			subButtonPanel.enabled = false;
		}
		if (ongoing != null)
			StopCoroutine(ongoing);
		ongoing = StartCoroutine(DelDropdownCtrl(false));
	}

	IEnumerator DelDropdownCtrl(bool isShow)
	{
		float t = 0;
		float sampleCurve = 0;
		
		float maxCurve = GameManager.instance.uiManager.dropDownCurve.Evaluate(1);
		if (isShow)
		{
			for (int i = 0; i < subButtons.Count; i++)
			{
				subButtons[i].gameObject.SetActive(true);
			}
		}
		while(t < DROPDOWNTIME)
		{
			sampleCurve = GameManager.instance.uiManager.dropDownCurve.Evaluate(t / DROPDOWNTIME);
			for (int i = 0; i < subButtons.Count; i++)
			{
				if (isShow)
				{
					subButtons[i].transform.localPosition = Vector3.down * sampleCurve * (i + 1) * DROPDOWNLENGTH;
				}
				else
				{
					subButtons[i].transform.localPosition = (Vector3.down * maxCurve * (i + 1) * DROPDOWNLENGTH) + (Vector3.up * sampleCurve * (i + 1) * DROPDOWNLENGTH);
				}
			}
			t += Time.unscaledDeltaTime;
			yield return null;
		}
		for (int i = 0; i < subButtons.Count; i++)
		{
			if (isShow)
			{
				subButtons[i].transform.localPosition = Vector3.down * maxCurve * (i + 1) * DROPDOWNLENGTH;
			}
			else
			{
				subButtons[i].transform.localPosition = Vector3.zero;
				subButtons[i].gameObject.SetActive(false);
			}
		}

	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		Enter();
		if (subButtons != null && subButtons.Count > 0 && !droppingdown)
		{
			ShowDropDown();
		}
		Debug.Log("!@!@");
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		Exit();
		if (subButtons != null && subButtons.Count > 0)
		{
			if (!subButtonIndicators.Contains(GameManager.instance.uiManager.toolbarUIShower.state))
			{
				ResetButton();
			}
			else
			{
				HideDropDown();
			}
		}
	}
}
