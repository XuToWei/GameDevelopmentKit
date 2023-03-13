using QFSW.QC.Utilities;
using TMPro;
using UnityEngine;

namespace QFSW.QC.Demo
{
    public class RobotCollector : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text = null;
        [SerializeField] private QuantumTheme theme = null;

        public int RescueCount { [Command("demo.rescue-count")] get; set; }

        private void Start() { UpdateText(); }
        private void UpdateText()
        {
            if (!theme) { text.text = $"{RescueCount} robots saved"; }
            else { text.text = $"{RescueCount.ToString().ColorText(theme.DefaultReturnValueColor)} robots saved"; }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Robot robot = collision.gameObject.GetComponent<Robot>();
                robot.Die();
                RescueCount++;
                UpdateText();
            }
        }
    }
}
