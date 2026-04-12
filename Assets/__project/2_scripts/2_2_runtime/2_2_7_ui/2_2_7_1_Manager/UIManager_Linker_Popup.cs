using FrogLibrary;
using UnityEngine;

public partial class UIManager
{
    private UIOptionPopup m_optionPopup;

    public UIOptionPopup OptionPopup
    {
        get
        {
            if (m_optionPopup == null)
            {
                m_optionPopup = AddressableUtil.Instantiate<UIOptionPopup>("ui_popup/option", m_trPopupParent);
                m_optionPopup.gameObject.SetActive(false);
            }
            
            return m_optionPopup;
        }
    }
}