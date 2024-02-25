using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

#if USE_InputSystem
using UnityEngine.InputSystem;
#endif
public enum InputDeviceType
{
    PS4 = 1,
    PS5 = 2,
    GamePad = 3,
    KeyMouse = 4,
    Xbox = 5,
}
public sealed partial class UIBeginnerGuideGamePad : GuideWidgetBase
{
    public const string DEVICE_GAMEPAD_NAME = "Gamepad";
    public const string DEVICE_MOUSE_NAME = "Mouse";
    public const string DEVICE_KEYBOARD_NAME = "Keyboard";
    public const string DEVICE_PS4_NAME = "DualShock4GamepadHID";
#if UNITY_EDITOR
    public const string DEVICE_PS5_NAME = "DualSenseGamepadPC";
#else
	public const string DEVICE_PS5_NAME = "PS5DualSenseGamepad";
#endif

    [SerializeField]
    UIBeginnerGuideGamePadSerialize serialize;

    private GuideGamePadData gamepadData;

    private Coroutine animationCoroutine;


    public override void Init(GuideWidgetData data)
    {
#if USE_InputSystem
        gamepadData = data as GuideGamePadData;
        if (gamepadData == null) return;
        foreach (var guide in gamepadData.guideList)
        {
            foreach (var action in guide.keys)
            {
                if (string.IsNullOrEmpty(action.actionName)) action.actions = null;
                else
                {
                    var inputAction = inputActionAsset.FindAction(action.actionName);
                    action.actions = InputActionReference.Create(inputAction);
                }
            }
        }
        gamepadData.ApplyTransformData(transform);
#endif
    }

    public override List<int> GetControlledInstanceIds()
    {
        List<int> list = new List<int>();
        return list;
    }

    public override void Show()
    {
#if USE_InputSystem
        ShowGuideDescAnim(gamepadData.GamePadAnimStr);
#endif
    }
    public override void Stop()
    {
#if USE_InputSystem
        StopAllCoroutines();
        serialize.ContainerAnimator.speed = 0;
#endif
    }

    private InputDeviceType GetNowGamepadType()
    {
        var deviceType = InputDeviceType.GamePad;
#if USE_InputSystem
        var currentGamePadName = Gamepad.current.name;
        if (currentGamePadName == DEVICE_PS4_NAME)
            deviceType = InputDeviceType.PS4;
        else if (currentGamePadName == DEVICE_PS5_NAME)
            deviceType = InputDeviceType.PS5;
        else
        {
            if (UXPlatform.IsXboxSeries())
                deviceType = InputDeviceType.Xbox;
            else
                deviceType = InputDeviceType.GamePad;
        }
#endif
        return deviceType;
    }

    private string GetCustomKeyNameByActionName(string actionName)
    {
        string joystickKey = null;
#if USE_InputSystem
        string joystickCtrlKey = null;
        InputAction action = inputActionAsset.FindAction(actionName);

        foreach (var binding in action.bindings)
        {
            var lowerName = "button";
            if ((binding.name?.ToLower() == lowerName && binding.isPartOfComposite) || !binding.isPartOfComposite)
            {
                joystickKey = binding.effectivePath;
            }
            if (binding.name?.ToLower() != lowerName && binding.isPartOfComposite)
            {
                joystickCtrlKey = binding.effectivePath;
            }
        }
#endif
        return joystickKey;
    }

    private Dictionary<InputDeviceType, GameObject> joystickTypeDic;
    private Dictionary<InputDeviceType, Dictionary<string, Transform>> joystickTypeButtonDic;
    private Dictionary<string, Transform> joystickXboxButtonDic;
    private Dictionary<string, Transform> joystickPcButtonDic;
    private Dictionary<string, Transform> joystickPsButtonDic;
    private Dictionary<string, Transform> joystickPs5ButtonDic;

    private Dictionary<string, Transform> joystickButtonDic;
    private List<(float duration, List<string> actionNames)> joystickAnimInfos;
    private int curTipAnimIndex = 0;

#if USE_InputSystem
    public InputActionAsset inputActionAsset;
#endif

    private void Awake()
    {
        InitJoystickObject();
    }

