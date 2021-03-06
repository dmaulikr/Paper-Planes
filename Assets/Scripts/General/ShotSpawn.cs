﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotSpawn : MonoBehaviour {

	public GameObject targetRotation;
	public bool multiFire = false;

	public virtual void CreateShot(bool isFiringBuffed=false) {

		// The parent should be the player or enemy sprite
		targetRotation = transform.parent.gameObject;		

		// Rotate shotSpawn relative to parent Player
		transform.localRotation = targetRotation.transform.rotation;	

		// Create the shot
		FiringShip firingShip = targetRotation.transform.GetComponent<IFires>() as FiringShip;		// We know that it'll be a firingship

		if (!isFiringBuffed) {
			//Debug.Log ("SHOT: " + firingShip.shot == null);
			//Debug.Log ("FIRINGSHOT: " + firingShip.fasterShot == null);

			// Straight shot
			PoolManager.Instance.ReuseObject (firingShip.shot, transform.position, transform.rotation * Quaternion.Inverse(targetRotation.transform.rotation));
			//GameObject shot = Instantiate (firingShip.shot, transform.position, transform.rotation * Quaternion.Inverse(targetRotation.transform.rotation)) as GameObject;

			if (multiFire) {
				// Left angled
				PoolManager.Instance.ReuseObject (firingShip.shot, transform.position,transform.rotation * Quaternion.Inverse(Quaternion.Euler(new Vector3(targetRotation.transform.localEulerAngles.x, targetRotation.transform.localEulerAngles.y, targetRotation.transform.localEulerAngles.z - 10))));
				//GameObject shot2 = Instantiate (firingShip.shot, transform.position,transform.rotation * Quaternion.Inverse(Quaternion.Euler(new Vector3(targetRotation.transform.localEulerAngles.x, targetRotation.transform.localEulerAngles.y, targetRotation.transform.localEulerAngles.z - 10)))) as GameObject;

				// Right angled
				PoolManager.Instance.ReuseObject (firingShip.shot, transform.position, transform.rotation * Quaternion.Inverse(Quaternion.Euler(new Vector3(targetRotation.transform.localEulerAngles.x, targetRotation.transform.localEulerAngles.y, targetRotation.transform.localEulerAngles.z + 10))));
				//GameObject shot3 = Instantiate (firingShip.shot, transform.position, transform.rotation * Quaternion.Inverse(Quaternion.Euler(new Vector3(targetRotation.transform.localEulerAngles.x, targetRotation.transform.localEulerAngles.y, targetRotation.transform.localEulerAngles.z + 10)))) as GameObject;

			}
		} else {
			
			// Straight shot
			PoolManager.Instance.ReuseObject (firingShip.fasterShot, transform.position, transform.rotation * Quaternion.Inverse(targetRotation.transform.rotation));
			//GameObject shot = Instantiate (firingShip.fasterShot, transform.position, transform.rotation * Quaternion.Inverse(targetRotation.transform.rotation)) as GameObject;

			if (multiFire) {
				// Left angled
				PoolManager.Instance.ReuseObject (firingShip.fasterShot, transform.position,transform.rotation * Quaternion.Inverse(Quaternion.Euler(new Vector3(targetRotation.transform.localEulerAngles.x, targetRotation.transform.localEulerAngles.y, targetRotation.transform.localEulerAngles.z - 10))));
				//GameObject shot2 = Instantiate (firingShip.fasterShot, transform.position,transform.rotation * Quaternion.Inverse(Quaternion.Euler(new Vector3(targetRotation.transform.localEulerAngles.x, targetRotation.transform.localEulerAngles.y, targetRotation.transform.localEulerAngles.z - 10)))) as GameObject;

				// Right angled
				PoolManager.Instance.ReuseObject (firingShip.fasterShot, transform.position,transform.rotation * Quaternion.Inverse(Quaternion.Euler(new Vector3(targetRotation.transform.localEulerAngles.x, targetRotation.transform.localEulerAngles.y, targetRotation.transform.localEulerAngles.z + 10))));
				//GameObject shot3 = Instantiate (firingShip.fasterShot, transform.position, transform.rotation * Quaternion.Inverse(Quaternion.Euler(new Vector3(targetRotation.transform.localEulerAngles.x, targetRotation.transform.localEulerAngles.y, targetRotation.transform.localEulerAngles.z + 10)))) as GameObject;

			}
		}
	}

}