using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
public class ReptesManager : MonoBehaviour
{
    public GameController game;
    public ParticleManager particleManager;

    public Image[] challengeStartButtons;
    public TMP_Text[] challengeStartText;

    public Color incompleteYellow;
    public Color completeYellow;
    public Color runningGrey;

    public void StartChallenges()
    {
        incompleteYellow = new Color(0.443f, 0.490f, 0.149f);
        completeYellow = new Color(0.874f, 0.874f, 0.172f);
        runningGrey = new Color(0.149f, 0.196f, 0.220f);

        var data = game.data;
        try
        {
            var temp = data.challengeCompleted.Count;
        }
        catch
        {
            data.challengeCompleted = Data.CreateList<bool>(12);
        }

        UpdateChallengeUI();
    }

    public void Update()
    {
        // EP.11 12:59
        var data = game.data;

        int repteActualId = data.currentChallenge;

        if (repteActualId < data.challengeCompleted.Count && !data.challengeCompleted[repteActualId]) // id de repte comença en 0
        {
            #region reptes
            switch (repteActualId)
            {
                // Repte 1: Arribar a 1e100 Quarks ràpidament
                case 1:
                    if (data.quark >= 1e100) CompleteChallenge(0);
                    break;

                // Repte 2: Tenir 500 nivells de Spin
                case 2:
                    if (data.spinLevels >= 500) CompleteChallenge(1);
                    break;

                // Repte 3: Tenir 10 Boosts d'acceleradors
                case 3:
                    if (data.boostCount >= 10) CompleteChallenge(2);
                    break;

                // Repte 4: Tenir 1.000.000 de Punts de Partícula (acumulats)
                case 4:
                    if (data.particlePoints >= 1e6) CompleteChallenge(3);
                    break;

                // Repte 5: Tenir 1e50 unitats del 5è Accelerador (AcceleratorsCount[4])
                case 5:
                    if (data.AcceleratorsCount[4] >= 1e6) CompleteChallenge(4);
                    break;

                // Repte 6: Arribar a un Multiplicador de Spin de 0.70 o inferior
                case 6:
                    if (game.spinMultiplier <= 0.70) CompleteChallenge(5);
                    break;

                // Repte 7: Tenir almenys 10.000 unitats de tots els Acceleradors (del 0 al 7)
                case 7:
                    bool allTenThousand = true;
                    for (int x = 0; x < 8; x++)
                    {
                        if (data.AcceleratorsCount[x] < 10000) allTenThousand = false;
                    }
                    if (allTenThousand) CompleteChallenge(6);
                    break;

                // Repte 8: Arribar a double.MaxValue de Quarks (El repte final)
                case 8:
                    if (data.quark >= double.MaxValue) CompleteChallenge(7);
                    break;
            }
            #endregion
        }
    }

    public void UpdateChallengeUI()
    {
        var data = game.data;

        for (int i = 0; i < challengeStartButtons.Length; i++)
        {
            if (data.challengeCompleted[i])
            {
                challengeStartButtons[i].color = completeYellow;
                challengeStartText[i].text = "Completat";
            }
            else if (data.currentChallenge == i)
            {
                challengeStartButtons[i].color = runningGrey;
                challengeStartText[i].text = "Intentant...";
            }
            else
            {
                challengeStartButtons[i].color = incompleteYellow;
                challengeStartText[i].text = "Començar";
            }
        }
    }

    public void StartChallenge(int id)
    {
        var data = game.data;
        if (id == data.currentChallenge) return; // Si ja estem en aquest repte, no fem res
        if (data.challengeCompleted[id]) return;
        data.currentChallenge = id;
        particleManager.Transcendence();
        
        UpdateChallengeUI();
    }

    public void CancelChallenge()
    {
        game.data.currentChallenge = 0;
        UpdateChallengeUI();
    }

    public void CompleteChallenge(int id)
    {
        game.data.challengeCompleted[id] = true;
        game.data.currentChallenge = 0;
        UpdateChallengeUI();
    }
}
