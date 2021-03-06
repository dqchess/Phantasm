﻿using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhaNetworkManager : PhaNetworkingMessager {

	public string OnlineSceneName = ""; 
	private static PhaNetworkManager singleton;
	public static PhaNetworkManager Singleton 
	{ 
		get 
		{ 
			return singleton; 
		} 
	}
	public static bool Ishost = false;

	//0 for agent, 1 for hacker.
	public static int characterSelection = 0;

	public GameObject AgentPrefab; GameObject AgentSpawned; Health AgentHealth; Rigidbody AgentRigidBody; NetworkedBehaviour AgentPrediction;
	public GameObject RemoteAgentPrefab;
	public GameObject HackerPrefab; GameObject SpawnedHacker; 
	public GameObject RemoteHackerPrefab;
	PhantomManager phantomManager;

	Vector3 previousPlayerVelocity;
	Quaternion previousPlayerRotation;

	public PhanSkipeManager skipeManager;

	private static bool NetworkInitialized = false;

	/// This function is called when the object becomes enabled and active.
	void OnEnable()
	{
		if (!NetworkInitialized && singleton == null)
		{
			singleton = this;
			PhaNetworkingAPI.mainSocket = PhaNetworkingAPI.InitializeNetworking();
			PhaNetworkManager.singleton.SendConnectionMessage(new StringBuilder("0.0.0.1"));
			Debug.Log("Networking initialized");

			SceneManager.activeSceneChanged += SpawnPlayer;
			NetworkInitialized = true;
			DontDestroyOnLoad(this);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	/// This function is called when the MonoBehaviour will be destroyed.
	void OnDestroy()
	{
		if (NetworkInitialized && singleton == this)
		{
			Debug.Log("Network is being shutdown");
			PhaNetworkingAPI.ShutDownNetwork(PhaNetworkingAPI.mainSocket);
			NetworkInitialized = false;
		}
	}

	public static IPAddress GetLocalHost()
	{
		//Get local IP address. Hope it doesn't change. It could. I should change this to whenever it specically tries to create a game.
		IPAddress[] ipv4Addresses = Dns.GetHostAddresses(Dns.GetHostName());
		for (int i = 0; i < ipv4Addresses.Length; i++)
		{
			if (ipv4Addresses.GetValue(i).ToString() != "127.0.0.1" && (ipv4Addresses.GetValue(i) as IPAddress).AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
			{
				PhaNetworkingAPI.hostAddress = ipv4Addresses.GetValue(i) as IPAddress;
				break;
			}
		}	
		return PhaNetworkingAPI.hostAddress;
	}

	// Update is called once per frame
	void FixedUpdate () {
		if (SceneManager.GetActiveScene().name != "Menu")
		{			
			MessageType receivedType;
			//So you know, this is a terrible set up, but it'll be functional.
			for (int i = 0; i < 20; i++)
			{//Receiving
				receivedType = (MessageType)ReceiveInGameMessage();
//				Debug.Log("receivedType: " + receivedType);
				switch	(receivedType)
				{
					case MessageType.PlayerUpdate:
					if(AgentPrediction != null)
					AgentPrediction.ReceiveBuffer(ref receiveBuffer);
					break;

					case MessageType.EnemyUpdate:
					phantomManager.ParsePhantomUpdate(int.Parse(receiveBuffer.ToString().Split(' ')[1]), receiveBuffer);
					break;

					case MessageType.HealthUpdate:
					AgentHealth.takeDamage(ParseHealthUpdate(receiveBuffer));
					break;

					case MessageType.ConsoleMessage:
					SpawnedHacker.GetComponent<RemoteTextEnter>().ReceiveCode(receiveBuffer);
					break;

					case MessageType.DoorUpdate:
					DoorManager.Singleton.parseDoorUpdate(ref receiveBuffer);
					break;

					case MessageType.ScoreUpdate:
					//TODO: Call the function for adding the new score data and saving it.
					break;

					case MessageType.AudioUpdate:
					skipeManager.ReceiveBuffer(ref receiveBuffer);
					break;

					default://This may be the first time I've ever had a reachable default statement...
					return; //No more messages, so let's have an early exit.
				}
			}
		}
	}
	
	void SpawnPlayer(Scene _scene1, Scene _scene2)
	{
		if (_scene2.name != "Menu")
		{		
			if (characterSelection == 0)
			{
				Ishost = true;			
				AgentSpawned = GameObject.Instantiate(AgentPrefab); //Local player is agent.
				AgentHealth = AgentSpawned.GetComponent<Health>();
				AgentRigidBody = AgentSpawned.GetComponent<Rigidbody>();
				previousPlayerVelocity = new Vector3(AgentRigidBody.velocity.x, AgentRigidBody.velocity.y, AgentRigidBody.velocity.z);
				previousPlayerRotation = new Quaternion(AgentSpawned.transform.rotation.x, AgentSpawned.transform.rotation.y, AgentSpawned.transform.rotation.z, AgentSpawned.transform.rotation.w);

				SpawnedHacker = GameObject.Instantiate(RemoteHackerPrefab);	
			}
			else if (characterSelection == 1)
			{
				Ishost = false;			
				AgentSpawned = GameObject.Instantiate(RemoteAgentPrefab);
				AgentHealth = AgentSpawned.GetComponent<Health>();
				Vector3 startPosition = FindObjectOfType<PlayerStartLocation>().transform.position;
				AgentSpawned.transform.position = new Vector3(startPosition.x, startPosition.y, startPosition.z);
				AgentSpawned.GetComponent<NetworkedMovement>().receivedPosition = new Vector3(startPosition.x, startPosition.y, startPosition.z);
				AgentRigidBody = AgentSpawned.GetComponent<Rigidbody>();
				AgentPrediction = AgentSpawned.GetComponent<NetworkedBehaviour>();
				SpawnedHacker = GameObject.Instantiate(HackerPrefab); //Local Player is Hacker. The order of instantiation here is important!
			}

			phantomManager = FindObjectOfType(typeof(PhantomManager)) as PhantomManager;
			if (phantomManager == null)
			{
				Debug.LogError("phantomManager not found. Fuck.");
			}

			//skipeManager.startSkipe();
		}
		else
		{
			int bytesreceived = 0;
			do
			{
				bytesreceived = PhaNetworkingAPI.ReceiveFrom(PhaNetworkingAPI.mainSocket, receiveBuffer, recvBufferSize);
				Debug.Log("Continuing to flush the fucking buffer, bytes: " + bytesreceived);
			} while (bytesreceived != 10035);
			//skipeManager.closeSkipe();
		}

	}
}
