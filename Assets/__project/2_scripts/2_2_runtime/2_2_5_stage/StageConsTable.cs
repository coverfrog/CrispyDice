using System;
using System.Collections.Generic;
using FrogLibrary;
using UnityEngine;

public class StageConsTable : ConstantTable
{
    [SerializeField] private UnityDictionary<ulong, StageCons> m_data = new();
        
    public UnityDictionary<ulong, StageCons> Data => m_data;
        
    public override void Load(IReadOnlyDictionary<int, IReadOnlyDictionary<int, IReadOnlyList<object>>> excel)
    {
        m_data.Clear();
        
        foreach ((int sheetIndex, IReadOnlyDictionary<int, IReadOnlyList<object>> sheetData) in excel)
        {
            foreach ((int row, IReadOnlyList<object> cols) in sheetData)
            {
                if (row < 2) continue;

                ulong id = Convert.ToUInt64(cols[0]);
                ulong enemyID = Convert.ToUInt64(cols[1]);

                var cons = new StageCons(id, enemyID);
                
                m_data.Add(id, cons);
            }
        }
    }
}