using UnityEngine;
using TMPro;

public class SurvivorPlayerHPDisplay : MonoBehaviour
{
    public SurvivorPlayerHealth playerHealth;
    public TextMeshProUGUI hpText;

    private void OnEnable()
    {
        if (playerHealth != null)
            playerHealth.HealthChanged += UpdateHPDisplay;
    }

    private void Start()
    {
        if (playerHealth != null)
        {
            UpdateHPDisplay(
                playerHealth.currentHealth,
                playerHealth.maxHealth
            );
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
            playerHealth.HealthChanged -= UpdateHPDisplay;
    }

    private void UpdateHPDisplay(float currentHealth, float maxHealth)
    {
        if (hpText == null)
            return;

        hpText.text =
            "HP: " +
            Mathf.CeilToInt(currentHealth) + " / " +
            Mathf.CeilToInt(maxHealth);
    }
}