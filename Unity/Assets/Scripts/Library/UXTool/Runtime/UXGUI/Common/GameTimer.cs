using UnityEngine.Profiling;
using ThunderFireUnityEx;

namespace GameLogic.Common
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;


    public enum TimerMode
    {
        OneTime,
        Permanent,
        Repeat
    }

    public sealed class GameTimer
    {
        public float duration { get; private set; }
        public float remainingLifetime { get; private set; }
        public float timeElapsed { get; private set; }
        public float normalizedTimeElapsed { get; private set; }
        public TimerMode mode { get; private set; }
        public Action<GameTimer> onComplete { get; private set; }
        public Action<GameTimer> onTick { get; private set; }
        public Action<GameTimer> onLateTick { get; private set; }
        public Action<GameTimer> onFixedTick { get; private set; }
        public Func<bool> ownerAlive { get; private set; }
        public object userData { get; private set; }

        private bool cancelled = false;
        private bool alive = true;

        private string name = "unnamed";

        private GameTimer(string name, float duration, Func<bool> ownerAlive, Action<GameTimer> onComplete, Action<GameTimer> onTick, Action<GameTimer> onLateTick, Action<GameTimer> onFixedTick, TimerMode mode, object userData)
        {
            Initialize(name, duration, ownerAlive, onComplete, onTick, onLateTick, onFixedTick, mode, userData);
        }

        public void Initialize(string name, float duration, Func<bool> ownerAlive, Action<GameTimer> onComplete, Action<GameTimer> onTick, Action<GameTimer> onLateTick, Action<GameTimer> onFixedTick, TimerMode mode, object userData)
        {
            this.name = name;
            this.duration = duration;
            this.ownerAlive = ownerAlive;
            this.onComplete = onComplete;
            this.onTick = onTick;
            this.onLateTick = onLateTick;
            this.onFixedTick = onFixedTick;
            this.mode = mode;
            this.normalizedTimeElapsed = 0;
            this.timeElapsed = 0;
            this.remainingLifetime = duration;
            this.userData = userData;
            this.cancelled = false;
            this.alive = true;
        }

        public void Clear()
        {
            this.duration = 0;
            this.ownerAlive = null;
            this.onComplete = null;
            this.onTick = null;
            this.onLateTick = null;
            this.onFixedTick = null;
            this.mode = TimerMode.OneTime;
            this.normalizedTimeElapsed = 0;
            this.timeElapsed = 0;
            this.remainingLifetime = duration;
            this.userData = null;
            this.cancelled = false;
            this.alive = false;
            this.name = "unnamed";
        }

#if UNITY_EDITOR
        public override string ToString()
        {
            if (userData != null)
            {
                return $"{name} {userData.ToString()} d: {duration}, m: {mode}, alive: {alive}";
            }
            return $"{name} d: {duration}, m: {mode}, alive: {alive}";
        }
#endif

        public static GameTimer NextFrame(string name, Action<GameTimer> action)
        {
            return New(name, 0, null, action, null, null, null, TimerMode.OneTime, null);
        }

        public static GameTimer CreateTimer(string name, float duration, Action<GameTimer> onComplete, Action<GameTimer> onTick = null, Action<GameTimer> onLateTick = null, Action<GameTimer> onFixedTick = null, TimerMode mode = TimerMode.OneTime, object userData = null)
        {
            return New(name, duration, null, onComplete, onTick, onLateTick, onFixedTick, mode, userData);
        }

        public static GameTimer CreateTimer(string name, float duration, Func<bool> ownerAlive, Action<GameTimer> onComplete, Action<GameTimer> onTick = null, Action<GameTimer> onLateTick = null, Action<GameTimer> onFixedTick = null, TimerMode mode = TimerMode.OneTime, object userData = null)
        {
            return New(name, duration, ownerAlive, onComplete, onTick, onLateTick, onFixedTick, mode, userData);
        }

        public static GameTimer CreateTimer(string name, Func<bool> ownerAlive, Action<GameTimer> onTick = null, Action<GameTimer> onLateTick = null, Action<GameTimer> onFixedTick = null, object userData = null)
        {
            return New(name, 0, ownerAlive, null, onTick, onLateTick, onFixedTick, TimerMode.Permanent, userData);
        }

        private static GameTimer New(string name, float duration, Func<bool> isOwnerExists, Action<GameTimer> onComplete, Action<GameTimer> onTick, Action<GameTimer> onLateTick, Action<GameTimer> onFixedTick, TimerMode mode, object userData)
        {
            duration = duration >= 0 ? duration : 0;
            GameTimer timer = new GameTimer(name, duration, isOwnerExists, onComplete, onTick, onLateTick, onFixedTick, mode, userData);

            internalManager.AddTimer(timer);
            return timer;
        }

        private static GameTimer New(string name, GameEntityTimerArchive archive, Func<bool> isOwnerExists, Action<GameTimer> onComplete, Action<GameTimer> onTick, Action<GameTimer> onLateTick, Action<GameTimer> onFixedTick, object userData)
        {
            var duration = archive.duration;
            var mode = archive.timeMode;
            duration = duration >= 0 ? duration : 0;
            var timer = new GameTimer(name, duration, isOwnerExists, onComplete, onTick, onLateTick, onFixedTick, mode, userData);
            internalManager.AddTimer(timer);
            return timer;
        }

        public void InnerFrameBeginFixedTick(float unscaledDeltaTime, float deltaTime)
        {
            if (cancelled)
            {
                return;
            }
            Profiler.BeginSample($"onFixedTick {name}");
            onFixedTick?.Invoke(this);
            Profiler.EndSample();
        }

        public void InnerFrameBeginTick(float unscaledDeltaTime, float deltaTime)
        {
            if (cancelled)
            {
                return;
            }
            Profiler.BeginSample($"onTick {name}");
            onTick?.Invoke(this);
            Profiler.EndSample();
        }

        public void InnerFrameEndTick(float unscaledDeltaTime, float deltaTime)
        {
            if (cancelled)
            {
                return;
            }
            Profiler.BeginSample($"onLateTick {name}");
            onLateTick?.Invoke(this);
            Profiler.EndSample();
            switch (mode)
            {
                default:
                case TimerMode.OneTime:
                    timeElapsed += deltaTime;
                    normalizedTimeElapsed = timeElapsed.SafeDivide(duration);
                    remainingLifetime = duration - timeElapsed;
                    if (normalizedTimeElapsed >= 1 || duration == 0)
                    {
                        Profiler.BeginSample($"onComplete {name}");
                        onComplete?.Invoke(this);
                        Profiler.EndSample();
                        Dispose();
                    }
                    break;
                case TimerMode.Permanent:
                    break;
                case TimerMode.Repeat:
                    timeElapsed += deltaTime;
                    normalizedTimeElapsed = timeElapsed.SafeDivide(duration);
                    remainingLifetime = duration - timeElapsed;
                    if (normalizedTimeElapsed >= 1 || duration == 0)
                    {
                        Profiler.BeginSample($"onComplete {name}");
                        onComplete?.Invoke(this);
                        Profiler.EndSample();
                        remainingLifetime = duration;
                        timeElapsed = 0;
                        normalizedTimeElapsed = 0;
                    }
                    break;
            }
            if (ownerAlive != null && !ownerAlive())
            {
                Dispose();
                return;
            }
        }

        public void Cancel()
        {
            cancelled = true;
            Dispose();
        }

        private void Dispose()
        {
            if (!alive)
            {
                return;
            }

            //同帧等待加入的timer不要dispose
            if (internalManager.ToBeAddTimer(this))
            {
                return;
            }
            Clear();
            internalManager.RemoveTimer(this);
        }

        public static void OnApplicationQuit()
        {
            timerManager?.Dispose();
        }

        private static GameTimerManager timerManager;
        private static GameTimerManager internalManager
        {
            get
            {
                if (timerManager == null)
                {
                    timerManager = new GameTimerManager();
                }
                return timerManager;
            }
        }

        public static void OnLevelStart()
        {
            internalManager.Reuse();
        }

        public static void OnLevelEnd()
        {
            internalManager.Dispose();
        }

        public static void FrameBeginTick(float unscaledDeltaTime, float deltaTime)
        {
            internalManager.FrameBeginTick(unscaledDeltaTime, deltaTime);
        }

        public static void FrameEndTick(float unscaledDeltaTime, float deltaTime)
        {
            internalManager.FrameEndTick(unscaledDeltaTime, deltaTime);
        }

        public static void FrameBeginFixedTick(float unscaledDeltaTime, float deltaTime)
        {
            internalManager.FrameBeginFixedTick(unscaledDeltaTime, deltaTime);
        }
    }

    public class GameTimerManager : IDisposable
    {
        private static HashSet<GameTimer> timers = new HashSet<GameTimer>();
        private static HashSet<GameTimer> pendingRemoveTimers = new HashSet<GameTimer>();
        private static HashSet<GameTimer> pendingAddTimers = new HashSet<GameTimer>();
        private bool alive = true;

        public GameTimerManager()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEditor.EditorApplication.update -= FixedTick;
                UnityEditor.EditorApplication.update -= Tick;
                UnityEditor.EditorApplication.update -= LateTick;
                UnityEditor.EditorApplication.update += FixedTick;
                UnityEditor.EditorApplication.update += Tick;
                UnityEditor.EditorApplication.update += LateTick;
            }
