using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationScript : MonoBehaviour
{
    [SerializeField] GameObject stationObject;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        FindAnyObjectByType<PlayerScript>().spawnTool(stationObject);
    }
}
