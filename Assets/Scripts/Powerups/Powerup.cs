﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// API inspired by: https://github.com/antfarmar/Unity-3D-Asteroids/blob/master/Assets/Asteroids/Scripts/Powerups/Powerup.cs
public class Powerup : MonoBehaviour {

	public PlayerShip player;
	//public List<ShotSpawn> prevSS = new List<ShotSpawn> ();
	public Stack<ShotSpawn> prevSS = new Stack<ShotSpawn> ();
	protected float powerDuration = 10.0f;
	protected float endTime;
	protected bool isVisible;
	protected string id = "";

	void Spawn() {
		transform.position = Vector3.zero;	// Temp fixed location
		SetVisibility (true);
	}

	void Despawn() {
		SetVisibility (false);
	}

	public virtual void ActivatePower() {
		PlayerShip.SSContainer curr = player.ssDict [id];
		PlayerShip.SSContainer activePowerup = (PlayerShip.SSContainer) player.activeSS.Peek ();	// Get the active powerup's shotspawns
		bool comp = curr.compareTo (activePowerup);		// Compare to most recent entry in Stack
		if (comp == 0) {
			// Add full duration
			CancelInvoke ("DeactivatePower");			// Enables powerup duration extension
			Invoke ("DeactivatePower", powerDuration);	// Reset to state before powerup obtained
			endTime = Time.time + powerDuration;		// Record end time of powerup
		} else if (comp == -1) {
			// Deque and add more duration to new (hardcoded to 1/2)
			player.activeSS.Pop();		// Remove last powerup
			CancelInvoke ("DeactivatePower");			// Enables powerup duration extension
			endTime = endTime + powerDuration * 0.5f;		// Set new end time
			Invoke ("DeactivatePower", endTime);	// Reset to state after extended duration
		} else {
			// No duration added from worse powerup (no effect)
		}
	}


	public virtual void DeactivatePower() {
		//CancelInvoke ("DeactivatePower");			// Just in case we removed a powerup through override
		Debug.Log ("POWERUP DEACTIVATED: " + id);
	}

	void ShowInScene() {
		Spawn();
		SetVisibility(true);
	}

	void HideInScene() {
		Despawn();
		SetVisibility(false);
	}

	void SetVisibility(bool isVisible) {
		this.isVisible = isVisible;
		gameObject.GetComponent<Renderer>().enabled = this.isVisible;
		gameObject.GetComponent<Collider>().enabled = this.isVisible;
	}
		
	void OnTriggerEnter(Collider other) {

		if (other.gameObject.CompareTag (Constants.PlayerTag)) {
			prevSS.Push(other.GetComponent<PlayerShip>().activeSS);		// Add the previous state
			if (player == null) {
				player = other.GetComponent<PlayerShip> ();
			}
			ActivatePower ();
			HideInScene ();
			Debug.Log ("PICKED UP POWERUP: " + this.id);	// Print message to console
		}
	}

	void Start () {
		//HideInScene ();		// Powerups start out hidden
	}

}

