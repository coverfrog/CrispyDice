using System;
using System.Diagnostics;
using System.Threading;
using Cysharp.Threading.Tasks;
using FrogLibrary;
using Mirror;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class GameSingleStageLoader : IStageLoader
{
    private ulong m_stageID;
    
    private IProgress<float> m_onProgress;
    private IProgress<bool> m_onComplete;
 
    private CancellationTokenSource m_ctsLoad;
    
    public UniTaskVoid Load(ulong stageID, IProgress<float> onProgress, IProgress<bool> onComplete)
    {
        m_onProgress = onProgress;
        m_onComplete = onComplete;
        
        m_stageID = stageID;
        
        m_ctsLoad = new CancellationTokenSource();

        return TaskLoad(m_ctsLoad.Token);
    }

    private async UniTaskVoid TaskLoad(CancellationToken cancelToken)
    {
        const string k_adrTable = "cons_table/stage";
        const float k_callTotal = 4;

        int call = 0;
        
        StageConsTable table;
        
        // --------------------------------------------------------------------------
        
        Log("로딩 시작");
        
        // [Task 1]--------------------------------------------------------------------------
        
        try
        {
            UniTask<StageConsTable> tableTask = AddressableUtil.LoadAsync<StageConsTable>(k_adrTable);

            bool isTableLoaded = !await UniTask
                .WaitUntil(() => tableTask.Status == UniTaskStatus.Succeeded, cancellationToken: cancelToken)
                .TimeoutWithoutException(TimeSpan.FromSeconds(3.0f));

            if (!isTableLoaded)
            {
                Log("테이블 로딩 실패", true);
                return;
            }
            
            table = tableTask.GetAwaiter().GetResult();
        }
        catch (Exception e)
        {
            Log("테이블 로딩 실패\n" + e.Message, true);
            return;
        }
        
        Log("테이블 로딩 성공");
        
        call++;
        m_onProgress?.Report(call / k_callTotal);

        // --------------------------------------------------------------------------

        if (!table.Data.TryGetValue(m_stageID, out StageCons stageCons))
        {
            Log("테이블에 키가 포함되어 있지 않습니다.", true);
            return;
        }
        
        // [Task 2]--------------------------------------------------------------------------

        UnitMonoSingle unit;
        
        try
        {
            UniTask<UnitMonoSingle> unitLocalSpawnTask = AddressableUtil.InstantiateAsync<UnitMonoSingle>("unit/single");
      
            bool isUnitLocalSpawned = !await UniTask
                .WaitUntil(() => unitLocalSpawnTask.Status == UniTaskStatus.Succeeded, cancellationToken: cancelToken)
                .TimeoutWithoutException(TimeSpan.FromSeconds(3.0f));

            if (!isUnitLocalSpawned)
            {
                Log("유닛 소환 실패 (로컬)", true);
                return;
            }
        
            unit = unitLocalSpawnTask.GetAwaiter().GetResult();
            unit.gameObject.name = $"Enemy";
            unit.transform.position = Vector3.right * 3;
            unit.Flip(false);
            unit.SetIsEnemy(true);
            
            await unit.TaskLoad(stageCons.EnemyID);
        }
        catch (Exception e)
        {
            Log("유닛 소환 실패 (로컬)\n" + e.Message, true);
            throw;
        }
        
        AddressableUtil.Unload(k_adrTable);
        
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
        
                
        // [Task 4]--------------------------------------------------------------------------

        UIManager.Instance.GameSinglePanel.UpdateUnitView(isSnap: true);

        for (int i = 0; i < 2; i++)
        {
            await UniTask.WaitForEndOfFrame(cancelToken);
        }

        call++;
        m_onProgress?.Report(call / k_callTotal);

        // --------------------------------------------------------------------------

        m_onComplete?.Report(true);
    }
    
    [Conditional("UNITY_EDITOR")]
    private void Log(object o, bool isError = false)
    {
        const string tag = "[Stage Loader]";
        
        if (isError)
            Debug.LogError($"{tag} {o}");
        else
            Debug.Log($"{tag} {o}");
    }
}