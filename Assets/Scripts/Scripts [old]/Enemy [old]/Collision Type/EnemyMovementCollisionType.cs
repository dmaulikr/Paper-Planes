﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementCollisionType : MonoBehaviour {


	public float speed = 1.0f;
	public float rotationSpeed = 90.0f;
	public GameObject target;

	private Vector2 initialPos;
	private Vector2 offset = new Vector2(0, 3);
		

	// Use this for initialization
	void Start () {
		initialPos = new Vector2(transform.position.x, transform.position.y);
		target = GameObject.FindGameObjectWithTag (Constants.PlayerTag);
	}
	
	void Update() {

		// Move enemy ship up and down
		//this.transform.position = Vector2.Lerp (initialPos - offset, initialPos + offset, (Mathf.Sin(speed * Time.time) + 1.0f) / 2.0f);	// Natural up and down movement

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
