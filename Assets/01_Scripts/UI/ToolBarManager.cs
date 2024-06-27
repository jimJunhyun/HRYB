using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ToolState
{
	Inventory,
	Medicine,
	Quest,
	Node,
	Setting,
	Fusion,
	Collection,


	None
}
public class ToolBarManager : MonoBehaviour
{
    public ToolState state;

	Dictionary<ToolState, GameObject> windows = new Dictionary<ToolState, GameObject>();
	internal Dictionary<ToolState, IOpenableWindowUI> openables = new Dictionary<ToolState, IOpenableWindowUI>();

	public List<ToolBtn> toolButtons;
	List<ToolBtn> parents;

	IOpenableWindowUI curOpened;

	private void Awake()
	{
		List<Transform> childs = new List<Transform>();
		ToolState[] arr = (ToolState[])System.Enum.GetValues(typeof(ToolState));
		for (int i = 1; i < transform.childCount; i++) // Index 0 = ToolBar
		{
			childs.Add(transform.GetChild(i));
		}
		for (int i = 0; i < arr.Length - 1; i++)
		{
			Transform c = childs.Find(item => item.name == arr[i].ToString());
			windows.Add(arr[i], c.gameObject);
			openables.Add(arr[i], c.GetComponent<IOpenableWindowUI>());
		}

		toolButtons = new List<ToolBtn>(GetComponentsInChildren<ToolBtn>());
		parents = new List<ToolBtn>();
		for (int i = 0; i < toolButtons.Count; i++)
		{
			if(toolButtons[i].indicating == ToolState.None)
				parents.Add(toolButtons[i]);
		}
	}

	private void Start()
	{
		ChangeStatus(ToolState.Inventory);
	}

	private void Update()
	{
		if (curOpened != null)
		{
			curOpened.WhileOpening();
		}
		
	}

	public void ChangeStatus(ToolState windowStat)
	{
		ToolOff();
		if (curOpened != null)
		{
			curOpened.OnClose();
		}
		state = windowStat;
		RefreshButtons();
		if(state != ToolState.None)
		{
			windows[state]?.SetActive(true);
			curOpened = openables[state];
			curOpened?.OnOpen();
		}
		else
		{
			if (curOpened != null)
			{
				curOpened.OnClose();
			}
			curOpened = null;
		}
	}

	public void CloseWindow()
	{
		ChangeStatus(ToolState.None);
	}

	void ToolOff()
	{
		if(curOpened != null)
		{
			curOpened.OnClose();
			curOpened = null;
		}
		foreach (var item in windows.Values)
		{
			item.SetActive(false);
		}
	}
	public void RefreshButtons()
	{
		
		for (int i = 0; i < toolButtons.Count; i++)
		{
			toolButtons[i].ResetButton();
			
			if (state != ToolState.None && toolButtons[i].indicating == state)
			{
				toolButtons[i].Focus();
			}
		}
		for (int i = 0; i < parents.Count; i++)
		{
			parents[i].ParentButtonRefresh();
		}
		
	}

	public void RefreshWindows()
	{
		foreach (var item in openables.Values)
		{ 
			if(item != null)
			{
				item.Refresh();
			}
		}
	}

}
