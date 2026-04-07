using System;
using FrogLibrary;
using UnityEngine;

[Serializable]
public class StatusCons
{
    [SerializeField] private ulong m_id;
    [SerializeField] private float m_hpMax;
    [SerializeField] private float m_spMax;
    
    public ulong ID => m_id;
    
    public float HPMax => m_hpMax;
    
    public float SPMax => m_spMax;

    public StatusCons(ulong id, float hpMax, float spMax)
    {
        m_id = id;
        m_hpMax = hpMax;
        m_spMax = spMax;
    }
}