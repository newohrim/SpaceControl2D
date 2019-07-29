using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnManager : MonoBehaviour {
	public Transform Player;
	//public gameEngine game;
	[Header("Mun Base")]
	public GameObject[] munBase;

	[Header("Mun Spawner")]
	public float minX;
	public float maxX;
	public float minY;
	public float maxY;
	public int minAmount;
	public int maxAmount;
	public float munTimer;

	[Header("Asteroid Spawn Timer")]
	public float minTimer;
	public float maxTimer;

	[Header("Asteroid Range")]
	public float minRange;
	public float maxRange;

	[Header("Asteroid Storage")]
	public Asteroid[] AsteroidStorage;

	[HideInInspector]
	public bool isSpawn = true;
	[HideInInspector]
	public int munCount = 0;

	float timer, _timer;

	
	// Use this for initialization
	void Start () {
		munCount = GameObject.FindGameObjectsWithTag("planet").Length;

		ResetTimer(); 
		SpawnOnStart();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(isSpawn) SpawnProcess();
		PlanetSpawner();
	}

	void SpawnOnStart()
	{
		int spawnAmount = Random.Range(minAmount, maxAmount);

		for(int i = 0; i < spawnAmount; i++)
		{
			PlanetSpawn();
			//Debug.Log("[SPAWN ON START]" + munCount);
		}
	}

	void SpawnProcess()
	{
		if(timer > 0)
		{
			timer -= Time.deltaTime;
		}
		if(timer <= 0)
		{
			float range = Random.Range(minRange, maxRange);
			SpawnAsteroid(AsteroidStorage[0], range);
			ResetTimer();
		}
	}

	void PlanetSpawner()
	{
		while(munCount < (minAmount + maxAmount) / 2) 
		{
			PlanetSpawn();
			//Debug.Log("Planet spawned");
		}

		//Debug.Log(munCount);
	}

	void PlanetSpawn()
	{
		int objIndex = Random.Range(0, munBase.Length);
		float x = transform.localPosition.x + Random.Range(minX, maxX);
		float y = transform.localPosition.y + Random.Range(minY, maxY);
		Vector3 randomSpawnPos = new Vector3(x, y, 0);
		GameObject munObj = Instantiate(munBase[objIndex], randomSpawnPos, Quaternion.identity) as GameObject;
		munCount++;
	}

	void SpawnAsteroid(Asteroid asteroid, float range)
	{
		int angle = Random.Range(0, 360);
		Vector3 pos = OrbitalPosition(angle, range, Player.position.x, Player.position.y);
		GameObject obj = Instantiate(asteroid.gameObject, pos, Quaternion.identity) as GameObject;
		obj.GetComponent<Asteroid>().player = Player;
		obj.GetComponent<Asteroid>().game = GetComponent<gameEngine>();
	}

	void ResetTimer()
	{
		timer = Random.Range(minTimer, maxTimer);
	}

	public static Vector3 OrbitalPosition(float angle, float radius, float x, float y)
	{
		float xPos = radius * Mathf.Cos(angle) + x;
		float yPos = radius * Mathf.Sin(angle) + y;
		Vector3 pos = new Vector3(xPos, yPos, 0);
		return pos;
	}
}
