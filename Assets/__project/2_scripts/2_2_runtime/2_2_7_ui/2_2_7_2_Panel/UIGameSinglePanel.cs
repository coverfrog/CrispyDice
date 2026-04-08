using System;
using RTLTMPro;
using UnityEngine;

public class UIGameSinglePanel : MonoBehaviour
{
    [SerializeField] private RTLTextMeshPro m_txtSubject;

    [Header("Betting")] 
    [SerializeField] private GameObject m_objBetting;
    [SerializeField] private UIDiceViewGroup m_diceGroup;

    public IProgress<int> OnBettingSelect;
    public IProgress<bool> OnBettingSubmit;
    
    // --------------------------------------------------------------------------
    
    public void Open()
    {
        gameObject.SetActive(true);
        
        m_diceGroup.Open();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
    
    // --------------------------------------------------------------------------

    public void SetSubject(string str)
    {
        if (m_txtSubject != null) m_txtSubject.text = str;
    }

    // --------------------------------------------------------------------------

    public void ActiveBetting(bool active)
    {
        if (m_objBetting != null) m_objBetting.SetActive(active);
    }
    
    public void SelectDice(int dice)
    {
        if (m_diceGroup != null) m_diceGroup.SelectDice(dice);
    }

    public void UnselectDice(int dice, float duration = 0.2f)
    {
        if (m_diceGroup != null) m_diceGroup.UnselectDice(dice, duration);
    }
    
    // --------------------------------------------------------------------------
    
    public void OnClick_BettingSelect(int dice)
    {
        OnBettingSelect?.Report(dice);
    }

    public void OnClick_BettingSubmit()
    {
        OnBettingSubmit?.Report(true);
    }
}