    private void InitJoystickObject()
    {
        joystickTypeDic = new Dictionary<InputDeviceType, GameObject>()
        {
            {InputDeviceType.Xbox, serialize.Xbox.gameObject },
            {InputDeviceType.GamePad, serialize.PC.gameObject },
            {InputDeviceType.PS4, serialize.PS.gameObject },
            {InputDeviceType.PS5, serialize.PS5.gameObject },
        };

        joystickXboxButtonDic = new Dictionary<string, Transform>()
        {
            { "<Gamepad>/leftTrigger",serialize.Xbox_lt.transform },
            { "<Gamepad>/leftShoulder",serialize.Xbox_lb.transform },
            { "<Gamepad>/rightTrigger",serialize.Xbox_rt.transform },
            { "<Gamepad>/rightShoulder",serialize.Xbox_rb.transform },
            { "<Gamepad>/dpad/left",serialize.Xbox_left.transform },
            { "<Gamepad>/dpad/right",serialize.Xbox_right.transform },
            { "<Gamepad>/dpad/down",serialize.Xbox_down.transform },
            { "<Gamepad>/dpad/up",serialize.Xbox_up.transform },
            { "<Gamepad>/buttonSouth",serialize.Xbox_buttonSouth.transform },
            { "<Gamepad>/buttonEast",serialize.Xbox_buttonEast.transform },
            { "<Gamepad>/buttonWest",serialize.Xbox_buttonWest.transform },
            { "<Gamepad>/buttonNorth",serialize.Xbox_buttonNorth.transform },
            { "<Gamepad>/start",serialize.Xbox_start.transform },
            { "<Gamepad>/select",serialize.Xbox_select.transform },
            { "<Gamepad>/rightStick",serialize.Xbox_rightstick.transform },
            { "<Gamepad>/rightStickPress",serialize.Xbox_rightstick.transform },
            { "<Gamepad>/leftStick",serialize.Xbox_leftstick.transform },
            { "<Gamepad>/leftStickPress",serialize.Xbox_leftstick.transform },
        };

        joystickPcButtonDic = new Dictionary<string, Transform>()
        {
            { "<Gamepad>/leftTrigger",serialize.PC_lt.transform },
            { "<Gamepad>/leftShoulder",serialize.PC_lb.transform },
            { "<Gamepad>/rightTrigger",serialize.PC_rt.transform },
            { "<Gamepad>/rightShoulder",serialize.PC_rb.transform },
            { "<Gamepad>/dpad/left",serialize.PC_left.transform },
            { "<Gamepad>/dpad/right",serialize.PC_right.transform },
            { "<Gamepad>/dpad/down",serialize.PC_down.transform },
            { "<Gamepad>/dpad/up",serialize.PC_up.transform },
            { "<Gamepad>/buttonSouth",serialize.PC_buttonSouth.transform },
            { "<Gamepad>/buttonEast",serialize.PC_buttonEast.transform },
            { "<Gamepad>/buttonWest",serialize.PC_buttonWest.transform },
            { "<Gamepad>/buttonNorth",serialize.PC_buttonNorth.transform },
            { "<Gamepad>/start",serialize.PC_start.transform },
            { "<Gamepad>/select",serialize.PC_select.transform },
            { "<Gamepad>/rightStick",serialize.PC_rightstick.transform },
            { "<Gamepad>/rightStickPress",serialize.PC_rightstick.transform },
            { "<Gamepad>/leftStick",serialize.PC_leftstick.transform },
            { "<Gamepad>/leftStickPress",serialize.PC_leftstick.transform },
        };

        joystickPsButtonDic = new Dictionary<string, Transform>()
        {
            { "<Gamepad>/leftTrigger",serialize.PS_lt.transform },
            { "<Gamepad>/leftShoulder",serialize.PS_lb.transform },
            { "<Gamepad>/rightTrigger",serialize.PS_rt.transform },
            { "<Gamepad>/rightShoulder",serialize.PS_rb.transform },
            { "<Gamepad>/dpad/left",serialize.PS_left.transform },
            { "<Gamepad>/dpad/right",serialize.PS_right.transform },
            { "<Gamepad>/dpad/down",serialize.PS_down.transform },
            { "<Gamepad>/dpad/up",serialize.PS_up.transform },
            { "<Gamepad>/buttonSouth",serialize.PS_buttonSouth.transform },
            { "<Gamepad>/buttonEast",serialize.PS_buttonEast.transform },
            { "<Gamepad>/buttonWest",serialize.PS_buttonWest.transform },
            { "<Gamepad>/buttonNorth",serialize.PS_buttonNorth.transform },
            { "<Gamepad>/start",serialize.PS_start.transform },
            { "<Gamepad>/select",serialize.PS_select.transform },
            { "<Gamepad>/rightStick",serialize.PS_rightstick.transform },
            { "<Gamepad>/rightStickPress",serialize.PS_rightstick.transform },
            { "<Gamepad>/leftStick",serialize.PS_leftstick.transform },
            { "<Gamepad>/leftStickPress",serialize.PS_leftstick.transform },
        };

        joystickPs5ButtonDic = new Dictionary<string, Transform>()
        {
            { "<Gamepad>/leftTrigger",serialize.PS5_lt.transform },
            { "<Gamepad>/leftShoulder",serialize.PS5_lb.transform },
            { "<Gamepad>/rightTrigger",serialize.PS5_rt.transform },
            { "<Gamepad>/rightShoulder",serialize.PS5_rb.transform },
            { "<Gamepad>/dpad/left",serialize.PS5_left.transform },
            { "<Gamepad>/dpad/right",serialize.PS5_right.transform },
            { "<Gamepad>/dpad/down",serialize.PS5_down.transform },
            { "<Gamepad>/dpad/up",serialize.PS5_up.transform },
            { "<Gamepad>/buttonSouth",serialize.PS5_buttonSouth.transform },
            { "<Gamepad>/buttonEast",serialize.PS5_buttonEast.transform },
            { "<Gamepad>/buttonWest",serialize.PS5_buttonWest.transform },
            { "<Gamepad>/buttonNorth",serialize.PS5_buttonNorth.transform },
            { "<Gamepad>/start",serialize.PS5_start.transform },
            { "<Gamepad>/select",serialize.PS5_select.transform },
            { "<Gamepad>/rightStick",serialize.PS5_rightstick.transform },
            { "<Gamepad>/rightStickPress",serialize.PS5_rightstick.transform },
            { "<Gamepad>/leftStick",serialize.PS5_leftstick.transform },
            { "<Gamepad>/leftStickPress",serialize.PS5_leftstick.transform },
        };

        joystickTypeButtonDic = new Dictionary<InputDeviceType, Dictionary<string, Transform>>()
        {
            {InputDeviceType.Xbox, joystickXboxButtonDic },
            {InputDeviceType.GamePad, joystickPcButtonDic },
            {InputDeviceType.PS4, joystickPsButtonDic },
            {InputDeviceType.PS5, joystickPs5ButtonDic },
        };

    }

