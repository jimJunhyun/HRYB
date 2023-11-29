using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public enum SheetNumber
{
		item = 607348165,
		YinYnag = 237835199
}

public class SheetItemTable
{
	public string name = "none";
	public string description = "none"; // 이건 안쓴다네요 그래도 시트에 있으니 일단 저장
	public string itemInfoTxt = "none";
	public ItemType type = ItemType.None;
	public YinYang yinYang = null;
	public int MaxItem = -1; // 0이면 최대

}

public class YinYangSheetData
{
	public string name = "none";
	public float yin = 0;
	public float yang = 0;
}

public class SheetManager : MonoBehaviour
{
	private Dictionary<int, SheetItemTable> _items = new();
	
	private Dictionary<int, YinYangSheetData> _yinYangData = new();


	const string _sheetAddress = "https://docs.google.com/spreadsheets/d/1U_d85oU7k3LJym1HeIO90zeiGZhk2D-k8w3PR9CgzaQ/";
		
	const string _sheetItemRow = "B4:H11";
	const string _sheetYinYangRow = "B4:E8";
	//https://docs.google.com/spreadsheets/d/1U_d85oU7k3LJym1HeIO90zeiGZhk2D-k8w3PR9CgzaQ/
	//https://docs.google.com/spreadsheets/d/1U_d85oU7k3LJym1HeIO90zeiGZhk2D-k8w3PR9CgzaQ/
	//https://docs.google.com/spreadsheets/d/1U_d85oU7k3LJym1HeIO90zeiGZhk2D-k8w3PR9CgzaQ/edit?usp=sharing


	[SerializeField] private int textIdx = 1;

	//[ContextMenu("ItemLog")]
	public SheetItemTable GetItems()
	{
		//Debug.Log($"Return {_items[textIdx].name}");
		return _items[textIdx];
	}
	

	public IEnumerator Start()
	{
		yield return StartCoroutine(GetYinYangSheetData());
		yield return StartCoroutine(GetItemSheetData());
	}

	IEnumerator GetItemSheetData()
	{
		string data;
		string ad = $"{_sheetAddress}export?format=tsv&gid={607348165}&range={_sheetItemRow}"; //range=" + plus;
		Debug.Log("Item");
	    using (UnityWebRequest www = UnityWebRequest.Get((ad)))
	    {
		    yield return www.SendWebRequest();

		    if (www.isDone)
		    {
			    data = www.downloadHandler.text;
			    
			    ItemDisplay(data);
			    
		    }
	    }
    }

	IEnumerator GetYinYangSheetData()
	{
		string data;
		string ad = $"{_sheetAddress}export?format=tsv&gid={237835199}&range={_sheetYinYangRow}"; //range=" + plus;
		Debug.Log("YinYang");
		using (UnityWebRequest www = UnityWebRequest.Get((ad)))
		{
			yield return www.SendWebRequest();

			if (www.isDone)
			{
				data = www.downloadHandler.text;
			    YinYangDataDisplay(data);
			}
		}
	}

	void YinYangDataDisplay(string sheetData)
	{
		Debug.Log(sheetData);
		string[] row = sheetData.Split('\n');
		for (int i = 0; i < row.Length; i++)
		{
			string[] colum = row[i].Split('\t');
			YinYangSheetData Data = new YinYangSheetData();
			
			int k = 1;
			Data.name = colum[k++];
			Debug.Log(colum[0]);
			Debug.Log(colum[1]);
			Debug.Log(colum[2]);
			Debug.Log(colum[3]);
			Data.yin = int.Parse(colum[k++]);
			Data.yang = int.Parse(colum[k++]);
			_yinYangData.Add(int.Parse(colum[0]), Data);
		}
	}

    void ItemDisplay(string sheetData)
    {
	    Debug.Log(sheetData);
	    string[] row = sheetData.Split('\n');
	    for (int i = 0; i < row.Length; i++)
	    {
		    string[] colum = row[i].Split('\t');
		    int k = 1;
		    SheetItemTable items = new SheetItemTable();
		    try
		    {
			    items.name = colum[k++];
			    items.description = colum[k++];
			    items.itemInfoTxt = colum[k++];
			    items.type = (ItemType)int.Parse(colum[k++]);

			    YinYangSheetData Yinyangdata = _yinYangData[int.Parse(colum[k++])];
			    
			    items.yinYang = new YinYang(Yinyangdata.yin, Yinyangdata.yang ); // 바뀔 예정
			    
			    items.MaxItem = int.Parse(colum[k++]);
		    }
		    catch
		    {
			    Debug.LogWarning(k);
			    Debug.LogWarning(colum[k++]);
			    
		    }
		    
		    _items.Add(int.Parse(colum[0]), items);

		    
		    Debug.Log($"item : {items}");
	    }

	    
	    
    }
    
}
