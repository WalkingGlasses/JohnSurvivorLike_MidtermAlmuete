using UnityEngine;

public class SurvivorJavelinProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 9f;
    public float lifetime = 3f;
    public float hitRadius = 0.25f;

    private Vector2 direction;
    private float damage;
    private float lifeTimer;
    private bool initialized;

    public void Initialize(Vector2 travelDirection, float projectileDamage)
    {
        direction = travelDirection.normalized;
        damage = projectileDamage;
        lifeTimer = lifetime;
        initialized = true;
    }

    private void Update()
    {
        if (!initialized)
            return;

        transform.position +=
            (Vector3)(direction * speed * Time.deltaTime);

        lifeTimer -= Time.deltaTime;

        if (lifeTimer <= 0f)
        {
            Destroy(gameObject);
            return;
        }

        for (int i = SurvivorEnemy.ActiveEnemies.Count - 1; i >= 0; i--)
        {
            SurvivorEnemy enemy = SurvivorEnemy.ActiveEnemies[i];

            if (enemy == null || !enemy.gameObject.activeInHierarchy)
                continue;

            Vector2 difference =
                (Vector2)(enemy.transform.position - transform.position);

            float combinedRadius = hitRadius + enemy.hitRadius;

            if (difference.sqrMagnitude <= combinedRadius * combinedRadius)
            {
                enemy.TakeDamage(damage);
                Destroy(gameObject);
                return;
            }
        }
    }
}