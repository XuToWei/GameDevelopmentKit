using System.Collections.Generic;
using UGF;

namespace ET.Client
{
	/// <summary>
	/// 管理所有UI GameObject
	/// </summary>
	[ComponentOf(typeof(Scene))]
	public class UIEventComponent: Entity, IAwake
	{
		[StaticField]
		public static UIEventComponent Instance;

		public Dictionary<UIFormId, AUIEvent> UIEvents { get; } = new();
	}
}