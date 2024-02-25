using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Animations;
using UnityEngine.Playables;
using GameLogic.Common;
using ThunderFireUnityEx;
public sealed class UIStateAnimatorStateRuntime
{
    public PlayQueuePB.EndingType Looping { get; private set; }
    public bool Replay { get; private set; }

    public AnimationMixerPlayable stateMixer;
    public float Cur { get; private set; } = 0;     //记录当前动画的时间点,可能有多个动画，记录的是当前总计播放时间点;
    public float Duration { get; private set; }     //记录所有动画总计时常;
    public int InuptCount { get; set; }     //记录动画数量;

    public PlayState MixerState = PlayState.Paused;  //缓存stateMixer的state;

    public float startTime = -1;

    private int m_CurrentClipIndex = -1;

    private int m_LastClipIndex = -1;

    private float m_TimeToNextClip;

    public Action stopCallBack;

    public string Name; //记录下名字，方便调试和查看;
    public void AddAnimationClipLenth(float length)
    {
        Duration += length;
    }
    public void InitializeSet(string name, bool replay, PlayQueuePB.EndingType loop)
    {
        Name = name;
        Replay = replay;
        Looping = loop;
    }

    public void LXPrepareFrame(float p_deltaTime)
    {
        if (UXGUIConfig.EnableOptimizeUIStateAnimator)
        {
            if (InuptCount == 0 || MixerState == PlayState.Paused) return;
        }
        else
        {
            if (stateMixer.GetInputCount() == 0 || stateMixer.GetPlayState() == PlayState.Paused) return;
        }

        float deltaTime = p_deltaTime * (float)stateMixer.GetSpeed();
        m_TimeToNextClip -= deltaTime;
        Cur += deltaTime;
        if (m_TimeToNextClip <= 0.0f)
        {
            if (m_CurrentClipIndex >= 0 && m_CurrentClipIndex < InuptCount)
            {
                //手动将当前clip设置到结束位置
                var _currentClip = (AnimationClipPlayable)stateMixer.GetInput(m_CurrentClipIndex);
                _currentClip.SetTime(_currentClip.GetAnimationClip().length);
            }
            m_CurrentClipIndex++;
            if (m_CurrentClipIndex >= InuptCount)
            {
                switch (Looping)
                {
                    case PlayQueuePB.EndingType.Pause:
                        Pause();
                        Cur = Duration;// 防止超出动画总时长
                        return;
                    case PlayQueuePB.EndingType.Quit:
                        Stop();
                        Cur = Duration;// 防止超出动画总时长
                        return;
                    case PlayQueuePB.EndingType.LoopLast:
                        m_CurrentClipIndex = InuptCount - 1;
                        var _currentClip = (AnimationClipPlayable)stateMixer.GetInput(m_CurrentClipIndex);
                        Cur -= _currentClip.GetAnimationClip().length;
                        break;
                    case PlayQueuePB.EndingType.LoopAll:
                        m_CurrentClipIndex = 0;
                        Cur -= Duration;
                        break;
                }
            }
            var currentClip = (AnimationClipPlayable)stateMixer.GetInput(m_CurrentClipIndex);
            currentClip.SetTime(0);
            m_TimeToNextClip = currentClip.GetAnimationClip().length;
        }

        //SetStartTime
        if (startTime >= 0)
        {
            float pos;
            (m_CurrentClipIndex, pos) = GetPosition(startTime);
            var currentClip = (AnimationClipPlayable)stateMixer.GetInput(m_CurrentClipIndex);
            currentClip.SetTime(pos);
            m_TimeToNextClip = currentClip.GetAnimationClip().length - pos;
            startTime = -1;
        }

        if (m_LastClipIndex != m_CurrentClipIndex)
        {
            for (int clipIndex = 0; clipIndex < InuptCount; ++clipIndex)
            {
                if (clipIndex == m_CurrentClipIndex)
                    stateMixer.SetInputWeight(clipIndex, 1.0f);
                else
                    stateMixer.SetInputWeight(clipIndex, 0.0f);
            }
            m_LastClipIndex = m_CurrentClipIndex;
        }
    }
    public (int, float) GetPosition(float time)
    {
        time = Mathf.Min(time, Duration);
        for (int clipIndex = 0; clipIndex < InuptCount; ++clipIndex)
        {
            var currentClip = (AnimationClipPlayable)stateMixer.GetInput(clipIndex);

            if (time <= currentClip.GetAnimationClip().length)
            {
                return (clipIndex, time);
            }
            time -= currentClip.GetAnimationClip().length;
        }
        return (-1, 0);
    }

