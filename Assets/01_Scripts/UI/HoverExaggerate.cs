using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HoverExaggerate : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public float sizeModifier;
	public Color colorModifier;

	public UnityEvent onClick;

	public bool polygonHoverCheck;


	Color origin;
    Image self;

	Collider2D outhit;

	bool f;
	bool foc 
	{
		get=>f;
		set
		{
			f = value;
			if (f)
				Exag();
			else
				DeExag();
		}
	}

	private void Awake()
	{
		self = GetComponent<Image>();
		origin = self.color;
		f = false;
	}

	private void Update()
	{
		if(!polygonHoverCheck)
			return;
		outhit = Physics2D.OverlapCircle(Input.mousePosition, 1f);
		if(outhit)
		{
			if (!foc)
			{
				foc = true;
			}
		}
		else
		{
			if (foc)
			{
				foc = false;
			}
		}

		if (Input.GetMouseButtonUp(0))
		{
			if (foc)
			{
				onClick.Invoke();
			}
		}
	}

	public void Exag()
	{
		if(self == null)
		{
			self = GetComponent<Image>();
			
		}

		self.rectTransform.localScale *= sizeModifier;
		self.color = colorModifier;
	}

	public void DeExag()
	{
		if (self == null)
		{
			self = GetComponent<Image>();
			origin = self.color;
		}

		self.rectTransform.localScale /= sizeModifier;
		self.color = origin;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if(!foc)
			foc = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if(foc)
			foc = false;
	}
}
