using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class SimpleCrafter : MonoBehaviour
{
	public Dictionary<string, List<KeyValuePair<Recipe, ItemAmountPair>>> categoryRecipesPair = new Dictionary<string, List<KeyValuePair<Recipe, ItemAmountPair>>>();
	List<KeyValuePair<Recipe, ItemAmountPair>> catgRecipes;
	public Recipe curRecipe;
	public int curShowingRecipe;

	public CategoryImages categoryImgPair;

	public Transform categories;

	public Image categoryIconDisplay;
	public TextMeshProUGUI categoryNameDisplay;
	public List<Image> categoryImages = new List<Image>();

	public Transform recipes;
	public List<RecipeUI> allRecipeImg = new List<RecipeUI>();

	public CraftUI craft;


	int categoryId = 0;
	int recipeId = 0;

	private void Awake()
	{
		categories = transform.Find("Categories");

		recipes = transform.Find("Recipes");
		craft = transform.Find("Craft").GetComponent<CraftUI>();

		categoryNameDisplay = GameObject.Find("CatgName").GetComponent<TextMeshProUGUI>();
		categoryIconDisplay = GameObject.Find("CatgImg").GetComponent<Image>();

	}

	private void Start()
	{
		foreach (var item in Crafter.recipeItemTable.Keys)
		{
			if(((Recipe)item).category != ""){
				if (!categoryRecipesPair.ContainsKey(((Recipe)item).category))
				{
					categoryRecipesPair[((Recipe)item).category] = new List<KeyValuePair<Recipe, ItemAmountPair>>();
					GameObject g = new GameObject($"Category{++categoryId}");
					Image i = g.AddComponent<Image>();
					Button b = g.AddComponent<Button>();
					i.sprite = categoryImgPair.info[((Recipe)item).category];
					b.targetGraphic = i;
					string catName = ((Recipe)item).category;
					b.onClick.AddListener(() => { CategoryOn(catName); });
					g.transform.SetParent(categories);
					categoryImages.Add(i);

				}
				categoryRecipesPair[((Recipe)item).category].Add(new KeyValuePair<Recipe, ItemAmountPair>((Recipe)item, (ItemAmountPair)Crafter.recipeItemTable[item]));
			}
			
		}

		On();
	}

	public void On()
	{
		gameObject.SetActive(true);
		CategoryOn("도구");
		CraftOff();
		Debug.Log("CROn");
	}

	public void Off()
	{
		gameObject.SetActive(false);
		Debug.Log("CROFf");
	}

	public void CategoryOn(string name)
	{
		categories.gameObject.SetActive(true);
		catgRecipes = categoryRecipesPair[name];
		categoryIconDisplay.sprite = categoryImgPair.info[name];
		categoryNameDisplay.text = name;
		RecipeOn();
	}

	public void RecipeOn()
	{
		for (int i = 0; i < catgRecipes.Count; i++)
		{
			if(i >= allRecipeImg.Count)
			{
				GameObject g = new GameObject($"RecipeCont{++recipeId}");
				Button b = g.AddComponent<Button>();
				Image img = g.AddComponent<Image>();
				img.sprite = GameManager.instance.uiBase;
				img.rectTransform.sizeDelta = new Vector2(250, 100);
				b.targetGraphic = img;
				int idx = recipeId - 1;
				b.onClick.AddListener(()=>{CraftOn(idx);});
				RecipeUI r = g.AddComponent<RecipeUI>();
				img.color = new Color(1, 1, 1, 0.3f);
				g.transform.SetParent(recipes);
				r.SetInfo(catgRecipes[i].Value.info.icon, catgRecipes[i].Value.info.MyName);
				allRecipeImg.Add(r);
			}
			else
			{
				allRecipeImg[i].SetInfo(catgRecipes[i].Value.info.icon, catgRecipes[i].Value.info.MyName);
			}
			allRecipeImg[i].gameObject.SetActive(true);
		}

		for (int i = 0; i < allRecipeImg.Count; i++)
		{
			if(i >= catgRecipes.Count)
			{
				allRecipeImg[i].gameObject.SetActive(false);
			}
		}
	}

	public void CraftOn(int idx)
	{
		craft.gameObject.SetActive(true);
		curShowingRecipe = idx;
		curRecipe = catgRecipes[curShowingRecipe].Key; 

		craft.SetRecipe(catgRecipes[curShowingRecipe]);


	}

	public void CraftOff()
	{
		craft.gameObject.SetActive(false);
	}
}
