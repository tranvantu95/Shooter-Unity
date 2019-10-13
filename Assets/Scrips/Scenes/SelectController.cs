using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class SelectController : BaseGame
{

    public Toggle[] arena;

	// Use this for initialization
	void Start () {
        if(GlobalData.Arena == -1) GlobalData.Arena = PlayerPrefs.GetInt("Arena");
        arena[GlobalData.Arena].isOn = true;

        AudioSource music = GameObject.FindGameObjectWithTag("BgSound").GetComponent<AudioSource>();
        if (PlayerPrefs.GetInt("musicOn") == 0 && !music.isPlaying) music.Play();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void setArena(int arena)
    {
        GlobalData.Arena = arena;
        PlayerPrefs.SetInt("Arena", arena);
    }

    public void PlayGame()
    {
        LoadScene("Play");
    }
}
