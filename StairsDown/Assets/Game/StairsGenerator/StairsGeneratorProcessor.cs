using System.Linq;
using Alg;
using Assets.Plugins.Alg;
using TowerGenerator;
using UnityEngine;

public class StairsGeneratorProcessor : Singleton<StairsGeneratorProcessor>
{
    public CardProvider Cards;
    public ChunkControllerBase CurrentChunk;
    public Transform FocusPoint;
    public  int SpawnedChunksCount { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        SpawnedChunksCount = 0;
    }


    void Update()
    {
        CardSpawnPoint curCard = null;
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Q))
            curCard = Cards.GetCard(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.W))
            curCard = Cards.GetCard(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.A))
            curCard = Cards.GetCard(2);
        else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.S))
            curCard = Cards.GetCard(3);

        // first ever stairs chunk
        if (SpawnedChunksCount == 0 ) 
        {
            var newChunkObj = ChunkFactory.CreateChunk(Cards.GetRandomMeta(),0);
            CurrentChunk = newChunkObj.GetComponent<ChunkControllerBase>();
            SetPosition(CurrentChunk, Vector3.zero);
            CurrentChunk.transform.SetParent(transform);

            // disable entry connector
            var connectors = CurrentChunk.GetConnectors();
            var sorted = connectors.OrderBy(c => c.transform.position.x).ToList();
            var entryConnector = sorted.Last();
            entryConnector.gameObject.SetActive(false);
            CurrentChunk.ExitConnector = sorted.First();

            // make sure spawner is activated
            CurrentChunk.GetComponent<StairsChunkController>().EnableSpawnerGroup(true);
            var spawner =  CurrentChunk.GetComponentInChildren<FallingObjectSpawner>();
            spawner.StartSpawning();                                                        
            SpawnedChunksCount++;
        }


        if (curCard != null && AimGadgetController.Instance.StatusActive)
        {
            // spawn a new chunk
            var newChunkObj = ChunkFactory.CreateChunk(curCard.Meta, curCard.Seed);
            var newChunk = newChunkObj.GetComponent<ChunkControllerBase>();

            var currentBounds = CurrentChunk.CalculateCurrentAABB(false, true);
            var newBounds = newChunk.GetComponent<ChunkControllerBase>().CalculateCurrentAABB(false, true);

            // spawner
            newChunk.GetComponent<StairsChunkController>().EnableSpawnerGroup(curCard.Spawner);


            newChunk.name = curCard.Meta.ChunkName;
            SetPosition(newChunk, currentBounds.center);
            newChunk.transform.SetParent(transform);

            newChunk.transform.localPosition += Vector3.left * (currentBounds.extents.x + newBounds.extents.x);

            // vertical and horiz connection fit
            {
                var connectors = CurrentChunk.GetConnectors();
                var exitConnector = connectors
                    .OrderBy(c => c.transform.position.x).First();

                var newConnectors = newChunk.GetConnectors();
                var newConnectorsSorted = newConnectors
                    .OrderBy(c => -c.transform.position.x).ToList();
                var entryNewConnector = newConnectorsSorted.First();
                var exitNewConnector = newConnectorsSorted.Last();

                newChunk.ExitConnector = exitNewConnector;

                CameraGameplay.Instance.Focus(exitNewConnector.transform);

                var verticalDelta = exitConnector.transform.position.y - entryNewConnector.transform.position.y;
                var horizontalDelta = exitConnector.transform.position.z - entryNewConnector.transform.position.z;
                newChunk.transform.position += new Vector3(0, verticalDelta, horizontalDelta);
            }

            // aiming
            newChunk.transform.position += AimGadgetController.Instance.GetOffset();
            var z = AimGadgetController.Instance.GetRotation();
            print(z);
            newChunk.transform.rotation = Quaternion.Euler(-90- z, 0, 0);



            CurrentChunk = newChunk;
            curCard.Next();
            SpawnedChunksCount++;
            curCard = null;
            AimGadgetController.Instance.Hide();
        }
    }

    public void SetPosition(ChunkControllerBase chunk, Vector3 pos)
    {
        var bounds = chunk.CalculateCurrentAABB(false, true);
        var offset = chunk.transform.position - bounds.center;
        chunk.transform.position = pos + offset;
    }

}
