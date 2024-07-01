using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JungGI : MonoBehaviour
{
	public float _delayTime = 0.5f;
	public float _gravityValue = 0.8f;
	 float groundThreshold = 0.5f;
	bool _init = false;
	bool _followPlayer = false;
	bool _goPlayer = false;

	float mpValue = 0;
	Transform tls;

	public void Init(Vector3 enemyPos, float t)
	{
		float x = Random.Range(-4.0f, 4.0f);
		float y = Random.Range(4f, 6f);
		float z = Random.Range(-4.0f, 4.0f);
		transform.parent = null;


		forceDir = new Vector3(x,y,z);
		_init = true;
		_goPlayer = false;
		_followPlayer = false;
		mpValue = t;
		tls = GameManager.instance.player.transform;
	}

	private void FixedUpdate()
	{
		if(_init && _followPlayer == false)
			MoveSet();
		if(_goPlayer == true)
		{
			transform.position += (tls.position - transform.position) * 9 * Time.deltaTime;
		}
	}


	private Vector3 fDir;
	public virtual Vector3 forceDir
	{
		get
		{
			return fDir;
		}
		set
		{
			fDir = value;
		}
	}


	public virtual void MoveSet()
	{
		if (!isGrounded)
		{
			forceDir -= Vector3.up * GameManager.GRAVITY * _gravityValue * Time.deltaTime;
		}
		transform.Translate(forceDir * Time.fixedDeltaTime, Space.World);
	}
	IEnumerator FollowingDelay(float t)
	{
		if (gameObject.TryGetComponent<ColliderCast>(out ColliderCast cols))
		{
			cols.Now(transform, (_life) =>
			{
				if (_life.TryGetComponent<PlayerLife>(out PlayerLife pl))
				{

					pl.StartCoroutine(pl.JunGIUP(cols, mpValue));

				}
			});
		}

		yield return new WaitForSeconds(t);
		_goPlayer = true;
	}


	public virtual bool isGrounded
	{
		get
		{
			if (Physics.Raycast(transform.position + new Vector3(0, 0.1f, 0), Vector3.down, groundThreshold, 1 << GameManager.GROUNDLAYER))
			{
				StartCoroutine(FollowingDelay(_delayTime));
				_followPlayer = true;
				return true;
			}
			return false;
		}
	}
}
