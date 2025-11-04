using UnityEngine;

namespace RPG.UI
{
    public class UIQuitGame : MonoBehaviour
    {
        /// <summary>
        /// Description:
        /// Closes the game or exits play mode depending on the case
        /// Input:
        /// none
        /// Return:
        /// void
        /// </summary>
        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
        }
    }
}
