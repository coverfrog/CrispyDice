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
        // 스테이지 로딩은 원래 따로 들어가야 하지만 초기 로딩에는 포함 되서 같이 넣어둠
        
        IGameInstaller installer = new GameSingleInstallerStory();
        IStageLoader loader = new GameSingleStageLoader();

        const float k_callTotal = 2;
        const float k_callInterval = 1.0f / k_callTotal;

        int call = 0;

        // --------------------------------------------------------------------------

        const float k_uiDuration = 0.3f;
        
        UILoadingPanel loadingPanel = UIManager.Instance.LoadingPanel;
        loadingPanel.Open();
        
        IProgress<float> progress = new Progress<float>(p =>
        {
            loadingPanel.UpdateProgress(this, p, duration: k_uiDuration);
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

        await UniTask.WaitForSeconds(k_uiDuration);
        await UniTask.WaitForSeconds(0.5f);
        
        // --------------------------------------------------------------------------
        
        loadingPanel.Close();
        
        // --------------------------------------------------------------------------
        
        Betting().Forget();
    }

    private async UniTaskVoid Betting()
    {
        bool isDone = false;
        int selectDice = 1;
        
        // --------------------------------------------------------------------------
        
        UIGameSinglePanel gameSinglePanel = UIManager.Instance.GameSinglePanel;
        gameSinglePanel.Open();
        gameSinglePanel.ActiveBetting(true);
        gameSinglePanel.SetSubject("BETTING");
        gameSinglePanel.SelectDice(selectDice);
        gameSinglePanel.OnBettingSelect = new Progress<int>(dice =>
        {
            int prevSelectDice = selectDice;
            selectDice = dice;
            
            gameSinglePanel.UnselectDice(prevSelectDice);
            gameSinglePanel.SelectDice(selectDice);
        });
        gameSinglePanel.OnBettingSubmit = new Progress<bool>(_ =>
        {
            isDone = true;
        });
        
        // --------------------------------------------------------------------------
        
        await UniTask.WaitUntil(() => isDone);

        const float k_uiDuration = 0.2f;
        
        gameSinglePanel.OnBettingSelect = null;
        gameSinglePanel.OnBettingSubmit = null;
        gameSinglePanel.UnselectDice(selectDice, k_uiDuration);
        
        await UniTask.WaitForSeconds(k_uiDuration);
        
        gameSinglePanel.ActiveBetting(false);
        
        // --------------------------------------------------------------------------
        
        RollDice().Forget();
    }

    private async UniTaskVoid RollDice()
    {
        UIGameSinglePanel gameSinglePanel = UIManager.Instance.GameSinglePanel;
        gameSinglePanel.SetSubject("ROLL");
        
        foreach (UnitMono1 unit in DataManager.Instance.Units)
        {
            unit.Dices.Clear();

            for (int i = 0; i < 3; i++)
            {
                unit.Dices.Add(Rand.Next(1, 6));
            }
        }
    }
}