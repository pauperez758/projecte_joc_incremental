using UnityEngine;
using BreakInfinity;
using Extendables;
using TMPro;
using UnityEngine.UI;
using static BreakInfinity.BigDouble;

public class AcceleratorsManager : MonoBehaviour
{
    // TUTORIAL PART 2 MIN 1:26:07

    public GameController game;
    public ParticleUpgradeManager particleUpgradeManager;
    public Accelerator[] accelerators;
    public GameObject[] acceleratorGameObjects;

    public TMP_Text acceleratorBoostText;
    public Image acceleratorBoostButton;

    public TMP_Text quarkCondensationText;
    public Image quarkCondensationButton;

    public GameObject singularityParent; // L'objecte "QuarkSingularity"
    public TMP_Text txtCost;             // El textMeshPro que indica el cost
    public Image singularityBtnImage;    // La Image del botó "Buy"

    public Color BuyGreen = new Color(0.071f, 0.478f, 0.125f);
    public Color BuyRed = new Color(0.701f, 0.286f, 0.365f);

    public string[] acceleratorsNames;

    public BigDouble[] acceleratorBaseCost;
    public BigDouble[] acceleratorCostMult;


    public BigDouble[] acceleratorsBaseCosts;
    public BigDouble AcceleratorCost(int id) => acceleratorBaseCost[id] * BigDouble.Pow(acceleratorCostMult[id], game.data.acceleratorTierMultipliers[id]);

    public BigDouble AcceleratorsUntil10Cost(int id) => AcceleratorCost(id) * 10 - AcceleratorCost(id) * game.data.acceleratorsLevels[id];

    private BigDouble AcceleratorBoost(int id) => game.achievementsManager.achievementBoost *
        Pow(game.data.particleUpgradeBought[1] ? 2.2 : 2, game.data.acceleratorTierMultipliers[id]) * game.data.acceleratorsBoosts[id];

    // Si id != 1 el boost es multiplica per 0.1. Els quarks 2-4 produeixen menys
    public BigDouble AcceleratorProduction(int id)
    {
        var data = game.data;

        double globalSpeed = 3.0; // *** DEBUG. PER FER EL JOC MÉS RÀPID. ELIMINAR DESPRÉS. ***


        return globalSpeed * (id == 1
            ? data.AcceleratorsCount[id - 1] 
            * AcceleratorBoost(id - 1) / (game.spin / 1000)
            : 0.1 
            * data.AcceleratorsCount[id - 1] 
            * AcceleratorBoost(id - 1) / (game.spin / 1000))
                * particleUpgradeManager.ParticleUpgradeBoostCurrently(0)
                * particleUpgradeManager.ParticleUpgradeBoostCurrently(2)
                * (id == 1 || id == 8 ? particleUpgradeManager.ParticleUpgradeBoostCurrently(4) : 1)
                * (id == 2 || id == 7 ? particleUpgradeManager.ParticleUpgradeBoostCurrently(5) : 1)
                * (id == 3 || id == 6 ? particleUpgradeManager.ParticleUpgradeBoostCurrently(8) : 1)
                * (id == 4 || id == 5 ? particleUpgradeManager.ParticleUpgradeBoostCurrently(9) : 1);
    }
    public void AcceleratorBoost()
    {
        var data = game.data;

        for (int i = 0; i < 8; i++)
        {
            var tempBoost = BigDouble.Pow(2, data.boostCount - i);
            data.acceleratorsBoosts[i] = tempBoost < 1 ? 1 : tempBoost;
            if (i == 7) data.acceleratorsBoosts[i] *= data.quarkCondensationBoost; // *= o +=? TODO: COMPROVAR QUINA OPCIÓ ÉS MÉS ÒPTIMA PEL "PACING" DEL JOC
        }
    }

    // Comprova si el jugador ha desbloquejat el següent quark després de comprar un nivell o un boost.
    public void CheckUnlocks(int id)
    {
        var data = game.data;
        if (id < 7 && !data.acceleratorsUnlocked[id + 1])
        {
            if (data.boostCount >= id - 2 && data.AcceleratorsCount[id] > 0)
            {
                data.acceleratorsUnlocked[id + 1] = true;
            }
        }
    }

    public void BuyAccelerators(int id)
    {
        var data = game.data;

        if (data.quark >= AcceleratorCost(id) && data.acceleratorsUnlocked[id])
        {
            data.quark -= AcceleratorCost(id);
            data.acceleratorsLevels[id]++;
            data.AcceleratorsCount[id]++;

            CheckUnlocks(id);

            if (data.acceleratorsLevels[id] >= 10)
            {
                data.acceleratorTierMultipliers[id]++;
                data.acceleratorsLevels[id] = 0;
            }
        }
    }

