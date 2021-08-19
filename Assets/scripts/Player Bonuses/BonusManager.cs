using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BonusManager
{
    [SerializeField]
    private PlayerBonus invulnerabilityBonus;
    [SerializeField]
    private SpeedBonus speedBonus;
    [SerializeField]
    private HealBonus healBonus;

    public PlayerBonus InvulnerablityBonus { get => invulnerabilityBonus; }
    public PlayerBonus SpeedBonus { get => speedBonus; }
    public PlayerBonus HealBonus { get => healBonus; }

    private PlayerBonus[] playerBonuses;

    // Not using a constructor, cause setting valuse through inspector
    public void Initialize()
    {
        playerBonuses = new PlayerBonus[] 
        {
            invulnerabilityBonus,
            speedBonus,
            healBonus
        };
    }

    public void UpdateBonuses(gameEngine forPlayer)
    {
        foreach(PlayerBonus bonus in playerBonuses)
        {
            if (bonus.IsActive)
                bonus.UpdateBonus(forPlayer);
        }
    }
}
