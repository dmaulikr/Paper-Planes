﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotMover : MonoBehaviour {

	public float speed;

	void Start () {
		GetComponent<Rigidbody>().AddForce(transform.up * speed * Time.deltaTime);
		//GetComponent<Rigidbody>().AddForce(transform.forward * -speed);
	}


	// Triggers on collision with enemy ship's rigidbody's collider
	void OnTriggerEnter(Collider other) {

		if (other.gameObject.CompareTag (Constants.EnemyTag)) {

			int scoreToAdd = 100;

			GameManager.playerScore += scoreToAdd;	// Add new score in GameManager
			UIManager.Instance.UpdateScore ();	// Update score in UI

			Destroy (this.gameObject);		// Remove the shot
			Debug.Log ("SHOT HIT ENEMY!");	// Print message to console
		}
	}
}
