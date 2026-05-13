using System;
using System.Collections.Generic;
using BreakInfinity;
using System.Linq;
using UnityEngine;

[Serializable]
public class Data
{
    public BigDouble totalPlayTime;
    public BigDouble quark;
    public BigDouble spinLevels;

    public BigDouble[] AcceleratorsCount = new BigDouble[8];
    public ushort[] acceleratorsLevels = new ushort[8];
    public BigDouble[] acceleratorsBoosts = new BigDouble[8];
    public BigDouble[] acceleratorTierMultipliers = new BigDouble[8];
    public bool[] acceleratorsUnlocked = new bool[8];

    public long boostCount;
    public long quarkSingularities;

    public BigDouble quarkCondensationBoost;
    public BigDouble highestFirstAccelerators;

    public List<bool> achievementUnlocks = new List<bool>(24); // canviar si poso més achievements

    public BigDouble particles;

    public BigDouble particlePoints;
    public BigDouble particlePlaytime;
    public BigDouble particleFastestPlaytime;
    public BigDouble particleGenerationTimer;

    #region Particle Upgrades
    public List<bool> particleUpgradeBought;
    public BigDouble particleUpgradePPGainLevel;
    #endregion
    public Data()
    {
        totalPlayTime = 0;

        //quark = 10; ACTIVAR MÉS TARD
        quark = 1e20; // *** DEBUG. PER FER EL JOC MÉS RÀPID. ELIMINAR DESPRÉS. ***
        spinLevels = 0;

        AcceleratorsCount = new BigDouble[8];
        acceleratorsLevels = new ushort[8];
        acceleratorTierMultipliers = new BigDouble[8];
        acceleratorsBoosts = new BigDouble[8];
        acceleratorsUnlocked = new bool[8];
        highestFirstAccelerators = 0;

        boostCount = 0;
        quarkSingularities = 0;
        quarkCondensationBoost = 1;

        for (int i = 0; i < 8; i++)
        {
            acceleratorsBoosts[i] = 1; // El multiplicador base ha de ser 1
        }

        achievementUnlocks = CreateList<bool>(24);

        particles = 0;
        particlePoints = 0;
        particlePlaytime = 0;
        particleFastestPlaytime = double.MaxValue;
        particleGenerationTimer = 0;

        #region Particle Upgrades
        particleUpgradeBought = CreateList<bool>(16);
        particleUpgradePPGainLevel = 0;
    #endregion
    }

    public static List<T> CreateList<T>(int capacity)
    {
        return Enumerable.Repeat(default(T), capacity).ToList();
    }
}