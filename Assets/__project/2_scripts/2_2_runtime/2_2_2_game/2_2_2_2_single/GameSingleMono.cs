using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using FrogLibrary;
using Mirror;
using UnityEngine;

public class GameSingleMono : MonoBehaviour
{
    [SerializeField] private DiceMonoGroup m_diceGroupMe;
    [SerializeField] private DiceMonoGroup m_diceGroupEnemy;

    private UnityDictionary<bool, GameSinglePlayer> m_players;
    
    private readonly UnityDictionary<ulong, List<GameSingleResult>> m_results = new();
    
    private GameSingleResult m_currentResult;

    private bool m_isQuit;
    
    private void Start()
    {
        Setup().Forget();
    }

    private void OnApplicationQuit()
    {
        m_isQuit = true;
    }

    private void OnDestroy()
    {
        if (m_players != null)
        {
            foreach (GameSinglePlayer player in m_players.Values)
            {
                UnitMonoSingle unit = player.Unit;

                if (unit != null)
                {
                    if (!m_isQuit)
                    {
                        if (unit.netId != 0)
                        {
                            NetworkServer.UnSpawn(unit.gameObject);
                        }
                    
                        Destroy(unit.gameObject);
                    }
                }
            }
        }
    }

    private async UniTaskVoid Setup()
    {
        await UniTask.WaitUntil(() => SessionManager.Instance != null);
        await UniTask.WaitUntil(() => UIManager.Instance != null);
        
        // --------------------------------------------------------------------------
                
#if true
        await UniTask.WaitUntil(() => SessionManager.Instance != null);

        SessionManager.Instance.Session_Host(null, null);
#endif
        
        // --------------------------------------------------------------------------

        IGameInstaller installer = new GameSingleInstallerSingle();
        IStageLoader loader = new GameSingleStageLoader();

        const float k_callTotal = 2;
        const float k_callInterval = 1.0f / k_callTotal;

        int call = 0;

        // --------------------------------------------------------------------------

        const float k_uiDuration = 0.3f;
        
        await UniTask.WaitUntil(() => UIManager.Instance != null);
        
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

        const ulong k_loadStageID = 1; // TODO: 일단 임시로 1번 스테이지만 로드

        if (!m_results.ContainsKey(k_loadStageID))
        {
            m_results.Add(k_loadStageID, new List<GameSingleResult>());
        }

        m_currentResult = new GameSingleResult(k_loadStageID);
        m_results[k_loadStageID].Add(m_currentResult);
        
        loader.Load(k_loadStageID, 
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

        List<UnitMonoSingle> units = DataManager.Instance.UnitSingles;

        m_players = new UnityDictionary<bool, GameSinglePlayer>()
        {
            {
                true, new GameSinglePlayer()
                {
                    Unit = units.Find(x => !x.IsEnemy),
                    DiceGroup = m_diceGroupMe
                }
            },
            {
                false, new GameSinglePlayer()
                {
                    Unit = units.Find(x => x.IsEnemy),
                    DiceGroup = m_diceGroupEnemy
                }
            }
        };
        
        await UniTask.WaitForSeconds(k_uiDuration);
        await UniTask.WaitForSeconds(0.5f);
        
        // --------------------------------------------------------------------------
        
        loadingPanel.Close();
        
        // --------------------------------------------------------------------------
        
        m_currentResult.BeginTime = DateTime.Now;
        
        // --------------------------------------------------------------------------
        
        Betting().Forget();
    }

    private async UniTaskVoid Betting()
    {
        bool isDone = false;
        int selectDice = m_players[true].Unit.GetSelectRemainRand();
        
        // --------------------------------------------------------------------------
        
        UIGameSinglePanel gameSinglePanel = UIManager.Instance.GameSinglePanel;
        gameSinglePanel.Open();
        gameSinglePanel.ActiveBetting(true);
        gameSinglePanel.ActiveDices(m_players[true].Unit.SelectedDices);
        gameSinglePanel.SetSubject("BETTING");
        gameSinglePanel.SelectDice(selectDice, true);
        gameSinglePanel.OnBettingSelect = new Progress<int>(dice =>
        {
            int prevSelectDice = selectDice;
            selectDice = dice;
            
            gameSinglePanel.UnselectDice(prevSelectDice, 0.2f);
            gameSinglePanel.SelectDice(selectDice);
        });
        gameSinglePanel.OnBettingSubmit = new Progress<bool>(_ =>
        {
            isDone = true;
        });
        
        // --------------------------------------------------------------------------
        
        await UniTask.WaitUntil(() => isDone);

        const float k_uiDuration = 0.2f;
        
        m_players[true].Unit.Select(selectDice); 
        m_players[false].Unit.SelectAuto();
        
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
        const int k_diceCount = 3;
        const float k_rollDuration = 1;
        
        // --------------------------------------------------------------------------

        bool isSubmit = false;
        
        UIGameSinglePanel gameSinglePanel = UIManager.Instance.GameSinglePanel;
        gameSinglePanel.SetSubject("ROLL");
        gameSinglePanel.ActiveRoll(true);
        gameSinglePanel.ActiveRollSubmit(true);
        gameSinglePanel.InteractableRollSubmit(false);
        gameSinglePanel.OnRollSubmit = new Progress<bool>(_ =>
        {
            isSubmit = true;
        });

        // --------------------------------------------------------------------------

        List<UnitMonoSingle> units = DataManager.Instance.UnitSingles;
        
        m_players[false].Unit.ActiveDice(true);
        
        foreach (UnitMonoSingle unit in units)
        {
            unit.Dices.Clear();
        }

        bool isClear = !await UniTask
            .WaitUntil(() => units.All(x => x.Dices.Count == 0))
            .TimeoutWithoutException(TimeSpan.FromSeconds(3));

        if (!isClear)
        {
            Debug.Assert(false, "Error");
            return;
        }
        
        foreach (UnitMonoSingle unit in units)
        {
            for (int i = 0; i < k_diceCount; i++)
            {
                unit.Dices.Add(Rand.Next(1, 6));
            }
        }
     
        bool isInit = !await UniTask
            .WaitUntil(() => units.All(x => x.Dices.Count == k_diceCount))
            .TimeoutWithoutException(TimeSpan.FromSeconds(3));
        
        if (!isInit)
        {
            Debug.Assert(false, "Error");
            return;
        }
        
        gameSinglePanel.InteractableRollSubmit(true);
        
        // --------------------------------------------------------------------------
        
        await UniTask.WaitUntil(() => isSubmit);
        
        gameSinglePanel.ActiveRollSubmit(false);

        m_currentResult.RollCount++;
        
        DiceFaces diceFaces = AddressableUtil.Instantiate<DiceFaces>("option/dice_faces");
        
        foreach (GameSinglePlayer player in m_players.Values)
        {
            player.RollDice(diceFaces, k_rollDuration);
        }
        
        AddressableUtil.Unload("option/dice_faces");

        await UniTask.WaitForSeconds(k_rollDuration);
        
        // --------------------------------------------------------------------------
        
        Battle().Forget();
    }

    private async UniTaskVoid Battle()
    {
        
        const float k_turnDuration = 0.2f;
        const float k_scaleDiceMe = 1.4f;
        const float k_scaleDiceEnemy = 2.0f;
        
        UIManager.Instance.GameSinglePanel.SetSubject("BATTLE");
        
        // [Sp]--------------------------------------------------------------------------

        if (m_players[true].IsSp())
        {
            bool isMax = m_players[true].Unit.ApplyStatSp();
         
            m_players[true].ScaleDice(0, k_turnDuration, k_scaleDiceMe);
            
            UIManager.Instance.GameSinglePanel.UpdateUnitView(k_turnDuration, false, true);

            await UniTask.WaitForSeconds(k_turnDuration);

            if (isMax)
            {
                m_players[true].Unit.SetStatStr(m_players[false].Unit[StatType.Hp]);
                m_players[true].Unit.SetStatSp(0);
                m_players[true].AttackNormal(m_players[false].Unit, k_turnDuration);
            }
        }
        else
        {
            m_players[true].ScaleDice(0, k_turnDuration, k_scaleDiceMe);
            await UniTask.WaitForSeconds(k_turnDuration);
            
        }

        if (m_players[false].IsSp())
        {
            bool isMax = m_players[false].Unit.ApplyStatSp();
            
            m_players[false].ScaleDice(0, k_turnDuration, k_scaleDiceEnemy);
            
            UIManager.Instance.GameSinglePanel.UpdateUnitView(k_turnDuration, false, false);

            await UniTask.WaitForSeconds(k_turnDuration);
            
            if (isMax)
            {
                m_players[false].Unit.SetStatStr(m_players[true].Unit[StatType.Hp]);
                m_players[false].Unit.SetStatSp(0);
                m_players[false].AttackNormal(m_players[true].Unit, k_turnDuration);
            }
        }
        else
        {
            m_players[false].ScaleDice(0, k_turnDuration, k_scaleDiceMe);
            await UniTask.WaitForSeconds(k_turnDuration);
        }
        
        UIManager.Instance.GameSinglePanel.UpdateUnitView(k_turnDuration, false);
        
        await UniTask.WaitForSeconds(k_turnDuration);
        
        if (CheckGameEnd())
        {
            return;
        }

        // [Attack]--------------------------------------------------------------------------

        if (m_players[true].IsAttack())
        {
            m_players[true].Unit.ApplyStatStr();
            m_players[true].AttackNormal(m_players[false].Unit, k_turnDuration);
            m_players[true].ScaleDice(1, k_turnDuration, k_scaleDiceMe);
        
            await UniTask.WaitForSeconds(k_turnDuration);
            await UniTask.WaitForSeconds(k_turnDuration);
        }
        else
        {
            m_players[true].ScaleDice(1, k_turnDuration, k_scaleDiceMe);
            await UniTask.WaitForSeconds(k_turnDuration);
        }

        if (m_players[false].IsAttack())
        {
            m_players[false].Unit.ApplyStatStr();
            m_players[false].AttackNormal(m_players[true].Unit, k_turnDuration);
            m_players[false].ScaleDice(1, k_turnDuration, k_scaleDiceEnemy);
        
            await UniTask.WaitForSeconds(k_turnDuration);
            await UniTask.WaitForEndOfFrame();
        }
        else
        {
            m_players[false].ScaleDice(1, k_turnDuration, k_scaleDiceMe);
            await UniTask.WaitForSeconds(k_turnDuration);
        }
        
        UIManager.Instance.GameSinglePanel.UpdateUnitView(k_turnDuration, false);
        
        await UniTask.WaitForSeconds(k_turnDuration);
        
        if (CheckGameEnd())
        {
            return;
        }
        
        // [Heal]--------------------------------------------------------------------------

        if (m_players[true].IsHeal())
        {
            m_players[true].ScaleDice(2, k_turnDuration, k_scaleDiceMe);
            m_players[true].Unit.ApplyStatHp();
        
            UIManager.Instance.GameSinglePanel.UpdateUnitView(k_turnDuration, false, true);

            await UniTask.WaitForSeconds(k_turnDuration);
            await UniTask.WaitForEndOfFrame();
        }
        else
        {
            m_players[true].ScaleDice(2, k_turnDuration, k_scaleDiceMe);
            await UniTask.WaitForSeconds(k_turnDuration);
        }
        
        if (m_players[false].IsHeal())
        {
            m_players[false].ScaleDice(2, k_turnDuration, k_scaleDiceEnemy);
            m_players[false].Unit.ApplyStatHp();
        
            UIManager.Instance.GameSinglePanel.UpdateUnitView(k_turnDuration, false, false);

            await UniTask.WaitForSeconds(k_turnDuration);
            await UniTask.WaitForEndOfFrame();
        }
        else
        {
            m_players[false].ScaleDice(2, k_turnDuration, k_scaleDiceMe);
            await UniTask.WaitForSeconds(k_turnDuration);
        }
        
        // [Betting]--------------------------------------------------------------------------
        
        Betting().Forget();
    }

    private bool CheckGameEnd()
    {
        if (!m_players[true].Unit.IsLive && !m_players[false].Unit.IsLive)
        {
            m_players[true].Unit.OnDead();
            m_players[false].Unit.OnDead();
            
            GameEnd(GameResult.Draw).Forget();
            
            return true;
        }
        
        if (!m_players[false].Unit.IsLive)
        {
            m_players[false].Unit.OnDead();
            
            GameEnd(GameResult.Win).Forget();
            
            return true;
        }
        
        if (!m_players[true].Unit.IsLive)
        {
            m_players[true].Unit.OnDead();
            
            GameEnd(GameResult.Lose).Forget();
            
            return true;
        }
        
        return false;
    }
    
    private async UniTaskVoid GameEnd(GameResult result)
    {
#if true
        Debug.Log($"[Game] 게임 결과 : {result}");
#endif
        m_currentResult.Result = result;
        m_currentResult.EndTime = DateTime.Now;

        UIGameSingleResultPanel panel = UIManager.Instance.GameSingleResultPanel;
        
        // --------------------------------------------------------------------------

        bool isDone = false;
        
        panel.Open(m_currentResult, new Progress<bool>(_ => isDone = true));
        
        await UniTask.WaitUntil(() => isDone);
        
        // --------------------------------------------------------------------------
        
        
    }
}