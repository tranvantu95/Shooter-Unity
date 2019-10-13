using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public enum TargetType
{
    Left, Right
}

public class TargetController : PlayerController {

    public GameObject GunRight, GunLeft;

    //public GameController gameController;
    public GameObject Targets, Player;
    public Position position;
    public TargetType type
    {
        get { return (position == Position.LeftBot || position == Position.LeftMid || position == Position.LeftTop) ? TargetType.Left : TargetType.Right; }
    }
    public bool died = false;
    public bool harmless = false;

    RectTransform rectTransform, targetsRecTransform;
    PlayerController playerController;

    float timeLife;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

	// Use this for initialization
	public override void Start () {
        targetsRecTransform = Targets.GetComponent<RectTransform>();
        playerController = Player.GetComponent<PlayerController>();
        setPos();

        Show();

        if (harmless)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(true);
        } 
        else
        {
            if (type == TargetType.Left)
            {
                target(HandLeft, playerController.Header.transform.position, true);
            }
            else
            {
                target(HandRight, playerController.Header.transform.position, false);
            }
        }
        
        StartCoroutine(delay(2, () =>
        {
            if (died || !gameController.action) return;

            if (harmless)
            {
                Hide();
                gameController.totalTargetDied++;
                gameController.TargetPositions.Remove(position);
            }
            else 
            {
                shooting(playerController.Header.transform.position);
                PlaySound("enemyFireMagnum");
                gameController.EndGame(true, type);
                StartCoroutine(delay(1, () => { Win(); }));
            }
        }));
        
    }
	
	// Update is called once per frame
	void Update () {
        //if (died || !gameController.running) return;

        //timeLife += Time.deltaTime;
        //if(timeLife > 2)
        //{
        //    Shooting();
        //    gameController.EndGame(true, type);
        //}
    }

    public override void shooting(Vector3 position)
    {
        GameObject gOb, bullet;
        Vector3 deltaRotHand, deltaRotBody;

        if (position.x > transform.position.x)
        {
            gOb = HandLeft;

            deltaRotHand = gOb.transform.eulerAngles;
            deltaRotHand.z += 25;

            deltaRotBody = Body.transform.localEulerAngles;
            deltaRotBody.z += 15;

            bullet = GunLeft.transform.GetChild(1).gameObject;
        }
        else
        {
            gOb = HandRight;

            deltaRotHand = gOb.transform.eulerAngles;
            deltaRotHand.z -= 25;

            deltaRotBody = Body.transform.localEulerAngles;
            deltaRotBody.z -= 15;

            bullet = GunRight.transform.GetChild(1).gameObject;
        }

        TweenRotation twRHand = TweenRotation.Begin(gOb, 0.05f, Quaternion.Euler(deltaRotHand));
        //twRHand.worldSpace = true;
        twRHand.method = UITweener.Method.EaseIn;
        twRHand.style = UITweener.Style.PingPong;
        twRHand.onLoop = () =>
        {
            twRHand.style = UITweener.Style.Once;
        };

        TweenRotation twRBody = TweenRotation.Begin(Body, 0.05f, Quaternion.Euler(deltaRotBody));
        twRBody.quaternionLerp = true;
        twRBody.method = UITweener.Method.EaseIn;
        twRBody.style = UITweener.Style.PingPong;
        twRBody.onLoop = () =>
        {
            twRBody.style = UITweener.Style.Once;
        };

        if (!bullet.activeSelf) bullet.SetActive(true);
        StartCoroutine(delay(0.1f, () => { if (bullet.activeSelf) bullet.SetActive(false); }));
    }

    public override void target(GameObject gOb, Vector3 pos, bool right)
    {
        int sign = right ? 1 : -1;
        Vector3 rot = gOb.transform.eulerAngles;
        rot.z = Vector2.Angle(Vector2.down, pos - gOb.transform.position) * sign;
        Vector3 rotChild = new Vector3(0, 0, rot.z / 180 * 15);
        rot.z -= rotChild.z;
        gOb.transform.eulerAngles = rot;
        gOb.transform.GetChild(0).localEulerAngles = rotChild;
    }

    public void setPos()
    {
        Vector2 pos = new Vector2();
        switch (position)
        {
            case Position.LeftBot:
                pos.x = targetsRecTransform.rect.xMin + rectTransform.rect.xMin;
                pos.y = targetsRecTransform.rect.yMin - rectTransform.rect.yMin;

                break;

            case Position.LeftMid:
                pos.x = targetsRecTransform.rect.xMin + rectTransform.rect.xMin;
                pos.y = 0;

                break;

            case Position.LeftTop:
                pos.x = targetsRecTransform.rect.xMin + rectTransform.rect.xMin;
                pos.y = targetsRecTransform.rect.yMax - rectTransform.rect.yMax;

                break;

            case Position.RightBot:
                pos.x = targetsRecTransform.rect.xMax + rectTransform.rect.xMax;
                pos.y = targetsRecTransform.rect.yMin - rectTransform.rect.yMin;

                break;

            case Position.RightMid:
                pos.x = targetsRecTransform.rect.xMax + rectTransform.rect.xMax;
                pos.y = 0;

                break;

            case Position.RightTop:
                pos.x = targetsRecTransform.rect.xMax + rectTransform.rect.xMax;
                pos.y = targetsRecTransform.rect.yMax - rectTransform.rect.yMax;

                break;
        }

        transform.localPosition = pos;
    }

    public void Show()
    {
        if (type == TargetType.Left)
        {
            TweenPosition twP = TweenPosition.Begin(gameObject, 0.5f, transform.localPosition + new Vector3(rectTransform.rect.width, 0, 0));
        }
        else
        {
            TweenPosition twP = TweenPosition.Begin(gameObject, 0.5f, transform.localPosition - new Vector3(rectTransform.rect.width, 0, 0));
        }
    }

    public void Hide()
    {
        TweenPosition twP;
        if (type == TargetType.Left)
        {
            twP = TweenPosition.Begin(gameObject, 0.5f, transform.localPosition - new Vector3(rectTransform.rect.width, 0, 0));
        }
        else
        {
            twP = TweenPosition.Begin(gameObject, 0.5f, transform.localPosition + new Vector3(rectTransform.rect.width, 0, 0));
        }
        twP.SetOnFinished(() => {
            Destroy(gameObject);
        });
    }

    public void Win()
    {
        TweenPosition twP = TweenPosition.Begin(gameObject, 0.5f, transform.localPosition + new Vector3(0, 25, 0));
        twP.method = UITweener.Method.EaseIn;
        twP.style = UITweener.Style.PingPong;

        PlaySound("enemyHitXmas1");
        //GetComponent<AudioSource>().Play();
    }

    public void Die()
    {
        died = true;
        GetComponent<Image>().raycastTarget = false;

        if((harmless || !gameController.action) && !gameController.learning)
        {
            TweenAlpha twA = TweenAlpha.Begin(gameObject, 0.5f, 0.0f);
            twA.from = 0.5f;
            twA.SetOnFinished(() =>
            {
                Destroy(gameObject);
            });

            return;
        }

        float z = type == TargetType.Right ? -15 : 15;
        Blood.SetActive(true);
        Vector3 pos = Blood.transform.localPosition;
        pos.x = 15 * Mathf.Sign(z);
        Blood.transform.localPosition = pos;

        TweenRotation twR = TweenRotation.Begin(Body, 0.05f, Quaternion.Euler(0, 0, z));
        twR.quaternionLerp = true;
        twR.method = UITweener.Method.EaseIn;
        twR.style = UITweener.Style.PingPong;
        twR.onLoop = () =>
        {
            twR.style = UITweener.Style.Once;
        };
        twR.SetOnFinished(() => {
            TweenAlpha twA = TweenAlpha.Begin(gameObject, 0.5f, 0.0f);
            //twA.from = 0.5f;
            twA.SetOnFinished(() =>
            {
                Destroy(gameObject);
            });
        });
    }

    //IEnumerator delay(float timeDelay, EventDelegate.Callback action)
    //{
    //    float time = Time.time;
    //    yield return new WaitForSeconds(timeDelay);
    //    while (gameController.pause) yield return new WaitForSeconds(0.03f); // pause
    //    if (gameController.timePause > time) yield return new WaitForSeconds(time + timeDelay - gameController.timePause);
    //    action();
    //}

}
