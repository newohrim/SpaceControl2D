using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpeedBonus", menuName = "SpaceControl2D/SpeedBonus")]
public class SpeedBonus : PlayerBonus
{
    private const float PLAYERS_DEFFAULT_SPEED_MULTIPLIER = 1.0f;

    [SerializeField]
    private float speedMultiplier = 2.0f;

    protected override void OnBonusActivatedHandler(gameEngine player)
    {
        player.SetSpeedMultiplier(speedMultiplier);
        player.OnSpeedUpBegin();
    }

    protected override void OnBonusDeactivatedHandler(gameEngine player)
    {
        player.SetSpeedMultiplier(PLAYERS_DEFFAULT_SPEED_MULTIPLIER);
        player.OnSpeedUpEnd();
    }
}
