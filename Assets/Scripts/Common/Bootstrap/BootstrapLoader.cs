using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum LoadMode
{
    Single,
    Additive
}

[System.Serializable]
public class SceneLoadInfo
{
    public LoadMode loadMode = LoadMode.Single;
    public string sceneName;

    public string GetSceneName()
    {
        return sceneName;
    }

    public bool IsValid()
    {
        return !string.IsNullOrEmpty(sceneName);
    }
}

[System.Serializable]
public class ManagerLoadInfo
{
    public bool isEnabled = true;
    public string managerName;
    public GameObject managerPrefab;
    public bool isPersistence = true;

    public string GetManagerName()
    {
        return managerName;
    }

    public bool IsValid()
    {
        return !string.IsNullOrEmpty(managerName);
    }
}

[System.Serializable]
public class PrefabLoadInfo
{
    public bool isEnable = true;
    public string customName;
    public GameObject prefab;
    public bool isPersistence = true;

    public string GetPrefabName()
    {
        if (!string.IsNullOrEmpty(customName))
            return customName;

        if (prefab != null)
            return prefab.name;

        return "Unknown Prefab";
    }

    public bool IsValid()
    {
        return prefab != null;
    }

}
public class BootstrapLoader : MonoBehaviour
{
    [SerializeField] private bool autoLoadScene = true;
    [SerializeField] private bool useActualProgressBar = true;
    [SerializeField] private float minimumLoadTime = 0.5f;
    [SerializeField] private BootstrapLoadingUI loadingUI;
    [SerializeField] List<SceneLoadInfo> sceneLoadInfo = new List<SceneLoadInfo>();
    [SerializeField] List<PrefabLoadInfo> prefabLoadInfo = new List<PrefabLoadInfo>();
    [SerializeField] List<ManagerLoadInfo> managerLoadInfo = new List<ManagerLoadInfo>();
    private Dictionary<System.Type, IManager> initializedManager = new Dictionary<System.Type, IManager>();
    private bool isInitialized = false;
    private bool forceRecreateManager = false;

    private void Start()
    {
        Debug.Log("[Bootstrap] Starting bootstrap initialization...");

        if (loadingUI != null)
        {
            if (useActualProgressBar)
            {
                loadingUI.SetProgress(0f);
            }
            else
            {
                loadingUI.StartLoading(minimumLoadTime);
            }
        }

        StartCoroutine(InitializeBootstrap());
    }

    IEnumerator InitializeBootstrap()
    {
        float startTime = Time.time;
        float effectiveMinimumLoadTime = minimumLoadTime;

        yield return StartCoroutine(InitializeCoreSystem());

        if (minimumLoadTime > 0)
        {
            float elapsedTime = Time.time - startTime;
            if (elapsedTime < effectiveMinimumLoadTime)
            {
                Debug.Log($"[Bootstrap] Waiting for minimum load time... ({effectiveMinimumLoadTime - elapsedTime:F2}s remaining)");
                yield return new WaitForSeconds(effectiveMinimumLoadTime - elapsedTime);
            }
        }

        isInitialized = true;
        Debug.Log($"[Bootstrap] Initialize Completed");

        if (autoLoadScene)
        {
            StartCoroutine(LoadScenes());
        }
        else
        {
            UpdateLoadingProgress(0.95f);
        }

        forceRecreateManager = false;
    }

    private IEnumerator InitializeCoreSystem()
    {
        Debug.Log("[Bootstrap] Initializing core systems...");

        ValidateManagers();

        int totalManagersToLoad = GetValidEnabledManagerCount();
        int initializedCount = 0;

        UpdateLoadingProgress(0f);

        if (managerLoadInfo == null && managerLoadInfo.Count == 0)
        {
            Debug.Log("No managers added in inspector");
            UpdateLoadingProgress(0.7f);
            yield break;
        }

        foreach (ManagerLoadInfo managerLoadInfo in managerLoadInfo)
        {
            if (!managerLoadInfo.isEnabled)
            {
                Debug.Log($"[Bootstrap] Manager '{managerLoadInfo.GetManagerName()}' is disabled. Skipping.");
                continue;
            }

            if (!managerLoadInfo.IsValid())
            {
                Debug.Log($"[Bootstrap] Manager '{managerLoadInfo.GetManagerName()}' is not valid or doesn't implement IManager. Skipping.");
                continue;
            }

            string managerName = managerLoadInfo.GetManagerName();
            Debug.Log($"[Bootstrap] Initializing {managerName}...");

            InitializeManagerFromInfo(managerLoadInfo);
            initializedCount++;

            UpdateLoadingProgress(CalculateManagerProgress(initializedCount, totalManagersToLoad));
        }

        Debug.Log($"[Bootstrap] Core systems initialized successfully! {initializedManager.Count} manager(s) loaded.");
        UpdateLoadingProgress(0.7f);

        InitializeOptionalPrefabs();
    }

