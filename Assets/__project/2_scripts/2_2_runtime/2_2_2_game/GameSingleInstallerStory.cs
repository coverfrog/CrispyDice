using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using FrogLibrary;
using Mirror;
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

    private async UniTaskVoid TaskInstall(CancellationToken token)
    {
#if UNITY_EDITOR
        Debug.Log("[Installer] 설치 시작");
#endif
        
        // --------------------------------------------------------------------------
        
#if UNITY_EDITOR
        Debug.Log("[Installer] 네트워크 연결 대기");
#endif
        
#if true
        for (float x = 0.0f; x < 1.0f; x += Time.deltaTime)
        {
            if (token.IsCancellationRequested)
            {
                return;
            }

            await UniTask.Yield(token);
        }

        SessionManager.Instance.Session_Host(null, null);
#endif

        bool isConn = !await UniTask
            .WaitUntil(() => NetworkClient.active, cancellationToken: token)
            .TimeoutWithoutException(TimeSpan.FromSeconds(3.0f));

        if (!isConn)
        {
            Debug.Assert(false, "[Installer] 네트워크 연결 실패");
            return;
        }
#if UNITY_EDITOR
        Debug.Log("[Installer] 네트워크 연결 성공");
#endif
        
        // --------------------------------------------------------------------------
        
#if UNITY_EDITOR
        Debug.Log("[Installer] 유닛 소환 시도");
#endif
        try
        {
            UniTask<UnitMono> unitTask0 = AddressableUtil.InstantiateAsync<UnitMono>("unit/1");
            UniTask<UnitMono> unitTask1 = AddressableUtil.InstantiateAsync<UnitMono>("unit/2");
            
            bool isUnitSpawned = !await UniTask
                .WaitUntil(() => unitTask0.Status == UniTaskStatus.Succeeded && unitTask1.Status == UniTaskStatus.Succeeded, cancellationToken: token)
                .TimeoutWithoutException(TimeSpan.FromSeconds(3.0f));

            if (!isUnitSpawned)
            {
                Debug.Assert(false, "[Installer] 유닛 소환 실패");
                return;
            }
        }
        catch
        {
            Debug.Assert(false, "[Installer] 유닛 소환 실패");
            return;
        }
        
#if UNITY_EDITOR
        Debug.Log("[Installer] 유닛 소환 성공");
#endif
        
        // --------------------------------------------------------------------------
        
        {   // 유닛 스탯
            
        }

        // _____
        
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