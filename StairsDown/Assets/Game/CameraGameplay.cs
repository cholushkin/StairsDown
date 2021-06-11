using System.Collections;
using System.Collections.Generic;
using Alg;
using UnityEngine;

public class CameraGameplay : Singleton<CameraGameplay>
{
    public Follower Follower;
    public LookAt Looker;
    public void Focus(Transform focusPoint)
    {
        Follower.Follow(focusPoint);
        Looker.Target = focusPoint;
    }
    
}
