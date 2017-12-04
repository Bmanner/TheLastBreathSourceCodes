using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class TimeManager : MonoBehaviour {

    public enum TimeUpdateType
    {
        Hour,
        Day
    }

    public static readonly string AM = "AM";
    public static readonly string PM = "PM";

    public float gameHourInRealSec = 20;

    public static int GameHour
    {
        get
        {
            var returnHour = gameHour % 12;

            if (!isAM && returnHour == 0)
                return 12; // 12PM 인 경우
            else
                return returnHour;
        }
    }
    public static int GameDay
    {
        get { return gameDay; }
    }
    public static bool IsAM
    {
        get { return isAM; }
    }
    public static bool HasUpdate
    {
        get
        {
            if (hasUpdate)
            {
                hasUpdate = false;
                return true;
            }
            else
                return hasUpdate;
        }
    }
    public static bool HasAMPMUpdate
    {
        get
        {
            if (hasAMPMUpdate)
            {
                hasAMPMUpdate = false;
                return true;
            }
            else
                return hasAMPMUpdate;
        }
    }
    public static bool HasDayUpdate
    {
        get
        {
            if (hasDayUpdate)
            {
                hasDayUpdate = false;
                return true;
            }
            else
                return hasDayUpdate;
        }
    }

    public static int GetOriginalGameHour()
    {
        return gameHour;
    }
}

public partial class TimeManager
{
    static int gameHour;
    static int gameDay;
    static bool isAM;
    static bool hasUpdate;
    static bool hasAMPMUpdate;
    static bool hasDayUpdate;

    List<ITimeUpdateListener> dayUpdateListners = new List<ITimeUpdateListener>();
    List<ITimeUpdateListener> hourUpdateListners = new List<ITimeUpdateListener>();

    float timeIncreaser = 0;

    // Use this for initialization
    void Awake()
    {
        gameHour = 1;   // TODO : 시간 세이브로드
        gameDay = 1;    // TODO : 시간 세이브로드
        isAM = true;    // TODO : 시간 세이브로드

        timeIncreaser = 0;

        hasUpdate = false;

        //gameSecInRealSec = StandardVariables.;

    }

    void Start()
    {
        // 메트로 씬에서만 쓰이는 초기화 내용
        if(SceneManager.GetActiveScene().name == "1.MetroScene")
        {
            dayUpdateListners.AddRange(GameObject.Find("hydroponics").transform.GetComponentsInChildren<Grower>());
        }

        dayUpdateListners.Add(GameObject.FindGameObjectWithTag("FineDustManager").GetComponent<FineDustManager>());
        hourUpdateListners.Add(GameObject.FindWithTag("Player").GetComponent<PlayerInfo>());
    }
    
    // Update is called once per frame
    void Update()
    {
        timeIncreaser += Time.deltaTime;
        if (timeIncreaser > gameHourInRealSec)
        {
            timeIncreaser = 0;
            gameHour++;
            foreach (var listner in hourUpdateListners)
                listner.TimeUpdate();
            hasUpdate = true;

            var changedAMPM = gameHour / 12 % 2 == 0;
            hasAMPMUpdate = isAM != changedAMPM;
            if (hasAMPMUpdate)
                isAM = changedAMPM;

            var changedDay = gameHour / 24 + 1;
            hasDayUpdate = gameDay != changedDay;
            if (hasDayUpdate)
            {
                gameDay = changedDay;
                foreach (var obj in dayUpdateListners)
                    obj.TimeUpdate();
            }
        }
       
    }
}