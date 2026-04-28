using System;
using BreakInfinity;

[Serializable]
public class Data
{
    public BigDouble quark;
    public BigDouble spinLevels;

    public BigDouble[] quarksCount = new BigDouble[8];
    public ushort[] quarksLevels = new ushort[8];
    public BigDouble[] quarkBoosts = new BigDouble[8];
    public BigDouble[] quarkTierMultipliers = new BigDouble[8];
    public bool[] quarksUnlocked = new bool[8];

    public long boostCount;
    public long quarkSingularities;

    public BigDouble quarkCondensationBoost;
    public BigDouble highestFirstQuarks;
    public Data()
    {
        //quark = 10; ACTIVAR MÉS TARD
        quark = 1e20; // *** DEBUG. PER FER EL JOC MÉS RÀPID. ELIMINAR DESPRÉS. ***
        spinLevels = 0;

        quarksCount = new BigDouble[8];
        quarksLevels = new ushort[8];
        quarkTierMultipliers = new BigDouble[8];
        quarkBoosts = new BigDouble[8];
        quarksUnlocked = new bool[8];
        highestFirstQuarks = 0;

        boostCount = 0;
        quarkSingularities = 0;
        quarkCondensationBoost = 1;

        for (int i = 0; i < 8; i++)
        {
            quarkBoosts[i] = 1; // El multiplicador base ha de ser 1
        }
    }
}