    public void SetTimeForTransient(float time)
    {
        float pos;
        (m_CurrentClipIndex, pos) = GetPosition(time);
        var currentClip = (AnimationClipPlayable)stateMixer.GetInput(m_CurrentClipIndex);
        currentClip.SetTime(pos);
    }

    public void SetTime(float time)
    {
        float pos;
        (m_CurrentClipIndex, pos) = GetPosition(time);
        var currentClip = (AnimationClipPlayable)stateMixer.GetInput(m_CurrentClipIndex);
        currentClip.SetTime(pos);
        m_TimeToNextClip = currentClip.GetAnimationClip().length - pos;
        startTime = -1;
    }

    public void Play(float speed)
    {
        stateMixer.SetSpeed(speed);
        stateMixer.Play();
        MixerState = PlayState.Playing;
        if (m_CurrentClipIndex >= 0 && m_CurrentClipIndex < InuptCount)
        {
            stateMixer.SetInputWeight(m_CurrentClipIndex, 1.0f);
            var currentClip = (AnimationClipPlayable)stateMixer.GetInput(m_CurrentClipIndex);
            currentClip.Play();
        }
        else
        {
            stateMixer.SetInputWeight(0, 1.0f);
            var currentClip = (AnimationClipPlayable)stateMixer.GetInput(0);
            currentClip.Play();
        }
    }

    public void Stop()
    {
        for (int clipIndex = 0; clipIndex < InuptCount; ++clipIndex)
        {
            var currentClip = (AnimationClipPlayable)stateMixer.GetInput(clipIndex);
            currentClip.Pause();
            currentClip.SetTime(currentClip.GetAnimationClip().length);
        }
        stateMixer.Pause();
        MixerState = PlayState.Paused;
        stopCallBack?.Invoke();
    }
    public void Pause()
    {
        for (int clipIndex = 0; clipIndex < InuptCount; ++clipIndex)
        {
            var currentClip = (AnimationClipPlayable)stateMixer.GetInput(clipIndex);
            currentClip.Pause();
        }
        stateMixer.Pause();
        MixerState = PlayState.Paused;
    }

    public void Rewind()
    {
        m_CurrentClipIndex = -1;
        m_LastClipIndex = -1;
        m_TimeToNextClip = 0;
        Cur = 0;
        for (int clipIndex = 0; clipIndex < InuptCount; ++clipIndex)
        {
            stateMixer.GetInput(clipIndex).SetTime(0.0f);
        }
    }

    public void SetToLast()
    {
        for (int clipIndex = 0; clipIndex < InuptCount; ++clipIndex)
        {
            var currentClip = (AnimationClipPlayable)stateMixer.GetInput(clipIndex);
            currentClip.SetTime(currentClip.GetAnimationClip().length);
        }
    }
}

public sealed class UIStateAnimatorLayerRuntime
{
    private bool skipDefaultAnimation = false;
    public bool Transient { get; private set; }
    private float crossFadeSpeed;

    Dictionary<string, UIStateAnimatorStateRuntime> layerStateDatas = new Dictionary<string, UIStateAnimatorStateRuntime>();
    List<UIStateAnimatorStateRuntime> layerStateDataList = new List<UIStateAnimatorStateRuntime>();
    Dictionary<string, int> nameIDTable = new Dictionary<string, int>();

