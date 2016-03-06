using UnityEditor;
using UnityEngine;


public enum TweenType
{
    Position,
    Scale,
    Rotation
}

public class SubmitButtonStyle
{
    public System.Action SubmitSuccess;
    public string SubmitTitle;
    public int SubmitButtonWidth;
    public int SubmitButtonHeight;

    public SubmitButtonStyle(System.Action submitSuccess = null, string title = "", int width = 0, int height = 0)
    {
        SubmitSuccess = submitSuccess;
        SubmitTitle = title;
        SubmitButtonWidth = width;
        SubmitButtonHeight = height;
    }
}

public class CustomButtonStyle
{
    public string ButtonTitle;
    public Texture2D ButtonIcon;
    public Texture2D ButtonUp;
    public Texture2D ButtonDown;
    public System.Action ButtonSubmitAction;
    public int ButtonHeight;
    public GUIStyle ButtonStyle;

    public CustomButtonStyle(string title, Texture2D up, Texture2D down, System.Action submitAction = null, int height = 50, Texture2D icon = null, GUIStyle style = null)
    {
        ButtonTitle = title;
        ButtonIcon = icon;
        ButtonUp = up;
        ButtonDown = down;
        ButtonSubmitAction = submitAction;
        ButtonHeight = height;
        ButtonStyle = style;
    }
}

public class EditorDisplayButton
{

    protected System.Action _submitCallback;
    protected Texture2D _buttonUp;
    protected Texture2D _buttonDown;
    protected Texture2D _buttonIcon;
    protected Texture2D _activeTex;
    protected Texture2D _prevTex;
    protected GUIContent _buttonContent;
    protected GUIStyle _buttonStyle;
    protected Rect _buttonRect;

    //protected int _buttonWidth;
    protected int _buttonHeight;

    protected string _buttonTitle;
    protected string _buttonText;

    //Submit button
    protected bool _submitButtonVisibile = false;
    protected string _submitButtonTitle;
    protected System.Action _submitButtonAction;
    protected int _submitButtonWidth;
    protected int _submitButtonHeight;

    public EditorDisplayButton(CustomButtonStyle buttonStyle, SubmitButtonStyle submitStyle = null)
    {
        _buttonUp = buttonStyle.ButtonUp;
        _buttonDown = buttonStyle.ButtonDown;
        _buttonIcon = buttonStyle.ButtonIcon;
        _submitCallback = buttonStyle.ButtonSubmitAction;
        _buttonUp.wrapMode = TextureWrapMode.Repeat;
        _buttonContent = new GUIContent(_buttonUp);
        _activeTex = _buttonUp;

        //_buttonTextField = string.Empty;
        _buttonTitle = buttonStyle.ButtonTitle;

        if (submitStyle != null)
        {
            _submitButtonVisibile = true;
            _submitButtonTitle = submitStyle.SubmitTitle;
            _submitButtonAction = submitStyle.SubmitSuccess;
            _submitButtonWidth = submitStyle.SubmitButtonWidth;
            _submitButtonHeight = submitStyle.SubmitButtonHeight;
        }

        _buttonHeight = buttonStyle.ButtonHeight;

        if (buttonStyle.ButtonStyle != null)
            _buttonStyle = buttonStyle.ButtonStyle;
        else
        {
            _buttonStyle = new GUIStyle();

        }
    }

    public virtual void Update(Event windowEvent, GUIStyle fontStyle, Editor window, int width, int height)
    { }

    public virtual void UpdateBits(ushort position, ushort scale, ushort rotation)
    { }
}



public class PositionTweenButton : EditorDisplayButton
{
    #region Position Tween
    private bool _interpolationPrev = false;
    private bool _interpolation = false;

    private const int ADJUSTABLE_HEIGHT_MIN = -10;
    private const int ADJUSTABLE_HEIGHT_MAX = -5;

