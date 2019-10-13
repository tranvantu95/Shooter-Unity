using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using System.Collections;
using System.Collections.Generic;

public enum Position
{
    LeftBot, LeftMid, LeftTop, RightBot, RightMid, RightTop
}

public class GameController : BaseGame {

    public AdsController adsController;
    public ScoreController scoreController;
    public GlideController glideController;

    public GameObject Canvas;
    public GameObject Player;
    public GameObject Targets, Target;
    public GameObject Notification;
    public GameObject currentNotification;
    Vector3 NotificationPosDefault;
    Transform Notifications;
    public GameObject Loser;
    public Image Background;
    public Text Score, TimeLife;

    public float timeLife = 0, timePause;
    public bool started = false;
    public bool stopped = false;
    public bool pause = false;
    public bool action { get { return started && !stopped; } }
    public bool running { get { return !pause && action; } }
    public bool learning;
    public bool _lock;
    IEnumerator IELoser;

    int score = 0;
    int ban_truot = 0;
    public int totalTarget = 0, totalTargetDied = 0;
    public int totalTargetPos { get { return totalTarget - totalTargetDied; } }
    public List<Position> TargetPositions = new List<Position>();

    int gameCount;

    void Awake()
    {

    }

	// Use this for initialization
	void Start () {
        NotificationPosDefault = currentNotification.transform.localPosition;
        Notifications = currentNotification.transform.parent;

        learning = true;
        //StartCoroutine(createRandomTarget());
        StartCoroutine(delay(0.5f, () => { StartCoroutine(createRandomTarget()); }));
        Player.GetComponent<Animator>().enabled = false;
        //Score.gameObject.SetActive(false);
        scoreController.gameObject.SetActive(false);
        glideController.gameObject.SetActive(false);
        TimeLife.gameObject.SetActive(false);

        CheckArena();
        GameObject.FindGameObjectWithTag("BgSound").GetComponent<AudioSource>().Pause();
    }

    // Update is called once per frame
    void Update () {
        //Debug.LogError(Random.Range(0, 1).ToString());
        if (!running) return;
        timeLife += Time.deltaTime;
        TimeLife.text = "Time: " + ((int)timeLife).ToString();
        //Debug.LogError(((int)timeLife).ToString());
    }

    void CheckArena()
    {
        if(GlobalData.Arena >= 0) Background.sprite = Resources.Load<Sprite>("Background/bg_" + GlobalData.Arena.ToString());
    }

    public void MainTouch(BaseEventData baseEventData)
    {
        if (_lock || pause || learning || adsController.isShowing()) return;

        if (!started)
        {
            StartGame();
            PlaySound("buttonShoot");
        }
        else if(!stopped)
        {
            glideController.set(ban_truot);
            ban_truot++;

            if (ban_truot >= 3) EndGame(false, TargetType.Left);

            Player.GetComponent<PlayerController>().shooting(baseEventData);
            PlaySound("miss1");
        }
        else
        {
            ReStartGame();
            PlaySound("shotgunReload");
        }
    }

    IEnumerator createRandomTarget_2()
    {
        int maxTarget = 3 + (int)(timeLife / 12);
        if (maxTarget > 18) maxTarget = 18;
        int minTarget = 1 + (int)(timeLife / 36);
        if (minTarget > 6) minTarget = 6;
        int numberTarget = Random.Range(minTarget, maxTarget + 1);

        for (int i = 0; i < numberTarget; i++)
        {
            //while(totalTargetPos >= 6 && action) yield return new WaitForSeconds(0.5f);
            List<Position> positions = new List<Position>();
            do
            {
                positions.AddRange(new Position[] { Position.LeftBot, Position.LeftMid, Position.LeftTop, Position.RightBot, Position.RightMid, Position.RightTop });
                foreach (Position targetPosition in TargetPositions) positions.Remove(targetPosition);
                if (positions.Count == 0) yield return new WaitForSeconds(0.5f);
            } while (positions.Count == 0 && action);

            int j = Random.Range(0, positions.Count);
            Position position = positions[j];

            if (action) createTarget(position);
            else break;
            yield return new WaitForSeconds(Random.Range(0.1f, 0.2f));
            while (pause) yield return new WaitForSeconds(0.03f); // pause
        }

        float delay = 1 + timeLife / 60;
        if (delay > 3) delay = 3;

        float time = Time.time;
        yield return new WaitForSeconds(delay);
        while (pause) yield return new WaitForSeconds(0.03f); // pause
        if (timePause > time) yield return new WaitForSeconds(time + delay - timePause);

        if (action) StartCoroutine(createRandomTarget_2());
    }

