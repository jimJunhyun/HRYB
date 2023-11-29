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
	public string name = "Error";
	public string description = "Error"; // 이건 안쓴다네요
	public string itemInfoTxt = "Error";
	public ItemType type = ItemType.None;
	public int YinYang = -1;
	public int MaxItem = -1;

}

public class SheetManager : MonoBehaviour
{
	private List<SheetItemTable> items = new();

	const string adress = "https://docs.google.com/spreadsheets/d/1U_d85oU7k3LJym1HeIO90zeiGZhk2D-k8w3PR9CgzaQ/";
	const string plus = "B4:H11";


	public void Start()
	{
		StartCoroutine(GetSheetData());
	}

	IEnumerator GetSheetData()
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
			    
			    DisplayText(data);
			    
		    }
	    }

    }

    void DisplayText(string sheetData)
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
			    items.YinYang = int.Parse(colum[k++]);
			    items.MaxItem = int.Parse(colum[k++]);
		    }
		    catch
		    {
			    Debug.Log(k);
			    Debug.Log(colum[k++]);
		    }

		    
		    Debug.Log($"item : {items}");
	    }

	    
	    
    }
    
}
