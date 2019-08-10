using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using GoogleMobileAds.Api;

public class gameEngine : MonoBehaviour {
	public bool TestAds = true;
	public GameObject player;
	public Animator border;
	public Image healthCounter;
	public float rangeSpeed = 5.0f;
	public float playerSpeed = 5.0f;
	public float cameraSpeed = 0.2f;
	public float backgroundSpeed = 0.1f;
	public Animator shopPanel;
	public bool isStopped = false;
	public planets[] orbitedObjects;
	public GameObject stars;
	public int orbitCount = 3;
	public Text pointsCount, rangeCount;

	[Header("Bonus Numbers")]
	public float invulnerableTimer;
	public float healSpeed;
	public float healAmount;
	public float speedBoost;
	public float speedBoostTime;

	[Header("UI Values")]
	public Animator GameOverText;
	public Animator FadeBackground;

	[HideInInspector]
	public float health;
	[HideInInspector]
	public bool isInvulnerable = false;
	[HideInInspector]
	public bool isHealing = false;
	[HideInInspector]
	public bool isSpeedBoosted = false;

	// [bonus timers]
		float timerInVulnurable;
		float timerSpeedBoost;
	// [/end]

	Vector3 mousePos, direction, prevPos;
	Animator anim;
	Vector2 prevOffset;
	RaycastHit hit;
	float score = 0;
	float playerRealSpeed;
	int i = 0;
	int orbited = 0;
	bool isShopOpen = false;
	float endHealth;
	float newHealthAmount;
	float bestScore = 0;

	private InterstitialAd interstitial;
	
	// Use this for initialization
	void Start () {
		LoadScore();
		Debug.Log("Best Score: " + bestScore);
		health = 100.0f;
		anim = player.GetComponent<Animator>();
		playerRealSpeed = playerSpeed;
		RequestInterstitial();
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if(Physics.Raycast(GetComponent<Camera>().ScreenPointToRay(Input.mousePosition), out hit))
		{
			//Debug.Log (hit.transform.gameObject.tag);
			if (hit.transform.gameObject.CompareTag ("background") && Vector3.Distance (player.transform.position, hit.point) >= 0.5f) 
			{
				mousePos = hit.point;
			}
		}
		
		if(!isStopped)
		{
			BonusManager();
			ClickDetection();

			if(health <= 0)
			{
				Debug.Log("Game Over");
				if (score > bestScore) SaveScore(); 

				GameOver();
			}

			
			//player.transform.position = new Vector3 (player.transform.position.x, player.transform.position.y, 0);
			//range = range - (Time.deltaTime * rangeSpeed);
			rangeCount.text = Mathf.Round(score).ToString();
			pointsCount.text = orbited + "/" + orbitCount;
			transform.position = Vector3.Lerp (transform.position, new Vector3(player.transform.position.x, player.transform.position.y, -10.0f), cameraSpeed * Time.deltaTime);
			if(i == 0)
			{
				direction = mousePos - player.transform.position;
				i = 1;
			}
			if(i == 1)
			{
				player.transform.Translate (new Vector3(direction.x, direction.y, 0) * playerRealSpeed * Time.deltaTime);
				i = 0;
			}

			if(prevPos != player.transform.position)
			{
				Vector2 offset = new Vector2 (player.transform.position.x - prevPos.x, player.transform.position.y - prevPos.y);
				prevOffset = offset;
				stars.GetComponent<Renderer> ().material.mainTextureOffset += offset * backgroundSpeed;
			}
			if(prevPos == player.transform.position)
			{
				stars.GetComponent<Renderer> ().material.mainTextureOffset = prevOffset;
			}

			prevPos = player.transform.position;
			//Debug.Log (prevPos + " " + player.transform.position);

		}
		else
		{
			if(Input.GetKeyDown(KeyCode.Mouse0))
			{
				FadeBackground.Play("fadeOut");
			}
		}
	}

