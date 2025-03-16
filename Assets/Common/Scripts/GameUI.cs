using TMPro;
using UnityEngine;

namespace Unity.Netcode.Samples.MultiplayerUseCases.Common
{
    /// <summary>
    /// Manages the UI of the "NetworkVariable vs RPCs" scene
    /// </summary>
    public class GameUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text startupLabel;
        [SerializeField] private TMP_Text controlsLabel;

        private void Start()
        {
            // Hide the controls label by default
            RefreshLabels(NetworkManager.Singleton && (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsServer));

            // Register to the connection event
            NetworkManager.Singleton.OnConnectionEvent += OnConnectionEvent;

            // Register to the server started event
            NetworkManager.Singleton.OnServerStarted += OnServerStarted;
        }

        private void OnDestroy()
        {
            if (NetworkManager.Singleton)
            {
                // Unregister from the connection event
                NetworkManager.Singleton.OnConnectionEvent -= OnConnectionEvent;

                // Unregister from the server started event
                NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
            }
        }

        private void OnServerStarted()
        {
            // Hide the startup label when the server is started
            RefreshLabels(true);
        }

        private void OnConnectionEvent(NetworkManager networkManager, ConnectionEventData connectionEventData)
        {
            if (connectionEventData.EventType == ConnectionEvent.ClientConnected)
            {
                if (NetworkManager.Singleton && NetworkManager.Singleton.IsServer)
                {
                    return; //you don't want to do actions twice when playing as a host
                }

                // Show the controls label when a client is connected
                RefreshLabels(true);
            }
            else if (connectionEventData.EventType == ConnectionEvent.ClientDisconnected)
            {
                // Show the startup label when a client is disconnected
                RefreshLabels(false);
            }
        }

        private void RefreshLabels(bool isConnected)
        {
            // Show the correct label based on the connection status
            startupLabel.gameObject.SetActive(!isConnected);
            controlsLabel.gameObject.SetActive(isConnected);
        }
    }
}