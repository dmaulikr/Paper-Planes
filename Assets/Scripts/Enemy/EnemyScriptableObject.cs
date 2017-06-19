﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemyScriptableObject : ScriptableObject
{
    // These are default values for values we need to reset
    // (applies for any value that might have changed)
    // Change values in ScrObj from now on for adjustments
    public int health;
    public float rotationSpeed;
    public float rotationFactor;
}