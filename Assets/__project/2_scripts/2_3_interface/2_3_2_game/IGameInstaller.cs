using System;

public interface IGameInstaller : IDisposable
{
    void Install(GameSingleMono owner);
}