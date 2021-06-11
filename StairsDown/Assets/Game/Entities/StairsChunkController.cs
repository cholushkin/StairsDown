using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class StairsChunkController : MonoBehaviour, CollisionPropagator.ICollisionHandle
{
    private bool _isCaptured = false;
    public FallingObjectSpawner Spawner;

    
    public void OnTouchByFallingObject(FallingObject fallingObject)
    {
        if (!_isCaptured)
            CaptureStairs();
    }

    private void CaptureStairs()
    {
        _isCaptured = true;

        if (Spawner != null)
        {
            if (!Spawner.gameObject.activeInHierarchy)
                return;

            Spawner.StartSpawning();
        }
    }
}
