using System.Collections;
using Unity.Netcode.Samples.MultiplayerUseCases.Common;
using UnityEngine;

namespace Unity.Netcode.Samples.MultiplayerUseCases.RPC
{
    /// <summary>
    /// Manages the mood of a player or NPC
    /// </summary>
    public class MoodManager : NetworkBehaviour
    {
        [SerializeField] private SpeechBubble m_SpeechBubblePrefab;
        private SpeechBubble m_SpeechBubble;

        [SerializeField, Tooltip("The seconds that will elapse between data changes"), Range(2, 5)]
        private float m_SecondsBetweenDataChanges;

        private float m_ElapsedSecondsSinceLastChange;

        // some mood messages
        private readonly string[] s_ChatMessages = new string[]
        {
            "Have a lovely day",
            "Are you pineapple?",
            "Duck you, sucker!",
            "Today I feel like shit!",
            "Wow you're awesome!"
        };

        private void Update()
        {
            // only the owner of the object should send messages
            if (!IsOwner)
            {
                //you don't want to send mood messages from other players, you only want to receive them
                return;
            }

            // increment the timer
            m_ElapsedSecondsSinceLastChange += Time.deltaTime;

            // send a message to all clients
            if (m_ElapsedSecondsSinceLastChange >= m_SecondsBetweenDataChanges)
            {
                // reset the timer
                m_ElapsedSecondsSinceLastChange = 0;

                // send a message to all clients
                ServerMoodMessageReceivedRpc(s_ChatMessages[Random.Range(0, s_ChatMessages.Length)]);
            }
        }

        [Rpc(SendTo.Server)]
        private void ServerMoodMessageReceivedRpc(string message)
        {
            // Here's an example of the type of operation you could do on the server to prevent
            // malicious actions from bad actors.

            // filter bad words
            string redactedMessage = OnServerFilterBadWords(message);

            // send the message to all clients
            ClientMoodMessageReceivedRpc(redactedMessage);
        }

        private string OnServerFilterBadWords(string message)
        {
            // filter bad words
            return MultiplayerUseCasesUtilities.FilterBadWords(message);
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void ClientMoodMessageReceivedRpc(string message)
        {
            // show the message in a speech bubble
            if (!m_SpeechBubble)
            {
                // create the speech bubble
                m_SpeechBubble = Instantiate(m_SpeechBubblePrefab.gameObject, Vector3.zero, Quaternion.Euler(new Vector3(45, 0, 0))).GetComponent<SpeechBubble>();

                // keep the speech bubble above the player's head
                var positionOffsetKeeper = m_SpeechBubble.gameObject.AddComponent<PositionOffsetKeeper>();

                // initialize the position offset keeper
                positionOffsetKeeper.Initialize(transform, new Vector3(0, 3, 0));
            }

            // show the message
            m_SpeechBubble.Setup(message);

            // hide the message after a while
            StartCoroutine(OnClientHideMessage());
        }

        private IEnumerator OnClientHideMessage()
        {
            // hide the message after a while
            yield return new WaitForSeconds(1);

            // hide the message
            m_SpeechBubble.Hide();
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            if (m_SpeechBubble)
            {
                // hide the message
                m_SpeechBubble.Hide();

                // destroy the speech bubble
                Destroy(m_SpeechBubble.gameObject);
            }
        }
    }
}