    private bool[] isPlaying;

    public AnimationMixerPlayable layerMixer;

    public int currentActiveIndex = -1;
    private int preIndex = -1;

    public bool isNeedReturn_OnBehaviourPlay = false;
    public void AddUIStateAnimatorStateRuntime(string name, UIStateAnimatorStateRuntime state)
    {
        layerStateDatas[name] = state;
        layerStateDataList.Add(state);
        nameIDTable[name] = layerStateDataList.Count - 1;
    }
    public void InitializeSet(float crossFadeTime, bool transient, bool skipDefault)
    {
        crossFadeSpeed = (crossFadeTime == 0 ? 0 : 1.0f / crossFadeTime);
        Transient = transient;
        skipDefaultAnimation = skipDefault;
        isPlaying = new bool[layerStateDataList.Count];
    }

    public void InitBranch()
    {
        if (!Transient)
        {
            EnableBranch(0, true);
        }
        preIndex = -1;
        if (!Transient)
        {
            if (skipDefaultAnimation)
            {
                SetBranch(0, 1, GetBranchDuration(0), true);
            }
            else
            {
                SetBranch(0, 1, 0, true);
            }
        }
    }
    public void LXPrepareFrame(float deltaTime)
    {
        if (Transient || layerStateDataList.Count == 0) return;
        if (crossFadeSpeed != 0)
        {
            var delta = crossFadeSpeed * deltaTime;
            for (int i = 0; i < layerStateDataList.Count; i++)
            {
                float w = layerMixer.GetInputWeight(i);
                if (i == currentActiveIndex && w < 1)
                {
                    if (w == 0)
                    {
                        EnableBranch(i, true);
                    }
                    layerMixer.SetInputWeight(i, Mathf.Min(w + delta, 1));
                }
                else if (i != currentActiveIndex && w != 0)
                {
                    layerMixer.SetInputWeight(i, Mathf.Max(w - delta, 0));
                    if (w - delta <= 0)
                    {
                        EnableBranch(i, false);
                    }
                }
            }
        }
        foreach (var state in layerStateDataList)
        {
            state.LXPrepareFrame(deltaTime);
        }
    }
    private void EnableBranch(int index, bool enable)
    {
        if (enable)
        {
            if (isPlaying[index]) return;
            layerMixer.ConnectInput(index, layerStateDataList[index].stateMixer, 0);
            layerMixer.SetInputWeight(index, 1.0f);
        }
        else
        {
            if (!isPlaying[index]) return;
            layerMixer.SetInputWeight(index, 0.0f);
            layerMixer.DisconnectInput(index);
        }
        isPlaying[index] = enable;
    }

    public void EnableAllBranches(bool enable)
    {
        for (int i = 0; i < layerStateDataList.Count; ++i)
        {
            EnableBranch(i, enable);
        }
    }
    public bool InBranch(string branchName)
    {
        return nameIDTable.ContainsKey(branchName) && InBranch(nameIDTable[branchName]);
    }
    private bool InBranch(int index)
    {
        return currentActiveIndex == index;
    }
    public void SkipToLastFrame()
    {
        for (int i = 0; i < layerStateDataList.Count; ++i)
        {
            UIStateAnimatorStateRuntime cur = layerStateDataList[i];
            cur.startTime = cur.Duration;
        }
    }
    public float GetBranchDuration(string branchName)
    {
        if (!nameIDTable.ContainsKey(branchName))
        {
            return -1;
        }
        return GetBranchDuration(nameIDTable[branchName]);
    }
    public float GetBranchDuration(int index)
    {
        var cur = layerStateDataList[index];
        return cur.Duration;
    }
    public float GetBranchCurPlayTime(string branchName)
    {
        if (!nameIDTable.ContainsKey(branchName))
        {
            return -1;
        }

        return GetBranchCurPlayTime(nameIDTable[branchName]);
    }

