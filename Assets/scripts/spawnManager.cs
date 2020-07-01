using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnManager : MonoBehaviour {
	public Transform Player;
	public GameObject TestSpot;
	
	#region Muns
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
	#endregion
	#region Asteroids
	[Header("Asteroid Spawn Timer")]
	public float minTimer;
	public float maxTimer;

	[Header("Asteroid Range")]
	public float minRange;
	public float maxRange;


	[Header("Asteroid Storage")]
	public GameObject[] AsteroidStorage;
	[Range(0.0f, 1f)]
	public float[] AsteroidSpawnRate; 
	#endregion

	[HideInInspector]
	public bool isSpawn = true;
	[HideInInspector]
	public int munCount = 0;

	float timer, _timer;
	private float rateSum;

	
	// Use this for initialization
	void Start () {
		if (AsteroidStorage.Length != AsteroidSpawnRate.Length)
			throw new UnityException("The length of AsteroidStorage and AsteroidSpawnRate " + 
			"are not the same");
		munCount = GameObject.FindGameObjectsWithTag("planet").Length;
		rateSum = SumTheRate();

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
			SpawnAsteroid(range);
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
		Collider2D found = Physics2D.OverlapCircle(new Vector2(x, y), 2f, 8);
		if (found == null){
			Vector3 randomSpawnPos = new Vector3(x, y, 0);
			GameObject munObj = Instantiate(munBase[objIndex], randomSpawnPos, Quaternion.identity) as GameObject;
			munCount++;
		}
		else 
			Debug.LogWarning("Найдено пересечение планет.");
	}

	void SpawnAsteroid(float range)
	{
		int angle = Random.Range(0, 360);
		Vector3 pos = OrbitalPosition(angle, range, Player.position.x, Player.position.y);
		GameObject obj = Instantiate(ChooseRandomAsteroid(), pos, Quaternion.identity) as GameObject;
		obj.GetComponent<IAsteroid>().player = Player;
		obj.GetComponent<IAsteroid>().game = GetComponent<gameEngine>();
		//Instantiate(TestSpot, GetComponent<gameEngine>().MoveSpot, Quaternion.identity);
	}
	
	private GameObject ChooseRandomAsteroid()
	{
		float rnd = Random.Range(0f, rateSum);
		for (int i = 0; i < AsteroidSpawnRate.Length; i++)
		{
			if (rnd <= AsteroidSpawnRate[i])
				return AsteroidStorage[i];
			rnd -= AsteroidSpawnRate[i];
		}

		throw new UnityException("Chance was out of rateSum.");
	}

	private float SumTheRate()
	{
		float sum = 0f;
		foreach(float rate in AsteroidSpawnRate)
		{
			sum += rate;
		}
		return sum;
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
