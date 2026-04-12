using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource m_asBgm;
    
    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayBgm(string adrPath)
    {
        
    }
}
