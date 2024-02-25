using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.Animations;
using UnityEngine.Playables;
using GameLogic.Common;
using NaughtyAttributes;
#if UNITY_EDITOR
using UnityEditor;
using System.Text.RegularExpressions;
using UnityEditor.Animations;

#endif


[Serializable]
public sealed class UIAnimState
{
    public string name;

    [Range(0, 1), Tooltip("暂时不支持"), HideInInspector]
    public float crossFadeTime;
    [Tooltip("勾选后程序如果重复进入该状态时动画重播，否则不作处理")]
    public bool replay = true;
    [Tooltip("循环模式，LoopAll循环全部动画，LoopLast单曲循环最后一个动画，Pause播完后暂停在最后一帧，Quit播完后退出")]
    public PlayQueuePB.EndingType loop;

    public AnimationClip[] animationQueue;

#if UNITY_EDITOR
    public bool Check(bool isTransient)
    {
        if (string.IsNullOrEmpty(name))
        {
            EditorUtility.DisplayDialog("State 错误", "State Name不能为空", "确认");
            return false;
        }

        if (!Regex.IsMatch(name, @"^[\w\d_]*$"))
        {
            EditorUtility.DisplayDialog("State 错误", $"State Name: {name} 不合法（只能由大小写、数字和_组成）", "确认");
            return false;
        }

        if (animationQueue == null || animationQueue.Length == 0)
        {
            EditorUtility.DisplayDialog("State 错误", "Animation Queue不能为空", "确认");
            return false;
        }

        for (int i = 0; i < animationQueue.Length; i++)
        {

            if (animationQueue[i] == null)
            {
                EditorUtility.DisplayDialog("Animation 错误", "Animation不能为空", "确认");
                return false;
            }

            if (animationQueue[i].frameRate != 30)
            {
                EditorUtility.DisplayDialog("Animation 错误", $"UI Animation必须为30帧 stateName:{name}", "确认");
                return false;
            }

            if (animationQueue[i].legacy)
            {
                EditorUtility.DisplayDialog("Animation 错误", "Legacy Animation cannot be used in Playables " + animationQueue[i].name, "确认");
                return false;
            }

            if (animationQueue[i].isLooping)
            {
                EditorUtility.DisplayDialog("Animation 错误", "Animation 文件不能设置为loop : " + animationQueue[i].name, "确认");
                return false;
            }

            if (isTransient && (loop != PlayQueuePB.EndingType.Quit))
            {
                EditorUtility.DisplayDialog("State 错误", "Transient Layer中的State 是一过性无状态动画，loop只允许为Quit", "确认");
                return false;
            }

            if (!isTransient && (loop == PlayQueuePB.EndingType.Quit))
            {
                EditorUtility.DisplayDialog("State 错误", "非Transient Layer中的State不允许为Quit，因为状态由Layer接管", "确认");
                return false;
            }
        }


        return true;
    }

#endif
}

[Serializable]
public sealed class UIAnimStateLayer
{
    public string name;
    [Range(0, 1), Tooltip("暂时不支持"), HideInInspector]
    public float weight = 1;
    public float crossFadeTime = 0;
    [Tooltip("勾选后该层为简单动画集合，不保证状态唯一，所有动画都可以同时播放，但动画播放完后必须Quit。不勾选该层为状态机，进行状态管理，保证只有一个激活状态")]
    public bool transient;
    [LabelText("生成时跳过默认动画")]
    [Tooltip("勾选后UI生成时不播放该层默认状态的动画，直接跳至默认动画的最后一帧")]
    public bool skipDefault;