    public void BuyUntil10Accelerators(int id)
    {
        var data = game.data;

        if (data.quark >= AcceleratorsUntil10Cost(id) && data.acceleratorsUnlocked[id])
        {
            data.quark -= AcceleratorsUntil10Cost(id);
            data.AcceleratorsCount[id] += 10 - data.acceleratorsLevels[id];
            data.acceleratorsLevels[id] = 0;
            data.acceleratorTierMultipliers[id]++;

            CheckUnlocks(id);
        }
    }

    public void BuyMax()
    {
        var data = game.data;

        for (int i = 7; i >= 0; i--)
        {
            if (!data.acceleratorsUnlocked[i]) continue;

            BigDouble costToFinishTen = AcceleratorsUntil10Cost(i);

            if (data.quark >= costToFinishTen)
            {
                data.quark -= costToFinishTen;
                data.AcceleratorsCount[i] += (10 - data.acceleratorsLevels[i]);
                data.acceleratorsLevels[i] = 0;
                data.acceleratorTierMultipliers[i]++;

                BigDouble costPerPack = AcceleratorCost(i);
                BigDouble mult = acceleratorCostMult[i];

                if (data.quark >= costPerPack)
                {
                    long toBuy = (long)Floor(Log(data.quark * (mult - 1) / costPerPack + 1, mult)).ToDouble();
                    if (toBuy > 0)
                    {
                        BigDouble totalBulkCost = costPerPack * (Pow(mult, toBuy) - 1) / (mult - 1);

                        if (totalBulkCost > data.quark) totalBulkCost = data.quark;

                        data.quark -= totalBulkCost;
                        data.AcceleratorsCount[i] += toBuy * 10;
                        data.acceleratorTierMultipliers[i] += toBuy;
                    }
                }
            }

            if (data.quark < 0) data.quark = 0;

            CheckUnlocks(i);
        }
    }

