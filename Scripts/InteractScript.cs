using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractScript : MonoBehaviour
{
    [SerializeField] GameObject interactObject;
    Transform characterTransform;
    [SerializeField] float radius = 1f;
    [SerializeField] float maxDistance = 1;
    // Start is called before the first frame update
    void Start()
    {
        characterTransform = interactObject.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        #region SphereCast
        RaycastHit hit;
        if (Physics.SphereCast(characterTransform.position, radius, characterTransform.forward, out hit, maxDistance))
        {
            hit.collider.GetComponent<MeshRenderer>().material.color =
               new Color(Random.Range(0.0f, 1.0f),
               Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
        }

        // Draw the sphere cast ray
        Debug.DrawRay(characterTransform.position, characterTransform.forward * maxDistance, Color.red);
        #endregion
        
    }
}
