using UnityEngine;
using UnityEngine.UI;

/// <summary>Base class for every Meta screen (Home, LevelSelect, Settings...). Derive and override as needed.</summary>
public class MetaBaseScreenUI : MetaBaseScreen
{
    [SerializeField] private Button PlayButton;

    private void OnEnable()
    {
        PlayButton.onClick.AddListener(OnPlayButtonClicked);
    }

    private void OnDisable()
    {
        PlayButton.onClick.RemoveListener(OnPlayButtonClicked);
    }

    private void OnPlayButtonClicked()
    {
        PopupManager.Instance.ShowPopup<LevelSelectionPopup>();
    }


}
