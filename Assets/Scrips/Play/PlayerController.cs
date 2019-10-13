using UnityEngine;
using UnityEngine.EventSystems;

using System.Collections;

public class PlayerController : MonoBehaviour
{

    public GameController gameController;
    public GameObject Header, Body, Blood;
    public GameObject HandRight, HandLeft, ChildHandRight, ChildHandLeft;

    [HideInInspector]
    public Animator animator;

    public bool homePlayer;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Use this for initialization
    public virtual void Start()
    {
        //TweenPosition twP = TweenPosition.Begin(gameObject, 0.5f, Vector3.zero);
        //twP.method = UITweener.Method.EaseIn;

        if(!homePlayer) PlaySound("itemPopup");

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void disableAnimator()
    {
        animator.enabled = false;
    }

    public virtual void setDefault()
    {

    }

    public void shooting(BaseEventData baseEventData)
    {
        PointerEventData eventData = baseEventData as PointerEventData;
        shooting(eventData.position);
    }

    public virtual void shooting(Vector3 position)
    {

    }

    public virtual void target(GameObject gOb, Vector3 pos, bool left)
    {

    }

    public GameObject copy(GameObject gOb)
    {
        GameObject _gOb = Instantiate(gOb) as GameObject;
        _gOb.transform.SetParent(gOb.transform.parent);
        _gOb.transform.localPosition = gOb.transform.localPosition;
        _gOb.transform.localScale = gOb.transform.localScale;
        _gOb.transform.localRotation = gOb.transform.localRotation;

        TweenAlpha twA = TweenAlpha.Begin(_gOb, 0.3f, 0);
        twA.from = 0.5f;
        twA.SetOnFinished(() =>
        {
            Destroy(_gOb);
        });

        return _gOb;
    }

    public virtual void Die(TargetType type)
    {
        float z = type == TargetType.Left ? -5 : 5;
        Blood.SetActive(true);
        Vector3 pos = Blood.transform.localPosition;
        pos.x = z * 3;
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
                animator.enabled = true;
                animator.SetTrigger("Die");
                animator.SetBool("Normal", false);
            }
        }));
    }

    public virtual void ReLife()
    {
        Blood.SetActive(false);
        animator.enabled = true;
        animator.SetBool("Normal", true);
    }

    public IEnumerator delay(float timeDelay, EventDelegate.Callback action)
    {
        float time = Time.time;
        yield return new WaitForSeconds(timeDelay);
        while (gameController.pause) yield return new WaitForSeconds(0.03f); // pause
        if (gameController.timePause > time) yield return new WaitForSeconds(time + timeDelay - gameController.timePause);
        action();
    }

    public Vector2 getLocalPointPosition(RectTransform rectTransform, PointerEventData eventData)
    {
        Vector2 localPointPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out localPointPosition);
        return localPointPosition;
    }

    public void PlaySound(string path)
    {
        if (!GlobalData.soundOn) return;
        AudioClip audioClip = Resources.Load<AudioClip>("Sounds/" + path);
        AudioSource.PlayClipAtPoint(audioClip, Camera.main.transform.position);
    }
}
