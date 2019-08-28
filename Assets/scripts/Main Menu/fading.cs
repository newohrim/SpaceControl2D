using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class fading : MonoBehaviour {

	public GameObject leaderboardButton;

	public void Fade(int n)
	{
		switch(n)
		{
			case 0:
				SceneManager.LoadScene(1);
			break;
		}
	}

	public void DestroyLeaderboardButton()
    {
        Destroy(leaderboardButton);
    }
}
