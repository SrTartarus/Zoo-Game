using UnityEngine;
using UnityEngine.Networking;

namespace Zoo.Player
{
    public class SyncPlayerTransform : NetworkBehaviour
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
            if (isLocalPlayer)
                return;

            InterpolatePosition();
            InterpolateRotation();
        }

        void FixedUpdate()
        {
            if (!isLocalPlayer)
                return;

            var posChanged = IsPositionChanged();

            if (posChanged)
            {
                CmdSendPosition(transform.position);
                _lastPosition = transform.position;
            }

            var rotChanged = IsRotationChanged();

            if (rotChanged)
            {
                CmdSendRotation(transform.localEulerAngles);
                _lastRotation = transform.localEulerAngles;
            }
        }

        private void InterpolatePosition()
        {
            transform.position = Vector3.Lerp(transform.position, _lastPosition, Time.fixedDeltaTime * _posLerpRate);
        }

        private void InterpolateRotation()
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(_lastRotation), Time.fixedDeltaTime * _rotLerpRate);
        }

        [Command(channel = Channels.DefaultUnreliable)]
        private void CmdSendPosition(Vector3 pos)
        {
            _lastPosition = pos;
        }

        [Command(channel = Channels.DefaultUnreliable)]
        private void CmdSendRotation(Vector3 rot)
        {
            _lastRotation = rot;
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