    public UIAnimState[] states;
    HashSet<string> stateNames;

#if UNITY_EDITOR
    public bool Check()
    {
        if (string.IsNullOrEmpty(name))
        {
            EditorUtility.DisplayDialog("Layer 错误", "Layer Name不能为空", "确认");
            return false;
        }

        if (states == null || states.Length == 0)
        {
            EditorUtility.DisplayDialog("Layer 错误", "states不能为空", "确认");
            return false;
        }

        foreach (var state in states)
        {
            if (!state.Check(transient))
            {
                return false;
            }
        }

        if (!Init())
        {
            EditorUtility.DisplayDialog("Layer 错误", "同一层States Name不能重复", "确认");
            return false;
        }

        return true;
    }

#endif

    public IEnumerable<string> GetStateNames()
    {
        return stateNames;
    }
    public bool Init()
    {
        stateNames = new HashSet<string>();
        foreach (var state in states)
        {
            if (stateNames.Contains(state.name))
            {
                return false;
            }

            stateNames.Add(state.name);
        }
        return true;
    }

}

[RequireComponent(typeof(Animator))]
[HelpURL("https://www.baidu.com")]
public sealed partial class UIStateAnimator : MonoBehaviour
{
    [ValueDropdown("GetTriggerName")]
    public string fadein;
    [ValueDropdown("GetTriggerName")]
    public string fadeout;
    public AnimOperationData LastOperation
    {
        get;
        private set;
    }

    public UIAnimStateLayer[] logicLayers;

#if UNITY_EDITOR
    //[Button("检查设置", DrawResult = false)]
    [Button("检查设置")]
    bool Check()
    {
        foreach (var layer in logicLayers)
        {
            if (!layer.Check())
                return false;
        }

        // 如果同时存在FadeIn 和 Fadeout动画
        if (!string.IsNullOrEmpty(fadein) && !string.IsNullOrEmpty(fadeout))
        {
            if (fadein == fadeout)
            {
                EditorUtility.DisplayDialog("fadein fadeout 错误", "fadein 和 fadeout 不能相同", "确认");
                return false;
            }

            UIAnimStateLayer fadeinLy = null, fadeoutLy = null;
            foreach (var ly in logicLayers)
            {
                foreach (var layerName in ly.GetStateNames())
                {
                    if (layerName == fadein)
                        fadeinLy = ly;
                    else if (layerName == fadeout)
                        fadeoutLy = ly;
                }
                if (fadeinLy != null && fadeinLy == fadeoutLy)
                    break;
            }

            if (fadeinLy == null)
            {
                EditorUtility.DisplayDialog("fadein 错误", "fadein State不存在，请重新选择", "确认");
                return false;
            }
            if (fadeoutLy == null)
            {
                EditorUtility.DisplayDialog("fadeout 错误", "fadeout State不存在，请重新选择", "确认");
                return false;
            }

            if (fadeinLy != fadeoutLy)
            {
                EditorUtility.DisplayDialog("fadein fadeout 错误", "fadein fadeout 必须在同一Layer", "确认");
                return false;
            }

            if (fadeinLy.transient)
            {
                EditorUtility.DisplayDialog("fadein fadeout 错误", "fadein fadeout 所在 Layer 不能为 Transient", "确认");
                return false;
            }
        }
        else if (!string.IsNullOrEmpty(fadein))
        {
            bool fadeInExist = CheckAnim(fadein, out UIAnimStateLayer fadeinLy);
            if (!fadeInExist)
            {
                EditorUtility.DisplayDialog("fadein 错误", "fadein State不存在，请重新选择", "确认");
                return false;
            }

            if (fadeinLy.transient)
            {
                EditorUtility.DisplayDialog("fadein 错误", "fadein 所在 Layer 不能为 Transient", "确认");
                return false;
            }
        }
        else if (!string.IsNullOrEmpty(fadeout))
        {
            bool fadeOutExist = CheckAnim(fadeout, out UIAnimStateLayer fadeoutLy);
            if (!fadeOutExist)
            {
                EditorUtility.DisplayDialog("fadeout 错误", "fadeout State不存在，请重新选择", "确认");
                return false;
            }

            if (fadeoutLy.transient)
            {
                EditorUtility.DisplayDialog("fadeout 错误", "fadeout 所在 Layer 不能为 Transient", "确认");
                return false;
            }
        }

        if (GetComponent<Animator>().runtimeAnimatorController != null)
        {
            EditorUtility.DisplayDialog("Animator 错误", "Animator Controller 没有清空！其他都正确，预览和提交前记得删除！", "确认");
            return false;
        }

        Debug.Log("没问题");

        return true;
    }

