
using UnityEngine;
using UnityEngine.InputSystem;

public class VitalityPower : MonoBehaviour
{
    [Header("Vitality")]
    [Range(1, 10)]
    public int vitalityRank = 1;

    public float startingMaxHealth = 350f;
    public float healthPerRank = 50f;

    [Header("Mending Soul Ultimate")]
    public Key ultimateKey = Key.Q;
    public float healingPerSecond = 20f;
    public float healingDuration = 5f;
    public float ultimateCooldown = 120f;

    private SurvivorPlayerHealth playerHealth;

    private float healingTimer;
    private float cooldownTimer;
    private bool isHealing;

    public bool IsHealing => isHealing;
    public float CooldownRemaining => Mathf.Max(0f, cooldownTimer);

    private void Awake()
    {
        playerHealth = GetComponent<SurvivorPlayerHealth>();

        if (playerHealth == null)
        {
            Debug.LogError(
                "VitalityPower requires SurvivorPlayerHealth on the same Player.",
                this
            );

            enabled = false;
        }
    }

    private void Start()
    {
        // Synchs Vitality power with actual health and shit, supposedly but life might be gaslighting me
        float desiredMaxHealth =
            startingMaxHealth + (vitalityRank - 1) * healthPerRank;

        playerHealth.maxHealth = desiredMaxHealth;
        playerHealth.currentHealth = Mathf.Min(
            playerHealth.currentHealth,
            desiredMaxHealth
        );

        playerHealth.ResetHealth();
    }

    private void Update()
    {
        if (playerHealth == null || playerHealth.IsDead)
        {
            isHealing = false;
            return;
        }

        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;

        if (Keyboard.current != null &&
            Keyboard.current[ultimateKey].wasPressedThisFrame &&
            cooldownTimer <= 0f &&
            !isHealing)
        {
            ActivateMendingSoul();
        }

        if (isHealing)
        {
            float healAmount = healingPerSecond * Time.deltaTime;
            playerHealth.Heal(healAmount);

            healingTimer -= Time.deltaTime;

            if (healingTimer <= 0f)
                isHealing = false;
        }
    }

    public void UpgradeVitality()
    {
        if (vitalityRank >= 10)
        {
            Debug.Log("Vitality is already at maximum rank.");
            return;
        }

        vitalityRank++;

        playerHealth.IncreaseMaxHealth(healthPerRank);

        Debug.Log(
            "Vitality Rank " + vitalityRank +
            " | Max HP: " + playerHealth.maxHealth
        );
    }

    public void ActivateMendingSoul()
    {
        if (playerHealth == null ||
            playerHealth.IsDead ||
            isHealing ||
            cooldownTimer > 0f)
        {
            return;
        }

        isHealing = true;
        healingTimer = healingDuration;
        cooldownTimer = ultimateCooldown;

        Debug.Log("Mending Soul activated!");
    }
}
