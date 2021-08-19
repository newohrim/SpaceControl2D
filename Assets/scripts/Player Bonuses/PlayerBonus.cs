using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bonus", menuName = "SpaceControl2D/PlayerBonus")]
public class PlayerBonus : ScriptableObject
{
    [SerializeField]
    protected float bonusTime = 5.0f;
    protected float currentTimer = 0.0f;
    protected bool isActive = false;

    public delegate void BonusState(gameEngine player);
    public event BonusState OnBonusActivate;
    public event BonusState OnBonusDeactivate;
    public bool IsActive { get => isActive; }

    public PlayerBonus()
    {
        OnBonusActivate += OnBonusActivatedHandler;
        OnBonusDeactivate += OnBonusDeactivatedHandler;
    }
    
    public void Activate(gameEngine player) 
    {
        isActive = true;
        OnBonusActivate?.Invoke(player);
        ResetTimers();
    }

    public void Deactivate(gameEngine player) 
    {
        isActive = false;
        OnBonusDeactivate?.Invoke(player);
        ResetTimers();
    }

    public void UpdateBonus(gameEngine player)
    {
        ActionSpecificToBonus(player);
        currentTimer -= Time.deltaTime;
        if (currentTimer <= 0.0f) 
        {
            Deactivate(player);
        }
    }
    
    private void ResetTimers() => currentTimer = bonusTime;
    protected virtual void ActionSpecificToBonus(gameEngine player) {}

    protected virtual void OnBonusActivatedHandler(gameEngine player)
    {
        player.OnInvulnerableBegin();
    }

    protected virtual void OnBonusDeactivatedHandler(gameEngine player)
    {
        player.OnInvulnerableEnd();
    }
}
