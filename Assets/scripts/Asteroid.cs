using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour, IAsteroid {
	public float damage;
	public float speed;
	public float deltaPower;
	public GameObject explosion;

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
		//direction = player.position - transform.position;
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
		game.OnHitHandler(damage);
		Destroy(gameObject);
	}

	public void HitPlanet(Collision2D col)
	{
		if(col.gameObject.GetComponent<planets>().isOrbited)
		{
			GameObject obj = Instantiate(explosion, transform.position, Quaternion.identity) as GameObject;
			Destroy(obj, 2);
			col.gameObject.GetComponent<planets>().AddPower(deltaPower);
			Destroy(gameObject);
		}
	}
}
