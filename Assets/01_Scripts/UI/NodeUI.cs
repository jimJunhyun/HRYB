using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NodeUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
	public PlayerNode indicating;

	Button button;
	Image nodeIcon;
	Image circleIndicator;
	Coroutine ongoing;

	internal bool Brushing
	{
		get => ongoing != null;
	}

	private void Start()
	{
		button = GetComponent<Button>();
		nodeIcon = transform.Find("NodeIcon").GetComponent<Image>();
		circleIndicator = transform.Find("CircleIndicator").GetComponent<Image>();
		if(indicating == null)
		{
			nodeIcon.enabled = false;
		}
		else if((GameManager.instance.uiManager.toolbarUIShower.openables[ToolState.Node] as NodeViewer).nodeSprites.Count > ((int)indicating.nodeType))
		{
			nodeIcon.enabled = true;
			nodeIcon.sprite = (GameManager.instance.uiManager.toolbarUIShower.openables[ToolState.Node] as NodeViewer).nodeSprites[((int)indicating.nodeType)];
		}
		circleIndicator.fillAmount = 0;
		button.onClick.AddListener(() => { (GameManager.instance.uiManager.toolbarUIShower.openables[ToolState.Node] as NodeViewer)?.SetSelected(this); });
		button.onClick.AddListener(() => { (GameManager.instance.uiManager.toolbarUIShower.openables[ToolState.Node] as NodeViewer)?.nodeLearner.OnOff(indicating);});
	}

	public void BrushStroke()
	{
		if(!Brushing)
		{
			circleIndicator.enabled = true;
			
			ongoing = StartCoroutine(DelStroke());
		}
	}

	public void OffBrush()
	{
		circleIndicator.enabled = false;
	}

	IEnumerator DelStroke()
	{
		float t = 0;
		circleIndicator.fillAmount = 0;
		while (t < NodeViewer.CIRCLESEC)
		{
			yield return null;
			t += Time.unscaledDeltaTime;
			circleIndicator.fillAmount = Mathf.Lerp(0, 1, t / NodeViewer.CIRCLESEC);
		}
		ongoing = null;
		
	}

	public void SetUpNodeUI(PlayerNode node) //안씀.
	{
		button = GetComponent<Button>();
		nodeIcon = GetComponent<Image>();
		indicating = node;
		switch (indicating.nodeType) //여기서 이미지를 정해주든 뭐든
		{
			case StatUpgradeType.White:
				//양
				break;
			case StatUpgradeType.Black:
				//음
				break;
			case StatUpgradeType.WhiteAtk:
				//양공격력
				break;
			case StatUpgradeType.BlackAtk:
				//음공격력
				break;
			case StatUpgradeType.MoveSpeed:
				//이속
				break;
			case StatUpgradeType.CooldownRdc:
				//쿨감?
				break;
			default:
				break;
		}

		
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		GameManager.instance.uiManager.detailer.ShowDetail(indicating, eventData.position);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		GameManager.instance.uiManager.detailer.OffDetail();
	}

	public void OnPointerMove(PointerEventData eventData)
	{
		GameManager.instance.uiManager.detailer.transform.position = eventData.position;
	}
}
