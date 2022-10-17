using System;
using UGF;
using UnityEngine;
using UnityGameFramework.Runtime;
using GameEntry = UGF.GameEntry;

namespace ET.Client
{
	/// <summary>
	/// 管理所有UI GameObject 以及UI事件
	/// </summary>
	[FriendOf(typeof(UIEventComponent))]
	public static class UIEventComponentSystem
	{
		[ObjectSystem]
		public class UIEventComponentAwakeSystem : AwakeSystem<UIEventComponent>
		{
			protected override void Awake(UIEventComponent self)
			{
				UIEventComponent.Instance = self;
				
				var uiEvents = EventSystem.Instance.GetTypes(typeof (UIEventAttribute));
				foreach (Type type in uiEvents)
				{
					object[] attrs = type.GetCustomAttributes(typeof(UIEventAttribute), false);
					if (attrs.Length == 0)
					{
						continue;
					}

					UIEventAttribute uiEventAttribute = attrs[0] as UIEventAttribute;
					AUIEvent aUIEvent = Activator.CreateInstance(type) as AUIEvent;
					self.UIEvents.Add(uiEventAttribute.UIFormId, aUIEvent);
				}
			}
		}
	}
}