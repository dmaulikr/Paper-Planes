﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class BomberBossStageTwoMS : MonoBehaviour, IMoveState
{
    [Header("GENERAL_DATA")]
    public Direction direction = Direction.PLAYER_UNDETECTED;
    public Direction Direction { get; set; }
    public BomberBossStageTwo bomberBossStageTwo;
    public bool slingshotMovementActive = false;
    public float movementSpeed = 5.0f;
    public float resizeDuration = 3.0f;
    public float currRingRetractTime;
    public float resizeStartTime;
    public int numMobsToSpawn = 6;
    public float bomberSpawnRadius = 7.0f;
    public float endTime;
    public bool spawned = false;
    public Dictionary<GameObject, GameObject> ringBomberDictionary = new Dictionary<GameObject, GameObject>();

    public GameObject coreSprite;
    public GameObject coreRingSprite;       // For the center core
    public GameObject ringSprite;

    [Header("ROTATION_DATA")]
    public Transform rotatingGearSprite;      // Gear that starts rotating faster whenever attack is about to initiate
    public float rotEndTime;        // A timer for our attacks that require rotation
    public Vector3 newRotationAngle;
    public float startingRotationFactor = 3.0f;
    public float rotationFactorInterval = 5.0f;     // How much we increase the rotation speed by
    public float maxRotationSpeed = 20.0f;

    //public float initialRotationDelay = 2.0f;
    //public float secondRotationDelay = 1.0f;
    public bool rotationSpeedIncreased = false;
    public float currentRotationFactor;
    public Rigidbody rushCollisionRigidBody;
    IEnumerator rotationRoutine;

    [Header("RUSH_ATTACK_DATA")]
    public bool rushAttackMovementActive = false;
    public float thrustFactor = 5.0f;
    public float rushAttackChargeTime = 5.0f;
    public float knockbackForce = 4000.0f;
    public Vector3 directionToPlayer;
    IEnumerator rushAttackMovementRoutine;

    [Header("SLINGSHOT_ATTACK_DATA")]
    public bool slingShotAttackActive = false;
    public GameObject spawnedMobsContainer;
    public GameObject bomberToSlingshot;           // Currently active bomber to slingshot
    IEnumerator slingShotMovementRoutine;

    void Start()
    {
        bomberBossStageTwo = GetComponent<BomberBossStageTwo>();
        rotationRoutine = StartRotating();
        StartCoroutine(rotationRoutine);
    }

    // Should NOT call this when starting for first time, only on reuse
    public void OnObjectReuse()
    {
        StopAllCoroutines();

        rotationRoutine = StartRotating();
        StartCoroutine(rotationRoutine);
    }

    public void UpdateState()
    {
        if (direction == Direction.PLAYER_DETECTED)
        {
            MoveToPlayer();     // Default movement
        }
    }

    public void ActivateMovementState(float endTime, Direction newDirection)
    {
        //Debug.Break();
        if (newDirection == Direction.BOMBER_BOSS_SLINGSHOT_MOVEMENT)
        {
            ActivateSlingShotMovement();
        }
        else if (newDirection == Direction.BOMBER_BOSS_RUSH_CHARGE_MOVEMENT)
        {
            //Debug.Break();
            ActivateRushMovement();
        }
    }

    public void DeactivateMovementState()
    {
        if (direction == Direction.BOMBER_BOSS_SLINGSHOT_MOVEMENT)
        {
            DeactivateSlingShotMovement();
        }
        else if (direction == Direction.BOMBER_BOSS_RUSH_FORWARDS_MOVEMENT)
        {
            DeactivateMovementState();
        }
    }

    public void ActivateRushMovement()
    {
        //Debug.Break();
        direction = Direction.BOMBER_BOSS_RUSH_CHARGE_MOVEMENT;
        rushAttackMovementActive = true;
        rushAttackMovementRoutine = RushAttackMovement();
        StartCoroutine(rushAttackMovementRoutine);
    }

    public void DeactivateRushMovement()
    {
        direction = Direction.PLAYER_DETECTED;
        rushAttackMovementActive = false;
        rushAttackMovementRoutine = null;
    }

    public void ActivateSlingShotMovement()
    {
        direction = Direction.BOMBER_BOSS_SLINGSHOT_MOVEMENT;
        slingShotAttackActive = true;
        slingShotMovementRoutine = SlingShotMovement();
        StartCoroutine(slingShotMovementRoutine);
    }

    public void DeactivateSlingShotMovement()
    {
        direction = Direction.PLAYER_DETECTED;
        slingShotAttackActive = false;
        slingShotMovementRoutine = null;
        //Debug.Break();
    }

    // This routine is for BOMBER_BOSS_SLINGSHOT_MOVEMENT
    // As a note: this looks pretty cool when we just keep spawning
    IEnumerator SlingShotMovement()
    {
        // Wait to come to a stop first before spawning
        // Spin for a certain duration, with rotation speed constant
        rotEndTime = Time.time + bomberBossStageTwo.initialSlingshotRotationDelay;
        while (Time.time < rotEndTime)
        {
            // Rotate the center sprite
            newRotationAngle = Vector3.forward * currentRotationFactor * Time.deltaTime;
            rotatingGearSprite.transform.Rotate(newRotationAngle);
            ringSprite.transform.Rotate(-newRotationAngle);
            yield return null;
        }

        // Now FS spawns bombers
        // Spin for a certain duration, with rotation speed constant
        rotEndTime = Time.time + bomberBossStageTwo.secondSlingshotRotationDelay;
        while (Time.time < rotEndTime)
        {
            // Rotate the center sprite
            newRotationAngle = Vector3.forward * currentRotationFactor * Time.deltaTime;
            rotatingGearSprite.transform.Rotate(newRotationAngle);
            ringSprite.transform.Rotate(-newRotationAngle);

            // Rotate ring of other bombers the other way
            newRotationAngle *= -1;
            spawnedMobsContainer.transform.Rotate(newRotationAngle);

            yield return null;
        }

        // Now we rotate, accelerating until all bombers are thrown
        // Rotate the center sprite
        while (((BomberBossStageTwoFS)(bomberBossStageTwo.fireState)).slingShotAttackActive)
        {
            newRotationAngle = Vector3.forward * currentRotationFactor * Time.deltaTime;
            rotatingGearSprite.transform.Rotate(newRotationAngle);
            ringSprite.transform.Rotate(-newRotationAngle);

            // Rotate ring of other bombers the other way
            newRotationAngle *= -1;
            spawnedMobsContainer.transform.Rotate(newRotationAngle);

            // Speed up rotation
            if (currentRotationFactor < maxRotationSpeed)
            {
                // Increase by our interval, or as much as possible under the max
                float rotationFactorDiff = maxRotationSpeed - currentRotationFactor;
                currentRotationFactor += Mathf.Min(rotationFactorInterval, rotationFactorDiff);
                //Debug.Break();
            }
            yield return null;
        }
        currentRotationFactor = startingRotationFactor;
    }

    // This routine is for BOMBER_BOSS_RUSH_MOVEMENT
    IEnumerator RushAttackMovement()
    {
        while (true)
        {
            if (rushAttackMovementActive)
            {
                //Debug.Break();
                // Start rotating faster, getting ready to rush to player. Eye should move, colors should change.
                float rushRotEndTime = Time.time + bomberBossStageTwo.rushAttackChargeDelay;
                while (Time.time < rushRotEndTime)
                {
                    newRotationAngle = Vector3.forward * currentRotationFactor * Time.deltaTime;
                    rotatingGearSprite.transform.Rotate(newRotationAngle);
                    ringSprite.transform.Rotate(-newRotationAngle);

                    // Rotate ring of other bombers the other way
                    newRotationAngle *= -1;
                    spawnedMobsContainer.transform.Rotate(newRotationAngle);

                    // Speed up rotation
                    if (currentRotationFactor < maxRotationSpeed)
                    {
                        // Increase by our interval, or as much as possible under the max
                        float rotationFactorDiff = maxRotationSpeed - currentRotationFactor;
                        currentRotationFactor += Mathf.Min(rotationFactorInterval, rotationFactorDiff);
                        //Debug.Break();
                    }
                    yield return null;
                }
                direction = Direction.BOMBER_BOSS_RUSH_FORWARDS_MOVEMENT;       // So we can sticky the player
                // Rush to player
                // Collisions will knock player back and knock powerups out of them
                directionToPlayer = (bomberBossStageTwo.target.transform.position - bomberBossStageTwo.transform.position).normalized;
                bomberBossStageTwo.GetComponent<Rigidbody>().AddForce(directionToPlayer * thrustFactor, ForceMode.Impulse);     // Propel boss forward
                currentRotationFactor = startingRotationFactor;

                yield return new WaitForSeconds(2.0f);      // To give time for rush
                rushAttackMovementActive = false;
            }
            yield return null;
        }
    }

    IEnumerator StartRotating()
    {
        while (true)
        {
            if (!slingShotAttackActive)
            {
                // GO into loop if we're under influence of attack; coroutine controls how long we stay in loop
                // Rush attack case
                while (rotationSpeedIncreased)
                {
                    // Rotate each of the sprites
                    Vector3 newRotationAngle = Vector3.forward * currentRotationFactor * Time.deltaTime;
                    rotatingGearSprite.transform.Rotate(newRotationAngle);
                    if (ringSprite != null)
                    {
                        ringSprite.transform.Rotate(newRotationAngle);
                    }
                    //transform.Rotate(Vector3.forward * currentRotationSpeed * Time.deltaTime); // Rotate the gear much faster
                    if (currentRotationFactor < maxRotationSpeed)
                    {
                        // Increase by our interval, or as much as possible under the max
                        float rotationFactorDiff = maxRotationSpeed - currentRotationFactor;
                        currentRotationFactor += Mathf.Min(rotationFactorInterval, rotationFactorDiff);
                    }
                    yield return null;
                }

                // The normal rotation case
                //currentRotationFactor = startingRotationFactor;
                Vector3 rotationAngle = Vector3.forward * startingRotationFactor * Time.deltaTime;
                rotatingGearSprite.transform.Rotate(rotationAngle);
                if (ringSprite != null)
                {
                    ringSprite.transform.Rotate(-rotationAngle);
                }
                yield return null;
            }
            yield return null;
        }
    }

    // Call this if IDLE
    public void TurnToPlayer()
    {
        if (bomberBossStageTwo.target != null)
        {

            Vector3 dist = (bomberBossStageTwo.target.transform.position - bomberBossStageTwo.transform.position).normalized;   // Find unit vector difference between target and this

            float zAngle = (Mathf.Atan2(dist.y, dist.x) * Mathf.Rad2Deg) - 90;  // Angle of rotation around z-axis (pointing upwards)
            Quaternion desiredRotation = Quaternion.Euler(0, 0, zAngle);        // Store rotation as an Euler, then Quaternion
            bomberBossStageTwo.transform.rotation = Quaternion.RotateTowards(bomberBossStageTwo.transform.rotation, desiredRotation, bomberBossStageTwo.rotationSpeed * Time.deltaTime);    // Rotate the enemy

        }
    }


    public void MoveToPlayer()
    {

        if (bomberBossStageTwo.target != null)
        {

            Vector3 dist = (bomberBossStageTwo.target.transform.position - bomberBossStageTwo.transform.position).normalized;   // Find unit vector difference between target and this

            float zAngle = (Mathf.Atan2(dist.y, dist.x) * Mathf.Rad2Deg) - 90;  // Angle of rotation around z-axis (pointing upwards)
                                                                                //Quaternion desiredRotation = Quaternion.Euler(0, 0, zAngle);        // Store rotation as an Euler, then Quaternion
                                                                                //bomberBossStageTwo.transform.rotation = Quaternion.RotateTowards(bomberBossStageTwo.transform.rotation, desiredRotation, bomberBossStageTwo.rotationSpeed * Time.deltaTime);    // Rotate the enemy

            bomberBossStageTwo.transform.position = Vector2.MoveTowards(bomberBossStageTwo.transform.position, bomberBossStageTwo.target.transform.position, movementSpeed * Time.deltaTime);

        }
    }

    public void MoveBackwards()
    {

        if (bomberBossStageTwo.target != null)
        {

            Vector3 dist = -(bomberBossStageTwo.target.transform.position - bomberBossStageTwo.transform.position).normalized;  // Find unit vector difference between target and this
            Vector3 angleDist = -dist;

            float zAngle = (Mathf.Atan2(angleDist.y, angleDist.x) * Mathf.Rad2Deg) - 90;  // Angle of rotation around z-axis (pointing upwards)
            Quaternion desiredRotation = Quaternion.Euler(0, 0, zAngle);       // Store rotation as an Euler, then Quaternion
            bomberBossStageTwo.transform.rotation = Quaternion.RotateTowards(bomberBossStageTwo.transform.rotation, desiredRotation, bomberBossStageTwo.rotationSpeed * Time.deltaTime);   // Rotate the enemy

            bomberBossStageTwo.transform.position = Vector2.MoveTowards(bomberBossStageTwo.transform.position, bomberBossStageTwo.transform.position + dist, Time.deltaTime * bomberBossStageTwo.speed);

        }

    }

}

