using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AISetter : MonoBehaviour
{
	protected Actor self;
	protected Actor player;
	
	protected Selecter head;
	
	protected bool stopped = false;
	
	
	
    // Start is called before the first frame update
    void Start()
    {
	    player = GameManager.instance.pActor;

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
	    if (!stopped)
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

    public void StartExamine()
    {
	    stopped = false;
    }
}
