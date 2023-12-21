using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxColliderCast : ColliderCast
{
	
	private BoxCollider _box;
	private void Awake()
	{
		try
		{
			_box = GetComponent<BoxCollider>();
		}
		catch
		{
			Debug.LogError($"Is it not Proper Collider! : BoxCollider => {gameObject.name}");
		}

		if (transform.localScale != Vector3.one)
		{
			Debug.LogError($"Object : {gameObject.name} is Not Scale Vector.oen(1,1,1)");
            
		}
	}

	// Update is called once per frame
    public override Collider[] ReturnColliders()
    {
	    Debug.LogError(_box.size);
	    return Physics.OverlapBox(transform.position + _box.center, _box.size, transform.rotation, Layer);
    }
    
}
