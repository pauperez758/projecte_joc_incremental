using System;
using BreakInfinity;

[Serializable]
public class Data
{
    public BigDouble quark;
    public BigDouble spinLevels;

    public BigDouble[] quarksCount = new BigDouble[8];
    public ushort[] quarksLevels = new ushort[8];
    public BigDouble[] quarkShiftBoosts = new BigDouble[8];
    public BigDouble[] quarksBoosts = new BigDouble[8];
    public bool[] quarksUnlocked = new bool[8];

    public long quarkBoosts;
    public Data()
    {
        quark = 10;
        spinLevels = 0;

        quarksCount = new BigDouble[8];
        quarksLevels = new ushort[8];
        quarksBoosts = new BigDouble[8];
        quarkShiftBoosts = new BigDouble[8];
        quarksUnlocked = new bool[8];

        quarkBoosts = 0;

        for (int i = 0; i < 8; i++)
        {
            quarkShiftBoosts[i] = 1; // El multiplicador base ha de ser 1
        }
    }
}
