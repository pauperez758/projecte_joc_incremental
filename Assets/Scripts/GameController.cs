using UnityEngine;
using TMPro;
using Extendables;

public class GameController : MonoBehaviour
{
    public Data data;

    public Quarks quarks;

    public TMP_Text quarksText;
    public TMP_Text quarksPerSecondText;

    public void Start()
    {
        data = new Data();
    }

    public void Update()
    {
        quarksText.text = $"Tens <color=#00F5FF>{data.quark.Notate(1)}</color> quarks.";
        quarksPerSecondText.text = $"Estàs aconseguint {quarks.QuarkProduction(1).Notate()} quarks per segon.";
    }
}
