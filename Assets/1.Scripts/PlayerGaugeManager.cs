using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGaugeManager : MonoBehaviour {
    /*
    public Image GuageBGImage;
    public Image GaugeImage;

    public float GaugeSpacing;
    public int GaugeAmount;
    public int MaxHealth;
    */
#if UNITY_EDITOR
    [ReadOnly]
#endif
    public int CurrentAmount;
    public float lerpDuration = 1f;
    public float AlertGaugePercent = .25f;

    public RectTransform BarMaskerRT;
    public RectTransform AfterImageMaskerRT;
    public Color AlertColor;

    public bool AfterDmgEffctOn = false;

    int maxAmount;

    float originalWidth;
    float alertWidth;
    
    Vector2 newBarSize;     // 유니티가 단일 스레드라 이거하나로 써도 문제가 없군...
    Image bar;
    Color originalColor;

    Coroutine lastCoroutine;
    
	// Use this for initialization
	void Start () {
        bar = transform.Find("BarMasker/Bar").GetComponent<Image>();
        originalColor = bar.color;
        originalWidth = BarMaskerRT.sizeDelta.x;
        newBarSize = BarMaskerRT.sizeDelta;
        alertWidth = originalWidth * AlertGaugePercent;

        if (!AfterDmgEffctOn)
            AfterImageMaskerRT.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {

    }

    public void Initialize(int GaugeMaxAmount)
    {
        maxAmount = GaugeMaxAmount;
        CurrentAmount = maxAmount;
    }

    public void Damage(int amount)
    {
        if(lastCoroutine != null)
            StopCoroutine(lastCoroutine);
        lastCoroutine = StartCoroutine(GaugeChangeTo(CurrentAmount - amount));
    }

    public void Restore(int amount)
    {
        if (lastCoroutine != null)
            StopCoroutine(lastCoroutine);
        lastCoroutine = StartCoroutine(GaugeChangeTo(CurrentAmount + amount));
    }

    IEnumerator GaugeChangeTo(int changedAmount)
    {
        float startBarWidth;
        float changedBarWidth;
        float time = 0f;
        float lerp = 0f;

        if (changedAmount < 0)
            changedAmount = 0;
        else if (changedAmount > maxAmount)
            changedAmount = maxAmount;

        // when decreasing(damaging) check if afterimage is shorter than HPbar
        if (AfterDmgEffctOn && changedAmount < CurrentAmount && BarMaskerRT.sizeDelta.x > AfterImageMaskerRT.sizeDelta.x)
        {
            StopCoroutine("AfterImage");
            AfterImageMaskerRT.sizeDelta = BarMaskerRT.sizeDelta;
        }

        CurrentAmount = changedAmount;
        startBarWidth = BarMaskerRT.sizeDelta.x;
        changedBarWidth = originalWidth * ((float)CurrentAmount / maxAmount);
        
        while (lerp < 1f)
        {
            // change masking object width
            newBarSize.x = Mathf.Lerp(startBarWidth, changedBarWidth, lerp);
            BarMaskerRT.sizeDelta = newBarSize;

            // Check if gauge is in alert (Lower than alertWidth) and change color of bar.
            if (newBarSize.x <= alertWidth)
                bar.color = AlertColor;
            else
                bar.color = originalColor;

            time += Time.deltaTime / lerpDuration;
            lerp = Mathf.Pow(time - 1, 3) + 1;

            yield return null;
        }

        newBarSize.x = changedBarWidth;
        BarMaskerRT.sizeDelta = newBarSize;

        if(AfterDmgEffctOn)
            yield return StartCoroutine("AfterImage", changedAmount);
    }

    IEnumerator AfterImage(int changedAmount)
    {
        float startBarWidth;
        float changedBarWidth;
        float time = 0f;
        float lerp = 0f;

        if (changedAmount < 0)
            changedAmount = 0;
        else if (changedAmount > maxAmount)
            changedAmount = maxAmount;

        CurrentAmount = changedAmount;
        startBarWidth = AfterImageMaskerRT.sizeDelta.x;
        changedBarWidth = originalWidth * ((float)CurrentAmount / maxAmount);

        while (lerp < 1f)
        {
            // change masking object width
            newBarSize.x = Mathf.Lerp(startBarWidth, changedBarWidth, lerp);
            AfterImageMaskerRT.sizeDelta = newBarSize;

            time += Time.deltaTime / (lerpDuration - .1f);
            lerp = Mathf.Pow(time - 1, 3) + 1;

            yield return null;
        }

        newBarSize.x = changedBarWidth;
        AfterImageMaskerRT.sizeDelta = newBarSize;

        yield return null;
    }
}
