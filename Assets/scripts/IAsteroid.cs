using UnityEngine;

public interface IAsteroid
{
    Transform player { get; set; }
    gameEngine game { get; set; }
    void HitPlayer(Collision2D col);
    void HitPlanet(Collision2D col);
}