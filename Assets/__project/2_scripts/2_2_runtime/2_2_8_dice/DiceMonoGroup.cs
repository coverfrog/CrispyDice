using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class DiceMonoGroup : MonoBehaviour
{
    [SerializeField] private List<DiceMono> m_dices = new();
    
    public void Roll(DiceFaces faces, SyncList<int> dices, float duration)
    {
        for (int i = 0; i < dices.Count; i++)
        {
            if (i >= m_dices.Count)
            {
                return;
            }
            
            m_dices[i].Roll(Quaternion.Euler(faces[dices[i]]),duration * 0.8f, duration * 0.2f);
        }
    }

    public void Scale(int index, float duration, float maxScale)
    {
        if (index >= m_dices.Count)
        {
            Debug.Assert(false, "유효하지 않습니다.");
            return;
        }
        
        m_dices[index].Scale(duration, maxScale);
    }
}