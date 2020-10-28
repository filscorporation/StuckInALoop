using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

public struct Cost
{
    public Cost(float energy = 0, float titan = 0, float crystals = 0, float time = 0)
    {
        Energy = energy;
        Titan = titan;
        Crystals = crystals;
        Time = time;
    }

    public float Energy;
    public float Titan;
    public float Crystals;
    public float Time;

    public string RelativeToString(Cost resources)
    {
        const string ok = "white";
        const string not = "red";
        
        StringBuilder sb = new StringBuilder();
        if (!Mathf.Approximately(Energy, 0))
            sb.Append($"<color={(Energy > resources.Energy + Mathf.Epsilon ? not : ok)}>Energy: {Energy}</color>, ");
        if (!Mathf.Approximately(Titan, 0))
            sb.Append($"<color={(Titan > resources.Titan + Mathf.Epsilon ? not : ok)}>Titan: {Titan}</color>, ");
        if (!Mathf.Approximately(Crystals, 0))
            sb.Append($"<color={(Crystals > resources.Crystals + Mathf.Epsilon ? not : ok)}>Crystals: {Crystals}</color>, ");
        if (!Mathf.Approximately(Time, 0))
            sb.Append($"<color={ok}>Time: {Time}</color>, ");
        if (sb.Length == 0)
            return "Nothing";
        sb.Remove(sb.Length - 2, 2);

        return sb.ToString();
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        if (!Mathf.Approximately(Energy, 0))
            sb.Append($"Energy: {Energy}, ");
        if (!Mathf.Approximately(Titan, 0))
            sb.Append($"Titan: {Titan}, ");
        if (!Mathf.Approximately(Crystals, 0))
            sb.Append($"Crystals: {Crystals}, ");
        if (!Mathf.Approximately(Time, 0))
            sb.Append($"Time: {Time}, ");
        if (sb.Length == 0)
            return "Nothing";
        sb.Remove(sb.Length - 2, 2);

        return sb.ToString();
    }
}

[Serializable]
public struct UpgradeData
{
    public UpgradeData(Upgrade upgrade)
    {
        Name = upgrade.Name;
        TimeLeft = upgrade.TimeLeft;
    }
    
    public string Name;
    public float TimeLeft;
}

public abstract class Upgrade
{
    public static readonly Dictionary<string, Upgrade> UpgradeDictionary = new Dictionary<string, Upgrade>();
    public static IEnumerable<Upgrade> ActiveUpgrades => UpgradeDictionary.Values.Where(u => u.Installed && Mathf.Approximately(u.TimeLeft, 0));

    public static void InitializeUpgradeDictionary()
    {
        foreach (Type type in Assembly.GetAssembly(typeof(Upgrade)).GetTypes()
            .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(Upgrade))))
        {
            Upgrade skill = (Upgrade)Activator.CreateInstance(type);
            UpgradeDictionary[skill.Name] = skill;
        }
    }

    public static List<UpgradeData> Save()
    {
        return UpgradeDictionary.Where(p => p.Value.Installed).Select(p => new UpgradeData(p.Value)).ToList();
    }

    public static void Load(List<UpgradeData> upgrades)
    {
        foreach (UpgradeData upgradeData in upgrades)
        {
            UpgradeDictionary[upgradeData.Name].Installed = true;
            UpgradeDictionary[upgradeData.Name].TimeLeft = upgradeData.TimeLeft;
        }
    }
    
    public abstract int Level { get; }
    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract Cost Cost { get; }
    public abstract void Apply(Spaceship player);
    public virtual void ShowComponent(Spaceship player) { }

    public bool Installed = false;
    public float TimeLeft = 0;
}