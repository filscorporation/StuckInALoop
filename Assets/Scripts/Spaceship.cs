using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataManagement;
using UIManagement;
using UnityEngine;

public class Spaceship : DataObject
{
    #region Types

    private enum State
    {
        Orbit,
        PrepareMove,
        Move,
    }

    #endregion

    #region Fields
    
    [SerializeField] private UIResources uiResources;
    [SerializeField] public float EnergyMax;
    [SerializeField] public float TitanMax;
    [SerializeField] public float CrystalsMax;
    [SerializeField] public float HealthMax;
    [SerializeField] public float Speed;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private GameObject canon;
    [SerializeField] private GameObject drillIdle;
    [SerializeField] private GameObject drillWorking;
    [SerializeField] public GameObject Storage;
    [SerializeField] public GameObject Accelerator;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip upgradeSound;

    #endregion

    #region Attributes

    private State state = State.Orbit;
    private float energy = 5;
    private float energyToRestore = 0;
    private float energyToDrain = 0;
    private float titan = 3;
    private float crystals = 0.1f;
    private bool canDrill = false;
    private readonly List<Upgrade> upgradesInProgress = new List<Upgrade>();
    
    private bool clockwise = true;
    
    private Planet currentPlanet;
    private Planet nextPlanet;
    private UIArrow nextPlanetArrow;
    private float angle;
    private float lastTangentOffset = -1;
    private const float MIN_TANGET_DIST = 1f;
    private float lastDistanceToTarget = -1;
    private Vector2 startMovePoint;

    #endregion

    #region Properties

    public bool IsDead { get; private set; } = false;
    
    public float Health { get; private set; }
    public float DefaultTitanPerSecond { get; set; } = 0.05f;
    public float TitanMiningSpeed { get; set; } = 0;
    public float CrystalMiningSpeed { get; set; } = 0;
    public float DefaultEnergyPerSecond { get; set; } = 0.2f;
    public float MoveEnergyToRestore { get; set; } = 0;
    public float TravelCostDiscount { get; set; } = 0;

    public bool CanRecycle { get; set; } = false;

    public bool CanDrill
    {
        get => canDrill;
        set
        {
            canDrill = value;
            drillIdle.SetActive(canDrill);
        }
    }

    private Vector3 forward => -transform.right;
    private float forwardSpeed => Speed / Mathf.PI / 2;

    public Cost Resources => new Cost(energy, titan, crystals, 0);

    #endregion

    private void Start()
    {
        Health = HealthMax;
        float minDist = -1;
        
        if (currentPlanet == null)
        {
            foreach (Planet planet in FindObjectsOfType<Planet>())
            {
                float newDist = Vector3.Distance(planet.transform.position, transform.position);
                if (minDist < 0 || newDist < minDist)
                {
                    minDist = newDist;
                    currentPlanet = planet;
                }
            }
            Orbit();
            FindObjectOfType<CameraController>().FollowInstant();
        }

        if (state != State.Move)
            OnEnterPlanetOrbit();

        if (!Mathf.Approximately(energyToDrain, 0))
            StartCoroutine(DrainEnergy(energyToDrain));
    }

