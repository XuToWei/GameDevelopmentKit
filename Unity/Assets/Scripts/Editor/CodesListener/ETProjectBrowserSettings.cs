using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ETProjectBrowserSettings : ScriptableObject
{
    public string ListenFolderPath = "Assets/Scripts/Codes/";
    private DirectoryInfo _dir;
    public DirectoryInfo ListenFolderInfo => _dir ??= new DirectoryInfo(ListenFolderPath);
}
