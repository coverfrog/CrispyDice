using FrogLibrary;
using UnityEngine;

[CreateAssetMenu(menuName = "DD/Stat/Colors", fileName = "Stat Colors")]
public class StatColors : ScriptableObject
{
    [SerializeField] private UnityDictionary<StatType, Color> m_colors = new();
        
    public Color this[StatType type] => m_colors[type];
}
