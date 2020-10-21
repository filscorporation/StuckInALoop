using UnityEngine;

public class SpaceshipCanon : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletPoint;
    [SerializeField] private AudioClip shootSound;
    
    public float EnergyPerShot = 1f;
    public float ReloadTime = 0.5f;
    private float cooldown = -1;

    private void Update()
    {
        if (Mathf.Approximately(Time.timeScale, 0))
            return;

        Vector2 mp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.eulerAngles = new Vector3(0, 0,
            Mathf.Atan2(transform.position.y - mp.y, transform.position.x - mp.x) * Mathf.Rad2Deg + 180);

        if (cooldown >= 0)
        {
            cooldown -= Time.deltaTime;
        }
        
        if (cooldown < 0 && Input.GetMouseButton(1))
        {
            if (GameManager.Instance.Player.TryPayResources(new Cost(EnergyPerShot, 0, 0, 0)))
            {
                Shoot();
            }
        }
    }

    private void Shoot()
    {
        GetComponent<AudioSource>().PlayOneShot(shootSound);
        cooldown = ReloadTime;
        Instantiate(bulletPrefab, bulletPoint.position, bulletPoint.rotation);
    }
}