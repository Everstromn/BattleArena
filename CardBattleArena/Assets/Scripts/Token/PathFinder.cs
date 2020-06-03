using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    private ArenaGrid myGrid;

    List<Node> openSet = new List<Node>();
    List<Node> closedSet = new List<Node>();

    private void Awake()
    {
        myGrid = FindObjectOfType<ArenaGrid>();
    }

    public List<Node> FindPath(Node startNode, Node targetNode)
    {
        List<Node> newPath = new List<Node>();

        openSet.Clear();
        closedSet.Clear();
        
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];

            for (int i = 0; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if(currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            foreach (Node neighbourNode in myGrid.GetNeighbours(currentNode))
            {
                if(!neighbourNode.walkable || neighbourNode.occupied || closedSet.Contains(neighbourNode)) { continue; }

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbourNode);
                if (newMovementCostToNeighbour < neighbourNode.gCost || !openSet.Contains(neighbourNode))
                {
                    neighbourNode.gCost = newMovementCostToNeighbour;
                    neighbourNode.hCost = GetDistance(neighbourNode, targetNode);
                    neighbourNode.parentNode = currentNode;
                    if(!openSet.Contains(neighbourNode)) { openSet.Add(neighbourNode); }
                }

            }

        }

        return newPath;
    }

    private int GetDistance(Node nodeA, Node nodeB)
    {

        int distanceX = Mathf.Abs(nodeA.xRow - nodeB.xRow);
        int distanceY = Mathf.Abs(nodeA.yRow - nodeB.yRow);

        if (distanceX > distanceY) {return 14 * distanceY + 10 * (distanceX - distanceY); } else { return 14 * distanceX + 10 * (distanceY - distanceX); }

    }

    private List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parentNode;
        }

        path.Reverse();

        return path;
    }
}
