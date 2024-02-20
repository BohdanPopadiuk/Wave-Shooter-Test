using System.Collections.Generic;
using System;
using ObjectPool;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static Action RestartGame;
    public static Action GameOverAction;
    public static Action<int, float> UpdateWaveProgress;//score && wave progress
    
    [SerializeField] private WavesParameters wavesParameters;
    [SerializeField] private EnemyController enemyPrefab;
    [SerializeField] private Transform enemySlotsParent;
    private readonly Queue<EnemySlot> _enemySpawnQueue = new Queue<EnemySlot>();
    private GameObjectPool _enemyPool;

    private List<GameObject> _activeEnemies = new List<GameObject>();
    private EnemySlot[] _enemySlots;
    private bool _spawnerBlocked;
    private int _currentWave;
    private int _currentWaveScore;
    private float _timer;
    private int _scoreDelta;
    
    public int Score { get; private set; }
    private int Threshold => wavesParameters.Waves[_currentWave].Threshold;
    private int LastWave => wavesParameters.Waves.Length - 1;
    private float SpawnTime => wavesParameters.Waves[_currentWave].SpawnTime;
    private float WaveProgress => (float)_currentWaveScore / _scoreDelta;//0-1
    private EnemyParameters[] GetEnemyParameters => wavesParameters.Waves[_currentWave].EnemiesParameters;
    
    private void Start()
    {
        EnemyController.DestroyEnemy += DestroyEnemy;
        RestartGame += StartNewGame;
        
        _enemySlots = enemySlotsParent.GetComponentsInChildren<EnemySlot>();
        _enemyPool = new GameObjectPool(enemyPrefab.gameObject, 10);

        StartNewGame();
    }

    private void Update()
    {
        EnemySpawner();
    }

    private void OnDestroy()
    {
        EnemyController.DestroyEnemy -= DestroyEnemy;
        RestartGame -= StartNewGame;
    }

    private void StartNewGame()
    {
        Debug.Log("StartNewGame");
        while (_activeEnemies.Count > 0)
        {
            GameObject enemy = _activeEnemies[0];
            _activeEnemies.Remove(enemy);
            _enemyPool.Return(enemy);
        }

        _enemySpawnQueue.Clear();
        foreach (EnemySlot slot in _enemySlots)
        {
            _enemySpawnQueue.Enqueue(slot);
        }

        Score = 0;
        _currentWave = 0;
        _timer = SpawnTime;
        _scoreDelta = Threshold;
        
        _spawnerBlocked = false;

        UpdateWaveProgress?.Invoke(Score, WaveProgress);
        SetCurrentWave();
    }

    private void DestroyEnemy(EnemyController enemy)
    {
        _activeEnemies.Remove(enemy.gameObject);
        _enemyPool.Return(enemy.gameObject);
        foreach (EnemySlot slot in _enemySlots)
        {
            if (enemy.gameObject == slot.Enemy)
                _enemySpawnQueue.Enqueue(slot);
        }

        Score++;
        _currentWaveScore++;

        UpdateWaveProgress?.Invoke(Score, WaveProgress);
        if (Score >= Threshold)
        {
            NextWave();
        }
    }

    private void EnemySpawner()
    {
        if(_spawnerBlocked) return;
        
        _timer -= Time.deltaTime;
        
        if (_timer <= 0)
        {
            if (_enemySpawnQueue.Count > 0)
            {
                _timer = SpawnTime;
                SpawnNewEnemy();
            }
            else
            {
                GameOver();
            }
        }
    }

    private void SpawnNewEnemy()
    {
        GameObject enemy = _enemyPool.Get();
        EnemySlot slot = _enemySpawnQueue.Dequeue();
        slot.PlaceEnemyInSlot(enemy);
        _activeEnemies.Add(enemy);
    }

    private void GameOver()
    {
        Debug.Log("GameOver");
        _spawnerBlocked = true;
        GameOverAction?.Invoke();
    }

    private void NextWave()
    {
        if (_currentWave != LastWave)
        {
            Debug.Log("NextWave");
            _currentWave++;
            SetCurrentWave();
        }
    }

    private void SetCurrentWave()
    {
        _currentWaveScore = 0;
        _scoreDelta = Threshold - Score;
        EnemyController.NewEnemyParameters?.Invoke(GetEnemyParameters);
    }
}
