using System;
using DG.Tweening;
using RTLTMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class UILoadingPanel : MonoBehaviour
{
    [SerializeField] private Image m_imgGage;
    [SerializeField] private RTLTextMeshPro m_txtProgress;

    private Tween m_twProgress;
    
    // --------------------------------------------------------------------------
    
    public void Open()
    {
        gameObject.SetActive(true);
        
        UpdateProgress(this, 0.0f, true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
        
        if (m_twProgress is { active: true })
        {
            m_twProgress.Kill();
        }
    }

    // --------------------------------------------------------------------------
    
    public void UpdateProgress(Object sender, float progress, bool isSnap = false, float duration = 0.3f)
    {
        progress = Mathf.Clamp01(progress);
        
        if (m_twProgress is { active: true })
        {
            m_twProgress.Kill();
        }

        if (isSnap)
        {
            OnUpdateProgress(progress);
        }
        else
        {
            m_twProgress = DOVirtual.Float(m_imgGage.fillAmount, progress, duration, OnUpdateProgress);
        }
    }

    private void OnUpdateProgress(float progress)
    {
        if (m_imgGage != null) m_imgGage.fillAmount = progress;
        if (m_txtProgress != null) m_txtProgress.text = $"{progress * 100.0f:0.0}%";
    }
}