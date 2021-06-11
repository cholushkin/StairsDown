using System;
using System.Collections.Generic;
using Assets.Plugins.Alg;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator.ChunkImporter
{
    public static class ChunkCooker
    {
        [Serializable]
        public class ChunkImportInformation
        {
            public ChunkImportInformation(string chunkName)
            {
                ChunkName = chunkName;
                ConformationType = new Dictionary<ChunkConformationType, int>(32);
            }
            public string ChunkName;
            public string[] ChunkClass;
            public uint Generation;

            public int CommandsProcessedAmount;

            public int GroupStackAmount;
            public int GroupSetAmount;
            public int GroupSwitchAmount;
            public int ChunkControllerAmount;
            public int CollisionDependentAmount;
            public int DimensionsIgnorantAmount;
            public int DimensionsStackAmount;
            public int SuppressionAmount;
            public int SuppressedByAmount;
            public int InductionAmount;
            public int InducedByAmount;
            public int HiddenAmount;
            public int ClassNameAmount;
            public int ConnectorAmount;
            public int TagAmount;
            public int GenerationAttributeAmount;
            public Dictionary<ChunkConformationType,int>  ConformationType;
        }

        public static GameObject Cook(GameObject semifinishedEnt, ChunkImportInformation chunkImportInformation)
        {
            Debug.Log($"Cooking entity: {semifinishedEnt}");

            ExecuteFbxCommands(semifinishedEnt, chunkImportInformation);

            ApplyColliders(semifinishedEnt);

            ApplyMaterials(semifinishedEnt);

            ApplyScripts(semifinishedEnt);

            ConfigureChunkController(semifinishedEnt); // tree

            return semifinishedEnt;
        }

        private static void ExecuteFbxCommands(GameObject semifinishedEnt, ChunkImportInformation chunkImportInformation)
        {
            void ProcessCommand(Transform tr)
            {
                var fbxProp = tr.GetComponent<FbxProps>();
                if (fbxProp == null)
                    return;
                FbxCommand.Execute(fbxProp, tr.gameObject, chunkImportInformation);
                tr.gameObject.RemoveComponent<FbxProps>();
            }
            semifinishedEnt.transform.ForEachChildrenRecursive(ProcessCommand);
        }

        private static void ApplyMaterials(GameObject chunk)
        {
            var matWall = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Wall.mat");
            var matConnector = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Connector.mat");
            Assert.IsNotNull(matWall);
            Assert.IsNotNull(matConnector);

            var renders = chunk.GetComponentsInChildren<Renderer>();

            foreach (var render in renders)
            {
                if(render.gameObject.GetComponent<Connector>() != null)
                    render.material = matConnector;
                else
                    render.material = matWall;
            }
        }

        private static void ApplyColliders(GameObject semifinishedEnt)
        {
            var renders = semifinishedEnt.GetComponentsInChildren<Renderer>();

            foreach (var render in renders)
            {
                if (render.gameObject.GetComponent<FallingObjectSpawner>() != null)
                {
                    
                }
                else
                {
                    render.gameObject.AddComponent<MeshCollider>();
                }
            }
        }

        private static void ApplyScripts(GameObject semifinishedEnt)
        {
            var stairsController = semifinishedEnt.AddComponent<StairsChunkController>();
            stairsController.Spawner = semifinishedEnt.GetComponentInChildren<FallingObjectSpawner>(true);

            var renders = semifinishedEnt.GetComponentsInChildren<Renderer>();
            foreach (var render in renders)
            {
                if (render.gameObject.GetComponent<FallingObjectSpawner>() != null)
                {

                }
                else
                {
                    var propagator = render.gameObject.AddComponent<CollisionPropagator>();
                    propagator.CollisionHandler = stairsController;
                }
            }
        }

        private static void ConfigureChunkController(GameObject chunk)
        {
            var chunkController = chunk.GetComponent<ChunkControllerBase>();
            Assert.IsNotNull(chunkController);

            var baseComponents = chunk.GetComponentsInChildren<BaseComponent>(true);
            foreach (var baseComponent in baseComponents)
            {
                baseComponent.ChunkController = chunkController;
                baseComponent.InfluenceGroup = baseComponent.transform.GetComponentInParent<Group>(); 
            }

            chunkController.Validate();
        }
    }
}