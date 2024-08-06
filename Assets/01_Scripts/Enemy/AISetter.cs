using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AISetter : MonoBehaviour
{
	protected Actor self;
	[SerializeField] Actor _player;
	[SerializeField] SkinnedMeshRenderer _skinned;
	public bool IsNotStarted = false;

	public Actor player
	{
		get
		{
			if(_player == null)
			{
				_player = FindObjectOfType<PlayerInter>().GetComponent<Actor>();

			}
			return _player;
		}
	}
	
	protected Selecter head;
	
	protected bool stopped = false;
	public bool StopState => stopped;


	public void IsNotAwake()
	{
		if(IsNotStarted == false)
		{
			StartInvoke();
		}
		self.life._hitEvent -= IsNotAwake;
	}

	public virtual void LookAt(Transform t)
	{
		Vector3 lookPos = t.position - transform.position;
		lookPos.y = transform.position.y;
		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookPos), Time.deltaTime * 40);

		if(self.move.isGrounded) 
			transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
	}

	public virtual void DieEvent(float delay = 0, float time =3)
	{
		for(int i = 0; i < _skinned.materials.Length;i++)
		{
			_skinned.materials[i].SetInt("_IsDissolve", 1);
			_skinned.materials[i].SetFloat("_DissolveHeight", 5);
			StartCoroutine(DissolveMat(_skinned.materials[i], delay, time));
		}
		Debug.LogError("Dieing");

	}

	IEnumerator DissolveMat(Material ms, float delay, float time)
	{
		yield return new WaitForSeconds(delay);
		float t = 0;
		while (t < time)
		{
			t += Time.deltaTime;
			ms.SetFloat("_DissolveHeight", Mathf.Lerp(5,-5, t / 3.0f));
			//Debug.LogError(_skinned.materials[0].GetInteger("_IsDissolve")	+ " + " +Mathf.Lerp(5,0, t / 3.0f) +" 돼잖앗 ㅣ발");
			yield return null;
		}
		Destroy(this.gameObject);
	}

	// Start is called before the first frame update
	void Start()
    {
	    //_skinned.materials[0].SetInteger("_IsDissolve", 0);
	    self = GetComponent<Actor>();
	    head = new Selecter();
		self.life._hitEvent += IsNotAwake;
		StartInvoke();
    }

	/// <summary>
	/// Same To Start
	/// </summary>
	public abstract void StartInvoke();

    // Update is called once per frame
    protected virtual void Update()
    {
	    if (!stopped && head != null)
	    {
		    head.Examine();
	    }
		else
		{
			(self.move as EnemyMoveModule).StopMove();
		}

		//if(stopEnAble >= 0.5f)
		//{
		//	StartExamine();
		//}

	    UpdateInvoke();
    }

    /// <summary>
    /// Same to Update
    /// </summary>
    protected abstract void UpdateInvoke();

    public void StopExamine()
    {
	    stopped = true;
	}

    public virtual void StartExamine()
    {
	    stopped = false;
    }

	public virtual void ResetStatus()
	{

	}
}
