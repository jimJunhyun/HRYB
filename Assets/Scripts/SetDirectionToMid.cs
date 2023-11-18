using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetDirectionToMid : MonoBehaviour
{
    Camera target;
	public float dist = 100f;

	private void Awake()
	{
		target = Camera.main;
	}

	// Update is called once per frame
	void Update()
    {
        transform.LookAt(target.transform.position + target.transform.forward * dist);
    }

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawRay(transform.position, transform.forward * dist);
	}
}
