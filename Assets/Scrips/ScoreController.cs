using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class ScoreController : MonoBehaviour {

    public GameObject numberDefault;
    public Sprite[] numberSprites;

    void Awake()
    {
        numberDefault.transform.SetParent(null);
    }

	// Use this for initialization
	void Start () {
        setScore("0");
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void setScore(string Score)
    {
        clearScore();

        foreach (char score in Score)
        {
            GameObject number = Instantiate(numberDefault) as GameObject;
            number.GetComponent<Image>().sprite = numberSprites[int.Parse(score.ToString())];
            number.transform.SetParent(gameObject.transform);
            number.transform.localScale = Vector3.one;
        }
    }

    public void clearScore()
    {
        int childCount = transform.childCount;
        for (int i = 1; i < childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}
