using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{	

	private void LateUpdate()
	{
		Vector3 dirFromCamera = transform.position - Camera.main.transform.position;
		transform.LookAt(transform.position + dirFromCamera);
	}
}