    public bool PositionTween
    {
        get { return _interpolation; }
        set
        {
            if (value != _interpolationPrev)
            {
                _interpolation = value;
                _interpolationPrev = value;

                if (_interpolationChanged != null)
                    _interpolationChanged(value);
            }
        }
    }
    private System.Action<bool> _interpolationChanged;
    #endregion

    #region Tween Time
    private float m_tweenTimePrev = 0;
    private float m_tweenTime = 0;
    public float TweenTime
    {
        get { return m_tweenTime; }
        set
        {
            if (value != m_tweenTimePrev)
            {
                m_tweenTime = value;
                m_tweenTimePrev = value;

                if (m_tweenTimeChanged != null)
                    m_tweenTimeChanged(value);
            }
        }
    }
    private System.Action<float> m_tweenTimeChanged;
    private WidgetTween m_widgetTween;
    private TweenType m_type;
    #endregion



    private string _toolTiptext;
    private bool _interpolationButtonVisible;

    public PositionTweenButton(CustomButtonStyle buttonStyle, SubmitButtonStyle submitStyle = null)
        : base(buttonStyle, submitStyle)
    {
    }

    public void Initialize(TweenType type, WidgetTween wTween, bool wTweens, float tweenTime, string tooltipText, System.Action<bool> positionChanged, System.Action<float> tweenTimeChanged)
    {
        PositionTween = wTweens;

        _toolTiptext = tooltipText;

        _interpolationChanged = positionChanged;
        m_tweenTimeChanged = tweenTimeChanged;


        if (!_submitButtonVisibile)
        {
            _submitButtonVisibile = true;
            _submitButtonTitle = PositionTween ? "Disable" : "Enable";
        }

        m_widgetTween = wTween;
        TweenTime = tweenTime;
        m_type = type;
    }

