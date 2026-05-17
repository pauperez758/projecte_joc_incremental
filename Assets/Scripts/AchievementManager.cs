using System.Collections.Generic;
using UnityEngine;
using TMPro;
using BreakInfinity;
using Extendables;

public class AchievementManager : MonoBehaviour
{
    public GameController game;

    public GameObject AchievementScreen;

    public List<Achievement> achievements = new List<Achievement>();
    public List<Row> rows = new List<Row>();
    public List<string> achievementNames;

    public TMP_Text achievementBoostText;

    public BigDouble achievementBoost => BigDouble.Pow(1.5, achievementsCompleted());

    public BigDouble achievementsCompleted()
    {
        var counter = 0;
        for (var i = 0; i < rows.Count; i++)
        {
            if (IsRowCompleted(i))
                counter++;
        }
        return counter;
    }

    public void StartAcheivements()
    {
        var data = game.data;

        achievementNames = new List<string>()
        {
            "Primera partícula de matèria",
            "Acceleració inicial",
            "Construint momentum",
            "Reacció en cadena",
            "Fusió quàntica",
            "Primera ruptura",
            "Més enllà del visible",
            "Singularitat incipient",
            "Domini quàntic",
            "El límit s'apropa",
            "Compressió total",
            "Col·lapsador de partícules",
            "Mestre de l'Spin",
            "Inestabilitat màxima",
            "Ruptura dimensional",
            "Transcendència"
        };

        foreach (var ach in AchievementScreen.GetComponentsInChildren<Achievement>())
            achievements.Add(ach);

        foreach (var row in AchievementScreen.GetComponentsInChildren<Row>())
            rows.Add(row);

        try { var temp = data.achievementUnlocks.Count; }
        catch { data.achievementUnlocks = Data.CreateList<bool>(16); }

        if (achievements.Count > data.achievementUnlocks.Count)
        {
            while (data.achievementUnlocks.Count < achievements.Count)
                data.achievementUnlocks.Add(false);
        }

        for (var i = 0; i < achievements.Count; i++)
            achievements[i].title.text = achievementNames[i];

        UpdateAchievementUI();
        UpdateRowUI();
    }

    private void UnlockAchievement(int id)
    {
        if (game.data.achievementUnlocks[id]) return;
        game.data.achievementUnlocks[id] = true;
        UpdateSpecificAchievementUI(id);

        int rowId = id / 8;
        if (IsRowCompleted(rowId))
            UpdateSpecificRowUI(rowId);
    }

    private bool IsRowCompleted(int id)
    {
        var counter = 0;
        for (var i = id * 8; i < id * 8 + 8; i++)
        {
            if (game.data.achievementUnlocks[i])
                counter++;
        }
        return counter == 8;
    }

    public void Update()
    {
        if (game.achievementsCanvas.gameObject.activeSelf)
            achievementBoostText.text = $"Multiplicador actual de fites en cada generador: {achievementBoost.Notate(1)}x";

        var data = game.data;

        if (data.AcceleratorsCount[0] >= 1 && !data.achievementUnlocks[0])
            UnlockAchievement(0);

        if (data.quark >= 1000 && !data.achievementUnlocks[1])
            UnlockAchievement(1);

        if (data.AcceleratorsCount[1] >= 1 && !data.achievementUnlocks[2])
            UnlockAchievement(2);

        if (data.AcceleratorsCount[2] >= 1 && !data.achievementUnlocks[3])
            UnlockAchievement(3);

        if (data.AcceleratorsCount[3] >= 1 && !data.achievementUnlocks[4])
            UnlockAchievement(4);

        if (data.boostCount >= 1 && !data.achievementUnlocks[5])
            UnlockAchievement(5);

        if (data.AcceleratorsCount[4] >= 1 && !data.achievementUnlocks[6])
            UnlockAchievement(6);

        if (data.AcceleratorsCount[5] >= 1 && !data.achievementUnlocks[7])
            UnlockAchievement(7);

        if (data.AcceleratorsCount[6] >= 1 && !data.achievementUnlocks[8])
            UnlockAchievement(8);

        if (data.AcceleratorsCount[7] >= 1 && !data.achievementUnlocks[9])
            UnlockAchievement(9);

        if (data.quarkCondensationBoost >= 100 && !data.achievementUnlocks[10])
            UnlockAchievement(10);

        if (data.quarkSingularities >= 1 && !data.achievementUnlocks[11])
            UnlockAchievement(11);

        if (data.spinLevels >= 100 && !data.achievementUnlocks[12])
            UnlockAchievement(12);

        if (data.boostCount >= 5 && !data.achievementUnlocks[13])
            UnlockAchievement(13);

        if (data.quarkSingularities >= 3 && !data.achievementUnlocks[14])
            UnlockAchievement(14);

        if (data.quark >= double.MaxValue && !data.achievementUnlocks[15])
            UnlockAchievement(15);
    }

    #region UI
    public void UpdateRowUI()
    {
        for (var i = 0; i < rows.Count; i++)
            UpdateSpecificRowUI(i);
    }

    private void UpdateSpecificRowUI(int id) =>
        rows[id].row.color = IsRowCompleted(id)
            ? new Color(0f, 1f, 0.76f)
            : new Color(0.1f, 0.1f, 0.12f);

    public void UpdateAchievementUI()
    {
        for (var i = 0; i < achievements.Count; i++)
            UpdateSpecificAchievementUI(i);
    }

    private void UpdateSpecificAchievementUI(int id) =>
        achievements[id].achievement.color = game.data.achievementUnlocks[id]
            ? new Color(0.64f, 0.28f, 1f)
            : new Color(0.2f, 0.12f, 0.3f);
    #endregion
}