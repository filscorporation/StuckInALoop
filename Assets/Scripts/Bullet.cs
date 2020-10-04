using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject destroyEffect;
    
    private const float SPEED = 5f;
    
    private void Update()
    {
        transform.position += transform.right * (SPEED * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.GetComponent<Spaceship>() != null)
            return;

        Destroy(Instantiate(destroyEffect, transform.position, transform.rotation), 3f);
        Destroy(gameObject);
    }
}