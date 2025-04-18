﻿using UnityEngine;
using UnityEngine.UI;

namespace eeGames.Widget
{
    public class wAudioSettings : Widget
    {

        [SerializeField]
        private Button m_closeButton;

        private void OnCloseButtonClick()
        {
            WidgetManager.Instance.Pop(WidgetName.AudioSetting, false);
        }


        protected override void Awake()
        {
            m_closeButton.onClick.AddListener(OnCloseButtonClick);
        }

        void OnDestroy()
        {
            m_closeButton.onClick.RemoveListener(OnCloseButtonClick);
            base.DestroyWidget();
        }
    }
}