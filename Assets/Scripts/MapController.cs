using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapController : MonoBehaviour
{
    public List<GameObject> terrainChunks;
    public GameObject player;
    public float checkerRadius;
    Vector3 noTerrainPosition;//direction where there ISNT a chunk
    public LayerMask terrainMask;
    private PlayerMovement pm;
    //public float terrainDistance = 20;//used to check distance between player and chunk to add next chunk
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pm = FindObjectOfType<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        ChunkChecker();
    }
    
    void ChunkChecker()
    {
        /*if (pm.moveDir.x > 0 &&  pm.moveDir.y == 0)//chunk checker going right
        {
            if(!Physics2D.OverlapCircle(player.transform.position + new Vector3(20, 0, 0), checkerRadius, terrainMask)) 
                noTerrainPosition = player.transform.position + new Vector3(20, 0, 0);
                SpawnChunk();
            
        }*/
        
    }

    void SpawnChunk()
    {
        int rand = Random. Range(0, terrainChunks .Count) ;
        Instantiate(terrainChunks[rand], noTerrainPosition, Quaternion. identity);
    }
}
