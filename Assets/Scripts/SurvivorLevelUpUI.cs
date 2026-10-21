
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class SurvivorLevelUpUI : MonoBehaviour
{
    [System.Serializable]
    public class UpgradeChoice
    {
        public SurvivorProgression.UpgradeType type;
        public string title;

        [TextArea]
        public string description;
    }

    [Header("UI References")]
    public GameObject panel;
    public Button[] choiceButtons = new Button[3];
    public TextMeshProUGUI[] choiceLabels = new TextMeshProUGUI[3];

    [Header("Upgrade Choices")]
    public UpgradeChoice[] upgrades =
    {
        new UpgradeChoice
        {
            type = SurvivorProgression.UpgradeType.MeleeDuplicate,
            title = "Melee Duplicate",
            description = "Add one more strike to each melee wave."
        },
        new UpgradeChoice
        {
            type = SurvivorProgression.UpgradeType.RadialJavelin,
            title = "Radial Javelin",
            description = "Increase javelin damage by 2."
        },
        new UpgradeChoice
        {
            type = SurvivorProgression.UpgradeType.Vitality,
            title = "Vitality",
            description = "Increase maximum health by 50 HP."
        }
    };

    private SurvivorProgression progression;

    private readonly List<UpgradeChoice> currentChoices =
        new List<UpgradeChoice>();

    private void Awake()
    {
        if (panel != null)
            panel.SetActive(false);

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (choiceButtons[i] == null)
                continue;

            int buttonIndex = i;

            choiceButtons[i].onClick.AddListener(
                () => SelectChoice(buttonIndex)
            );
        }
    }

    public void ShowChoices(SurvivorProgression source)
    {
        progression = source;
        currentChoices.Clear();

        foreach (UpgradeChoice upgrade in upgrades)
        {
            if (upgrade != null && IsUpgradeAvailable(upgrade.type))
                currentChoices.Add(upgrade);
        }

        // Randomize the order of the available choices.
        for (int i = currentChoices.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            UpgradeChoice temporary = currentChoices[i];
            currentChoices[i] = currentChoices[randomIndex];
            currentChoices[randomIndex] = temporary;
        }

        if (panel != null)
            panel.SetActive(true);

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            bool hasChoice = i < currentChoices.Count;

            if (choiceButtons[i] != null)
            {
                choiceButtons[i].gameObject.SetActive(hasChoice);
                choiceButtons[i].interactable = hasChoice;
            }

            if (hasChoice &&
                i < choiceLabels.Length &&
                choiceLabels[i] != null)
            {
                UpgradeChoice choice = currentChoices[i];

                choiceLabels[i].text =
                    choice.title + "\n" + choice.description;
            }
        }

        if (currentChoices.Count == 0)
        {
            Debug.LogWarning("No upgrades are currently available.");
            HideChoices();
            Time.timeScale = 1f;
        }
    }

    private bool IsUpgradeAvailable(
        SurvivorProgression.UpgradeType type)
    {
        if (progression == null)
            return false;

        switch (type)
        {
            case SurvivorProgression.UpgradeType.MeleeDuplicate:
                return progression.meleeWeapon != null &&
                       progression.meleeWeapon.weaponRank < 10;

            case SurvivorProgression.UpgradeType.RadialJavelin:
                return progression.javelinWeapon != null &&
                       progression.javelinWeapon.weaponRank < 10;

            case SurvivorProgression.UpgradeType.Vitality:
                return progression.vitalityPower != null &&
                       progression.vitalityPower.vitalityRank < 10;
        }

        return false;
    }

    private void SelectChoice(int index)
    {
        if (progression == null ||
            index < 0 ||
            index >= currentChoices.Count)
            return;

        progression.SelectUpgrade(currentChoices[index].type);
    }

    public void HideChoices()
    {
        if (panel != null)
            panel.SetActive(false);
    }
}
