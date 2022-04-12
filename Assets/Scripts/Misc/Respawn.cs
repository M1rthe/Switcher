using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Respawn : MonoBehaviour
{
    CharacterController characterController;
    PlayerMovement playerMovement;

    void OnTriggerEnter(Collider other)
    {
        if ((~LayerMask.GetMask("Player", "GhostPlayer") & (1 << other.gameObject.layer)) != 0) return; //Collision with layermask

        characterController = other.GetComponentInChildren<CharacterController>();
        playerMovement = other.GetComponentInChildren<PlayerMovement>();

        other.GetComponentInChildren<AudioManager>().PlayAudio("Death");

        playerMovement.enabled = false;
        
        GameManager.hud.deathScreen.SetActive(true);

        StartCoroutine(Wait(5, delegate {
            characterController.enabled = false;
            other.transform.position = GameManager.spawnPoint.position;
            other.transform.rotation = GameManager.spawnPoint.rotation;
            characterController.enabled = true;
            playerMovement.enabled = true;
            GameManager.hud.deathScreen.SetActive(false);
        }));
    }

    IEnumerator Wait(float time, UnityAction unityAction)
    {
        yield return new WaitForSeconds(time);
        unityAction.Invoke();
    }
}
