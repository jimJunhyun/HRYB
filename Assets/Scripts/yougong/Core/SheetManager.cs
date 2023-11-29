using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SheetData
{
		
}

public class SheetItemTable
{
	public int index;
	public string name = "none";
	public string description = "none"; // 이건 안쓴다네요
	public string itemInfoTxt = "none";
	public ItemType type = ItemType.None;
	public int YinYang = -1; // 클레스화
	public int MaxItem = -1; // 0이면 최대

}

public class SheetManager : MonoBehaviour
{
	private List<SheetItemTable> items = new();

	const string adress = "https://docs.google.com/spreadsheets/d/1U_d85oU7k3LJym1HeIO90zeiGZhk2D-k8w3PR9CgzaQ/";
	//https://docs.google.com/spreadsheets/d/1U_d85oU7k3LJym1HeIO90zeiGZhk2D-k8w3PR9CgzaQ/
	//https://docs.google.com/spreadsheets/d/1U_d85oU7k3LJym1HeIO90zeiGZhk2D-k8w3PR9CgzaQ/
	//https://docs.google.com/spreadsheets/d/1U_d85oU7k3LJym1HeIO90zeiGZhk2D-k8w3PR9CgzaQ/edit?usp=sharing
	const string plus = "B4:H11";


	public void Start()
	{
		StartCoroutine(GetItemSheetData());
	}

	IEnumerator GetItemSheetData()
	{
		string data;
		string ad = adress + "export?format=tsv&range=" + plus;
	    Debug.Log("Init");
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

	IEnumerator GetYinYangShhetData()
	{
		
	}

    void ItemDisplay(string sheetData)
    {
	    Debug.Log(sheetData);
	    string[] row = sheetData.Split('\n');
	    for (int i = 0; i < row.Length; i++)
	    {
		    string[] colum = row[i].Split('\t');
		    int k = 0;
		    SheetItemTable items = new SheetItemTable();
		    try
		    {
			    items.index = int.Parse(colum[k++]);
			    items.name = colum[k++];
			    items.description = colum[k++];
			    items.itemInfoTxt = colum[k++];
			    items.type = (ItemType)int.Parse(colum[k++]);
			    items.YinYang = int.Parse(colum[k++]); // 바뀔 예정
			    items.MaxItem = int.Parse(colum[k++]);
		    }
		    catch
		    {
			    Debug.LogWarning(k);
			    Debug.LogWarning(colum[k++]);
		    }

		    
		    Debug.Log($"item : {items}");
	    }

	    
	    
    }
    
}
