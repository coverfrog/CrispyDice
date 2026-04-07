using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameSingleMono : MonoBehaviour
{
    private bool m_isInstalled;
    
    private void Start()
    {
        Setup().Forget();
    }

    private async UniTaskVoid Setup()
    {
        IGameInstaller installer = new GameSingleInstallerStory();
        IStageLoader loader = new GameSingleStageLoader();

        const float k_callTotal = 2;
        const float k_callInterval = 1.0f / k_callTotal;

        int call = 0;
        int callTarget = 1;
     
        // --------------------------------------------------------------------------

        IProgress<float> progress = new Progress<float>(p =>
        {
            Debug.Log($"[작업 진행] {p}");
        });
        
        // --------------------------------------------------------------------------
        
        UIManager.Instance.LoadingPanel.Open();
        
        // --------------------------------------------------------------------------
        
        installer.Install(p =>
        {
            progress.Report(call * k_callInterval + p * k_callInterval);
            
            if (Mathf.Approximately(p, 1.0f)) 
                call++;
        });
        
        await UniTask.WaitUntil(() => call == callTarget);
        
        // --------------------------------------------------------------------------

        callTarget++;
        
        loader.Load(1, p =>
        {
            progress.Report(call * k_callInterval + p * k_callInterval);
            
            if (Mathf.Approximately(p, 1.0f)) 
                call++;
        });
        
        await UniTask.WaitUntil(() => call == callTarget);
    }
}