using System;
using System.Collections;
using UnityEngine;

public class Spaceship : MonoBehaviour
{
    private enum State
    {
        Orbit,
        PrepareMove,
        Move,
    }

    private State state = State.Orbit;

    [SerializeField] private UIResources uiResources;
    
    [SerializeField] public float EnergyMax;
    private float energy = 20;
    public float MoveEnergyToRestore = 0;
    private float energyToRestore = 0;
    [SerializeField] public float TitanMax;
    private float titan = 3;
    [SerializeField] public float CrystalsMax;
    private float crystals;

    [SerializeField] public float HealthMax;
    public float Health;

    public float DefaultTitanPerSecond = 0.05f;
    public float TitanMiningSpeed = 0;
    public float DefaultEnergyPerSecond = 0.2f;
    
    [SerializeField] private float speed;
    private bool clockwise = true;
    
    private Planet currentPlanet;
    private Planet nextPlanet;
    private UIArrow nextPlanetArrow;
    [SerializeField] private GameObject arrowPrefab;
    private float angle;
    private float lastTangentOffset = -1;
    private const float MIN_TANGET_DIST = 1f;
    private float lastDistanceToTarget = -1;
    private Vector2 startMovePoint;

    private Vector3 forward => -transform.right;
    private float forwardSpeed => speed / Mathf.PI / 2;

    private void Start()
    {
        Health = HealthMax;
        float minDist = -1;
        foreach (Planet planet in FindObjectsOfType<Planet>())
        {
            float newDist = Vector3.Distance(planet.transform.position, transform.position);
            if (minDist < 0 || newDist < minDist)
            {
                minDist = newDist;
                currentPlanet = planet;
            }
        }
    }

