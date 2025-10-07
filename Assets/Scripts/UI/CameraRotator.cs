using UnityEngine;

namespace RPG.UI
{
    public class CameraRotator : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed = 1f;

        private void Update()
        {
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        }
    }
}