    public void ShowGuideDescAnim(string guideDescAnim)
    {
        UpdateCurrentJoystickType();

        if (!string.IsNullOrEmpty(guideDescAnim))
        {
            InitJoystickTipsAnimList(guideDescAnim);
            curTipAnimIndex = 0;
            ChangeGuideAnim();
        }
        else
        {
            foreach (var joystickButton in joystickButtonDic.Values)
            {
                joystickButton.gameObject.SetActive(false);
            }
        }
    }

    private static Regex actionKeyTipRegex = new Regex(@"\{(\w+)\}");
    private void InitJoystickTipsAnimList(string featureDescAnim)
    {
        joystickAnimInfos = new List<(float duration, List<string> joystickNames)>();

        //featureDescAnim = "3,{HeroSkill1}+{DPad_Down}|5|3,{Action4}";
        //tipsAnimString = "3,{HeroSkill1}+{DPad_Down}|3,{Action4}";

        string[] joystickTipsAnimList = featureDescAnim.Split('|');

        for (int i = 0; i < joystickTipsAnimList.Length; i++)
        {
            string animString = joystickTipsAnimList[i];
            string[] animParams = animString.Split(',');
            float animDuration = float.Parse(animParams[0]);
            List<string> actionNames = new List<string>();
            if (animParams.Length > 1)
            {
                foreach (Match match in actionKeyTipRegex.Matches(animParams[1]))
                {
                    actionNames.Add(match.Groups[1].Value);
                }
            }
            joystickAnimInfos.Add((animDuration, actionNames));
        }
    }

    private void UpdateCurrentJoystickType()
    {
        InputDeviceType joystickType = InputDeviceType.GamePad; // GetNowGamepadType();
        foreach (InputDeviceType key in joystickTypeDic.Keys)
        {
            joystickTypeDic[key].SetActive(joystickType == key);
        }

        joystickButtonDic = joystickTypeButtonDic[joystickType];
    }

    private IEnumerator ChangeGuideAnimTimer(float time)
    {
        yield return new WaitForSeconds(time);
        ChangeGuideAnim();
    }

    private void ChangeGuideAnim()
    {
        UpdateJoystickTipsPos(curTipAnimIndex);
        UpdateJoystickTipsAnim(curTipAnimIndex);
        curTipAnimIndex++;
        if (curTipAnimIndex > joystickAnimInfos.Count - 1)
        {
            curTipAnimIndex = 0;
        }
    }

    private void UpdateJoystickTipsPos(int animIndex)
    {
        var animInfo = joystickAnimInfos[animIndex];

        List<string> joystickNames = new List<string>();
        foreach (string actionName in animInfo.actionNames)
        {
            string joystickname = GetCustomKeyNameByActionName(actionName);
            joystickNames.Add(joystickname);
        }

        foreach (string key in joystickButtonDic.Keys)
        {
            if (joystickNames.Contains(key))
            {
                joystickButtonDic[key].gameObject.SetActive(true);
            }
            else
            {
                joystickButtonDic[key].gameObject.SetActive(false);
            }
        }
    }

    private void UpdateJoystickTipsAnim(int animIndex)
    {
        serialize.ContainerAnimator.Play("GamePadGuide", -1, 0);

        var animInfo = joystickAnimInfos[animIndex];

        if (animInfo.duration != -1)
        {
            StartCoroutine(ChangeGuideAnimTimer(animInfo.duration));
        }
    }
}
