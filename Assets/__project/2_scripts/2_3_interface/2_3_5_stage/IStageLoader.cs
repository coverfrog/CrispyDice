using System;

public interface IStageLoader
{
    void Load(ulong stageID, Action onLoaded);
}