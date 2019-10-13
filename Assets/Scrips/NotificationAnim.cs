using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using System.Collections;

public class NotificationAnim : MonoBehaviour {

    RectTransform rectTransform, parentRectTransform;

    public GameController gameController;
    public bool isShow, isHiding;
    public float delayShow = 0;

    void Awake()
    {
    }

    // Use this for initialization
    void Start () {
        rectTransform = GetComponent<RectTransform>();
        parentRectTransform = transform.parent.GetComponent<RectTransform>();
        //transform.localPosition = getPosLeft();
        if(delayShow > 0)
        {
            GetComponent<Text>().enabled = false;
            StartCoroutine(delay(delayShow, () => {
                GetComponent<Text>().enabled = true;
                show();
            }));
        } 
        else show();
    }

    // Update is called once per frame
    void Update () {
	
	}

    Vector3 getPosLeft()
    {
        Vector3 pos = transform.localPosition;
        pos.x = parentRectTransform.rect.xMin + rectTransform.rect.xMin;
        return pos;
    }

    Vector3 getPosRight()
    {
        Vector3 pos = transform.localPosition;
        pos.x = parentRectTransform.rect.xMax + rectTransform.rect.xMax;
        return pos;
    }

    public void show()
    {
        isShow = true;

        transform.localPosition = getPosLeft();
        Vector3 pos = transform.localPosition;
        pos.x = 0;
        TweenPosition twP = TweenPosition.Begin(gameObject, 1.0f, pos);
        twP.method = UITweener.Method.BounceIn;
        twP.onFinished.Clear();
    }

    public void hide(float duration)
    {
        if (isHiding) return;
        isHiding = true;

        if(duration == 0)
        {
            TweenPosition twP = TweenPosition.Begin(gameObject, 0.5f, getPosRight());
            twP.method = UITweener.Method.Linear;
            twP.AddOnFinished(() => { isShow = false; isHiding = false; Destroy(gameObject); });
            return;
        }
        StartCoroutine(delay(duration, () => {
            TweenPosition twP = TweenPosition.Begin(gameObject, 0.5f, getPosRight());
            twP.method = UITweener.Method.Linear;
            twP.AddOnFinished(() => { isShow = false; isHiding = false; Destroy(gameObject); });
        }));
    }

    public void hide()
    {
        hide(0);
    }

    public void addOnFinished(EventDelegate.Callback action)
    {
        TweenPosition twP = GetComponent<TweenPosition>();
        if (twP == null) return;
        twP.AddOnFinished(action);
    }

    public IEnumerator delay(float timeDelay, EventDelegate.Callback action)
    {
        float time = Time.time;
        yield return new WaitForSeconds(timeDelay);
        while (gameController.pause) yield return new WaitForSeconds(0.03f); // pause
        if (gameController.timePause > time) yield return new WaitForSeconds(time + timeDelay - gameController.timePause);
        action();
    }

}
