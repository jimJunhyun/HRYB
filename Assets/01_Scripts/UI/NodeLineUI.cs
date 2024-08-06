using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeLineUI : MonoBehaviour
{
    public PlayerNode previous;
	public PlayerNode next;

	Image self;

	private void Awake()
	{
		self = GetComponent<Image>();
	}

	private void Start()
	{
		Refresh();
	}

	public void Refresh()
	{
		if(previous.completed && next.completed)
		{
			self.color = Color.black;
		}
		else if (previous.completed)
		{
			self.color = new Color(0.65f, 0.65f, 0.65f, 1);
		}
		else
		{
			self.color = new Color(0.85f, 0.85f, 0.85f, 1);
		}
	}
}
