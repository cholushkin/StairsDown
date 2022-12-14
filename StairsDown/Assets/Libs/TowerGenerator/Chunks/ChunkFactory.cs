using GameLib.Random;
using UnityEngine;

namespace TowerGenerator
{
    public class ChunkFactory
    {
        private static IPseudoRandomNumberGenerator _rnd = RandomHelper.CreateRandomNumberGenerator();
        private static readonly float[] _angles = { 0f, 90f, 180f, 270f };

        public static Vector3 GetAttachPosition(Bounds parent, Vector3 attachDirection)
        {
            return Vector3.zero;
        }


        public static GameObject CreateChunk(MetaBase meta, long seed)
        {
            var visSegPrefab = (GameObject)Resources.Load("Chunks/" + meta.ChunkName);
            visSegPrefab.SetActive(false);
            var visSegment = Object.Instantiate(visSegPrefab);
            visSegment.name = visSegPrefab.name;
            
            visSegment.transform.localPosition = Vector3.zero;

            // rotation 
            visSegment.transform.localRotation = Quaternion.identity* Quaternion.Euler(-90,0,0);

            var baseChunkController = visSegment.GetComponent<ChunkControllerBase>();
            baseChunkController.Seed = seed;
            baseChunkController.Init();
            visSegment.SetActive(true);
            baseChunkController.SetConfiguration();

            // centering
            //var segBB = baseChunkController.CalculateCurrentAABB();
            //var offset = baseChunkController.transform.position - segBB.center;
            //visSegment.transform.position += offset;
            return visSegment;
        }

        public static GameObject CreateChunk(Blueprint.Segment bpSegment, Transform parent)
        {
            var topology = bpSegment.Topology;

            var visSegPrefab = (GameObject)Resources.Load("Chunks/" + topology.Geometry.Meta.ChunkName);
            visSegPrefab.SetActive(false);
            var visSegment = Object.Instantiate(visSegPrefab);
            visSegment.name = visSegPrefab.name;

            visSegment.transform.position = parent.position + topology.Geometry.Bounds.center;
            visSegment.transform.SetParent(parent);

            // rotation 
            visSegment.transform.Rotate(visSegment.transform.up, topology.Geometry.Rotation);

            var visSegController = visSegment.GetComponent<ChunkControllerBase>();
            visSegController.Seed = bpSegment.Visual.Seed;
            visSegController.Init();
            visSegment.SetActive(true);
            if (visSegController is ChunkControllerDimensionsBased dimBasedController)
                dimBasedController.SetDimensionIndex(topology.Geometry.SizeIndex);

            visSegController.SetConfiguration();

            // centering
            var segBB = visSegController.CalculateCurrentAABB(true, false);
            var offset = visSegController.transform.position - segBB.center;
            visSegment.transform.position += offset;
            return visSegment;
        }
    }
}