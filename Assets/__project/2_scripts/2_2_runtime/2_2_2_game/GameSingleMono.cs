using System;
using UnityEngine;

public class GameSingleMono : MonoBehaviour
{
    private bool m_isInstalled;
    
    private void Start()
    {
        Install();
    }

    private void Install()
    {
        IGameInstaller installer = new GameSingleInstallerStory();
        installer.Install(OnInstalled);
    }

    private void OnInstalled()
    {
        IStageLoader loader = new GameSingleStageLoader();
        loader.Load(1, OnStageLoaded);
    }

    private void OnStageLoaded()
    {
        
    }
}