
using UnityEngine;

public class SurvivorEnemy : MonoBehaviour
{
    public enum EnemyType
    {
        Butcher,
        ManicBombard,
        Roller,
        EliteLancer,
        CaptainVor
    }
    
    public static readonly System.Collections.Generic.List<SurvivorEnemy>
        ActiveEnemies = new System.Collections.Generic.List<SurvivorEnemy>();

    public float hitRadius = 0.4f;

    [Header("Enemy Type")]
    public EnemyType enemyType = EnemyType.Butcher;
    
    [Header("XP Drop")]
    public SurvivorXPGem xpGemPrefab;

    [Header("Base Stats")]
    public float baseHealth = 30f;
    public float baseSpeed = 2f;
    public float contactDamage = 10f;

    [Header("Combat")]
    public float attackRange = 1.2f;
    public float attackCooldown = 1f;
    public float bomberRadius = 2.5f;
    public float bomberDamage = 25f;

    [Header("Lancer")]
    public float rangedAttackRange = 6f;

    [Header("Difficulty Scaling")]
    public float healthIncreasePerRank = 0.08f;
    public float speedIncreasePerRank = 0.01f;

    [Header("Hit Feedback")]
    public float hitFlashDuration = 0.08f;
    
    [Header("Elite Lancer Projectile")]
    public SurvivorEnemyProjectile projectilePrefab;
    public float rangedProjectileDamage = 12f;
    public float rangedProjectileSpeed = 5f;
    public float rangedProjectileCooldown = 2f;
    
    [Header("Rewards")]
    public int xpReward = 3;

    private SurvivorProgression playerProgression;

    private Transform player;
    private SurvivorPlayerHealth playerHealth;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private float currentHealth;
    private float moveSpeed;
    private float attackTimer;
    private float hitFlashTimer;

    private int playerRank = 1;
    private bool isDead;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    public void Initialize(
        Transform target,
        int rank,
        float healthMultiplier = 1f,
        float speedMultiplier = 1f)
    {
        player = target;

        playerHealth = player != null
            ? player.GetComponent<SurvivorPlayerHealth>()
            : null;
        playerProgression = player != null
            ? player.GetComponent<SurvivorProgression>()
            : null;

        playerRank = Mathf.Clamp(rank, 1, 27);

        float rankHealthScale =
            1f + healthIncreasePerRank * (playerRank - 1);

        float rankSpeedScale =
            1f + speedIncreasePerRank * (playerRank - 1);

        currentHealth = baseHealth * rankHealthScale * healthMultiplier;
        moveSpeed = baseSpeed * rankSpeedScale * speedMultiplier;

        attackTimer = Random.Range(0f, attackCooldown);
        isDead = false;
    }

    private void Update()
    {
        if (isDead || player == null)
            return;

        if (playerHealth == null)
            playerHealth = player.GetComponent<SurvivorPlayerHealth>();

        if (playerHealth == null || playerHealth.IsDead)
            return;

        if (hitFlashTimer > 0f)
        {
            hitFlashTimer -= Time.deltaTime;

            if (hitFlashTimer <= 0f && spriteRenderer != null)
                spriteRenderer.color = originalColor;
        }

        attackTimer -= Time.deltaTime;

        Vector2 toPlayer = (Vector2)(
            player.position - transform.position);

        float distanceSquared = toPlayer.sqrMagnitude;

        switch (enemyType)
        {
            case EnemyType.Butcher:
                ChasePlayer(toPlayer, distanceSquared);
                TryContactAttack(distanceSquared);
                break;

            case EnemyType.Roller:
                ChasePlayer(toPlayer, distanceSquared);
                TryContactAttack(distanceSquared);
                break;

            case EnemyType.ManicBombard:
                ChasePlayer(toPlayer, distanceSquared);

                if (distanceSquared <= bomberRadius * bomberRadius)
                    TryBombAttack();
                break;

            case EnemyType.EliteLancer:
                TryRangedAttack(distanceSquared, toPlayer);
                break;

            case EnemyType.CaptainVor:
                ChasePlayer(toPlayer, distanceSquared);
                TryContactAttack(distanceSquared);
                break;
        }
    }

    private void ChasePlayer(Vector2 direction, float distanceSquared)
    {
        if (direction.sqrMagnitude <= 0.0001f)
            return;

        if (enemyType == EnemyType.ManicBombard &&
            distanceSquared <= bomberRadius * bomberRadius)
            return;

        Vector2 movement = direction.normalized
            * moveSpeed * Time.deltaTime;

        transform.position += (Vector3)movement;
    }

    private void TryContactAttack(float distanceSquared)
    {
        float hitRadius = attackRange;

        if (distanceSquared > hitRadius * hitRadius)
            return;

        if (attackTimer > 0f)
            return;

        if (playerHealth != null)
            playerHealth.TakeDamage(contactDamage);

        attackTimer = attackCooldown;
    }

    private void TryBombAttack()
    {
        if (attackTimer > 0f)
            return;

        float distanceSquared = (
            (Vector2)(player.position - transform.position)
        ).sqrMagnitude;

        if (distanceSquared <= bomberRadius * bomberRadius)
        {
            if (playerHealth != null)
                playerHealth.TakeDamage(bomberDamage);

            // The bomber will be removed by its bomb explosion, as it has a bomb
            attackTimer = attackCooldown;
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead || damage <= 0f)
            return;

        currentHealth -= damage;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
            hitFlashTimer = hitFlashDuration;
        }

        if (currentHealth <= 0f)
            Die();
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        if (xpGemPrefab != null && player != null)
        {
            SurvivorXPGem gem = Instantiate(
                xpGemPrefab,
                transform.position,
                Quaternion.identity
            );

            gem.Initialize(player, playerProgression, xpReward);
        }
        else if (playerProgression != null)
        {
            // Fallback if no gem prefab has been assigned and god has failed us PLEASE
            playerProgression.AddXP(xpReward);
        }

        gameObject.SetActive(false);
    }

    public float GetHealthFraction()
    {
        return baseHealth > 0f
            ? Mathf.Clamp01(currentHealth / baseHealth)
            : 0f;
    }
    
    private void OnEnable()
    {
        if (!ActiveEnemies.Contains(this))
            ActiveEnemies.Add(this);
    }

    private void OnDisable()
    {
        ActiveEnemies.Remove(this);
    }
    
    private void TryRangedAttack(
        float distanceSquared,
        Vector2 directionToPlayer)
    {
        if (distanceSquared >
            rangedAttackRange * rangedAttackRange)
            return;

        if (attackTimer > 0f)
            return;

        if (projectilePrefab == null)
            return;

        if (directionToPlayer.sqrMagnitude <= 0.0001f)
            return;

        SurvivorEnemyProjectile projectile = Instantiate(
            projectilePrefab,
            transform.position,
            Quaternion.identity
        );

        projectile.Initialize(
            player,
            directionToPlayer,
            rangedProjectileDamage,
            rangedProjectileSpeed
        );

        attackTimer = rangedProjectileCooldown;
    }
}
