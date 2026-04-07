using System;
using System.Diagnostics;
using System.Threading;
using Cysharp.Threading.Tasks;
using FrogLibrary;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class GameSingleStageLoader : IStageLoader
{
    private ulong m_stageID;
    private Action m_onLoaded;
 
    private CancellationTokenSource m_ctsLoad;
    
    public void Load(ulong stageID, Action onLoaded)
    {
        m_onLoaded = onLoaded;
        m_stageID = stageID;
        
        m_ctsLoad = new CancellationTokenSource();
        
        TaskLoad(m_ctsLoad.Token).Forget();
    }

    private async UniTaskVoid TaskLoad(CancellationToken cancelToken)
    {
        const string k_adrTable = "cons_table/stage";

        StageConsTable table;
        
        // --------------------------------------------------------------------------
        
        Log("로딩 시작");
        
        // --------------------------------------------------------------------------
        
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

        // --------------------------------------------------------------------------

        if (!table.Data.TryGetValue(m_stageID, out StageCons stageCons))
        {
            Log("테이블에 키가 포함되어 있지 않습니다.", true);
            return;
        }
        
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
            unit.gameObject.name = $"Enemy";
            unit.transform.position = Vector3.right;
            unit.Flip(false);
            
            await unit.TaskLoad(stageCons.EnemyID);
        }
        catch (Exception e)
        {
            Log("유닛 소환 실패\n" + e.Message, true);
            throw;
        }
        
        Log("유닛 소환 성공");
                
        // --------------------------------------------------------------------------
        
        AddressableUtil.Unload(k_adrTable);
        
        // --------------------------------------------------------------------------
        
        m_onLoaded?.Invoke();
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