    private void Update()
    {
        UpdateSpaceship();
        UpdateUI();
        
        switch (state)
        {
            case State.Orbit:
                Orbit();
                break;
            case State.PrepareMove:
                TryStartMovementToPlanet();
                break;
            case State.Move:
                Move();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void UpdateSpaceship()
    {
        UpdateCurrentPlanetDangerLevelEffect();
        
        energy += DefaultEnergyPerSecond * Time.deltaTime;
        if (energy > EnergyMax)
            energy = EnergyMax;
        
        titan += DefaultTitanPerSecond * Time.deltaTime;
        if (titan > TitanMax)
            titan = TitanMax;

        if (currentPlanet != null && currentPlanet.Titan > 0 && !Mathf.Approximately(titan, TitanMax))
        {
            if (currentPlanet.Titan < TitanMiningSpeed * Time.deltaTime)
            {
                titan += currentPlanet.Titan;
                currentPlanet.Titan = 0;
            }
            else
            {
                titan += TitanMiningSpeed * Time.deltaTime;
                currentPlanet.Titan -= TitanMiningSpeed * Time.deltaTime;
            }
        }

        CheckIfDead();
    }

    private void CheckIfDead()
    {
        if (Health <= 0)
        {
            Dead();
        }
    }

    private void Dead()
    {
        Debug.Log("Dead");
    }

    private void UpdateCurrentPlanetDangerLevelEffect()
    {
        if (currentPlanet != null && state != State.Move && currentPlanet.DangerLevel != DangerLevel.None)
        {
            switch (currentPlanet.DangerLevel)
            {
                case DangerLevel.Low:
                    Health -= 1 * Time.deltaTime;
                    break;
                case DangerLevel.High:
                    Health -= 3 * Time.deltaTime;
                    break;
                case DangerLevel.None:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private void UpdateUI()
    {
        uiResources.SetEnegry(energy, EnergyMax);
        uiResources.SetHealth(Health, HealthMax);
        uiResources.SetTitan(titan, TitanMax);
        uiResources.SetCrystals(crystals, CrystalsMax);
    }

    private void Orbit()
    {
        if (currentPlanet == null)
            return;
        
        float r = currentPlanet.GetOrbit();
        float angleSpeed = speed / r * Time.deltaTime * 10;
        angle = (angle + (clockwise ? angleSpeed : -angleSpeed)) % 360;
        Vector3 offset = AngleToOrbitPoint(angle, r);
        transform.position = currentPlanet.transform.position + offset;
        transform.eulerAngles = new Vector3(0, 0, 180 - angle);
    }

    private void TryStartMovementToPlanet()
    {
        if (nextPlanet == null)
        {
            state = State.Orbit;
            return;
        }

        Vector2 lastPosition = transform.position;
        float lastRotation = transform.eulerAngles.z;
        Orbit();

        float dist1 = Vector3.Distance(transform.position, nextPlanet.transform.position);
        float dist2 = Vector3.Distance(transform.position + forward, nextPlanet.transform.position);
        if (dist1 < dist2)
        {
            lastTangentOffset = -1;
            return;
        }

        float newTangetOffset = Mathf.Abs(ClosestDistanceToTanget() - nextPlanet.GetOrbit());

        if (lastTangentOffset > 0 && newTangetOffset < MIN_TANGET_DIST && newTangetOffset > lastTangentOffset)
        {
            transform.position = lastPosition;
            transform.eulerAngles = new Vector3(0, 0, lastRotation);
            state = State.Move;
            lastTangentOffset = -1;
            startMovePoint = transform.position;
            Move();
            StartCoroutine(DrainEnergy());

            return;
        }

        lastTangentOffset = newTangetOffset;
    }

    private void Move()
    {
        Vector2 lastPosition = transform.position;
        float lastRotation = transform.eulerAngles.z;
        
        transform.position += forward * Time.deltaTime * forwardSpeed;

        float dist = Vector3.Distance(transform.position, nextPlanet.transform.position);

        if (lastDistanceToTarget > 0 && dist > lastDistanceToTarget)
        {
            transform.position = lastPosition;
            transform.eulerAngles = new Vector3(0, 0, lastRotation);
            state = State.Orbit;
            if (IsCrossing(currentPlanet.transform.position, nextPlanet.transform.position, startMovePoint, transform.position))
                clockwise = !clockwise;
            currentPlanet = nextPlanet;
            CheckWin();
            nextPlanet = null;
            lastDistanceToTarget = -1;
            angle = OrbitPointToAngle(transform.position);
            
            if (nextPlanetArrow != null)
                Destroy(nextPlanetArrow.gameObject);
            
            return;
        }

        lastDistanceToTarget = dist;
    }

    private void CheckWin()
    {
        if (currentPlanet.IsHomePlanet)
        {
            GameManager.Instance.Win();
        }
    }

    public void MoveToPlanet(Planet planet)
    {
        if (state == State.Move)
            return;
        
        if (planet == currentPlanet)
            return;

        if (planet == nextPlanet)
        {
            nextPlanet = null;
            
            if (nextPlanetArrow != null)
                Destroy(nextPlanetArrow.gameObject);
            state = State.Orbit;
            return;
        }

        float cost = EnergyToMoveToPlanet(planet);
        if (cost > energy)
        {
            uiResources.NotEnoughEnergy();
            return;
        }
        nextPlanet = planet;
        state = State.PrepareMove;

        if (nextPlanetArrow != null)
            Destroy(nextPlanetArrow.gameObject);
        nextPlanetArrow = Instantiate(arrowPrefab).GetComponent<UIArrow>();
        nextPlanetArrow.Initialize(nextPlanet.transform);
    }

    public Planet GetCurrentPlanet()
    {
        return currentPlanet;
    }

    public void TakeDamage(float damage)
    {
        Health -= damage;
    }

    public IEnumerator Repair(float time)
    {
        float healthToRepair = HealthMax - Health;
        float step = healthToRepair / time;
        while (healthToRepair > 0)
        {
            healthToRepair -= step * Time.deltaTime;
            Health += step * Time.deltaTime;
            if (Health > HealthMax)
            {
                Health = HealthMax;
                yield break;
            }

            yield return null;
        }
    }

    public bool TryPayResources(Cost cost)
    {
        if (GameManager.Instance.FreeEverything)
            return true;
        
        if (energy >= cost.Energy && titan >= cost.Titan && crystals >= cost.Crystals)
        {
            energy -= cost.Energy;
            titan -= cost.Titan;
            crystals -= cost.Crystals;
            
            return true;
        }

        return false;
    }
    
    #region Utility

    private IEnumerator DrainEnergy()
    {
        float cost = EnergyToMoveToPlanet(nextPlanet);
        energyToRestore = cost * MoveEnergyToRestore;
        float dist = Vector3.Distance(transform.position, nextPlanet.transform.position);
        float time = dist / forwardSpeed * 0.8f;
        float drainStep = cost / time;

        while (time > 0)
        {
            energy -= drainStep * Time.deltaTime;
            time -= Time.deltaTime;
            yield return null;
        }

        if (!Mathf.Approximately(energyToRestore, 0))
        {
            energy += energyToRestore;
            if (energy > EnergyMax)
                energy = EnergyMax;
            energyToRestore = 0;
        }
    }

    private bool IsCrossing(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {
        Vector2 s1 = p2 - p1;
        Vector2 s2 = p4 - p3;

        float s = (-s1.y * (p1.x - p3.x) + s1.x * (p1.y - p3.y)) / (-s2.x * s1.y + s1.x * s2.y);
        float t = ( s2.x * (p1.y - p3.y) - s2.y * (p1.x - p3.x)) / (-s2.x * s1.y + s1.x * s2.y);

        return s >= 0 && s <= 1 && t >= 0 && t <= 1;
    }

    private Vector2 AngleToOrbitPoint(float alpha, float r)
    {
        return new Vector2(Mathf.Sin(Mathf.Deg2Rad * alpha) * r, Mathf.Cos(Mathf.Deg2Rad * alpha) * r);
    }

    private float OrbitPointToAngle(Vector2 point)
    {
        return Mathf.Atan2(point.x - currentPlanet.transform.position.x, point.y - currentPlanet.transform.position.y) * Mathf.Rad2Deg;
    }

    public float EnergyToMoveToPlanet(Planet planet)
    {
        float distance = Vector3.Distance(currentPlanet.transform.position, planet.transform.position);

        return distance * currentPlanet.Mass / 2;
    }

    private float ClosestDistanceToTanget()
    {
        float x0 = currentPlanet.transform.position.x;
        float y0 = currentPlanet.transform.position.y;
        float x1 = transform.position.x;
        float y1 = transform.position.y;
        float x2 = nextPlanet.transform.position.x;
        float y2 = nextPlanet.transform.position.y;
        float a = x1 - x0;
        float b = y1 - y0;
        float r = currentPlanet.GetOrbit();
        float c = r * r + (x1 - x0) * x0 + (y1 - y0) * y0;
        float d = Mathf.Abs(a * x2 + b * y2 - c) / Mathf.Sqrt(a * a + b * b);

        return d;
    }
    
    #endregion
}