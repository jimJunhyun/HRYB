using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum AreaShapeMode
{
	None,
	Ellipse,
	Rectangle,
	Triangle,

}

public interface IRangeSelect
{
	public AreaShapeMode shape {get; set;}
	public Vector3 selectPoint { get;set;}
	public Vector2 XYDiameter { get;set;}

	public bool IsInside(Vector3 v);

}
