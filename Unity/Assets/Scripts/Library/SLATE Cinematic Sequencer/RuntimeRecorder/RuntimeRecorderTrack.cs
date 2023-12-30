using UnityEngine;

namespace Slate
{

    [Attachable(typeof(ActorGroup))]
    [Icon(typeof(Animation))]
    [Description("Use the runtime recorder clip to record transform animation (position, rotation, scale) of the actor and its children objects in runtime.\n\nIt is highly recomended to group multiple objects if they are related (like for example a character or chunks of a physics shatter) under an empty root gameobject and use that empty root gameobject as the actor of this group.")]
    public class RuntimeRecorderTrack : CutsceneTrack
    {
        public RuntimeRecorder.Options recorderOptions = RuntimeRecorder.Options.Default();
    }
}