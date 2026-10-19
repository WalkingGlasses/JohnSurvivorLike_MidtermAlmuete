using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PropRandomizer : MonoBehaviour
{
    public List<GameObject> propSpawnpoints;
    public List<GameObject> propPrefabs;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnProps();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnProps()
    {
        foreach (GameObject sp in propSpawnpoints)
        {
            int rand = Random.Range(0, propPrefabs.Count);
            GameObject prop = Instantiate(propPrefabs[rand], sp.transform.position, Quaternion.identity);
            prop.transform.parent = sp.transform;
        }
    }
}
