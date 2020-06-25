using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MechanicPrefab : MonoBehaviour
{
    public SO_Mechanic myMechanic;
    [SerializeField] private TMP_Text mechanicNameDisplay = null;

    public void OnSpawn(SO_Mechanic _mech)
    {
        myMechanic = _mech;
        mechanicNameDisplay.text = myMechanic.mechanicName;
    }

    public void ShowButton()
    {
        FindObjectOfType<Glossary>().UpdateDescription(myMechanic.mechanicDescription);
    }
}
