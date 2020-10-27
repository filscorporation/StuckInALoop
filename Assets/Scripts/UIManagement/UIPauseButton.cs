using UnityEngine;
using UnityEngine.UI;

namespace UIManagement
{
    public class UIPauseButton : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private Sprite pauseIcon;
        [SerializeField] private Sprite unpauseIcon;

        private bool isPaused = false;

        private void Update()
        {
            if (isPaused != GameManager.Instance.Pause)
            {
                isPaused = GameManager.Instance.Pause;
                if (isPaused)
                {
                    iconImage.sprite = unpauseIcon;
                }
                else
                {
                    iconImage.sprite = pauseIcon;
                }
            }
        }

        public void OnClick()
        {
            if (GameManager.Instance.Pause)
            {
                iconImage.sprite = pauseIcon;
                GameManager.Instance.Pause = false;
            }
            else
            {
                iconImage.sprite = unpauseIcon;
                GameManager.Instance.Pause = true;
            }
        }
    }
}