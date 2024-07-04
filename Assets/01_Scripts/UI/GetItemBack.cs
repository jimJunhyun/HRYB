using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class GetItemBack : MonoBehaviour
{
	private int idx = 0;

	public Item item;
	public int count;

	Image img;
	TextMeshProUGUI text;


	private void OnEnable()
	{
		CancelInvoke();
		StartCoroutine(DelDestroy());
	}
 
	public void SetInfo(Item i, int cnt)
	{
		idx = 0;

		item = i;
		count = cnt;

		if (img == null)
		{
			img = transform.Find("Image").GetComponent<Image>();
		}
		if(text == null)
		{
			text = transform.Find("Name").GetComponent<TextMeshProUGUI>();
		}

		img.sprite = i.icon;
		text.text = $"{i.MyName} x{cnt}";

		GameManager.instance.uiManager.RefreshGetItemQ(this);
	}

	public void Move()
	{
		if(idx + 1 >= GameManager.instance.uiManager.getItemUiSlot.Count)
		{
			PoolManager.ReturnObject(GameManager.instance.uiManager.getItemList[0].gameObject);
			GameManager.instance.uiManager.getItemList.RemoveAt(0);
			return;
		}

		Vector3 currentPos = transform.localPosition;
		Vector3 targetPos = GameManager.instance.uiManager.getItemUiSlot[++idx].localPosition;

		StartCoroutine(AnimatedMove(currentPos, targetPos, 0.5f));
	}

	private IEnumerator AnimatedMove(Vector3 current, Vector3 target, float time)
	{
		float t = 0.0f;
		while (t <= time)
		{
			t += Time.unscaledDeltaTime;

			transform.localPosition = Vector3.Lerp(current, target, t / time);

			yield return null;
		}
	}

	private IEnumerator DelDestroy()
	{
		yield return new WaitForSecondsRealtime(3);
		GameManager.instance.uiManager.getItemList.Remove(this);
		PoolManager.ReturnObject(gameObject);
	}

}
