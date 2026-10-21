using UnityEngine;
using TMPro;

public class SurvivorXPDisplay : MonoBehaviour
{
    [Header("References")]
    public SurvivorProgression progression;
    public TextMeshProUGUI xpText;

    private void OnEnable()
    {
        if (progression != null)
            progression.XPChanged += UpdateDisplay;
    }

    private void Start()
    {
        if (progression != null)
        {
            UpdateDisplay(
                progression.currentXP,
                progression.xpToNextLevel,
                progression.playerLevel
            );
        }
    }

    private void OnDisable()
    {
        if (progression != null)
            progression.XPChanged -= UpdateDisplay;
    }

    private void UpdateDisplay(
        int currentXP,
        int requiredXP,
        int level)
    {
        if (xpText == null)
            return;

        xpText.text =
            "Level " + level +
            "\nXP: " + currentXP + " / " + requiredXP;
    }
}