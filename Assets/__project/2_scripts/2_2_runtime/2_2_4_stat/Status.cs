using System;
using Mirror;
using UnityEngine;

public class Status : NetworkBehaviour
{
    public readonly SyncDictionary<StatType, float> m_status = new();

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
        m_status[StatType.Sp] = spMax;
        m_status[StatType.SpMax] = spMax;
    }
}