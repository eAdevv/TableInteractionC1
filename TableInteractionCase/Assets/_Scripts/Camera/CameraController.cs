using UnityEngine;
using System.Collections.Generic;
using System.Collections;
namespace TableInteraction.CameraSettings
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform cameraPoint;
        private Camera myCamera;


        private void Awake()
        {
            myCamera = Camera.main;
            StartCoroutine(SmoothCameraCoroutine());
        }

        private IEnumerator SmoothCameraCoroutine()
        {
            Quaternion rotation = cameraPoint.rotation;
            float moveDuration = 5f;

            float time = 0f;

            while (time < moveDuration)
            {
                myCamera.transform.position = Vector3.MoveTowards(myCamera.transform.position, cameraPoint.transform.position,
                    (time / moveDuration) * Vector3.Distance(myCamera.transform.position, cameraPoint.transform.position));

                myCamera.transform.rotation = Quaternion.Slerp(transform.rotation, rotation, time / moveDuration);

                time += Time.deltaTime;
                yield return null;
            }
            myCamera.transform.position = cameraPoint.transform.position;
            myCamera.transform.rotation = rotation;
        }

    }
}
