using System.Collections;
using GameLib.Random;
using ResourcesHelper;
using UnityEngine;

public class FallingObjectSpawner : MonoBehaviour
{
    public bool WorkOnStart;
    public Range PerSpawnDelay;
    public Range Scale;


    private IPseudoRandomNumberGenerator _rnd;
    public static FallingObjectSpawner CurrentSpawner;

    void Reset()
    {
        WorkOnStart = false;
        PerSpawnDelay = new Range(0.1f, 0.4f);
        Scale = new Range(1f, 1.5f);
    }

    void Start()
    {
        if (WorkOnStart)
        {
            StartSpawning();
        }
    }

    public void StartSpawning()
    {
        CurrentSpawner?.StopSpawning();
        CurrentSpawner = this;
        _rnd = RandomHelper.CreateRandomNumberGenerator();
        StartCoroutine(SpawningCoroutine());
    }

    IEnumerator SpawningCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(_rnd.FromRange(PerSpawnDelay));
            var prefab = FallingObjectsHolder.Instance.FallingObjectPrefabs.GetRandom(_rnd);
            var gObj = Instantiate(prefab, transform.position, Quaternion.Euler(
                _rnd.Range(0f, 360f),
                _rnd.Range(0f, 360f),
                _rnd.Range(0f, 360f)
            ));

            gObj.transform.localScale *= _rnd.FromRange(Scale);
        }
    }

    public void StopSpawning()
    {
        StopAllCoroutines();
    }
}
