﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TriFirePowerup : Powerup {

	private string id = "TriFirePowerup";

	public override void ActivatePower() {

		//endTime = Time.time + powerDuration;
		timeObtained = Time.time;

		Debug.Log ("POWERUP ACTIVATED!: " + id);	// Identify powerup

		PlayerShip.SSContainer curr = player.ssDict [PlayerShip.Weapons.TRI];
		PlayerShip.SSContainer activePowerup = (PlayerShip.SSContainer) player.activeSS.Peek ();	// Get the active powerup's shotspawns

		int comp = curr.CompareTo (activePowerup);		// Compare to most recent entry in Stack

		if (!activePowerup.ID.Equals(PlayerShip.Weapons.NORMAL)) {
			if (comp == 0) {
				// [POWERUPS EQUAL] Add full duration
				CancelInvoke ("DeactivatePower");			// Enables powerup duration extension
				Invoke ("DeactivatePower", (PlayerShip.SSContainer.weaponTime - Time.time) + powerDuration);	// Reset to state before powerup obtained

				Debug.Log("REMAINING TIME: " + (PlayerShip.SSContainer.weaponTime - Time.time));

				endTime = Time.time + (PlayerShip.SSContainer.weaponTime - Time.time) + powerDuration;		// Record end time of powerup
				PlayerShip.SSContainer.weaponTime = endTime;						// Record new end time

				Debug.Log("EQUAL POWERUP!");
			} else if (comp == 1) {
				// [NEW POWERUP BETTER] Deque and add more duration to new (hardcoded to 1/2)
				player.RemovePowerup();							// Remove last powerup
				player.SetWeapons (PlayerShip.Weapons.TRI);		// Set weapons

				float remainingTime = PlayerShip.SSContainer.weaponTime - Time.time;
				Debug.Log("REMAINING TIME: " + remainingTime);
				Debug.Log(String.Format("ID: {0}, ENDTIME: {1}", id, PlayerShip.SSContainer.weaponTime));		// Test to see if we're starting this new powerup at correct time

				endTime = PlayerShip.SSContainer.weaponTime + powerDuration * 0.5f;		// Set new end time
				PlayerShip.SSContainer.weaponTime = endTime;						// Record new end time

				Debug.Log ("ENDTIME: " + endTime);

				activePowerup.activePowerup.CancelInvoke ("DeactivatePower");	// Stop the other power from deactivating (hence popping the better powerup)
				CancelInvoke ("DeactivatePower");								// Enables powerup duration extension
				Invoke ("DeactivatePower", endTime - Time.time);				// Reset to state after extended duration

				Debug.Log(String.Format("ID: {0}, REMAININGTIME2: {1} ; ENDTIME: {2}", id, (endTime - Time.time), endTime));

				Debug.Log("GOT BETTER POWERUP!");
			} else {
				// [OLD POWERUP BETTER] No duration added from worse powerup (no effect)
				Debug.Log("GOT WORSE POWERUP!");
			}
		} else {
			// [NO POWERUPS ACTIVE] so just add new powerup!
			player.SetWeapons (PlayerShip.Weapons.TRI);	// Set weapons

			endTime = Time.time + powerDuration;
			PlayerShip.SSContainer.weaponTime = endTime;		// Set end time

			PlayerShip.SSContainer activePowerup2 = (PlayerShip.SSContainer) player.activeSS.Peek ();	// Get the active powerup's shotspawns
			activePowerup2.activePowerup = this;

			CancelInvoke ("DeactivatePower");			// Enables powerup duration extension
			Invoke ("DeactivatePower", powerDuration);	// Reset to state before powerup obtained

			Debug.Log("GOT FIRST POWERUP (NONE ACTIVE PRIOR)!");
		}
	}

	public override void DeactivatePower() {

		//player.activeSS = prevSS;

		// Unprotected deque op
		player.activeSS.Pop();
		base.DeactivatePower ();
	}
}
