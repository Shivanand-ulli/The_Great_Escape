using UnityEngine;

public interface IManager 
{
    void InitializeManager();
    bool IsInitialized();
    string GetManagerName();
}

public interface ICleanUpManager : IManager
{
    void CleanUp();
}
