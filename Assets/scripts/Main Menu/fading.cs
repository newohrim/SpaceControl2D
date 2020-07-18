using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class fading : MonoBehaviour {

	public GameObject leaderboardButton;
	public int firstRun = 0;

	public void Fade(int n)
	{
		switch(n)
		{
			case 0:
				firstRun = PlayerPrefs.GetInt ("firstRun");
                //PlayerPrefs.DeleteAll (); <-- Код, который удаляет данные из реестра.  

                if (firstRun == 0) 
				{
                    firstRun = 1;
                    PlayerPrefs.SetInt ("firstRun", firstRun);
                    // Тут ваш код, который выполнится при первом запуске игры. У меня переход на сцену, где создается персонаж.
                    SceneManager.LoadScene (2);
                } 
				else 
				{
                    //Код, который будет выполняться до конца жизни.
                    SceneManager.LoadScene (1);
                }
			break;
		}
	}

	public void DestroyLeaderboardButton()
    {
        Destroy(leaderboardButton);
    }
}
