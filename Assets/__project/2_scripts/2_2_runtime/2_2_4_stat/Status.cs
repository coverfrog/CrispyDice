using System;
using Mirror;
using UnityEngine;

public class Status : NetworkBehaviour
{
    public readonly SyncDictionary<StatType, float> m_status = new();

    public float this[StatType type]
    {
        get => m_status[type];
        set => m_status[type] = value;
    }
    
    public void Setup(StatusCons cons)
    {
        foreach (StatType type in Enum.GetValues(typeof(StatType)))
        {
            m_status.Add(type, 0);
        }

        float hpMax = cons.HPMax;
        m_status[StatType.Hp] = hpMax;
        m_status[StatType.HpMax] = hpMax;
        
        float spMax = cons.SPMax;
        m_status[StatType.Sp] = 0;
        m_status[StatType.SpMax] = spMax;
    }
}