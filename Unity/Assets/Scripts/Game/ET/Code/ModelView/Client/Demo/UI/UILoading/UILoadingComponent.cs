using UnityEngine;
using UnityEngine.UI;

namespace ET.Client
{
	[EnableMethod]
    [ComponentOf(typeof (UGFUIForm))]
	public class UILoadingComponent : Entity, IAwake
	{
		public Text text;
	}
}
