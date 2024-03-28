using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AISetter : MonoBehaviour
{
	protected Actor self;
	[SerializeField] Actor _player;

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

	public virtual void LookAt(Transform t)
	{
		Vector3 lookPos = t.position - transform.position;
		lookPos.y = transform.position.y;
		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookPos), Time.deltaTime * 40);
	}



	// Start is called before the first frame update
	void Start()
    {

	    self = GetComponent<Actor>();
	    head = new Selecter();
		StartInvoke();
    }

    /// <summary>
    /// Same To Start
    /// </summary>
    protected abstract void StartInvoke();

    // Update is called once per frame
    private void Update()
    {
	    if (!stopped && head != null)
	    {
		    head.Examine();
	    }
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
}
