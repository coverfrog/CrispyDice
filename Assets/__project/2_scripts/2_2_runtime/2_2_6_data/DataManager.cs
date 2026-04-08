using System;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    public readonly List<UnitMonoSingle> UnitSingles = new();
    
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}