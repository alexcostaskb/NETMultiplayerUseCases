using System;
using UnityEngine;

namespace Unity.Netcode.Samples.MultiplayerUseCases.Proximity
{
    /// <summary>
    /// Informs about the proximity status of the local player
    /// </summary>
    public class ProximityChecker : MonoBehaviour
    {
        [SerializeField, Tooltip("At which distance will the player be considered 'close'?")]
        private float m_ActivationRadius = 0.1f;

        [SerializeField, Tooltip("A visual representation of the radius?")]
        private Transform m_RadiusRepresentation;

        private Transform m_Transform;

        private event Action<bool> OnLocalPlayerProximityStatusChanged;

        internal bool LocalPlayerIsClose { get; private set; }

        private void Awake()
        {
            // cache the transform for performance
            m_Transform = transform;

            // set the radius representation to the correct position
            if (m_RadiusRepresentation)
            {
                const float k_OffsetFromGround = 0.01f;
                m_RadiusRepresentation.transform.localPosition = new Vector3(0, (m_Transform.lossyScale.y / -2) + k_OffsetFromGround, 0);
            }
        }

        internal void AddListener(Action<bool> callback)
        {
            OnLocalPlayerProximityStatusChanged += callback;
        }

        internal void RemoveListener(Action<bool> callback)
        {
            OnLocalPlayerProximityStatusChanged -= callback;
        }

        private void Update()
        {
            // update the radius representation
            if (m_RadiusRepresentation)
            {
                m_RadiusRepresentation.localScale = new Vector3(m_ActivationRadius * 2, m_RadiusRepresentation.localScale.y, m_ActivationRadius * 2);
            }

            // check if the local player is close enough
            bool oldValue = LocalPlayerIsClose;
            LocalPlayerIsClose = LocalPlayerIsCloseEnough(m_Transform.position, m_ActivationRadius);

            if (oldValue != LocalPlayerIsClose)
            {
                OnLocalPlayerProximityStatusChanged?.Invoke(LocalPlayerIsClose);
            }
        }

        /// <summary>
        /// returns true if the local player is close enough to the point
        /// </summary>
        /// <param name="point"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        private bool LocalPlayerIsCloseEnough(Vector3 point, float range)
        {
            //Note: This example shows how to use NetworkManager.Singleton.LocalClient.PlayerObject instead of a custom static flag to detect the local player
            if (NetworkManager.Singleton == null || NetworkManager.Singleton.LocalClient == null)
            {
                return false;
            }

            NetworkObject localPlayer = NetworkManager.Singleton.LocalClient.PlayerObject;

            if (!localPlayer)
            {
                return false;
            }

            // the player is close enough if the distance between the point and the player is less than the range
            return Vector3.Distance(point, localPlayer.transform.position) < range;
        }
    }
}