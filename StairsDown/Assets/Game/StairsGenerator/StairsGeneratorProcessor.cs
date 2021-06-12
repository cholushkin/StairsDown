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
        if (_spawnedChunks == 0 ) 
        {
            var newChunkObj = ChunkFactory.CreateChunk(Cards.GetRandomMeta(),0);
            CurrentChunk = newChunkObj.GetComponent<ChunkControllerBase>();
            SetPosition(CurrentChunk, Vector3.zero);

            // disable entry connector
            var connectors = CurrentChunk.GetConnectors();
            var sorted = connectors.OrderBy(c => c.transform.position.x).ToList();
            var entryConnector = sorted.Last();
            entryConnector.gameObject.SetActive(false);
            CurrentChunk.ExitConnector = sorted.First();

            // make sure spawner is activated
            // todo: remove hack
            var spawner =  CurrentChunk.GetComponentInChildren<FallingObjectSpawner>();
            spawner.gameObject.SetActive(true);
            spawner.StartSpawning();
            _spawnedChunks++;
        }


        if (curCard != null && AimGadgetController.Instance.StatusActive)
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
            _spawnedChunks++;
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