    IEnumerator createRandomTarget()
    {
        List<Position> positions = new List<Position>();
        positions.AddRange(new Position[] { Position.LeftBot, Position.LeftMid, Position.LeftTop, Position.RightBot, Position.RightMid, Position.RightTop });
        foreach(Position position in TargetPositions) positions.Remove(position);

        int maxTarget = 1 + (int)(timeLife / 12);
        if (maxTarget > 6) maxTarget = 6;
        if (maxTarget > positions.Count) maxTarget = positions.Count;
        int minTarget = 1 + (int)(timeLife / 36);
        if (minTarget > 3) minTarget = 3;
        if (minTarget > maxTarget) minTarget = maxTarget;
        int numberTarget = Random.Range(minTarget, maxTarget + 1);

        if (learning) numberTarget = totalTarget + 1;

        for (int i = 0; i < numberTarget; i++)
        {
            int j = Random.Range(0, positions.Count);
            Position position = positions[j];
            positions.RemoveAt(j);

            if (learning || action) createTarget(position);
            else break;
            yield return new WaitForSeconds(Random.Range(0.1f, 0.2f));
            while (pause) yield return new WaitForSeconds(0.03f); // pause
        }

        if(!learning)
        {
            float delay = 1 + timeLife / 60;
            if (delay > 2) delay = 2;

            float time = Time.time;
            yield return new WaitForSeconds(delay);
            while (pause) yield return new WaitForSeconds(0.03f); // pause
            if(timePause > time) yield return new WaitForSeconds(time + delay - timePause);

            if (action) StartCoroutine(createRandomTarget());
        }
    }

    void createTarget(Position position)
    {
        GameObject target = Instantiate(Target) as GameObject;
        target.transform.SetParent(Targets.transform);
        target.transform.localScale = Vector3.one;
        TargetController targetController = target.GetComponent<TargetController>();

        targetController.gameController = this;
        targetController.Targets = Targets;
        targetController.Player = Player;
        targetController.position = position;

        totalTarget++;
        TargetPositions.Add(position);
        if (totalTarget % 15 == 0) targetController.harmless = true;

        AddListener(target, EventTriggerType.PointerDown, (BaseEventData baseEventData) =>
        {
            if(learning)
            {
                Player.GetComponent<PlayerController>().shooting(baseEventData);
                PlaySound("shotgun1");
                targetController.Die();
                totalTargetDied++;
                TargetPositions.Remove(position);

                if (totalTargetDied >= 3)
                {
                    learning = false;
                    //Score.gameObject.SetActive(true);
                    scoreController.gameObject.SetActive(true);
                    glideController.gameObject.SetActive(true);
                    TimeLife.gameObject.SetActive(true);
                    totalTarget = 0;
                    totalTargetDied = 0;
                    StartGame();
                }
                else if(TargetPositions.Count == 0)
                {
                    StartCoroutine(delay(0.5f, () => { StartCoroutine(createRandomTarget()); }));                    
                }
                return;
            }

            if (!running || targetController.died) return;
            Player.GetComponent<PlayerController>().shooting(baseEventData);
            //PlaySound("shotgun1");
            targetController.Die();

            totalTargetDied++;
            TargetPositions.Remove(position);

            if (targetController.harmless)
            {
                glideController.set(ban_truot);
                ban_truot++;

                if (ban_truot >= 3) EndGame(false, TargetType.Left);
                PlaySound("miss1");
            }
            else
            {
                score += 5;
                //Score.text = "Score: " + score.ToString();
                scoreController.setScore(score.ToString());
                PlaySound("shotgun1");
            }
        });
    }

    void setPos(GameObject gOb, Position position)
    {
        Vector2 pos = new Vector2();
        switch (position)
        {
            case Position.LeftBot:
                pos.x = Targets.GetComponent<RectTransform>().rect.xMin + gOb.GetComponent<RectTransform>().rect.xMin;
                pos.y = Targets.GetComponent<RectTransform>().rect.yMin - gOb.GetComponent<RectTransform>().rect.yMin;

                break;

            case Position.LeftMid:
                pos.x = Targets.GetComponent<RectTransform>().rect.xMin + gOb.GetComponent<RectTransform>().rect.xMin;
                pos.y = 0;

                break;

            case Position.LeftTop:
                pos.x = Targets.GetComponent<RectTransform>().rect.xMin + gOb.GetComponent<RectTransform>().rect.xMin;
                pos.y = Targets.GetComponent<RectTransform>().rect.yMax - gOb.GetComponent<RectTransform>().rect.yMax;

                break;

            case Position.RightBot:
                pos.x = Targets.GetComponent<RectTransform>().rect.xMax + gOb.GetComponent<RectTransform>().rect.xMax;
                pos.y = Targets.GetComponent<RectTransform>().rect.yMin - gOb.GetComponent<RectTransform>().rect.yMin;

                break;

            case Position.RightMid:
                pos.x = Targets.GetComponent<RectTransform>().rect.xMax + gOb.GetComponent<RectTransform>().rect.xMax;
                pos.y = 0;

                break;

            case Position.RightTop:
                pos.x = Targets.GetComponent<RectTransform>().rect.xMax + gOb.GetComponent<RectTransform>().rect.xMax;
                pos.y = Targets.GetComponent<RectTransform>().rect.yMax - gOb.GetComponent<RectTransform>().rect.yMax;

                break;
        }

        gOb.transform.localPosition = pos;
        gOb.GetComponent<TargetController>().position = position;
    }

