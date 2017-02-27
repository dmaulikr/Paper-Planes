﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BomberShip : Ship {

	/** INSTANCE VARS */

	// Attributes values unique to BomberShip
	protected float _speed = 1.0f;	
	protected float _rotationSpeed = 100.0f;
	protected int _health = 100;
	protected int _enemyPoints = 100;

	// Unique attributes
	[Header("Explosion Attributes")]
	public int explosionDamage = 50;
	public float explosionDelay = 3.0f;
	public float damageRange = 3.0f;
	public bool isExploding = false;
	public bool explosionActive = false;

	private Rigidbody rb;


	/** HELPER METHODS */
	protected override void Initialize() {

		// Do normal initalization
		base.Initialize ();

		// Adjust attributes
		this.Speed = _speed;
		this.RotationSpeed = _rotationSpeed;
		this.Health = _health;
		this.EnemyPoints = _enemyPoints;

		rb = GetComponent<Rigidbody> ();	// For use in adjusting velocity
	}

	public override void Move () {

		/** MOVEMENT UPDATE */
		if (isExploding) {

			transform.Rotate(Vector3.forward * 150 * Time.deltaTime);	// Rotate the enemy MUCH FASTER; needs adjustment

			// If co-routine not running
			if (!explosionActive) {
				Debug.Log ("STARTED COROUTINE");
				StartCoroutine(BeginExplosion());
			}

			return;		// Break out of method

		} else {
			
			/** ROTATION UPDATE */

			// Enemy ship turns to face player
			Vector3 dist = target.transform.position - transform.position;	// Find vector difference between target and this
			dist.Normalize ();		// Get unit vector

			float zAngle = (Mathf.Atan2(dist.y, dist.x) * Mathf.Rad2Deg) + 180;	// Angle of rotation around z-axis (pointing upwards)

			Quaternion desiredRotation = Quaternion.Euler (0, 0, zAngle);		// Store rotation as an Euler, then Quaternion

			transform.rotation = Quaternion.RotateTowards (transform.rotation, desiredRotation, rotationSpeed * Time.deltaTime);	// Rotate the enemy

			/** MOVEMENT UPDATE */
			transform.position = Vector2.MoveTowards (transform.position, target.transform.position, Time.deltaTime * speed);

		}

	}		


	/** UNITY CALLBACKS */
	protected override void Start () {

		// Call our overridden initalization method
		Initialize ();

		// Check that we're calling the right Start() method
		Debug.Log("BOMBER SHIP START");

	}

	protected override void Update() {

		// Use default movement
		base.Update ();

	}

	// For shots
	protected void OnTriggerEnter(Collider other) {

		// If player shot hits us...
		if (other.gameObject.CompareTag (Constants.PlayerShot)) {

			Destroy (other.gameObject);		// Destroy the shot that hit us

			health -= GameManager.Singleton.playerDamage;			// We lost health

			// If the enemy stays far enough away and kills us...
			if (health <= 0 && !isExploding) {
				
				Instantiate (explosion, transform.position, transform.rotation);
				Destroy (this.gameObject);		// We're dead, so get rid of this object :/

				GameManager.Singleton.playerScore += enemyPoints;	// Add new score in GameManager
				UIManager.Singleton.UpdateScore ();	// Update score in UI

				Debug.Log("BOMBER ENEMY KILLED! Obtained: " + enemyPoints + "points!");
			}

			//Debug.Log ("ENEMY HEALTH: " + health);	// Print message to console */
		}
	}


	/** CO-ROUTINES */

	IEnumerator BeginExplosion() {

		// Set this to have only one co-routine running
		explosionActive = true;

		Debug.Log ("EXPLOSION COUNTDOWN BEGINS!");

		yield return new WaitForSeconds(explosionDelay);	 // Wait for a few seconds while beeping animation plays

		Instantiate (explosion, transform.position, transform.rotation);	// Explode! 

		/** Rudimentary way of checking if Player is within our explosion range; not normalized. */
		Vector3 playerPosition = target.transform.position;
		Vector3 ourPosition = transform.position;

		float xDiff = Mathf.Abs(playerPosition.x - ourPosition.x);
		float yDiff = Mathf.Abs(playerPosition.y - ourPosition.y);

		if (xDiff <= damageRange || yDiff <= damageRange) {
			GameManager.Singleton.playerHealth -= explosionDamage;	// Player damaged by explosion
			UIManager.Singleton.UpdateHealth ();	// Update health in UI
		}

		Destroy (transform.gameObject);		// We're dead, so get rid of this object :/
		Debug.Log("BOMBER ENEMY EXPLODED!");

	}
}
