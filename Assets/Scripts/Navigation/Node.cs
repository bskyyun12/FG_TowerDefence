using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Node
{
	public float f, g, h;
	public Node cameFrom;

	public Vector3 worldPosition;
	public Vector2Int grid;
	public bool walkable;

	public Node(Vector3 worldPosition, int x, int y, bool walkable)
	{
		this.worldPosition = worldPosition;
		grid.x = x;
		grid.y = y;
		this.walkable = walkable;
	}

	public Vector3 GetWorldPosition()
	{
		return worldPosition;
	}
}
