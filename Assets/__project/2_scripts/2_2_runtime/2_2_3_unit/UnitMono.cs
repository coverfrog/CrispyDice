using System;
using Cysharp.Threading.Tasks;
using FrogLibrary;
using Mirror;
using UnityEngine;

[RequireComponent(typeof(NetworkIdentity))]
public abstract class UnitMono : NetworkBehaviour
{
    [Header("Static")]
    [SerializeField] private Transform m_trFlip;
    [SerializeField] private Transform m_trScale;
    [SerializeField] private Status m_status;
    
    [Header("Dynamic")]
    [SerializeField] private Animator m_animator;
    
    // --------------------------------------------------------------------------

    public readonly SyncList<int> Dices = new();
    
    // --------------------------------------------------------------------------

    public async UniTask TaskLoad(ulong id)
    {
        SPUM_Prefabs spum = await AddressableUtil.InstantiateAsync<SPUM_Prefabs>($"model/{id}", m_trFlip);
        m_animator = spum._anim;

        StatusConsTable statusTable = await AddressableUtil.LoadAsync<StatusConsTable>("cons_table/status");
        m_status.Setup(statusTable.Data[id]);
        
        AddressableUtil.Unload("cons_table/status");
    }
    
    // --------------------------------------------------------------------------

    public void Flip(bool isRight)
    {
        if (!m_trFlip)
        {
            Debug.Assert(false,"[Unit] Flip 탐색 오류");
            return;
        }

        m_trFlip.localScale = isRight ? new Vector3(-1, 1, 1) : new Vector3(+1, 1, 1);
    }
    
    // --------------------------------------------------------------------------

    public void ApplyStatus()
    {
        // TODO : 하드 코딩 부분, 데이터 객체 확립 후에 데이터 세팅
        
        m_status[StatType.Sp] = Mathf.Min(m_status[StatType.Sp] + Dices[0], m_status[StatType.SpMax]);
        m_status[StatType.Str] = Dices[1];
        m_status[StatType.Hp] = Mathf.Min(m_status[StatType.Hp] + Dices[2], m_status[StatType.HpMax]);
    }

    public float this[StatType type]
    {
        get => m_status[type];
    }
}