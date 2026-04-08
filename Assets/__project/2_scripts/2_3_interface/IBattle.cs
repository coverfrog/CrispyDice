using System;
using Cysharp.Threading.Tasks;

public interface IBattle
{
    UniTaskVoid Attack(UnitMono sender, UnitMono target, IProgress<bool> onComplete);
}