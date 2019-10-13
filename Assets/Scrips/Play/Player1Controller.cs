using UnityEngine;
using UnityEngine.EventSystems;

using System.Collections;


public class Player1Controller : PlayerController {

    public GameObject GunRight, GunLeft;

    Vector3 HandRightRotDefault, HandLeftRotDefault, ChildHandRightRotDefault, ChildHandLeftRotDefault, GunRightRotDefault, GunLeftRotDefault;

    float timeRotGun, oldTimeRotGun;

    bool animPlaying;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Use this for initialization
    public override void Start () {
        base.Start();

        HandRightRotDefault = HandRight.transform.localEulerAngles;
        HandLeftRotDefault = HandLeft.transform.localEulerAngles;
        ChildHandRightRotDefault = ChildHandRight.transform.localEulerAngles;
        ChildHandLeftRotDefault = ChildHandLeft.transform.localEulerAngles;
        GunRightRotDefault = GunRight.transform.localEulerAngles;
        GunLeftRotDefault = GunLeft.transform.localEulerAngles;

    }

    // Update is called once per frame
    void Update () {
        if (homePlayer || !gameController.started && !gameController.learning)
        {
            timeRotGun += Time.deltaTime;
            if(timeRotGun - oldTimeRotGun > 7)
            {
                oldTimeRotGun = timeRotGun;
                animator.SetTrigger("RotateGun");
            }
        }

    }

    public override void setDefault()
    {
        HandRight.transform.localEulerAngles = HandRightRotDefault;
        HandLeft.transform.localEulerAngles = HandLeftRotDefault;
        ChildHandRight.transform.localEulerAngles = ChildHandRightRotDefault;
        ChildHandLeft.transform.localEulerAngles = ChildHandLeftRotDefault;
        GunRight.transform.localEulerAngles = GunRightRotDefault;
        GunLeft.transform.localEulerAngles = GunLeftRotDefault;
    }

    public override void shooting(Vector3 position)
    {
        GameObject gOb, bullet;
        Vector3 deltaRotHand, deltaRotBody;

        if(!animPlaying)
        {
            GameObject clone = copy(gameObject);
            clone.GetComponent<PlayerController>().Header.SetActive(false);
            clone.GetComponent<PlayerController>().enabled = false;
        }

        if (position.x > transform.position.x)
        {
            if (!animPlaying) HandRight.transform.localEulerAngles = HandRightRotDefault;
            if (!animPlaying) ChildHandRight.transform.localEulerAngles = ChildHandRightRotDefault;

            gOb = HandLeft;
            target(gOb, position, true);

            deltaRotHand = gOb.transform.eulerAngles;
            deltaRotHand.z += 15;

            deltaRotBody = Body.transform.localEulerAngles;
            deltaRotBody.z += 5;

            //GetComponent<Animator>().SetTrigger("Quay Trai");

            bullet = GunLeft.transform.GetChild(0).gameObject;
        }
        else
        {
            if (!animPlaying) HandLeft.transform.localEulerAngles = HandLeftRotDefault;
            if (!animPlaying) ChildHandLeft.transform.localEulerAngles = ChildHandLeftRotDefault;

            gOb = HandRight;
            target(gOb, position, false);

            deltaRotHand = gOb.transform.eulerAngles;
            deltaRotHand.z -= 15;

            deltaRotBody = Body.transform.localEulerAngles;
            deltaRotBody.z -= 5;

            //GetComponent<Animator>().SetTrigger("Quay Phai");

            bullet = GunRight.transform.GetChild(0).gameObject;
        }

        animPlaying = true;

        TweenRotation twRHand = TweenRotation.Begin(gOb, 0.05f, Quaternion.Euler(deltaRotHand));
        //twRHand.worldSpace = true;
        twRHand.method = UITweener.Method.EaseIn;
        twRHand.style = UITweener.Style.PingPong;
        twRHand.onLoop = () =>
        {
            twRHand.style = UITweener.Style.Once;
        };
        twRHand.SetOnFinished(() => { animPlaying = false; });

        Body.transform.localEulerAngles = Vector3.zero;
        TweenRotation twRBody = TweenRotation.Begin(Body, 0.05f, Quaternion.Euler(deltaRotBody));
        twRBody.quaternionLerp = true;
        twRBody.method = UITweener.Method.EaseIn;
        twRBody.style = UITweener.Style.PingPong;
        twRBody.onLoop = () =>
        {
            twRBody.style = UITweener.Style.Once;
        };

        //animator.SetTrigger("Attack LB");

        if(!bullet.activeSelf) bullet.SetActive(true);
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

        //if (Mathf.Abs(rot.z) < 90) child.localEulerAngles = Vector3.zero;
        //else if (rot.z > 0) child.localEulerAngles = new Vector3(0, 0, 10);
        //else child.localEulerAngles = new Vector3(0, 0, -10);

    }

    public override void Die(TargetType type)
    {
        float z = type == TargetType.Left ? -15 : 15;
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

        StartCoroutine(delay(1, () => {
            if (gameController.started && gameController.stopped)
            {
                animator.SetTrigger("Die");
                animator.SetBool("Normal", false);
                animator.enabled = true;
            }
        }));
    }

    public override void ReLife()
    {
        base.ReLife();

        //timeRotGun = oldTimeRotGun = 0;
    }

    public void RotateGun()
    {
        TweenRotation twRGunR = TweenRotation.Begin(GunRight, 1, Quaternion.Euler(0, 0, -360 * 3));
        twRGunR.quaternionLerp = true;

        TweenRotation twRGunL = TweenRotation.Begin(GunLeft, 1, Quaternion.Euler(0, 0, 360 * 3));
        twRGunL.quaternionLerp = true;
    }

}
