using DG.Tweening;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour, IClickable
{
    public static Action<EnemyController> DestroyEnemy;
    public static Action<EnemyParameters[]> NewEnemyParameters;
    
    [SerializeField] private EnemyParameters[] enemyParameters;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private ParticleSystem particlesOfDestruction;
    
    [SerializeField] private float scaleAnimDuration = .4f;
    [SerializeField] private Ease scaleEase = Ease.InOutBack;
    [SerializeField] private Color[] colors;
    
    private Color _currentColor;
    private EnemyParameters _currentEnemyParameters;
    private Tween _scaleTween;
    
    private Color GetRandomColor => colors[Random.Range(0, colors.Length)];
    private EnemyParameters GetRandomParameters => enemyParameters[Random.Range(0, enemyParameters.Length)];


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
        _currentEnemyParameters = GetRandomParameters;
        _currentColor = GetRandomColor;

        meshRenderer.materials[0].color = _currentColor;

        Vector3 newScale = _currentEnemyParameters.Scale;
        
        ParticleSystem.MainModule main = particlesOfDestruction.main;
        main.startColor = _currentColor;
        particlesOfDestruction.transform.localScale = newScale;
        
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
        Destroy();
    }
}
