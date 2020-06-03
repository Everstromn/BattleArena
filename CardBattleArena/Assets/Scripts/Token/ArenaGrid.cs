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



}
