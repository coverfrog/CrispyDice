public class GameSinglePlayer
{
    public DiceMonoGroup DiceGroup;
    public UnitMonoSingle Unit;

    public void Roll(DiceFaces faces, float duration)
    {
        DiceGroup.Roll(faces, Unit.Dices, duration);
    }

    public void ApplyStatus()
    {
        Unit.ApplyStatus();   
    }
}