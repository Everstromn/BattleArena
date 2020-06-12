using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "Level")]
public class SO_Level : ScriptableObject
{
    public string levelName;
    public int sceneIndex;
    [TextArea(5, 10)] public string levelDesc;
}
