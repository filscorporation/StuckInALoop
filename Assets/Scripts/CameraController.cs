using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float minSize;
    [SerializeField] private float maxSize;
    private const float SCROLL_SENSITIVITY = 0.3f;
    
    private void Update()
    {
        float sizeDelta = Input.mouseScrollDelta.y;
        if (!Mathf.Approximately(sizeDelta, 0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - sizeDelta * SCROLL_SENSITIVITY, minSize, maxSize);
        }
        
        Vector3 target = GameManager.Instance.Player.GetCurrentPlanet().transform.position;
        Vector3 position = Vector3.Lerp(transform.position, target, Time.deltaTime * 3f);
        transform.position = new Vector3(position.x, position.y, transform.position.z);
    }
}