    private void Update()
    {
        if (GameManager.Instance.Pause)
            return;
        
        UpdateSpaceship();
        ProcessUpgrades();
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

        bool drilled = false;
        if (CanDrill)
        {
            if (state != State.Move && currentPlanet != null)
            {
                if (currentPlanet.Titan > 0 && !Mathf.Approximately(titan, TitanMax))
                {
                    drilled = true;
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

                if (currentPlanet.Crystals > 0 && !Mathf.Approximately(crystals, CrystalsMax))
                {
                    drilled = true;
                    if (currentPlanet.Crystals < CrystalMiningSpeed * Time.deltaTime)
                    {
                        crystals += currentPlanet.Crystals;
                        currentPlanet.Crystals = 0;
                    }
                    else
                    {
                        crystals += CrystalMiningSpeed * Time.deltaTime;
                        currentPlanet.Crystals -= CrystalMiningSpeed * Time.deltaTime;
                    }
                }
            }
        }
        
        drillIdle.SetActive(state != State.Move && CanDrill && !drilled);
        drillWorking.SetActive(state != State.Move && CanDrill && drilled);

        CheckIfDead();
    }

    private void ProcessUpgrades()
    {
        List<Upgrade> upgradesToRemove = new List<Upgrade>();
        foreach (Upgrade upgrade in upgradesInProgress)
        {
            upgrade.TimeLeft -= Time.deltaTime;

            if (upgrade.TimeLeft <= 0)
            {
                upgrade.TimeLeft = 0;
                upgrade.Apply(this);
                upgradesToRemove.Add(upgrade);
                GetComponent<AudioSource>().PlayOneShot(upgradeSound);
            }
        }

        foreach (Upgrade upgrade in upgradesToRemove)
        {
            upgradesInProgress.Remove(upgrade);
        }
    }

    private void CheckIfDead()
    {
        if (IsDead)
            return;
        
        if (Health <= 0)
        {
            Dead();
        }
    }

    private void Dead()
    {
        IsDead = true;
        GetComponent<AudioSource>().PlayOneShot(deathSound);
        GameManager.Instance.Lose();
    }

    public void Won()
    {
        GetComponent<AudioSource>().PlayOneShot(winSound);
    }

    public void ApplyUpgrade(Upgrade upgrade)
    {
        if (GameManager.Instance.Cheat)
        {
            upgrade.Apply(this);
            return;
        }
        
        upgrade.TimeLeft = upgrade.Cost.Time;
        upgradesInProgress.Add(upgrade);
    }

    private void UpdateCurrentPlanetDangerLevelEffect()
    {
        if (currentPlanet != null && state != State.Move && currentPlanet.DangerLevel != DangerLevel.None)
        {
            switch (currentPlanet.DangerLevel)
            {
                case DangerLevel.Low:
                    TakeDamage(1 * Time.deltaTime);
                    break;
                case DangerLevel.High:
                    TakeDamage(3 * Time.deltaTime);
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
        float angleSpeed = Speed / r * Time.deltaTime * 10;
        angle = (angle + (clockwise ? angleSpeed : -angleSpeed)) % 360;
        Vector3 offset = AngleToOrbitPoint(angle, r);
        transform.position = currentPlanet.transform.position + offset;
        transform.eulerAngles = new Vector3(0, 0, angle + (clockwise ? -90 : 90));
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
            StartCoroutine(DrainEnergy(EnergyToMoveToPlanet(nextPlanet)));

            OnLeavePlanetOrbit();

            return;
        }

        lastTangentOffset = newTangetOffset;
    }

    private void Move()
    {
        Vector2 lastPosition = transform.position;
        float lastRotation = transform.eulerAngles.z;
        
        transform.position += forward * (Time.deltaTime * forwardSpeed);

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
            
            OnEnterPlanetOrbit();
            
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

    private void OnLeavePlanetOrbit()
    {
        
    }

    private void OnEnterPlanetOrbit()
    {
        drillIdle.transform.localScale = new Vector3(1, clockwise ? -1 : 1, 1);
        drillWorking.transform.localScale = new Vector3(1, clockwise ? -1 : 1, 1);
        Storage.transform.localScale = new Vector3(1, clockwise ? -1 : 1, 1);
    }

    public Planet GetCurrentPlanet()
    {
        return currentPlanet;
    }

    public void TakeDamage(float damage)
    {
        Health -= damage;
        UIManager.Instance.ShowDamageEffect();
    }

    public void Recycle(float amount)
    {
        titan += amount;
        if (titan > TitanMax)
            titan = TitanMax;
    }

    public void EnableCanon()
    {
        canon.SetActive(true);
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
        if (GameManager.Instance.Cheat)
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

    private IEnumerator DrainEnergy(float cost)
    {
        energyToDrain = cost;
        energyToRestore = cost * MoveEnergyToRestore;
        float dist = Vector3.Distance(transform.position, nextPlanet.transform.position);
        float time = dist / forwardSpeed * 0.8f;
        float drainStep = cost / time;

        while (time > 0)
        {
            energyToDrain -= drainStep * Time.deltaTime;
            energy -= drainStep * Time.deltaTime;
            time -= Time.deltaTime;
            yield return null;
        }

        energyToDrain = 0;
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
        return new Vector2(Mathf.Cos(Mathf.Deg2Rad * alpha) * r, Mathf.Sin(Mathf.Deg2Rad * alpha) * r);
    }

    private float OrbitPointToAngle(Vector2 point)
    {
        Vector2 position = currentPlanet.transform.position;
        return Mathf.Atan2(point.y - position.y, point.x - position.x) * Mathf.Rad2Deg;
    }

    public float EnergyToMoveToPlanet(Planet planet)
    {
        float distance = Vector3.Distance(currentPlanet.transform.position, planet.transform.position);
        return distance * currentPlanet.Mass / 2 * (1 - TravelCostDiscount);
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
    
    #region Data
    
    public override IData ToData()
    {
        Transform tr = transform;
        Vector3 position = tr.position;
        
        return new SpaceshipData
        {
            SpaceshipState = (int)state,
            PositionX = position.x,
            PositionY = position.y,
            Angle = tr.eulerAngles.z,
            EnergyMax = EnergyMax,
            Energy = energy,
            MoveEnergyToRestore = MoveEnergyToRestore,
            EnergyToRestore = energyToRestore,
            EnergyToDrain = energyToDrain,
            TitanMax = TitanMax,
            Titan = titan,
            CrystalsMax = CrystalsMax,
            Crystals = crystals,
            HealthMax = HealthMax,
            Health = Health,
            DefaultTitanPerSecond = DefaultTitanPerSecond,
            TitanMiningSpeed = TitanMiningSpeed,
            CrystalMiningSpeed = CrystalMiningSpeed,
            DefaultEnergyPerSecond = DefaultEnergyPerSecond,
            CanRecycle = CanRecycle,
            CanDrill = CanDrill,
            Speed = Speed,
            TravelCostDiscount = TravelCostDiscount,
            Clockwise = clockwise,
            CurrentPlanetIndex = currentPlanet == null ? -1 : currentPlanet.Index,
            NextPlanetIndex = nextPlanet == null ? -1 : nextPlanet.Index,
            UpgradeDatas = Upgrade.Save(),
        };
    }

    [Serializable]
    public class SpaceshipData : IData
    {
        public int Priority => -1;
        
        public int SpaceshipState;
        public float PositionX;
        public float PositionY;
        public float Angle;
        public float EnergyMax;
        public float Energy;
        public float MoveEnergyToRestore;
        public float EnergyToRestore;
        public float EnergyToDrain;
        public float TitanMax;
        public float Titan;
        public float CrystalsMax;
        public float Crystals;
        public float HealthMax;
        public float Health;
        public float DefaultTitanPerSecond;
        public float TitanMiningSpeed;
        public float CrystalMiningSpeed;
        public float DefaultEnergyPerSecond;
        public bool CanRecycle;
        public bool CanDrill;
        public float Speed;
        public float TravelCostDiscount;
        public bool Clockwise;
        public int CurrentPlanetIndex;
        public int NextPlanetIndex;
        public List<UpgradeData> UpgradeDatas;
        
        public DataObject ToObject()
        {
            Spaceship spaceship = GameManager.Instance.Player;

            spaceship.state = (State) SpaceshipState;
            Transform tr = spaceship.transform;
            tr.position = new Vector3(PositionX, PositionY);
            tr.eulerAngles = new Vector3(0, 0, Angle);
            spaceship.EnergyMax = EnergyMax;
            spaceship.energy = Energy;
            spaceship.MoveEnergyToRestore = MoveEnergyToRestore;
            spaceship.energyToRestore = EnergyToRestore;
            spaceship.energyToDrain = EnergyToDrain;
            spaceship.TitanMax = TitanMax;
            spaceship.titan = Titan;
            spaceship.CrystalsMax = CrystalsMax;
            spaceship.crystals = Crystals;
            spaceship.HealthMax = HealthMax;
            spaceship.Health = Health;
            spaceship.DefaultTitanPerSecond = DefaultTitanPerSecond;
            spaceship.TitanMiningSpeed = TitanMiningSpeed;
            spaceship.CrystalMiningSpeed = CrystalMiningSpeed;
            spaceship.DefaultEnergyPerSecond = DefaultEnergyPerSecond;
            spaceship.CanRecycle = CanRecycle;
            spaceship.CanDrill = CanDrill;
            spaceship.Speed = Speed;
            spaceship.TravelCostDiscount = TravelCostDiscount;
            spaceship.clockwise = Clockwise;
            spaceship.currentPlanet = CurrentPlanetIndex == -1 ? null : PlanetGenerator.Instance.GetPlanetByIndex(CurrentPlanetIndex);
            spaceship.nextPlanet = NextPlanetIndex == -1 ? null : PlanetGenerator.Instance.GetPlanetByIndex(NextPlanetIndex);
            
            spaceship.angle = spaceship.OrbitPointToAngle(spaceship.transform.position);
            Upgrade.Load(UpgradeDatas);
            spaceship.upgradesInProgress.AddRange(Upgrade.UpgradeDictionary.Values.Where(u => u.Installed && u.TimeLeft > Mathf.Epsilon));
            foreach (Upgrade upgrade in Upgrade.ActiveUpgrades)
            {
                upgrade.ShowComponent(spaceship);
            }

            spaceship.WasLoaded = true;

            return spaceship;
        }
    }
    
    #endregion
}