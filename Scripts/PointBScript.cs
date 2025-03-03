using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PointBScript : MonoBehaviour
{
    [SerializeField] GameObject stationObject;
    [SerializeField] Transform[] interactionPoints;
    [SerializeField] float activityTime = 5;
    float activityCounter = 0;
    PlayerScript player;
    Player2Script player2;
    GameObject playerItem;
    GameObject playerItem2;
    bool isInteract = false;
    bool itemInserted = false;
    bool activityDone = false;

    [SerializeField] Image pointerImage;
    [SerializeField] Transform pointA; // Reference to the starting point
    [SerializeField] Transform pointB; // Reference to the ending point
    [SerializeField] RectTransform safeZone; // Reference to the safe zone RectTransform
    [SerializeField] float moveSpeed = 5; // Speed of the pointer movement
    [SerializeField] float skillCheckInterval = 10;
    float skillCheckTimer = 0;
    RectTransform pointerTransform;
    Vector3 targetPosition;
    [SerializeField] bool skillCheckActive = false;
    [SerializeField] bool skillCheckSuccess = false;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerScript>();
        player2 = FindObjectOfType<Player2Script>();
        pointerTransform = pointerImage.GetComponent<RectTransform>();
        targetPosition = pointB.position;
        pointerTransform.position = pointA.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(activityDone)
        {
            return;
        }

        playerItem = player.getItemHolding();
        playerItem2 = player2.getItemHolding();
    }

    void FixedUpdate()
    {
        if (skillCheckActive)
        {
            pointerTransform.position = Vector3.MoveTowards(pointerTransform.position, targetPosition, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(pointerTransform.position, pointB.position) < 0.1f)
            {
                EndSkillCheck(false);
            }
        }
    }

    #region Activity Functions
    void OnTriggerEnter(Collider other)
    {
        if(activityDone)
        {
            return;
        }

        if (other.gameObject.tag == "User")
        {
            if (player != null)
            {
                player.SetCurrentActivity(this);
            }
            if (player2 != null)
            {
                player2.SetCurrentActivity(this);
            }
        }

        playerItem = player.getItemHolding();
        playerItem2 = player2.getItemHolding();
        
        if (playerItem != null && other.tag == "User")
        {
            if (stationObject.tag == playerItem.tag)
            {
                isInteract = true;
            }
        }

        if (playerItem2 != null && other.tag == "User")
        {
            if (stationObject.tag == playerItem2.tag)
            {
                isInteract = true;
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
        if(activityDone)
        {
            return;
        }

        if (other.gameObject.tag == "User")
        {
            if (player != null)
            {
                player.SetCurrentActivity(null);
            }

            if (player2 != null)
            {
                player2.SetCurrentActivity(null);
            }
        }
        
        if (other.tag == "User")
        {
            isInteract = false;
        }
    }

    public bool doGen(GameObject playerObject, GameObject avatarObject)
    {
        playerItem = player.getItemHolding();
        if (isInteract && !activityDone)
        {
            itemInserted = true;
            Destroy(playerItem);
            MovePlayerToInteractionPoint(playerObject, avatarObject);

            return true;
        }

        return false;
    }

    public bool doGen2(GameObject playerObject, GameObject avatarObject)
    {
        playerItem2 = player2.getItemHolding();
        if (isInteract && !activityDone)
        {
            itemInserted = true;
            Destroy(playerItem2);
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
    #endregion

    #region Skill Check Functions
    public void SkillCheckProgress(bool isInteract)
    {
        if (isInteract)
        {
            skillCheckTimer += Time.deltaTime;

            if (skillCheckTimer >= skillCheckInterval)
            {
                skillCheckTimer = 0;
                StartSkillCheck();
            }

            if(skillCheckSuccess)
            {
                activityCounter = activityCounter + (.2f * activityTime);

                ResetSkillCheck();
            }

            activityCounter += Time.deltaTime;
            if(activityCounter >= activityTime)
            {
                activityDone = true;
                PlayerScript.isInteractingPointB = false;
                Player2Script.isInteractingPointB = false;
                MeshRenderer activityMesh = GetComponent<MeshRenderer>();
                activityMesh.material.color = Color.green;
            }

            Debug.Log("Activity Counter: " + activityCounter);
        }
    }
    public void StartSkillCheck()
    {
        skillCheckSuccess = false;
        skillCheckActive = true;
        pointerTransform.position = pointA.position;
        targetPosition = pointB.position;
    }

    public void EndSkillCheck(bool success)
    {
        skillCheckActive = false;
        if(success)
        {
            skillCheckSuccess = true;
        }
        else
        {
            skillCheckSuccess = false;
        }
        pointerTransform.position = pointA.position;
    }

    public void CheckSuccess()
    {
        // Check if the pointer is within the safe zone
        if (RectTransformUtility.RectangleContainsScreenPoint(safeZone, pointerTransform.position, null))
        {
            EndSkillCheck(true);
        }
        else
        {
            EndSkillCheck(false);
        }
    }

    public bool ResetSkillCheck()
    {
        skillCheckSuccess = false;
        skillCheckActive = false;
        pointerTransform.position = pointA.position;

        return skillCheckSuccess;
    }

    public void SkillCheckControl(KeyCode key)
    {
        if (skillCheckActive)
        {
            if (Input.GetKeyDown(key))
            {
                CheckSuccess();
            }
        }
    }
    #endregion
}