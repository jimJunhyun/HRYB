using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

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
	public float SmoothTime = 1f;
	public int MaxEmissiveLevel;
	[SerializeField]
	private int CurrentEmissiveLevel = 1; //1~Max
	public bool isChanging = false;
	private Transform effect;

	private WaitForSeconds wait = new WaitForSeconds(0.1f);
	private WaitForSeconds stopWait = new WaitForSeconds(1f);

    void Awake()
    {
		if (playerTr == null) playerTr = GameObject.Find("Player").transform;
		if(light == null) light = GetComponentInChildren<Light>();
		if (effect == null) effect = transform.GetChild(0);

    }

    void Update()
	{
		if (Input.GetKeyDown(LightKey) && !isChanging)
		{
			if (CurrentEmissiveLevel + 1 > MaxEmissiveLevel) CurrentEmissiveLevel = 0;
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
		float targetValue = CurrentEmissiveLevel == 0 ? 0 : Mathf.Lerp(minEmissive, maxEmissive, ((float)CurrentEmissiveLevel-1f) / (float)(MaxEmissiveLevel-1f));
		StartCoroutine(SmoothEmissive(targetValue));
	}

	private IEnumerator SmoothEmissive(float value)
	{
		isChanging = true;
		var particle = effect.GetComponent<VisualEffect>();
		if (particle.pause) particle.Play();

		float t = 0;
		float i = light.intensity;

		while(t < SmoothTime)
		{
			light.intensity = Mathf.Lerp(i, value, t / SmoothTime);
			yield return wait;
			t += 0.1f;
		}

		if (CurrentEmissiveLevel == 0)
		{
			//particle.Stop();
			yield return stopWait;
			
		}
		isChanging = false;
	}
}
