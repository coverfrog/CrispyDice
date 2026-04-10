using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using FrogLibrary;
using Mirror;
using UnityEngine;

[RequireComponent(typeof(NetworkIdentity))]
public abstract class UnitMono : NetworkBehaviour
{
    private static readonly int k_animAttack = Animator.StringToHash("2_Attack");
    private static readonly int k_animDamaged = Animator.StringToHash("3_Damaged");
    private static readonly int k_animTargetDuration = Animator.StringToHash("TargetDuration");
    private static readonly int k_animDeath = Animator.StringToHash("4_Death");
    private static readonly int k_animIsDeath = Animator.StringToHash("isDeath");

    // --------------------------------------------------------------------------
    
    [Header("Static")]
    [SerializeField] private Transform m_trFlip;
    [SerializeField] private Transform m_trScale;
    [SerializeField] private Status m_status;
    
    [Header("Dynamic")]
    [SerializeField] private Animator m_animator;

    [Header("Debug")] 
    public bool IsLive;
    
    // --------------------------------------------------------------------------

    public readonly SyncList<int> Dices = new();

    public readonly SyncList<int> SelectedDices = new();
    
    [Header("SyncVar")]
    [SyncVar] public int SelectDice;
    
    // --------------------------------------------------------------------------

    public async UniTask TaskLoad(ulong id)
    {
        SPUM_Prefabs spum = await AddressableUtil.InstantiateAsync<SPUM_Prefabs>($"model/{id}", m_trFlip);
        m_animator = spum._anim;

        StatusConsTable statusTable = await AddressableUtil.LoadAsync<StatusConsTable>("cons_table/status");
        m_status.Setup(statusTable.Data[id]);
        
        AddressableUtil.Unload("cons_table/status");

        m_animator.SetBool(k_animIsDeath, false);
        
        IsLive = true;
    }
    
    // --------------------------------------------------------------------------

    public void Select(int dice)
    {
        if (SelectedDices.Contains(dice))
        {
            Debug.Assert(false, $"[{gameObject.name}] 중복 추가 - {dice}");
            return;
        }
        
        SelectDice = dice;
        SelectedDices.Add(dice);
    }
    
    public void SelectAuto()
    {
        int dice = GetSelectRemainRand();
        
        Select(dice);
    }

    public int GetSelectRemainRand()
    {
        List<int> notSelects = Enumerable.Range(1, 6).Where(x => !SelectedDices.Contains(x)).ToList();

        if (notSelects is { Count: 0 })
        {
            SelectedDices.Clear();
            notSelects = Enumerable.Range(1, 6).ToList();
        }
        
        int index = Rand.Next(0, notSelects.Count - 1);
        int dice = notSelects[index];
        
        return dice;
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

    public void AttackNormal(UnitMono target, float duration)
    {
        m_animator.SetTrigger(k_animAttack);
        m_animator.SetFloat(k_animTargetDuration, duration);
        
        target.OnDamage(this, m_status[StatType.Str]);
    }
    
    // --------------------------------------------------------------------------

    public void OnDamage(UnitMono sender, float damage)
    {
        m_status[StatType.Hp] = Mathf.Max(0, m_status[StatType.Hp] - damage);

        if (m_status[StatType.Hp] == 0)
        {
            IsLive = false;
        }
        
        m_animator.SetTrigger(k_animDamaged);    
    }

    public void OnDead()
    {
        m_animator.SetTrigger(k_animDeath);
        m_animator.SetBool(k_animIsDeath, true);
    }
    
    // --------------------------------------------------------------------------

    public bool ApplyStatSp()
    {
        m_status[StatType.Sp] = Mathf.Min(m_status[StatType.Sp] + Dices[0], m_status[StatType.SpMax]);
        return Mathf.Approximately(m_status[StatType.Sp], m_status[StatType.SpMax]);
    }
    
    public void ApplyStatHp()
    {
        m_status[StatType.Hp] = Mathf.Min(m_status[StatType.Hp] + Dices[2], m_status[StatType.HpMax]);
    }
    
    public void ApplyStatStr()
    {
        m_status[StatType.Str] = Dices[1];
    }
    
    // --------------------------------------------------------------------------

    public void SetStatSp(float sp)
    {
        m_status[StatType.Sp] = Mathf.Clamp(sp, 0, m_status[StatType.SpMax]);
    }
    
    // --------------------------------------------------------------------------

    public float this[StatType type]
    {
        get => m_status[type];
    }
}