    public override void Update(Event windowEvent, GUIStyle fontStyle, Editor window, int width, int height)
    {
        if (_activeTex == null)
            return;

        Rect original = GUILayoutUtility.GetRect(_buttonContent, _buttonStyle, GUILayout.Height(_buttonHeight));
        _buttonRect = original;


        Rect textField = new Rect(original);
        textField.xMin += 10;
        textField.xMax -= width * 0.32f;
        textField.yMin += 10;
        textField.yMax -= 20;

        Rect iconField = new Rect(original);
        iconField.xMin += 10;
        iconField.xMax -= (width - 60);
        iconField.yMin += 5;
        iconField.yMax -= 35;

        Rect lerpSpeedLabelField = new Rect(original);
        lerpSpeedLabelField.xMin += 10;
        lerpSpeedLabelField.xMax -= (width - 100);
        lerpSpeedLabelField.yMin += 65;
        lerpSpeedLabelField.yMax -= 92;



        Rect lerpPositionField = new Rect(original);
        lerpPositionField.xMin += 10;
        //lerpPositionField.xMin += 65;
        lerpPositionField.xMax -= (width - (width * 0.35f));
        lerpPositionField.yMin += 105 + ADJUSTABLE_HEIGHT_MIN;
        lerpPositionField.yMax -= 65 + ADJUSTABLE_HEIGHT_MAX;

        Rect lerpPositionXField = new Rect(original);
        lerpPositionXField.xMin += (width - (width * 0.68f));
        lerpPositionXField.xMax -= (width - (width * 0.55f));
        lerpPositionXField.yMin += 105 + ADJUSTABLE_HEIGHT_MIN;
        lerpPositionXField.yMax -= 65 + ADJUSTABLE_HEIGHT_MAX;

        Rect lerpPositionYField = new Rect(original);
        lerpPositionYField.xMin += (width - (width * 0.48f));
        lerpPositionYField.xMax -= (width - (width * 0.75f));
        lerpPositionYField.yMin += 105 + ADJUSTABLE_HEIGHT_MIN;
        lerpPositionYField.yMax -= 65 + ADJUSTABLE_HEIGHT_MAX;

        Rect lerpPositionZField = new Rect(original);
        lerpPositionZField.xMin += (width - (width * 0.275f));
        lerpPositionZField.xMax -= (width - (width * 0.96f));
        lerpPositionZField.yMin += 105 + ADJUSTABLE_HEIGHT_MIN;
        lerpPositionZField.yMax -= 65 + ADJUSTABLE_HEIGHT_MAX;

        Rect lerpRotationField = new Rect(original);
        lerpRotationField.xMin += 10;
        //lerpRotationField.xMin += 65;
        lerpRotationField.xMax -= (width - (width * 0.35f));
        lerpRotationField.yMin += 140 + ADJUSTABLE_HEIGHT_MIN - 10;
        lerpRotationField.yMax -= 30 + ADJUSTABLE_HEIGHT_MAX + 10;

        Rect lerpRotationXField = new Rect(original);
        lerpRotationXField.xMin += (width - (width * 0.68f));
        lerpRotationXField.xMax -= (width - (width * 0.55f));
        lerpRotationXField.yMin += 140 + ADJUSTABLE_HEIGHT_MIN - 10;
        lerpRotationXField.yMax -= 30 + ADJUSTABLE_HEIGHT_MAX + 10;

        Rect lerpRotationYField = new Rect(original);
        lerpRotationYField.xMin += (width - (width * 0.48f));
        lerpRotationYField.xMax -= (width - (width * 0.75f));
        lerpRotationYField.yMin += 140 + ADJUSTABLE_HEIGHT_MIN - 10;
        lerpRotationYField.yMax -= 30 + ADJUSTABLE_HEIGHT_MAX + 10;

        Rect lerpRotationZField = new Rect(original);
        lerpRotationZField.xMin += (width - (width * 0.275f));
        lerpRotationZField.xMax -= (width - (width * 0.96f));
        lerpRotationZField.yMin += 140 + ADJUSTABLE_HEIGHT_MIN - 10;
        lerpRotationZField.yMax -= 30 + ADJUSTABLE_HEIGHT_MAX + 10;

        Rect lerpScaleField = new Rect(original);
        lerpScaleField.xMin += 10;
        //lerpScaleField.xMin += 65;
        lerpScaleField.xMax -= (width - (width * 0.35f));
        lerpScaleField.yMin += 175 + ADJUSTABLE_HEIGHT_MIN - 20;
        lerpScaleField.yMax -= -5 + ADJUSTABLE_HEIGHT_MAX + 20;

        Rect lerpScaleXField = new Rect(original);
        lerpScaleXField.xMin += (width - (width * 0.68f));
        lerpScaleXField.xMax -= (width - (width * 0.55f));
        lerpScaleXField.yMin += 175 + ADJUSTABLE_HEIGHT_MIN - 20;
        lerpScaleXField.yMax -= -5 + ADJUSTABLE_HEIGHT_MAX + 20;

        Rect lerpScaleYField = new Rect(original);
        lerpScaleYField.xMin += (width - (width * 0.48f));
        lerpScaleYField.xMax -= (width - (width * 0.75f));
        lerpScaleYField.yMin += 175 + ADJUSTABLE_HEIGHT_MIN - 20;
        lerpScaleYField.yMax -= -5 + ADJUSTABLE_HEIGHT_MAX + 20;

        Rect lerpScaleZField = new Rect(original);
        lerpScaleZField.xMin += (width - (width * 0.275f));
        lerpScaleZField.xMax -= (width - (width * 0.96f));
        lerpScaleZField.yMin += 175 + ADJUSTABLE_HEIGHT_MIN - 20;
        lerpScaleZField.yMax -= -5 + ADJUSTABLE_HEIGHT_MAX + 20;


        if (windowEvent.isMouse && _buttonRect.Contains(windowEvent.mousePosition))
        {
            if (windowEvent.type == EventType.MouseDown)
            {
                if (_buttonDown != null)
                {
                    _activeTex = _buttonDown;
                    _buttonContent.image = _buttonUp;
                }
            }
            else if (windowEvent.type == EventType.MouseUp)
            {
                if (_buttonUp != null)
                {
                    _activeTex = _buttonUp;
                    _buttonContent.image = _buttonUp;
                }

                if (_submitCallback != null)
                    _submitCallback();
            }
        }
        else if (windowEvent.isMouse)
        {
            if (_buttonUp != null)
            {
                _activeTex = _buttonUp;
                _buttonContent.image = _buttonUp;
            }
        }

        GUI.DrawTexture(_buttonRect, _activeTex, ScaleMode.StretchToFill);

        if (_buttonIcon != null)
            GUI.DrawTexture(iconField, _buttonIcon, ScaleMode.StretchToFill);

        if (fontStyle != null)
            GUI.Label(_buttonRect, _buttonTitle, fontStyle);

        if (!string.IsNullOrEmpty(_toolTiptext))
        {
            GUI.color = Color.yellow;
            GUI.TextArea(textField, _toolTiptext);
            GUI.color = Color.white;
        }

        if (_submitButtonVisibile)
        {
            bool fullScreenButton = string.IsNullOrEmpty(_toolTiptext);

            Rect submitField = new Rect(original);
            submitField.xMin += !fullScreenButton ? (width * 0.68f) + _submitButtonWidth : 10 + _submitButtonWidth;
            submitField.xMax -= 10 - _submitButtonWidth;
            submitField.yMin += 20 + _submitButtonHeight;
            submitField.yMax -= 125 - _submitButtonHeight;

            Rect lerpSpeedLabel = new Rect(original);
            lerpSpeedLabel.xMin += 10;
            lerpSpeedLabel.xMax -= 10;
            lerpSpeedLabel.yMin += 52;
            lerpSpeedLabel.yMax -= 75;

            Rect lerpSpeedField = new Rect(original);
            lerpSpeedField.xMin += (width * 0.25f) + _submitButtonWidth;
            lerpSpeedField.xMax -= 10 - _submitButtonWidth;
            lerpSpeedField.yMin += 65 + _submitButtonHeight;
            lerpSpeedField.yMax -= 85;



            _submitButtonTitle = PositionTween ? "Disable" : "Enable";

            GUI.color = PositionTween ? Color.green : Color.gray;

            if (GUI.Button(submitField, _submitButtonTitle))
            {
                PositionTween = !PositionTween;
                _submitButtonTitle = PositionTween ? "Disable" : "Enable";

                if (_submitButtonAction != null)
                    _submitButtonAction();
            }
            GUI.color = Color.white;

            GUI.Label(lerpSpeedLabel, "Tween Time", fontStyle);



            GUI.enabled = PositionTween;

            TweenTime = GUI.HorizontalSlider(lerpSpeedField, TweenTime, 0f, 10f);  // TODO : Replace magic numbers with consts

            float tempLerpSpeed = TweenTime;
            if (float.TryParse(GUI.TextField(lerpSpeedLabelField, TweenTime.ToString()), out tempLerpSpeed))
            {
                TweenTime = tempLerpSpeed;
            }


            #region Lerp Position GUI


            if (GUI.Button(lerpPositionField, "From"))
            {
                switch (m_type)
                {
                    case TweenType.Position:
                        var newPos = m_widgetTween.gameObject.transform.position; //.GetComponent<RectTransform>()
                        Debug.Log("Pos : " + newPos);

                        string[] dimension = UnityEditor.UnityStats.screenRes.Split('x');
                        newPos.x /= System.Int32.Parse(dimension[0]);
                        newPos.y /= System.Int32.Parse(dimension[1]);

                        m_widgetTween.TweenData.sPosition.From = newPos;
                        break;
                    case TweenType.Scale:
                        m_widgetTween.TweenData.sScale.From = m_widgetTween.gameObject.transform.localScale;
                        break;
                    case TweenType.Rotation:
                        m_widgetTween.TweenData.sRotation.From = m_widgetTween.gameObject.transform.rotation.eulerAngles;
                        break;

                }

            }


            switch (m_type)
            {
                case TweenType.Position:
                    GUI.Button(lerpPositionXField, "X:" + (System.Math.Round(m_widgetTween.TweenData.sPosition.From.x, 2).ToString()));
                    GUI.color = Color.white;

                    GUI.Button(lerpPositionYField, "Y:" + (System.Math.Round(m_widgetTween.TweenData.sPosition.From.y, 2).ToString()));
                    GUI.color = Color.white;

                    GUI.Button(lerpPositionZField, "Z:" + (System.Math.Round(m_widgetTween.TweenData.sPosition.From.z, 2).ToString()));
                    GUI.color = Color.white;

                    break;
                case TweenType.Scale:
                    GUI.Button(lerpPositionXField, "X:" + (System.Math.Round(m_widgetTween.TweenData.sScale.From.x, 2).ToString()));
                    GUI.color = Color.white;

                    GUI.Button(lerpPositionYField, "Y:" + (System.Math.Round(m_widgetTween.TweenData.sScale.From.y, 2).ToString()));
                    GUI.color = Color.white;

                    GUI.Button(lerpPositionZField, "Z:" + (System.Math.Round(m_widgetTween.TweenData.sScale.From.z, 2).ToString()));
                    GUI.color = Color.white;
                    break;
                case TweenType.Rotation:

                    GUI.Button(lerpPositionXField, "X:" + (System.Math.Round(m_widgetTween.TweenData.sRotation.From.x, 2).ToString()));
                    GUI.color = Color.white;

                    GUI.Button(lerpPositionYField, "Y:" + (System.Math.Round(m_widgetTween.TweenData.sRotation.From.y, 2).ToString()));
                    GUI.color = Color.white;

                    GUI.Button(lerpPositionZField, "Z:" + (System.Math.Round(m_widgetTween.TweenData.sRotation.From.z, 2).ToString()));
                    GUI.color = Color.white;

                    break;
            }



            #endregion



            #region Lerp Rotation GUI

            if (GUI.Button(lerpRotationField, "To"))
            {
                switch (m_type)
                {
                    case TweenType.Position:
                        var newPos = m_widgetTween.gameObject.transform.position;//.GetComponent<RectTransform>()
                        string[] dimension = UnityEditor.UnityStats.screenRes.Split('x');
                        newPos.x /= System.Int32.Parse(dimension[0]);
                        newPos.y /= System.Int32.Parse(dimension[1]);

                        m_widgetTween.TweenData.sPosition.To = newPos;
                        break;
                    case TweenType.Scale:
                        m_widgetTween.TweenData.sScale.To = m_widgetTween.gameObject.transform.localScale;
                        break;
                    case TweenType.Rotation:
                        m_widgetTween.TweenData.sRotation.To = m_widgetTween.gameObject.transform.rotation.eulerAngles;
                        break;

                }
            }


            switch (m_type)
            {
                case TweenType.Position:
                    GUI.Button(lerpRotationXField, "X:" + (System.Math.Round(m_widgetTween.TweenData.sPosition.To.x, 2).ToString()));
                    GUI.color = Color.white;

                    GUI.Button(lerpRotationYField, "Y:" + (System.Math.Round(m_widgetTween.TweenData.sPosition.To.y, 2).ToString()));
                    GUI.color = Color.white;

                    GUI.Button(lerpRotationZField, "Z:" + (System.Math.Round(m_widgetTween.TweenData.sPosition.To.z, 2).ToString()));
                    GUI.color = Color.white;

                    break;
                case TweenType.Scale:
                    GUI.Button(lerpRotationXField, "X:" + (System.Math.Round(m_widgetTween.TweenData.sScale.To.x, 2).ToString()));
                    GUI.color = Color.white;

                    GUI.Button(lerpRotationYField, "Y:" + (System.Math.Round(m_widgetTween.TweenData.sScale.To.y, 2).ToString()));
                    GUI.color = Color.white;

                    GUI.Button(lerpRotationZField, "Z:" + (System.Math.Round(m_widgetTween.TweenData.sScale.To.z, 2).ToString()));
                    GUI.color = Color.white;
                    break;
                case TweenType.Rotation:

                    GUI.Button(lerpRotationXField, "X:" + (System.Math.Round(m_widgetTween.TweenData.sRotation.To.x, 2).ToString()));
                    GUI.color = Color.white;

                    GUI.Button(lerpRotationYField, "Y:" + (System.Math.Round(m_widgetTween.TweenData.sRotation.To.y, 2).ToString()));
                    GUI.color = Color.white;

                    GUI.Button(lerpRotationZField, "Z:" + (System.Math.Round(m_widgetTween.TweenData.sRotation.To.z, 2).ToString()));
                    GUI.color = Color.white;

                    break;
            }



            #endregion

            #region Lerp Scale GUI
            //  GUI.color = LerpScale ? Color.white : Color.gray;




            if (GUI.Button(lerpScaleField, "Hide"))
            {

                switch (m_type)
                {
                    case TweenType.Position:
                        var newPos = m_widgetTween.gameObject.transform.position; //.GetComponent<RectTransform>()
                        string[] dimension = UnityEditor.UnityStats.screenRes.Split('x');
                        newPos.x /= System.Int32.Parse(dimension[0]);
                        newPos.y /= System.Int32.Parse(dimension[1]);

                        m_widgetTween.TweenData.ePosition = newPos;
                        break;
                    case TweenType.Scale:
                        m_widgetTween.TweenData.eScale = m_widgetTween.gameObject.transform.localScale;
                        break;
                    case TweenType.Rotation:
                        m_widgetTween.TweenData.eRotation = m_widgetTween.gameObject.transform.rotation.eulerAngles;
                        break;

                }
            }


            switch (m_type)
            {
                case TweenType.Position:
                    GUI.Button(lerpScaleXField, "X:" + (System.Math.Round(m_widgetTween.TweenData.ePosition.x, 2).ToString()));
                    GUI.color = Color.white;

                    GUI.Button(lerpScaleYField, "Y:" + (System.Math.Round(m_widgetTween.TweenData.ePosition.y, 2).ToString()));
                    GUI.color = Color.white;

                    GUI.Button(lerpScaleZField, "Z:" + (System.Math.Round(m_widgetTween.TweenData.ePosition.z, 2).ToString()));
                    GUI.color = Color.white;

                    break;
                case TweenType.Scale:
                    GUI.Button(lerpScaleXField, "X:" + (System.Math.Round(m_widgetTween.TweenData.eScale.x, 2).ToString()));
                    GUI.color = Color.white;

                    GUI.Button(lerpScaleYField, "Y:" + (System.Math.Round(m_widgetTween.TweenData.eScale.y, 2).ToString()));
                    GUI.color = Color.white;

                    GUI.Button(lerpScaleZField, "Z:" + (System.Math.Round(m_widgetTween.TweenData.eScale.z, 2).ToString()));
                    GUI.color = Color.white;
                    break;
                case TweenType.Rotation:

                    GUI.Button(lerpScaleXField, "X:" + (System.Math.Round(m_widgetTween.TweenData.eRotation.x, 2).ToString()));
                    GUI.color = Color.white;

                    GUI.Button(lerpScaleYField, "Y:" + (System.Math.Round(m_widgetTween.TweenData.eRotation.y, 2).ToString()));
                    GUI.color = Color.white;

                    GUI.Button(lerpScaleZField, "Z:" + (System.Math.Round(m_widgetTween.TweenData.eRotation.z, 2).ToString()));
                    GUI.color = Color.white;

                    break;
            }



            #endregion

            GUI.enabled = true;
        }

        if (_prevTex != _activeTex)
        {
            _prevTex = _activeTex;
            window.Repaint();
        }
    }
}
