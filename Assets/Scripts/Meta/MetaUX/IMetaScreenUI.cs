using UnityEngine;

public interface IMetaScreenUI
{
    MetaScreenType GetScreenType();
    GameObject GetScreenGameObject();
    bool IsScreenInitialized();
    bool IsScreenVisible();
    void OnScreenInitialize();
    void OnScreenShow();
    void OnScreenHide();
    void RefreshScreen();
}
