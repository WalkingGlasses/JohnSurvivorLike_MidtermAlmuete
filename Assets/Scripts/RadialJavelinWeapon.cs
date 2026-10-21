using UnityEngine;

public class RadialJavelinWeapon : MonoBehaviour
{
    [Header("Projectile")]
    public SurvivorJavelinProjectile projectilePrefab;

    [Header("Weapon Stats")]
    [Range(1, 10)]
    public int weaponRank = 1;

    public int projectilesPerVolley = 8;
    public float fireInterval = 1.5f;
    public float baseDamage = 12f;
    public float damagePerRank = 2f;

    private float fireTimer;

    private void Update()
    {
        if (projectilePrefab == null)
            return;

        fireTimer -= Time.deltaTime;

        if (fireTimer <= 0f)
        {
            FireVolley();
            fireTimer = fireInterval;
        }
    }

    private void FireVolley()
    {
        float damage =
            baseDamage + (weaponRank - 1) * damagePerRank;

        int count = Mathf.Max(1, projectilesPerVolley);

        for (int i = 0; i < count; i++)
        {
            float angle = i * (360f / count);
            float radians = angle * Mathf.Deg2Rad;

            Vector2 direction = new Vector2(
                Mathf.Cos(radians),
                Mathf.Sin(radians)
            );

            SurvivorJavelinProjectile projectile = Instantiate(
                projectilePrefab,
                transform.position,
                Quaternion.identity
            );

            projectile.Initialize(direction, damage);
        }
    }

    public void SetRank(int newRank)
    {
        weaponRank = Mathf.Clamp(newRank, 1, 10);
    }
}