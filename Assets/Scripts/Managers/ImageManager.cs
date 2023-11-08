using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ImageManager : MonoBehaviour
{
	public Sprite MedicineSprite;


	public ItemNameDictionary dictionary;
	private void Start()
	{
		var icons = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "itemicons"));
		if(icons == null)
			Debug.LogError("LOAD FAIL");

		Sprite[] allIcons =  icons.LoadAllAssets<Sprite>();
		for (int i = 0; i < allIcons.Length; i++)
		{
			(Item.nameDataHashT[dictionary.Dict[allIcons[i].name].GetHashCode()] as Item).icon = allIcons[i];
			Debug.Log($"아이템 스프라이트 설정 완료 : {allIcons[i].name} -> {(Item.nameDataHashT[dictionary.Dict[allIcons[i].name].GetHashCode()] as Item).MyName}");
		}
	}
}
