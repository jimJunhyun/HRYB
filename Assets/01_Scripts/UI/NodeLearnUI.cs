using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NodeLearnUI : MonoBehaviour
{
	PlayerNode showing;
	internal bool isOn = false;
	Transform scroller;
	Coroutine ongoing;
	Button learnBtn;

	TextMeshProUGUI title;
	NeededResource req;

	Vector3 offPos;
	Vector3 onPos;

	private void Awake()
	{
		scroller = transform.Find("Scroller");
		title = scroller.Find("Names/NodeName").GetComponent<TextMeshProUGUI>();
		req = scroller.Find("Requires/NeededResource").GetComponent<NeededResource>();
		learnBtn = scroller.Find("LearnButton").GetComponent<Button>();

		offPos = scroller.position;
		onPos = offPos + Vector3.down * Screen.height;

		isOn = false;
	}

	private void Start()
	{
		gameObject.SetActive(false);
	}

	public void On(PlayerNode node)
	{
		if(ongoing != null)
		{
			StopCoroutine(ongoing);
		}
		gameObject.SetActive(true);
		showing = node;
		ongoing = StartCoroutine(DelScroll(true));

		RefreshInfo();
	}

	public void Off()
	{
		if(ongoing != null)
			StopCoroutine(ongoing);
		showing = null;
		ongoing = GameManager.instance.StartCoroutine(DelScroll(false));

	}

	public void ImmediateOff()
	{
		if (ongoing != null)
			StopCoroutine(ongoing);
		showing = null;
		ongoing = null;

		scroller.position = offPos;
		gameObject.SetActive(false);

		isOn = false;
	}

	public void Learn()
	{
		if (isOn && showing != null)
		{
			showing.LearnNode();
			RefreshInfo();
		}
	}

	public void RefreshInfo()
	{
		if(!showing)
			return;
		req.SetInfo(((int)showing.needPoint));
		title.text = NodeUtility.GetName(showing);
		learnBtn.interactable = showing.ExamineLearnable();
	}

	public IEnumerator DelScroll(bool direction)
	{
		float t = 0;
		while(t < NodeViewer.MOVESEC)
		{
			yield return null;
			t += Time.unscaledDeltaTime;
			scroller.transform.position = Vector3.Lerp(onPos, offPos, (direction ? 1 - t / NodeViewer.MOVESEC : t / NodeViewer.MOVESEC));
		}
		scroller.position = (direction ? onPos : offPos);

		isOn = direction;
		ongoing = null;
		gameObject.SetActive(isOn);
	}

	public void OnOff(PlayerNode node)
	{
		Debug.Log("창키고끄는중.....");
		if (!isOn)
		{
			(GameManager.instance.uiManager.toolbarUIShower.openables[ToolState.Node] as NodeViewer)?.ShowLearner(node);	
		}
		else
		{
			if(showing == node)
			{
				(GameManager.instance.uiManager.toolbarUIShower.openables[ToolState.Node] as NodeViewer)?.UnshowLearner();
			}
			else
			{
				showing = node;
				RefreshInfo();
				Debug.Log("이미켜져있다");
			}
		}
	}
}
