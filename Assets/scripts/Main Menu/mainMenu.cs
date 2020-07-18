using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenu : MonoBehaviour {

	public Renderer background;
	public Animator FadeBackground;
	public GameObject Earth;
	public float backgroundScrollSpeed = 1.0f;
	public float rotateSpeed = 1.0f;
	public Transform[] Moons;

	float offset;
	float[] angle = new float[3];

	// Use this for initialization
	void Start () {
		for(int i = 0; i < angle.Length; i++)
		{
			angle[i] = Mathf.PI;
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		offset += backgroundScrollSpeed * Time.deltaTime;
		background.material.mainTextureOffset = new Vector2(offset, 0); 

		for(int i = 0; i < Moons.Length; i++)
		{
			if(Moons[i] != null)
			{
				Moons[i].position = planets.OrbitalPosition(angle[i], i + 2, Earth.transform.position);
				angle[i] += rotateSpeed * Time.deltaTime;
			}
		}

		
	}

	public void Click(int n)
	{
		switch(n)
		{
			case 0:
				FadeBackground.Play("fadeOut");
				Moons[0].GetComponent<TrailRenderer>().enabled = false;
			break;
		}
	}
}
