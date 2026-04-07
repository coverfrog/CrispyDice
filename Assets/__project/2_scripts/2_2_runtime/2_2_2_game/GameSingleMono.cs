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

        // --------------------------------------------------------------------------

        const float uiUpdateDuration = 0.3f;
        
        UILoadingPanel loadingPanel = UIManager.Instance.LoadingPanel;
        
        loadingPanel.Open();
        
        IProgress<float> progress = new Progress<float>(p =>
        {
            loadingPanel.UpdateProgress(this, p, duration: uiUpdateDuration);
        });

        // [ Task 1 ]--------------------------------------------------------------------------

        bool isDone = false;
        
        installer.Install(
            new Progress<float>(p =>
            {
                progress.Report(call * k_callInterval + p * k_callInterval);
            }),
            new Progress<bool>(b =>
            {
                call++;
                isDone = true;
            }));
        
        await UniTask.WaitUntil(() => isDone);
        
        // [ Task 2 ]--------------------------------------------------------------------------

        isDone = false;
        
        loader.Load(1, 
            new Progress<float>(p =>
            {
                progress.Report(call * k_callInterval + p * k_callInterval);
            }),
            new Progress<bool>(b =>
            {
                call++;
                isDone = true;
            }));
        
        await UniTask.WaitUntil(() => isDone);
        
        // --------------------------------------------------------------------------

        await UniTask.WaitForSeconds(uiUpdateDuration);
        
        // --------------------------------------------------------------------------
        
        loadingPanel.Close();
    }
}