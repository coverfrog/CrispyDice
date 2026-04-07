using System;
using UnityEngine;

public class GameSingleMono : MonoBehaviour
{
    private void Start()
    {
        Setup();
    }

    private void Setup()
    {
        IGameInstaller installer = new GameSingleInstallerStory();
        installer.Install(this);
    }
}