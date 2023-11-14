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

	private void Awake()
	{
		if( (itemImg = transform.Find("ResItemImg").GetComponent<Image>()) == null)
		{
			GameObject g = new GameObject("ResItemBgnd");
			GameObject g1 = new GameObject("ResItemImg");
			Image img = g.AddComponent<Image>();
			img.rectTransform.sizeDelta = Vector2.one * BGNDSCALE;
			img.sprite = GameManager.instance.uiBase;
			
			itemImg = g1.AddComponent<Image>();
			itemImg.rectTransform.sizeDelta = Vector2.one * IMGSCALE;

			g.transform.SetParent(transform);
			g1.transform.SetParent(g.transform);
		}
		if((itemImg = transform.Find("ResItemName").GetComponent<Image>()) == null)
		{
			GameObject g = new GameObject("ResItemName");
			g.transform.SetParent(transform);
			itemName = g.AddComponent<TextMeshProUGUI>();
		}
	}

	public void SetInfo(Sprite img, string n)
	{
		itemImg.sprite = img;
		itemName.text = n;
	}
}
