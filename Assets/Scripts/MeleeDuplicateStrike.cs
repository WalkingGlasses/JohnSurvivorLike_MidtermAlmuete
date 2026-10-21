using UnityEngine;

public class MeleeDuplicateStrike : MonoBehaviour
{
    [Header("Strike Settings")]
    public float speed = 7f;
    public float lifetime = 0.35f;
    public float hitRadius = 0.45f;

    private Vector2 direction;
    private float damage;
    private float lifeTimer;
    private bool initialized;

    public void Initialize(Vector2 strikeDirection, float strikeDamage)
    {
        direction = strikeDirection.normalized;
        damage = strikeDamage;
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