    public float GetBranchCurPlayTime(int index)
    {
        var cur = layerStateDataList[index];
        return cur.Cur;
    }
    public void SetBranch(string branchName, float speed = 1, float startTime = 0, bool noCrossFade = false)
    {
        if (!nameIDTable.ContainsKey(branchName)) return;
        SetBranch(nameIDTable[branchName], speed, startTime, noCrossFade);
    }

    public void SetBranch(int index, float speed = 1, float startTime = 0, bool noCrossFade = false)
    {
        currentActiveIndex = index;
        if (Transient)
        {
            UIStateAnimatorStateRuntime curState = layerStateDataList[currentActiveIndex];
            if (curState.Replay || layerMixer.GetInputWeight(currentActiveIndex) == 0)
            {
                curState.Rewind();
                curState.startTime = startTime;
            }
            EnableBranch(currentActiveIndex, true);
            //curState.stopCallBack = () => { EnableBranch(currentActiveIndex, false); };
            if (!isNeedReturn_OnBehaviourPlay) curState.Rewind();
            curState.Play(speed);
            if (startTime > 0) curState.SetTimeForTransient(startTime);
            return;
        }

        if (currentActiveIndex == preIndex)
        {
            UIStateAnimatorStateRuntime curState = layerStateDataList[currentActiveIndex];
            curState.startTime = startTime;
            if (curState.Replay)
            {
                curState.Rewind();
                curState.Play(speed);
                if (UXGUIConfig.EnableOptimizeUIStateAnimator && startTime > 0) curState.SetTime(startTime);
            }
            return;
        }

        for (int clipIndex = 0; clipIndex < layerStateDataList.Count; ++clipIndex)
        {
            var curState = layerStateDataList[clipIndex];
            if (clipIndex == currentActiveIndex)
            {
                if (crossFadeSpeed == 0 || noCrossFade) EnableBranch(clipIndex, true);
                curState.startTime = startTime;
                if (curState.Replay || !isNeedReturn_OnBehaviourPlay) curState.Rewind();
                curState.Play(speed);
                if (UXGUIConfig.EnableOptimizeUIStateAnimator && startTime > 0) curState.SetTime(startTime);
            }
            else
            {
                curState.Pause();
                if (crossFadeSpeed == 0 || noCrossFade)
                {
                    EnableBranch(clipIndex, false);
                }
            }
        }
        preIndex = currentActiveIndex;
    }
    public void SetIsNeedReturn_OnBehaviourPlay()
    {
        isNeedReturn_OnBehaviourPlay = true;
    }
    public void SetStateToLast(string name)
    {
        if (!nameIDTable.ContainsKey(name)) return;
        if (currentActiveIndex >= 0 && currentActiveIndex < layerStateDataList.Count)
        {
            UIStateAnimatorStateRuntime curState = layerStateDataList[currentActiveIndex];
            curState.SetToLast();
        }
    }

    public void Pause()
    {
        for (int clipIndex = 0; clipIndex < layerStateDataList.Count; ++clipIndex)
        {
            var curState = layerStateDataList[clipIndex];
            curState.Pause();
        }
        preIndex = -1;
        currentActiveIndex = -1;
    }

#if UNITY_EDITOR
    public List<UIStateAnimatorStateRuntime> GetLayerStateDataList()
    {
        return layerStateDataList;
    }
#endif
}

public partial class UIStateAnimator : MonoBehaviour
{
    PlayableGraph m_graph;
    Dictionary<string, UIStateAnimatorLayerRuntime> allLayerDatas;
    List<UIStateAnimatorLayerRuntime> allLayerDatasList;

    Dictionary<string, GameTimer> timerDict = new Dictionary<string, GameTimer>();

