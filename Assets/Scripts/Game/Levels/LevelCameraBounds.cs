using Cinemachine;
using UnityEngine;

/// <summary>
/// Put this on the level prefab. When the level is spawned it hands the level's BG polygon collider
/// to every Cinemachine Confiner 2D in the scene, so one shared camera works for all levels.
/// </summary>
public class LevelCameraBounds : MonoBehaviour
{
    [Tooltip("The BG Polygon Collider 2D inside this level prefab.")]
    [SerializeField] private Collider2D boundingShape;

    private void OnEnable()
    {
        ApplyBounds();
    }

    public void ApplyBounds()
    {
        if (boundingShape == null)
        {
            Debug.LogWarning("[LevelCameraBounds] Bounding Shape is not assigned.", this);
            return;
        }

        CinemachineConfiner2D[] confiners = FindObjectsByType<CinemachineConfiner2D>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        for (int i = 0; i < confiners.Length; i++)
        {
            confiners[i].m_BoundingShape2D = boundingShape;
            confiners[i].InvalidateCache(); // required after changing the shape
        }
    }
}
