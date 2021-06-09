using System.Collections;
using System.Collections.Generic;
using GameLib.DataStructures;
using GameLib.Random;
using TowerGenerator;
using UnityEngine;

public class StairsGeneratorProcessor : MonoBehaviour
{
    private TreeNode<Blueprint.Segment> _nextSegment;
    private IPseudoRandomNumberGenerator _rnd;
    private TreeNode<Blueprint.Segment> _stairs;
    private TreeNode<Blueprint.Segment> _lastSegment;

    void Awake()
    {
        _rnd = RandomHelper.CreateRandomNumberGenerator();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //// prepare next segment

            //var visSegPrefab = (GameObject)Resources.Load("Chunks/" + _rnd.FromArray(MetaProvider.Metas).ChunkName);
            //visSegPrefab.SetActive(false);
            //var visSegment = Object.Instantiate(visSegPrefab);
            //visSegment.name = visSegPrefab.name;
            //visSegment.transform.position = parent.position + topology.Geometry.Bounds.center;
            //visSegment.transform.SetParent(parent);

            //// rotation 
            //visSegment.transform.Rotate(visSegment.transform.up, topology.Geometry.Rotation);

            //var visSegController = visSegment.GetComponent<ChunkControllerBase>();
            //visSegController.Seed = bpSegment.Visual.Seed;
            //visSegController.Init();
            //visSegment.SetActive(true);
            //if (visSegController is ChunkControllerDimensionsBased dimBasedController)
            //    dimBasedController.SetDimensionIndex(topology.Geometry.SizeIndex);

            //visSegController.SetConfiguration();


            //CreateMemorySegment(_lastSegment, Vector3.forward, 
            //    Vector3.zero, )
        }
    }

    public void StartGenerate(Transform outcomeRoot)
    {
       // StartCoroutine(GenerateRoutine());
    }

    //protected IEnumerator GenerateRoutine()
    //{
    //    while (true)
    //    {
    //        yield return new WaitUntil(() => _nextMeta != null );
    //        ChunkFactory.in

    //    }

    //}
}
