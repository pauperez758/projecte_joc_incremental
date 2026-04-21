using UnityEngine;
using TMPro;
using Extendables;
using BreakInfinity;
using static BreakInfinity.BigDouble;

public class GameController : MonoBehaviour
{
    public Data data;

    public Quarks quarks;

    public TMP_Text quarksText;
    public TMP_Text quarksPerSecondText;
    public TMP_Text spinText;
    public TMP_Text spinCostText;

    public BigDouble spin => 1000 * Pow(0.89, data.spinLevels);
    public BigDouble spinCost => 1000 * Pow(10, data.spinLevels);


    public void Start()
    {
        data = new Data();
    }

    public void Update()
    {
        quarksText.text = $"Tens <color=#00F5FF>{data.quark.Notate(1)}</color> quarks.";
        quarksPerSecondText.text = $"Estàs aconseguint {quarks.QuarkProduction(1).Notate()} quarks per segon.";

        spinText.text = $"Spin: {(spin == 1000 ? "1000" : (spin * (((1e3 / Pow(10, spin.Exponent)) * 1e3)) / 10000).ToString("F0"))} {(spin < 100 ? $"/ {(((1e3 / Pow(10, spin.Exponent)) * 1e3)) / 10000}" : "")}";
        spinCostText.text = $"Cost: {spinCost.Notate(0)}";
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
