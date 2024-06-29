using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolidFollow : MonoBehaviour
{
    public bool fixX;
    public bool fixY;
    public bool fixZ;

	Vector3 offset;

	public Transform target;

	private void Awake()
	{
		offset = transform.position - target.position;
		if(!fixX)
			offset.x = 0;
		if (!fixY)
			offset.y = 0;
		if (!fixZ)
			offset.z = 0;
	}

	// Update is called once per frame
	void LateUpdate()
    {
        transform.position = target.position + offset;
    }
}
