using System.Collections;
using System.Collections.Generic;
using Assets.Plugins.Alg;
using DG.Tweening;
using GameLib.Random;
using TowerGenerator;
using UnityEngine;

public class CardSpawnPoint : MonoBehaviour
{
    public CardProvider CardProvider;
    private GameObject _current;
    private ChunkControllerBase _chunkController;

    private IPseudoRandomNumberGenerator _rnd = RandomHelper.CreateRandomNumberGenerator();
    //public CameraEntShowroomController CameraEntShowroomController;
  


    public void Update()
    {
        if (_current == null)
        {
            Place(CardProvider.GetRandomMeta(), _rnd.GetState());
        }
    }

    public void Place(MetaBase metaToPlace, IPseudoRandomNumberGeneratorState seed)
    {
        // remove _current
        Destroy(_current);

        // spawn new
        _current = ChunkFactory.CreateChunkRnd(metaToPlace, seed, transform, transform.position);

        // disable stand
        _current.transform.ForEachChildrenRecursiveTo(_HasTag);

        //CameraEntShowroomController.FitView(_current);
        _current.transform.localScale = Vector3.zero;
        _current.transform.DOScale(1f, 1f).SetEase(Ease.OutElastic);
        _chunkController = _current.GetComponent<ChunkControllerBase>();

    }

    private bool _HasTag(Transform tr)
    {
        print(tr.gameObject.name);
        if( tr.gameObject.HasTag("Stand"))
        {
            tr.gameObject.SetActive(false);
        }
        return false;
    }

    void OnDrawGizmos()
    {
        if (_current != null)
        {
            //Gizmos.DrawLine(Vector3.zero, Vector3.zero+Vector3.right*100f);
            var bounds = _chunkController.CalculateCurrentAABB();
            Gizmos.DrawWireCube(
                bounds.center,
                bounds.size);

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(bounds.center, 0.25f);

            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, 0.25f);
        }
    }
}
