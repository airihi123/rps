using UnityEngine;
using DamageNumbersPro;
using DG.Tweening;

public class RpsCharacter : MonoBehaviour
{
    [Header("Animation")]
    public Animator animator;
    
    [Header("VFX")]
    public Transform effectSpawnPoint;
    public GameObject hitEffectPrefab;
    
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip hitSound;
    public AudioClip attackSound;
    
    [Header("Damage Numbers")]
    public DamageNumber damageNumberPrefab;

    [Header("Move Icon Prefabs")]
    [Tooltip("Assign prefabs for each move icon. Each prefab should have a SpriteRenderer or Image.")]
    public GameObject rockIconPrefab;
    public GameObject paperIconPrefab;
    public GameObject scissorsIconPrefab;
    
    [Header("Icon Animation Settings")]
    public float iconDisplayDuration = 2f;
    public float iconAppearDuration = 0.3f;
    public float iconDisappearDuration = 0.5f;
    
    private static readonly int IdleTrigger = Animator.StringToHash("Idle");
    private static readonly int AttackTrigger = Animator.StringToHash("Attack");
    private static readonly int HitTrigger = Animator.StringToHash("Hit");

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
            
        // Auto-link spawn point
        if (effectSpawnPoint == null)
        {
            effectSpawnPoint = transform.Find("EffectSpawnPoint");
        }
    }

    public void PlayIdle()
    {
        if (animator != null)
            animator.SetTrigger(IdleTrigger);
    }

    public void PlayAttack()
    {
        if (animator != null)
            animator.SetTrigger(AttackTrigger);
        
        PlaySound(attackSound);
    }

    public void PlayHit()
    {
        if (animator != null)
            animator.SetTrigger(HitTrigger);
        
        PlaySound(hitSound);
        PlayHitEffect();
    }

    public void ShowDamage(int amount)
    {
        if (damageNumberPrefab == null) return;
        
        Vector3 spawnPos = effectSpawnPoint != null ? effectSpawnPoint.position : transform.position + Vector3.up;
        damageNumberPrefab.Spawn(spawnPos, amount);
    }

    public void ShowText(string text)
    {
        if (damageNumberPrefab == null) return;

        Vector3 spawnPos = effectSpawnPoint != null ? effectSpawnPoint.position : transform.position + Vector3.up;
        damageNumberPrefab.Spawn(spawnPos, text);
    }

    public void ShowMoveIcon(RpsMove move)
    {
        GameObject prefab = null;
        switch (move)
        {
            case RpsMove.Rock: prefab = rockIconPrefab; break;
            case RpsMove.Paper: prefab = paperIconPrefab; break;
            case RpsMove.Scissors: prefab = scissorsIconPrefab; break;
        }

        if (prefab == null)
        {
            Debug.LogWarning($"[RpsCharacter] ShowMoveIcon: prefab for {move} is not assigned!");
            return;
        }

        Vector3 spawnPos = effectSpawnPoint != null ? effectSpawnPoint.position : transform.position;
        spawnPos += Vector3.up * 1.5f; // Above the character

        // Instantiate the prefab
        GameObject iconObj = Instantiate(prefab, spawnPos, Quaternion.identity);
        
        // Appear animation (scale from 0 to 1 with bounce)
        iconObj.transform.localScale = Vector3.zero;
        iconObj.transform.DOScale(Vector3.one, iconAppearDuration).SetEase(Ease.OutBack);
        
        // Disappear animation after delay
        float disappearStartTime = iconDisplayDuration - iconDisappearDuration;
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(disappearStartTime);
        seq.Append(iconObj.transform.DOScale(Vector3.zero, iconDisappearDuration).SetEase(Ease.InBack));
        seq.AppendCallback(() => Destroy(iconObj));
        
        Debug.Log($"[RpsCharacter] ShowMoveIcon spawned: {move} at {spawnPos}");
    }

    public void PlayHitEffect()
    {
        if (hitEffectPrefab == null) return;
        
        Vector3 spawnPos = effectSpawnPoint != null ? effectSpawnPoint.position : transform.position;
        GameObject effect = Instantiate(hitEffectPrefab, spawnPos, Quaternion.identity);
        Destroy(effect, 2f); // Auto-destroy after 2 seconds
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
