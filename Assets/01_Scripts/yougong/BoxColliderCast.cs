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
			_box.isTrigger = true;
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
	    Vector3 dir = Owner.forward;
	    dir.x *= _box.center.x;
	    dir.y *= _box.center.y;
	    dir.z *= _box.center.z;
	    //Debug.LogError(_box.size);
	    return Physics.OverlapBox(transform.position + dir, _box.size, Owner.rotation, Layer);
    }
    
}
