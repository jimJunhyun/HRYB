using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemDetailInfoShower : MonoBehaviour
{
    Image fry;
    Image stir;
    Image burn;
    Image mash;

	private void Awake()
	{
		fry = transform.Find("Fry").GetComponent<Image>();
		stir = transform.Find("Stir").GetComponent<Image>();
		burn = transform.Find("Burn").GetComponent<Image>();
		mash = transform.Find("Mash").GetComponent<Image>();
	}

	public void SetInfo(HashSet<ProcessType> processes)
	{
		Color c = fry.color;
		c.a = processes.Contains(ProcessType.Fry) ? 1 : 0.5f;
		fry.color = c;

		c = stir.color;
		c.a = processes.Contains(ProcessType.Stir) ? 1 : 0.5f;
		stir.color = c;

		c = burn.color;
		c.a = processes.Contains(ProcessType.Burn) ? 1 : 0.5f;
		burn.color = c;

		c = mash.color;
		c.a = processes.Contains(ProcessType.Mash) ? 1 : 0.5f;
		mash.color = c;

	}

	public void ResetInfo()
	{
		Color c = fry.color;
		c.a = 0.5f;
		fry.color = c;

		c = stir.color;
		c.a = 0.5f;
		stir.color = c;

		c = burn.color;
		c.a = 0.5f;
		burn.color = c;

		c = mash.color;
		c.a = 0.5f;
		mash.color = c;
	}
}
