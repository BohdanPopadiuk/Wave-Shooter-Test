using UnityEngine;

public class EnemySlot : MonoBehaviour
{
    [SerializeField] private GameObject positionPreview;
    public GameObject Enemy { get; private set; }

    private void Awake()
    {
        positionPreview.SetActive(false);
    }

    public void PlaceEnemyInSlot(GameObject enemy)
    {
        Enemy = enemy;
        Enemy.transform.position = transform.position;
    }
}