    bool need_update = false;
    void InitEx()
    {
        var animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = null;

        m_graph = PlayableGraph.Create(transform.gameObject.name + $" X: {transform.position.x}, Y:{transform.position.y}");
        m_graph.SetTimeUpdateMode(DirectorUpdateMode.UnscaledGameTime);
        //Output
        var playableOutput = AnimationPlayableOutput.Create(m_graph, "AnimationEx", animator);
        var mixNode = AnimationMixerPlayable.Create(m_graph, logicLayers.Length);

        allLayerDatas = new Dictionary<string, UIStateAnimatorLayerRuntime>();
        allLayerDatasList = new List<UIStateAnimatorLayerRuntime>();

        for (int i = 0; i < logicLayers.Length; ++i)
        {
            var layer = logicLayers[i];
            layer.Init();
            UIStateAnimatorLayerRuntime layerData = new UIStateAnimatorLayerRuntime();
            foreach (var state in layer.states)
            {
                if (state == null || state.animationQueue == null || state.animationQueue.Length == 0)
                {
#if UNITY_EDITOR                    
                    string prefabPath = transform.PathFromRoot();
#else
                    string prefabPath = name;
#endif
                    Debug.Log($"Prefab:{prefabPath}, State: {state.name} 没有设置动画状态, 找GUI看看");
                }
                UIStateAnimatorStateRuntime stateData = new UIStateAnimatorStateRuntime();
                stateData.InitializeSet(state.name, state.replay, state.loop);
                stateData.stateMixer = AnimationMixerPlayable.Create(m_graph, state.animationQueue.Length);
                AnimationClip[] clips = state.animationQueue;

                if (clips.Length > 1 || state.loop == PlayQueuePB.EndingType.LoopAll || state.loop == PlayQueuePB.EndingType.LoopLast
                    || layer.crossFadeTime > 0 || layer.skipDefault == true) need_update = true;

                for (int j = 0; j < clips.Length; ++j)
                {
                    AnimationClipPlayable playable = AnimationClipPlayable.Create(m_graph, clips[j]);
                    playable.SetApplyFootIK(false);
                    playable.SetApplyPlayableIK(false);
                    m_graph.Connect(playable, 0, stateData.stateMixer, j);
                    stateData.stateMixer.SetInputWeight(j, 1.0f);
                    stateData.AddAnimationClipLenth(clips[j].length);
                }
                stateData.InuptCount = stateData.stateMixer.GetInputCount();
                layerData.AddUIStateAnimatorStateRuntime(state.name, stateData);
            }
            layerData.InitializeSet(layer.crossFadeTime, layer.transient, layer.skipDefault);
            layerData.layerMixer = AnimationMixerPlayable.Create(m_graph, layer.states.Length);

            mixNode.SetInputWeight(i, 1.0f);
            m_graph.Connect(layerData.layerMixer, 0, mixNode, i);

            allLayerDatas[layer.name] = layerData;
            allLayerDatasList.Add(layerData);
            layerData.InitBranch(); //默认InitBranch时会选择第一个连起来
        }
        //至此只有layer和state还没有连起来。
        playableOutput.SetSourcePlayable(mixNode);
        m_graph.Play();
        //GraphVisualizerClient.Show(m_graph);
        Initialized = true;
    }

    #region UIStateAnimator static management

