using FrogLibrary;

public partial class UIManager
{
    private UILoadingPanel m_loadingPanel;

    public UILoadingPanel LoadingPanel
    {
        get
        {
            if (m_loadingPanel == null)
            {
                m_loadingPanel = AddressableUtil.Instantiate<UILoadingPanel>("ui_panel/loading", m_trOverlayCanvas);
                m_loadingPanel.gameObject.SetActive(false);
            }
            
            return m_loadingPanel;
        }
    }
}