    bool CheckAnim(string stateName, out UIAnimStateLayer layer)
    {
        foreach (var ly in logicLayers)
        {
            foreach (var layerName in ly.GetStateNames())
            {
                if (layerName == stateName)
                {
                    layer = ly;
                    return true;
                }
            }
        }

        layer = null;
        return false;
    }

    //下拉列表显示函数，编辑器显示用;
    public string[] GetTriggerName()
    {
        if (this == null) return null;
        HashSet<string> triggerNameSet = new HashSet<string>();
        triggerNameSet.Add(string.Empty);
        if (logicLayers != null)
        {
            foreach (var layer in logicLayers)
            {
                layer.Init();
                foreach (var stateName in layer.GetStateNames())
                {
                    if (!triggerNameSet.Contains(stateName))
                        triggerNameSet.Add(stateName);
                }
            }
        }
        return triggerNameSet.ToArray();
    }
#endif

    public bool Initialized { get; private set; } = false;
    private void Awake()
    {
        if (!Initialized)
        {
            InitEx();
            if (UXGUIConfig.EnableOptimizeUIStateAnimator == false || need_update == true)
                _allaliveUIStateAnimators.Add(this);
        }
    }

    public void TryInit()
    {
        if (!Initialized)
        {
            InitEx();
            if (UXGUIConfig.EnableOptimizeUIStateAnimator == false || need_update == true)
                _allaliveUIStateAnimators.Add(this);
        }
    }

    private void OnDestroy()
    {
        Initialized = false;
        if (m_graph.IsValid())
        {
            m_graph.Destroy();
        }
        if (UXGUIConfig.EnableOptimizeUIStateAnimator == false || need_update == true)
            _allaliveUIStateAnimators.Remove(this);

        foreach (var data in timerDict)
        {
            var stateCallBack = data.Value;
            if (stateCallBack != null)
                stateCallBack.Cancel();
        }
        timerDict.Clear();
    }
    private void OnEnable()
    {
        Activate();
    }

    private void OnDisable()
    {
        Deactivate();
    }

    // 会被重复调用，这里面的逻辑需要保证幂等性
    private void Activate()
    {
        if (Initialized)
        {
            if (m_graph.IsValid() && !m_graph.IsPlaying())
            {
                m_graph.Play();

                //这个地方在再次激活的时候把配置为Transient的直接设置到尾帧
                SkipToTransientStateLastFrame();
            }
            if (UXGUIConfig.EnableOptimizeUIStateAnimator == false || need_update == true)
                _allaliveUIStateAnimators.Add(this);
        }
    }

    // 会被重复调用，这里面的逻辑需要保证幂等性
    private void Deactivate()
    {
        if (Initialized)
        {
            if (m_graph.IsValid() && m_graph.IsPlaying())
            {
                m_graph.Stop();
            }
            if (UXGUIConfig.EnableOptimizeUIStateAnimator == false || need_update == true)
                _allaliveUIStateAnimators.Remove(this);
        }
    }
    public void LXPrepareFrame(float deltaTime)
    {
        foreach (var layer in allLayerDatasList)
        {
            layer.LXPrepareFrame(deltaTime);
        }
    }

    [Button("预览(Runtime Only)")]
    public void SetStateInPreview(
#if UNITY_EDITOR
        [ValueDropdown("GetTriggerName")]
#endif
        string stateName,

        float speed = 1,
        float startTime = 0,
        [HideInInspector]
        Action<GameTimer> stateDoneCallBack = null
        )
    {
        if (!Initialized)
        {
            InitEx();
        }

        SetStateEx(stateName, speed, startTime, stateDoneCallBack);
    }



