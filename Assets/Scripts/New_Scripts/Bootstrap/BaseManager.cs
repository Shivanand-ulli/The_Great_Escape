using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public abstract class BaseManager : MonoBehaviour,ICleanUpManager
{
    [Header("Manager Settings")]
    [SerializeField] protected bool initializeOnAwake = false;
    protected bool isInitialized = false;

    protected virtual void Awake()
    {
        if(!initializeOnAwake)
        {
            InitializeManager();
        }
    }

    protected virtual void OnDestroy()
    {
        CleanUp();
    }

    public void InitializeManager()
    {
        if(isInitialized)
        {
            Debug.Log(GetType().Name + "already initialized");
        }

        OnInitialize();

        isInitialized = true;
    }

    public bool IsInitialized()
    {
        return isInitialized;
    }

    public string GetManagerName()
    {
        return GetType().Name;
    }

    public void CleanUp()
    {
        if(!isInitialized) return;

        OnCleanUp();

        isInitialized = false;
    }

    protected abstract void OnInitialize();
    protected virtual void OnCleanUp()
    {
        
    }
}

public abstract class SingletonManager<T> : BaseManager where T : SingletonManager<T>
{
    protected static T instance;
    public static T Instance = instance;

    protected override void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = (T) this;

        base.Awake();
    }

    protected override void OnDestroy()
    {
        if(instance == this)
        {
            instance = null;
        }
        
        base.Awake();
    }
} 
