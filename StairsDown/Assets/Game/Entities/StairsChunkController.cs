using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using GameLib;
using TowerGenerator;
using UnityEngine;
using UnityEngine.Assertions;

// game domain logic of stairs chunk 
public class StairsChunkController : MonoBehaviour, CollisionPropagator.ICollisionHandle
{
    public FallingObjectSpawner Spawner;
    
    private bool _isCaptured = false;
    private ChunkControllerBase _chunk;

    

    void Awake()
    {
        _chunk = GetComponent<ChunkControllerBase>();
        Assert.IsNotNull(_chunk);
    }

    
    public void OnTouchByFallingObject(FallingObject fallingObject)
    {
        if (!_isCaptured)
            CaptureStairs();
    }

    private void CaptureStairs()
    {
        _isCaptured = true;

        AimGadgetController.Instance.Attach(_chunk.ExitConnector.transform);
        
        if (Spawner != null)
        {
            if (!Spawner.gameObject.activeInHierarchy)
                return;

            Spawner.StartSpawning();
        }
    }
}
