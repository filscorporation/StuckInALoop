using UnityEngine;

public class UIArrow : MonoBehaviour
{
    private Transform target;
    private float timePassed = 0;

    private void Update()
    {
        transform.position = Camera.main.WorldToScreenPoint(target.position + new Vector3(0, 2 + 0.3f * Mathf.Sin(2 * timePassed)));
        
        timePassed += Time.deltaTime;
    }

    public void Initialize(Transform _target)
    {
        transform.SetParent(FindObjectOfType<Canvas>().transform);
        transform.SetAsFirstSibling();
        target = _target;
    }
}