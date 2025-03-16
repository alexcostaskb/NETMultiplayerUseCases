using Unity.Netcode.Samples.MultiplayerUseCases.Common;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Unity.Netcode.Samples.MultiplayerUseCases.Proximity
{
    /// <summary>
    /// Manages the color of a Networked object
    /// </summary>
    public class ColorManager : NetworkBehaviour
    {
        private NetworkVariable<Color32> m_NetworkedColor = new NetworkVariable<Color32>();
        private Material m_Material;
        private ProximityChecker m_ProximityChecker;
        private InputAction interactAction;

        private void Awake()
        {
            m_Material = GetComponent<Renderer>().material;
            m_ProximityChecker = GetComponent<ProximityChecker>();
        }

        private void Start()
        {
            // find the interact action
            interactAction = InputSystem.actions.FindAction("Interact");
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsClient)
            {
                /* in this case, you need to manually load the initial Color to catch up with the state of the network variable.
                * This is particularly useful when re-connecting or hot-joining a session */
                OnClientColorChanged(m_Material.color, m_NetworkedColor.Value);
                m_NetworkedColor.OnValueChanged += OnClientColorChanged;
                m_ProximityChecker.AddListener(OnClientLocalPlayerProximityStatusChanged);
            }
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            // remove the listeners
            if (IsClient)
            {
                m_NetworkedColor.OnValueChanged -= OnClientColorChanged;
                m_ProximityChecker.RemoveListener(OnClientLocalPlayerProximityStatusChanged);
            }
        }

        private void Update()
        {
            if (!IsClient || !m_ProximityChecker.LocalPlayerIsClose)
            {
                /* note: in this case there's only client-side logic and therefore the script returns early.
                 * In a real production scenario, you would have an UpdateManager running all Updates from a centralized point.
                 * An alternative to that is to disable behaviours on client/server depending to what is/is not going to be executed on that instance. */
                return;
            }

            if (interactAction.WasPressedThisFrame())
            {
                OnClientRequestColorChange();
            }
        }

        private void OnClientRequestColorChange()
        {
            ServerChangeColorRpc();
        }

        [Rpc(SendTo.Server)]
        private void ServerChangeColorRpc()
        {
            // change the color on the server
            m_NetworkedColor.Value = MultiplayerUseCasesUtilities.GetRandomColor();
        }

        private void OnClientColorChanged(Color32 previousColor, Color32 newColor)
        {
            // change the color on the client
            m_Material.color = newColor;
        }

        private void OnClientLocalPlayerProximityStatusChanged(bool isClose)
        {
            Debug.Log($"Local player is now {(isClose ? "close" : "far")}");
        }
    }
}