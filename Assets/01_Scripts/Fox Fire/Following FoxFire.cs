using System;
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
	private Actor orbitTarget;


	FoxFireMode mode = FoxFireMode.FollowPlayer;
	public FoxFireMode Mode
	{
		get=>mode;
		set=>mode = value;
	}

	[Header("Following")]
	public float XOffset = 0.4f;
	public float YOffset = 1.0f;
	public float ZOffset = 0.3f;

	public float FollowingSpeed = 1f;
	public float flyMaxTime = 15f;
	public float orbitMaxTime =7f;
	public float orbitRadius = 5f;
	public float orbitSpeed = 5f;

	public float maxDistance = 10;

	public float orbitJitterFreq = 1.3f;
	public float orbitJitterPower = 0.3f;

	public float maxAccDmg = 10000;
	public float dmgRate = 0.2f;
	public float maxExpDmg = 2000;

	private float accDmg = 0;
	private Vector3 flyDirPow;
	private float startT;

	private Rigidbody rig;
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
	private SkillRoot prevLClickAttack;

    void Awake()
    {
		if (playerTr == null) playerTr = GameObject.Find("Player").transform;
		if(light == null) light = GetComponentInChildren<Light>();
		if (effect == null) effect = transform.GetChild(0);
		if(rig == null) rig = GetComponent<Rigidbody>();
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
		startT = Time.time;
		if (orbitTarget)
		{
			orbitTarget.life.RemoveAllStatEff(StatEffID.FoxBewitched);
		}

		orbitTarget = null;
	}

	public void Orbit(Transform trm)
	{
		Actor target = trm.GetComponent<Actor>();
		if (target)
		{
			orbitTarget = target;
			startT = Time.time;
			mode = FoxFireMode.Attatched;

			StatusEffects.ApplyStat(target, GameManager.instance.pActor, StatEffID.FoxBewitched, -1);
			prevLClickAttack = (GameManager.instance.pActor.cast as PlayerCast).ConnectSkillDataTo(GameManager.instance.skillLoader.GetHumenSkill("ExplodeFoxFire"), SkillSlotInfo.LClick, PlayerForm.Magic);
			Debug.Log($"CHANGED ATK TO : explode FROM : {prevLClickAttack}");
		}
	}

	public void Explode()
	{
		float dmg = Mathf.Clamp(accDmg * dmgRate, 1, maxExpDmg);
		Actor ac = orbitTarget;
		Follow();

		GameManager.instance.shower.GenerateDamageText(transform.position, dmg, YYInfo.White);
		ac.life.DamageYY(0, dmg, DamageType.NoEvadeHit, 0, 0, GameManager.instance.pActor);
		
		Debug.Log($"펑. {dmg}댐");
		

	}

	public void Follow()
	{
		mode = FoxFireMode.FollowPlayer;
		if (orbitTarget)
		{
			orbitTarget.life.RemoveAllStatEff(StatEffID.FoxBewitched);
		}
		if (prevLClickAttack)
		{
			(GameManager.instance.pActor.cast as PlayerCast).ConnectSkillDataTo(prevLClickAttack, SkillSlotInfo.LClick, PlayerForm.Magic);
			Debug.Log($"ATTACK ROLLBACK TO : {prevLClickAttack}");
			prevLClickAttack = null;
		}
		accDmg = 0;
		orbitTarget = null;
	}

	private void Following()
	{
		Vector3 targetPos = playerTr.TransformPoint(new Vector3(XOffset, YOffset, ZOffset));

		transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * FollowingSpeed);
	}

	private void Flying()
	{
		if(Time.time - startT < flyMaxTime && (GameManager.instance.pActor.transform.position - transform.position).sqrMagnitude < maxDistance * maxDistance )
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
		float rad2 = (Time.time * orbitSpeed * orbitJitterFreq) % 360 * Mathf.Deg2Rad;
		transform.position = orbitTarget.transform.TransformPoint(new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * orbitRadius + Vector3.up * (((Mathf.Cos(rad2) + Mathf.Sin(rad2)) * orbitJitterPower) + orbitTarget.transform.localScale.y * 0.5f));
		if(Time.time - startT > orbitMaxTime || (GameManager.instance.pActor.transform.position - transform.position).sqrMagnitude > maxDistance * maxDistance)
		{
			Follow();
		}
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

	internal void Accumulate(YinYang dmg)
	{
		if(mode == FoxFireMode.Attatched)
		{
			Debug.Log("DAMAGE ADDED : " + dmg.white);
			accDmg += dmg.white;
			if(accDmg > maxAccDmg)
			{
				Explode();
			}
		}
	}
}
