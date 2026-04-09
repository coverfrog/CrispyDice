public class GameSinglePlayer
{
    public DiceMonoGroup DiceGroup;
    public UnitMonoSingle Unit;

    public void RollDice(DiceFaces faces, float duration)
    {
        DiceGroup.Roll(faces, Unit.Dices, duration);
    }
    
    public void ScaleDice(int index, float duration, float maxScale)
    {
        DiceGroup.Scale(index, duration, maxScale);
    }

    public void ApplyStatus()
    {
        Unit.ApplyStatus();   
    }

    public void Attack(UnitMono target, float duration)
    {
        Unit.AttackNormal(target, duration);
    }
}