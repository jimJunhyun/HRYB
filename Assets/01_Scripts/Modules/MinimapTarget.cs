using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MinimapRenderType
{
	MiniPlayer,
	MiniEnemy,
	MiniEliteEnemy,

}

public class MinimapTarget : Module
{
    public bool Rendered => (GameManager.instance.player.transform.position - transform.position).sqrMagnitude <= GameManager.instance.pActor.sight.GetSightRange() * GameManager.instance.pActor.sight.GetSightRange();
	public MinimapRenderType type;
	public bool ignoreAngle = false;
	bool prevRenderState;

	private void Start()
	{
		prevRenderState = false;
	}

	private void Update()
	{
		if (Rendered != prevRenderState)
		{
			prevRenderState = Rendered;
			if (prevRenderState)
			{
				GameManager.instance.minimap.AddRender(this);
			}
			else
			{
				GameManager.instance.minimap.RemoveRender(this);
			}
		}
	}
}
