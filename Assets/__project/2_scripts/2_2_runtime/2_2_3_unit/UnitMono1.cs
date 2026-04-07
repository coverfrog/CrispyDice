using System;
using Cysharp.Threading.Tasks;
using FrogLibrary;
using Mirror;
using UnityEngine;

[RequireComponent(typeof(NetworkIdentity))]
public class UnitMono1 : NetworkBehaviour
{
    [Header("Static")]
    [SerializeField] private Transform m_trFlip;
    [SerializeField] private Transform m_trScale;
    [SerializeField] private Status m_status;
    
    [Header("Dynamic")]
    [SerializeField] private Animator m_animator;
    
    public override void OnStartClient()
    {
        base.OnStartClient();

        DataManager.Instance.Units.Add(this);
    }

    public override void OnStopClient()
    {
        base.OnStopClient();    
        
        DataManager.Instance.Units.Remove(this);
    }

    public async UniTask TaskLoad(ulong id)
    {
        SPUM_Prefabs spum = await AddressableUtil.InstantiateAsync<SPUM_Prefabs>($"model/{id}", m_trFlip);
        m_animator = spum._anim;

        StatusConsTable statusTable = await AddressableUtil.LoadAsync<StatusConsTable>("cons_table/status");
        m_status.Setup(statusTable.Data[id]);
        
        AddressableUtil.Unload("cons_table/status");
    }
}