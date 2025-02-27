using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScript : MonoBehaviour
{
    [SerializeField] Camera gameCamera;
    // Update is called once per frame
    void Update()
    {
        if (gameCamera != null)
        {
            Vector3 direction = gameCamera.transform.position - transform.position;
            direction.y = direction.z = 0.0f;
            transform.LookAt(gameCamera.transform.position - direction);
        }
    }
}
