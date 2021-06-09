using Assets.Plugins.Alg;
using DG.Tweening;
using UnityEngine;


namespace TowerGenerator
{
    public class OrtoCameraSizeFitter : MonoBehaviour
    {
        public Camera Camera;
        public float Duration;
        public bool DoJobOnAwake;
        public GameObject ObjectToFit;

        void Awake()
        {
            if(DoJobOnAwake)
                DoFit();
        }

        public void DoFit()
        {
            var bbs = ObjectToFit.BoundBox().size;
            Camera.DOOrthoSize(bbs.y * 0.55f,  Duration);
        }

    }
}