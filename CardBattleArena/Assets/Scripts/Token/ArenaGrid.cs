using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaGrid : MonoBehaviour
{
    public Node[,] grid;
    int maxX;
    int maxY;

    public List<Node> path;

    private void Awake()
    {
        Node[] nodelist = FindObjectsOfType<Node>();
        maxX = 0;
        maxY = 0;

        foreach (Node node in nodelist)
        {
            if (node.xRow > maxX) { maxX = node.xRow; }
            if (node.yRow > maxY) { maxY = node.yRow; }
        }

        grid = new Node[maxX + 1, maxY + 1];

        foreach (Node node in nodelist)
        {
            grid[node.xRow, node.yRow] = node;
        }

    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if(x == 0 && y == 0) { continue; }


                int checkX = node.xRow + x;
                int checkY = node.yRow + y;

                if(checkX >= 0 && checkX <= maxX && checkY >= 0 && checkY <= maxY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }

            }
        }

        return neighbours;
    }

    public List<Node> GetNeighboursInRange(Node node, int range)
    {
        List<Node> neighbours = new List<Node>();

        int min = 0 - range; 
        int max = range;

        for (int x = min; x <= max; x++)
        {
            for (int y = min; y <= max; y++)
            {
                if (x == 0 && y == 0) { continue; }


                int checkX = node.xRow + x;
                int checkY = node.yRow + y;

                if (checkX >= 0 && checkX <= maxX && checkY >= 0 && checkY <= maxY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }

            }
        }

        return neighbours;
    }

    public int GetDistance(Node nodeA, Node nodeB)
    {

        int distanceX = Mathf.Abs(nodeA.xRow - nodeB.xRow);
        int distanceY = Mathf.Abs(nodeA.yRow - nodeB.yRow);

        if (distanceX > distanceY) { return 14 * distanceY + 10 * (distanceX - distanceY); } else { return 14 * distanceX + 10 * (distanceY - distanceX); }

    }

    public List<Node> GetNeighboursInRangeVariableSize(Node node, int rangeXMin, int rangeXMax, int rangeYMin, int rangeYMax)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = rangeXMin; x <= rangeXMax; x++)
        {
            for (int y = rangeYMin; y <= rangeYMax; y++)
            {
                if (x == 0 && y == 0) { continue; }


                int checkX = node.xRow + x;
                int checkY = node.yRow + y;

                if (checkX >= 0 && checkX <= maxX && checkY >= 0 && checkY <= maxY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }

            }
        }

        return neighbours;
    }

}
