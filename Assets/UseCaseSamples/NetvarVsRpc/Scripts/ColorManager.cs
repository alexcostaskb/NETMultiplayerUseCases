using Unity.Netcode.Samples.MultiplayerUseCases.Common;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Unity.Netcode.Samples.MultiplayerUseCases.NetVarVsRpc
{
    /// <summary>
    /// Manages the color of a Networked object
    /// </summary>
    public class ColorManager : NetworkBehaviour
    {
        [SerializeField]
        private bool m_UseNetworkVariableForColor; // if true, the color will be synchronized using a NetworkVariable

        private NetworkVariable<Color32> m_NetworkedColor = new NetworkVariable<Color32>();
        private Material m_Material;
        private InputAction interactAction;

        private void Awake()
        {
            // cache the material for performance
            m_Material = GetComponent<Renderer>().material;
        }

        private void Start()
        {
            // find the interact action
            interactAction = InputSystem.actions.FindAction("Interact");
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            // if we are on the client, we need to catch up with the current state of the network variable
            if (IsClient)
            {
                // if we are using a NetworkVariable, we need to catch up with the current state
                if (m_UseNetworkVariableForColor)
                {
                    /* in this case, you need to manually load the initial Color to catch up with the state of the network variable.
                     * This is particularly useful when re-connecting or hot-joining a session
                    */
                    OnClientColorChanged(m_Material.color, m_NetworkedColor.Value);
                    m_NetworkedColor.OnValueChanged += OnClientColorChanged;
                }
            }
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            if (IsClient)
            {
                if (m_UseNetworkVariableForColor)
                {
                    m_NetworkedColor.OnValueChanged -= OnClientColorChanged;
                }
            }
        }

        private void Update()
        {
            if (!IsClient)
            {
                /* note: in this case there's only client-side logic and therefore the scripts returns early.
                 * In a real production scenario, you would have an UpdateManager script running all Updates from a centralized point.
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
            // generate a new random color
            Color32 newColor = MultiplayerUseCasesUtilities.GetRandomColor();

            // change the color on the server
            if (m_UseNetworkVariableForColor)
            {
                m_NetworkedColor.Value = newColor;

                return;
            }

            // if we are not using a NetworkVariable, we need to notify the clients directly
            ClientNotifyColorChangedRpc(newColor);
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void ClientNotifyColorChangedRpc(Color32 newColor)
        {
            // update the color
            m_Material.color = newColor;
        }

        private void OnClientColorChanged(Color32 previousColor, Color32 newColor)
        {
            // update the color
            m_Material.color = newColor;
        }
    }
}