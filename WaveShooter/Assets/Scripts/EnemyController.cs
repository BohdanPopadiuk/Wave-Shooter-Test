using DG.Tweening;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour, IClickable
{
    public static Action<EnemyController> DestroyEnemy;
    public static Action<EnemyParameters[]> NewEnemyParameters;

    [SerializeField] private Collider col;
    [SerializeField] private EnemyParameters[] enemyParameters;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private ParticleSystem particlesOfDestruction;
    
    [SerializeField] private float scaleAnimDuration = .4f;
    [SerializeField] private Ease scaleEase = Ease.InOutCubic;
    [SerializeField] private Color[] colors;

    private ParticleSystem.MainModule _particleModule;
    private Color _currentColor;
    private EnemyParameters _currentEnemyParameters;
    private Tween _scaleTween;

    private Color GetRandomColor => colors[Random.Range(0, colors.Length)];
    private EnemyParameters GetRandomParameters => enemyParameters[Random.Range(0, enemyParameters.Length)];

    private void Awake()
    {
        _particleModule = particlesOfDestruction.GetComponent<ParticleSystem>().main;
    }

    private void OnEnable()
    {
        NewEnemyParameters += UpdateEnemyParameters;
        
        CreateNewEnemy();
    }

    private void OnDisable()
    {
        NewEnemyParameters -= UpdateEnemyParameters;
        
        if(_scaleTween != null) _scaleTween.Kill();
    }

    private void UpdateEnemyParameters(EnemyParameters[] enemyParameters)
    {
        this.enemyParameters = enemyParameters;
    }

    private void CreateNewEnemy()
    {
        col.enabled = true;
        _currentEnemyParameters = GetRandomParameters;
        _currentColor = GetRandomColor;

        meshRenderer.materials[0].color = _currentColor;

        Vector3 newScale = _currentEnemyParameters.Scale;

        _particleModule.startColor = new ParticleSystem.MinMaxGradient( _currentColor );
        particlesOfDestruction.transform.localScale = newScale * 1.5f;
        
        transform.localScale = Vector3.zero;
        _scaleTween = transform
            .DOScale(newScale, scaleAnimDuration)
            .SetEase(scaleEase);

    }

    private void Destroy()
    {
        if(_scaleTween != null) _scaleTween.Kill();
        
        particlesOfDestruction.Play();
        
        _scaleTween = transform
            .DOScale(Vector3.zero, scaleAnimDuration)
            .SetEase(scaleEase)
            .OnComplete(() =>
            {
                DestroyEnemy?.Invoke(this);
            });
    }

    public void OnMouseClick()
    {
        col.enabled = false;
        Destroy();
    }
}
