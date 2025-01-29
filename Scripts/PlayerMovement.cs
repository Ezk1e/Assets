using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    CharacterController characterController;
    [SerializeField] GameObject playerObject;
    [SerializeField] float walkSpeed, gravity, knockbackForce, knockbackTime, rotateSpeed;
    float knockbackCounter;
    Vector3 moveDirection;
    Vector3 origPos;
    bool isHit = false;

    [SerializeField] GameObject interactObject;
    Transform characterTransform;
    [SerializeField] Vector3 boxSize = new Vector3(1, 1, 1);
    [SerializeField] float maxDistance = 1;
    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        walkSpeed = 6f;
        gravity = 1f;
        knockbackForce = 10f;
        knockbackTime = .25f;
        rotateSpeed = 10f;

        origPos = transform.position;

        characterTransform = interactObject.GetComponent<Transform>();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        #region Movement
        if(knockbackCounter <= 0)
        {
            isHit = false;
            
            
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);

            float speedX;
            float speedY;

            speedX = walkSpeed * Input.GetAxis("Vertical");
            speedY = walkSpeed * Input.GetAxis("Horizontal");
                
            float movementDirectionY = moveDirection.y;
            moveDirection = (forward * speedX) + (right * speedY);
            moveDirection.y = movementDirectionY;

            if (!characterController.isGrounded)
            {
                moveDirection.y -= gravity;
            }
            
        }
        else
        {
            knockbackCounter -= Time.deltaTime;
        }
        characterController.Move(moveDirection * Time.deltaTime);
        #endregion

        #region Character Rotation
        if(Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            Quaternion newRotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0f, moveDirection.z));
            playerObject.transform.rotation = Quaternion.Slerp(playerObject.transform.rotation, newRotation, Time.deltaTime * rotateSpeed);
        }
        #endregion

        Debug.Log(characterTransform.lossyScale);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.name == "DeathPlane")
        {
            Vector3 spawnPosition = origPos;
            spawnPosition.y = origPos.y + 2;
            transform.position = spawnPosition;
        }
    }

    public void knockbackPlayer(Vector3 direction)
    {
        isHit = true;
        knockbackCounter = knockbackTime;
        moveDirection = direction * knockbackForce;
    }

    private void OnDrawGizmos()
    {
        if(characterTransform == null)
        {
            return;
        }
        
        RaycastHit hit;
        if(Physics.BoxCast(characterTransform.position, boxSize/2, characterTransform.forward, out hit, characterTransform.rotation, maxDistance))
        {
            hit.collider.GetComponent<MeshRenderer>().material.color =
                new Color(Random.Range(0.0f, 1.0f),
                Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
            
            Gizmos.color = Color.red;
            Gizmos.DrawRay(characterTransform.position, characterTransform.forward * hit.distance);
            Gizmos.DrawWireCube(characterTransform.position + characterTransform.forward * hit.distance, boxSize);
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(characterTransform.position, characterTransform.forward * maxDistance);
        }
    }
}

