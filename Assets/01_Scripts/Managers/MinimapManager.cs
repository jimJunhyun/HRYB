using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class MinimapManager : MonoBehaviour
{
	
	public const int MINIMAPTEXSIZE = 300;

	MinimapShower shower;

	List<MinimapTarget> targs = new List<MinimapTarget>();
	List<GameObject> icons = new List<GameObject>();

	private void Awake()
	{
		shower = GameObject.Find("MinimapContent").GetComponent<MinimapShower>();
	}

	private void LateUpdate()
	{
		for (int i = 0; i < targs.Count; i++)
		{
			Vector3 offset = targs[i].transform.position - GameManager.instance.player.transform.position;
			float xDiff = MINIMAPTEXSIZE * 0.5f * (offset.x / GameManager.instance.pActor.sight.GetSightRange());
			float yDiff = MINIMAPTEXSIZE * 0.5f * (offset.z / GameManager.instance.pActor.sight.GetSightRange());
			if(!targs[i].ignoreAngle)
				icons[i].transform.rotation = Quaternion.Euler(0, 0, -360 - targs[i].transform.eulerAngles.y);
			else
				icons[i].transform.rotation = Quaternion.identity;
			icons[i].transform.localPosition = new Vector3(xDiff, yDiff, 0);
		}
	}


	public void DoRender()
	{
		ClearRender();
		for (int k = 0; k < targs.Count; k++)
		{
			Vector3 offset = targs[k].transform.position - GameManager.instance.player.transform.position;
			float xDiff = MINIMAPTEXSIZE * 0.5f * (offset.x / GameManager.instance.pActor.sight.GetSightRange());
			float yDiff = MINIMAPTEXSIZE * 0.5f * (offset.z / GameManager.instance.pActor.sight.GetSightRange());
			Quaternion q = Quaternion.Euler(0, 0, targs[k].transform.eulerAngles.y);

			GameObject icon = PoolManager.GetObject(targs[k].type.ToString(), shower.transform);

			icons.Add(icon);
		}
	}

	public void ClearRender()
	{
		for (int i = 0; i < icons.Count; i++)
		{
			PoolManager.ReturnObject(icons[i]);
		}
		icons.Clear();
	}

	public void AddRender(MinimapTarget targ)
	{
		targs.Add(targ);
		DoRender();
	}

	public void RemoveRender(MinimapTarget targ)
	{
		targs.Remove(targ);
		DoRender();
	}
}
