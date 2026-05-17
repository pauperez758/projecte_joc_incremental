using System;
using System.Collections.Generic;
using BreakInfinity;
using System.Linq;
using UnityEngine;
using JetBrains.Annotations;

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

    public List<bool> achievementUnlocks = new List<bool>(16); // canviar si poso més achievements

    public BigDouble particles;

    public BigDouble particlePoints;
    public BigDouble particlePlaytime;
    public BigDouble particleFastestPlaytime;
    public BigDouble particleGenerationTimer;

    #region Particle Upgrades
    public List<bool> particleUpgradeBought;
    public BigDouble particleUpgradePPGainLevel;
    #endregion

    #region challenges
    public List<bool> challengeCompleted;
    public int currentChallenge;
    #endregion
    public Data()
    {
        totalPlayTime = 0;

        quark = 10;

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

        achievementUnlocks = CreateList<bool>(16);

        particles = 0;
        particlePoints = 0;
        particlePlaytime = 0;
        particleFastestPlaytime = double.MaxValue;
        particleGenerationTimer = 0;

        #region Particle Upgrades
        particleUpgradeBought = CreateList<bool>(16);
        particleUpgradePPGainLevel = 0;
        #endregion

        #region challenges
        challengeCompleted = CreateList<bool>(12);
        currentChallenge = 1;
        #endregion
    }

    public static List<T> CreateList<T>(int capacity)
    {
        return Enumerable.Repeat(default(T), capacity).ToList();
    }
}