    public static HashSet<UIStateAnimator> _allaliveUIStateAnimators = new HashSet<UIStateAnimator>();
    public static void PrepareFrameAllAliveUIStateAnimators(float deltaTime)
    {
        foreach (var uiStateAnimator in _allaliveUIStateAnimators)
        {
            if (uiStateAnimator.Initialized && uiStateAnimator.enabled)
            {
                uiStateAnimator.LXPrepareFrame(deltaTime);
            }
        }
    }
    #endregion
    public void SkipToTransientStateLastFrame()
    {
        for (int i = 0; i < allLayerDatasList.Count; ++i)
        {
            if (!allLayerDatasList[i].Transient) continue;
            allLayerDatasList[i].SkipToLastFrame();
        }
    }
    private void SetStateEx(string stateName, float speed = 1, float startTime = 0, Action<GameTimer> stateDoneCallBack = null)
    {
        foreach (var layer in allLayerDatasList)
        {
            layer.SetBranch(stateName, speed, startTime);
        }
        LastOperation = new AnimOperationData(stateName, speed);
        if (timerDict.ContainsKey(stateName))
        {
            var stateCallBack = timerDict[stateName];
            if (this == (UIStateAnimator)stateCallBack.userData)
                stateCallBack.Cancel();
            timerDict.Remove(stateName);
        }
        if (stateDoneCallBack != null)
        {
            var stateCallBack = GameTimer.CreateTimer("UIStateAnimatorStateRuntime.SetStateEx", (GetDuration(stateName) - startTime) / speed, null, stateDoneCallBack, null, null, null, TimerMode.OneTime, this);
            timerDict[stateName] = stateCallBack;
        }
    }
    public float GetDuration(string stateName)
    {
        if (!Initialized)
        {
            return 0f;
        }

        float res = -1;
        foreach (var layer in allLayerDatasList)
        {
            res = Mathf.Max(res, layer.GetBranchDuration(stateName));
        }
        return res;
    }

    public float GetDurationByLayer(string stateName, string layerName)
    {
        if (!Initialized)
        {
            return 0f;
        }

        float res = -1;
        if (allLayerDatas.ContainsKey(layerName))
        {
            res = allLayerDatas[layerName].GetBranchDuration(stateName);
        }
        return res;
    }

    public float GetCurrentPlayTime(string layerName, string stateName)
    {
        if (!allLayerDatas.ContainsKey(layerName))
        {
            Debug.Log($"{gameObject.name}对象的UIStateAnimator脚本上不存在{layerName}名称的动画层");
            return -1;
        }
        return allLayerDatas[layerName].GetBranchCurPlayTime(stateName);
    }

    public bool Fadein(bool skipAnim = false)
    {
        if (string.IsNullOrEmpty(fadein))
        {
            return false;
        }
        else
        {
            float pos = skipAnim ? GetDuration(fadein) : 0;
            SetStateEx(fadein, 1, pos);
            return true;
        }
    }

    public bool FadeOut(bool skipAnim = false, Action<GameTimer> callBack = null)
    {
        if (string.IsNullOrEmpty(fadeout))
        {
            return false;
        }
        else
        {
            float pos = skipAnim ? GetDuration(fadeout) : 0;
            SetStateEx(fadeout, 1, pos, callBack);
            return true;
        }
    }
    public void SetIsNeedReturn_OnBehaviourPlay()
    {
        foreach (var layer in allLayerDatasList)
        {
            layer.SetIsNeedReturn_OnBehaviourPlay();
        }
    }

    /// <summary>
    /// 强制当前的State结束, 并触发回调
    /// </summary>
    public void Stop()
    {
        if (!Initialized)
        {
            return;
        }

        foreach (var layerData in allLayerDatasList)
        {
            layerData.EnableAllBranches(false);
        }
        foreach (var data in timerDict)
        {
            var stateCallBack = data.Value;
            stateCallBack.onComplete(stateCallBack);
        }

        timerDict.Clear();
    }

    //提供一个Pause 接口
    public void PauseAll()
    {
        if (!Initialized)
        {
            return;
        }
        foreach (var layerData in allLayerDatasList)
        {
            layerData.Pause();
        }
    }

    public bool InState(string stateName)
    {
        if (!Initialized)
        {
            return false;
        }

        foreach (var layer in allLayerDatasList)
        {
            if (layer.InBranch(stateName))
            {
                return true;
            }
        }
        return false;
    }

    public void SetStateToLast(string stateName)
    {
        if (!Initialized)
        {
            return;
        }

        foreach (var layerData in allLayerDatasList)
        {
            layerData.SetStateToLast(stateName);
        }
    }
}
