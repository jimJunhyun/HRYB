using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecipeUI : MonoBehaviour
{
	public const float BGNDSCALE = 85;
	public const float IMGSCALE = 85;

    public Image itemImg;
	public TextMeshProUGUI itemName;

	public void SetInfo(Sprite img, string n)
	{
		if ((itemImg) == null)
		{
			GameObject g = new GameObject("ResItemBgnd");
			GameObject g1 = new GameObject("ResItemImg");
			Image bgndImg = g.AddComponent<Image>();
			bgndImg.rectTransform.sizeDelta = Vector2.one * BGNDSCALE;
			bgndImg.sprite = GameManager.instance.uiBase;
			bgndImg.rectTransform.localPosition = new Vector2(-65, 0);

			itemImg = g1.AddComponent<Image>();
			itemImg.rectTransform.sizeDelta = Vector2.one * IMGSCALE;
			itemImg.rectTransform.localPosition = new Vector2(-65, 0);

			g.transform.SetParent(transform);
			g1.transform.SetParent(g.transform);
			Debug.Log("GENERATEDNEWIMAGE");
		}
		if ((itemName) == null)
		{
			GameObject g = new GameObject("ResItemName");
			g.transform.SetParent(transform);
			itemName = g.AddComponent<TextMeshProUGUI>();
			itemName.font = GameManager.instance.tmpText;
			itemName.rectTransform.localPosition = new Vector2(77.5f, 17.5f);
			itemName.color = Color.black;
		}
		itemName.text = n;
		itemImg.sprite = img;
		Debug.Log("SETMYIMAGE");
	}
}
