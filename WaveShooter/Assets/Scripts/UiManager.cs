using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [Header("Game UI")]
    [SerializeField] private Button exitButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private TextMeshProUGUI scoreText;
    
    [SerializeField] private Image progressBar;

    [Header("Game Over UI")]
    [SerializeField] private float showPanelAnimDuration = .5f;
    [SerializeField] private CanvasGroup gameOverPanel;
    
    [SerializeField] private Button gameOverRestartButton;
    [SerializeField] private Button gameOverExitButton;
    [SerializeField] private TextMeshProUGUI gameOverScoreText;
    

    private void Start()
    {
        GameManager.UpdateWaveProgress += UpdateWaveProgress;
        GameManager.GameOverAction += GameOver;
        
        exitButton.onClick.AddListener(Exit);
        gameOverExitButton.onClick.AddListener(Exit);
            
        restartButton.onClick.AddListener(RestartGame);
        gameOverRestartButton.onClick.AddListener(GameOverRestart);
    }

    private void OnDestroy()
    {
        GameManager.UpdateWaveProgress -= UpdateWaveProgress;
        GameManager.GameOverAction -= GameOver;
    }
    
    private void UpdateWaveProgress(int score, float progress)
    {
        string textScore = $"SCORE: {score}";
        
        scoreText.text = textScore;
        gameOverScoreText.text = textScore;
        progressBar.fillAmount = progress;
    }

    private void Exit()
    {
        Application.Quit();
    }

    private void RestartGame()
    {
        GameManager.RestartGame?.Invoke();
    }
    
    private void GameOverRestart()
    {
        GameManager.RestartGame?.Invoke();
        HidePanel(gameOverPanel);
    }

    private void GameOver()
    {
        ShowPanel(gameOverPanel);
    }
    
    private void ShowPanel(CanvasGroup panel)
    {
        panel.blocksRaycasts = true;
        panel.DOFade(1, showPanelAnimDuration)
            .OnComplete(() =>
            {
                panel.interactable = true;
            });
    }

    private void HidePanel(CanvasGroup panel)
    {
        panel.interactable = false;
        panel.DOFade(0, showPanelAnimDuration)
            .OnComplete(() =>
            {
                panel.blocksRaycasts = false;
            });
    }
}
