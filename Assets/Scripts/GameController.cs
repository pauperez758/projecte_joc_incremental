using UnityEngine;
using TMPro;
using Extendables;
using BreakInfinity;
using static BreakInfinity.BigDouble;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public Data data;
    public SaveManager saveManager;
    public AchievementManager achievementsManager;
    public AcceleratorsManager acceleratorsManager;
    public ParticleUpgradeManager particleUpgradeManager;
    public ReptesManager reptesManager;

    public Canvas configCanvas;
    public Canvas acceleratorsCanvas;
    public Canvas achievementsCanvas;
    public Canvas particlesCanvas;
    public Canvas challengeCanvas;

    public Canvas particleUpgradesCanvas;

    public Canvas crearUsuariCanvas;
    public Canvas iniciarSessioCanvas;
    public Canvas tancarSessioCanvas;

    public CanvasGroup nonTranscendence;
    public CanvasGroup transcendence;

    public TMP_Text quarksText;
    public TMP_Text quarksPerSecondText;
    public TMP_Text spinText;

    public Image spinCostButton;
    public TMP_Text spinCostText;

    // La Singularity millora l'efectivitat del Spin. Ho tinc en compte abans de calcular l'spinMultiplier
    public BigDouble spinMultiplier
    {
        get
        {
            // L'efecte de singularitat es duplica si s'ha comprat la millora 16 d'acceleradors ("Les Singularitats de Quarks són el doble d'efectives")
            BigDouble singularityEffect = 0.02 * (data.particleUpgradeBought[13] ? 2 : 1);
            return Max(0.5, 0.89 - (data.quarkSingularities * singularityEffect));
        }
    }
    public BigDouble spin => 1000 * Pow(spinMultiplier, data.spinLevels);
    public BigDouble spinCost => 1000 * Pow(10, data.spinLevels);


    public void Start()
    {
        data = SaveSystem.SaveExists("playerData") ? SaveSystem.LoadPlayer<Data>("playerData") : new Data();
        
        achievementsManager.StartAcheivements();
        particleUpgradeManager.StartParticleUpgrades();
        reptesManager.StartChallenges();

        saveManager.EnableAutoSave();
    }


    public void Update()
    {
        data.totalPlayTime += Time.deltaTime;
        data.particlePlaytime += Time.deltaTime;
        CanvasGroupChange(data.quark < double.MaxValue, nonTranscendence);
        CanvasGroupChange(data.quark >= double.MaxValue, transcendence);

        quarksText.text = $"Tens <color=#D142F0>{data.quark.Notate(1)}</color> quarks.";
        quarksPerSecondText.text = $"Estàs aconseguint <color=#D142F0>{acceleratorsManager.AcceleratorProduction(1).Notate()}</color> quarks per segon.";

        spinText.text = $"Spin: {(spin == 1000 ? "1000" : (spin * (((1e3 / Pow(10, spin.Exponent)) * 1e3)) / 10000).ToString("F0"))} {(spin < 100 ? $"/ {(((1e3 / Pow(10, spin.Exponent)) * 1e3)) / 10000}" : "")}";
        spinCostText.text = $"Cost: {spinCost.Notate(0)}";
        spinCostButton.color = data.quark >= spinCost ? acceleratorsManager.BuyGreen : acceleratorsManager.BuyRed;
        
        //SaveTime += Time.deltaTime; Ja no cal guardar cada 15 segons. El sistema nou amb backend hauria de funcionar
        //if (SaveTime < 15) return;
        //SaveTime = 0;
        //SaveSystem.SavePlayer(data, "playerData");
    }

    public void BuySpin()
    {
        if (data.quark >= spinCost)
        {
            data.quark -= spinCost;
            data.spinLevels++;
        }
    }

    public void BuyMaxSpin()
    {
        if (data.quark < spinCost) return;

        BigDouble currentCost = spinCost;
        BigDouble mult = 10;

        BigDouble maxLevels = Floor(Log10(data.quark * (mult - 1) / currentCost + 1));

        if (maxLevels <= 0) return;

        BigDouble totalCost = currentCost * (Pow(mult, maxLevels) - 1) / (mult - 1);

        if (totalCost > data.quark) totalCost = data.quark;

        data.quark -= totalCost;
        data.spinLevels += maxLevels;
    }

    public void Navigate(string location)
    {
        configCanvas.gameObject.SetActive(false);
        acceleratorsCanvas.gameObject.SetActive(false);
        achievementsCanvas.gameObject.SetActive(false);
        particlesCanvas.gameObject.SetActive(false);
        challengeCanvas.gameObject.SetActive(false);

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
            case "challenges":
                challengeCanvas.gameObject.SetActive(true);
                break;
            case "config":
                configCanvas.gameObject.SetActive(true);
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

    public void ConfigNavigate(string location)
    {
        crearUsuariCanvas.gameObject.SetActive(false);
        iniciarSessioCanvas.gameObject.SetActive(false);
        tancarSessioCanvas.gameObject.SetActive(false);

        switch (location)
        {
            case "crearUsuari": crearUsuariCanvas.gameObject.SetActive(true);
                break;
            case "iniciarSessio": iniciarSessioCanvas.gameObject.SetActive(true);
                break;
            case "tancarSessio": tancarSessioCanvas.gameObject.SetActive(true);
                break;
        }
    }

    public void CanvasGroupChange(bool statement, CanvasGroup group)
    {
        group.alpha = statement ? 1 : 0;
        group.blocksRaycasts = group.interactable = statement;
    }


    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    /// <summary>
    /// ELIMINAR DESPRÉS!!!! PER FER DEBUG.
    /// </summary>
    public void DEBUGMULTQUARKSX10(){
                data.quark *= 10;
    }
}