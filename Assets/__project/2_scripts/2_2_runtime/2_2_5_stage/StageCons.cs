using System;
using UnityEngine;

[Serializable]
public class StageCons
{
    [SerializeField] private ulong m_id;
    [SerializeField] private ulong m_enemyID;
    
    public ulong ID => m_id;
    public ulong EnemyID => m_enemyID;

    public StageCons(ulong id, ulong enemyID)
    {
        m_id = id;
        m_enemyID = enemyID;
    }
}