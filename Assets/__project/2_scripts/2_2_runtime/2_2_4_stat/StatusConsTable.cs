using System;
using System.Collections.Generic;
using FrogLibrary;
using UnityEngine;

public class StatusConsTable : ConstantTable
{
    [SerializeField] private UnityDictionary<ulong, StatusCons> m_data = new();
    
    public UnityDictionary<ulong, StatusCons> Data => m_data;
    
    public override void Load(IReadOnlyDictionary<int, IReadOnlyDictionary<int, IReadOnlyList<object>>> excel)
    {
        m_data.Clear();
        
        foreach ((int sheetIndex, IReadOnlyDictionary<int, IReadOnlyList<object>> sheetData) in excel)
        {
            foreach ((int row, IReadOnlyList<object> cols) in sheetData)
            {
                if (row < 2) continue;

                ulong id = Convert.ToUInt64(cols[0]);
                float hpMax = Convert.ToSingle(cols[1]);
                float spMax = Convert.ToSingle(cols[2]);

                var cons = new StatusCons(id, hpMax, spMax);
                
                m_data.Add(id, cons);
            }
        }
    }
}