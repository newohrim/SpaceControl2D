using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mainMenuButton : MonoBehaviour {
	public mainMenu menu;
	Animator anim;
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnMouseEnter()
	{
		anim.Play("mainMenuButtonClick");
	}

	void OnMouseUp()
	{
		menu.Click(0);
	}

	void OnMouseExit()
	{
		anim.Play("mainMenuButton");
	}
}
