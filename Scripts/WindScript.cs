using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindScript : MonoBehaviour
{
    PlayerScript player;
    Player2Script player2;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerScript>();
        player2 = FindObjectOfType<Player2Script>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "User")
        {
            player.walkSpeed = 3f;
            player2.walkSpeed = 3f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "User")
        {
            player.walkSpeed = 6f;
            player2.walkSpeed = 6f;
        }
    }   
}
