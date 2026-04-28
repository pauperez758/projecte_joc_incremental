using UnityEngine;
using TMPro;
using Extendables;
using BreakInfinity;
using static BreakInfinity.BigDouble;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public Data data;

    public Quarks quarks;

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


    public void Awake()
    {
        data = SaveSystem.SaveExists("playerData") ? SaveSystem.LoadPlayer<Data>("playerData") : new Data();
    }


    public void Update()
    {
        quarksText.text = $"Tens <color=#00F5FF>{data.quark.Notate(1)}</color> quarks.";
        quarksPerSecondText.text = $"Estàs aconseguint {quarks.QuarkProduction(1).Notate()} quarks per segon.";

        spinText.text = $"Spin: {(spin == 1000 ? "1000" : (spin * (((1e3 / Pow(10, spin.Exponent)) * 1e3)) / 10000).ToString("F0"))} {(spin < 100 ? $"/ {(((1e3 / Pow(10, spin.Exponent)) * 1e3)) / 10000}" : "")}";
        spinCostText.text = $"Cost: {spinCost.Notate(0)}";
        spinCostButton.color = data.quark >= spinCost ? quarks.BuyGreen : quarks.BuyRed;
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
}