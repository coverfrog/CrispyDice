using System.Collections.Generic;
using FrogLibrary;
using Mirror;
using UnityEngine;

public class UIDiceViewGroup : MonoBehaviour
{
    [SerializeField] private UnityDictionary<int, UIDiceView> m_dices = new();
    
    public void Open()
    {
        gameObject.SetActive(true);

        foreach (UIDiceView view in m_dices.Values)
        {
            view.UnselectDice(isSnap: true);
        }
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    // --------------------------------------------------------------------------
    
    public void ActiveDices(SyncList<int> selectedDices)
    {
        foreach (UIDiceView view in m_dices.Values)
        {
            view.gameObject.SetActive(false);
        }
        
        for (int i = 1; i <= 6; i++)
        {
            if (selectedDices.Contains(i))
                continue;

            if (m_dices.TryGetValue(i, out UIDiceView view))
            {
                view.gameObject.SetActive(true);
            }
        }
    }
    public void SelectDice(int dice, bool isSnap = false)
    {
        if (m_dices.TryGetValue(dice, out UIDiceView view))
        {
            view.SelectDice(isSnap);
        }
    }

    public void UnselectDice(int dice, float duration = 0.2f)
    {
        if (m_dices.TryGetValue(dice, out UIDiceView view))
        {
            view.UnselectDice(duration);
        }
    }
}