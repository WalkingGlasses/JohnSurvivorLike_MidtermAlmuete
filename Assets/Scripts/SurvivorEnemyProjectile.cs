using UnityEngine;

public class SurvivorEnemyProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 5f;
    public float lifetime = 5f;
    public float hitRadius = 0.25f;

    private Vector2 direction;
    private Transform player;
    private SurvivorPlayerHealth playerHealth;
    private float damage;
    private float lifeTimer;
    private bool initialized;

    public void Initialize(
        Transform target,
        Vector2 travelDirection,
        float projectileDamage,
        float projectileSpeed)
    {
        player = target;
        direction = travelDirection.normalized;
        damage = projectileDamage;
        speed = projectileSpeed;
        lifeTimer = lifetime;

        playerHealth = player != null
            ? player.GetComponent<SurvivorPlayerHealth>()
            : null;

        initialized = player != null && direction.sqrMagnitude > 0f;
    }

    private void Update()
    {
        if (!initialized)
            return;

        if (player == null || playerHealth == null ||
            playerHealth.IsDead)
        {
            Destroy(gameObject);
            return;
        }

        transform.position +=
            (Vector3)(direction * speed * Time.deltaTime);

        lifeTimer -= Time.deltaTime;

        if (lifeTimer <= 0f)
        {
            Destroy(gameObject);
            return;
        }

        Vector2 difference =
            (Vector2)(player.position - transform.position);

        float combinedRadius = hitRadius + 0.4f;

        if (difference.sqrMagnitude <=
            combinedRadius * combinedRadius)
        {
            playerHealth.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}