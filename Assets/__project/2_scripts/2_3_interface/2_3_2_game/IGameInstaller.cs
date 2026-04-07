using System;
using Cysharp.Threading.Tasks;

public interface IGameInstaller : IDisposable
{
    UniTaskVoid Install(Action<float> onProgress);
}