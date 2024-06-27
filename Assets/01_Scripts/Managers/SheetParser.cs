using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// export?format=tsv&gid=XXXX&range=B3:P
/// </summary>
public class SheetParser
{
	readonly string url;

	string tsvFormatText;

	/// <summary>
	/// 가로줄갯수
	/// </summary>
	public readonly int attributeCount;

	/// <summary>
	/// 세로줄갯수
	/// </summary>
	public int cardinality { get; private set; }

	bool indexed;

	public  bool inited;

	public event Action<SheetParser> onCompleted;

	List<string> heads;

	Dictionary<string, List<string>> headValuesPair; //세로의 이름과 그 밑에 있는 데이터들 리스트

	List<List<string>> tupleDatas; //가로의 데이터들

	Dictionary<string, List<string>> indexAttributePairs; //가로의 인덱스와 데이터 사항

	public bool Contains(string head, string value)
	{
		return inited && headValuesPair.ContainsKey(head) && headValuesPair[head].Contains(value);
	}

	public bool Contains(string index)
	{
		return indexed && inited && indexAttributePairs.ContainsKey(index);
	}

	public string GetAttribute(int rowOrder, int columnOrder)
	{
		if (!inited)
			throw new UnityException("시트의 데이터가 초기화되지 않았습니다.");
		if(rowOrder >= cardinality)
			throw new UnityException($"값의 갯수는 {rowOrder}보다 적습니다.");
		if (columnOrder >= attributeCount)
			throw new UnityException($"값의 갯수는 {columnOrder}보다 적습니다.");
		return tupleDatas[rowOrder][columnOrder];
	}

	public string GetAttribute(string headName, int rowOrder)
	{
		if (!inited)
			throw new UnityException("시트의 데이터가 초기화되지 않았습니다.");
		if(!headValuesPair.ContainsKey(headName))
			throw new UnityException($"이름 : {headName}인 인덱스가 존재하지 않습니다.");
		if (rowOrder >= attributeCount)
			throw new UnityException($"값의 갯수는 {rowOrder}보다 적습니다.");
		return headValuesPair[headName][rowOrder];
	}

	public List<string> GetTuple(int idx)
	{
		if (!inited)
			throw new UnityException("시트의 데이터가 초기화되지 않았습니다.");
		if (idx >= cardinality)
			throw new UnityException($"값의 갯수는 {idx}보다 적습니다.");
		return tupleDatas[idx];
	}

	public List<string> GetTupleByIndex(string index)
	{
		if (!inited)
			throw new UnityException("시트의 데이터가 초기화되지 않았습니다.");
		if (!indexAttributePairs.ContainsKey(index))
			throw new UnityException($"이름 : {index}인 인덱스가 존재하지 않습니다.");
		return indexAttributePairs[index];
	}

	public string GetAttributeByIndex(string index, int columnOrder)
	{
		if (!inited)
			throw new UnityException("시트의 데이터가 초기화되지 않았습니다.");
		if (!indexAttributePairs.ContainsKey(index))
			throw new UnityException($"이름 : {index}인 인덱스가 존재하지 않습니다.");
		if(columnOrder >= attributeCount)
			throw new UnityException($"값의 갯수는 {columnOrder}보다 적습니다.");
		return indexAttributePairs[index][columnOrder];
	}

	
	public SheetParser(string link, string from, string to, bool hasIndex = true, bool useCoroutine  = true)
	{
		url = link;
		heads = new List<string>();
		headValuesPair = new Dictionary<string, List<string>>();
		tupleDatas = new List<List<string>>();

		indexed = hasIndex;
		if (indexed)
		{
			indexAttributePairs = new Dictionary<string, List<string>>();
		}

		inited = false;

		int f = 0;
		int t = 0;

		int dig = 1;
		for (int i = from.Length - 1; i >= 0; --i)
		{
			f += (from[i] - 'A' + 1) * dig;
			dig *= 26;
		}
		dig = 1;
		for (int i = from.Length - 1; i >= 0; --i)
		{
			t += (to[i] - 'A' + 1) * dig;
			dig *= 26;
		}
		attributeCount = t - f + 1;

		if (useCoroutine)
		{
			GameManager.instance.StartCoroutine(Load());
		}
		else
		{
			UnityWebRequest req = UnityWebRequest.Get(url);
			var waiter = req.SendWebRequest();

			waiter.completed += (x)=>OnLoadCompleted(x, req);
			
		}

	}

	public void OnLoadCompleted(AsyncOperation oper, UnityWebRequest req)
	{
		if (oper.isDone)
		{
			
			
			tsvFormatText = req.downloadHandler.text;
			Debug.Log(tsvFormatText);

			req.Dispose();

			string[] rows = tsvFormatText.Split('\n');

			cardinality = rows.Length - 1;

			string[] cols;
			for (int i = 0; i < rows.Length; i++)
			{
				cols = rows[i].Split('\t');
				List<string> colsSplit = new List<string>();
				for (int j = 0; j < cols.Length; j++)
				{
					if (i == 0)
					{
						heads.Add(cols[j].Trim());
						headValuesPair.Add(cols[j].Trim(), new List<string>());
					}
					else
					{
						headValuesPair[heads[j]].Add(cols[j].Trim());
						colsSplit.Add(cols[j].Trim());
					}
				}
				if (i != 0)
				{
					tupleDatas.Add(colsSplit);
				}

				if (indexed && i != 0)
				{
					indexAttributePairs.Add(colsSplit[0], colsSplit);
				}
			}

			inited = true;

			onCompleted.Invoke(this);
		}
		
	}

	

	public IEnumerator Load()
	{
		using(UnityWebRequest req = UnityWebRequest.Get(url))
		{
			yield return req.SendWebRequest();

			if (req.isDone)
			{
				tsvFormatText = req.downloadHandler.text;
				Debug.Log(tsvFormatText);
			}
		}

		string[] rows = tsvFormatText.Split('\n');

		cardinality = rows.Length - 1;

		string[] cols;
		for (int i = 0; i < rows.Length; i++)
		{
			cols = rows[i].Split('\t');
			List<string> colsSplit = new List<string>();
			for (int j = 0; j < cols.Length; j++)
			{
				if(i == 0)
				{
					heads.Add(cols[j].Trim());
					headValuesPair.Add(cols[j].Trim(), new List<string>());
				}
				else
				{
					headValuesPair[heads[j]].Add(cols[j].Trim());
					colsSplit.Add(cols[j].Trim());
				}
			}
			if(i != 0)
			{
				tupleDatas.Add(colsSplit);
			}

			if (indexed && i != 0)
			{
				indexAttributePairs.Add(colsSplit[0], colsSplit);
			}
		}

		inited = true;
		onCompleted?.Invoke(this);
	}

}
