using System;
using System.Collections.Generic;
using UnityEngine;

namespace ET
{
    public class UGFEntitySystemSingleton : Singleton<UGFEntitySystemSingleton>, ISingletonAwake
    {
        private TypeSystems TypeSystems { get; set; }
        
        private readonly DoubleMap<Type, long> ugfTypeLongHashCode = new();
        
        public void Awake()
        {
            this.TypeSystems = new(InstanceQueueIndex.Max);
            foreach (Type type in CodeTypes.Instance.GetTypes(typeof (UGFUIFormSystemAttribute)))
            {
                SystemObject obj = (SystemObject)Activator.CreateInstance(type);

                if (obj is not ISystemType iSystemType)
                {
                    continue;
                }

                TypeSystems.OneTypeSystems oneTypeSystems = this.TypeSystems.GetOrCreateOneTypeSystems(iSystemType.Type());
                oneTypeSystems.Map.Add(iSystemType.SystemType(), obj);
                int index = iSystemType.GetInstanceQueueIndex();
                if (index > InstanceQueueIndex.None && index < InstanceQueueIndex.Max)
                {
                    oneTypeSystems.QueueFlag[index] = true;
                }
            }
            
            foreach (Type type in CodeTypes.Instance.GetTypes(typeof (UGFUIWidgetSystemAttribute)))
            {
                SystemObject obj = (SystemObject)Activator.CreateInstance(type);

                if (obj is not ISystemType iSystemType)
                {
                    continue;
                }

                TypeSystems.OneTypeSystems oneTypeSystems = this.TypeSystems.GetOrCreateOneTypeSystems(iSystemType.Type());
                oneTypeSystems.Map.Add(iSystemType.SystemType(), obj);
                int index = iSystemType.GetInstanceQueueIndex();
                if (index > InstanceQueueIndex.None && index < InstanceQueueIndex.Max)
                {
                    oneTypeSystems.QueueFlag[index] = true;
                }
            }
            
            foreach (var kv in CodeTypes.Instance.GetTypes())
            {
                Type type = kv.Value;
                if (typeof(UGFUIForm).IsAssignableFrom(type) || typeof(UGFUIWidget).IsAssignableFrom(type))
                {
                    long hash = type.FullName.GetLongHashCode();
                    try
                    {
                        this.ugfTypeLongHashCode.Add(type, type.FullName.GetLongHashCode());
                    }
                    catch (Exception e)
                    {
                        Type sameHashType = this.ugfTypeLongHashCode.GetKeyByValue(hash);
                        throw new Exception($"long hash add fail: {type.FullName} {sameHashType.FullName}", e);
                    }
                }
            }
        }
        
        public long GetLongHashCode(Type type)
        {
            return this.ugfTypeLongHashCode.GetValueByKey(type);
        }

        
        public TypeSystems.OneTypeSystems GetOneTypeSystems(Type type)
        {
            return this.TypeSystems.GetOneTypeSystems(type);
        }

