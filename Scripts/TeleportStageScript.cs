using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportStageScript : MonoBehaviour
{
    [SerializeField] Transform teleportDestination;
    [SerializeField] Transform cameraPoint;
    [SerializeField] Camera mainCamera;
    [SerializeField] float cameraMoveDuration = 1.0f;
    [SerializeField] GameObject newSpawnPoint;
    List<GameObject> players;
    HashSet<GameObject> playersInTeleport;
    // Start is called before the first frame update
    void Start()
    {
        playersInTeleport = new HashSet<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        players = new List<GameObject>(GameObject.FindGameObjectsWithTag("User"));

        if(playersInTeleport.Count == players.Count)
        {
            TeleportPlayers();
            playersInTeleport.Clear();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(players.Contains(other.gameObject))
        {
            playersInTeleport.Add(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(players.Contains(other.gameObject))
        {
            playersInTeleport.Remove(other.gameObject);
        }
    }

    void TeleportPlayers()
    {
        foreach (GameObject player in players)
        {
            player.transform.position = teleportDestination.position;
        }

        FindObjectOfType<PlayerScript>().setSpawnPoint(newSpawnPoint.transform.position);
        FindObjectOfType<Player2Script>().setSpawnPoint(newSpawnPoint.transform.position);

        StartCoroutine(MoveCameraSmoothly(cameraPoint.position, cameraMoveDuration));
    }

    IEnumerator MoveCameraSmoothly(Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = mainCamera.transform.position;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            mainCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.position = targetPosition;
    }
}
