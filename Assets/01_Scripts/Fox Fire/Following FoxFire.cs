using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public enum FoxFireMode
{
	FollowPlayer,
	Flying,
	Attatched,
}

public class FollowingFoxFire : MonoBehaviour
{
	private Transform playerTr;
	private Transform orbittingTr;

	FoxFireMode mode = FoxFireMode.FollowPlayer;

	[Header("Following")]
	public float XOffset = 0.4f;
	public float YOffset = 1.0f;
	public float ZOffset = 0.3f;

	public float FollowingSpeed = 1f;
	public float flyMaxTime = 15f;
	public float orbitRadius = 5f;
	public float orbitSpeed = 5f;

	private Vector3 flyDirPow;
	private float flyStartT;

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

		switch (mode)
		{
			case FoxFireMode.FollowPlayer:
				Following();
				break;
			case FoxFireMode.Flying:
				Flying();
				break;
			case FoxFireMode.Attatched:
				Orbitting();
				break;
			default:
				break;
		}

	}

	private void OnTriggerEnter(Collider other)
	{
		if(mode == FoxFireMode.Flying)
		{
			Actor actor = other.GetComponent<Actor>();
			if (actor && !(actor.move is PlayerMove))
			{
				Orbit(actor.transform);
			}
			else
			{
				Follow();
			}
		}
		
	}

	public void Fly(Vector3 dir, float pow)
	{
		mode = FoxFireMode.Flying;
		flyDirPow = dir * pow;
		flyStartT = Time.time;
	}

	public void Orbit(Transform trm)
	{
		mode = FoxFireMode.Attatched;
		orbittingTr = trm;
	}

	public void Follow()
	{
		mode = FoxFireMode.FollowPlayer;
	}

	private void Following()
	{
		Vector3 targetPos = playerTr.TransformPoint(new Vector3(XOffset, YOffset, ZOffset));

		transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * FollowingSpeed);
	}

	private void Flying()
	{
		if(Time.time - flyStartT < flyMaxTime)
		{
			transform.position += flyDirPow * Time.deltaTime;
		}
		else
		{
			Follow();
		}
		
	}

	private void Orbitting()
	{
		float rad = (Time.time * orbitSpeed) % 360 * Mathf.Deg2Rad;
		transform.position = orbittingTr.TransformPoint(new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * orbitRadius);
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
