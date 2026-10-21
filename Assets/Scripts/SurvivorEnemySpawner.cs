
using UnityEngine;
using System.Collections.Generic;

public class SurvivorEnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemyEntry
    {
        public SurvivorEnemy prefab;

        [Min(0f)]
        public float spawnWeight = 1f;
    }

    [Header("References")]
    public Transform player;
    public Camera targetCamera;
    public List<EnemyEntry> enemyTypes = new List<EnemyEntry>();

    [Header("Spawn Position")]
    public float offscreenPadding = 2f;

    [Header("Starting Difficulty")]
    public float startingSpawnInterval = 2f;
    public int startingSpawnCount = 1;

    [Header("Difficulty Over Time")]
    public float difficultyInterval = 30f;
    public float spawnIntervalReduction = 0.1f;
    public int extraEnemiesPerIncrease = 1;
    public float minimumSpawnInterval = 0.25f;

    [Header("Performance")]
    public int maxAliveEnemies = 150;

    private float survivalTimer;
    private float spawnTimer;
    private float currentSpawnInterval;
    private int currentSpawnCount;
    private int playerRank = 1;
    private int aliveEnemies;
    private float difficultyTimer;

    private readonly List<SurvivorEnemy> activeEnemies =
        new List<SurvivorEnemy>();

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
                player = playerObject.transform;
        }

        if (targetCamera == null)
            targetCamera = Camera.main;

        if (player == null || targetCamera == null)
        {
            Debug.LogError(
                "EnemySpawner needs a Player and a Camera.");
            enabled = false;
            return;
        }

        currentSpawnInterval = startingSpawnInterval;
        currentSpawnCount = startingSpawnCount;
    }

    private void Update()
    {
        if (player == null || targetCamera == null)
            return;

        survivalTimer += Time.deltaTime;
        difficultyTimer += Time.deltaTime;
        spawnTimer += Time.deltaTime;

        RemoveInactiveEnemies();

        if (difficultyTimer >= difficultyInterval)
        {
            difficultyTimer -= difficultyInterval;
            IncreaseDifficulty();
        }

        if (spawnTimer >= currentSpawnInterval)
        {
            spawnTimer = 0f;
            SpawnEnemies();
        }
    }

    private void IncreaseDifficulty()
    {
        currentSpawnInterval = Mathf.Max(
            minimumSpawnInterval,
            currentSpawnInterval - spawnIntervalReduction);

        currentSpawnCount += extraEnemiesPerIncrease;

        Debug.Log(
            "Difficulty increased. Spawn count: "
            + currentSpawnCount);
    }

    private void SpawnEnemies()
    {
        for (int i = 0; i < currentSpawnCount; i++)
        {
            if (aliveEnemies >= maxAliveEnemies)
                break;

            SurvivorEnemy prefab = ChooseEnemyPrefab();

            if (prefab == null)
                return;

            Vector3 spawnPosition = GetOffscreenPosition();

            SurvivorEnemy enemy = Instantiate(
                prefab,
                spawnPosition,
                Quaternion.identity);

            enemy.Initialize(player, playerRank);

            activeEnemies.Add(enemy);
            aliveEnemies++;
        }
    }

    private SurvivorEnemy ChooseEnemyPrefab()
    {
        float totalWeight = 0f;

        foreach (EnemyEntry entry in enemyTypes)
        {
            if (entry.prefab != null)
                totalWeight += Mathf.Max(0f, entry.spawnWeight);
        }

        if (totalWeight <= 0f)
            return null;

        float roll = Random.Range(0f, totalWeight);

        foreach (EnemyEntry entry in enemyTypes)
        {
            if (entry.prefab == null)
                continue;

            roll -= Mathf.Max(0f, entry.spawnWeight);

            if (roll <= 0f)
                return entry.prefab;
        }

        return null;
    }

    private Vector3 GetOffscreenPosition()
    {
        float halfHeight = targetCamera.orthographicSize;
        float halfWidth = halfHeight * targetCamera.aspect;

        Vector3 cameraPosition = targetCamera.transform.position;

        float left = cameraPosition.x - halfWidth - offscreenPadding;
        float right = cameraPosition.x + halfWidth + offscreenPadding;
        float bottom = cameraPosition.y - halfHeight - offscreenPadding;
        float top = cameraPosition.y + halfHeight + offscreenPadding;

        int edge = Random.Range(0, 4);

        float x;
        float y;

        switch (edge)
        {
            case 0:
                x = left;
                y = Random.Range(bottom, top);
                break;

            case 1:
                x = right;
                y = Random.Range(bottom, top);
                break;

            case 2:
                x = Random.Range(left, right);
                y = top;
                break;

            default:
                x = Random.Range(left, right);
                y = bottom;
                break;
        }

        return new Vector3(x, y, player.position.z);
    }

    private void RemoveInactiveEnemies()
    {
        for (int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            if (activeEnemies[i] == null ||
                !activeEnemies[i].gameObject.activeInHierarchy)
            {
                activeEnemies.RemoveAt(i);
                aliveEnemies--;
            }
        }

        aliveEnemies = Mathf.Max(0, aliveEnemies);
    }

    public void SetPlayerRank(int rank)
    {
        playerRank = Mathf.Clamp(rank, 1, 27);
    }

    public float GetSurvivalTime()
    {
        return survivalTimer;
    }

    public int GetAliveEnemyCount()
    {
        return aliveEnemies;
    }
}
