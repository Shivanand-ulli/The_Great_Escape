using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleportpad : MonoBehaviour
{
    public Transform entryPoint;  // The point where the player enters
    public Transform exitPoint;   // The point where the player exits

    public string playerTag = "Player"; // The tag of the player GameObject

    private bool playerInside = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object entering the trigger is the player and if the player isn't already inside
        if (other.CompareTag(playerTag) && !playerInside)
        {
            // Teleport the player to the exit position
            other.transform.position = exitPoint.position;
            playerInside = true;  // Set the flag to true to prevent immediate re-teleportation
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Reset the flag when the player exits the teleport area
        if (other.CompareTag(playerTag))
        {
            playerInside = false;
        }
    }
}
