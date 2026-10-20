
using UnityEngine;
using System.Collections.Generic;

public class MapController : MonoBehaviour
{
    [Header("Terrain Prefabs")]
    public List<GameObject> terrainChunks;

    [Header("Player")]
    public GameObject player;

    [Header("Chunk Settings")]
    public Vector2 chunkSize = new Vector2(20f, 20f);

    [Min(1)]
    public int chunksAroundPlayer = 1;

    [Header("Generation")]
    public bool generateStartingArea = true;

    private HashSet<Vector2Int> generatedChunks =
        new HashSet<Vector2Int>();

    private void Start()
    {
        if (player == null)
        {
            Debug.LogError("MapController: Assign the Player.");
            enabled = false;
            return;
        }

        if (terrainChunks == null || terrainChunks.Count == 0)
        {
            Debug.LogError("MapController: Add terrain prefabs.");
            enabled = false;
            return;
        }

        if (chunkSize.x <= 0f || chunkSize.y <= 0f)
        {
            Debug.LogError("Chunk size must be greater than zero.");
            enabled = false;
            return;
        }

        if (generateStartingArea)//extra option in case lang since may starting area na yeah?
        {
            CheckChunks();
        }
    }

    private void Update()
    {
        CheckChunks();
    }

    private void CheckChunks()
    {
        Vector3 playerPosition = player.transform.position;

        //finds cell closest to layer pos
        Vector2Int playerChunk = new Vector2Int(
            Mathf.RoundToInt(playerPosition.x / chunkSize.x),
            Mathf.RoundToInt(playerPosition.y / chunkSize.y)
        );

        //checks surrounding cells including DIAGNUL
        for (int x = -chunksAroundPlayer;
             x <= chunksAroundPlayer; x++)
        {
            for (int y = -chunksAroundPlayer;
                 y <= chunksAroundPlayer; y++)
            {
                Vector2Int chunkCoordinate = new Vector2Int(
                    playerChunk.x + x,
                    playerChunk.y + y
                );

                if (!generatedChunks.Contains(chunkCoordinate))
                {
                    SpawnChunk(chunkCoordinate);
                }
            }
        }
    }

    private void SpawnChunk(Vector2Int coordinate)
    {
        int randomIndex = Random.Range(0, terrainChunks.Count);

        Vector3 spawnPosition = new Vector3(
            coordinate.x * chunkSize.x,
            coordinate.y * chunkSize.y,
            0f
        );

        GameObject newChunk = Instantiate(
            terrainChunks[randomIndex],
            spawnPosition,
            Quaternion.identity
        );

        newChunk.name = "Terrain Chunk " + coordinate;

        // records the cell chunk thingy so it doesnt respawn on it self again
        generatedChunks.Add(coordinate);
    }
}
