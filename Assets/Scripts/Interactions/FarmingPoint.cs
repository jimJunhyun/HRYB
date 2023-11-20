using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmingPoint : MonoBehaviour, IInterable
{
	const float RECENTERINGTIME = 0.2f;


	public float interTime = 1.0f;
	public bool isInterable = true;
	public bool isDestroyed = true;
	bool altInterable  =false;

	public bool regenable = true;
	public float regenSec = 6f;

	public string resItem;
	public int amount;

	public bool IsInterable { get => isInterable; set => isInterable = value; }
	public float InterTime { get => interTime; set => interTime = value; }

	public bool AltInterable => altInterable;

	public InterType interType { get ; set ; } = InterType.PickUp;
	public AltInterType altInterType { get ; set ; } = AltInterType.None;

	ItemRarity spotStat;

	Renderer r;
	Collider c;
	Material mat;

	WaitForSeconds sec;

	//Coroutine ongoing;

	private void Awake()
	{
		r = GetComponent<Renderer>();
		mat = r.material;
		r.material = new Material(mat);
		mat = r.material;
		c = GetComponent<Collider>();
		GlowOff();
		sec = new WaitForSeconds(regenSec);
	}

	public void GlowOn()
	{
		mat.SetFloat(IInterable.GlowPowerHash, 0.5f);
		
	}

	public void GlowOff()
	{
		mat.SetFloat(IInterable.GlowPowerHash, 0f);
	}

	public void InteractWith()
	{
		//if(ongoing == null)
		//{
		//	ongoing = GameManager.instance.StartCoroutine(DelInter()); //임시. 풀링으로 변경하면 그냥 여기서 하면 됨/
		//}
		Inter();
	}

	public void Inter()
	{
		int leftovers = 0;
		if (Item.nameDataHashT.ContainsKey(resItem.GetHashCode()))
		{
			Item result = (Item.nameDataHashT[resItem.GetHashCode()] as Item);
			result.SetRarity(spotStat);
			leftovers = (GameManager.instance.pinven.AddItem(result, amount));
		}
		if(leftovers > 0)
		{ 
			Debug.Log("아이템 떨구겠다.");
		}
		Debug.Log(transform.name);
		if(regenable && isDestroyed)
		{
			c.enabled = false;
			r.enabled = false;
			IsInterable = false;
			StartCoroutine(DelRegen());
		}
		else if (isDestroyed)
		{
			Destroy(gameObject);
			//PoolManager.ReturnObject(gameObject);
		}
	}

	public IEnumerator DelRegen()
	{
		yield return sec;
		r.enabled = true;
		c.enabled = true;
		IsInterable = true;
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
