using UnityEngine;
using System.Collections;

public class HomeController : BaseGame {

	// Use this for initialization
	void Start () {
        AudioSource music = GameObject.FindGameObjectWithTag("BgSound").GetComponent<AudioSource>();
        if (PlayerPrefs.GetInt("musicOn") == 0 && !music.isPlaying) music.Play();
        DontDestroyOnLoad(music.gameObject);
        GlobalData.soundOn = PlayerPrefs.GetInt("soundOn") == 0;
    }

    // Update is called once per frame
    void Update () {
	
	}

}
