using UnityEngine;
using System.Collections;

public class Player2Controller : PlayerController
{

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public override void shooting(Vector3 position)
    {
        TweenRotation twRBody = TweenRotation.Begin(Body, 0.05f, Quaternion.Euler(0, 0, -10));
        twRBody.quaternionLerp = true;
        twRBody.method = UITweener.Method.EaseIn;
        twRBody.style = UITweener.Style.PingPong;
        twRBody.onLoop = () =>
        {
            twRBody.style = UITweener.Style.Once;
        };
    }
}
