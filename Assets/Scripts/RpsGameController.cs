using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RpsGameController : MonoBehaviour
{
    public enum GameState { Matching, Analysis, Playing, Revealing, MatchEnd }

    [Header("Settings")]
    public float countdownTime = 5f;
    public int maxHealth = 30;
    public int damageAmount = 10;
    
    [Header("Current State")]
    public GameState currentState;
    public float timer;
    public RpsMove playerMove = RpsMove.None;
    public RpsMove botMove = RpsMove.None;
    
    public int playerHealth;
    public int botHealth;
    
    [Header("Characters")]
    public RpsCharacter playerCharacter;
    public RpsCharacter botCharacter;
    
    private RpsUIManager _uiManager;

    private void Awake()
    {
        _uiManager = GetComponent<RpsUIManager>();
        
        // Auto-link characters if not assigned
        if (playerCharacter == null)
        {
            GameObject pc = GameObject.Find("PlayerCharacter");
            if (pc != null) playerCharacter = pc.GetComponent<RpsCharacter>();
        }
        if (botCharacter == null)
        {
            GameObject bc = GameObject.Find("BotCharacter");
            if (bc != null) botCharacter = bc.GetComponent<RpsCharacter>();
        }
    }

    private void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        StopAllCoroutines();
        StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop()
    {
        // Initialize Match
        playerHealth = maxHealth;
        botHealth = maxHealth;
        _uiManager.UpdateHealthUI(playerHealth, botHealth, maxHealth);
        
        // Set characters to Idle
        if (playerCharacter != null) playerCharacter.PlayIdle();
        if (botCharacter != null) botCharacter.PlayIdle();

        currentState = GameState.Matching;
        yield return new WaitForSeconds(1f);

        // Loop until someone dies
        while (playerHealth > 0 && botHealth > 0)
        {
            // 1. Analysis (1s for testing, can be 3s)
            currentState = GameState.Analysis;
            
            // Reset characters to Idle at start of each round
            if (playerCharacter != null) playerCharacter.PlayIdle();
            if (botCharacter != null) botCharacter.PlayIdle();

            yield return new WaitForSeconds(1f);

            // 2. Playing (Selection) - 5s
            currentState = GameState.Playing;
            _uiManager.AnimateSelectionPanel(true);
            
            timer = countdownTime;
            playerMove = RpsMove.None;
            botMove = (RpsMove)Random.Range(1, 4);

            _uiManager.ResetRoundUI();

            while (timer > 0)
            {
                timer -= Time.deltaTime;
                _uiManager.UpdateTimer(timer);
                yield return null;
            }

            // Auto-select
            if (playerMove == RpsMove.None)
            {
                playerMove = (RpsMove)Random.Range(1, 4);
                _uiManager.ShowPlayerAutoSelection(playerMove);
            }

            // 3. Revealing - Show icons immediately, then attacks
            currentState = GameState.Revealing;
            _uiManager.AnimateSelectionPanel(false);
            
            // Show move icons on characters FIRST
            if (playerCharacter != null) playerCharacter.ShowMoveIcon(playerMove);
            if (botCharacter != null) botCharacter.ShowMoveIcon(botMove);
            
            _uiManager.ShowReveal(playerMove, botMove);
            yield return new WaitForSeconds(1f); // Brief pause to see icons

            // 4. Round Result (Apply Damage + Animations)
            ApplyRoundResult();
            yield return new WaitForSeconds(2.5f);
        }

        // 5. Match End
        currentState = GameState.MatchEnd;
        bool playerWon = playerHealth > 0;
        _uiManager.ShowFinalResult(playerWon);
    }

    public void SetPlayerMove(RpsMove move)
    {
        if (currentState == GameState.Playing)
        {
            playerMove = move;
            _uiManager.OnMoveSelected(move);
            _uiManager.AnimateSelectionPanel(false); // Hide panel once selected
        }
    }

    private void ApplyRoundResult()
    {
        string resultText = "";
        
        if (playerMove == botMove)
        {
            // Draw - both attack, no damage
            resultText = "Draw! No Damage.";
            if (playerCharacter != null) 
            {
                playerCharacter.PlayAttack();
                playerCharacter.ShowText("MISS");
            }
            if (botCharacter != null) 
            {
                botCharacter.PlayAttack();
                botCharacter.ShowText("MISS");
            }
        }
        else if ((playerMove == RpsMove.Rock && botMove == RpsMove.Scissors) ||
                 (playerMove == RpsMove.Paper && botMove == RpsMove.Rock) ||
                 (playerMove == RpsMove.Scissors && botMove == RpsMove.Paper))
        {
            // Player wins
            resultText = "You Win Round!";
            botHealth -= damageAmount;
            
            if (playerCharacter != null) playerCharacter.PlayAttack();
            if (botCharacter != null)
            {
                botCharacter.PlayHit();
                botCharacter.ShowDamage(damageAmount);
            }
        }
        else
        {
            // Bot wins
            resultText = "You Lose Round!";
            playerHealth -= damageAmount;
            
            if (botCharacter != null) botCharacter.PlayAttack();
            if (playerCharacter != null)
            {
                playerCharacter.PlayHit();
                playerCharacter.ShowDamage(damageAmount);
            }
        }

        playerHealth = Mathf.Max(0, playerHealth);
        botHealth = Mathf.Max(0, botHealth);
        
        _uiManager.UpdateHealthUI(playerHealth, botHealth, maxHealth);
        _uiManager.ShowFinalResult(resultText);
    }
}

