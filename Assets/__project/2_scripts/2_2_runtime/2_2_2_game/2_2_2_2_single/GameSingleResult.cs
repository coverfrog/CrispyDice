using System;

[Serializable]
public class GameSingleResult
{
    public ulong StageID;
    public GameResult Result;
    public int RollCount;
    public DateTime BeginTime;
    public DateTime EndTime;
    
    public GameSingleResult(ulong stageID)
    {
        StageID = stageID;
    }
}