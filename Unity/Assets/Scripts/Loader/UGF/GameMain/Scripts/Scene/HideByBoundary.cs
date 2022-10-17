//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using UnityEngine;
using UnityGameFramework.Runtime;

namespace UGF
{
    public class HideByBoundary : MonoBehaviour
    {
        private void OnTriggerExit(Collider other)
        {
            GameObject go = other.gameObject;
            UnityGameFramework.Runtime.Entity entity = go.GetComponent<UnityGameFramework.Runtime.Entity>();
            if (entity == null)
            {
                Log.Warning("Unknown GameObject '{0}', you must use entity only.", go.name);
                Destroy(go);
                return;
            }

            GameEntry.Entity.HideEntity(entity);
        }
    }
}
