using System;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.Netcode.Samples.MultiplayerUseCases.SelectionScreen
{
    [Serializable]
    internal struct SelectableScene
    {
        [SerializeField] internal string SceneName;
        [SerializeField] internal string DisplayName;
        [SerializeField] internal Texture2D Image;
    }

    /// <summary>
    /// An UI that allows players to pick a scene to load
    /// </summary>
    internal class SceneSelectionUI : MonoBehaviour
    {
        [SerializeField] private SelectableScene[] m_Scenes;
        [SerializeField] private GridLayoutGroup m_Container;
        [SerializeField] private SceneSelectionElement m_SceneUIPrefab;

        private void OnEnable()
        {
            // Setup the UI
            Setup();
        }

        private void Setup()
        {
            // Clear the container
            DestroyAllChildrenOf(m_Container.transform);

            // Create the scene UI elements
            foreach (var scene in m_Scenes)
            {
                // Instantiate the scene UI prefab
                SceneSelectionElement sceneUI = Instantiate(m_SceneUIPrefab, m_Container.transform);

                // Setup the scene UI
                sceneUI.Setup(scene);
            }
        }

        private static void DestroyAllChildrenOf(Transform t)
        {
            int childrenToRemove = t.childCount;

            // Remove all children
            for (int i = childrenToRemove - 1; i >= 0; i--)
            {
                GameObject.Destroy(t.GetChild(i).gameObject);
            }
        }
    }
}