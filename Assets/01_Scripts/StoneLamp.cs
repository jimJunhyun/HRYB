using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;

public class StoneLamp : MonoBehaviour
{
	MeshRenderer meshRenderer;
	Vector3 distance;
	Color color;

	Material material;

	private void Awake()
	{
		meshRenderer = GetComponentInChildren<MeshRenderer>();
		material = new Material(meshRenderer.material);

		meshRenderer.material = material;

		color = material.GetColor("_EmissionColor");
	}

	void Start()
    {
		material.DisableKeyword("_EMISSION");
    }

	private void Update()
	{
		if(Vector3.Distance(GameManager.instance.player.transform.position, this.transform.position) < 10)
		{
			material.EnableKeyword("_EMISSION");
			material.SetColor("_EmissionColor", color * Mathf.Clamp(Mathf.Lerp(-10, 3, 0.05f), -10, 3));
		}
	}

}
