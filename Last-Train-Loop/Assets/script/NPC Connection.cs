using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCConnection : MonoBehaviour
{
    public Transform seat, man;
    //playerœ‡πÿ
    GameObject player;
    CharacterController characterController;
    PlayerController playerController;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            characterController = GetComponent<CharacterController>();
            playerController = GetComponent<PlayerController>();
        }
    }
    public void PlayerSeatedDown()
    {
        if (player != null)
        {
            player.transform.position = seat.position;
            player.transform.LookAt(man.transform.position + new Vector3(0, 0.2f,0));
            playerController.enabled = false;
            characterController.enabled = false;
        }
    }
    public void PlayerEnabled()
    {
        characterController.enabled = true;
        playerController.enabled = true;
    }
}
