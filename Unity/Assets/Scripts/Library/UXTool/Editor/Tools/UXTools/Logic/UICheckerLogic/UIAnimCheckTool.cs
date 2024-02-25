#if UNITY_EDITOR && ODIN_INSPECTOR
using System;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace ThunderFireUITool
{
    public class UIAnimCheckTool
    {
        public static void BeginAnimCheck()
        {
            UIResCheckWindow.animDatas.Clear();

            string animFolderPath = AssetDatabase.LoadAssetAtPath<UIAtlasCheckRuleSettings>(ThunderFireUIToolConfig.UICheckSettingFullPath)?.animFolderPath;
            if (animFolderPath == null || !Directory.Exists(animFolderPath)) return;
            string[] files = Directory.GetFiles(animFolderPath, "*.anim", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(file);
                HashSet<float> frames = new HashSet<float>();
                HashSet<float> spriteFrames = new HashSet<float>();
                List<float> allFrames = new List<float>();
                EditorCurveBinding[] floatCurves = AnimationUtility.GetCurveBindings(clip);
                HashSet<float> rebuildKeyFrames = new HashSet<float>();
                List<(float, float)> rebuildFrames = new List<(float, float)>();
                float rebuildCount = 0;
                foreach (var floatCurve in floatCurves)
                {
                    var rebuildFlag = JudgeRebuild(floatCurve);
                    AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, floatCurve);
                    foreach (Keyframe frame in curve.keys)
                    {
                        frames.Add(frame.time);
                        allFrames.Add(frame.time);
                    }
                    if (!rebuildFlag) continue;
                    if (curve.keys.Any(s =>
                            float.IsPositiveInfinity(s.inTangent) || float.IsPositiveInfinity(s.outTangent)))
                    {
                        foreach (Keyframe frame in curve.keys)
                            rebuildKeyFrames.Add(frame.time);
                    }
                    else
                    {
                        if (curve.keys.Length > 1)
                        {
                            var flag = false;
                            var start = curve.keys[0];
                            for (var i = 1; i < curve.keys.Length; i++)
                            {
                                var end = curve.keys[i];
                                if (Math.Abs(end.value - start.value) < 0.000001f)
                                {
                                    start = curve.keys[i];
                                    continue;
                                }
                                flag = true;
                                rebuildFrames.Add((start.time, end.time));
                                rebuildFrames = MergeInterval(rebuildFrames);
                                start = curve.keys[i];
                            }

                            if (!flag) rebuildFrames.Add((curve.keys[0].time, curve.keys[0].time));
                        }
                        else if (curve.keys.Length == 1)
                        {
                            var start = curve.keys[0];
                            var end = curve.keys[0];
                            rebuildFrames.Add((start.time, end.time));
                            rebuildFrames = MergeInterval(rebuildFrames);
                        }
                    }
                }
                EditorCurveBinding[] objRefCurves = AnimationUtility.GetObjectReferenceCurveBindings(clip);
                foreach (var objRefCurve in objRefCurves)
                {
                    var spriteFlag = false;
                    var type = objRefCurve.type;
                    if (type.ToString() == "UnityEngine.SpriteRenderer")
                        spriteFlag = true;
                    ObjectReferenceKeyframe[] curveFrames = AnimationUtility.GetObjectReferenceCurve(clip, objRefCurve);
                    foreach (var frame in curveFrames)
                    {
                        frames.Add(frame.time);
                        allFrames.Add(frame.time);
                        rebuildKeyFrames.Add(frame.time);
                        if (spriteFlag)
                        {
                            spriteFrames.Add(frame.time);
                        }
                    }
                }

                foreach (var item in rebuildFrames)
                    rebuildCount += (item.Item2 - item.Item1) / (1 / clip.frameRate) + 1;
                foreach (var item in rebuildKeyFrames)
                {
                    if (rebuildFrames.Any(s => item >= s.Item1 && item <= s.Item2)) continue;
                    rebuildCount += 1;
                }

                var animData = new UIResCheckWindow.AnimData()
                {
                    animator = clip,
                    clipLength = Convert.ToInt32(clip.length / (1 / clip.frameRate)) + 1,
                    keyFramesCount = frames.Count,
                    totalCurvesCount = floatCurves.Length + objRefCurves.Length,
                    isLoop = clip.isLooping,
                    isLoopString = clip.isLooping ? "是" : "否",
                    allKeyFramesCount = allFrames.Count,
                    spriteKeyFrames = spriteFrames.Count,
                    rebuildCount = Convert.ToInt32(rebuildCount),
                };

                UIResCheckWindow.animDatas.Add(animData);
            }
            UIResCheckWindow.animDatas.Sort((x, y) => -x.rebuildCount.CompareTo(y.rebuildCount));
        }

        private static List<(float, float)> MergeInterval(List<(float, float)> list)
        {
            var result = new List<(float, float)>();
            list.Sort((x, y) => x.Item1.CompareTo(y.Item1));
            var curItem = list[0];
            for (var i = 1; i < list.Count; i++)
            {
                var nextItem = list[i];
                if (nextItem.Item1 > curItem.Item2)
                {
                    result.Add(curItem);
                    curItem = nextItem;
                }
                else
                    curItem.Item2 = nextItem.Item2;

            }
            result.Add(curItem);
            return result;
        }

        public static void CheckSingleClip(UIResCheckWindow.AnimDetailData animDetailData)
        {
            AnimationClip clip = animDetailData.animator;
            EditorCurveBinding[] floatCurves = AnimationUtility.GetCurveBindings(clip);
            foreach (var floatCurve in floatCurves)
            {
                var curve = AnimationUtility.GetEditorCurve(clip, floatCurve);
                var rebuildFlag = JudgeRebuild(floatCurve);
                var singleCurve = new UIResCheckWindow.FloatCurveData()
                {
                    property = floatCurve.propertyName,
                    type = floatCurve.type.ToString(),
                    path = floatCurve.path,
                    curve = curve,
                    keyFramesCount = curve.length,
                };
                if (curve.length > 0)
                    singleCurve.clipLength =
                        Convert.ToInt32((curve.keys[curve.length - 1].time - curve.keys[0].time) /
                                        (1 / clip.frameRate)) + 1;
                if (rebuildFlag)
                {
                    if (curve.keys.Any(s =>
                            float.IsPositiveInfinity(s.inTangent) || float.IsPositiveInfinity(s.outTangent)))
                        singleCurve.rebuildCount = curve.length;
                    else
                        singleCurve.rebuildCount = singleCurve.clipLength;

                }

                animDetailData.floatCurves.Add(singleCurve);
                animDetailData.floatCurves.Sort((x, y) => -x.rebuildCount.CompareTo(y.rebuildCount));
            }

            EditorCurveBinding[] objRefCurves = AnimationUtility.GetObjectReferenceCurveBindings(clip);
            foreach (var objRefCurve in objRefCurves)
            {
                var curveFrames = AnimationUtility.GetObjectReferenceCurve(clip, objRefCurve);

                var singleCurve = new UIResCheckWindow.ObjectCurveData()
                {
                    property = objRefCurve.propertyName,
                    type = objRefCurve.type.ToString(),
                    path = objRefCurve.path,
                    keyFramesCount = curveFrames.Length,
                };
                if (curveFrames.Length > 0)
                    singleCurve.clipLength =
                        Convert.ToInt32((curveFrames[curveFrames.Length - 1].time - curveFrames[0].time) /
                                        (1 / clip.frameRate)) + 1;
                if (objRefCurve.type == typeof(SpriteRenderer))
                {
                    singleCurve.isSprite = true;
                    singleCurve.spriteKeyFrames = curveFrames.Length;
                }
                singleCurve.rebuildCount = curveFrames.Length;
                animDetailData.objectCurves.Add(singleCurve);
                animDetailData.objectCurves.Sort((x, y) => -x.rebuildCount.CompareTo(y.rebuildCount));
            }
        }

        private static bool JudgeRebuild(EditorCurveBinding floatCurve)
        {
            var propertyName = floatCurve.propertyName;
            var ansFlag = !((floatCurve.type == typeof(RectTransform) &&
                                  (propertyName.StartsWith("m_LocalScale") ||
                                   propertyName.StartsWith("m_AnchoredPosition") ||
                                   propertyName.StartsWith("localEulerAnglesRaw") ||
                                   propertyName.StartsWith("m_LocalPosition") ||
                                   propertyName.StartsWith("m_LocalRotation"))) ||
                                 floatCurve.type == typeof(CanvasGroup) ||
                                 floatCurve.type == typeof(CanvasRenderer));
            var uiAnimCheckSetting =
                AssetDatabase.LoadAssetAtPath<UIAnimCheckSetting>(ThunderFireUIToolConfig.UICheckAnimFullPath);
            var flag = false;
            foreach (var item in uiAnimCheckSetting.objects)
            {
                if (item.DontRebuild == null) continue;
                if (floatCurve.type.Name == item.DontRebuild.name)
                {
                    if (item.PropertyNames.Count > 0)
                    {
                        if (item.PropertyNames.All(string.IsNullOrEmpty))
                        {
                            flag = true;
                            break;
                        }
                        var nameFlag = false;
                        foreach (var name in item.PropertyNames)
                            nameFlag = nameFlag || propertyName == name;
                        flag = nameFlag;
                    }
                    else
                        flag = true;
                    break;
                }
            }
            return ansFlag && !flag;
        }
    }
}
#endif