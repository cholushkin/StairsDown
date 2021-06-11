using DG.Tweening;
using UnityEngine;

public class FallingObject : MonoBehaviour
{
    private Rigidbody _rigidbody;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (_rigidbody.IsSleeping())
            StartDiying();
    }

    private void StartDiying()
    {
        transform.DOScale(Vector3.zero, 1f).OnComplete(() => Object.Destroy(gameObject));
        enabled = false;
    }
}