        public void UGFUIWidgetOnInit(UGFUIWidget ugfUIWidget)
        {
            if (ugfUIWidget is not IUGFUIWidgetOnInit)
            {
                return;
            }

            List<SystemObject> systems = this.TypeSystems.GetSystems(ugfUIWidget.GetType(), typeof(IUGFUIWidgetOnInitSystem));
            if (systems == null)
            {
                return;
            }

            foreach (IUGFUIWidgetOnInitSystem system in systems)
            {
                if (system == null)
                {
                    continue;
                }

                try
                {
                    system.Run(ugfUIWidget);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public void UGFUIWidgetOnOpen(UGFUIWidget ugfUIWidget)
        {
            if (ugfUIWidget is not IUGFUIWidgetOnOpen)
            {
                return;
            }

            List<SystemObject> systems = this.TypeSystems.GetSystems(ugfUIWidget.GetType(), typeof(IUGFUIWidgetOnOpenSystem));
            if (systems == null)
            {
                return;
            }

            foreach (IUGFUIWidgetOnOpenSystem system in systems)
            {
                if (system == null)
                {
                    continue;
                }

                try
                {
                    system.Run(ugfUIWidget);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public void UGFUIWidgetOnClose(UGFUIWidget ugfUIWidget, bool isShutdown)
        {
            if (ugfUIWidget is not IUGFUIWidgetOnClose)
            {
                return;
            }

            List<SystemObject> systems = this.TypeSystems.GetSystems(ugfUIWidget.GetType(), typeof(IUGFUIWidgetOnCloseSystem));
            if (systems == null)
            {
                return;
            }

            foreach (IUGFUIWidgetOnCloseSystem system in systems)
            {
                if (system == null)
                {
                    continue;
                }

                try
                {
                    system.Run(ugfUIWidget, isShutdown);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public void UGFUIWidgetOnPause(UGFUIWidget ugfUIWidget)
        {
            if (ugfUIWidget is not IUGFUIWidgetOnPause)
            {
                return;
            }

            List<SystemObject> systems = this.TypeSystems.GetSystems(ugfUIWidget.GetType(), typeof(IUGFUIWidgetOnPauseSystem));
            if (systems == null)
            {
                return;
            }

            foreach (IUGFUIWidgetOnPauseSystem system in systems)
            {
                if (system == null)
                {
                    continue;
                }

                try
                {
                    system.Run(ugfUIWidget);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public void UGFUIWidgetOnResume(UGFUIWidget ugfUIWidget)
        {
            if (ugfUIWidget is not IUGFUIWidgetOnResume)
            {
                return;
            }

            List<SystemObject> systems = this.TypeSystems.GetSystems(ugfUIWidget.GetType(), typeof(IUGFUIWidgetOnResumeSystem));
            if (systems == null)
            {
                return;
            }

            foreach (IUGFUIWidgetOnResumeSystem system in systems)
            {
                if (system == null)
                {
                    continue;
                }

                try
                {
                    system.Run(ugfUIWidget);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public void UGFUIWidgetOnCover(UGFUIWidget ugfUIWidget)
        {
            if (ugfUIWidget is not IUGFUIWidgetOnCover)
            {
                return;
            }

            List<SystemObject> systems = this.TypeSystems.GetSystems(ugfUIWidget.GetType(), typeof(IUGFUIWidgetOnCoverSystem));
            if (systems == null)
            {
                return;
            }

            foreach (IUGFUIWidgetOnCoverSystem system in systems)
            {
                if (system == null)
                {
                    continue;
                }

                try
                {
                    system.Run(ugfUIWidget);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public void UGFUIWidgetOnReveal(UGFUIWidget ugfUIWidget)
        {
            if (ugfUIWidget is not IUGFUIWidgetOnReveal)
            {
                return;
            }

            List<SystemObject> systems = this.TypeSystems.GetSystems(ugfUIWidget.GetType(), typeof(IUGFUIWidgetOnRevealSystem));
            if (systems == null)
            {
                return;
            }

            foreach (IUGFUIWidgetOnRevealSystem system in systems)
            {
                if (system == null)
                {
                    continue;
                }

                try
                {
                    system.Run(ugfUIWidget);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public void UGFUIWidgetOnRefocus(UGFUIWidget ugfUIWidget)
        {
            if (ugfUIWidget is not IUGFUIWidgetOnRefocus)
            {
                return;
            }

            List<SystemObject> systems = this.TypeSystems.GetSystems(ugfUIWidget.GetType(), typeof(IUGFUIWidgetOnRefocusSystem));
            if (systems == null)
            {
                return;
            }

            foreach (IUGFUIWidgetOnRefocusSystem system in systems)
            {
                if (system == null)
                {
                    continue;
                }

                try
                {
                    system.Run(ugfUIWidget);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public void UGFUIWidgetOnUpdate(UGFUIWidget ugfUIWidget, float elapseSeconds, float realElapseSeconds)
        {
            if (ugfUIWidget is not IUGFUIWidgetOnUpdate)
            {
                return;
            }

            List<SystemObject> systems = this.TypeSystems.GetSystems(ugfUIWidget.GetType(), typeof(IUGFUIWidgetOnUpdateSystem));
            if (systems == null)
            {
                return;
            }

            foreach (IUGFUIWidgetOnUpdateSystem system in systems)
            {
                if (system == null)
                {
                    continue;
                }

                try
                {
                    system.Run(ugfUIWidget, elapseSeconds, realElapseSeconds);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public void UGFUIWidgetOnDepthChanged(UGFUIWidget ugfUIWidget, int uiGroupDepth, int depthInUIGroup)
        {
            if (ugfUIWidget is not IUGFUIWidgetOnDepthChanged)
            {
                return;
            }

            List<SystemObject> systems = this.TypeSystems.GetSystems(ugfUIWidget.GetType(), typeof(IUGFUIWidgetOnDepthChangedSystem));
            if (systems == null)
            {
                return;
            }

            foreach (IUGFUIWidgetOnDepthChangedSystem system in systems)
            {
                if (system == null)
                {
                    continue;
                }

                try
                {
                    system.Run(ugfUIWidget, uiGroupDepth, depthInUIGroup);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public void UGFUIWidgetOnRecycle(UGFUIWidget ugfUIWidget)
        {
            if (ugfUIWidget is not IUGFUIWidgetOnRecycle)
            {
                return;
            }

            List<SystemObject> systems = this.TypeSystems.GetSystems(ugfUIWidget.GetType(), typeof(IUGFUIWidgetOnRecycleSystem));
            if (systems == null)
            {
                return;
            }

            foreach (IUGFUIWidgetOnRecycleSystem system in systems)
            {
                if (system == null)
                {
                    continue;
                }

                try
                {
                    system.Run(ugfUIWidget);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public void UGFUIFormOnInit(UGFUIForm ugfUIForm)
        {
            if (ugfUIForm is not IUGFUIFormOnInit)
            {
                return;
            }

            List<SystemObject> systems = this.TypeSystems.GetSystems(ugfUIForm.GetType(), typeof(IUGFUIFormOnInitSystem));
            if (systems == null)
            {
                return;
            }

            foreach (IUGFUIFormOnInitSystem system in systems)
            {
                if (system == null)
                {
                    continue;
                }

                try
                {
                    system.Run(ugfUIForm);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public void UGFUIFormOnOpen(UGFUIForm ugfUIForm)
        {
            if (ugfUIForm is not IUGFUIFormOnOpen)
            {
                return;
            }
            
            List<SystemObject> iUGFUIFormOnOpenSystems = this.TypeSystems.GetSystems(ugfUIForm.GetType(), typeof (IUGFUIFormOnOpenSystem));
            if (iUGFUIFormOnOpenSystems == null)
            {
                return;
            }

            foreach (IUGFUIFormOnOpenSystem iUGFUIFormOnOpenSystem in iUGFUIFormOnOpenSystems)
            {
                if (iUGFUIFormOnOpenSystem == null)
                {
                    continue;
                }

                try
                {
                    iUGFUIFormOnOpenSystem.Run(ugfUIForm);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public void UGFUIFormOnClose(UGFUIForm ugfUIForm, bool isShutdown)
        {
            if (ugfUIForm is not IUGFUIFormOnClose)
            {
                return;
            }

            List<SystemObject> systems = this.TypeSystems.GetSystems(ugfUIForm.GetType(), typeof(IUGFUIFormOnCloseSystem));
            if (systems == null)
            {
                return;
            }

            foreach (IUGFUIFormOnCloseSystem system in systems)
            {
                if (system == null)
                {
                    continue;
                }

                try
                {
                    system.Run(ugfUIForm, isShutdown);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public void UGFUIFormOnCover(UGFUIForm ugfUIForm)
        {
            if (ugfUIForm is not IUGFUIFormOnCover)
            {
                return;
            }

            List<SystemObject> systems = this.TypeSystems.GetSystems(ugfUIForm.GetType(), typeof(IUGFUIFormOnCoverSystem));
            if (systems == null)
            {
                return;
            }

            foreach (IUGFUIFormOnCoverSystem system in systems)
            {
                if (system == null)
                {
                    continue;
                }

                try
                {
                    system.Run(ugfUIForm);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public void UGFUIFormOnDepthChanged(UGFUIForm ugfUIForm, int uiGroupDepth, int depthInUIGroup)
        {
            if (ugfUIForm is not IUGFUIFormOnDepthChanged)
            {
                return;
            }

            List<SystemObject> systems = this.TypeSystems.GetSystems(ugfUIForm.GetType(), typeof(IUGFUIFormOnDepthChangedSystem));
            if (systems == null)
            {
                return;
            }

            foreach (IUGFUIFormOnDepthChangedSystem system in systems)
            {
                if (system == null)
                {
                    continue;
                }

                try
                {
                    system.Run(ugfUIForm, uiGroupDepth, depthInUIGroup);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public void UGFUIFormOnReveal(UGFUIForm ugfUIForm)
        {
            if (ugfUIForm is not IUGFUIFormOnReveal)
            {
                return;
            }

            List<SystemObject> systems = this.TypeSystems.GetSystems(ugfUIForm.GetType(), typeof(IUGFUIFormOnRevealSystem));
            if (systems == null)
            {
                return;
            }

            foreach (IUGFUIFormOnRevealSystem system in systems)
            {
                if (system == null)
                {
                    continue;
                }

                try
                {
                    system.Run(ugfUIForm);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public void UGFUIFormOnPause(UGFUIForm ugfUIForm)
        {
            if (ugfUIForm is not IUGFUIFormOnPause)
            {
                return;
            }

            List<SystemObject> systems = this.TypeSystems.GetSystems(ugfUIForm.GetType(), typeof(IUGFUIFormOnPauseSystem));
            if (systems == null)
            {
                return;
            }

            foreach (IUGFUIFormOnPauseSystem system in systems)
            {
                if (system == null)
                {
                    continue;
                }

                try
                {
                    system.Run(ugfUIForm);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public void UGFUIFormOnRecycle(UGFUIForm ugfUIForm)
        {
            if (ugfUIForm is not IUGFUIFormOnRecycle)
            {
                return;
            }

            List<SystemObject> systems = this.TypeSystems.GetSystems(ugfUIForm.GetType(), typeof(IUGFUIFormOnRecycleSystem));
            if (systems == null)
            {
                return;
            }

            foreach (IUGFUIFormOnRecycleSystem system in systems)
            {
                if (system == null)
                {
                    continue;
                }

                try
                {
                    system.Run(ugfUIForm);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public void UGFUIFormOnRefocus(UGFUIForm ugfUIForm)
        {
            if (ugfUIForm is not IUGFUIFormOnRefocus)
            {
                return;
            }

            List<SystemObject> systems = this.TypeSystems.GetSystems(ugfUIForm.GetType(), typeof(IUGFUIFormOnRefocusSystem));
            if (systems == null)
            {
                return;
            }

            foreach (IUGFUIFormOnRefocusSystem system in systems)
            {
                if (system == null)
                {
                    continue;
                }

                try
                {
                    system.Run(ugfUIForm);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public void UGFUIFormOnResume(UGFUIForm ugfUIForm)
        {
            if (ugfUIForm is not IUGFUIFormOnResume)
            {
                return;
            }

            List<SystemObject> systems = this.TypeSystems.GetSystems(ugfUIForm.GetType(), typeof(IUGFUIFormOnResumeSystem));
            if (systems == null)
            {
                return;
            }

            foreach (IUGFUIFormOnResumeSystem system in systems)
            {
                if (system == null)
                {
                    continue;
                }

                try
                {
                    system.Run(ugfUIForm);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public void UGFUIFormOnUpdate(UGFUIForm ugfUIForm, float elapseSeconds, float realElapseSeconds)
        {
            if (ugfUIForm is not IUGFUIFormOnUpdate)
            {
                return;
            }

            List<SystemObject> systems = this.TypeSystems.GetSystems(ugfUIForm.GetType(), typeof(IUGFUIFormOnUpdateSystem));
            if (systems == null)
            {
                return;
            }

            foreach (IUGFUIFormOnUpdateSystem system in systems)
            {
                if (system == null)
                {
                    continue;
                }

                try
                {
                    system.Run(ugfUIForm, elapseSeconds, realElapseSeconds);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public void UGFEntityOnInit(UGFEntity ugfEntity)
        {
            if (ugfEntity is not IUGFEntityOnInit)
            {
                return;
            }

            List<SystemObject> systems = this.TypeSystems.GetSystems(ugfEntity.GetType(), typeof(IUGFEntityOnInitSystem));
            if (systems == null)
            {
                return;
            }

            foreach (IUGFEntityOnInitSystem system in systems)
            {
                if (system == null)
                {
                    continue;
                }

                try
                {
                    system.Run(ugfEntity);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public void UGFEntityOnShow(UGFEntity ugfEntity)
        {
            if (ugfEntity is not IUGFEntityOnShow)
            {
                return;
            }

            List<SystemObject> systems = this.TypeSystems.GetSystems(ugfEntity.GetType(), typeof(IUGFEntityOnShowSystem));
            if (systems == null)
            {
                return;
            }

            foreach (IUGFEntityOnShowSystem system in systems)
            {
                if (system == null)
                {
                    continue;
                }

                try
                {
                    system.Run(ugfEntity);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public void UGFEntityOnHide(UGFEntity ugfEntity, bool isShutdown)
        {
            if (ugfEntity is not IUGFEntityOnHide)
            {
                return;
            }

            List<SystemObject> systems = this.TypeSystems.GetSystems(ugfEntity.GetType(), typeof(IUGFEntityOnHideSystem));
            if (systems == null)
            {
                return;
            }

            foreach (IUGFEntityOnHideSystem system in systems)
            {
                if (system == null)
                {
                    continue;
                }

                try
                {
                    system.Run(ugfEntity, isShutdown);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public void UGFEntityOnRecycle(UGFEntity ugfEntity)
        {
            if (ugfEntity is not IUGFEntityOnRecycle)
            {
                return;
            }

            List<SystemObject> systems = this.TypeSystems.GetSystems(ugfEntity.GetType(), typeof(IUGFEntityOnRecycleSystem));
            if (systems == null)
            {
                return;
            }

            foreach (IUGFEntityOnRecycleSystem system in systems)
            {
                if (system == null)
                {
                    continue;
                }

                try
                {
                    system.Run(ugfEntity);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public void UGFEntityOnAttached(UGFEntity ugfEntity, UGFEntity childEntity, Transform parentTransform)
        {
            if (ugfEntity is not IUGFEntityOnAttached)
            {
                return;
            }

            List<SystemObject> systems = this.TypeSystems.GetSystems(ugfEntity.GetType(), typeof(IUGFEntityOnAttachedSystem));
            if (systems == null)
            {
                return;
            }

            foreach (IUGFEntityOnAttachedSystem system in systems)
            {
                if (system == null)
                {
                    continue;
                }

                try
                {
                    system.Run(ugfEntity, childEntity, parentTransform);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public void UGFEntityOnDetached(UGFEntity ugfEntity, UGFEntity childEntity)
        {
            if (ugfEntity is not IUGFEntityOnDetached)
            {
                return;
            }

            List<SystemObject> systems = this.TypeSystems.GetSystems(ugfEntity.GetType(), typeof(IUGFEntityOnDetachedSystem));
            if (systems == null)
            {
                return;
            }

            foreach (IUGFEntityOnDetachedSystem system in systems)
            {
                if (system == null)
                {
                    continue;
                }

                try
                {
                    system.Run(ugfEntity, childEntity);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public void UGFEntityOnAttachTo(UGFEntity ugfEntity, UGFEntity parentEntity, Transform parentTransform)
        {
            if (ugfEntity is not IUGFEntityOnAttachTo)
            {
                return;
            }

            List<SystemObject> systems = this.TypeSystems.GetSystems(ugfEntity.GetType(), typeof(IUGFEntityOnAttachToSystem));
            if (systems == null)
            {
                return;
            }

            foreach (IUGFEntityOnAttachToSystem system in systems)
            {
                if (system == null)
                {
                    continue;
                }

                try
                {
                    system.Run(ugfEntity, parentEntity, parentTransform);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public void UGFEntityOnDetachFrom(UGFEntity ugfEntity, UGFEntity parentEntity)
        {
            if (ugfEntity is not IUGFEntityOnDetachFrom)
            {
                return;
            }

            List<SystemObject> systems = this.TypeSystems.GetSystems(ugfEntity.GetType(), typeof(IUGFEntityOnDetachFromSystem));
            if (systems == null)
            {
                return;
            }

            foreach (IUGFEntityOnDetachFromSystem system in systems)
            {
                if (system == null)
                {
                    continue;
                }

                try
                {
                    system.Run(ugfEntity, parentEntity);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public void UGFEntityOnUpdate(UGFEntity ugfEntity, float elapseSeconds, float realElapseSeconds)
        {
            if (ugfEntity is not IUGFEntityOnUpdate)
            {
                return;
            }

            List<SystemObject> systems = this.TypeSystems.GetSystems(ugfEntity.GetType(), typeof(IUGFEntityOnUpdateSystem));
            if (systems == null)
            {
                return;
            }

            foreach (IUGFEntityOnUpdateSystem system in systems)
            {
                if (system == null)
                {
                    continue;
                }

                try
                {
                    system.Run(ugfEntity, elapseSeconds, realElapseSeconds);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }
    }
}
