using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameSingleInstallerStory : IGameInstaller
{
    private GameSingleMono m_owner;
    
    private CancellationTokenSource m_ctsInstall;
    
    public void Install(GameSingleMono owner)
    {
        m_owner = owner;
        
        m_ctsInstall = new CancellationTokenSource();
        
        TaskInstall(m_ctsInstall.Token);
    }

    private void TaskInstall(CancellationToken token)
    {
#if UNITY_EDITOR
        Debug.Log("[Installer] 설치 시작");
#endif

        {   // 유닛 소환
            
        }

        {   // 유닛 스탯
            
        }

        {   // 유닛 UI
            
        }
    }

    public void Dispose()
    {
        m_owner = null;

        if (m_ctsInstall != null)
        {
            m_ctsInstall.Cancel();
            m_ctsInstall.Dispose();
            m_ctsInstall = null;
        }
    }
}