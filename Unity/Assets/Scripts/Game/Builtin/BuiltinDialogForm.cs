using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class BuiltinDialogForm : BaseBuiltinForm
    {
        [SerializeField] private Text m_TitleText;
        
        [SerializeField] private Text m_MessageText;
        
        [SerializeField] private Text m_ConfirmText;

        [SerializeField] private Text m_CancelText;

        [SerializeField] private Text m_OtherText;

        [SerializeField] private Button m_ConfirmButton;

        [SerializeField] private Button m_CancelButton;

        [SerializeField] private Button m_OtherButton;

        private BuiltinDialogParams m_DialogParams;

        /// <summary>
        /// 打开对话框。
        /// </summary>
        /// <param name="dialogParams"></param>
        public void Open(BuiltinDialogParams dialogParams)
        {
            m_DialogParams = dialogParams;
            
            m_TitleText.text = m_DialogParams.Title;
            m_MessageText.text = m_DialogParams.Message;
            if (string.IsNullOrEmpty(m_DialogParams.ConfirmText))
            {
                m_ConfirmButton.gameObject.SetActive(false);
            }
            else
            {
                m_ConfirmText.text = m_DialogParams.ConfirmText;
                m_ConfirmButton.onClick.RemoveAllListeners();
                m_ConfirmButton.onClick.AddListener(OnClickConfirmButton);
                m_ConfirmButton.gameObject.SetActive(true);
            }
            
            if (string.IsNullOrEmpty(m_DialogParams.CancelText))
            {
                m_CancelButton.gameObject.SetActive(false);
            }
            else
            {
                m_CancelText.text = m_DialogParams.CancelText;
                m_CancelButton.onClick.RemoveAllListeners();
                m_CancelButton.onClick.AddListener(OnClickCancelButton);
                m_CancelButton.gameObject.SetActive(true);
            }

            if (string.IsNullOrEmpty(m_DialogParams.OtherText))
            {
                m_OtherButton.gameObject.SetActive(false);
            }
            else
            {
                m_OtherText.text = m_DialogParams.OtherText;
                m_OtherButton.onClick.RemoveAllListeners();
                m_OtherButton.onClick.AddListener(OnClickOtherButton);
                m_OtherButton.gameObject.SetActive(true);
            }
            
            if (m_DialogParams.PauseGame)
            {
                GameEntry.Base.PauseGame();
            }
            gameObject.SetActive(true);
        }

        /// <summary>
        /// 关闭对话框
        /// </summary>
        protected override void Close()
        {
            base.Close();
            if (m_DialogParams.PauseGame)
            {
                GameEntry.Base.ResumeGame();
            }
        }

        private void OnClickConfirmButton()
        {
            m_DialogParams.OnClickConfirm?.Invoke(m_DialogParams.UserData);
        }
        
        private void OnClickCancelButton()
        {
            m_DialogParams.OnClickCancel?.Invoke(m_DialogParams.UserData);
        }

        private void OnClickOtherButton()
        {
            m_DialogParams.OnClickOther?.Invoke(m_DialogParams.UserData);
        }
    }
}
