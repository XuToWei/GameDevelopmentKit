using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Profiling;

namespace Game
{
    public class TestType : MonoBehaviour
    {
        [Button]
        public void TestName()
        {
            Profiler.BeginSample("TestName");
            for (int i = 0; i < 100; i++)
            {
                string s = typeof(TestType).Name;
            }
            Profiler.EndSample();
        }
        
        [Button]
        public void TestFullName()
        {
            Profiler.BeginSample("TestFullName");
            for (int i = 0; i < 100; i++)
            {
                string s = typeof(TestType).FullName;
            }
            Profiler.EndSample();
        }

        private void Update()
        {
            TestName();
            TestFullName();
        }
    }
}
