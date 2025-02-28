using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationScript : MonoBehaviour
{
    [SerializeField] GameObject stationObject;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "User")
        {
            PlayerScript player = other.gameObject.GetComponent<PlayerScript>();
            if (player != null)
            {
                player.SetCurrentStation(this);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "User")
        {
            PlayerScript player = other.gameObject.GetComponent<PlayerScript>();
            if (player != null)
            {
                player.SetCurrentStation(null);
            }
        }
    }

    public GameObject spawnTool(Transform itemLocation)
    {
        GameObject item = Instantiate(stationObject, itemLocation.transform.position, itemLocation.transform.rotation, itemLocation.transform);
        Rigidbody hasItemRigid = item.GetComponent<Rigidbody>();
        hasItemRigid.isKinematic = true;

        return item;
    }
}
