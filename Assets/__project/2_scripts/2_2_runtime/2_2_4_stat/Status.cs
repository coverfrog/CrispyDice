using System;
using Mirror;
using UnityEngine;

public class Status : NetworkBehaviour
{
    public readonly SyncDictionary<StatType, float> m_status = new();

    public void Setup()
    {
        foreach (StatType type in Enum.GetValues(typeof(StatType)))
        {
            m_status.Add(type, 0);
        }
    }
}