	void ClickDetection()
	{
		if(Input.GetKeyUp(KeyCode.Mouse0)){
			RaycastHit _hit;
			if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out _hit))
			{
				if(_hit.transform.tag == "planet_col"/* && touch.phase == TouchPhase.Ended*/)
				{
					GameObject pl = _hit.transform.parent.gameObject;
					planets planet = pl.GetComponent<planets>();

					if(planet.isPowered)
					{
						//Destroy(pl); // [ATTENTION] may cause errors
						planet.isPowered = false;

						if(planet.Type == planets.PlanetType.InVulnerableMun)
						{
							isInvulnerable = true;
							timerInVulnurable = invulnerableTimer * planet.orbitLevel;
						}
						if(planet.Type == planets.PlanetType.GreenHealPlanet)
						{
							// heal
							isHealing = true;
							newHealthAmount = healAmount/* * planet.orbitLevel*/;
							endHealth = health + newHealthAmount;
						}
						if(planet.Type == planets.PlanetType.RedSpeedPlanet)
						{
							// speed boost
							isSpeedBoosted = true;
							timerSpeedBoost = speedBoostTime;
							player.GetComponent<TrailRenderer>().enabled = true;
						}

						pl.GetComponent<Animator>().Play("munDestroy");
						GetComponent<spawnManager>().munCount--;
					}
				}
			}
		}
	}

	void BonusManager()
	{ 
		if(isInvulnerable)
		{
			Debug.Log(invulnerableTimer + " " + timerInVulnurable);
			if(timerInVulnurable > 0) 
			{
				timerInVulnurable -= Time.deltaTime;
				border.gameObject.SetActive(true);
			}
			else
			{
				timerInVulnurable = invulnerableTimer;
				isInvulnerable = false;
				border.gameObject.SetActive(false);
			}
		}
		if(isHealing)
		{	
			if(health < endHealth && health <= 100)
			{
				health += healSpeed * Time.deltaTime;
				anim.Play("healing");
			}
			else
			{
				isHealing = false;
				anim.Play("New State");
			}

			if(health > 100) health = 100;

			healthCounter.fillAmount = health / 100;
		}
		if(isSpeedBoosted)
		{
			if(timerSpeedBoost > 0)
			{
				timerSpeedBoost -= Time.deltaTime;
				playerRealSpeed = playerSpeed * speedBoost;
			}
			else
			{
				isSpeedBoosted = false;
				timerSpeedBoost = speedBoostTime;
				playerRealSpeed = playerSpeed;
				player.GetComponent<TrailRenderer>().enabled = false;
			}
		}
	}

	public void GameOver()
	{
		GameOverText.Play("GameOverTextOpen");
		GameObject.Find("BestScore").GetComponent<Text>().text = "Best Score: " + bestScore.ToString();
		isStopped = true;
		GetComponent<spawnManager>().isSpawn = false;

		// Ads section

		if(interstitial.IsLoaded()) interstitial.Show();
	}

	public void Orbit(planets obj)
	{
		for(int i = 0; i < orbitedObjects.Length; i++)
		{
			if(orbitedObjects[i] == null)
			{
				orbitedObjects [i] = obj;
				orbited++;
				obj.orbitLevel = i + 1;
				Debug.Log(i + 1);
				return;
			}
		}

		Debug.LogError("gameEngine.Orbit()_Error");
	}

	public void DeOrbit(planets obj)
	{
		Debug.Log("Object " + obj.name + " was DeOrbited");

		for(int i = 0; i < orbitedObjects.Length; i++)
		{
			if(obj == orbitedObjects[i])
			{
				orbitedObjects [i] = null;
				orbited--;
				return;
			}
		}
	}

	public bool FindOrbit()
	{
		for(int i = 0; i < orbitedObjects.Length; i++)
		{
			if(orbitedObjects[i] == null)
			{
				//Debug.Log("Orbit found");
				return true;
			}
		}
		
		return false;
	}

	public void GetDamage(float damage)
	{
		if(damage > 0 && !isInvulnerable)
		{ 
			health -= damage;
			if(isHealing) endHealth -= damage;

			//isHealing = false; // may be turn on
		}
		if(isInvulnerable) score += damage / 5;

		healthCounter.fillAmount = health / 100;
	}

	public void OnButtonEnter(string func)
	{
		if(func == "shopButton")
		{
			if(!isShopOpen)
			{
				shopPanel.Play("shopOpen");
				isShopOpen = true;
				mousePos = transform.position;
			}
			else
			{
				shopPanel.Play("shopClose");
				isShopOpen = false;
			}
		}
	}

	void SaveScore()
	{
		if(!Directory.Exists(Application.persistentDataPath + GetAndroidInternalFilesDir() + "/saves"))
		{
			Directory.CreateDirectory(Application.persistentDataPath + GetAndroidInternalFilesDir() + "/saves");
		}

		saveInfo info = new saveInfo();
		info.bestScore = score;
		FileStream fs = new FileStream(Application.persistentDataPath + GetAndroidInternalFilesDir() + "/saves/save.sv", FileMode.Create);
		BinaryFormatter formatter = new BinaryFormatter();
		formatter.Serialize(fs, info);
		fs.Close();

		bestScore = score;
		Debug.Log("Saved score: " + score);
	}

	void LoadScore()
	{
		Debug.Log(Application.persistentDataPath + GetAndroidInternalFilesDir());
		if (Directory.Exists (Application.persistentDataPath + GetAndroidInternalFilesDir() + "/saves/")) { // я не знаю почему инвертированное услолвие, работает только так!
			BinaryFormatter formatter = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + GetAndroidInternalFilesDir() + "/saves/save.sv", FileMode.Open);
			saveInfo info = new saveInfo ();
			info = (saveInfo)formatter.Deserialize (file);
			bestScore = info.bestScore;
			file.Close();

			Debug.Log("Loading...");
		}
	}

	public static string GetAndroidInternalFilesDir()
	{
    string[] potentialDirectories = new string[]
    {
		"/storage/emulated/0/Android/data/SpaceControl2D",
		"/files/storage",
        "/storage",
        "/sdcard",
        "/storage/emulated/0",
        "/mnt/sdcard",
        "/storage/sdcard0",
        "/storage/sdcard1"
    };

    if(Application.platform == RuntimePlatform.Android)
    {
        for(int i = 0; i < potentialDirectories.Length; i++)
        {
            if(Directory.Exists(potentialDirectories[i]))
            {
                return potentialDirectories[i];
            }
        }
    }
	//Debug.LogError("[GameEngine.FindPath] ERROR no path found");
    return "";
	}

	private void RequestInterstitial()
	{
   	 	#if UNITY_ANDROID
			string adUnitId;
       		if(!TestAds) adUnitId = "ca-app-pub-7331899533603518/3362036107";
			else adUnitId = "ca-app-pub-3940256099942544/1033173712";
   	 	/*#elif UNITY_IPHONE
       		adUnitId = "ca-app-pub-3940256099942544/4411468910";*/
   	 	#else
        	string adUnitId = "unexpected_platform";
    	#endif

    	// Initialize an InterstitialAd.
    	this.interstitial = new InterstitialAd(adUnitId);
		AdRequest request = new AdRequest.Builder().AddTestDevice("	ca-app-pub-3940256099942544/1033173712").Build();
		this.interstitial.LoadAd(request);
	}
}

[System.Serializable]
public class saveInfo
{
	public float bestScore;
}

/* BUGS SECTION /
	- Когда планет на орбите становится столько же, сколько всего слотов в массиве, часть из них исчезает
	Решено: На Trail Renderer был включен AutoDestruct
end*/