    public void BuyAcceleratorBoost()
    {
        var data = game.data;
        if (data.boostCount <= 4)
        {
            switch ((int)data.boostCount)
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
                if (data.AcceleratorsCount[id] < AcceleratorBoostCost)
                    return;
                BoostReset();
                data.boostCount++;
            }
        }
        else
        {
            // Per a boosts > 4 que requereixen l'últim quark disponible
            if (data.AcceleratorsCount[7] >= AcceleratorBoostCost)
            {
                BoostReset();
                data.boostCount++;
            }
        }
    }

    public void BoostReset()
    {
        var data = game.data;

        data.quark = 10;
        data.AcceleratorsCount = new BigDouble[8];
        data.acceleratorsLevels = new ushort[8];
        data.acceleratorTierMultipliers = new BigDouble[8];
        data.acceleratorsUnlocked = new bool[8];

        if (data.particleUpgradeBought[3])
        {
            for (int i = 0; i < 5; i++)
                data.acceleratorsUnlocked[i] = true;
            data.acceleratorsUnlocked[5] = data.particleUpgradeBought[7];
            data.acceleratorsUnlocked[6] = data.particleUpgradeBought[11];
            data.acceleratorsUnlocked[7] = data.particleUpgradeBought[15];
        }
        data.spinLevels = 0;
        data.quarkCondensationBoost = 1;
        data.highestFirstAccelerators = 0;
    }

    public BigDouble AcceleratorBoostCost => (game.data.boostCount > 4
        ? 20 + (game.data.boostCount - 4) * 15 : 20) - (game.data.particleUpgradeBought[12] ? 9 : 0);


    #region QuarkSingularity
    public BigDouble QuarkSingularityCost => 80 + game.data.quarkSingularities * 60 
        - (game.data.particleUpgradeBought[13] ? 9 : 0);

    public void BuyQuarkSingularity()
    {
        if (game.data.AcceleratorsCount[7] >= QuarkSingularityCost)
        {
            game.data.quarkSingularities++;
            SingularityReset();
        }
    }

    public void SingularityReset()
    {
        var data = game.data;

        BoostReset();
        data.boostCount = 0;

        BlockAccelerators(); // Bloqueja de l'1 al 7

        data.acceleratorsUnlocked[0] = true; // No és necessari però per assegurar que el primer quark està desbloquejat
        CheckUnlocks(0);

    }
    #endregion

    /// <summary>
    /// Bloqueja els quarks 2-7, deixant només el primer desbloquejat.
    /// </summary>
    public void BlockAccelerators()
    {
        for (int i = 1; i < 8; i++) game.data.acceleratorsUnlocked[i] = false;
    }

    #region QuarkCondensation
    private BigDouble quarkCondensationToGet
    {
        get
        {
            var data = game.data;
            var current = data.AcceleratorsCount[0] == 0 ? 0 : Floor(Log10(Abs(data.AcceleratorsCount[0])));
            var highest = data.highestFirstAccelerators == 0 ? 0 : Floor(Log10(Abs(data.highestFirstAccelerators)));

            return current > highest
            ? (current - highest < 10
                ? 1
                : (Pow(Max(Floor(Log10(data.AcceleratorsCount[0])) / 10, 1), 2) + data.quarkCondensationBoost - 1) / data.quarkCondensationBoost)
            : 1;
        }
    }

    public void QuarkCondensation()
    {
        if (quarkCondensationToGet > 1)
        {
            var data = game.data;

            data.highestFirstAccelerators = game.data.AcceleratorsCount[0];
            data.quarkCondensationBoost *= quarkCondensationToGet;

            for (var i = 0; i < 7; i++)
            {
                data.AcceleratorsCount[i] = 0;
                data.acceleratorsLevels[i] = 0;
                data.acceleratorTierMultipliers[i] = 0;
            }
        }
    }
    #endregion
    private void Start()
    {
        acceleratorsNames = new[] { "Primer", "Segon", "Tercer", "Quart", "Cinquè", "Sisè", "Setè", "Vuitè" };
        // *** DEBUG. PER FER EL JOC MÉS RÀPID. ELIMINAR DESPRÉS. ***
        acceleratorBaseCost = new BigDouble[] { 10, 100, 1e4, 1e5, 1e8, 1e11, 1e15, 1e20 };
        acceleratorCostMult = new BigDouble[] { 1e2, 1e3, 1e4, 1e5, 1e6, 1e7, 1e8, 1e10 };

    }

    private void Update()
    {
        var data = game.data;
        AcceleratorBoost();
        data.quark += AcceleratorProduction(1) * Time.deltaTime;
        for (int i = 0; i < 7; i++) data.AcceleratorsCount[i] += AcceleratorProduction(i + 2) * Time.deltaTime;
        if (!data.acceleratorsUnlocked[0]) data.acceleratorsUnlocked[0] = true;


        //UI
        if (game.acceleratorsCanvas.gameObject.activeSelf)
        {
            int activeQrkIndex = data.boostCount >= 4 ? 7 : (int)data.boostCount + 3;
            acceleratorBoostText.text = $"Boost d'accelerador: necessita {AcceleratorBoostCost.Notate(0)} {acceleratorsNames[activeQrkIndex]} Quarks";
            acceleratorBoostButton.color = data.AcceleratorsCount[activeQrkIndex] >= AcceleratorBoostCost ? BuyGreen : BuyRed;

            quarkCondensationText.text = $"Condensació de quarks (x{quarkCondensationToGet.Notate()})";
            quarkCondensationButton.gameObject.SetActive(data.boostCount > 4);

            if (txtCost != null)
                txtCost.text = $"Cost: {QuarkSingularityCost.Notate(0)} 8th Accelerador";

            if (singularityBtnImage != null)
                singularityBtnImage.color = data.AcceleratorsCount[7] >= QuarkSingularityCost ? BuyGreen : BuyRed;

            if (singularityParent != null)
                singularityParent.SetActive(data.boostCount >= 5 || data.quarkSingularities > 0);

            for (int i = 0; i < 8; i++)
            {
                accelerators[i].nameText.text = $"{acceleratorsNames[i]} Accelerador x{AcceleratorBoost(i).Notate(1)}";
                accelerators[i].infoText.text = $"{data.AcceleratorsCount[i].Notate(2)} ({data.acceleratorsLevels[i]})";
                accelerators[i].currentCostText.text = $"Cost: {AcceleratorCost(i).Notate()}";
                accelerators[i].until10CostText.text = $"Fins a 10: Cost : {AcceleratorsUntil10Cost(i).Notate()}";
                accelerators[i].currentCostButton.color = data.quark >= AcceleratorCost(i) ? BuyGreen : BuyRed;
                accelerators[i].until10CostButton.color = data.quark >= AcceleratorsUntil10Cost(i) ? BuyGreen : BuyRed;

                acceleratorGameObjects[i].SetActive(data.acceleratorsUnlocked[i]);
            }
        }
    }
}