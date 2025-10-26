using UnityEngine;

namespace ET.Client
{
    [EnableMethod]
    [ComponentOf(typeof(UIComponent))]
    public partial class UILobbyComponent : UGFUIForm, IUGFUIFormOnInit
    {
        public ETUILobby View { get; set; }
    }
}
