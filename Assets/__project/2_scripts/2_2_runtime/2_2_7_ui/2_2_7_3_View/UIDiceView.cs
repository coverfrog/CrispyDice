using DG.Tweening;
using UnityEngine;

public class UIDiceView : MonoBehaviour
{
    private const float k_maxScale = 1.3f;
    private const float k_duration = 0.2f;
    
    private Tween m_twScale;
    
    public void SelectDice(bool isSnap = false)
    {
        if (m_twScale is { active: true })
        {
            m_twScale.Kill();
        }

        if (isSnap)
        {
            transform.localScale = Vector3.one * k_maxScale;
        }

        else
        {
            m_twScale = transform.DOScale(k_maxScale, k_duration);
        }
    }

    public void UnselectDice(float duration = 0.2f, bool isSnap = false)
    {
        if (m_twScale is { active: true })
        {
            m_twScale.Kill();
        }

        if (isSnap)
        {
            transform.localScale = Vector3.one;
        }
        else
        {
            m_twScale = transform.DOScale(1.0f, duration);
        }
    }
}
