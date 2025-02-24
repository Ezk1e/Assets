using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointBScript : MonoBehaviour
{
    [SerializeField] GameObject stationObject;
    [SerializeField] Transform[] interactionPoints;
    PlayerScript player;
    GameObject playerItem;
    bool isInteract = false;
    bool itemInserted = false;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        playerItem = player.getItemHolding();
    }

    void OnTriggerEnter(Collider other)
    {
        if(playerItem != null)
        {
            if(other.tag == "User")
            {
                if(stationObject.tag == playerItem.tag)
                {
                    isInteract = true;
                }
            }
        }

        if(itemInserted)
        {
            if(other.tag == "User")
            {
                isInteract = true;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "User")
        {
            isInteract = false;
        }
    }

    public bool doGen(GameObject playerObject, GameObject avatarObject)
    {
        if(isInteract)
        {
            itemInserted = true;
            if(playerItem != null)
            {
                Destroy(playerItem);
            }
            MovePlayerToInteractionPoint(playerObject, avatarObject);

            return true;
        }

        return false;
    }

    void MovePlayerToInteractionPoint(GameObject playerObject, GameObject avatarObject)
    {
        if(interactionPoints.Length > 0)
        {
            Transform closestPoint = interactionPoints[0];
            float closestDistance = Vector3.Distance(playerObject.transform.position, interactionPoints[0].position);

            foreach(Transform point in interactionPoints)
            {
                float distance = Vector3.Distance(playerObject.transform.position, point.position);
                if(distance < closestDistance)
                {
                    closestPoint = point;
                    closestDistance = distance;
                }
            }

            playerObject.transform.position = closestPoint.position;
            avatarObject.transform.rotation = Quaternion.LookRotation(transform.position);
        }
    }
}
