using UnityEngine;

/// <summary>
/// Put this on an empty object inside each level prefab where the player should begin.
/// LevelManager moves the player here when the level starts.
/// </summary>
public class LevelStartPoint : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.4f);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 1f);
    }
}
