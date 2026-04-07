using System;
using UnityEngine;

public partial class UIManager : MonoBehaviour
{
    [SerializeField] private Transform m_trOverlayCanvas;
    
    public static UIManager Instance {get; private set;}

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}