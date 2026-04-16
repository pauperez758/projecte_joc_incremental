using System;
using UnityEngine;
using BreakInfinity;
using Extendables;
using UnityEngine.Rendering;
using TMPro;
using UnityEngine.UI;

public class Quarks : MonoBehaviour 
{
    // TUTORIAL PART 2 MIN 1:26:07

    public GameController game;
    public Quark[] quarks = new Quark[8];
    public GameObject[] quarkGameObjects = new GameObject[8];

    public TMP_Text quarkBoostText;
    public Image quarksBoostButton;

    public Color BuyGreen = new Color(0.071f, 0.478f, 0.125f);
    public Color BuyRed = new Color(0.701f, 0.286f, 0.365f);

    public string[] quarksNames;

    public BigDouble[] quarkBaseCost;
    public BigDouble[] quarkCostMult;

    public BigDouble[] quarksBaseCosts = new BigDouble[8];
    public BigDouble QuarkCost(int id) => quarkBaseCost[id] * BigDouble.Pow(quarkCostMult[id], game.data.quarksBoosts[id]);

    public BigDouble QuarksUntil10Cost(int id) => QuarkCost(id) * 10 - QuarkCost(id) * game.data.quarksLevels[id];

    private BigDouble QuarkBoost(int id) => BigDouble.Pow(2, game.data.quarksBoosts[id]) * game.data.quarkShiftBoosts[id];
    
    // Si id != 1 el boost es multiplica per 0.1. Els quarks 2-4 produeixen menys
    public BigDouble QuarkProduction(int id)
    {
        var data = game.data;

        double globalSpeed = 7.0; // *** DEBUG. PER FER EL JOC MÉS RÀPID. ELIMINAR DESPRÉS. ***


        return globalSpeed * (id == 1 
            ? data.quarksCount[id - 1] * QuarkBoost(id - 1) 
            : 0.1 * data.quarksCount[id - 1] * QuarkBoost(id - 1));
    }
    public void QuarkBoost()
    {
        var data = game.data;

        for (int i = 0; i < 8; i++)
        {
            var tempBoost = BigDouble.Pow(2, data.quarkBoosts - i);
            data.quarkShiftBoosts[i] = tempBoost < 1 ? 1 : tempBoost;
        }
    }

    public void BuyQuarks(int id)
    {
        var data = game.data;

        if (data.quark >= QuarkCost(id) && data.quarksUnlocked[id])
        {
            data.quark -= QuarkCost(id);
            data.quarksLevels[id]++;
            data.quarksCount[id]++;
            if (!data.quarksUnlocked[id + 1] && id < 7 && data.quarkBoosts >= id - 2) data.quarksUnlocked[id + 1] = true;
            if (data.quarksLevels[id] >= 10)
            {
                data.quarksBoosts[id]++;
                data.quarksLevels[id] = 0;
            }
        }
    }

    public void BuyUntil10Quarks(int id)
    {
        var data = game.data;

        if (data.quark >= QuarksUntil10Cost(id) && data.quarksUnlocked[id])
        {
            data.quark -= QuarksUntil10Cost(id);
            data.quarksCount[id] += 10 - data.quarksLevels[id];
            data.quarksLevels[id] = 0;
            data.quarksBoosts[id]++;
            if (id < 7 && !data.quarksUnlocked[id + 1])
            {
                if (!data.quarksUnlocked[id + 1] && id < 7 && data.quarkBoosts >= id - 2) data.quarksUnlocked[id + 1] = true;
            }
        }
    }

    public void BuyQuarkBoost()
    {
        var data = game.data;
        if (data.quarkBoosts <= 4)
        {
            switch ((int)data.quarkBoosts)
            {
                case 0:
                    Boost(3);
                    break;
                case 1:
                    Boost(4);
                    break;
                case 2:
                    Boost(5);
                    break;
                case 3:
                    Boost(6);
                    break;
                case 4:
                    Boost(7);
                    break;
            }
            void Boost(int id)
            {
                if (data.quarksCount[id] < QuarkBoostCost)
                    return;
                BoostReset();
                data.quarkBoosts++;
            }
        }
    }

    public void BoostReset()
    {
        var data = game.data;

        data.quark = 10;
        data.quarksCount = new BigDouble[8];
        data.quarksLevels = new ushort[8];
        data.quarksBoosts = new BigDouble[8];
        data.quarksUnlocked = new bool[8];
    }

    public BigDouble QuarkBoostCost => game.data.quarkBoosts > 4 
        ? 20 + (game.data.quarkBoosts - 4) * 15
        : 20;

    private void Start()
    {
        quarksNames = new[] {"First", "Second", "Third", "Fourth", "Fifth", "Sixth", "Seventh", "Eighth"};
        //quarkBaseCost = new BigDouble[] { 10, 100, 1e4, 1e6, 1e9, 1e13, 1e18, 1e24 };
        //quarkCostMult = new BigDouble[] { 1e3, 1e4, 1e5, 1e6, 1e8, 1e10, 1e12, 1e15 };

        // *** DEBUG. PER FER EL JOC MÉS RÀPID. ELIMINAR DESPRÉS. ***
        quarkBaseCost = new BigDouble[] { 10, 100, 1e4, 1e5, 1e8, 1e11, 1e15, 1e20 };
        quarkCostMult = new BigDouble[] { 1e2, 1e3, 1e4, 1e5, 1e6, 1e7, 1e8, 1e10 };

    }

    private void Update()
    {
        var data = game.data;
        QuarkBoost();
        int activeQrkIndex = data.quarkBoosts >= 4 ? 7 : (int)data.quarkBoosts + 3;

        quarkBoostText.text = $"Quark Boost ({data.quarkBoosts}): requires {QuarkBoostCost} {quarksNames[activeQrkIndex]} Quarks";
        quarksBoostButton.color = data.quarksCount[activeQrkIndex] >= QuarkBoostCost ? BuyGreen : BuyRed;
        if (!data.quarksUnlocked[0]) data.quarksUnlocked[0] = true;

        data.quark += QuarkProduction(1) * Time.deltaTime;
        for (int i = 0; i < 6; i++)
        {
            data.quarksCount[i] += QuarkProduction(i + 2) * Time.deltaTime;
        }

        // De moment i < 4 ja que tinc quatre quarks
        for (int i = 0; i < 8; i++)
        {
            quarks[i].nameText.text = $"{quarksNames[i]} Quark x{QuarkBoost(i).Notate(1)}";
            quarks[i].infoText.text = $"{data.quarksCount[i].Notate(2)} ({data.quarksLevels[i]})";
            quarks[i].currentCostText.text = $"Cost: {QuarkCost(i).Notate()}";
            quarks[i].until10CostText.text = $"Fins a 10: Cost : {QuarksUntil10Cost(i).Notate()}";
            quarks[i].currentCostButton.color = data.quark >= QuarkCost(i) ? BuyGreen : BuyRed;
            quarks[i].until10CostButton.color = data.quark >= QuarksUntil10Cost(i) ? BuyGreen : BuyRed;

            quarkGameObjects[i].SetActive(data.quarksUnlocked[i]);
        }
    }
}
