
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
            if ( IsRowCompleted(i)) 
                counter++;
        }
        return counter;
    }

    public void StartAcheivements()
    {
        var data = game.data;
        achievementNames = new List<string>()
        {
            "Quarks 1",
            "Achievement 2",
            "Achievement 3",
            "Achievement 4",
            "Achievement 5",
            "Achievement 6",
            "Achievement 7",
            "Achievement 8",
            "Achievement 9",
            "Achievement 10",
            "Achievement 11",
            "Achievement 12",
            "Achievement 13",
            "Achievement 14",
            "Achievement 15",
            "Achievement 16",
            "Achievement 17",
            "Achievement 18",
            "Achievement 19",
            "Achievement 20",
            "Achievement 21",
            "Achievement 22",
            "Achievement 23",
            "Achievement 24"
        };
        foreach (var ach in AchievementScreen.GetComponentsInChildren<Achievement>()) 
            achievements.Add(ach);

        foreach (var row in AchievementScreen.GetComponentsInChildren<Row>()) 
            rows.Add(row);

        try
        {
            var temp = data.achievementUnlocks.Count;
        }
        catch
        {
            data.achievementUnlocks = Data.CreateList<bool>(16);
        }

        if (achievements.Count > data.achievementUnlocks.Count)
        {
            while (data.achievementUnlocks.Count < achievements.Count)
            {
                data.achievementUnlocks.Add(false);
            }
        }

        for (var i = 0; i < achievements.Count; i++)
        {
            achievements[i].title.text = achievementNames[i];
        }
        UpdateAchievementUI();
        UpdateRowUI();
    }

    
    private void UnlockAchievement(int id)
    {
        if (game.data.achievementUnlocks[id]) return;

        game.data.achievementUnlocks[id] = true;
        UpdateSpecificAchievementUI(id);

        // Calculem quina fila (row) pertany aquest achievement
        int rowId = id / 8;

        if (IsRowCompleted(rowId))
        {
            UpdateSpecificRowUI(rowId);
        }
    }

    private bool IsRowCompleted(int id)
    {
        var counter = 0;
        for (var i = id * 8; i < id * 8 + 8; i++)
        {
            if (game.data.achievementUnlocks[i])
            {
                counter++;
            }
        }
        return counter == 8;
    }

    public void Update()
    {
        if (game.achievementsCanvas.gameObject.activeSelf) achievementBoostText.text = $"Multiplicador actual de fites en cada generador: {achievementBoost.Notate(1)}x";
        var data = game.data;
        
        //TODO: CONDICIONALS D'ACHIEVEMENTS. AQUESTS SÓN EXEMPLES. Realment l'únic que canvia són les condicions.
        for (var i = 0; i < 8; i++)
        {
            if (data.AcceleratorsCount[i] > 0 && !data.achievementUnlocks[i]) UnlockAchievement(i);
        }
        if (data.quark >= double.MaxValue && !data.achievementUnlocks[8]) UnlockAchievement(8);
        if (data.quark >= float.MaxValue && !data.achievementUnlocks[9]) UnlockAchievement(9);
        if (data.AcceleratorsCount[7] >= 99 && !data.achievementUnlocks[10]) UnlockAchievement(10);
        
    }
    #region UI
    private void UpdateRowUI()
    {
        for (var i = 0; i < rows.Count; i++)
        {
            UpdateSpecificRowUI(i);
        }
    }

    private void UpdateSpecificRowUI(int id) => 
        rows[id].row.color = IsRowCompleted(id) ? new Color(0f, 1f, 0.76f)
        : new Color(0.1f, 0.1f, 0.12f);

    private void UpdateAchievementUI()
    {
        for (var i = 0; i < achievements.Count; i++)
        {
            UpdateSpecificAchievementUI(i);
        }
    }
    private void UpdateSpecificAchievementUI(int id) => achievements[id].achievement.color =
        game.data.achievementUnlocks[id] ? new Color(0.64f, 0.28f, 1f) : new Color(0.2f, 0.12f, 0.3f);

    #endregion
}
