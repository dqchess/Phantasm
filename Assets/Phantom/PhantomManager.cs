﻿using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class PhantomManager : PhaNetworkingMessager {

	private static PhantomManager singleton;
	public static PhantomManager Singleton 
	{ 
		get 
		{ 
			return singleton; 
		} 
	}

	//Network manager
	PhaNetworkManager netManager = null;

	//Data of the phantoms
	public List<PhantomSpawnLocation> ListOfPhantomSpawners;
	public GameObject[] phantoms;
	public Vector3[] PreviousPositions;

	int size;

	// Use this for initialization
	void Awake () {
		singleton = this;
		netManager = PhaNetworkManager.Singleton;
		PhantomSpawnLocation[] tempSpawnLocations = GetComponentsInChildren<PhantomSpawnLocation>();
		size = tempSpawnLocations.Length;
		ListOfPhantomSpawners = new List<PhantomSpawnLocation>(size);
		phantoms = new GameObject[size];
		PreviousPositions = new Vector3[size];
		for (int i = 0; i < size; i++)
		{
			ListOfPhantomSpawners.Add(tempSpawnLocations[i]);
		}
		size = 0;
	}

	public void AddPhantom(ref GameObject givenPhantom)
	{
		phantoms[size] = givenPhantom;
		PreviousPositions[size] = givenPhantom.transform.position;
		size++;
	}

	public void ParsePhantomUpdate(int id, StringBuilder buffer)
	{
		ReceiveEnemyUpdate(phantoms[id], ref buffer);
	}


	/// <summary>
	/// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
	/// </summary>
	void FixedUpdate()
	{
		for (int i = 0; i < size; i++)
		{
			if (PreviousPositions[i] != phantoms[i].transform.position)
			{
				PreviousPositions[i] = phantoms[i].transform.position;

				if (PhaNetworkManager.Ishost)
				{
					//Send new position;
					SendEnemyUpdate(phantoms[i].transform.position, phantoms[i].transform.rotation, i, PhaNetworkingAPI.targetIP);
				}
			}
		}
	}
}
