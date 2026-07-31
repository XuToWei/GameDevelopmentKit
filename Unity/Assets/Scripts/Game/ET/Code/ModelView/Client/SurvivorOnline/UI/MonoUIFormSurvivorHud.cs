using CodeBind;
using UnityEngine;
using UnityEngine.UI;

namespace ET.Client
{
    [MonoCodeBind]
    public sealed partial class MonoUIFormSurvivorHud: AETMonoUGFUIForm
    {
        [SerializeField]
        private Text roomText;

        [SerializeField]
        private Text hpText;

        [SerializeField]
        private Text levelText;

        [SerializeField]
        private Text tickText;

        [SerializeField]
        private Text phaseText;

        public Text RoomText => this.roomText;

        public Text HpText => this.hpText;

        public Text LevelText => this.levelText;

        public Text TickText => this.tickText;

        public Text PhaseText => this.phaseText;
    }
}
