using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Mechanic", menuName = "Mechanic")]
public class SO_Mechanic : ScriptableObject
{
    public string mechanicName;
    [TextArea(5, 10)] public string mechanicDescription;

}
