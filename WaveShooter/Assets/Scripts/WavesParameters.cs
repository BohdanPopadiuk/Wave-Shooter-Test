using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Parameters/WavesParameters")]
public class WavesParameters: ScriptableObject
{
    [field: SerializeField] public Wave[] Waves { get; private set; }
}

[Serializable]
public struct Wave
{
    [field: SerializeField] public int Threshold { get; private set; }
    [field: SerializeField] public float SpawnTime { get; private set; }
    [field: SerializeField] public EnemyParameters[] EnemiesParameters { get; private set; }
}