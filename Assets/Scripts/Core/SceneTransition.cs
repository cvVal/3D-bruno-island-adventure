using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.Core
{
    public static class SceneTransition
    {
        public static IEnumerator Initiate(int sceneIndex, AudioSource audioSourceCmp)
        {
            const float duration = 2f;

            while (audioSourceCmp.volume > 0)
            {
                audioSourceCmp.volume -= Time.deltaTime / duration;
                
                yield return new WaitForEndOfFrame();
            }
            
            SceneManager.LoadScene(sceneIndex);
        }
    }
}