using UnityEngine;
using UnityEngine.UI;

public class LevelHintPanel : PopupBase
{
    [SerializeField] private Button okButton;

    protected override void OnInit()
    {
        if (okButton != null) okButton.onClick.AddListener(OnOkClicked);
    }

    protected override void OnDestroy()
    {
        if (okButton != null) okButton.onClick.RemoveListener(OnOkClicked);

        base.OnDestroy();
    }

    private void OnOkClicked()
    {
        Close();
    }
}
