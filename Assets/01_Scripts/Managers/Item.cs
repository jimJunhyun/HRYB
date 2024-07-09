using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ItemType
{
	None = -1,
	Solid,
	Liquid,
	Air,
}

public enum ItemRarity
{
	S,
	A,
	B,
	C,
	D,
	F,

	Medicine,
	Material,
}

public class Specials
{
	//public System.Func<bool> onActivated;
	//public float removeTime;

	//public void DeleteSpecial()
	//{
	//	onActivated = null;
	//}

	public List<float> modAdd = new List<float>();
	public List<float> modMult = new List<float>();
	public float effTime;
	const int MODCOUNT = 5;
	const int DATAFROM = 2;

	public Specials(List<string> strs, string effT)
	{
		for (int i = DATAFROM; i < strs.Count; i++)
		{
			Debug.Log(i+ " : "+ strs[i]);
			if(i % 2 == 0)
			{
				modAdd.Add(float.Parse(strs[i]));
			}
			else
			{
				modMult.Add(float.Parse(strs[i]));
			}
		}


		effTime = float.Parse(effT);
	}

	public void Use(Actor user)
	{
		for (int i = 0; i < MODCOUNT; i++)
		{
			Debug.Log(((StatUpgradeType)i).ToString() + " 가 " + modAdd[i] + " 만큼 증가, " + modMult[i] + "% 만큼 증가. " + effTime + " 초 동안.");
			user.HandleStatus((StatUpgradeType)i, modAdd[i], modMult[i], effTime);
		}
	}
}

[System.Serializable]
public class Item : IComparable // #################
{
    public static Hashtable nameDataHashT = new Hashtable()
	{
		//{"나뭇가지".GetHashCode(), new Item("나뭇가지","나무에서 열리는 가지를 나뭇가지라고 부르더라", ItemType.Solid, 999, null, -1, false) },
		//{"인삼".GetHashCode(), new YinyangItem("인삼", "사실 인삼이나 산삼이나 거기서 거기다", ItemType.Solid, 999, null, false, -1, DetailAmount.Empty, false, "삼") },
		//{"물".GetHashCode(), new YinyangItem("물", "물로 보지 말랜다", ItemType.Liquid,  999, null, false, -1, DetailAmount.Empty, false, "수")},
		//{"활".GetHashCode(), new Item("활", "적당한 나무활이다. 사용해서 장착해볼까?", ItemType.Solid, 999, new Specials(()=>{
		//	GameManager.instance.pinven.ObtainWeapon();
		//	return true;
		//}, Mathf.Infinity), -1, false) },
		//{"밧줄".GetHashCode(), new Item("밧줄", "무언가에 매기 위해 맨 줄이다.", ItemType.Solid,  999, new Specials(()=>
		//{
		//	return (GameManager.instance.pActor.atk as PlayerAttack).ThrowRope();
		//}, Mathf.Infinity), -1, false) },
		//{"섬유".GetHashCode(), new Item("섬유", "튼튼한 섬유이다.", ItemType.Solid, 999, null, -1, false) },
		//{"산삼".GetHashCode(), new YinyangItem("산삼", "심 봤 다", ItemType.Solid, 999, null, false, -1, DetailAmount.Empty, false, "삼") },
		//{"잘린 산삼".GetHashCode(), new YinyangItem("잘린 산삼", "손질 완료된 산삼이다.", ItemType.Solid, 999, null, false, -1, DetailAmount.Empty, false, "삼") },
		//{"녹제".GetHashCode(), new YinyangItem("녹제", "사슴의 발굽이다.", ItemType.Solid, 999, null, false, -1,  DetailAmount.Empty, false) },
		//{"녹각".GetHashCode(), new YinyangItem("녹각", "사슴의 뿔이다.", ItemType.Solid, 999, null, false, -1,  DetailAmount.Empty, false) },
		//{"녹용".GetHashCode(), new YinyangItem("녹용", "사슴의 뿔을 손질했다.", ItemType.Solid, 999, null, false, -1, DetailAmount.Empty, false) },
		//{"도약탕".GetHashCode(), new Medicines("도약탕", "다리의 힘을 비약적으로 상승시켜 땅을 박차고 공중으로 도약할 수 있게 해준다.", ItemType.Liquid, 999, null, false, -1, DetailAmount.Empty) },
	}; //같은 이름의 아이템을 같은 물건으로 취급하기 위해 사용.

	//static Hashtable allYinyangItems;
	//public static Hashtable AllYinyangItems
	//{
	//	get
	//	{
	//		if(allYinyangItems == null)
	//		{
	//			allYinyangItems = new Hashtable();
	//			foreach (int item in nameDataHashT.Keys)
	//			{
	//				if(nameDataHashT[item] is YinyangItem yy && !(nameDataHashT[item] is Medicines))
	//				{
	//					allYinyangItems.Add(item, nameDataHashT[item]);
	//				}
	//			}
	//		}
	//		return allYinyangItems;
	//	}
	//}

