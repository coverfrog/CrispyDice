using System;
using DG.Tweening;
using UnityEngine;

public class DiceMono : MonoBehaviour
{
    private Tween m_twRot;
    private Tween m_twScale;

    public void Roll(Quaternion lastRot, float durSpin, float durComplete)
    {
        CancelRoll();
        
        const float k_randMin = 360.0f * 4;
        const float k_randMax = 360.0f * 5;
        
        Vector3 spinEuler = new Vector3(
            Rand.Next(k_randMin, k_randMax),
            Rand.Next(k_randMin, k_randMax),
            Rand.Next(k_randMin, k_randMax));
        
        m_twRot = transform.DORotate(transform.eulerAngles + spinEuler, durSpin, RotateMode.FastBeyond360)
            .SetEase(Ease.OutBounce)
            .OnComplete(() =>
            {
                m_twRot = transform.DORotateQuaternion(lastRot, durComplete).SetEase(Ease.OutBack);
            });
    }

    public void CancelRoll()
    {
        if (m_twRot is { active: true })
        {
            m_twRot.Kill();
            m_twRot = null;
        }
    }
    
    public void Scale(float duration, float maxScale)
    {
        CancelScale();

        m_twScale = transform.DOScale(maxScale, duration).SetLoops(2, LoopType.Yoyo);
    }

    public void CancelScale()
    {
        if (m_twScale is { active: true })
        {
            m_twScale.Kill();
            m_twScale = null;
        }
    }
}