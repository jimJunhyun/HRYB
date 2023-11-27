using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuickSlot : MonoBehaviour
{
	public Image img;
	public TMP_Text count;


	private void Update()
	{
		GetComponent<Image>().sprite = img.sprite;
		GetComponentInChildren<TMP_Text>().text = count.text;
	}
}