	//static Hashtable allMedicines;
	//public static Hashtable AllMedicines
	//{
	//	get
	//	{
	//		if (allMedicines == null)
	//		{
	//			allMedicines = new Hashtable();
	//			foreach (int item in nameDataHashT.Keys)
	//			{
	//				if ((nameDataHashT[item] is Medicines))
	//				{
	//					allMedicines.Add(item, nameDataHashT[item]);
	//				}
	//			}
	//		}
	//		return allMedicines;
	//	}
	//}

	public static T GetItem<T>(string name) where T : Item
	{
		//if(nameDataHashT[name] != null)
		//	Debug.Log(nameDataHashT[name].GetType().ToString());
		if(nameDataHashT[name] is T)
		{
			//Debug.Log("찾았습니다");
			return (T)nameDataHashT[name];
		}
		//Debug.Log("찾지 못했습ㄴ디ㅏ.");
		return null;
	}

	//public static Item GetItemBoxed(string name)
	//{
	//	if (nameDataHashT.ContainsKey(name))
	//	{
	//		if(nameDataHashT[name] is Medicines)
	//		{
	//			return (Medicines)nameDataHashT[name];
	//		}
	//		else if (nameDataHashT[name] is YinyangItem)
	//		{
	//			return (YinyangItem)nameDataHashT[name];
	//		}
	//		else
	//		{
	//			return (Item)nameDataHashT[name];
	//		}
	//	}
	//	return null;
	//}

	public static Dictionary<Item, int> useCount = new Dictionary<Item, int>();

	public int Id {get => MyName.GetHashCode();}
    public string originalName;
	public virtual string MyName
	{
		get => originalName; set => originalName = value;
	}

	public string desc;

    public Sprite icon;
	public ItemType itemType;

	public Specials onUse;

    public int maxStack;

	public int defaultPrice;
	public bool Sellable => defaultPrice > 0;
	
	//public ItemRarity rarity;
	
	

	//public void SetRarity(ItemRarity val)
	//{
	//	rarity = val;
	//	switch (val)
	//	{
	//		case ItemRarity.S:
	//			break;
	//		case ItemRarity.A:
	//			break;
	//		case ItemRarity.B:
	//			break;
	//		case ItemRarity.C:
	//			break;
	//		case ItemRarity.D:
	//			break;
	//		case ItemRarity.F:
	//			break;
	//		default:
	//			break;
	//	}
	//}

	const int NAMEFROM = 2;
	const int TYPEIDX = 4;

