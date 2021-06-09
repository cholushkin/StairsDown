using System.Collections;
using GameLib.Random;
using ResourcesHelper;
using UnityEngine;

public class FallingObjectSpawner : MonoBehaviour
{
    public FallingObjectsHolder Prefabs;
    public bool WorkOnStart;
    public Range PerSpawnDelay;
    public Range Scale;


    private IPseudoRandomNumberGenerator _rnd;

    void Start()
    {
        if (WorkOnStart)
        {
            _rnd = RandomHelper.CreateRandomNumberGenerator();
            StartCoroutine(SpawningCoroutine());
        }
        
    }

    IEnumerator SpawningCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(_rnd.FromRange(PerSpawnDelay));
            var prefab = Prefabs.FallingObjectPrefabs.GetRandom(_rnd);
            var gObj = Instantiate(prefab, transform.position, Quaternion.Euler(
                _rnd.Range(0f, 360f),
                _rnd.Range(0f, 360f),
                _rnd.Range(0f, 360f)
            ));

            gObj.transform.localScale *= _rnd.FromRange(Scale);
        }
    }
}
