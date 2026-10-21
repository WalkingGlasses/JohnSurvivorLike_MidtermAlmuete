using UnityEngine;

public class MeleeDuplicateWeapon : MonoBehaviour
{
    [Header("Strike Prefab")]
    public MeleeDuplicateStrike strikePrefab;

    [Header("Weapon Rank")]
    [Range(1, 10)]
    public int weaponRank = 1;

    [Header("Attack Settings")]
    public int startingStrikes = 6;
    public float fireInterval = 1.2f;
    public float baseDamage = 10f;
    public float damagePerRank = 1f;

    private float fireTimer;

    private void Update()
    {
        if (strikePrefab == null)
            return;

        fireTimer -= Time.deltaTime;

        if (fireTimer <= 0f)
        {
            FireWave();
            fireTimer = fireInterval;
        }
    }

    private void FireWave()
    {
        int strikeCount = startingStrikes + weaponRank - 1;

        float damage =
            baseDamage + (weaponRank - 1) * damagePerRank;

        for (int i = 0; i < strikeCount; i++)
        {
            float angle = i * (360f / strikeCount);
            float radians = angle * Mathf.Deg2Rad;

            Vector2 direction = new Vector2(
                Mathf.Cos(radians),
                Mathf.Sin(radians)
            );

            MeleeDuplicateStrike strike = Instantiate(
                strikePrefab,
                transform.position,
                Quaternion.identity
            );

            strike.Initialize(direction, damage);
        }
    }

    public void SetRank(int newRank)
    {
        weaponRank = Mathf.Clamp(newRank, 1, 10);
    }
}