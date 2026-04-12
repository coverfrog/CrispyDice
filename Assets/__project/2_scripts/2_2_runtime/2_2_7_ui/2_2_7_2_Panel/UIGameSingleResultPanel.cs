using System;
using DG.Tweening;
using RTLTMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIGameSingleResultPanel : UIPanel
{
    [SerializeField] private Image m_imgDimmed;
    [SerializeField] private Button m_btnSubmit;
    [SerializeField] private RTLTextMeshPro m_txtResult;
    [SerializeField] private RTLTextMeshPro m_txtRollCount;

    private const float k_openDuration = 1.0f;
    
    private Tween m_twDimmedFade;
    private Tween m_twSubmitFade;
    private Tween m_twResultFade;
    private Tween m_twRollFade;

    private IProgress<bool> m_onSubmit;
    
    public void Open(GameSingleResult result, IProgress<bool> onSubmit)
    {
        Open();

        m_onSubmit = onSubmit;
        
        if (m_txtResult != null) m_txtResult.text = result.Result.ToString();
        if (m_txtRollCount != null) m_txtRollCount.text = $"Roll : {result.RollCount}";
        if (m_btnSubmit != null) m_btnSubmit.interactable = false;

        Dimmed();
    }
    
    // --------------------------------------------------------------------------
    
    public void CancelDimmed()
    {
        if (m_twDimmedFade is { active: true })
        {
            m_twDimmedFade.Kill();
            m_twDimmedFade = null;
        }

        if (m_twSubmitFade is { active: true })
        {
            m_twSubmitFade.Kill();
            m_twSubmitFade = null;
        }
        
        if (m_twResultFade is { active: true })
        {
            m_twResultFade.Kill();
            m_twResultFade = null;
        }

        if (m_twRollFade is { active: true })
        {
            m_twRollFade.Kill();
            m_twRollFade = null;
        }
    }
    
    public void Dimmed()
    {
        CancelDimmed();

        if (m_imgDimmed)
        {
            m_imgDimmed.color = new Color(0, 0, 0, 0);
            m_twDimmedFade = m_imgDimmed.DOFade(1.0f, k_openDuration);
        }
        
        if (m_btnSubmit)
        {
            m_btnSubmit.image.color = new Color(1, 1, 1, 0);
            m_twSubmitFade = m_btnSubmit.image.DOFade(1.0f, k_openDuration).OnComplete(() => m_btnSubmit.interactable = true);
        }
        
        if (m_txtResult)
        {
            m_txtResult.color = new Color(1, 1, 1, 0);
            m_twResultFade = m_txtResult.DOFade(1.0f, k_openDuration);
        }
        
        if (m_txtRollCount)
        {
            m_txtRollCount.color = new Color(1, 1, 1, 0);
            m_twRollFade = m_txtRollCount.DOFade(1.0f, k_openDuration);
        }
    }
    
    // --------------------------------------------------------------------------

    public void OnClick_Submit()
    {
        SessionManager.Instance.Session_Exit(null, null);
    }
}
