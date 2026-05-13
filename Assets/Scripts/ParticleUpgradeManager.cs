using BreakInfinity;
using Extendables;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering.Universal;
using UnityEngine;
using static BreakInfinity.BigDouble;

public class ParticleUpgradeManager : MonoBehaviour
{
    public GameController game;

    public GameObject particlePanel;
    public List<ParticleUpgrade> particleUpgrade;

    public string[] particleNames;
    public BigDouble[] particleUpgradeCosts;

    public Color grey;
    public Color green;
    public Color skyBlue;

    public BigDouble particleUpgradePPGainCost => 10 * Pow(10, game.data.particleUpgradePPGainLevel);
    public BigDouble particleUpgradePPGainBoost => Pow(2, game.data.particleUpgradePPGainLevel);

    public void StartParticleUpgrades()
    {
        var data = game.data;

        try
        {
            var temp = data.particleUpgradeBought.Count;
        }
        catch
        {
            data.achievementUnlocks = Data.CreateList<bool>(16);
        }

        foreach (var upgrade in particlePanel.gameObject.GetComponentsInChildren<ParticleUpgrade>())
            particleUpgrade.Add(upgrade);
        green = new Color(0.262f, 0.627f, 0.278f);
        grey = new Color(0.149f, 0.196f, 0.219f);
        skyBlue = new Color(0.529f, 0.808f, 0.922f);

        particleNames = new[]
        {

            "Els acceleradors normals reben un multiplicador basat en el temps jugat \nActualment: ",
            "Augmenta el multiplicador per comprar 10 acceleradors\n2x 2.2x",
            "Els acceleradors normals reben un multiplicador basat en el temps passat en la transcendència actual\nActualment: ",
            "Comences amb el 5è accelerador desbloquejat",
            "El primer i vuitè accelerador reben un multiplicador basat en l'estadística de transcendències\nActualment: ",
            "El segon i setè accelerador reben un multiplicador basat en l'estadística de transcendències\nActualment: ",
            "Multiplicador pel primer accelerador basat en els punts de transcendència no gastats\nActualment: ",
            "Comences amb el 6è accelerador desbloquejat",
            "El tercer i sisè accelerador reben un multiplicador basat en l'estadística de transcendències\nActualment: ",
            "El quart i cinquè accelerador reben un multiplicador basat en l'estadística de transcendències\nActualment: ",
            "Augmenta el multiplicador del boost d'acceleració\n2x 2.5x",
            "Comences amb el 7è accelerador desbloquejat",
            "Redueix en 9 el nombre d'acceleradors necessaris per als boosts d'acceleració i les Singularitats de Quarks",
            "Les Singularitats de Quarks són el doble d'efectives",
            "Generació de punts de transcendència basada en la transcendència més ràpida\nActualment: ",
            "Comences amb el 8è accelerador desbloquejat i una Singularitat de Quarks",

            "Multiplica els punts de partícula de totes les fonts per 2\nActualment: " // No l'inclueixo aquí perquè és un upgrade de partícules, no d'acceleradors
        };

        particleUpgradeCosts = new BigDouble[]
        {
            1, 1, 3, 20, 1, 1, 5, 40, 1, 1, 7, 80, 1, 2, 10, 500
        };
    }
    public void Update()
    {
        var data = game.data;
        if (game.particleUpgradesCanvas.gameObject.activeSelf)
        {
            for (int i = 0; i < 17; i++)
            {
                switch (i)
                {
                    case 0:
                    case 2:
                    case 4:
                    case 5:                    
                    case 6:
                    case 8:
                    case 9:
                    case 16:
                        SetCurrently(i);
                        break;
                    case 14:
                        particleUpgrade[i].description.text =
                    $"{particleNames[i]}{ParticleUpgradeBoostCurrently(i).ToTimeFormat()}x" +
                    $"\nCost: {(i < 16 ? particleUpgradeCosts[i] : particleUpgradePPGainCost)}";
                        break;
                    default:
                        particleUpgrade[i].description.text = $"{particleNames[i]}\nCost: {particleUpgradeCosts[i]}";
                        break;
                }
                #region LÒGICA COLORS
                if (i == 16)
                {
                    if (data.particlePoints >= particleUpgradePPGainCost)
                        particleUpgrade[i].image.color = skyBlue;
                    else
                        particleUpgrade[i].image.color = Color.white;
                }
                else
                {
                    bool unlocked = ParticleUpgradeUnlocked(i);
                    bool bought = data.particleUpgradeBought[i];
                    bool canAfford = data.particlePoints >= particleUpgradeCosts[i];

                    if (!unlocked)
                    {
                        particleUpgrade[i].image.color = grey;
                    }
                    else if (bought)
                    {
                        particleUpgrade[i].image.color = green;
                    }
                    else if (canAfford)
                    {
                        particleUpgrade[i].image.color = skyBlue;
                    }
                    else
                    {
                        particleUpgrade[i].image.color = Color.white;
                    }
                }
                #endregion
            }

            void SetCurrently(int id)
            {
                particleUpgrade[id].description.text =
                    $"{particleNames[id]}x{ParticleUpgradeBoostCurrently(id).Notate(2)}" +
                    $"\nCost: {( id < 16 ? particleUpgradeCosts[id] : particleUpgradePPGainCost).Notate(2)}";
            }
        }
    }

    public void BuyParticleUpgrade(int id)
    { 
        var data = game.data;
        if (id == 16)
        {
            if (data.particlePoints >= particleUpgradePPGainCost)
            {
                data.particlePoints -= particleUpgradePPGainCost;
                data.particleUpgradePPGainLevel += 1;
            }
            return;
        }

        if (ParticleUpgradeUnlocked(id) && data.particlePoints >= particleUpgradeCosts[id])
        {
            if (!data.particleUpgradeBought[id])
            {
                data.particleUpgradeBought[id] = true;
                data.particlePoints -= particleUpgradeCosts[id];
            }
        }
    }

    public BigDouble ParticleUpgradeBoostCurrently(int id)
    {
        var data = game.data;
        if (id == 16) return particleUpgradePPGainBoost;
        if (!data.particleUpgradeBought[id]) return 1;
        switch (id)
        {
            case 0: return Log(data.totalPlayTime + 1, 25);
            case 2: return Log10(data.particlePlaytime + 1);
            case 4: return data.particles / 5 + 1;
            case 5: return data.particles / 5 + 1;
            case 6: return Pow(data.particlePoints, 1.5) + 1;
            case 8: return data.particles / 5 + 1;
            case 9: return data.particles / 5 + 1;
            case 14: return data.particleFastestPlaytime * 10;
        }

        return 0;
    }
        
    public bool ParticleUpgradeUnlocked(int id)
    {
        if (id == 16) return true;

        if (id == 0) return true;

        if (game.data.particleUpgradeBought[id]) return true;

        int cols = 4;
        int row = id / cols;
        int col = id % cols;

        if (row > 0 && game.data.particleUpgradeBought[id - 4]) return true;

        // Avall
        if (row < 3 && game.data.particleUpgradeBought[id + 4]) return true;

        // Esquerra
        if (col > 0 && game.data.particleUpgradeBought[id - 1]) return true;

        // Dreta
        if (col < 3 && game.data.particleUpgradeBought[id + 1]) return true;

        return false;
    }
}
