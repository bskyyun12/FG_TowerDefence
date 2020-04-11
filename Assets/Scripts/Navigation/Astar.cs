using System;
using UnityEngine;
using System.Collections.Generic;

namespace AI
{
	public class Astar
	{
		public static Node[,] grid;
		static List<Node> nodes = new List<Node>();
		static List<Node> pathList = new List<Node>();
		static List<Node> neighbours = new List<Node>();

		public static void SetGrid(Vector2Int size)
		{
			grid = new Node[size.x, size.y];
		}

		public static void AddNode(Vector3 worldPosition, int x, int y, bool walkable)
		{
			grid[x, y] = new Node(worldPosition, x, y, walkable);
			nodes.Add(grid[x, y]);
		}

		public static List<Node> Neighbours(Node node)
		{
			neighbours.Clear();

			for (int x = -1; x <= 1; x++)
			{
				for (int y = -1; y <= 1; y++)
				{
					if (x == 0 && y == 0
						|| x == -1 && y == 1
						|| x == -1 && y == -1
						|| x == 1 && y == 1
						|| x == 1 && y == -1
						)
					{
						continue;
					}

					int checkX = node.grid.x + x;
					int checkY = node.grid.y + y;

					if (InBounds(checkX, checkY))
					{
						Node neighbour = grid[checkX, checkY];
						if (IsNodeWalkable(neighbour))
						{
							neighbours.Add(neighbour);
						}
					}
				}
			}
			return neighbours;
		}

		public static bool IsNodeWalkable(Node node)
		{
			return node.walkable;
		}

		public static bool InBounds(int x, int y)
		{
			return x >= 0 && x < grid.GetLength(0) && y >= 0 && y < grid.GetLength(1);
		}

		static Node FindNode(Vector3 worldPosition)
		{
			//foreach (Node n in nodes)
			foreach (Node n in nodes)
			{
				if (n.GetWorldPosition() == worldPosition)
					return n;
			}
			return null;
		}

		public static int GetPathLength()
		{
			return pathList.Count;
		}

		public static Vector3 GetWorldPosition(int index)
		{
			return pathList[index].worldPosition;
		}

		//public void PrintPath()
		//{
		//	foreach (Node n in pathList)
		//	{
		//		Debug.Log(n.id.name);
		//	}
		//}

		public static IEnumerable<Node> FindPath(Vector3 startPos, Vector3 endPos)
		{
			Node startNode = FindNode(startPos);
			Node endNode = FindNode(endPos);

			if (startNode == null || endNode == null)
			{
				return pathList;
			}

			List<Node> open = new List<Node>();
			List<Node> closed = new List<Node>();
			float tentative_g_score = 0;
			bool tentative_is_better;

			startNode.g = 0;
			startNode.h = Distance(startNode, endNode);
			startNode.f = startNode.h;
			open.Add(startNode);

			while (open.Count > 0)
			{
				int i = LowestF(open);
				Node thisnode = open[i];
				if (thisnode == endNode)  //path found
				{
					ReconstructPath(startNode, endNode);
					return pathList;
				}

				open.RemoveAt(i);
				closed.Add(thisnode);

				foreach (Node neighbour in Neighbours(thisnode))
				{
					neighbour.g = thisnode.g + Distance(thisnode, neighbour);

					if (closed.IndexOf(neighbour) > -1)
						continue;

					tentative_g_score = thisnode.g + Distance(thisnode, neighbour);

					if (open.IndexOf(neighbour) == -1)
					{
						open.Add(neighbour);
						tentative_is_better = true;
					}
					else if (tentative_g_score < neighbour.g)
					{
						tentative_is_better = true;
					}
					else
						tentative_is_better = false;

					if (tentative_is_better)
					{
						neighbour.cameFrom = thisnode;
						neighbour.g = tentative_g_score;
						neighbour.h = Distance(thisnode, endNode);
						neighbour.f = neighbour.g + neighbour.h;
					}
				}
			}

			return pathList;
		}

		public static void ReconstructPath(Node startId, Node endId)
		{
			pathList.Clear();
			pathList.Add(endId);

			var parent = endId.cameFrom;
			while (parent != startId && parent != null)
			{
				pathList.Insert(0, parent);
				parent = parent.cameFrom;
			}
			pathList.Insert(0, startId);
		}

		static float Distance(Node a, Node b)
		{
			int dstX = Mathf.Abs(a.grid.x - b.grid.x);
			int dstY = Mathf.Abs(a.grid.y - b.grid.y);

			if (dstX > dstY)
				return 14 * dstY + 10 * (dstX - dstY);
			return 14 * dstX + 10 * (dstY - dstX);
		}

		static int LowestF(List<Node> l)
		{
			float lowestf = 0;
			int count = 0;
			int iteratorCount = 0;

			for (int i = 0; i < l.Count; i++)
			{
				if (i == 0)
				{
					lowestf = l[i].f;
					iteratorCount = count;
				}
				else if (l[i].f <= lowestf)
				{
					lowestf = l[i].f;
					iteratorCount = count;
				}
				count++;
			}
			return iteratorCount;
		}
	}
}
