using System;
using Cysharp.Threading.Tasks;

public interface IGameInstaller : IDisposable
{
    UniTaskVoid Install(IProgress<float> onProgress, IProgress<bool> onComplete);
}