using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Unity.Netcode.Samples.MultiplayerUseCases.SelectionScreen
{
    /// <summary>
    /// Allows to select a scene in the SelectionScene
    /// </summary>
    internal class SceneSelectionElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Button m_SceneButton;
        [SerializeField] private TMP_Text m_TitleLabel;
        [SerializeField] private Image m_ContrastImage;
        [SerializeField] private Image m_OutlineImage;
        [SerializeField] private Color m_OutlineColor;
        [SerializeField] private Color m_OutlineHighlightColor;

        internal void Setup(SelectableScene selectableScene)
        {
            // Set the button click event
            m_SceneButton.onClick.RemoveAllListeners();
            m_SceneButton.onClick.AddListener(() => OnClick(selectableScene.SceneName));
            m_TitleLabel.text = selectableScene.DisplayName;

            if (selectableScene.Image)
            {
                // Create a sprite from the texture
                m_SceneButton.image.sprite = Sprite.Create(selectableScene.Image, new Rect(0, 0, selectableScene.Image.width, selectableScene.Image.height), new Vector2(0.5f, 0.5f));
            }

            // Disable the overlay elements
            EnableOverlayElements(false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            // Enable the overlay elements
            EnableOverlayElements(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // Disable the overlay elements
            EnableOverlayElements(false);
        }

        private void EnableOverlayElements(bool enable)
        {
            // Enable or disable the overlay elements
            m_ContrastImage.gameObject.SetActive(enable);
            m_TitleLabel.gameObject.SetActive(enable);
            m_OutlineImage.color = enable ? m_OutlineHighlightColor : m_OutlineColor;
        }

        private void OnClick(string sceneName)
        {
            // Load the scene
            LoadScene(sceneName);
        }

        private void LoadScene(string sceneName)
        {
            // Load the scene
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
    }
}