using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "GhostGarden/GameData")]
public class GameData : ScriptableObject
{
    [Header("Time")]
    public int currentDay = 1;
    public Season currentSeason = Season.Spring;
    public int nudgesRemainingToday = 3;
    public int nudgesPerDay = 3;

    [Header("Garden")]
    public PlantSlot[] plants;

    public void ResetForNewDay()
    {
        nudgesRemainingToday = nudgesPerDay;
    }

    public bool UseNudge()
    {
        if (nudgesRemainingToday <= 0) return false;
        nudgesRemainingToday--;
        return true;
    }

    public void AdvanceDay()
    {
        currentDay++;
        if (currentDay > 90 && currentSeason == Season.Spring)  currentSeason = Season.Summer;
        if (currentDay > 180 && currentSeason == Season.Summer) currentSeason = Season.Fall;
        if (currentDay > 270 && currentSeason == Season.Fall)   currentSeason = Season.Winter;
    }
}

public enum Season { Spring, Summer, Fall, Winter }

[System.Serializable]
public class PlantSlot
{
    public string plantName;
    [Range(0f, 1f)] public float health = 1f;
    public PlantStage stage = PlantStage.Sprout;

    public void ApplyDailyDecay(float amount)
    {
        health = Mathf.Max(0f, health - amount);
        UpdateStage();
    }

    public void ApplyCare()
    {
        health = Mathf.Min(1f, health + 0.2f);
        UpdateStage();
    }

    void UpdateStage()
    {
        if      (health > 0.75f) stage = PlantStage.Bloom;
        else if (health > 0.4f)  stage = PlantStage.Sprout;
        else if (health > 0.1f)  stage = PlantStage.Wilting;
        else                     stage = PlantStage.Dead;
    }
}

public enum PlantStage { Seed, Sprout, Bloom, Wilting, Dead }