using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyMono : MonoBehaviour
{
    private bool m_isActive;

    private IEnumerator Start()
    {
        m_isActive = false;
        
        yield return new WaitUntil(() => SessionManager.Instance != null);
        yield return new WaitUntil(() => UIManager.Instance != null);
        yield return new WaitUntil(() => AudioManager.Instance != null);
        
        m_isActive = true;
        
        UIManager.Instance.GameSingleResultPanel.Close();
    }

    public void OnClick_Play()
    {
        if (!m_isActive)
        {
            return;
        }
        
        SceneManager.LoadScene("__project/1_scenes/GameSingle");
        UIManager.Instance.LoadingPanel.Open();
    }

    public void OnClick_HowTo()
    {
        if (!m_isActive)
        {
            return;
        }
    }

    public void OnClick_Option()
    {
        if (!m_isActive)
        {
            return;
        }
        
        UIManager.Instance.OptionPopup.Open();
    }
}
