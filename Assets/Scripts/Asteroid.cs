using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Asteroid : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float minDamage;
    [SerializeField] private float maxDamage;
    [SerializeField] public float Rarity;
    [SerializeField] private GameObject destroyEffect;
    public Vector3 Direction;

    private float rotationSpeed;
    private int rotationDirection;
    
    private void Start()
    {
        rotationSpeed = Random.Range(speed * 0.5f, speed * 1.5f) / Mathf.PI;
        rotationDirection = Random.Range(0, 1f) >= 0.5f ? -1 : 1;
    }

    private void Update()
    {
        transform.position += Direction * (speed * Time.deltaTime);
        transform.eulerAngles += new Vector3(0, 0, rotationDirection * rotationSpeed);

        Vector2 pos = Camera.main.WorldToScreenPoint(transform.position);
        if (pos.x < -Screen.width || pos.x > 2 * Screen.width || pos.y < -Screen.height || pos.y > 2 * Screen.height)
        {
            Destroy();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Spaceship spaceship = other.gameObject.GetComponent<Spaceship>();
        if (spaceship != null)
        {
            spaceship.TakeDamage(Random.Range(minDamage, maxDamage));
        }
        Destroy();
    }

    private void Destroy()
    {
        Destroy(Instantiate(destroyEffect, transform.position, transform.rotation), 3f);
        Destroy(gameObject);
    }
}