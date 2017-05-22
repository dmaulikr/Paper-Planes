﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MissileDamageScrObj : UpgradableScriptableObject {

	// Prices for each diff. upgrade?
	public List<int> pricesList = new List<int> {
		// Indexed by level - 1
		2000,		// Lvl 0
		4000,		// Lvl 1
		8000		// Lvl 2

	};

	public List<int> damagesList = new List<int> {
		// Indexed by level - 1
		50,		// Lvl 0
		55,		// Lvl 1
		60		// Lvl 2

	};

	void OnEnable() {
		// Set all the data about this powerup
		powerupName = "Missile Damage HISDFJLKDSFJKDS";
		powerupPrice = 2000;
		powerupInfo = "DESCRIPTION FOR MISSILE DAMAGE";
		currLvl = 0;
		MAX_LEVEL = pricesList.Count;
	}

	public override void UpgradePowerup() {
		HomingMissileScrObj parentPow = (HomingMissileScrObj) parentPowerup;
		if (currLvl < MAX_LEVEL) {
			currLvl += 1;		// Increase level of skill
			parentPow.damage = damagesList [currLvl];	// Vars w/ 'level' are the index vars
		} else {
			Debug.Log ("ALREADY MAXED OUT " + powerupName + "AT MAX LVL OF " + currLvl);
		}
	}
}