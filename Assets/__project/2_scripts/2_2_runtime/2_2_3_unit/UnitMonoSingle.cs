using System;
using UnityEngine;
using UnityEngine.UI;

public class UnitMonoSingle : UnitMono
{
    [Header("Static")]
    [SerializeField] private RawImage m_imgDice;
    
    public bool IsEnemy { get; private set; }

    public void SetIsEnemy(bool value)
    {
        IsEnemy = value;
    }

    private void Awake()
    {
        ActiveDice(false);
    }

    public void ActiveDice(bool active)
    {
        if (m_imgDice != null) m_imgDice.gameObject.SetActive(active);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        DataManager.Instance.UnitSingles.Add(this);
    }

    public override void OnStopClient()
    {
        base.OnStopClient();    
        
        DataManager.Instance.UnitSingles.Remove(this);
    }
}
