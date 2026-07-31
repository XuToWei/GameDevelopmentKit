using CodeBind;
using UnityEngine;
using UnityEngine.UI;

namespace ET.Client
{
    [MonoCodeBind]
    public sealed partial class MonoUIFormSurvivorLobby: AETMonoUGFUIForm
    {
        [SerializeField]
        private InputField roomCodeInput;

        [SerializeField]
        private Button joinButton;

        [SerializeField]
        private Button startButton;

        [SerializeField]
        private Text statusText;

        public InputField RoomCodeInput => this.roomCodeInput;

        public Button JoinButton => this.joinButton;

        public Button StartButton => this.startButton;

        public Text StatusText => this.statusText;
    }
}
