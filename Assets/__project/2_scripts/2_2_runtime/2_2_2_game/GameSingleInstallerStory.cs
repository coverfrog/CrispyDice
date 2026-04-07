using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using FrogLibrary;
using Mirror;
using UnityEngine;

public class GameSingleInstallerStory : IGameInstaller
{
    private Action m_onInstalled;
    private CancellationTokenSource m_ctsInstall;
    
    public void Install(Action onInstalled)
    {
        m_onInstalled = onInstalled;
        m_ctsInstall = new CancellationTokenSource();
        
        TaskInstall(m_ctsInstall.Token).Forget();
    }

    private async UniTaskVoid TaskInstall(CancellationToken cancelToken)
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
            if (cancelToken.IsCancellationRequested)
            {
                return;
            }

            await UniTask.Yield(cancelToken);
        }

        SessionManager.Instance.Session_Host(null, null);
#endif

        bool isConn = !await UniTask
            .WaitUntil(() => NetworkClient.active, cancellationToken: cancelToken)
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
            UniTask<UnitMono1> unitLocalSpawnTask = AddressableUtil.InstantiateAsync<UnitMono1>("unit/1");
                
            bool isUnitLocalSpawned = !await UniTask
                .WaitUntil(() => unitLocalSpawnTask.Status == UniTaskStatus.Succeeded, cancellationToken: cancelToken)
                .TimeoutWithoutException(TimeSpan.FromSeconds(3.0f));

            if (!isUnitLocalSpawned)
            {
                Debug.Assert(false, "[Installer] 유닛 소환 실패");
                return;
            }

            UnitMono1 unit = unitLocalSpawnTask.GetAwaiter().GetResult();
            unit.gameObject.name = "Player";
            
            await unit.TaskLoad(1);
        }
        catch (Exception e)
        {
            Debug.Assert(false, "[Installer] 유닛 소환 실패 : " + e.Message);
            return;
        }
        
#if UNITY_EDITOR
        Debug.Log("[Installer] 유닛 소환 성공");
#endif
        
        // --------------------------------------------------------------------------
        
        m_onInstalled?.Invoke();
    }

    public void Dispose()
    {
        if (m_ctsInstall != null)
        {
            m_ctsInstall.Cancel();
            m_ctsInstall.Dispose();
            m_ctsInstall = null;
        }
    }
}