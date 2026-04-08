using FrogLibrary;

public partial class UIManager
{
    private UIGameSinglePanel m_gameSinglePanel;

    public UIGameSinglePanel GameSinglePanel
    {
        get
        {
            if (m_gameSinglePanel == null)
            {
                m_gameSinglePanel = AddressableUtil.Instantiate<UIGameSinglePanel>("ui_panel/game_single", m_trOverlayCanvas);
                m_gameSinglePanel.gameObject.SetActive(false);
            }
            
            return m_gameSinglePanel;
        }
    }
    
    // --------------------------------------------------------------------------
    
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