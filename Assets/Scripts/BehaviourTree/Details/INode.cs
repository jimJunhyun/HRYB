using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeStatus
{
	Run,
	Sucs,
	Fail,
}

public interface INode
{
    public NodeStatus Examine();
}
