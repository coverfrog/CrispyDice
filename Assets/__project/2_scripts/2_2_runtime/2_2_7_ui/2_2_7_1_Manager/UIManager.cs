using System;
using UnityEngine;

public partial class UIManager : MonoBehaviour
{
    [SerializeField] private Transform m_trOverlayCanvas;
    
    public static UIManager Instance {get; private set;}

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}