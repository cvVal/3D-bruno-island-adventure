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
        private float _walkTime;
        private float _pauseTime;
        private bool _isWalking = true;

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
            _walkTime += Time.deltaTime;
            
            if (_walkTime > walkDuration) _isWalking = false;

            if (!_isWalking)
            {
                _pauseTime += Time.deltaTime;
                
                if (_pauseTime < pauseDuration) return;
                
                ResetTimers();
            }
            
            _lengthWalked += Time.deltaTime * _agentCmp.speed;
            
            if (_lengthWalked > _splineLength) _lengthWalked = 0f;
            
            _splinePosition = Mathf.Clamp01(_lengthWalked / _splineLength);
        }

        public void ResetTimers()
        {
            _pauseTime = 0f;
            _walkTime = 0f;
            _isWalking = true;
        }

        public Vector3 GetFartherOutPosition()
        {
            var tempSplinePosition = _splinePosition + 0.02f;
            
            if (tempSplinePosition >= 1f) tempSplinePosition -= 1f;

            return _splineCmp.EvaluatePosition(tempSplinePosition);
        }
    }
}
