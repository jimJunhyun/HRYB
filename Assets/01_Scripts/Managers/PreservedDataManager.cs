using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreservedDataManager : MonoBehaviour
{
	public bool assetbundleLoaded;
	public bool gameDataLoaded;
	public int lastSave;

	private void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}
}