	public static IEnumerator InitializeItem()
	{
		nameDataHashT.Clear();
		if (GameManager.instance)
		{
			GameManager.instance.saver.imageManager.dictionary.Dict.Clear();
		}
		else if (PreservedDataManager.instance)
		{
			PreservedDataManager.instance.imageManager.dictionary.Dict.Clear();

		}
		SheetParser data = new SheetParser("https://docs.google.com/spreadsheets/d/1U_d85oU7k3LJym1HeIO90zeiGZhk2D-k8w3PR9CgzaQ/export?format=tsv&gid=607348165&range=B3:R", "B", "R");
		SheetParser useData = new SheetParser("https://docs.google.com/spreadsheets/d/1U_d85oU7k3LJym1HeIO90zeiGZhk2D-k8w3PR9CgzaQ/export?format=tsv&gid=36776999&range=B3:M", "B", "M");
		yield return new WaitUntil(() => data.inited && useData.inited);
		
		for (int i = 0; i < data.cardinality; i++)
		{
			List<string> tup = data.GetTuple(i);
			//for (int j = 0; j < tup.Count; j++)
			//{
			//	Debug.Log(j + " : " +tup[j]);
			//}
			Item itm = null;
			Specials sp = null;
			if (tup[NAMEFROM + 10].Length > 0)
			{
				sp = new Specials(useData.GetTupleByIndex(tup[NAMEFROM + 10]), tup[NAMEFROM + 11]);

			}

			switch (int.Parse(tup[TYPEIDX]))
			{
				case 0:
					itm = new Item(tup[NAMEFROM], tup[NAMEFROM + 1], ItemType.None,int.Parse( tup[NAMEFROM + 13]), null/**/, int.Parse(tup[NAMEFROM + 12]), true);
					Debug.Log(itm.MyName + " : " + itm.desc);
					break;
				case 1:
					itm = new YinyangItem(tup[NAMEFROM], tup[NAMEFROM + 1], ItemType.None,int.Parse( tup[NAMEFROM + 13]), sp, true, int.Parse(tup[NAMEFROM + 12]), 
						new DetailAmount(
							(float)int.Parse(tup[NAMEFROM + 3]) / 100.0f,
							(float)int.Parse(tup[NAMEFROM + 4]) / 100.0f,
							(float)int.Parse(tup[NAMEFROM + 5]) / 100.0f,
							(float)int.Parse(tup[NAMEFROM + 6]) / 100.0f,
							(float)int.Parse(tup[NAMEFROM + 7]) / 100.0f,
							(float)int.Parse(tup[NAMEFROM + 8]) / 100.0f,
							(float)int.Parse(tup[NAMEFROM + 9]) / 100.0f
							), sp != null);
					Debug.Log(itm.MyName + " : " + itm.desc);
					break;
				case 2:
					itm = new Medicines(tup[NAMEFROM], tup[NAMEFROM + 1], ItemType.None, int.Parse(tup[NAMEFROM + 13]), sp, true, int.Parse(tup[NAMEFROM + 12]),
						new DetailAmount(
							(float)int.Parse(tup[NAMEFROM + 3]) / 100.0f,
							(float)int.Parse(tup[NAMEFROM + 4]) / 100.0f,
							(float)int.Parse(tup[NAMEFROM + 5]) / 100.0f,
							(float)int.Parse(tup[NAMEFROM + 6]) / 100.0f,
							(float)int.Parse(tup[NAMEFROM + 7]) / 100.0f,
							(float)int.Parse(tup[NAMEFROM + 8]) / 100.0f,
							(float)int.Parse(tup[NAMEFROM + 9]) / 100.0f
							));
					//Debug.Log(itm.MyName + " : " + itm.desc);
					break;
				default:
					break;
			}
			if (GameManager.instance)
			{
				GameManager.instance.saver.imageManager.dictionary.Dict.Add(tup[NAMEFROM + 14].Trim(), itm.originalName);

			}
			else if (PreservedDataManager.instance)
			{
				PreservedDataManager.instance.imageManager.dictionary.Dict.Add(tup[NAMEFROM + 14].Trim(), itm.originalName);
			}
			
		}


		Debug.Log("아이템 초기화 완료.");

		//foreach (Item item in nameDataHashT.Values)
		//{
		//	Debug.Log($"{item.MyName} : {item.desc}");
		//}
	}

	public Item(Item dat)
	{
		originalName = dat.originalName;
		desc = dat.desc;
		itemType = dat.itemType;
		maxStack = dat.maxStack;
		onUse = dat.onUse;
		icon = dat.icon;
		defaultPrice = dat.defaultPrice;
	}

	public Item(string n, string d, ItemType iType, int max, Specials useFunc, int price ,bool isNewItem)
	{
        originalName = n;
		desc = d;
		if (isNewItem)
		{
			InsertToTable();
			
			//경험치주기@@@@@@@@@@@@@@@@@@22
		}
		itemType = iType;
		maxStack = max;
		onUse = useFunc;
		defaultPrice = price;
	}

	public virtual void InsertToTable()
	{
		if (!nameDataHashT.ContainsKey(MyName))
		{
			Debug.Log($"새로운 아이템 : {MyName}");
			nameDataHashT.Add(MyName, this);
		}
	}

	public override int GetHashCode()
	{
		return MyName.GetHashCode();
	}

	public static bool operator ==(Item left, Item right)
	{
		//Debug.Log($"{left?.MyName} == {right?.MyName} --> {left?.MyName == right?.MyName }");
		return left?.MyName == right?.MyName ;
	}

    public static bool operator !=(Item left, Item right)
    {
		return left?.MyName != right?.MyName ;
    }

    public override bool Equals(object obj)
	{
		Debug.Log("???");
		return obj is Item i && i.MyName == MyName;
	}

	public virtual void Use()
	{
		bool res = false;
		GameManager.instance.qManager.InvokeOnChanged(CompletionAct.UseItem, MyName);
		
		onUse.Use(GameManager.instance.pActor);

		GameManager.instance.pinven.RemoveItem(this);
		//if (onUse?.onActivated != null && (res = onUse.onActivated.Invoke()))
		//{
		//	Debug.Log("ITEM REMOVED");
		//}
		//else
		//{
		//	Debug.Log($"{onUse}");
		//	Debug.Log($"{onUse?.onActivated != null} && {res}"); //사용실패. 아이템 효과가 없거나 사용이 성공하지 못함.
		//}
	}

	public void SetSprite(Sprite sp)
	{
		icon = sp;
	}

	public virtual int CompareTo(object obj)
	{
		if(obj is Item item)
		{
			if(originalName.CompareTo(item.originalName) == 0)
			{
				return 0;
			}
			else
				return originalName.CompareTo(item.originalName);
		}
		return 0;
	}
}

