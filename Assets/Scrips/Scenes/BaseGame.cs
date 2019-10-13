using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BaseGame : MonoBehaviour {

    public bool isPaused;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnApplicationFocus(bool focusStatus)
    {
        isPaused = focusStatus;
    }

    void OnApplicationPause(bool pauseStatus)
    {
        isPaused = pauseStatus;
    }

    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void ButtonClick()
    {
        PlaySound("buttonClick");
    }

    public void PlaySound(string path)
    {
        if (!GlobalData.soundOn) return;
        AudioClip audioClip = Resources.Load<AudioClip>("Sounds/" + path);
        AudioSource.PlayClipAtPoint(audioClip, Camera.main.transform.position);
    }

    public void RateApp()
    {
        #if UNITY_ANDROID
        //Application.OpenURL("market://details?q=pname:com.zing.zalo");
        Application.OpenURL("market://details?id=net.mobigame.zombietsunami");

        //StartCoroutine(delay(1f, () => {
        //    if(!isPaused) Application.OpenURL("http://play.google.com/store/apps/details?id=net.mobigame.zombietsunami");
        //}));
        #endif
    }

    public void MoreApp()
    {
        #if UNITY_ANDROID
        Application.OpenURL("market://search?q=pub:Mobigame+S.A.R.L.");
        //Application.OpenURL("market://developer?id=Mobigame+S.A.R.L.");

        //StartCoroutine(delay(1f, () => {
        //    if (!isPaused) Application.OpenURL("http://play.google.com/store/apps/developer?id=Mobigame+S.A.R.L.");
        //}));
        #endif
    }

    IEnumerator delay(float timeDelay, EventDelegate.Callback action)
    {
        yield return new WaitForSeconds(timeDelay);
        action();
    }
}
