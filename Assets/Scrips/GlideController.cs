using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class GlideController : MonoBehaviour {

    public Sprite x_1, x_2;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void set(int index)
    {
        if (index >= transform.childCount) return;
        transform.GetChild(index).GetComponent<Image>().sprite = x_2;
    }

    public void reset()
    {
        foreach(Transform child in transform) child.GetComponent<Image>().sprite = x_1;
    }
}