    private void InitializeManagerFromInfo(ManagerLoadInfo managerLoadInfo)
    {
        string managerName = managerLoadInfo.GetManagerName();
        IManager manager = null;
        MonoBehaviour managerInstance = null;

        bool creationSuccess = false;

        try
        {
            MonoBehaviour existingManager = FindFirstObjectByType(managerLoadInfo.managerPrefab.GetType()) as MonoBehaviour;

            if (existingManager != null && existingManager is IManager)
            {
                if (forceRecreateManager)
                {
                    Destroy(existingManager.gameObject);
                }
                else
                {
                    manager = existingManager as IManager;
                    managerInstance = existingManager;
                    creationSuccess = true;
                }
            }

            if (!creationSuccess && managerLoadInfo.managerPrefab != null)
            {
                Debug.Log("Creating " + managerName + " creating from prefab");
                GameObject managerGO = Instantiate(managerLoadInfo.managerPrefab);
                managerInstance = managerGO.GetComponent<MonoBehaviour>();
                manager = managerInstance as IManager;
                managerName = managerInstance.name;

                if (managerLoadInfo.isPersistence)
                {
                    DontDestroyOnLoad(managerInstance);
                    Debug.Log(managerName + "is set to Don'tDestroyOnLoad");
                }

                creationSuccess = true;
            }
            else
            {
                Debug.Log("Manager prefab is null for {managerName}");
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log($"[Bootstrap] Error creating {managerName}: {ex.Message}");
            creationSuccess = false;
        }

        if (creationSuccess && manager != null)
        {
            bool initSuccess = false;
            try
            {
                manager.InitializeManager();
                initSuccess = true;
            }
            catch (System.Exception ex)
            {
                Debug.Log($"[Bootstrap] Error during {managerName} initialization: {ex.Message}");
                throw;
            }

            if (initSuccess)
            {
                initializedManager[manager.GetType()] = manager;
                Debug.Log($"[Bootstrap] {managerName} is initialized");
            }
            else
            {
                Debug.Log($"[Bootstrap] is failed to initialize {managerName}");
            }
        }
        else
        {
            Debug.Log($"[Bootstrap] Failed to create or find {managerName}");
        }
    }

    private void ValidateManagers()
    {
        if (managerLoadInfo == null && managerLoadInfo.Count == 0)
        {
            return;
        }

        int enabledCount = 0;
        int disabledCount = 0;
        int validCount = 0;
        int invalidCount = 0;

        foreach (ManagerLoadInfo managerLoad in managerLoadInfo)
        {
            if (managerLoad.isEnabled)
                enabledCount++;
            else
                disabledCount++;

            if (managerLoad.isEnabled && managerLoad.IsValid())
            {
                validCount++;
            }
            else if (managerLoad.isEnabled && !managerLoad.IsValid())
            {
                invalidCount++;
                if (managerLoad.managerPrefab == null)
                {
                    Debug.Log($"[Bootstrap] Manager prefab is null!");
                }
                else if (!(managerLoad.managerPrefab is IManager))
                {
                    Debug.Log($"[Bootstrap] Manager '{managerLoad.managerPrefab.GetType().Name}' doesn't implement IManager interface!");
                }
            }

            Debug.Log($"[Bootstrap] Manager Summary: {enabledCount} enabled, {disabledCount} disabled, {validCount} valid, {invalidCount} invalid");

            if (validCount == 0 && enabledCount > 0)
            {
                Debug.Log("[Bootstrap] No valid managers found!");
            }
            else if (invalidCount > 0)
            {
                Debug.Log($"[Bootstrap] {invalidCount} invalid manager(s) found, {validCount} valid manager(s) available.");
            }
            else
            {
                Debug.Log($"[Bootstrap] All {validCount} manager(s) are valid.");
            }
        }
    }

    private int GetValidEnabledManagerCount()
    {
        if (managerLoadInfo == null)
        {
            return 0;
        }

        int count = 0;
        foreach (var managerInfo in managerLoadInfo)
        {
            if (managerInfo.isEnabled && managerInfo.IsValid())
            {
                count++;
            }
        }

        return count;
    }

    private void InitializeOptionalPrefabs()
    {
        if (prefabLoadInfo == null && prefabLoadInfo.Count == 0)
        {
            Debug.Log("[Bootstrap] No optional prefabs configured to load.");
            return;
        }

        int loadCount = 0;
        foreach (PrefabLoadInfo prefabInfo in prefabLoadInfo)
        {
            if (!prefabInfo.isEnable)
            {
                Debug.Log($"[Bootstrap] Prefab '{prefabInfo.GetPrefabName()}' is disabled. Skipping.");
                continue;
            }

            if (!prefabInfo.IsValid())
            {
                Debug.Log("[Bootstrap] Optional prefab reference is null. Skipping.");
                continue;
            }

            string prefabName = prefabInfo.customName;
            if (GameObject.Find(prefabName) != null)
            {
                Debug.Log("[Bootstrap] Prefab '{prefabName}' already exists in scene. Skipping.");
                continue;
            }

            GameObject prefabInstance = Instantiate(prefabInfo.prefab);
            prefabInstance.name = prefabInfo.customName;

            if (prefabInfo.isPersistence)
            {
                DontDestroyOnLoad(prefabInstance);
                Debug.Log($"[Bootstrap] Prefab '{prefabName}' set to DontDestroyOnLoad");
            }

            loadCount++;
        }
    }

    private IEnumerator LoadScenes()
    {
        if (!isInitialized)
        {
            Debug.Log("[Bootstrap] Attempting to load scenes before initialization is complete!");
        }

        if (sceneLoadInfo == null || sceneLoadInfo.Count == 0)
        {
            Debug.Log("[Bootstrap] No scenes configured to load! Add scenes in the Inspector.");
            UpdateLoadingProgress(0.95f);
            yield break;
        }

        Debug.Log($"[Bootstrap] Loading {sceneLoadInfo.Count} scene(s)...");

        for (int i = 0; i < sceneLoadInfo.Count; i++)
        {
            SceneLoadInfo sceneInfo = sceneLoadInfo[i];

            if (!sceneInfo.IsValid())
            {
                Debug.Log($"[Bootstrap] Scene at index {i} is not valid (no scene asset or name). Skipping.");
                continue;
            }

            string sceneName = sceneInfo.GetSceneName();
            LoadMode loadMode = sceneInfo.loadMode;

            Debug.Log($"[Bootstrap] Loading scene: {sceneName} (Mode: {loadMode})");

            UnityEngine.SceneManagement.LoadSceneMode unityMode = (UnityEngine.SceneManagement.LoadSceneMode)loadMode;

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, unityMode);

            if (asyncLoad == null)
            {
                Debug.Log($"[Bootstrap] Failed to load scene: {sceneName}");
            }

            while (!asyncLoad.isDone)
            {
                UpdateLoadingProgress(CalculateSceneProgress(i, sceneLoadInfo.Count, asyncLoad.progress));
                yield return null;
            }

            Debug.Log($"[Bootstrap] Scene loaded: {sceneName}");
            UpdateLoadingProgress(CalculateSceneProgress(i + 1, sceneLoadInfo.Count, asyncLoad.progress));
        }

        Debug.Log("[Bootstrap] All scenes loaded successfully!");
        UpdateLoadingProgress(0.95f);

        // // Hide loading UI after all scenes are loaded
        if (loadingUI != null)
        {
            loadingUI.HideLoadingScreen();
        }
    }

    private void UpdateLoadingProgress(float progress)
    {
        if (!useActualProgressBar || loadingUI == null)
        {
            return;
        }

        loadingUI.SetProgress(progress);
    }

    private float CalculateManagerProgress(int initializedCount, int totalManagers)
    {
        if (totalManagers <= 0)
        {
            return 0.7f;
        }

        float normalized = Mathf.Clamp01((float)initializedCount / totalManagers);
        return Mathf.Lerp(0f, 0.7f, normalized);
    }

    private float CalculateSceneProgress(int completedScenes, int totalScenes, float sceneProgress)
    {
        if (totalScenes <= 0)
        {
            return 0.95f;
        }

        float normalizedSceneProgress = Mathf.Clamp01(sceneProgress / 0.9f);
        float normalizedTotal = Mathf.Clamp01((completedScenes + normalizedSceneProgress) / totalScenes);
        return Mathf.Lerp(0.7f, 0.95f, normalizedTotal);
    }

    
}
