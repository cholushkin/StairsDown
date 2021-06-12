using System.Collections;
using System.Collections.Generic;
using Alg;
using UnityEngine;

public class Difficulty : Singleton<Difficulty>
{
    
    // chance to get spawner
    [Range(0f, 1f)]
    public float SpawnerChance;

    // rate of spawner


    // aim move speed
    // aim gadget diversity 
    public void Decrease()
    {
        SpawnerChance -= 0.01f / 5f; // 1 percent per 5 conquers
        SpawnerChance = Mathf.Clamp(SpawnerChance, 0.1f, 1f);
    }
}
