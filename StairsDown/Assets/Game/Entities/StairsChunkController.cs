using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Assets.Plugins.Alg;
using DG.Tweening;
using GameLib;
using TowerGenerator;
using UnityEngine;
using UnityEngine.Assertions;

// game domain logic of stairs chunk 
public class StairsChunkController : MonoBehaviour, CollisionPropagator.ICollisionHandle
{
    public FallingObjectSpawner Spawner;
    
    private bool _isCaptured = false;
    private bool _isSpawnerGroupEnabled;
    private ChunkControllerBase _chunk;

    

    void Awake()
    {
        _chunk = GetComponent<ChunkControllerBase>();
        Assert.IsNotNull(_chunk);
    }

    public void EnableSpawnerGroup(bool flag)
    {
        // find group by tag=SpawnerGroup
        _isSpawnerGroupEnabled = flag;
        transform.ForEachChildrenRecursiveTo(_HasTag);
    }
    private bool _HasTag(Transform tr)
    {
        if (tr.gameObject.HasTag("SpawnerGroup"))
        {
            tr.gameObject.SetActive(_isSpawnerGroupEnabled);
            return true;
        }
        return false;
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

        // start delayed destroy
        transform.DOMove(transform.position + Vector3.down * 50, 10)
            .SetEase(Ease.InQuint)
            .SetDelay(20f)
            .OnComplete(() => Destroy(gameObject));

        Difficulty.Instance.Decrease();

        if (Spawner != null)
        {
            if (Spawner.gameObject.activeInHierarchy)
                Spawner.StartSpawning();
        }

       
    }
}
