using UnityEngine;
using TMPro;
using Extendables;
using BreakInfinity;
using static BreakInfinity.BigDouble;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public Data data;
    public AchievementManager achievementsManager;
    public AcceleratorsManager acceleratorsManager;
    public ParticleUpgradeManager particleUpgradeManager;

    public Canvas acceleratorsCanvas;
    public Canvas achievementsCanvas;
    public Canvas particlesCanvas;

    public Canvas particleUpgradesCanvas;

    public CanvasGroup nonTranscendence;
    public CanvasGroup transcendence;

    public TMP_Text quarksText;
    public TMP_Text quarksPerSecondText;
    public TMP_Text spinText;

    public Image spinCostButton;
    public TMP_Text spinCostText;

    // La Singularity millora l'efectivitat del Spin (com les Galaxies d'AD)
    public BigDouble spinMultiplier => Max(0.5, 0.89 - (data.quarkSingularities * 0.02));
    public BigDouble spin => 1000 * Pow(spinMultiplier, data.spinLevels);
    public BigDouble spinCost => 1000 * Pow(10, data.spinLevels);

    public float SaveTime;


    public void Start()
    {
        data = SaveSystem.SaveExists("playerData") ? SaveSystem.LoadPlayer<Data>("playerData") : new Data();
        achievementsManager.StartAcheivements();

        particleUpgradeManager.StartParticleUpgrades();
    }


    public void Update()
    {
        data.totalPlayTime += Time.deltaTime;
        data.particlePlaytime += Time.deltaTime;
        CanvasGroupChange(data.quark < double.MaxValue, nonTranscendence);
        CanvasGroupChange(data.quark >= double.MaxValue, transcendence);

        quarksText.text = $"Tens <color=#00F5FF>{data.quark.Notate(1)}</color> quarks.";
        quarksPerSecondText.text = $"Estàs aconseguint {acceleratorsManager.AcceleratorProduction(1).Notate()} quarks per segon.";

        spinText.text = $"Spin: {(spin == 1000 ? "1000" : (spin * (((1e3 / Pow(10, spin.Exponent)) * 1e3)) / 10000).ToString("F0"))} {(spin < 100 ? $"/ {(((1e3 / Pow(10, spin.Exponent)) * 1e3)) / 10000}" : "")}";
        spinCostText.text = $"Cost: {spinCost.Notate(0)}";
        spinCostButton.color = data.quark >= spinCost ? acceleratorsManager.BuyGreen : acceleratorsManager.BuyRed;
        SaveTime += Time.deltaTime;

        if (SaveTime < 15) return;
        SaveTime = 0;
        SaveSystem.SavePlayer(data, "playerData");
    }

    public void BuySpin()
    {
        if (data.quark >= spinCost)
        {
            data.quark -= spinCost;
            data.spinLevels++;
        }
    }

    //TODO: BUYMAXSPIN
    public void BuyMaxSpin()
    {

    }

    public void Navigate(string location)
    {
        acceleratorsCanvas.gameObject.SetActive(false);
        achievementsCanvas.gameObject.SetActive(false);
        particlesCanvas.gameObject.SetActive(false);

        switch (location)
        {
            case "quarks":
                acceleratorsCanvas.gameObject.SetActive(true);
                break;
            case "achievements":
                achievementsCanvas.gameObject.SetActive(true);
                break;
            case "particles":
                particlesCanvas.gameObject.SetActive(true);
                break;
        }
    }

    public void ParticleNavigate(string location)
    {
        particleUpgradesCanvas.gameObject.SetActive(false);

        switch (location)
        {
            case "particleUpgrades": particleUpgradesCanvas.gameObject.SetActive(true);
                break;
        }
    }

    public void CanvasGroupChange(bool statement, CanvasGroup group)
    {
        group.alpha = statement ? 1 : 0;
        group.blocksRaycasts = group.interactable = statement;
    }
}