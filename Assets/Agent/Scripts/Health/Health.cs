﻿using UnityEngine;
using System.Collections;
using UnityEditor;

public class Health : MonoBehaviour {

    [Tooltip("Amount of health for the object")]
    public float health;
    private float currentHealth;

    public float deathDelay;

    public bool destroyOnDeath = true;

	// Use this for initialization
	void Start () {
        currentHealth = health;
	}
	
    public void takeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0.0f && destroyOnDeath)
        {
            Kill(deathDelay);
        }
        else
        {

        }

    }

    public void Kill(float delay = 0.0f)
    {
        Destroy(gameObject, delay);
    }
}
