using System.Collections.Generic;
using FrogLibrary;
using UnityEngine;

public class UIDiceViewGroup : MonoBehaviour
{
    [SerializeField] private UnityDictionary<int, UIDiceView> m_dices = new();
    
    public void Open(params int[] dices)
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
    
    public void SelectDice(int dice)
    {
        if (m_dices.TryGetValue(dice, out UIDiceView view))
        {
            view.SelectDice();
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