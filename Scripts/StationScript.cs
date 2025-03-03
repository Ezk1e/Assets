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
            Player2Script player2 = other.gameObject.GetComponent<Player2Script>();
            if (player != null)
            {
                player.SetCurrentStation(this);
            }
            if (player2 != null)
            {
                player2.SetCurrentStation(this);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "User")
        {
            PlayerScript player = other.gameObject.GetComponent<PlayerScript>();
            Player2Script player2 = other.gameObject.GetComponent<Player2Script>();
            if (player != null)
            {
                player.SetCurrentStation(null);
            }
            if (player2 != null)
            {
                player2.SetCurrentStation(null);
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
