using System;
using DG.Tweening;
using RTLTMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIStatView : MonoBehaviour
{
    [SerializeField] private Image m_imgGage;
    [SerializeField] private RTLTextMeshPro m_txtSubject;

    private Tween m_twGage;
    
    public void OnUpdate(float cur, float max, float duration = 0.2f, bool isSnap = false)
    {
        if (m_txtSubject != null) m_txtSubject.text = $"({cur}/{max})";

        if (m_twGage is { active: true })
        {
            m_twGage.Kill();
            m_twGage = null;
        }
        
        if (m_imgGage)
        {
            if (isSnap)
            {
                m_imgGage.fillAmount = cur / max;
            }
            else
            {
                m_twGage = m_imgGage.DOFillAmount(cur/max, duration);
            }
        }
    }

    private void OnDisable()
    {
        if (m_twGage is { active: true })
        {
            m_twGage.Kill();
            m_twGage = null;
        }

    }
}
