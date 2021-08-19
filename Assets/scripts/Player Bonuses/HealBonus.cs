using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HealBonus", menuName = "SpaceControl2D/HealBonus")]
public class HealBonus : PlayerBonus
{
    [SerializeField]
    private float healingPerSecond = 5.0f;

    protected override void ActionSpecificToBonus(gameEngine player)
    {
        player.AddHealth(healingPerSecond * Time.deltaTime);
    }

    // MUST OVERRIDE EVENT HANDLERS!!
    protected override void OnBonusActivatedHandler(gameEngine player) 
    {
        player.OnHealingBegin();
    }
    protected override void OnBonusDeactivatedHandler(gameEngine player) 
    {
        player.OnHealingEnd();
    }
}