    GameObject createNotification()
    {
        GameObject clone = Instantiate(Notification) as GameObject;
        clone.transform.SetParent(Notifications);
        clone.transform.localPosition = NotificationPosDefault;
        clone.transform.localScale = Vector3.one;
        clone.GetComponent<NotificationAnim>().gameController = this;

        return clone;
    }

    public void ReStartGame()
    {
        if (!started) return;

        started = false;
        stopped = false;
        pause = false;
        timeLife = 0;
        score = 0;
        ban_truot = 0;
        totalTarget = 0;
        totalTargetDied = 0;
        TargetPositions.Clear();

        ClearTargets();
        Player.GetComponent<PlayerController>().ReLife();
        glideController.reset();
        //Score.text = "Score: " + score.ToString();
        scoreController.setScore(score.ToString());
        TimeLife.text = "Time: " + ((int)timeLife).ToString();

        if (currentNotification != null) currentNotification.GetComponent<NotificationAnim>().hide();
        currentNotification = createNotification();
        currentNotification.GetComponent<Text>().text = "Tap To Play!";

        if (Loser.activeSelf)
        {
            Loser.SetActive(false);
            StopCoroutine(IELoser);
        }

        // Ads
        CheckAds();
    }

    public void StartGame()
    {
        started = true;
        GlobalData.gameCount++;

        StartCoroutine(delay(3.0f, () => { StartCoroutine(createRandomTarget_2()); }));

        if (Player.GetComponent<Animator>().enabled)
        {
            Player.GetComponent<Animator>().enabled = false;
            Player.GetComponent<PlayerController>().setDefault();
        }

        if (currentNotification != null) currentNotification.GetComponent<NotificationAnim>().hide();
        currentNotification = createNotification();
        currentNotification.GetComponent<Text>().text = "Ready!";
        currentNotification.GetComponent<NotificationAnim>().hide(2);
    }

    public void EndGame(bool die, TargetType type)
    {
        stopped = true;
        if(die) Player.GetComponent<PlayerController>().Die(type);
        StopCoroutine(createRandomTarget());

        _lock = true;
        float timeDelay = die ? 2f : 0;

        StartCoroutine(delay(timeDelay, () =>
        {
            Loser.SetActive(true);

            IELoser = delay(5f, () =>
            {
                if (Loser.activeSelf) Loser.SetActive(false);
            });

            StartCoroutine(IELoser);
        }));

        StartCoroutine(delay(timeDelay + 1f, () =>
        {
            _lock = false;

            if (currentNotification != null) currentNotification.GetComponent<NotificationAnim>().hide();
            currentNotification = createNotification();
            currentNotification.GetComponent<Text>().text = "Tap To Restart!";
        }));      
    }

    public void PauseGame()
    {
        pause = true;
        timePause = Time.time;
    }

    public void ResumeGame()
    {
        pause = false; 
    }

    public void TogglePause()
    {
        if (pause) ResumeGame();        
        else PauseGame();
    }

    public void GoSelectArena()
    {
        LoadScene("Select");
    }

    public void GoHome()
    {
        LoadScene("Home");
    }

    public void CheckAds()
    {
        //if (GlobalData.gameCount != GlobalData.gameCountGoHome && GlobalData.gameCount % 3 == 0)
        //{
        //    GlobalData.gameCountGoHome = GlobalData.gameCount;
        //    adsController.ShowAds();
        //}
    }

    void ClearTargets()
    {
        foreach (Transform child in Targets.transform) child.GetComponent<TargetController>().Die();
    }

    IEnumerator delay(float timeDelay, EventDelegate.Callback action)
    {
        float time = Time.time;
        yield return new WaitForSeconds(timeDelay);
        while (pause) yield return new WaitForSeconds(0.03f); // pause
        if (timePause > time) yield return new WaitForSeconds(time + timeDelay - timePause);
        action();
    }

    private void AddListener(GameObject gOb, EventTriggerType type, UnityAction<BaseEventData> call)
    {
        EventTrigger eventTrigger = gOb.GetComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback = new EventTrigger.TriggerEvent();
        entry.callback.AddListener(call);

        eventTrigger.triggers.Add(entry);
    }
}
