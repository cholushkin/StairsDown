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
    private int _spawnedChunks;

    protected override void Awake()
    {
        base.Awake();
        _spawnedChunks = 0;
    }


    void Update()
    {
        CardSpawnPoint curCard = null;
        if (Input.GetKeyDown(KeyCode.Alpha1))
            curCard = Cards.GetCard(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            curCard = Cards.GetCard(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            curCard = Cards.GetCard(2);
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            curCard = Cards.GetCard(3);


        // first ever stairs chunk
        if (_spawnedChunks == 0) 
        {
            var newChunkObj = ChunkFactory.CreateChunk(Cards.GetRandomMeta(),0);
            CurrentChunk = newChunkObj.GetComponent<ChunkControllerBase>();
            SetPosition(CurrentChunk, Vector3.zero);

            // disable entry connector
            var connectors = CurrentChunk.GetConnectors();
            var entryConnector = connectors
                .OrderBy(c => c.transform.position.x).Last();
            entryConnector.gameObject.SetActive(false);

            // make sure spawner is activated
            // todo: remove hack
            var spawner =  CurrentChunk.GetComponentInChildren<FallingObjectSpawner>();
            spawner.gameObject.SetActive(true);
            spawner.StartSpawning();
            _spawnedChunks++;
        }


        if (curCard != null)
        {
            // spawn a new chunk
            var newChunkObj = ChunkFactory.CreateChunk(curCard.Meta, curCard.Seed);
            var newChunk = newChunkObj.GetComponent<ChunkControllerBase>();

            var currentBounds = CurrentChunk.CalculateCurrentAABB(false, true);
            var newBounds = newChunk.GetComponent<ChunkControllerBase>().CalculateCurrentAABB(false, true);

            newChunk.name = curCard.Meta.ChunkName;
            SetPosition(newChunk, currentBounds.center);
            newChunk.transform.SetParent(transform);

            newChunk.transform.localPosition += Vector3.left * (currentBounds.extents.x + newBounds.extents.x);

            // vertical and horiz connection fit
            {
                var connectors = CurrentChunk.GetConnectors();
                var closestConnector = connectors
                    .OrderBy(c => c.transform.position.x).First();

                var newConnectors = newChunk.GetConnectors();
                var newConnectorsSorted = newConnectors
                    .OrderBy(c => -c.transform.position.x).ToList();
                var closestNewConnector = newConnectorsSorted.First();
                var farestNewConnector = newConnectorsSorted.Last();

                CameraGameplay.Instance.Focus(farestNewConnector.transform);

                var verticalDelta = closestConnector.transform.position.y - closestNewConnector.transform.position.y;
                var horizontalDelta = closestConnector.transform.position.z - closestNewConnector.transform.position.z;
                newChunk.transform.position += new Vector3(0, verticalDelta, horizontalDelta);
            }

            CurrentChunk = newChunk;
            curCard.Next();
            _spawnedChunks++;
            curCard = null;
        }
    }

    public void SetPosition(ChunkControllerBase chunk, Vector3 pos)
    {
        var bounds = chunk.CalculateCurrentAABB(false, true);
        var offset = chunk.transform.position - bounds.center;
        chunk.transform.position = pos + offset;
    }

}
