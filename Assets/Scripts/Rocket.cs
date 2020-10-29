using System.Linq;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [SerializeField] private GameObject destroyEffect;

    private bool launched = false;
    private const float SPEED = 2f;
    private const float ROTATE_SPEED = 100f;
    private const float MIN_WAIT_FOR_TARGET = 1f;
    private Asteroid target;
    private float minWaitForTargetTimer = MIN_WAIT_FOR_TARGET;

    private void Update()
    {
        if (!launched)
            return;

        if (target != null)
        {
            Vector2 from = transform.position;
            Vector2 to = target.transform.position;
            float angle = Mathf.Atan2(from.y - to.y, from.x - to.x) * Mathf.Rad2Deg;
            angle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, angle, ROTATE_SPEED * Time.deltaTime);
            transform.eulerAngles = new Vector3(0, 0, angle);
        }
        else if (minWaitForTargetTimer <= 0)
        {
            target = FindClosestTarget();
            minWaitForTargetTimer = MIN_WAIT_FOR_TARGET;
        }
        else
        {
            minWaitForTargetTimer -= Time.deltaTime;
        }
        
        transform.position += -transform.right * (SPEED * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.GetComponent<Spaceship>() != null)
            return;

        Destroy(Instantiate(destroyEffect, transform.position, transform.rotation), 3f);
        Destroy(gameObject);
    }

    public void Launch()
    {
        launched = true;
        transform.SetParent(null);
        target = FindClosestTarget();
        Destroy(gameObject, 20f);
    }

    private Asteroid FindClosestTarget()
    {
        return FindObjectsOfType<Asteroid>().OrderBy(a => Vector2.Distance(a.transform.position, transform.position)).FirstOrDefault();
    }
}