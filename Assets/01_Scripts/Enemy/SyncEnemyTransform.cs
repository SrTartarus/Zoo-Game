using UnityEngine;
using UnityEngine.Networking;

namespace Zoo.Enemy
{
    public class SyncEnemyTransform : NetworkBehaviour
    {
        [SerializeField]
        private float _posLerpRate = 15;
        [SerializeField]
        private float _rotLerpRate = 15;
        [SerializeField]
        private float _posThreshold = 0.1f;
        [SerializeField]
        private float _rotThreshold = 1f;

        [SyncVar]
        private Vector3 _lastPosition;

        [SyncVar]
        private Vector3 _lastRotation;

        // Update is called once per frame
        void Update()
        {
            TransmitTransform();
            LerpTransform();
        }

        void TransmitTransform()
        {
            if (!isServer)
                return;

            if(IsPositionChanged() || IsRotationChanged())
            {
                _lastPosition = transform.position;
                _lastRotation = transform.localEulerAngles;
            }
        }

        void LerpTransform()
        {
            if (isServer)
                return;

            InterpolatePosition();
            InterpolateRotation();
        }

        private void InterpolatePosition()
        {
            transform.position = Vector3.Lerp(transform.position, _lastPosition, Time.deltaTime * _posLerpRate);
        }

        private void InterpolateRotation()
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(_lastRotation), Time.deltaTime * _rotLerpRate);
        }

        private bool IsPositionChanged()
        {
            return Vector3.Distance(transform.position, _lastPosition) > _posThreshold;
        }

        private bool IsRotationChanged()
        {
            return Vector3.Distance(transform.localEulerAngles, _lastRotation) > _rotThreshold;
        }
    }
}