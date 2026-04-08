using System;
using Cysharp.Threading.Tasks;

public interface IStageLoader
{
    UniTaskVoid Load(ulong stageID, IProgress<float> onProgress, IProgress<bool> onComplete);
}