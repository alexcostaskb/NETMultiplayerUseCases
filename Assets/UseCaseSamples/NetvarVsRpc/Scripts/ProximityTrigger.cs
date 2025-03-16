using Unity.Netcode.Samples.MultiplayerUseCases.Common;
using UnityEngine;

namespace Unity.Netcode.Samples.MultiplayerUseCases.NetVarVsRpc
{
    /// <summary>
    /// Toggles an object when the local player is close enough
    /// </summary>
    public class ProximityTrigger : MonoBehaviour
    {
        [SerializeField]
        private GameObject objectToToggle;

        [SerializeField, Tooltip("At which distance will the trigger be triggered?")]
        private float m_ActivationRadius = 0.5f;

        private Transform m_Transform;

        private void Awake()
        {
            // cache the transform for performance
            m_Transform = transform;
        }

        private void Update()
        {
            // toggle the object based on the proximity of the local player
            objectToToggle.SetActive(LocalPlayerIsCloseEnough(m_Transform.position, m_ActivationRadius));
        }

        internal static bool LocalPlayerIsCloseEnough(Vector3 point, float range)
        {
            // if there is no local player, it can't be close enough
            if (!PlayerManager.s_LocalPlayer)
            {
                return false;
            }

            // the local player is close enough if the distance between the point and the local player is less than the range
            return Vector3.Distance(point, PlayerManager.s_LocalPlayer.transform.position) < range;
        }
    }
}