    public void SetState(
#if UNITY_EDITOR
        [ValueDropdown("GetTriggerName")]
#endif
        string stateName,

        float speed = 1,
        float startTime = 0,
        [HideInInspector]
        Action<GameTimer> stateDoneCallBack = null
        )
    {
        if (!Initialized)
        {
            return;
        }

        SetStateEx(stateName, speed, startTime, stateDoneCallBack);
    }

#if UNITY_EDITOR
    [Button("启用动画预览/编辑")]
    public void CreateController()
    {
        AnimatorController editorController = new AnimatorController();
        editorController.AddLayer("testLayer");
        var sm = editorController.layers[0].stateMachine;
        foreach (var layer in logicLayers)
        {
            foreach (var state in layer.states)
            {
                foreach (var anim in state.animationQueue)
                {
                    var s = sm.AddState(anim.name);
                    s.motion = anim;
                }
            }
        }
        GetComponent<Animator>().runtimeAnimatorController = editorController;
    }

    [Button("日志输入当前Graph各个节点状态")]
    public void printGraphNodeState()
    {
        Debug.Log("日志输入当前Graph各个节点状态开始");
        Debug.Log("m_graph IsPlaying: " + m_graph.IsPlaying() + " m_graph.IsValid: " + m_graph.IsValid() + " m_graph.IsDone: " + m_graph.IsDone());
        Debug.Log("m_graph.GetPlayableCount: " + m_graph.GetPlayableCount());
        var root = m_graph.GetRootPlayable(0);
        Debug.Log("rootPlayable input: " + root.GetInputCount());
        //Debug.Log(root.GetInputCount());
        //Debug.Log(root.GetOutputCount());
        for (int i = 0; i < allLayerDatasList.Count; ++i)
        {
            var layerNode = allLayerDatasList[i].layerMixer;
            Debug.Log($"------layerNode Index: <color=orange>[{i}]</color>  Name:{logicLayers[i].name} PlayState: {layerNode.GetPlayState()} IsValid: {layerNode.IsValid()} Weight: {root.GetInputWeight(i)} InputCount: {layerNode.GetInputCount()}");
            //for(int j = 0;j < layerNode.GetInputCount();)
            var states = allLayerDatasList[i].GetLayerStateDataList();
            for (int j = 0; j < states.Count; ++j)
            {
                var stateNode = states[j].stateMixer;
                var name = logicLayers[i].states[j].name;
                Debug.Log($">>>>>>>>>>>>stateNode Index: <color=red>[{j}]</color> Name:{name} PlayState: {stateNode.GetPlayState()} IsValid: {stateNode.IsValid()} Weight: {layerNode.GetInputWeight(j)} InputCount: {stateNode.GetInputCount()}");
                for (int k = 0; k < stateNode.GetInputCount(); ++k)
                {
                    AnimationClipPlayable clipNode = (AnimationClipPlayable)stateNode.GetInput(k);
                    Debug.Log($"#################### clipNode Index: <color=green>[{k}]</color> Name:{clipNode.GetAnimationClip().name} PlayState: {clipNode.GetPlayState()} IsValid: {clipNode.IsValid()} Weight: {stateNode.GetInputWeight(k)}  time:{clipNode.GetTime()}");
                }
            }
        }
        Debug.Log("日志输入当前Graph各个节点状态结束");
    }
#endif
}

public class PlayQueuePB
{
    public enum EndingType
    {
        Pause,
        Quit,
        LoopAll,
        LoopLast
    }
}

public struct AnimOperationData
{
    public string name;
    public float startTime;
    public float speed;
    public AnimOperationData(string name, float startTime, float speed = 1)
    {
        this.name = name;
        this.startTime = startTime;
        this.speed = speed;
    }
    public AnimOperationData(string name, float speed = 1)
    {
        this.name = name;
        this.startTime = Time.time;
        this.speed = speed;
    }
}