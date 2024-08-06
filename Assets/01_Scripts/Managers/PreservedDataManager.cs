using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreservedDataManager : MonoBehaviour
{
	[HideInInspector]
	public bool assetbundleLoaded;
	[HideInInspector]
	public bool gameDataLoaded;
	public int lastSave;

	public PrefabManager pManager;
	public ImageManager imageManager;
	public ItemPedia pedia;
	public SkillLoader skillLoader;

	public bool isEditor = false;

	public static PreservedDataManager instance;

	bool loadEnd = false;
	float loadAmount;
	Slider loadBar;

//#if !UNITY_EDITOR
	private void Awake()
	{
//#if !UNITY_EDITOR
		if(isEditor)
			return;
//#endif

		if(PreservedDataManager.instance == null)
		{
			instance = this;
		}

		imageManager = GameObject.Find("ImageManager").GetComponent<ImageManager>();
		pManager = GameObject.Find("PrefabManager").GetComponent<PrefabManager>();

		if(loadBar == null)
		{
			loadBar = GameObject.Find("LoadingBar").GetComponent<Slider>();
		}

		DontDestroyOnLoad(gameObject);

		StartCoroutine(InitializeAll());
	}

	private void OnEnable()
	{
//#if !UNITY_EDITOR
		if(isEditor)
			return;
//#endif
		loadEnd = false;
		loadAmount = 0;
		RefreshLoadBar();
	}

	void RefreshLoadBar()
	{
		if (!loadEnd)
		{
			loadBar.value = loadAmount;
		}
		else
		{
			loadBar.value = 1;
		}
		
	}

	void LoadComplete(float level)
	{
		loadAmount += level;
		loadAmount = Mathf.Clamp(loadAmount, 0,  1);
		if(loadAmount >= 1f)
			loadEnd = true;
		RefreshLoadBar();
	}


	IEnumerator InitializeAll()
	{
		if (!gameDataLoaded)
		{
			yield return StartCoroutine(Item.InitializeItem());
			LoadComplete(0.1f);

			yield return StartCoroutine(Crafter.InitializeRecipe());
			LoadComplete(0.1f);

			yield return StartCoroutine(Crafter.InitializeTrim());
			LoadComplete(0.1f);

			gameDataLoaded = true;
		}
		else
		{
			LoadComplete(0.3f);
		}
		yield return null;
		pedia = new ItemPedia();

		if (!assetbundleLoaded)
		{
			imageManager.DoLoad();
			LoadComplete(0.1f);
			pManager.DoLoad();
			LoadComplete(0.1f);
			skillLoader = new SkillLoader();
			LoadComplete(0.1f);

			assetbundleLoaded = true;
		}
		else
		{
			LoadComplete(0.3f);
		}


		var op = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Official_World");
		op.allowSceneActivation = false;
		while (op.progress < 0.9f)
		{
			Debug.Log("PROG " + op.progress);
			yield return null;
		}

		LoadComplete(0.95f);
		yield return new WaitForSeconds(0.5f);
		op.allowSceneActivation = true;
		Debug.Log("로드 다했다...!");
	}
//#endif
}
