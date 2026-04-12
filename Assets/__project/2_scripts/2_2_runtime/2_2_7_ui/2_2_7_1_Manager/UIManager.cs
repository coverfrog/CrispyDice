using System;
using System.Collections.Generic;
using UnityEngine;

public partial class UIManager : MonoBehaviour
{
    [SerializeField] private Transform m_trPanelParent;
    [SerializeField] private Transform m_trPopupParent;
    
    public static UIManager Instance {get; private set;}
    
    public readonly Stack<UIPopup> PopupStack = new Stack<UIPopup>();

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