using UnityEngine;

public class UIUnitView : MonoBehaviour
{
    [SerializeField] private UIStatView m_hpStat;
    [SerializeField] private UIStatView m_spStat;
    
    public void UpdateUnitView(UnitMono unit, float duration = 0.2f, bool isSnap = false)
    {
        OnUpdateUnitView(unit, m_hpStat, StatType.Hp, StatType.HpMax, duration, isSnap);

        if (unit.IsLive)
        {
            OnUpdateUnitView(unit, m_spStat, StatType.Sp, StatType.SpMax, duration, isSnap);
        }
    }

    private void OnUpdateUnitView(UnitMono unit, UIStatView view, StatType cur, StatType max, float duration = 0.2f, bool isSnap = false)
    {
        float vc = unit[cur];
        float vm = unit[max];
        
        view.OnUpdate(vc, vm, duration, isSnap);
    }
}
