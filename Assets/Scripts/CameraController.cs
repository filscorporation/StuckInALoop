using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float minSize;
    [SerializeField] public float MaxSize;
    private const float SCROLL_SENSITIVITY = 0.3f;
    private float baseSize = 5;
    
    private void Update()
    {
        float sizeDelta = Input.mouseScrollDelta.y;
        if (!Mathf.Approximately(sizeDelta, 0) && !EventSystem.current.IsPointerOverGameObject())
        {
            baseSize = Mathf.Clamp(baseSize - sizeDelta * SCROLL_SENSITIVITY, minSize, MaxSize);
        }
        
        Vector3 target = GameManager.Instance.Player.GetCurrentPlanet().transform.position;
        Vector3 position = Vector3.Lerp(transform.position, target, Time.deltaTime * 3f);
        transform.position = new Vector3(position.x, position.y, transform.position.z);
        
        Scale();
    }

    private void Scale()
    {
        float ratio = Screen.width / (float)Screen.height;

        Camera.main.orthographicSize = baseSize / ratio * 2;
    }

    public void ZoomIn()
    {
        baseSize = Mathf.Clamp(baseSize - 2 * SCROLL_SENSITIVITY, minSize, MaxSize);
    }

    public void ZoomOut()
    {
        baseSize = Mathf.Clamp(baseSize + 2 * SCROLL_SENSITIVITY, minSize, MaxSize);
    }
}