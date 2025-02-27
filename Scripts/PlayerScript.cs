using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    #region Character Variables
    CharacterController characterController;
    [SerializeField] GameObject avatarObject;
    [SerializeField] Transform cameraPivot;
    [SerializeField] float walkSpeed = 6f;
    [SerializeField] float gravity = 1f;
    [SerializeField] float rotateSpeed = 10f; 
    Vector3 moveDirection, origPos;
    StaminaScript stamina;
    [SerializeField] float dashSpeed = 10f;
    [SerializeField] float dashDuration = 1f;
    [SerializeField] float dashCooldown = 1f;
    bool isDashing = false;
    bool isDashCooldown = false;
    // knockbackForce, knockbackTime
    float knockbackCounter;
    // bool isHit = false;
    #endregion

    #region Interact Variables
    Transform interactHitbox;
    [SerializeField] GameObject interactPivot;
    [SerializeField] Vector3 boxSize = new Vector3(1, 2, 2);
    [SerializeField] float maxDistance = 1;
    bool isInteractingStation = false;
    public static bool isInteractingPointB = false;
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
        stamina = GetComponent<StaminaScript>();
        // knockbackForce = 10f;
        // knockbackTime = .25f;

        origPos = transform.position;
        interactHitbox = interactPivot.GetComponent<Transform>();

        hasItem = null;
    }

    // Update is called once per frame
    void Update()
    {
        if(isDashing)
        {
            return;
        }

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

        if(Input.GetKeyDown(KeyCode.F) && !isDashCooldown)
        {
            StartCoroutine(Dash());
        }

        FindAnyObjectByType<PointBScript>().SkillCheckControl();
    }

    void FixedUpdate()
    {
        if(isDashing)
        {
            return;
        }

        #region Movement
        if(knockbackCounter <= 0)
        {
            // isHit = false;
            
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);
                
            moveDirection = (forward * Input.GetAxisRaw("Vertical")) + (right * Input.GetAxisRaw("Horizontal"));

            float movementDirectionY = moveDirection.y;
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
            characterController.Move(moveDirection.normalized * walkSpeed * Time.deltaTime);
        }
        #endregion

        #region Character Rotation
        if(Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            if(!isInteractingPointB)
            {
                transform.rotation = Quaternion.Euler(0, cameraPivot.rotation.eulerAngles.y, 0);
                Quaternion newRotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0f, moveDirection.z));
                avatarObject.transform.rotation = Quaternion.Slerp(avatarObject.transform.rotation, newRotation, Time.deltaTime * rotateSpeed);
            }
        }
        #endregion
    }

    private IEnumerator Dash()
    {
        if(stamina.ConsumeStamina(1))
        {
            isDashCooldown = true;
            isDashing = true;
            stamina.isDashing = true;
            float startTime = Time.time;
            Vector3 dashDirection = moveDirection.normalized * dashSpeed;

            while (Time.time < startTime + dashDuration)
            {
                characterController.Move(dashDirection * Time.deltaTime);
                yield return null;
            }

            isDashing = false;
            stamina.isDashing = false;

            yield return new WaitForSeconds(dashCooldown);
            isDashCooldown = false;
        }
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