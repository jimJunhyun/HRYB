using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmingPoint : MonoBehaviour, IInterable
{
	const float RECENTERINGTIME = 0.2f;
	public const string CANVASNAME = "IndicatorCanv";


	public float interTime = 1.0f;
	public bool isInterable = true;
	public bool isDestroyed = true;
	bool altInterable = false;

	public bool regenable = true;
	public float regenSec = 6f;

	public List<string> resItem;
	public int amount;

	public float yOffset = 2;
	
	public string Name { get => transform.name; }
	public bool IsInterable { get => isInterable; set => isInterable = value; }
	public float InterTime { get => interTime; set => interTime = value; }

	public bool AltInterable => altInterable;

	public InterType interType { get ; set ; } = InterType.PickUp;
	public AltInterType altInterType { get ; set ; } = AltInterType.None;

	ItemRarity spotStat;

	bool getable;

	GameObject canv;

	List<Renderer> r;
	Collider c;
	Material mat;

	WaitForSeconds sec;

	//Coroutine ongoing;

	private void Awake()
	{
		r = new List<Renderer>(GetComponentsInChildren<Renderer>());
		mat = r[0].material;
		mat = new Material(mat);
		foreach (Renderer item in r)
		{
			item.material = mat;
		}
		c = GetComponent<Collider>();
		GlowOff();
		sec = new WaitForSeconds(regenSec);
		getable = true;
	}

	private void Start()
	{
		canv = PoolManager.GetObject(CANVASNAME, transform);
		canv.transform.localPosition = Vector3.up * yOffset;
	}

	private void Update()
	{
		if(getable && (transform.position - GameManager.instance.player.transform.position).sqrMagnitude <= GameManager.instance.pActor.sight.GetSightRange() * GameManager.instance.pActor.sight.GetSightRange())
		{
			canv.SetActive(true);
		}
		else
		{
			canv.SetActive(false);
		}
	}

	public void GlowOn()
	{
		mat.SetFloat(IInterable.GlowPowerHash, 0.5f);
		mat.SetFloat(IInterable.GlowOnHash, 1);

		//mat.SetInt(IInterable.GlowOnHash, 1);
	}

	public void GlowOff()
	{
		//mat.SetFloat(IInterable.GlowPowerHash, 0f);
		mat.SetFloat(IInterable.GlowOnHash, 0);
	}

	public void InteractWith()
	{
		if (getable)
		{
			Inter();
		}
	}

	public void Inter()
	{
		int leftovers = 0;
		for (int i = 0; i < resItem.Count; i++)
		{
			if (Item.nameDataHashT.ContainsKey(resItem[i]))
			{
				Item result = Item.GetItem<Item>(resItem[i]);
				//result.SetRarity(spotStat);
				leftovers += (GameManager.instance.pinven.AddItem(result, amount));
			}
		}
		
		if(leftovers > 0)
		{ 
			Debug.Log("아이템 떨구겠다.");
		}
		Debug.Log(transform.name);
		if (isDestroyed)
		{
			Debug.Log("재생대기중");
			c.enabled = false;
			foreach (var item in r)
			{
				item.enabled = false;
			}
			canv.SetActive(false);
			IsInterable = false;
			
			getable = false;
		}
		if(regenable)
		{
			StartCoroutine(DelRegen());
		}
	}

	public IEnumerator DelRegen()
	{
		yield return sec;
		Debug.Log($"재생함{transform.name}");
		foreach (var item in r)
		{
			item.enabled = true;
		}
		c.enabled = true;
		IsInterable = true;
		canv.SetActive(true);
		getable = true;
	}


	public void AltInterWith()
	{
		AltInter();
	}

	public void AltInter()
	{
		throw new System.NotImplementedException();
	}
}
