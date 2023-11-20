using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftUI : MonoBehaviour
{
	public const float SIZEDELTA = 75;

	Image itemImg;
	TextMeshProUGUI itemName;
	TextMeshProUGUI itemDesc;

	ScrollRect recipeList;

	List<CraftReqItemUI> crReq;


	Recipe recipe;
	ItemAmountPair objItem;
	

	private void Awake()
	{
		recipeList = GetComponentInChildren<ScrollRect>();
		
		itemImg = transform.GetChild(0).GetChild(0).Find("IconImg").GetComponent<Image>();
		itemName = transform.GetChild(0).Find("ResName").GetComponent<TextMeshProUGUI>();
		itemDesc = transform.GetChild(0).Find("ResDesc").GetComponent<TextMeshProUGUI>();
		crReq = new List<CraftReqItemUI>(GetComponentsInChildren<CraftReqItemUI>());
	}

	public void SetRecipe(KeyValuePair<Recipe, ItemAmountPair> r)
	{
		recipe = r.Key;
		objItem = r.Value;

		itemImg.sprite = r.Value.info.icon;
		itemName.text = r.Value.info.MyName;
		itemDesc.text = r.Value.info.desc;
		int idx = 0;
		foreach (var item in recipe.recipe)
		{
			if(crReq.Count <= idx)
			{
				GameObject g = new GameObject($"ItemAmount{crReq.Count +1}");
				g.AddComponent<RectTransform>().sizeDelta = Vector2.one * SIZEDELTA;
				CraftReqItemUI req = g.AddComponent<CraftReqItemUI>();
				crReq.Add(req);
				g.transform.SetParent(recipeList.content);
			}
			crReq[idx].gameObject.SetActive(true);
			crReq[idx].SetInfo(item.info.MyName, item.num, GameManager.instance.pinven.inven.SumContains(item.info));
			++idx;
		}

		for (int i = 0; i < crReq.Count; i++)
		{
			if(i >= recipe.recipe.Count)
			{
				crReq[i].gameObject.SetActive(false);
			}
		}
	}

	public void Craft()
	{
		GameManager.instance.craftManager.crafter.Craft(objItem);
		SetRecipe(new KeyValuePair<Recipe, ItemAmountPair>(recipe, objItem));
	}
}
