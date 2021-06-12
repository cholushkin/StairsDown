using System.Collections;
using System.Collections.Generic;
using Assets.Plugins.Alg;
using DG.Tweening;
using GameLib.Random;
using TowerGenerator;
using UnityEngine;

public class CardSpawnPoint : MonoBehaviour
{
    public MetaBase Meta;
    public long Seed;
    public bool Spawner;
    public CardProvider CardProvider;
    public Transform RotPivot;
    private GameObject _current;
    private ChunkControllerBase _chunkController;

    private IPseudoRandomNumberGenerator _rnd = RandomHelper.CreateRandomNumberGenerator();

    private Bounds _fitBounds;
    //public CameraEntShowroomController CameraEntShowroomController;


    public void Awake()
    {
        _fitBounds = gameObject.BoundBox();
    }
  


    public void Update()
    {
        if (_current == null)
        {
            Place(CardProvider.GetRandomMeta(), _rnd.GetState());
        }
    }

    public void Next()
    {
        if (_current == null)
            return;

        _chunkController.transform.DOScale(0.0f, 0.5f).SetEase(Ease.InBack).OnComplete(()=>Destroy(_current.gameObject));
    }

    public void Place(MetaBase metaToPlace, IPseudoRandomNumberGeneratorState seed)
    {
        // remove _current
        Destroy(_current);

        // spawn new
        _current = ChunkFactory.CreateChunk(metaToPlace, seed.AsNumber());


        // assign current card values
        Seed = seed.AsNumber();
        Meta = metaToPlace;
        _rnd.Next();

        

        // segment rotation
        var rot = _current.AddComponent<RotatingSegment>();
        rot.AlongAxis = Vector3.up;
        rot.SegmentAngle = 20;
        rot.LoopDuration = 4;
        rot.StartRotating();
        rot.InitialAngle = new Vector3(-90,0,30);


        // spawner
        var v = Random.value;
        Spawner = v < Difficulty.Instance.SpawnerChance;
        print(v);
        _current.GetComponent<StairsChunkController>().EnableSpawnerGroup(Spawner);

        // disable stand
        _current.transform.ForEachChildrenRecursiveTo(_HasTag);


        _chunkController = _current.GetComponent<ChunkControllerBase>();
        var chunkBounds = _chunkController.CalculateCurrentAABB(true, false);
        var sx = _fitBounds.size.x / chunkBounds.size.x;
        var sy = _fitBounds.size.y / chunkBounds.size.y;
        var sz = _fitBounds.size.z / chunkBounds.size.z;

        var s = Mathf.Min(sx, Mathf.Min(sy, sz));
        var scale = new Vector3(s,s,s);
        _chunkController.transform.localScale = scale;

        // centering
        _chunkController.transform.position = Vector3.zero;
        var offset = _chunkController.transform.position - chunkBounds.center * s;
        _chunkController.transform.position = transform.position;
        _chunkController.transform.position += offset;

        // animation
        _chunkController.transform.DOScale(scale * 0.6f, 1f).SetEase(Ease.OutElastic).From();
    }

    private bool _HasTag(Transform tr)
    {
        if( tr.gameObject.HasTag("Stand"))
        {
            tr.gameObject.SetActive(false);
        }
        return false;
    }

    //void OnDrawGizmos()
    //{
    //    if (_current != null)
    //    {
    //        //Gizmos.DrawLine(Vector3.zero, Vector3.zero+Vector3.right*100f);
    //        var bounds = _chunkController.CalculateCurrentAABB(false,true);
    //        Gizmos.DrawWireCube(
    //            bounds.center,
    //            bounds.size);

    //        Gizmos.color = Color.red;
    //        Gizmos.DrawSphere(bounds.center, 0.25f);

    //        Gizmos.color = Color.yellow;
    //        Gizmos.DrawSphere(transform.position, 0.25f);
    //    }
    //}
}
