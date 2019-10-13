using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using System.Collections;

public class SettingController : BaseGame
{

    public Button Sound, Music;
    public bool soundOn, musicOn;

    public AudioSource music;

    void Awake()
    {
        soundOn = PlayerPrefs.GetInt("soundOn") == 0;
        musicOn = PlayerPrefs.GetInt("musicOn") == 0;

        music = GameObject.FindGameObjectWithTag("BgSound").GetComponent<AudioSource>();
    }

    // Use this for initialization
    void Start () {

        Toggle(Sound, soundOn);
        Toggle(Music, musicOn);

    }

    // Update is called once per frame
    void Update () {
	
	}

    public void ToggleSound()
    {
        if(soundOn)
        {
            soundOn = false;
        }
        else
        {
            soundOn = true;
        }

        Toggle(Sound, soundOn);
        PlayerPrefs.SetInt("soundOn", soundOn ? 0 : 1);
        GlobalData.soundOn = soundOn;
    }

    public void ToggleMusic()
    {
        if (musicOn)
        {
            musicOn = false;
            music.Pause();
        }
        else
        {
            musicOn = true;
            music.Play();
        }

        Toggle(Music, musicOn);
        PlayerPrefs.SetInt("musicOn", musicOn ? 0 : 1);
    }

    void Toggle(Button button, bool isOn)
    {
        if(isOn)
        {
            button.transform.GetChild(0).gameObject.SetActive(false);
            button.transform.GetChild(1).gameObject.SetActive(true);
            button.targetGraphic = button.transform.GetChild(1).GetComponent<Graphic>();
        }
        else
        {
            button.transform.GetChild(0).gameObject.SetActive(true);
            button.transform.GetChild(1).gameObject.SetActive(false);
            button.targetGraphic = button.transform.GetChild(0).GetComponent<Graphic>();
        }
    }
}
