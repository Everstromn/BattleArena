using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialsManager : MonoBehaviour
{

    public static MaterialsManager instance;
    private void Awake()
    {

        int gameManagerCount = FindObjectsOfType<MaterialsManager>().Length;

        if (gameManagerCount > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }

        if (instance != null)
        {
            Debug.LogError("More than one Materials Manager");
        }
        instance = this;
    }

    public Material baseTokenMaterial;
    
    public Material blueTeamCreature;
    public Material blueTeamCreatureAction;
    public Material blueTeamBuilding;
    
    public Material redTeamCreature;
    public Material redTeamCreatureAction;
    public Material redTeamBuilding;

    public Material nodeBasic;
    public Material nodeUnwalkable;

    public Material nodeBlueBuilding;
    public Material nodeBlueCreature;

    public Material nodeRedBuilding;
    public Material nodeRedCreature;

    public Material nodeWalkNow;
    public Material nodeWalkLater;
}
