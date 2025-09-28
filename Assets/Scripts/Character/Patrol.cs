using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Splines;

namespace RPG.Character
{
    public class Patrol : MonoBehaviour
    {
        [SerializeField] private GameObject splineGameObject;
        [SerializeField] private float walkDuration = 3f;
        [SerializeField] private float pauseDuration = 2f;

        private SplineContainer _splineCmp;
        private NavMeshAgent _agentCmp;

        private float _splinePosition;
        private float _splineLength;
        private float _lengthWalked;
        private float walkTime;
        private float pauseTime;
        private bool isWalking = true;

        private void Awake()
        {
            if (splineGameObject is null)
            {
                Debug.LogWarning($"{name} does not have a Spline.", this);
                return;
            }

            _splineCmp = splineGameObject.GetComponent<SplineContainer>();
            _agentCmp = GetComponent<NavMeshAgent>();

            _splineLength = _splineCmp.CalculateLength();
        }

        public Vector3 GetNextPosition()
        {
            return _splineCmp.EvaluatePosition(_splinePosition);
        }

        public void CalculateNextPosition()
        {
            walkTime += Time.deltaTime;
            
            if (walkTime > walkDuration) isWalking = false;

            if (!isWalking)
            {
                pauseTime += Time.deltaTime;
                
                if (pauseTime < pauseDuration) return;
                
                ResetTimers();
            }
            
            _lengthWalked += Time.deltaTime * _agentCmp.speed;
            
            if (_lengthWalked > _splineLength) _lengthWalked = 0f;
            
            _splinePosition = Mathf.Clamp01(_lengthWalked / _splineLength);
        }

        public void ResetTimers()
        {
            pauseTime = 0f;
            walkTime = 0f;
            isWalking = true;
        }

        public Vector3 GetFartherOutPosition()
        {
            var tempSplinePosition = _splinePosition + 0.02f;
            
            if (tempSplinePosition >= 1f) tempSplinePosition -= 1f;

            return _splineCmp.EvaluatePosition(tempSplinePosition);
        }
    }
}
