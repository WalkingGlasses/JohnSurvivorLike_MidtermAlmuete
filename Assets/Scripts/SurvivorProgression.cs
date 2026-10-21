
using UnityEngine;
using System;

public class SurvivorProgression : MonoBehaviour
{
    public enum UpgradeType
    {
        MeleeDuplicate,
        RadialJavelin,
        Vitality
    }

    [Header("Level Progression")]
    public int playerLevel = 1;
    public int currentXP = 0;
    public int xpToNextLevel = 10;
    [Range(1f, 100f)]
    public float xpGrowthPercent = 20f;

    [Header("Upgrade References")]
    public MeleeDuplicateWeapon meleeWeapon;
    public RadialJavelinWeapon javelinWeapon;
    public VitalityPower vitalityPower;

    [Header("UI")]
    public SurvivorLevelUpUI levelUpUI;

    public event Action<int, int, int> XPChanged;

    private bool choosingUpgrade;

    private void Awake()
    {
        Time.timeScale = 1f;

        if (meleeWeapon == null)
            meleeWeapon = GetComponent<MeleeDuplicateWeapon>();

        if (javelinWeapon == null)
            javelinWeapon = GetComponent<RadialJavelinWeapon>();

        if (vitalityPower == null)
            vitalityPower = GetComponent<VitalityPower>();
    }

    private void Start()
    {
        XPChanged?.Invoke(currentXP, xpToNextLevel, playerLevel);
    }

    public void AddXP(int amount)
    {
        if (amount <= 0)
            return;

        currentXP += amount;

        XPChanged?.Invoke(currentXP, xpToNextLevel, playerLevel);

        if (!choosingUpgrade)
            CheckForLevelUp();
    }

    private void CheckForLevelUp()
    {
        if (choosingUpgrade || currentXP < xpToNextLevel)
            return;

        currentXP -= xpToNextLevel;
        playerLevel++;

        xpToNextLevel = Mathf.CeilToInt(
            xpToNextLevel * (1f + xpGrowthPercent / 100f)
        );

        XPChanged?.Invoke(currentXP, xpToNextLevel, playerLevel);

        choosingUpgrade = true;
        Time.timeScale = 0f;

        if (levelUpUI == null)
        {
            Debug.LogError(
                "SurvivorProgression needs a SurvivorLevelUpUI reference.",
                this
            );

            choosingUpgrade = false;
            Time.timeScale = 1f;
            return;
        }

        levelUpUI.ShowChoices(this);
    }

    public void SelectUpgrade(UpgradeType upgrade)
    {
        if (!choosingUpgrade)
            return;

        switch (upgrade)
        {
            case UpgradeType.MeleeDuplicate:
                if (meleeWeapon != null)
                    meleeWeapon.SetRank(meleeWeapon.weaponRank + 1);
                break;

            case UpgradeType.RadialJavelin:
                if (javelinWeapon != null)
                    javelinWeapon.SetRank(javelinWeapon.weaponRank + 1);
                break;

            case UpgradeType.Vitality:
                if (vitalityPower != null)
                    vitalityPower.UpgradeVitality();
                break;
        }

        choosingUpgrade = false;

        if (levelUpUI != null)
            levelUpUI.HideChoices();

        // if the xp gave you a TONNA levels, then play all the next levels ups before you go back to gameplay in case
        if (currentXP >= xpToNextLevel)
        {
            CheckForLevelUp();
        }
        else
        {
            Time.timeScale = 1f;
        }
    }
}
