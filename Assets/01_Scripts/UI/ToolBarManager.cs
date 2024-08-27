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

public struct OverlayToolStat
{
	Stack<ToolState> overlayStatus;
	ToolState curStat;

	public OverlayToolStat(ToolState initVal)
	{
		curStat = initVal;
		overlayStatus = new Stack<ToolState>();
	}

	public ToolState CurStat
	{
		get
		{
			if(overlayStatus.Count> 0)
			{
				return overlayStatus.Peek();
			}
			return curStat;
		}
	}

	public void Reset(ToolState stt)
	{
		overlayStatus.Clear();
		curStat = stt;
	}

	public void Overlay(ToolState stt)
	{
		overlayStatus.Push(stt);
	}

	public ToolState CloseUppermost()
	{
		if(overlayStatus.Count > 0)
		{
			return overlayStatus.Pop();
		}
		ToolState stt = curStat;
		curStat = ToolState.None;
		return stt;
	}

	public IEnumerable<ToolState> CloseAllOverlay()
	{
		while(overlayStatus.Count > 0)
		{
			yield return overlayStatus.Pop();
		}
	}

	public IEnumerable<ToolState> CloseAll()
	{
		while (overlayStatus.Count > 0)
		{
			yield return overlayStatus.Pop();
		}
		yield return curStat;
		curStat = ToolState.None;
	}

	public IEnumerable<ToolState> GetAll()
	{
		
		foreach (var item in overlayStatus)
		{
			yield return item;
		}
		yield return curStat;
	}
}

public class ToolBarManager : MonoBehaviour
{
    public OverlayToolStat state;

	Dictionary<ToolState, GameObject> windows = new Dictionary<ToolState, GameObject>();
	internal Dictionary<ToolState, IOpenableWindowUI> openables = new Dictionary<ToolState, IOpenableWindowUI>();

	//public List<ToolBtn> toolButtons;
	//internal List<ToolBtn> parents;

	//internal Dictionary<ToolState, ToolBtn> toolStateButtonPair = new Dictionary<ToolState, ToolBtn>();


	internal bool opened = false;

	private void Awake()
	{
		List<Transform> childs = new List<Transform>();
		ToolState[] arr = (ToolState[])System.Enum.GetValues(typeof(ToolState));
		for (int i = 0; i < transform.childCount-1; i++) // Last Index = ToolBar
		{
			childs.Add(transform.GetChild(i));
		}
		for (int i = 0; i < arr.Length - 1; i++)
		{
			Transform c = childs.Find(item => item.name == arr[i].ToString());
			windows.Add(arr[i], c.gameObject);
			openables.Add(arr[i], c.GetComponent<IOpenableWindowUI>());
		}

		state = new OverlayToolStat(ToolState.None);

		//toolButtons = new List<ToolBtn>(GetComponentsInChildren<ToolBtn>());
		//parents = new List<ToolBtn>();
		//toolStateButtonPair = new Dictionary<ToolState, ToolBtn>();
		//for (int i = 0; i < toolButtons.Count; i++)
		//{
		//	if(toolButtons[i].indicating == ToolState.None)
		//		parents.Add(toolButtons[i]);
		//	else if (!toolStateButtonPair.ContainsKey(toolButtons[i].indicating))
		//	{
		//		toolStateButtonPair.Add(toolButtons[i].indicating, toolButtons[i]);
		//	}
		//}
		opened = false;
	}

	private void Start()
	{
		InitCloseAll();
		ChangeStatus(ToolState.Inventory);
	}

	private void Update()
	{
		foreach (var item in state.GetAll())
		{
			if(item != ToolState.None)
			{
				openables[item]?.WhileOpening();
			}
		}
		
	}

	public void ChangeStatus(ToolState windowStat)
	{
		if(windowStat == ToolState.None)
		{
			foreach (var item in state.CloseAll())
			{
				if(item != ToolState.None)
				{
					openables[item]?.OnClose();
					
					windows[item]?.SetActive(false);
				}
			}
			state.Reset(ToolState.None);

			gameObject.SetActive(false);
			GameManager.instance.LockCursor();
			opened = false;
			Time.timeScale = 1;

			return;
		}

		gameObject.SetActive(true);
		opened = true;
		GameManager.instance.UnLockCursor();
		Time.timeScale = 0;

		if (openables[windowStat].isOverlay)
		{
			state.Overlay(windowStat);
		}
		else
		{
			foreach (var item in state.CloseAll())
			{
				if (openables.ContainsKey(item))
				{
					openables[item]?.OnClose();		
					windows[item]?.SetActive(false);
				}
			}

			state.Reset(windowStat);
		}

		windows[windowStat]?.SetActive(true);
		openables[windowStat]?.OnOpen();

		//RefreshButtons();

	}

	public void CloseUppermost()
	{
		ToolState uppermost = state.CloseUppermost();
		windows[uppermost]?.SetActive(false);
		openables[uppermost]?.OnClose();
		if(state.CurStat == ToolState.None)
		{
			CloseWindow();
		}
	}

	public void ChangeStatus(string name)
	{
		ChangeStatus((ToolState)System.Enum.Parse(typeof(ToolState), name));
	}

	public void CloseWindow()
	{
		ChangeStatus(ToolState.None);
	}

	void InitCloseAll()
	{
		foreach (var item in windows.Values)
		{
			item.SetActive(false);
		}
	}
	public void RefreshButtons()
	{
		
		//for (int i = 0; i < toolButtons.Count; i++)
		//{
		//	toolButtons[i].ResetButton();
		//	
		//	if (state != ToolState.None && toolButtons[i].indicating == state)
		//	{
		//		toolButtons[i].Focus();
		//	}
		//}
		//for (int i = 0; i < parents.Count; i++)
		//{
		//	parents[i].ParentButtonRefresh();
		//}
		
	}

	public void RefreshWindows()
	{
		foreach (var item in state.GetAll())
		{
			openables[item].Refresh();
		}
	}

}
