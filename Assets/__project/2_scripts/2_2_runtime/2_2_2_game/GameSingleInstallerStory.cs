using System;
using System.Diagnostics;
using System.Threading;
using Cysharp.Threading.Tasks;
using FrogLibrary;
using Mirror;
using UnityEngine;
using Debug = UnityEngine.Debug;

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
        Log("설치 시작");
        
        // --------------------------------------------------------------------------
        
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
            Log("네트워크 연결 실패", true);
            return;
        }
        
        Log("네트워크 연결 성공");
        
        
        // --------------------------------------------------------------------------
        
        try
        {
            UniTask<UnitMono1> unitLocalSpawnTask = AddressableUtil.InstantiateAsync<UnitMono1>("unit/1");
                
            bool isUnitLocalSpawned = !await UniTask
                .WaitUntil(() => unitLocalSpawnTask.Status == UniTaskStatus.Succeeded, cancellationToken: cancelToken)
                .TimeoutWithoutException(TimeSpan.FromSeconds(3.0f));

            if (!isUnitLocalSpawned)
            {
                Log("유닛 소환 실패", true);
                return;
            }

            UnitMono1 unit = unitLocalSpawnTask.GetAwaiter().GetResult();
            unit.gameObject.name = "Player";
            unit.transform.position = Vector3.left;
            unit.Flip(true);
            
            await unit.TaskLoad(1);
        }
        catch (Exception e)
        {
            Log("유닛 소환 실패\n" + e.Message, true);
            return;
        }
        
        Log("유닛 소환 성공");
        
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

    [Conditional("UNITY_EDITOR")]
    private void Log(object o, bool isError = false)
    {
        const string tag = "[Game Installer]";
        
        if (isError)
            Debug.LogError($"{tag} {o}");
        else
            Debug.Log($"{tag} {o}");
    }
}