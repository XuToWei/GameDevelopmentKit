using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
#if UNITY_EDITOR
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ThunderFireUITool
{
#if UNITY_EDITOR_WIN
    public static class SVNContextMenus
    {
        [MenuItem("Assets/SVN/Commit", false, -900)]
        public static void Commit()
        {
            TortoiseSVNLogic.CommitSelected();
        }
    }
#endif
}
#endif