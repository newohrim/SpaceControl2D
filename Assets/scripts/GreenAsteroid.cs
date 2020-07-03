using UnityEngine;

public class GreenAsteroid : MonoBehaviour, IAsteroid {
	public GameObject Instance { get { return gameObject; } }

    public GameObject explosion;
    public float speed;
    public float healthAmount;

	Vector3 direction;
	
	public Transform player { get; set;}
	public gameEngine game {get; set;}

	// Use this for initialization
	void Start () {
		direction = game.MoveSpot - transform.position;
		Destroy(gameObject, 8);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		transform.Translate(direction.normalized * speed * Time.deltaTime);
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		if(col.gameObject.CompareTag("Player"))
		{
			HitPlayer(col);
		}
		if(col.gameObject.CompareTag("planet"))
		{
			HitPlanet(col);
		}
	}

	public void HitPlayer(Collision2D col)
	{
		player.GetComponent<Animator>().PlayInFixedTime("GreenHit");
		game.AddHealth(healthAmount);
		Destroy(gameObject);
	}

	public void HitPlanet(Collision2D col)
	{
        // Nothing happens
        GameObject obj = Instantiate(explosion, transform.position, Quaternion.identity) as GameObject;
		Destroy(obj, 2);
        Destroy(gameObject);
	}
}