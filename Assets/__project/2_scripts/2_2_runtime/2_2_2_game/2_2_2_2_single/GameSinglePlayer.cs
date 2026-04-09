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

    public void Attack(UnitMono target, float duration)
    {
        Unit.AttackNormal(target, duration);
    }

    public bool IsSp()
    {
        return Unit.Dices[0] == Unit.SelectDice;
    }
    
    public bool IsAttack()
    {
        return Unit.Dices[1] == Unit.SelectDice;
    }
    
    public bool IsHeal()
    {
        return Unit.Dices[2] == Unit.SelectDice;
    }
}