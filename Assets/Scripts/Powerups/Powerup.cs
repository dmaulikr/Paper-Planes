﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// API inspired by: https://github.com/antfarmar/Unity-3D-Asteroids/blob/master/Assets/Asteroids/Scripts/Powerups/Powerup.cs
public class Powerup : PoolObject
{

    public PowerupScriptableObject powerupData;         // Ref to data container; data about upgrades, etc.

    public PlayerShip player;
    //public List<ShotSpawn> prevSS = new List<ShotSpawn> ();
    public Stack<ShotSpawn> prevSS = new Stack<ShotSpawn>();
    public float powerDuration = 3.0f;
    public float existsInWorldDuration = 10.0f;
    public float endTime;
    public float timeObtained;
    public GameObject pickupParticlePrefab;     // Particle system that plays on particle pickup
    protected bool isVisible;

    [Header("FADE_LERP_LOGIC")]
    public float fadeDuration = 2.0f;      // How long tutorial text stays before disappearing
    public float fadeLerpRatio;
    public Color fadeStartColor;
    public Color fadeEndColor;
    public bool assignedStartColor = false;
    public SpriteRenderer rawSprite;

    IEnumerator destroyAfterDurationRoutine;
    IEnumerator fadePowerupRoutine;
    public string powerupID;

    public override void Start()
    {
        rawSprite = GetComponent<SpriteRenderer>();
        player = GameManager.Singleton.playerShip.GetComponent<PlayerShip>();
        assignedStartColor = false;
        ShowInScene();
        base.Start();
    }

    void Update()
    {
        if (player == null)
        {
            player = GameManager.Singleton.playerShip.GetComponent<PlayerShip>();
        }
    }

    public override void OnObjectReuse()
    {
        Start();
        DestroyAfterDuration();     // Destroy if not picked up for a set duration
    }
    public override void DestroyForReuse()
    {
        rawSprite.material.color = fadeStartColor;      // Reset color
        base.DestroyForReuse();
    }

    public virtual void ActivatePowerup()
    {
        // Stop destroy routine if picked up
        if (destroyAfterDurationRoutine != null)
        {
            StopCoroutine(destroyAfterDurationRoutine);
            destroyAfterDurationRoutine = null;
        }
        if (fadePowerupRoutine != null)
        {
            StopCoroutine(fadePowerupRoutine);
            fadePowerupRoutine = null;
        }
        GameManager.Singleton.numPowerupsCollected += 1;
    }

    public void DestroyAfterDuration()
    {
        if (destroyAfterDurationRoutine != null)
        {
            StopCoroutine(destroyAfterDurationRoutine);
            destroyAfterDurationRoutine = null;
        }
        destroyAfterDurationRoutine = DestroyAfterDurationRoutine();
        StartCoroutine(destroyAfterDurationRoutine);
    }
    IEnumerator DestroyAfterDurationRoutine()
    {
        try
        {
            // Wait for set duration
            Invoke("FadePowerup", existsInWorldDuration - fadeDuration);    // Allocate some time to fade
        }
        catch (Exception exception)
        {
            // Invoke issues
        }
        yield return new WaitForSeconds(existsInWorldDuration);

        // Destroy if not picked up
        DestroyForReuse();

    }
    // Controls fadeout of powerup
    public void FadePowerup()
    {
        try
        {
            if (fadePowerupRoutine != null)
            {
                StopCoroutine(fadePowerupRoutine);
                fadePowerupRoutine = null;
            }
            fadePowerupRoutine = FadePowerupRoutine();
            StartCoroutine(fadePowerupRoutine);
        }
        catch (Exception exception)
        {
            // Sometimes errors if already inactive
        }

    }
    IEnumerator FadePowerupRoutine()
    {
        if (!assignedStartColor)
        {
            fadeStartColor = rawSprite.material.color;
            fadeStartColor.a = 1.0f;

            fadeEndColor = fadeStartColor;
            fadeEndColor.a = 0.0f;

            assignedStartColor = true;
        }
        fadeLerpRatio = 0.0f;
        while (fadeLerpRatio < 1.0f)
        {
            fadeLerpRatio += (Time.deltaTime / fadeDuration);
            Color lerpColor = Color.Lerp(fadeStartColor, fadeEndColor, fadeLerpRatio);
            rawSprite.material.color = lerpColor;
            yield return null;
        }
    }

    public virtual void DeactivatePowerup()
    {
        //CancelInvoke ("DeactivatePower");			// Just in case we removed a powerup through override
        Debug.Log(String.Format("{0} POWERUP DEACTIVATED AT ENDTIME: {1}", powerupID, Time.time));
        DestroyForReuse();
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        // Play effect, activate powerup, destroy
        if (other.gameObject.CompareTag(Constants.PlayerTag))
        {
            if (pickupParticlePrefab != null)
            {
                PoolManager.Instance.ReuseObject(pickupParticlePrefab, transform.position, Quaternion.identity);
            }
            ActivatePowerup();
            HideInScene();      // Can't destroy it yet b/c it needs to invoke a method

            //DestroyForReuse();
            //HideInScene();
        }
    }

    //void Spawn()
    //{
    //    transform.position = Vector3.zero;  // Temp fixed location
    //    SetVisibility(true);
    //}

    //void Despawn()
    //{
    //    SetVisibility(false);
    //}

    void ShowInScene()
    {
        //Spawn();
        SetVisibility(true);
    }

    public void HideInScene()
    {
        //Despawn();
        SetVisibility(false);
    }

    void SetVisibility(bool isVisible)
    {
        this.isVisible = isVisible;
        gameObject.GetComponent<Renderer>().enabled = this.isVisible;
        gameObject.GetComponent<Collider>().enabled = this.isVisible;
    }

    /*public void CancelInvoke() {
		CancelInvoke ("DeactivatePower");				// Enables powerup duration extension
	}*/
}

