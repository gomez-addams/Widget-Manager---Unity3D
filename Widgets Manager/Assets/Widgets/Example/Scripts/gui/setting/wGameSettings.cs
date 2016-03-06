using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using eeGames.Widget;

public class wGameSettings : Widget {

    [SerializeField]
    private Button m_closeButton;

    private void OnCloseButtonClick()
    {
        WidgetManager.Instance.Pop(this.Id, false);
    }


    void Awake()
    {
        m_closeButton.onClick.AddListener(OnCloseButtonClick);
    }

    void OnDestroy()
    {
        m_closeButton.onClick.RemoveListener(OnCloseButtonClick);
        base.DestroyWidget();
    }
}
