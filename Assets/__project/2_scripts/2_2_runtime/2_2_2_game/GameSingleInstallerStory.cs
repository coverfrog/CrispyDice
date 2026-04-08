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
    private IProgress<float> m_onProgress;
    private IProgress<bool> m_onComplete;
    
    private CancellationTokenSource m_ctsInstall;
    
    public UniTaskVoid Install(IProgress<float> onProgress, IProgress<bool> onComplete)
    {
        m_onProgress = onProgress;
        m_onComplete = onComplete;

        m_ctsInstall = new CancellationTokenSource();

        return TaskInstall(m_ctsInstall.Token);
    }

    private async UniTaskVoid TaskInstall(CancellationToken cancelToken)
    {
        const float k_callTotal = 3;

        int call = 0;

        // --------------------------------------------------------------------------
        
        Log("설치 시작");
        
        // [Task 1]--------------------------------------------------------------------------
        
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
        
        call++;
        m_onProgress?.Report(call / k_callTotal);
        
        // [Task 2]--------------------------------------------------------------------------

        UnitMono1 unit;
        
        try
        {
            UniTask<UnitMono1> unitLocalSpawnTask = AddressableUtil.InstantiateAsync<UnitMono1>("unit/1");
                
            bool isUnitLocalSpawned = !await UniTask
                .WaitUntil(() => unitLocalSpawnTask.Status == UniTaskStatus.Succeeded, cancellationToken: cancelToken)
                .TimeoutWithoutException(TimeSpan.FromSeconds(3.0f));

            if (!isUnitLocalSpawned)
            {
                Log("유닛 소환 실패 (로컬)", true);
                return;
            }

            unit = unitLocalSpawnTask.GetAwaiter().GetResult();
            unit.gameObject.name = "Player";
            unit.transform.position = Vector3.left * 2;
            unit.Flip(true);
            
            await unit.TaskLoad(1);
        }
        catch (Exception e)
        {
            Log("유닛 소환 실패 (로컬)\n" + e.Message, true);
            return;
        }
        
        Log("유닛 소환 성공 (로컬)");
        
        call++;
        m_onProgress?.Report(call / k_callTotal);
        
        // [Task 3]--------------------------------------------------------------------------
        
        NetworkServer.Spawn(unit.gameObject);
        
        bool isUnitNetSpawned = !await UniTask
            .WaitUntil(() => unit.netIdentity.netId != 0, cancellationToken: cancelToken)
            .TimeoutWithoutException(TimeSpan.FromSeconds(3.0f));
        
        if (!isUnitNetSpawned)
        {
            Log("유닛 소환 실패 (네트워크)", true);
            return;
        }
        
        Log("유닛 소환 성공 (네트워크)");
        
        call++;
        m_onProgress?.Report(call / k_callTotal);
        
        // --------------------------------------------------------------------------
        
        m_onComplete?.Report(true);
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