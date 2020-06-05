using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Utils : MonoBehaviour
{
    public static int SortByHealth(TokenManager t1, TokenManager t2)
    {
        return t1.currentHealth.CompareTo(t2.currentHealth);
    }

    public static int SortByDistanceToNode(Node n1, Node n2)
    {
        return n1.pathfinderDistanceToNode.CompareTo(n2.pathfinderDistanceToNode);
    }


}
