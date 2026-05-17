using UnityEngine;
using BreakInfinity;
using static BreakInfinity.BigDouble;
using System;

public class OfflineProductionManager : MonoBehaviour
{
    public GameController game;
    public AcceleratorsManager acceleratorsManager;

    private const string LAST_SAVE_TIME_KEY = "lastSaveTime";
    private const int MAX_OFFLINE_HOURS = 8;

    public void CalculateOfflineProduction()
    {
        if (!PlayerPrefs.HasKey(LAST_SAVE_TIME_KEY)) return;

        string savedTime = PlayerPrefs.GetString(LAST_SAVE_TIME_KEY);

        DateTime lastSave = DateTime.Parse(savedTime, null, System.Globalization.DateTimeStyles.RoundtripKind);
        double secondsOffline = (DateTime.UtcNow - lastSave).TotalSeconds;

        secondsOffline = Math.Min(secondsOffline, MAX_OFFLINE_HOURS * 3600);
        if (secondsOffline < 10) return;

        BigDouble gained = TaylorProduction(secondsOffline);
        game.data.quark += gained;
    }

    private BigDouble TaylorProduction(double t)
    {
        var data = game.data;
        int numAccelerators = 8;

        BigDouble[] boosts = new BigDouble[numAccelerators];
        for (int i = 0; i < numAccelerators; i++)
            boosts[i] = acceleratorsManager.AcceleratorBoost(i);

        BigDouble[] counts = new BigDouble[numAccelerators];
        for (int i = 0; i < numAccelerators; i++)
            counts[i] = data.AcceleratorsCount[i];

        BigDouble spin = game.spin / 1000;

        BigDouble result = 0;
        BigDouble factorial = 1;
        BigDouble tPow = 1;

        BigDouble dQ = counts[0] * boosts[0] / spin;
        result += dQ * t;

        for (int order = 1; order < numAccelerators; order++)
        {
            factorial *= order + 1;
            tPow *= t;

            BigDouble chainFactor = 1;
            for (int j = 0; j < order; j++)
                chainFactor *= 0.11 * boosts[j] / spin;

            BigDouble term = counts[order] * boosts[order] / spin * chainFactor * tPow / factorial;
            result += term;
        }

        return result < 0 ? 0 : result;
    }
}