using UnityEngine;
using UnityEngine.UI;

public class PauseButton : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Sprite pauseIcon;
    [SerializeField] private Sprite unpauseIcon;
    
    private bool isPaused;
    private float timeScale = 1f;

    public void OnClick()
    {
        if (isPaused)
        {
            iconImage.sprite = pauseIcon;
            isPaused = false;
            Time.timeScale = timeScale;
        }
        else
        {
            iconImage.sprite = unpauseIcon;
            isPaused = true;
            timeScale = Time.timeScale;
            Time.timeScale = 0;
        }
    }
}