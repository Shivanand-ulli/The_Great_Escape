using System;
using System.Collections.Generic;
using UnityEngine;

public class PopupManager : SingletonManager<PopupManager>
{
    [Header("Popup Configuration")]
    [SerializeField] private PopupDatabase popupDatabase;
    [SerializeField] private GameObject popupBackgroundPrefab;
    
    [Header("Canvas Settings")]
    [SerializeField] private Canvas uiCanvas;

    private readonly List<PopupBase> popups = new();
    private readonly Dictionary<Type, GameObject> popupLookup = new();

    // Public properties
    public PopupDatabase PopupDatabase => popupDatabase;
    public GameObject PopupBackgroundPrefab => popupBackgroundPrefab;
    public Canvas UICanvas => uiCanvas;
    public int ActivePopupCount => popups.Count;

    /// <summary>
    /// Gets the background prefab for the specified tag from the PopupDatabase,
    /// falling back to the default popupBackgroundPrefab if not found.
    /// </summary>
    /// <param name="tag">Background tag</param>
    /// <returns>The background prefab</returns>
    public GameObject GetPopupBackgroundPrefab(string tag = null)
    {
        if (popupDatabase != null)
        {
            var bg = popupDatabase.GetBackgroundPrefab(tag);
            if (bg != null) return bg;
        }
        return popupBackgroundPrefab;
    }

    // ============================
    // INITIALIZATION
    // ============================

