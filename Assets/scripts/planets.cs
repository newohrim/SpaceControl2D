using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class planets : MonoBehaviour {
	public const float ViewDistance = 25.0f;
	public PlanetType Type;
	public GameObject pl, cam;
	public Image powerCounter;
	public float force, inForce;
	public float maxSpeed = 5.0f;
	public float gravityNeed;
	//Rigidbody2D rig;
	Animator anim;
	public bool isOrbited = false;
	public float orbitLevel;
	public float scoreSpeed;
	public float score = 0;

	[HideInInspector]
	public float power = 0;
	public bool isPowered = false;
	[HideInInspector]
	public bool isDeOrbit = false;

	float alpha = 0;
	float timer = 0.0f;
	public float timerSpeed = 1.0f;
	public float orbitOffset;
	int n = 0;

	public enum PlanetType
	{
		InVulnerableMun,
		GreenHealPlanet,
		RedSpeedPlanet
	}

	// Use this for initialization
	void Awake () {
		pl = GameObject.FindGameObjectWithTag("Player");
		cam = GameObject.FindGameObjectWithTag("MainCamera");
		//rig = GetComponent<Rigidbody2D> ();
		anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		if (isDeOrbit) DeOrbit();

		//float speed = rig.velocity.magnitude;
		float plDistance = Vector3.Distance (transform.position, pl.transform.position);

		if(plDistance > ViewDistance)
		{
			cam.GetComponent<spawnManager>().munCount--;
			Destroy(gameObject);
		}

		if(isOrbited)
		{
			score += Time.deltaTime * scoreSpeed * orbitLevel;
		}
		//if (plDistance > ViewDistance) Destroy(gameObject);
		if (plDistance <= gravityNeed) 
		{

			if (!isOrbited && Camera.main.GetComponent<gameEngine>().FindOrbit()) //если есть свободные орбиты
			{
				Camera.main.GetComponent<gameEngine> ().Orbit (this); // need a fix [ERROR] || 29.04.19 UPD: need to fix perfomance
				isOrbited = true;
			}

			if(isOrbited){
			if(n == 0) 
			{
				Vector3 line = transform.position - pl.transform.position;
				alpha = Vector3.Angle(line, Vector3.right);
				if(Mathf.Sin(alpha) < 0) alpha = 2 * Mathf.PI - alpha;
				alpha = Mathf.PI * alpha / 180;
				n = 1;
			}

			//alpha = alpha + (force * Time.deltaTime); // alpha angle is not moving when n = 1
			Vector3 newPos = OrbitalPosition(alpha, orbitLevel, pl);

			if(n == 1)
			{
				//Debug.Log(orbitLevel);
				if(Vector3.Distance(transform.position, pl.transform.position) > orbitLevel + 0.5f)
				{
					transform.position = Vector3.Lerp(transform.position, newPos, 0.1f);
				}
				else {
					//Debug.Log("break");
					n = 2;
				}
			}
			if(n == 2)
			{
				alpha = alpha + (force * Time.deltaTime);
				transform.position = newPos;
			}
			}

		} else if(isOrbited) {
			//Camera.main.GetComponent<gameEngine> ().DeOrbit (this);
			//isOrbited = false;
		}
	}

	public static Vector3 OrbitalPosition(float angle, float radius, GameObject center)
	{
		//float sub_angle = angle;

		//float cosA = Mathf.Cos(angle);
		//float sinA = Mathf.Sin(angle);

		//if (sinA < 0) sub_angle = 2 * Mathf.PI - angle;

		float a = center.transform.position.x;
		float b = center.transform.position.y;
		float x = (radius + 0.5f) * Mathf.Cos(angle) + a;
		float y = (radius + 0.5f) * Mathf.Sin(angle) + b;
		Vector3 pos = new Vector3(x, y, 0);
		return pos;
	}

	public void AddPower(float deltaPower)
	{
		power += deltaPower;
		if (power >= 100)
		{
			powerCounter.fillAmount = 1;
			isPowered = true;
			// animation starts
			anim.Play("simpleCounterFull");
		}
		else
		{
			powerCounter.fillAmount = power / 100;
		}
	}

	public void DeOrbit()
	{
		Camera.main.GetComponent<gameEngine>().DeOrbit(this);
		Debug.Log("Planet Destroyed");
		Destroy(gameObject);
	}
}
