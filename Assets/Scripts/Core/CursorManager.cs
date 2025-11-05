using UnityEngine;

namespace RPG.Core
{
    public class CursorManager : MonoBehaviour {
    
        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
