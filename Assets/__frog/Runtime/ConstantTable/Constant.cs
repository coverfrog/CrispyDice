using System;
using UnityEngine;

namespace FrogLibrary
{
    [Serializable]
    public class Constant
    {
        [SerializeField] private ulong m_id;
        
        public ulong Id => m_id;

        public Constant(ulong id)
        {
            m_id = id;
        }
    }
}