using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FineDustManager : MonoBehaviour, ITimeUpdateListener {
    [System.Serializable]
    public struct DustState
    {
        public int Boundary;
        public Color Color;
        public int Probability;
    }

    public DustState[] DustStates;

    int targetDustStateIndex = 0;
    int curDustStateIndex = 0;
    int dustValue = 0;

    //readonly float LERP_TIME = 2f;

    void ITimeUpdateListener.TimeUpdate()
    {
        DustDailyRefresh();
    }

	// Use this for initialization
	void Start () {
        dustValue = 100;

        StartCoroutine(LateStart());
	}

    IEnumerator LateStart()
    {
        yield return null;

        DustDailyRefresh(); // Late Start로 하지 않으면 컬러부분에서 눌 엑셉션 오류 뜸. TODO : 추후 공부 필요
    }

    IEnumerator LerpDustValue(int startValue, int targetValue)
    {
        int multiplier = 1;

        bool isIncrease = targetValue > startValue;
        var gap = Mathf.Abs(targetValue - startValue);
        //var updateBoundary = LERP_TIME / gap;

        if (gap > 120)
            multiplier = 2;
        if (gap > 240)
            multiplier = 3;

        //float counter = 0f;

        while(dustValue != targetValue)
        {
            gap = Mathf.Abs(dustValue - targetValue);
            if (gap < 5)
                multiplier = 1;

            dustValue = isIncrease ? dustValue + 1 * multiplier : dustValue - 1 * multiplier;
            //counter = 0f;
            CheckCurStateWithDustVal();

            UIController.Instance.ChangeDustText(dustValue, DustStates[curDustStateIndex].Color);

            //counter += Time.deltaTime;

            yield return null;
        }

        yield return null;
    }

    void DustDailyRefresh()
    {
        int randomNum = Random.Range(1, 100);
        int accumulateP = 0;
        for (int i = 0; i < DustStates.Length; i++)
        {
            if (randomNum > accumulateP && randomNum <= accumulateP + DustStates[i].Probability)
            {
                targetDustStateIndex = i;
                break;
            }
            accumulateP += DustStates[i].Probability;
        }

        randomNum = Random.Range(targetDustStateIndex == 0 ? 0 : DustStates[targetDustStateIndex - 1].Boundary + 1, DustStates[targetDustStateIndex].Boundary);

        if (randomNum != dustValue)
            StartCoroutine(LerpDustValue(dustValue, randomNum));
    }

    void CheckCurStateWithDustVal()
    {
        for(int i = 0; i < DustStates.Length; i++)
        {
            if (dustValue < DustStates[i].Boundary)
            {
                curDustStateIndex = i;
                break;
            }
        }
    }
}
