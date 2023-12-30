using UnityEngine;
using System.Collections.Generic;

namespace Slate
{

    ///<summary>Interface for all directable elements of an IDirector (groups, tracks, clips..)</summary>
    public interface IDirectable
    {

        IDirector root { get; }
        IDirectable parent { get; }
        IEnumerable<IDirectable> children { get; }

        GameObject actor { get; }
        string name { get; }
        bool isActive { get; }
        bool isCollapsed { get; }
        bool isLocked { get; }

        float startTime { get; }
        float endTime { get; }
        float blendIn { get; }
        float blendOut { get; }
        bool canCrossBlend { get; }

        void Validate(IDirector root, IDirectable parent);
        bool Initialize();
        void Enter();
        void Exit();
        void Update(float time, float previousTime);
        void ReverseEnter();
        void Reverse();

        void RootEnabled();
        void RootUpdated(float time, float previousTime);
        void RootDisabled();
        void RootDestroyed();

#if UNITY_EDITOR
        void DrawGizmos(bool selected);
        void SceneGUI(bool selected);
#endif

    }

    ///----------------------------------------------------------------------------------------------

    ///<summary>For Directables that contain keyable parameters.</summary>
    public interface IKeyable : IDirectable
    {
        AnimationDataCollection animationData { get; }
        object animatedParametersTarget { get; }
    }

    ///<summary>For Directables that wrap content, like an animation/audio clip.</summary>
    public interface ISubClipContainable : IDirectable
    {
        float subClipOffset { get; set; }
        float subClipSpeed { get; }
        float subClipLength { get; }
    }
}