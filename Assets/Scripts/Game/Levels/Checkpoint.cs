using UnityEngine;

/// <summary>
/// Put this on a trigger collider in the level and give each one its own number (1, 2, 3...).
/// The last checkpoint the player passed is where they come back to after losing a life.
/// Passing an earlier checkpoint again never moves the respawn point backwards.
/// LevelManager sets the level start as the default and resets this when a level starts.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class Checkpoint : MonoBehaviour
{
    [Tooltip("Order in the level: 1 = first checkpoint, 2 = second, and so on.")]
    [SerializeField, Min(1)] private int checkpointNumber = 1;

    [Tooltip("Optional tick/flag object that is shown once this checkpoint has been passed.")]
    [SerializeField] private GameObject reachedMark;

    private static Vector3 respawnPosition;
    private static bool hasRespawnPosition;
    private static int lastReachedNumber;

    /// <summary>Number of the last checkpoint passed. 0 = none yet.</summary>
    public static int LastReachedNumber => lastReachedNumber;

    private bool isReached;

    /// <summary>Where the player should come back to. Uses the fallback if nothing was set.</summary>
    public static Vector3 GetRespawnPosition(Vector3 fallback)
    {
        return hasRespawnPosition ? respawnPosition : fallback;
    }

    /// <summary>Sets the default respawn point (the level start). Called by LevelManager.</summary>
    public static void SetStartPosition(Vector3 position)
    {
        respawnPosition = position;
        hasRespawnPosition = true;
    }

    /// <summary>Forgets all checkpoints. Called by LevelManager when a level starts.</summary>
    public static void ResetCheckpoints()
    {
        respawnPosition = Vector3.zero;
        hasRespawnPosition = false;
        lastReachedNumber = 0;
    }

    private void Awake()
    {
        if (reachedMark != null) reachedMark.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isReached || !other.CompareTag("Player")) return;

        isReached = true;
        if (reachedMark != null) reachedMark.SetActive(true);

        // Only move the respawn point forward, never back to an earlier checkpoint.
        if (checkpointNumber > lastReachedNumber)
        {
            lastReachedNumber = checkpointNumber;
            respawnPosition = transform.position;
            hasRespawnPosition = true;
        }
    }
}
