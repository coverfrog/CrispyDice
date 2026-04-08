using DG.Tweening;
using UnityEngine;

public class DiceMono : MonoBehaviour
{
    private Tween m_twTween;

    public void Roll(Quaternion lastRot, float durSpin, float durComplete)
    {
        const float k_randMin = 360.0f * 4;
        const float k_randMax = 360.0f * 5;
        
        Vector3 spinEuler = new Vector3(
            Rand.Next(k_randMin, k_randMax),
            Rand.Next(k_randMin, k_randMax),
            Rand.Next(k_randMin, k_randMax));
        
        transform.DORotate(transform.eulerAngles + spinEuler, durSpin, RotateMode.FastBeyond360)
            .SetEase(Ease.OutBounce)
            .OnComplete(() =>
            {
                m_twTween = transform.DORotateQuaternion(lastRot, durComplete).SetEase(Ease.OutBack);
            });
    }
}