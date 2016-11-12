﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

public class Agent : NetworkBehaviour
{

    public GameObject AgentUI;
    private Text SubObjectiveCounter;
    private Text AmmoCounter;
    // Use this for initialization
    void Start()
    {

    }

    // Awake is called when the script instance is being loaded
    public void Awake()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        AgentUI = Instantiate(AgentUI) as GameObject;

        Text[] textReferences;
        textReferences = AgentUI.GetComponentsInChildren<Text>();
        for (int i = 0; i < textReferences.Length; i++)
        {
            if (textReferences[i].name == "SubObjectiveCounter")
            {
                SubObjectiveCounter = textReferences[i];
                SetNumberOfObjectivesCompleted(FindObjectOfType<GameState>().numberOfSubObjectives);
            }
            else if (textReferences[i].gameObject.name == "AmmoCounter")
            {
                AmmoCounter = textReferences[i];
                SetAmmoCount(GetComponent<GunHandle>().weaponSettings.currentNumberOfRounds);
            }
        }

        SplashScreen endGameScreen = AgentUI.GetComponentInChildren<SplashScreen>();
        endGameScreen.screenOwner = gameObject;
        endGameScreen.OnTimeReached.AddListener(() => { CustomNetworkManager.singleton.GetComponent<NetworkManagerHUD>().showGUI = true; });
    }

    // Called on clients for player objects for the local client (only)
    public override void OnStartLocalPlayer()
    {
        AgentUI = Instantiate(AgentUI) as GameObject;

        Text[] textReferences;
        textReferences = AgentUI.GetComponentsInChildren<Text>();
        for (int i = 0; i < textReferences.Length; i++)
        {
            if (textReferences[i].name == "SubObjectiveCounter")
            {
                SubObjectiveCounter = textReferences[i];
                SetNumberOfObjectivesCompleted(FindObjectOfType<GameState>().numberOfSubObjectives);
            }
            else if (textReferences[i].gameObject.name == "AmmoCounter")
            {
                AmmoCounter = textReferences[i];
                SetAmmoCount(GetComponent<GunHandle>().weaponSettings.currentNumberOfRounds);
            }
        }
        CustomNetworkManager.singleton.GetComponent<NetworkManagerHUD>().showGUI = false;
        
        SplashScreen endGameScreen = AgentUI.GetComponentInChildren<SplashScreen>();
        endGameScreen.screenOwner = gameObject;
    }

    public void SetNumberOfObjectivesCompleted(int _objectivesCompleted)
    {
        SubObjectiveCounter.text = _objectivesCompleted.ToString();
    }

    public void SetAmmoCount(int _ammo)
    {
        AmmoCounter.text = _ammo.ToString("00");
    }

    // This function is called when the behaviour becomes disabled or inactive
    public void OnDisable()
    {
        AgentUI.GetComponentInChildren<SplashScreen>().createSplashScreen(0);
        GetComponent<GunHandle>().gunReference.gameObject.SetActive(false);
    }



    // This function is called when the MonoBehaviour will be destroyed
    public void OnDestroy()
    {
        //Destroy(AgentUI);

    }
}
