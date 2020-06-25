using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Glossary : MonoBehaviour
{
    public SO_Mechanic[] mechanics;

    [SerializeField] private GameObject prefabMechanic = null;
    [SerializeField] private GameObject mechanicContainer = null;

    [SerializeField] private TMP_Text descriptionObj = null;

    public void CloseGlossary() { gameObject.SetActive(false); }

    public void OnLoad()
    {
        //Clear the current Mechanics
        foreach (Transform mech in mechanicContainer.transform) { Destroy(mech.gameObject); }

        //ForEach mechanic, create a new prefab
        foreach (SO_Mechanic mech in mechanics)
        {
            GameObject newMech = Instantiate(prefabMechanic, mechanicContainer.transform);
            newMech.GetComponent<MechanicPrefab>().OnSpawn(mech);
        }

    }

    public void UpdateDescription(string _text)
    {
        descriptionObj.text = _text;
    }


}
