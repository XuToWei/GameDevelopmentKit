using UnityEngine;
using RecorderProperty = Slate.RuntimeRecorder.RecorderProperty;


namespace Slate
{
    ///Transform Recorder
	public struct TransformRecorder
    {
        public RecorderProperty[] recordedProperties { get; private set; }
        public Transform transform { get; private set; }
        public string hierarchyPath { get; private set; }

        public System.Type type => typeof(Transform);

        public TransformRecorder(string hierarchyPath, Transform transform) {
            this.hierarchyPath = hierarchyPath;
            this.transform = transform;
            this.recordedProperties = new RecorderProperty[10];
            InitProperties();
        }

        void InitProperties() {
            recordedProperties[0] = new RecorderProperty("localPosition.x");
            recordedProperties[1] = new RecorderProperty("localPosition.y");
            recordedProperties[2] = new RecorderProperty("localPosition.z");

            recordedProperties[3] = new RecorderProperty("localRotation.x");
            recordedProperties[4] = new RecorderProperty("localRotation.y");
            recordedProperties[5] = new RecorderProperty("localRotation.z");
            recordedProperties[6] = new RecorderProperty("localRotation.w");

            recordedProperties[7] = new RecorderProperty("localScale.x");
            recordedProperties[8] = new RecorderProperty("localScale.y");
            recordedProperties[9] = new RecorderProperty("localScale.z");
        }

        public void RecordFrame(float time) {
            recordedProperties[0].Record(time, transform.localPosition.x);
            recordedProperties[1].Record(time, transform.localPosition.y);
            recordedProperties[2].Record(time, transform.localPosition.z);

            recordedProperties[3].Record(time, transform.localRotation.x);
            recordedProperties[4].Record(time, transform.localRotation.y);
            recordedProperties[5].Record(time, transform.localRotation.z);
            recordedProperties[6].Record(time, transform.localRotation.w);

            recordedProperties[7].Record(time, transform.localScale.x);
            recordedProperties[8].Record(time, transform.localScale.y);
            recordedProperties[9].Record(time, transform.localScale.z);
        }
    }
}