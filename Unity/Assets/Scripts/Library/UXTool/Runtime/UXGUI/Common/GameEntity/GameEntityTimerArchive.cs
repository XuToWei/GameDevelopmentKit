using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLogic.Common;

using NaughtyAttributes;


[System.Serializable]
public class GameEntityTimerArchive 
{
    [LabelText("触发时间")]
    public float duration;
    public TimerMode timeMode;
}
