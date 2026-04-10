using System;
using System.Collections.Generic;
using Mirror;
using RTLTMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGameSinglePanel : UIPanel
{
    [SerializeField] private RTLTextMeshPro m_txtSubject;

    [Header("Betting")] 
    [SerializeField] private GameObject m_objBetting;
    [SerializeField] private UIDiceViewGroup m_diceGroup;

    [Header("Roll")]
    [SerializeField] private GameObject m_objRoll;
    [SerializeField] private Button m_btnRollSubmit;

    [Header("Unit")]
    [SerializeField] private UIUnitView m_unitMe;
    [SerializeField] private UIUnitView m_unitEnemy;
    
    public IProgress<int> OnBettingSelect;
    public IProgress<bool> OnBettingSubmit;
    public IProgress<bool> OnRollSubmit;
    
    // --------------------------------------------------------------------------
    
    public override void Open()
    {
        base.Open();
        
        m_diceGroup.Open();
        
        ActiveBetting(false);
        ActiveRoll(false);
    }

    // --------------------------------------------------------------------------

    public void SetSubject(string str)
    {
        if (m_txtSubject != null) m_txtSubject.text = str;
    }

    // --------------------------------------------------------------------------

    public void UpdateUnitView(float duration, bool isSnap)
    {
        m_unitMe.UpdateUnitView(DataManager.Instance.UnitSingles.Find(x => !x.IsEnemy), duration, isSnap);
        m_unitEnemy.UpdateUnitView(DataManager.Instance.UnitSingles.Find(x => x.IsEnemy), duration, isSnap);
    }
    
    public void UpdateUnitView(float duration, bool isSnap, bool isMe)
    {
        if (isMe)
            m_unitMe.UpdateUnitView(DataManager.Instance.UnitSingles.Find(x => !x.IsEnemy), duration, isSnap);
        else
            m_unitEnemy.UpdateUnitView(DataManager.Instance.UnitSingles.Find(x => x.IsEnemy), duration, isSnap);
    }
    
    // --------------------------------------------------------------------------

    public void ActiveBetting(bool active)
    {
        if (m_objBetting != null) m_objBetting.SetActive(active);
    }

    public void ActiveDices(SyncList<int> selectedDices)
    {
        if (m_diceGroup != null) m_diceGroup.ActiveDices(selectedDices);
    }

    public void SelectDice(int dice, bool isSnap = false)
    {
        if (m_diceGroup != null) m_diceGroup.SelectDice(dice, isSnap);
    }

    public void UnselectDice(int dice, float duration)
    {
        if (m_diceGroup != null) m_diceGroup.UnselectDice(dice, duration);
    }
    
    // --------------------------------------------------------------------------

    public void ActiveRoll(bool active)
    {
        if (m_objRoll != null) m_objRoll.SetActive(active);
    }

    public void ActiveRollSubmit(bool active)
    {
        if (m_btnRollSubmit != null) m_btnRollSubmit.gameObject.SetActive(active);
    }
    
    public void InteractableRollSubmit(bool interactable)
    {
        if (m_btnRollSubmit != null) m_btnRollSubmit.interactable = interactable;
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

    public void OnClick_RollSubmit()
    {
        OnRollSubmit?.Report(true);
    }
}
