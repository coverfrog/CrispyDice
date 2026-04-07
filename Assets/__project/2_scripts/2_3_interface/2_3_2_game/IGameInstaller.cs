using System;

public interface IGameInstaller : IDisposable
{
    void Install(Action onInstalled);
}