using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LevelDifficult
{
    Easy,
    Normal,
    Hard
}

public class Level : MonoBehaviour
{
    [SerializeField]
    private LevelDifficult _difficult;
    [SerializeField]
    private int _enemyCount;
    [SerializeField] 
    private bool _isBossEnabled;
    [SerializeField]
    private Boss _endLevelBoss;
    [SerializeField]
    private BossWeakness _bossWeakness;

    private void Start()
    {
        Debug.Log($"diff: {_difficult}, enemyCount: {_enemyCount}, " +
                  $"isBossEnabled: {_isBossEnabled}, boss: {_endLevelBoss}, " +
                  $"weakness: {_bossWeakness}");
    }
}
