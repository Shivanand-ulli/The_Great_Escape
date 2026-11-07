using UnityEngine;

public interface IGameScreenUI 
{
    GameScreenType GetScreenType();
    GameObject GetScreenGameObject();
    bool IsScreenInitialized();
    bool IsScreenVisible();
    void OnScreenInitialize();
    void OnScreenShow();
    void OnScreenHide();
    void RefreshScreen();
}
