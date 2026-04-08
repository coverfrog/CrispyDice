using System;
using System.Threading;
using Cysharp.Threading.Tasks;

public class GameSingleBattle : IBattle
{
    private UnitMono m_sender;
    private UnitMono m_target;
    
    private IProgress<bool> m_onComplete;

    private CancellationTokenSource m_ctsAttack;
    
    public UniTaskVoid Attack(UnitMono sender, UnitMono target, IProgress<bool> onComplete)
    {
        m_sender = target;
        m_target = target;
        
        m_onComplete = onComplete;
        
        m_ctsAttack = new CancellationTokenSource();

        return TaskAttack(m_ctsAttack.Token);
    }

    private async UniTaskVoid TaskAttack(CancellationToken cancellation)
    {
        
    }
}