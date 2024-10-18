using UnityEngine;
using DG.Tweening;

namespace TableInteraction.CameraSettings
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform cameraPoint;
        private Camera myCamera;


        private void Awake()
        {
            myCamera = Camera.main;
            myCamera.transform.DOMove(cameraPoint.transform.position, 3f);
            Vector3 rotation = cameraPoint.rotation.eulerAngles;
            myCamera.transform.DORotate(rotation,3f);
        }
    }
}
