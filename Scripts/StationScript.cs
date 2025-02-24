using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationScript : MonoBehaviour
{
    [SerializeField] GameObject stationObject;
    // Update is called once per frame
    void Update()
    {
        FindAnyObjectByType<PlayerScript>().spawnTool(stationObject);
    }
}
