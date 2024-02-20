using UnityEngine;

[CreateAssetMenu(menuName = "Parameters/EnemyParameters")]
public class EnemyParameters : ScriptableObject
{
    [field: SerializeField] public Vector3 Scale { get; private set; } = Vector3.one;
    [field: SerializeField, Min(1)] public int Reward { get; private set; }
}