    protected override void OnInitialize()
    {
        Debug.Log("Initializing Popup Manager");

        // Get or create Canvas component
        if (uiCanvas == null)
        {
            uiCanvas = GetComponent<Canvas>();
        }

        if (uiCanvas == null)
        {
            Debug.LogError("PopupManager requires a Canvas component! Add a Canvas component or assign it in the Inspector.");
            return;
        }

        // Build the popup lookup dictionary
        BuildPopupLookup();

        Debug.Log($"Popup Manager initialized with {popupLookup.Count} popup types registered.");
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Debug.Log("Cleaning up Popup Manager");
            CloseAll();
            popupLookup.Clear();
        }
    }

    /// <summary>
    /// Builds the popup type lookup dictionary from the database
    /// </summary>
    private void BuildPopupLookup()
    {
        popupLookup.Clear();

        if (popupDatabase == null)
        {
            Debug.LogWarning("Popup Database is not assigned! Assign it in the Inspector.");
            return;
        }

        if (popupDatabase.popupPrefabs == null || popupDatabase.popupPrefabs.Count == 0)
        {
            Debug.LogWarning("Popup Database has no popup prefabs assigned!");
            return;
        }

        int validCount = 0;
        int invalidCount = 0;

        foreach (var prefab in popupDatabase.popupPrefabs)
        {
            if (prefab == null)
            {
                invalidCount++;
                continue;
            }

            var popupScript = prefab.GetComponent<PopupBase>();
            if (popupScript != null)
            {
                Type type = popupScript.GetType();
                if (!popupLookup.ContainsKey(type))
                {
                    popupLookup[type] = prefab;
                    validCount++;
                }
                else
                {
                    Debug.LogWarning($"Duplicate popup type found: {type.Name}. Only the first one will be used.");
                }
            }
            else
            {
                Debug.LogWarning($"Prefab '{prefab.name}' does not have a PopupBase component!");
                invalidCount++;
            }
        }

        Debug.Log($"Popup lookup built: {validCount} valid, {invalidCount} invalid/skipped.");
    }

    // ============================
    // POPUP DISPLAY
    // ============================
    
    /// <summary>
    /// Shows a popup of the specified type.
    /// If the popup is already open, returns the existing instance.
    /// </summary>
    /// <typeparam name="T">Type of popup to show (must derive from PopupBase)</typeparam>
    /// <returns>Instance of the popup, or null if failed</returns>
    public T ShowPopup<T>() where T : PopupBase
    {
        // Clean up null references before checking
        CleanupNullPopups();
        
        // Check if popup is already open
        foreach (var openPopup in popups)
        {
            if (openPopup != null && openPopup is T existingPopup)
            {
                Debug.Log($"Popup {typeof(T).Name} already open, bringing to front.");
                existingPopup.transform.SetAsLastSibling();
                return existingPopup;
            }
        }

        // Try to get prefab from lookup
        if (!popupLookup.TryGetValue(typeof(T), out var prefab))
        {
            Debug.LogError($"Popup prefab not found for type {typeof(T).Name}. Make sure it's added to the PopupDatabase.");
            return null;
        }

        if (uiCanvas == null)
        {
            Debug.LogError("UI Canvas is null! Cannot instantiate popup.");
            return null;
        }

        // Instantiate the popup
        try
        {
            var instance = Instantiate(prefab, uiCanvas.transform);
            T popup = instance.GetComponent<T>();

            if (popup == null)
            {
                Debug.LogError($"Instantiated prefab does not have component {typeof(T).Name}!");
                Destroy(instance);
                return null;
            }

            // Calculate sorting order (each popup layer above the previous)
            int order = 100 + popups.Count * 10;
            popup.Init(uiCanvas, order);

            // Add to active popups stack
            popups.Add(popup);
            
            Debug.Log($"Showed popup: {typeof(T).Name} (Order: {order}, Total: {popups.Count})");
            
            return popup;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error showing popup {typeof(T).Name}: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Closes the most recently opened popup (top of the stack)
    /// </summary>
    public void CloseTopPopup()
    {
        // Clean up null references first
        CleanupNullPopups();

        if (popups.Count == 0)
        {
            Debug.Log("No popups to close.");
            return;
        }

        PopupBase top = popups[^1];
        popups.RemoveAt(popups.Count - 1);
        
        string popupName = top != null ? top.GetType().Name : "Unknown";
        Debug.Log($"Closing popup: {popupName} (Remaining: {popups.Count})");
        
        if (top != null)
        {
            top.Close();
        }
    }

    /// <summary>
    /// Closes a specific popup instance
    /// </summary>
    /// <param name="popup">The popup to close</param>
    public void ClosePopup(PopupBase popup)
    {
        if (popup == null)
        {
            Debug.LogWarning("Cannot close null popup!");
            return;
        }

        if (popups.Remove(popup))
        {
            Debug.Log($"Closed popup: {popup.GetType().Name} (Remaining: {popups.Count})");
            popup.Close();
        }
        else
        {
            Debug.LogWarning($"Popup {popup.GetType().Name} was not in the active popups list!");
        }
    }

    /// <summary>
    /// Closes all open popups
    /// </summary>
    public void CloseAll()
    {
        // Clean up null references first
        CleanupNullPopups();

        if (popups.Count == 0)
        {
            Debug.Log("No popups to close.");
            return;
        }

        int count = popups.Count;
        Debug.Log($"Closing all {count} popup(s).");

        // Work on a copy and clear first: closing a popup can remove it from the list.
        List<PopupBase> toClose = new List<PopupBase>(popups);
        popups.Clear();

        foreach (var popup in toClose)
        {
            if (popup != null)
            {
                popup.Close();
            }
        }
    }

    // ============================
    // UTILITY METHODS
    // ============================
    
    /// <summary>
    /// Removes null references from the popups list.
    /// Call this periodically to clean up destroyed popups that weren't properly removed.
    /// </summary>
    private void CleanupNullPopups()
    {
        int removedCount = popups.RemoveAll(popup => popup == null);
        if (removedCount > 0)
        {
            Debug.LogWarning($"Cleaned up {removedCount} null popup reference(s) from the list.");
        }
    }

    /// <summary>
    /// Removes a popup from the list when it's destroyed externally.
    /// Called by PopupBase.OnDestroy() to ensure proper cleanup.
    /// </summary>
    /// <param name="popup">The popup being destroyed</param>
    internal void OnPopupDestroyed(PopupBase popup)
    {
        if (popup == null) return;
        
        if (popups.Remove(popup))
        {
            Debug.Log($"Removed destroyed popup: {popup.GetType().Name} (Remaining: {popups.Count})");
        }
    }
    
    /// <summary>
    /// Checks if a specific popup type is currently open
    /// </summary>
    /// <typeparam name="T">Popup type to check</typeparam>
    /// <returns>True if the popup is open, false otherwise</returns>
    public bool IsPopupOpen<T>() where T : PopupBase
    {
        CleanupNullPopups(); // Clean up before checking
        
        foreach (var popup in popups)
        {
            if (popup != null && popup is T)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Gets a currently open popup of the specified type
    /// </summary>
    /// <typeparam name="T">Popup type to get</typeparam>
    /// <returns>The popup instance if open, null otherwise</returns>
    public T GetOpenPopup<T>() where T : PopupBase
    {
        CleanupNullPopups(); // Clean up before checking
        
        foreach (var popup in popups)
        {
            if (popup != null && popup is T typedPopup)
                return typedPopup;
        }
        return null;
    }

    /// <summary>
    /// Reloads the popup database (useful for hot-reloading in editor)
    /// </summary>
    public void ReloadPopupDatabase()
    {
        Debug.Log("Reloading popup database...");
        BuildPopupLookup();
    }
}
