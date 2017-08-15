﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    public float xBound;
    public float yBound;
    public float spawnDelay = 1.0f;             // Delay between enemy spawns. A random # btwn 0 and this.
    public bool spawnEnabled = false;
    public int currLevel = 0;       // This is used from EnemySpawner. Enemies UP TO this index are allowed to be spawned. Reflects same-named value from GameManager.
    public bool bossSpawnEnabled = false;
    public List<Collider> mapColliders = new List<Collider>();     // Reference to GameManager list
    public GameObject mapCentre;        // The center of the map

    // Revised system for spawning enemies
    [Header("NEW_ENEMY_SPAWN_LOGIC")]
    // Indices must be matched up properly
    public List<GameObject> baseEnemyTypes;     // List of all lvl_1 normal enemies
    public List<Row> enemyTypeUpgrades;         // List of all upgrades for normal enemies

    public List<GameObject> baseBossTypes;      // List of all lvl_1 normal bosses

    // Booleans for each enemy type
    public bool pawnLevel2Unlocked = false;
    public bool pawnLevel3Unlocked = false;
    public bool rangedLevel2Unlocked = false;
    public bool rangedLevel3Unlocked = false;
    public bool bomberLevel2Unlocked = false;
    public bool bomberLevel3Unlocked = false;
    public bool turretLevel2Unlocked = false;
    public bool turretLevel3Unlocked = false;

    // The maximum of each enemy we can have alive at a time. MAY be subject to change as level increases.
    [Header("MAX_ENEMY_COUNTS")]
    public int MAX_PAWNS = 5;
    public int MAX_RANGED = 15;
    public int MAX_BOMBERS = 20;
    public int MAX_DROPSHIPS = 5;
    public int MAX_MEDICS = 5;
    public int MAX_TURRETS = 10;
    public int MAX_ASSASSINS = 5;
    public int MAX_BOSSES = 2;

    // The number of each mob alive right now.
    // Remember to update with bosses later on.
    [Header("NUM_ENEMIES_ALIVE")]
    public int NUM_PAWNS_ALIVE = 0;
    public int NUM_RANGED_ALIVE = 0;
    public int NUM_BOMBERS_ALIVE = 0;
    public int NUM_DROPSHIPS_ALIVE = 0;
    public int NUM_MEDICS_ALIVE = 0;
    public int NUM_TURRETS_ALIVE = 0;
    public int NUM_ASSASSINS_ALIVE = 0;
    public int NUM_BOSSES_ALIVE = 0;

    IEnumerator enemySpawnRoutine;

    public List<GameObject> enemyShips = new List<GameObject>();


    void Start()
    {
        //Vector3 boxSize = GetComponent<BoxCollider>().size;
        //xBound = boxSize.x / 2;
        //yBound = boxSize.y / 2;

        enemySpawnRoutine = StartSpawningEnemies();
        StartCoroutine(enemySpawnRoutine);
    }

    // Increase the number of total enemies that can be spawned, and how many of each.
    public void IncreaseLevel()
    {
        currLevel += 1;
    }

    public void EndLevel()
    {
        spawnEnabled = false;
    }

    public void RestartLevel()
    {
        // Reset mob counts
        NUM_PAWNS_ALIVE = 0;
        NUM_RANGED_ALIVE = 0;
        NUM_BOMBERS_ALIVE = 0;
        NUM_DROPSHIPS_ALIVE = 0;
        NUM_MEDICS_ALIVE = 0;
        NUM_TURRETS_ALIVE = 0;
        NUM_ASSASSINS_ALIVE = 0;
        NUM_BOSSES_ALIVE = 0;

        // Reset enemy upgrade booleans
        pawnLevel2Unlocked = false;
        pawnLevel3Unlocked = false;
        rangedLevel2Unlocked = false;
        rangedLevel3Unlocked = false;
        bomberLevel2Unlocked = false;
        bomberLevel3Unlocked = false;
        turretLevel2Unlocked = false;
        turretLevel3Unlocked = false;
        currLevel = 0;
        spawnEnabled = true;

        StopAllCoroutines();
        enemySpawnRoutine = StartSpawningEnemies();
        StartCoroutine(enemySpawnRoutine);
    }

    public void RecordKill(EnemyType enemyType)
    {
        switch (enemyType)
        {
            case EnemyType.Pawn:
                NUM_PAWNS_ALIVE -= 1;
                break;
            case EnemyType.Ranged:
                NUM_RANGED_ALIVE -= 1;
                break;
            case EnemyType.Bomber:
                NUM_BOMBERS_ALIVE -= 1;
                break;
            case EnemyType.Turret:
                NUM_TURRETS_ALIVE -= 1;
                break;
            case EnemyType.Boss:
                NUM_BOSSES_ALIVE -= 1;
                break;
        }
        // Upgrade levels of enemies as necessary
        int playerScore = GameManager.Singleton.playerScore;
        if (playerScore > 10000)
        {

        }
        else if (playerScore > 1400)
        {
            if (!turretLevel3Unlocked)
            {
                turretLevel3Unlocked = true;
                Debug.Log("TURRET_LVL3_UNLOCKED!");
            }
        }
        else if (playerScore > 1200)
        {
            if (!turretLevel2Unlocked)
            {
                turretLevel2Unlocked = true;
                Debug.Log("TURRET_LVL2_UNLOCKED!");
            }
        }
        else if (playerScore > 1000)
        {
            if (!bomberLevel3Unlocked)
            {
                bomberLevel3Unlocked = true;
                Debug.Log("BOMBER_LVL3_UNLOCKED!");
            }
        }
        else if (playerScore > 800)
        {
            if (!bomberLevel2Unlocked)
            {
                bomberLevel2Unlocked = true;
                Debug.Log("BOMBER_LVL2_UNLOCKED!");
            }
        }
        else if (playerScore > 600)
        {
            if (!rangedLevel3Unlocked)
            {
                rangedLevel3Unlocked = true;
                Debug.Log("RANGED_LVL3_UNLOCKED!");
            }
        }
        else if (playerScore > 400)
        {
            if (!rangedLevel2Unlocked)
            {
                rangedLevel2Unlocked = true;
                Debug.Log("RANGED_LVL2_UNLOCKED!");
            }
        }
        else if (playerScore > 200)
        {
            if (!pawnLevel3Unlocked)
            {
                pawnLevel3Unlocked = true;
                Debug.Log("PAWN_LVL3_UNLOCKED!");
            }
        }
        else if (playerScore > 100)
        {
            if (!pawnLevel2Unlocked)
            {
                pawnLevel2Unlocked = true;
                Debug.Log("PAWN_LVL2_UNLOCKED!");
            }
        }
    }

    IEnumerator StartSpawningEnemies()
    {
        mapColliders = GameManager.Singleton.mapColliders;  // Ref to map colliders
        int colliderCountEndIndex = mapColliders.Count - 1;
        while (true)
        {
            // Wait for a bit before we check to see if spawns are enabled (naive level restart logic)
            yield return new WaitForSeconds(2.0f);
            List<float> possibleSpawnAngles = new List<float>() { 90f, -30f, 30f };
            while (spawnEnabled)
            {
                // [TEST] spawn ships.
                // Spawn within map range.
                // First, choose a collider
                int colliderIndex = Random.Range(0, colliderCountEndIndex);
                //int colliderIndex = 0;
                BoxCollider targetCollider = (BoxCollider)mapColliders[colliderIndex];


                // Convert collider size vector to local space, then store it.
                // Scale these vectors, flip sign

                // Convert collider local space dimensions to world space
                Vector3 rawWidth = mapCentre.transform.TransformDirection(new Vector3(targetCollider.size.x / 2, 0, 0));
                Vector3 rawHeight = mapCentre.transform.TransformDirection(new Vector3(0, targetCollider.size.y / 2, 0));

                Debug.Log("RAW WIDTH: " + rawWidth);
                Debug.Log("RAW HEIGHT: " + rawHeight);

                rawWidth.x = rawWidth.x + mapCentre.transform.position.x;
                rawHeight.y = rawHeight.y + mapCentre.transform.position.y;

                // Rotation logic
                float randomRotation = possibleSpawnAngles[colliderIndex];
                Vector3 rotateDirWidth = rawWidth - new Vector3(mapCentre.transform.position.x, 0, 0);
                rotateDirWidth = Quaternion.Euler(new Vector3(0, 0, randomRotation)) * rotateDirWidth;
                Debug.Log("ROTATE_WIDTH: " + rotateDirWidth);
                // This is making width become height! Just scale every value?
                //rawWidth.x = rotateDir.x + mapCentre.transform.position.x;

                Vector3 rotateDirHeight = rawHeight - new Vector3(0, mapCentre.transform.position.y, 0);
                rotateDirHeight = Quaternion.Euler(new Vector3(0, 0, randomRotation)) * rotateDirHeight;

                rotateDirWidth *= Random.Range(0.1f, 1f);
                rotateDirHeight *= Random.Range(0.1f, 1f);

                // Possible sign flips
                if (Random.Range(0f, 1f) > 0.5f)
                {
                    rotateDirWidth *= -1;
                }
                if (Random.Range(0f, 1f) > 0.5f)
                {
                    rotateDirHeight *= -1;
                }
                // Sum up hori and vert components, and shift center to mapCentre as the origin
                Vector3 totalVector = rotateDirWidth + rotateDirHeight + mapCentre.transform.position;


                // Spawn UP TO current level progression.
                // Use / remove from DICT when MAX_CAP reached. Then remove / reset upon level reset or when enough of the enemy eliminated.
                // OR we could use current method, which is just instance vars tracking each type of enemy.
                int selectedEnemyIndex = Random.Range(0, baseEnemyTypes.Count);
                GameObject enemyShip = baseEnemyTypes[selectedEnemyIndex];
                EnemyType enemyType = (enemyShip.GetComponent<Ship>() != null) ? enemyShip.GetComponent<Ship>().enemyType : enemyShip.GetComponent<Turret>().enemyType;

                // First select a valid enemy
                bool alreadySpawnedMax = true;
                while (alreadySpawnedMax)
                {
                    switch (enemyType)
                    {
                        case EnemyType.Pawn:
                            if (NUM_PAWNS_ALIVE < MAX_PAWNS)
                            {
                                // Select upgraded form, otherwise just keep original form
                                if (pawnLevel3Unlocked)
                                {
                                    enemyShip = enemyTypeUpgrades[selectedEnemyIndex].row[1];
                                }
                                else if (pawnLevel2Unlocked)
                                {
                                    enemyShip = enemyTypeUpgrades[selectedEnemyIndex].row[0];
                                }
                                NUM_PAWNS_ALIVE += 1;
                                alreadySpawnedMax = false;
                            }
                            break;
                        case EnemyType.Ranged:
                            if (NUM_RANGED_ALIVE < MAX_RANGED)
                            {
                                // Select upgraded form, otherwise just keep original form
                                if (rangedLevel3Unlocked)
                                {
                                    enemyShip = enemyTypeUpgrades[selectedEnemyIndex].row[1];
                                }
                                else if (rangedLevel2Unlocked)
                                {
                                    enemyShip = enemyTypeUpgrades[selectedEnemyIndex].row[0];
                                }
                                NUM_RANGED_ALIVE += 1;
                                alreadySpawnedMax = false;
                            }
                            break;
                        case EnemyType.Bomber:
                            if (NUM_BOMBERS_ALIVE < MAX_BOMBERS)
                            {
                                // Select upgraded form, otherwise just keep original form
                                if (bomberLevel3Unlocked)
                                {
                                    enemyShip = enemyTypeUpgrades[selectedEnemyIndex].row[1];
                                }
                                else if (bomberLevel2Unlocked)
                                {
                                    enemyShip = enemyTypeUpgrades[selectedEnemyIndex].row[0];
                                }
                                NUM_BOMBERS_ALIVE += 1;
                                alreadySpawnedMax = false;
                            }
                            break;

                        case EnemyType.Turret:
                            if (NUM_TURRETS_ALIVE < MAX_TURRETS)
                            {
                                // Select upgraded form, otherwise just keep original form
                                if (turretLevel3Unlocked)
                                {
                                    enemyShip = enemyTypeUpgrades[selectedEnemyIndex].row[1];
                                }
                                else if (turretLevel2Unlocked)
                                {
                                    enemyShip = enemyTypeUpgrades[selectedEnemyIndex].row[0];
                                }
                                NUM_TURRETS_ALIVE += 1;
                                alreadySpawnedMax = false;
                            }
                            break;
                        case EnemyType.Boss:
                            if (NUM_BOSSES_ALIVE < MAX_BOSSES)
                            {
                                NUM_BOSSES_ALIVE += 1;
                                alreadySpawnedMax = false;
                            }
                            break;
                    }
                    if (!alreadySpawnedMax)
                    {
                        // We've found a valid enemyType, so spawn that!
                        break;
                    }
                    // Find another eligible spawnable enemy type
                    selectedEnemyIndex = Random.Range(0, baseEnemyTypes.Count);
                    enemyShip = baseEnemyTypes[selectedEnemyIndex];
                    enemyType = (enemyShip.GetComponent<Ship>() != null) ? enemyShip.GetComponent<Ship>().enemyType : enemyShip.GetComponent<Turret>().enemyType;

                    yield return null;
                }

                //Debug.Log("ENEMY TYPE: " + enemyType);
                // Now spawn the enemy
                //Vector3 spawnLocation = new Vector3(rawWidth.x, rawHeight.y, 0f);

                PoolObject poolObject = PoolManager.Instance.ReuseObjectRef(enemyShip, totalVector, Quaternion.identity);

                // Orient the enemy towards player
                Vector3 dist = (GameManager.Singleton.playerShip.transform.position - poolObject.transform.position).normalized;    // Find vector difference between target and this
                float zAngle = (Mathf.Atan2(dist.y, dist.x) * Mathf.Rad2Deg) - 90;  // Angle of rotation around z-axis (pointing upwards)
                Quaternion desiredRotation = Quaternion.Euler(0, 0, zAngle);        // Store rotation as an Euler, then Quaternion
                poolObject.transform.rotation = desiredRotation;

                // Wait a bit before spawning next enemy.
                // We COULD spawn multiple at a time / formations!
                yield return new WaitForSeconds(Random.Range(0, spawnDelay));
            }
            yield return null;
        }
    }
}

