using UnityEngine;

namespace RPG.Utility
{
    public class Billboard : MonoBehaviour
    {
        private GameObject _camera;

        private void Awake()
        {
            _camera = GameObject.FindGameObjectWithTag(Constants.CameraTag);
        }

        private void LateUpdate()
        {
            var cameraDirection = transform.position + _camera.transform.forward;
            transform.LookAt(cameraDirection);
        }
    }
}