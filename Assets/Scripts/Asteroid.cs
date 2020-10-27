using System;
using DataManagement;
using UnityEngine;
using Random = UnityEngine.Random;

public class Asteroid : DataObject
{
    [SerializeField] private float maxHealth;
    private float health;
    [SerializeField] private float speed;
    [SerializeField] private float minDamage;
    [SerializeField] private float maxDamage;
    [SerializeField] public float Rarity;
    [SerializeField] private GameObject destroyEffect;
    public Vector3 Direction;
    public int PrefabIndex;
    
    private float rotationSpeed;
    private int rotationDirection;
    
    private void Start()
    {
        health = maxHealth;
        rotationSpeed = Random.Range(speed * 0.5f, speed * 1.5f) / Mathf.PI;
        rotationDirection = Random.Range(0, 1f) >= 0.5f ? -1 : 1;
    }

    private void Update()
    {
        transform.position += Direction * (speed * Time.deltaTime);
        transform.eulerAngles += new Vector3(0, 0, rotationDirection * rotationSpeed * Time.deltaTime * 100);

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

        if (other.gameObject.GetComponent<Bullet>() != null)
        {
            TakeDamage(1f);
            return;
        }
        
        Destroy();
    }

    private void Destroy()
    {
        Destroy(Instantiate(destroyEffect, transform.position, transform.rotation), 3f);
        Destroy(gameObject);
    }

    private void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            GameManager.Instance.Player.Recycle(maxHealth);
            Destroy();
        }
    }
    
    #region Data

    public override IData ToData()
    {
        return new PlanetData
        {
            PrebafIndex = PrefabIndex,
            PositionX = transform.position.x,
            PositionY = transform.position.y,
            DirectionX = Direction.x,
            DirectionY = Direction.y,
            Angle = transform.eulerAngles.z,
            RotationSpeed = rotationSpeed,
            RotationDirection = rotationDirection,
        };
    }

    [Serializable]
    public class PlanetData : IData
    {
        public int Priority => 0;
        
        public int PrebafIndex;
        public float PositionX;
        public float PositionY;
        public float DirectionX;
        public float DirectionY;
        public float Angle;
        public float RotationSpeed;
        public int RotationDirection;
        
        public DataObject ToObject()
        {
            AsteroidsGenerator generator = FindObjectOfType<AsteroidsGenerator>();
            Asteroid asteroid = generator.SpawAsteroid(PrebafIndex, new Vector2(PositionX, PositionY), new Vector2(DirectionX, DirectionY));
            
            asteroid.transform.eulerAngles = new Vector3(0, 0, Angle);
            asteroid.rotationSpeed = RotationSpeed;
            asteroid.rotationDirection = RotationDirection;

            asteroid.WasLoaded = true;
            
            return asteroid;
        }
    }
    
    #endregion
}