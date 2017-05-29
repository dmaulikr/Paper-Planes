﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ShurikenDamageScrObj : UpgradableScriptableObject {

	// Prices for each diff. upgrade?
	public List<int> pricesList = new List<int> {
		// Indexed by level - 1
		2000,		// Lvl 0
		4000,		// Lvl 1
		8000		// Lvl 2

	};

	public List<int> damagesList = new List<int> {
		// Indexed by level - 1
		45,		// Lvl 0
		50,		// Lvl 1
		55		// Lvl 2

	};

	void OnEnable() {
		// Set all the data about this powerup
		currLvl = 0;
		powerupName = "Shuriken Damage: Tier " + (currLvl + 1);
		powerupPrice = pricesList[0];		// Default price is lvl 1 price
		powerupInfo = "A shuriken does <b>" + damagesList[currLvl] + "</b> damage.";
		MAX_LEVEL = pricesList.Count;
	}

	public override int UpgradePowerup() {
		ShurikenScrObj parentPow = (ShurikenScrObj) parentPowerup;
		if (currLvl <= MAX_LEVEL) {
			parentPow.damage = damagesList [currLvl];	// Vars w/ 'level' are the index vars
			currLvl += 1;		// Increase level of skill

			if (currLvl < MAX_LEVEL) {
				powerupName = "Shuriken Damage: Tier " + (currLvl + 1);
				powerupPrice = pricesList[currLvl];
				powerupInfo = "A shuriken does <b>" + damagesList[currLvl] + "</b> damage.";
			}

			return 1;			// Note if purchase is successful
		} else {
			Debug.Log ("ALREADY MAXED OUT " + powerupName + " AT MAX LVL OF " + currLvl);
			return -1;			// Note that powerup upgrades are already maxed out
		}
	}

	public override int GetPrice() {
		if (currLvl < MAX_LEVEL) {
			//powerupPrice = pricesList [currLvl];
			return powerupPrice;
		} else {
			Debug.Log ("ALREADY MAXED OUT " + powerupName + " AT MAX LVL OF " + (currLvl - 1));
			return -1;
		}
	}
}