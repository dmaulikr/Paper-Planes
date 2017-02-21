﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManagerBomberType : MonoBehaviour {

	public GameObject explosion;

	private int enemyHealth = 100;
	private int shotDamage = 20;
	private int enemyPoints = 100;

	private GameObject player;
	private Rigidbody rb;

	void Start() {
		rb = GetComponent<Rigidbody> ();	// For use in adjusting velocity
		player = GameObject.FindGameObjectWithTag (Constants.PlayerTag);	// Get the player
	}

	// Triggers on collision with enemy ship's rigidbody's collider

	/** ENEMY TAKES CARE OF WHAT SHOT DOES TO IT */
	void OnTriggerEnter(Collider other) {

		// If player shot hits us...
		if (other.gameObject.CompareTag (Constants.PlayerShot)) {

			Destroy (other.gameObject);		// Destroy the shot that hit us

			enemyHealth -= shotDamage;			// We lost health

			if (enemyHealth <= 0) {
				Instantiate (explosion, transform.position, transform.rotation);
				Destroy (this.gameObject);		// We're dead, so get rid of this object :/
				GameManager.Singleton.playerScore += enemyPoints;	// Add new score in GameManager
				UIManager.Singleton.UpdateScore ();	// Update score in UI
				Debug.Log("ENEMY KILLED! Obtained: " + enemyPoints + "points!");
			}

			Debug.Log ("ENEMY HEALTH: " + enemyHealth);	// Print message to console
		}
	}
}
