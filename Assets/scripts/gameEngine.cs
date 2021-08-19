using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using GoogleMobileAds.Api;

public class gameEngine : MonoBehaviour {
	public GameObject player;
	public Animator border;
	public Image healthCounter;
	public float rangeSpeed = 5.0f;
	public float playerSpeed = 5.0f;
	public const float MaxHealth = 100f;
	public float cameraSpeed = 0.2f;
	public float backgroundSpeed = 0.1f;
	public Animator shopPanel;
	public bool isStopped = false;
	public planets[] orbitedObjects;
	public GameObject stars;
	public int orbitCount = 3;
	public Text pointsCounter, rangeCount;

	[Header("Ads")]
	[SerializeField]
	private bool TestAds = false;
	[SerializeField]
	private bool adsEnabled = true;

	[Header("Bonus Numbers")]
	[SerializeField]
	private BonusManager bonusManager;

	[Header("UI Values")]
	public Animator GameOverText;
	public Animator FadeBackground;

	[Header("Sounds")]
	public AudioClip planetHit1;
	public AudioClip planetHit2;
	public AudioClip redPlanetBonusSound;

	private float health = 100.0f;
	private int pointsCount = 0;

	public Vector3 MoveSpot { get; set; }

	Vector3 mousePos, direction, prevPos;
	Animator anim;
	AudioSource AudioPlayer;
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
	private float speedMultiplier = 1.0f;

	private InterstitialAd interstitial;
	
	void Start () {
		LoadScore();
		bonusManager.Initialize();
		Debug.Log("Best Score: " + bestScore);
		health = MaxHealth;
		anim = player.GetComponent<Animator>();
		AudioPlayer = GetComponent<AudioSource>();
		playerRealSpeed = playerSpeed;
		if (adsEnabled)
			RequestInterstitial();
	}

	void Update() 
	{
		if(!isStopped) 
		{
			ClickDetection();
			bonusManager.UpdateBonuses(this);
			MoveAndGarbageStuff();
		}
		else
		{
			if(Input.GetKeyDown(KeyCode.Mouse0))
			{
				FadeBackground.Play("fadeOut");
			}
		}
	}

	void MoveAndGarbageStuff()
	{
		if(Physics.Raycast(GetComponent<Camera>().ScreenPointToRay(Input.mousePosition), out hit))
		{
			if (hit.transform.gameObject.CompareTag ("background") && Vector3.Distance (player.transform.position, hit.point) >= 0.5f) 
			{
				mousePos = hit.point;
			}
		}
		
		if(!isStopped)
		{
			rangeCount.text = Mathf.Round(score).ToString();
			transform.position = Vector3.Lerp (transform.position, new Vector3(player.transform.position.x, player.transform.position.y, -10.0f), cameraSpeed * Time.deltaTime);
			if(i == 0)
			{
				direction = mousePos - player.transform.position;
				i = 1;
			}
			if(i == 1)
			{
				MoveSpot = player.transform.position + new Vector3(direction.x, direction.y, 0) * playerSpeed;
				Debug.DrawLine(player.transform.position, MoveSpot, Color.green);
				player.transform.Translate (new Vector3(direction.x, direction.y, 0) * playerRealSpeed * speedMultiplier * Time.deltaTime);
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
		}
	}

	void ClickDetection()
	{
		if(Input.GetKeyUp(KeyCode.Mouse0))
		{
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

						switch(planet.Type) 
						{
							case planets.PlanetType.InVulnerableMun:
								bonusManager.InvulnerablityBonus.Activate(this);
								break;
							case planets.PlanetType.GreenHealPlanet:
								bonusManager.HealBonus.Activate(this);
								break;
							case planets.PlanetType.RedSpeedPlanet:
								bonusManager.SpeedBonus.Activate(this);
								break;
							default:
								Debug.LogError("Unknown planet type.");
								break;
						}
					}
					pl.GetComponent<Animator>().Play("munDestroy");
					GetComponent<spawnManager>().munCount--;
				}
			}
		}
	}

	public void GameOver()
	{
		Debug.Log("Game Over");
		if (score > bestScore) 
		{
			SaveScore();
		}
		leaderboard.AddScoreToLeaderboard(GPGSIds.leaderboard_best_scores_world, (long)bestScore);
		GameOverText.Play("GameOverTextOpen");
		GameOverText.GetComponent<Text>().text = "Game Over";
		if (Application.systemLanguage == SystemLanguage.Russian)
			GameOverText.GetComponent<Text>().text = "Игра Окончена";
		GameObject.Find("BestScore").GetComponent<Text>().text = "Best Score: " + bestScore.ToString();
		isStopped = true;
		GetComponent<spawnManager>().isSpawn = false;

		// Ads section
		if(score < 10 || !adsEnabled) return;
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
		if(damage > 0 && !bonusManager.InvulnerablityBonus.IsActive)
		{ 
			health -= damage;
			UpdateHealthCounter();
			if(health <= 0)
			{
				GameOver();
			}
		}
	}

	public void OnHitHandler(float damage)
	{
		anim.PlayInFixedTime("SimpleHit");
		if (bonusManager.InvulnerablityBonus.IsActive) 
		{
			score++;
			pointsCount++;
			AudioPlayer.volume = 0.5f;
			AudioPlayer.PlayOneShot(planetHit1);
		}
		else 
		{
			GetDamage(damage);
			AudioPlayer.volume = 1.0f;
			AudioPlayer.PlayOneShot(planetHit2);
		}
	}

	public void AddHealth(float healthAmount)
	{
		health += healthAmount;
		if (healthAmount > MaxHealth)
			health = MaxHealth;
		UpdateHealthCounter();
	}

	public void SetSpeedMultiplier(float speedMultiplier)
	{
		this.speedMultiplier = speedMultiplier;
	}

	public void OnInvulnerableBegin()
	{
		border.gameObject.SetActive(true);
		pointsCounter.GetComponent<Animator>().Play("counterActivate");
	}

	public void OnInvulnerableEnd()
	{
		border.gameObject.SetActive(false);
		pointsCounter.GetComponent<Animator>().Play("counterDisable");
		pointsCount = 0;
	}
	
	public void OnHealingBegin()
	{
		anim.Play("healing");
	}

	public void OnHealingEnd()
	{
		anim.Play("New State");
	}

	public void OnSpeedUpBegin() 
	{
		player.GetComponent<TrailRenderer>().enabled = true;
		AudioPlayer.PlayOneShot(redPlanetBonusSound);
	}

	public void OnSpeedUpEnd() 
	{
		player.GetComponent<TrailRenderer>().enabled = false;
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

	private void UpdateHealthCounter()
	{
		healthCounter.fillAmount = health / MaxHealth;
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
		AdRequest request = new AdRequest.Builder().Build();
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
