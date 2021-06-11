using UnityEngine;

public class CollisionPropagator : MonoBehaviour
{
    public interface ICollisionHandle
    {
        void OnTouchByFallingObject(FallingObject fallingObject);
    }

    public StairsChunkController CollisionHandler;

    void OnCollisionEnter(Collision collision)
    {
        
        var fallingObject = collision.gameObject.GetComponent<FallingObject>();
        if (fallingObject != null)
        {
            CollisionHandler.OnTouchByFallingObject(fallingObject);
        }
    }
}
