using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmingPoint : MonoBehaviour, IInterable
{
	public float interRange = 1f;
	public bool isInterable = true;
	public bool isDestroyed = true;

	Transform player;
	Collider col;
	RaycastHit hit;

	public bool IsInterable { get => isInterable; set => isInterable = value; }

	private void Awake()
	{
		col = GetComponent<Collider>();
	}

	public bool IsInRange()
	{
		if(player == null)
		{
			player = GameManager.instance.player.transform;
		}
		Vector3 dir = (player.position - transform.position).normalized;
		if(Physics.Raycast(transform.position, dir, out hit, interRange))
		{
			if(hit.collider.transform == player)
			{
				if((col.ClosestPoint(hit.point) - hit.point).magnitude <= interRange)
				{
					Debug.Log("CLOSEENOUGH");
					return true;
				}
				Debug.Log("TOOFAR");
			}
			Debug.Log("NOTPLAYER");
		}
		Debug.Log("NOTHING");
		return false;
	}

	public void InteractWith()
	{
		Debug.Log(transform.name);
		if (isDestroyed)
		{
			Destroy(gameObject); //임시. 이후 풀링으로 변경.
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(transform.position, interRange);
	}
}