#endif
        }

#if UNITY_EDITOR
        private void Tick()
        {
            FrameBeginTick(Time.unscaledDeltaTime, Time.deltaTime);
        }

        private void FixedTick()
        {
            FrameBeginFixedTick(Time.unscaledDeltaTime, Time.deltaTime);
        }

        private void LateTick()
        {
            FrameEndTick(Time.unscaledDeltaTime, Time.deltaTime);
        }
#endif

        ~GameTimerManager()
        {
            Dispose(true);
        }

        public void Reuse()
        {
            if (alive)
            {
                return;
            }
            alive = true;
            GC.ReRegisterForFinalize(this);
        }

        public void Dispose()
        {
            Dispose(false);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool fromFinalizer)
        {
            if (!alive)
            {
                return;
            }
            timers.Clear();
            pendingRemoveTimers.Clear();
            pendingAddTimers.Clear();
            alive = false;
        }

        public void FrameBeginTick(float unscaledDeltaTime, float deltaTime)
        {
            if (timers.Count > 0)
            {
                foreach (GameTimer timer in timers)
                {
                    timer.InnerFrameBeginTick(unscaledDeltaTime, deltaTime);
                }
            }
        }

        public void FrameBeginFixedTick(float unscaledDeltaTime, float deltaTime)
        {
            if (timers.Count > 0)
            {
                foreach (GameTimer timer in timers)
                {
                    timer.InnerFrameBeginFixedTick(unscaledDeltaTime, deltaTime);
                }
            }
        }

        public void FrameEndTick(float unscaledDeltaTime, float deltaTime)
        {
            if (timers.Count > 0)
            {
                foreach (GameTimer timer in timers)
                {
                    timer.InnerFrameEndTick(unscaledDeltaTime, deltaTime);
                }
            }
            AddOrRemoveTimer();
        }

        public void AddTimer(GameTimer timer)
        {
            pendingAddTimers.Add(timer);
        }

        public void RemoveTimer(GameTimer timer)
        {
            pendingRemoveTimers.Add(timer);
        }

        private void AddOrRemoveTimer()
        {
            foreach (GameTimer timer in pendingRemoveTimers)
            {
                timers.Remove(timer);
            }
            pendingRemoveTimers.Clear();
            foreach (GameTimer timer in pendingAddTimers)
            {
                timers.Add(timer);
            }
            pendingAddTimers.Clear();
        }

        public bool ToBeAddTimer(GameTimer timer)
        {
            return pendingAddTimers.Contains(timer);
        }
    }
}