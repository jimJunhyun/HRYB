using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingFoxFire : MonoBehaviour
{
	private Transform playerTr;

	[Header("Following")]
	public float XOffset = 0.4f;
	public float YOffset = 1.0f;
	public float ZOffset = 0.3f;

	public float FollowingSpeed = 1f;

	private Light light;
	[Header("Lighting")]
	public KeyCode LightKey = KeyCode.L;
	public float minEmissive, maxEmissive;
	public int MaxEmissiveLevel;
	[SerializeField]
	private int CurrentEmissiveLevel = 1; //1~Max

    void Awake()
    {
		if (playerTr == null) playerTr = GameObject.Find("Player").transform;    
		if(light == null) light = GetComponentInChildren<Light>();

    }

    void Update()
	{
		if (Input.GetKeyDown(LightKey))
		{
			if (CurrentEmissiveLevel + 1 > MaxEmissiveLevel) CurrentEmissiveLevel = 1;
			else CurrentEmissiveLevel++;

			SetEmissiveValue();
		}

		Following();

	}

	private void Following()
	{
		Vector3 targetPos = playerTr.TransformPoint(new Vector3(XOffset, YOffset, ZOffset));

		transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * FollowingSpeed);
	}
	private void SetEmissiveValue()
	{
		float targetValue = Mathf.Lerp(minEmissive, maxEmissive, ((float)CurrentEmissiveLevel-1f) / (float)(MaxEmissiveLevel-1f));
		light.intensity = targetValue;
	}
}
