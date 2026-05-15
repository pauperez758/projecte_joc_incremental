using BreakInfinity;
using Extendables;
using TMPro;
using UnityEngine;

//TUTORIAL EP.10 1:15:43
public class ParticleManager : MonoBehaviour
{
    public GameController game;
    public ParticleUpgradeManager particleUpgradeManager;
    public Animator transIn;
    public Animator transOut;

    public TMP_Text[] particlePointsText = new TMP_Text[2];

    public void Update()
    {
        var data = game.data;

        foreach (var pText in particlePointsText)
        {
            pText.text = $"Tens <color=#87CEEB>{data.particlePoints.Notate(2)}</color> partícules";
        }

        if (data.particleUpgradeBought[14])
        {
            data.particleGenerationTimer += Time.deltaTime;
            if (data.particleGenerationTimer >= particleUpgradeManager.ParticleUpgradeBoostCurrently(14))
            {
                data.particleGenerationTimer = 0;
                data.particlePoints += particleUpgradeManager.particleUpgradePPGainBoost;
            }
        }
    }
    public void Transcendence()
    {
        transOut.transform.localScale = Vector3.one;
        transOut.Play("TranscendenceOut", 0, 0);
        transIn.Play("TranscendenceIn", 0, 0);
        Invoke(nameof (TranscendenceReset), 1f);
        game.data.particlePoints += particleUpgradeManager.particleUpgradePPGainBoost;
    }

    public void TranscendenceReset()
    {
        var data = game.data;

        data.quark = 1e20; // *** DEBUG. PER FER EL JOC MÉS RÀPID. ELIMINAR DESPRÉS. ***
        data.spinLevels = 0;
        data.AcceleratorsCount = new BigDouble[8];
        data.acceleratorsLevels = new ushort[8];
        data.acceleratorTierMultipliers = new BigDouble[8];
        data.acceleratorsBoosts = new BigDouble[8];
        data.acceleratorsUnlocked = new bool[8];
        
        if (data.particleUpgradeBought[3])
        {
            for (int i = 0; i < 5; i++)
                data.acceleratorsUnlocked[i] = true;
            data.acceleratorsUnlocked[5] = data.particleUpgradeBought[7];
            data.acceleratorsUnlocked[6] = data.particleUpgradeBought[11];
            data.acceleratorsUnlocked[7] = data.particleUpgradeBought[15];
        }
        data.highestFirstAccelerators = 0;
        data.boostCount = 0;
        data.quarkSingularities = data.particleUpgradeBought[15] ? 1 : 0; // Si la millora 16 està comprada, es comença amb una singularitat
        data.quarkCondensationBoost = 1;

        for (int i = 0; i < 8; i++)
        {
            data.acceleratorsBoosts[i] = 1; // El multiplicador base ha de ser 1
        }
        if (data.particlePlaytime < data.particleFastestPlaytime)
            data.particleFastestPlaytime = data.particlePlaytime;
        data.particlePlaytime = 0;

        // FORÇAR EL RETORN DE L'ESCALA
        transOut.transform.localScale = Vector3.one;
        transOut.Play("New State", 0, 0);
    }
}
