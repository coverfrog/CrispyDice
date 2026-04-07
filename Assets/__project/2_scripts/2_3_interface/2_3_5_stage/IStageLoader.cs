using System;
using Cysharp.Threading.Tasks;

public interface IStageLoader
{
    UniTaskVoid Load(ulong stageID, Action<float> onProgress);
}