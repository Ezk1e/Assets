using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    #region Character Variables
    CharacterController characterController;
    [SerializeField] GameObject avatarObject;
    [SerializeField] float walkSpeed, gravity, rotateSpeed; // knockbackForce, knockbackTime
    float knockbackCounter;
    Vector3 moveDirection, origPos;
    // bool isHit = false;
    #endregion

    #region Interact Variables
    [SerializeField] GameObject interactPivot;
    Transform interactHitbox;
    [SerializeField] Vector3 boxSize;
    [SerializeField] float maxDistance = 1;
    bool isInteractingStation = false;
    bool isInteractingPointB = false;
    #endregion

    #region Item Variables
    [SerializeField] Transform itemLocation;
    GameObject hasItem;
    Rigidbody hasItemRigid;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        walkSpeed = 6f;
        gravity = 1f;
        // knockbackForce = 10f;
        // knockbackTime = .25f;
        rotateSpeed = 10f;

        origPos = transform.position;

        interactHitbox = interactPivot.GetComponent<Transform>();
        boxSize = new Vector3(1, 2, 2);
        maxDistance = 1;

        hasItem = null;
    }
    // Update is called once per frame
    void Update()
    {
        #region Interact
        if(Input.GetKeyDown(KeyCode.E))
        {
            if(hasItem == null)
            {
                RaycastHit hit;
                if(Physics.BoxCast(interactHitbox.position, boxSize/2, interactHitbox.forward, out hit, interactHitbox.rotation, maxDistance))
                {
                    if(hit.collider.tag == "Interactable")
                    {
                        isInteractingStation = true;
                    }

                    if(hit.collider.tag == "Tool")
                    {
                        pickItem(hit.collider.gameObject);
                    }
                }
            }

            isInteractingPointB = FindAnyObjectByType<PointBScript>().doGen(this.gameObject, avatarObject);
        }
        else
        {
            isInteractingStation = false;
        }

        if(Input.GetKeyDown(KeyCode.Q))
        {
            if(hasItem != null)
            {
                dropItem();
            }
        }
        #endregion

        if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S))
        {
            isInteractingPointB = false;
        }
    }

    void FixedUpdate()
    {
        #region Movement
        if(knockbackCounter <= 0)
        {
            // isHit = false;
            
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);

            float speedX = walkSpeed * Input.GetAxis("Vertical");
            float speedY = walkSpeed * Input.GetAxis("Horizontal");
                
            float movementDirectionY = moveDirection.y;
            moveDirection = (forward * speedX) + (right * speedY);
            moveDirection.y = movementDirectionY;

            if (!characterController.isGrounded)
            {
                moveDirection.y -= gravity;
            }
            else
            {
                gravity = 1f;
            }
            
        }
        else
        {
            knockbackCounter -= Time.deltaTime;
        }

        if(!isInteractingPointB)
        {
            characterController.Move(moveDirection * Time.deltaTime);
        }
        #endregion

        #region Character Rotation
        if(Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            if(!isInteractingPointB)
            {
                Quaternion newRotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0f, moveDirection.z));
                avatarObject.transform.rotation = Quaternion.Slerp(avatarObject.transform.rotation, newRotation, Time.deltaTime * rotateSpeed);
            }
        }
        #endregion
    }

    private void pickItem(GameObject item)
    {
        hasItem = item;
        hasItemRigid = item.GetComponent<Rigidbody>();
        hasItemRigid.isKinematic = true;
        hasItemRigid.transform.parent = itemLocation;
        hasItemRigid.transform.position = itemLocation.position;
    }

    private void dropItem()
    {
        hasItemRigid.isKinematic = false;
        hasItemRigid.transform.parent = null;
        hasItem = null;
    }

    public void spawnTool(GameObject item)
    {
        if(isInteractingStation)
        {
            hasItem = Instantiate(item, itemLocation.position, itemLocation.rotation, itemLocation);
            hasItemRigid = hasItem.GetComponent<Rigidbody>();
            hasItemRigid.isKinematic = true;
        }
    }

    public GameObject getItemHolding()
    {
        return hasItem;
    }

    // public void knockbackPlayer(Vector3 direction)
    // {
    //     isHit = true;
    //     knockbackCounter = knockbackTime;
    //     moveDirection = direction * knockbackForce;
    // }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.name == "DeathPlane")
        {
            Vector3 spawnPosition = origPos;
            spawnPosition.y = origPos.y + 2;
            transform.position = spawnPosition;
        }
    }

    private void OnDrawGizmos()
    {
        if(interactHitbox == null)
        {
            return;
        }
        
        RaycastHit hit;
        if(Physics.BoxCast(interactHitbox.position, boxSize/2, interactHitbox.forward, out hit, interactHitbox.rotation, maxDistance))
        {           
            Gizmos.color = Color.red;
            Gizmos.DrawRay(interactHitbox.position, interactHitbox.forward * hit.distance);
            Gizmos.DrawWireCube(interactHitbox.position + interactHitbox.forward * hit.distance, boxSize);
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(interactHitbox.position, interactHitbox.forward * maxDistance);
        }
    }
}

