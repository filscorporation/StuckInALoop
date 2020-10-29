using UnityEngine;

public class SpaceshipRocketLauncher : MonoBehaviour
{
    [SerializeField] private GameObject rocketPrefab;
    [SerializeField] private Transform rocketPoint;
    [SerializeField] private AudioClip shootSound;
    
    public Cost CostPerShot = new Cost(2, 1, 0, 0);
    public float ReloadTime = 2f;
    private float cooldown = -1;
    private Rocket rocket;

    private void Update()
    {
        if (Mathf.Approximately(Time.timeScale, 0))
            return;

        if (cooldown >= 0)
        {
            cooldown -= Time.deltaTime;
        }
        
        if (cooldown < 0 && rocket == null)
        {
            rocket = Instantiate(rocketPrefab, rocketPoint.position, rocketPoint.rotation).GetComponent<Rocket>();
            rocket.transform.SetParent(transform);
        }
        
        if (cooldown < 0 && Input.GetKey(KeyCode.Q))
        {
            if (GameManager.Instance.Player.TryPayResources(CostPerShot))
            {
                Shoot();
            }
        }
    }

    private void Shoot()
    {
        GetComponent<AudioSource>().PlayOneShot(shootSound);
        cooldown = ReloadTime;
        rocket.Launch();
        rocket = null;
    }
}