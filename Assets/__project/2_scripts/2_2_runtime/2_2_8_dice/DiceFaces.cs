using FrogLibrary;
using UnityEngine;

[CreateAssetMenu(menuName = "DD/Dice/Faces", fileName = "Dice Faces")]
public class DiceFaces : ScriptableObject
{
    [SerializeField] private UnityDictionary<int, Vector3> m_faces = new();
        
    public Vector3 this[int faceIndex] => m_faces[faceIndex];
    
}