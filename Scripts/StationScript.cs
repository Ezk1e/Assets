using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationScript : MonoBehaviour
{
    [SerializeField] GameObject stationObject;
    [SerializeField] Transform characterLocation;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(FindAnyObjectByType<PlayerScript>().isInteractingWithObject());
        if(FindAnyObjectByType<PlayerScript>().isInteractingWithObject())
        {
            Vector3  spawnLocation = characterLocation.position + characterLocation.forward;
            Instantiate(stationObject, spawnLocation, characterLocation.rotation, characterLocation);
        }
    }
}
