using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour {
	
	public AsteroidType Type;
	public float damage;
	public float speed;
	public float deltaPower;
	public GameObject explosion;

	Vector3 direction;

	public enum AsteroidType
	{
		simple
	}
	
	[HideInInspector]
	public Transform player;
	[HideInInspector]
	public gameEngine game;

	// Use this for initialization
	void Start () {
		direction = player.position - transform.position;
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
			player.GetComponent<Animator>().PlayInFixedTime("SimpleHit");
			game.GetDamage(damage);
			Destroy(gameObject);
		}
		if(col.gameObject.CompareTag("planet"))
		{
			if(col.gameObject.GetComponent<planets>().isOrbited){
			GameObject obj = Instantiate(explosion, transform.position, Quaternion.identity) as GameObject;
			Destroy(obj, 2);
			col.gameObject.GetComponent<planets>().AddPower(deltaPower);
			Destroy(gameObject);
			}
		}
	}
}
