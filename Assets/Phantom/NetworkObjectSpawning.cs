﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class NetworkObjectSpawning : NetworkBehaviour
{
    public GameObject phantomGameObject;
    private PhantomSpawnLocation[] spawnLocations;
    // Use this for initialization
    void Start()
    {
        
        
    }


    public override void OnStartServer()
    {
        base.OnStartServer();
        if (!isServer)
        {
            return;
        }
        spawnLocations = FindObjectsOfType<PhantomSpawnLocation>();
        if (spawnLocations.Length == 0)
        {
            Debug.Log("There are no spawn locations for the phantom.");
            NetworkServer.Spawn(Instantiate(phantomGameObject, Vector3.up * 2, Quaternion.identity) as GameObject);
        }
        else
        {
            PhantomSpawnLocation tempPos = spawnLocations[Random.Range(0, spawnLocations.Length - 1)];
            NetworkServer.Spawn(Instantiate(phantomGameObject, tempPos.transform.position, Quaternion.identity) as GameObject);
            phantomGameObject.GetComponent<Phantom>().previousSpawnLocation = tempPos;
        }
    }
    
}
