using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class RpsUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI playerChoiceText;
    public TextMeshProUGUI botChoiceText;
    
    [Header("Buttons")]
    public Button rockButton;
    public Button paperButton;
    public Button scissorsButton;
    public Button restartButton;

    [Header("Panels")]
    public RectTransform selectionPanel;
    public RectTransform resultPanel;

    [Header("Health UI")]
    public Image playerHpBar;
    public Image botHpBar;
    public TextMeshProUGUI playerHpText;
    public TextMeshProUGUI botHpText;

    private RpsGameController _gameController;

    private void Awake()
    {
        _gameController = GetComponent<RpsGameController>();
        FindReferences();
        
        if (rockButton != null) rockButton.onClick.AddListener(() => _gameController.SetPlayerMove(RpsMove.Rock));
        if (paperButton != null) paperButton.onClick.AddListener(() => _gameController.SetPlayerMove(RpsMove.Paper));
        if (scissorsButton != null) scissorsButton.onClick.AddListener(() => _gameController.SetPlayerMove(RpsMove.Scissors));
        
        if (restartButton != null)
            restartButton.onClick.AddListener(() => _gameController.StartGame());
            
        ResetUI();
    }

    private void FindReferences()
    {
        // Auto-find components if not set
        if (statusText == null) statusText = FindInCanvas<TextMeshProUGUI>("StatusText");
        if (timerText == null) timerText = FindInCanvas<TextMeshProUGUI>("TimerText");
        if (resultText == null) resultText = FindInCanvas<TextMeshProUGUI>("ResultText");
        if (playerChoiceText == null) playerChoiceText = FindInCanvas<TextMeshProUGUI>("PlayerChoiceText");
        if (botChoiceText == null) botChoiceText = FindInCanvas<TextMeshProUGUI>("BotChoiceText");
        
        if (rockButton == null) rockButton = FindInCanvas<Button>("RockBtn");
        if (paperButton == null) paperButton = FindInCanvas<Button>("PaperBtn");
        if (scissorsButton == null) scissorsButton = FindInCanvas<Button>("ScissorsBtn");
        if (restartButton == null) restartButton = FindInCanvas<Button>("RestartBtn");

        if (playerHpBar == null) playerHpBar = FindInCanvas<Image>("PlayerHpBar");
        if (botHpBar == null) botHpBar = FindInCanvas<Image>("BotHpBar");
        if (playerHpText == null) playerHpText = FindInCanvas<TextMeshProUGUI>("PlayerHpText");
        if (botHpText == null) botHpText = FindInCanvas<TextMeshProUGUI>("BotHpText");

        if (selectionPanel == null)
        {
            GameObject sp = GameObject.Find("SelectionPanel");
            if (sp != null)
            {
                selectionPanel = sp.GetComponent<RectTransform>();
                Debug.Log($"[RpsUI] SelectionPanel found: {selectionPanel != null}");
            }
            else
            {
                Debug.LogWarning("[RpsUI] SelectionPanel NOT found in scene!");
            }
        }

        if (resultPanel == null)
        {
            GameObject rp = GameObject.Find("ResultPanel");
            if (rp != null)
            {
                resultPanel = rp.GetComponent<RectTransform>();
                Debug.Log($"[RpsUI] ResultPanel found: {resultPanel != null}");
            }
        }
    }

    private T FindInCanvas<T>(string name) where T : Component
    {
        GameObject obj = GameObject.Find(name);
        if (obj != null) return obj.GetComponent<T>();
        return null;
    }

    public void UpdateTimer(float time)
    {
        timerText.text = Mathf.CeilToInt(time).ToString();
        statusText.text = "SELECT YOUR MOVE!";
    }

    public void OnMoveSelected(RpsMove move)
    {
        playerChoiceText.text = "Selected: " + move.ToString();
        // Visual feedback for selection
    }

    public void ShowPlayerAutoSelection(RpsMove move)
    {
        playerChoiceText.text = "Auto-Selected: " + move.ToString();
    }

    public void ShowReveal(RpsMove player, RpsMove bot)
    {
        statusText.text = "REVEAL!";
        playerChoiceText.text = "Player: " + player.ToString();
        botChoiceText.text = "Bot: " + bot.ToString();
    }

    public void ShowFinalResult(bool playerWon)
    {
        statusText.text = "MATCH END";

        // 한국어 텍스트 설정
        resultText.text = playerWon ? "승" : "패";

        // 승/패에 따른 색상 변경
        resultText.color = playerWon ?
            new Color(1f, 0.84f, 0f) :  // 승리: 금색
            new Color(0.8f, 0.2f, 0.2f); // 패배: 빨간색

        // 결과 패널 애니메이션
        if (resultPanel != null)
        {
            resultPanel.gameObject.SetActive(true);

            // Fade-in + Scale-up 애니메이션
            resultPanel.localScale = Vector3.zero;
            resultPanel.DOScale(Vector3.one, 0.5f)
                .SetEase(Ease.OutBack);

            CanvasGroup cg = resultPanel.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.alpha = 0f;
                cg.DOFade(1f, 0.3f);
            }
        }

        // 다시하기 버튼 지연 표시
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.5f);
        seq.AppendCallback(() =>
        {
            restartButton.gameObject.SetActive(true);
            restartButton.transform.DOScale(Vector3.one, 0.3f)
                .SetEase(Ease.OutBack)
                .From(Vector3.zero);
        });
    }

    // 기존 코드와의 호환성을 위한 오버로드
    public void ShowFinalResult(string result)
    {
        statusText.text = "MATCH END";
        resultText.text = result;
        restartButton.gameObject.SetActive(true);
    }
    
    public void UpdateHealthUI(int playerHp, int botHp, int maxHp)
    {
        if (playerHpBar != null)
        {
            playerHpBar.fillAmount = (float)playerHp / maxHp;
        }
        if (botHpBar != null)
        {
            botHpBar.fillAmount = (float)botHp / maxHp;
        }
        
        if (playerHpText != null) playerHpText.text = $"{playerHp}/{maxHp}";
        if (botHpText != null) botHpText.text = $"{botHp}/{maxHp}";
    }

    public void ResetRoundUI()
    {
        playerChoiceText.text = "Selecting...";
        botChoiceText.text = "Selecting...";
    }
    
    public void ResetUI()
    {
        resultText.text = "";
        playerChoiceText.text = "Waiting...";
        botChoiceText.text = "Waiting...";
        restartButton.gameObject.SetActive(false);

        // 결과 패널 숨김
        if (resultPanel != null)
        {
            resultPanel.gameObject.SetActive(false);
        }

        UpdateHealthUI(30, 30, 30); // Reset bars to full

        if (selectionPanel != null)
        {
            selectionPanel.anchoredPosition = new Vector2(0, -500f);
        }
    }

    public void AnimateSelectionPanel(bool show)
    {
        Debug.Log($"[RpsUI] AnimateSelectionPanel({show}), selectionPanel: {selectionPanel != null}");
        if (selectionPanel == null) return;

        float targetY = show ? 0f : -500f;
        Debug.Log($"[RpsUI] Tweeting to Y: {targetY}");
        selectionPanel.DOAnchorPosY(targetY, 0.5f).SetEase(Ease.OutBack);
    }
}
