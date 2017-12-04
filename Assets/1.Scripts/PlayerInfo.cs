using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : CharacterBase, ITimeUpdateListener {

    //public Inventory Inven;

    //TEST CODE : 지우기
    public bool TestGUIOnOff = true;

    public int Hunger = 200;
    public int LungHealth = 200;

    public PlayerGaugeManager HPGaugeManager;
    public PlayerGaugeManager HungerGaugeManager;
    public PlayerGaugeManager LungGaugeManager;

    PlayerMovement playerMoveCon;
    GameObject screenBloodEffect;
    Image bloodEffectImg;

    void Awake(){
        //TODO : 퍼블릭 말고 코드에서 직접 찾도록 변경
        HPGaugeManager.Initialize(Health);
        HungerGaugeManager.Initialize(Hunger);
        LungGaugeManager.Initialize(LungHealth);

        playerMoveCon = GetComponent<PlayerMovement>();
        screenBloodEffect = UIController.Instance.transform.Find("ScreenEffect/BloodEffect").gameObject;
        bloodEffectImg = screenBloodEffect.GetComponent<Image>();
    }

	void Update () {
		
	}

    public override void Damage(int dmgAmount)
    {
        // TODO : 닷지하는 내내 무적. 추후 수정
        if (playerMoveCon.IsDodging)
            return;

        base.Damage(dmgAmount);
        HPGaugeManager.Damage(dmgAmount);

        // Screen Blood Effect Fade Sequence
        // im.GetComponent<CanvasRenderer>().SetAlpha(0.5f); 이걸로도 알파값 수정 가능.
        // bloodEffectImg.color = new Color(1, 1, 1, 0); 컬러로 할 경우 인자는 0~1.
        screenBloodEffect.SetActive(true);
        bloodEffectImg.CrossFadeAlpha(1 - (float)hp / Health + .2f, 0, true);
        bloodEffectImg.CrossFadeAlpha(0, 4, true);

        // TODO : 인스펙터에서 확률 수정할 수 있도록 나중에 변경. EnemyController도 마찬가지.
        if (Random.Range(0,4) == 3)
        {
            playerMoveCon.GetHit();
        }
    }

    void ITimeUpdateListener.TimeUpdate()
    {
        HungerGaugeManager.Damage(8);
    }

    IEnumerator ScreenBloodEffect()
    {
        yield return null;
    }
    
    // TODO : Test Code
    void OnGUI()
    {
        if (TestGUIOnOff)
        {
            // Make a background box
            GUI.Box(new Rect(400, 5, 100, 100), "Test Buttons");

            // Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
            if (GUI.Button(new Rect(410, 30, 80, 20), "Damage 10"))
            {
                Damage(10);
            }
            if (GUI.Button(new Rect(410, 55, 80, 20), "Damage 100"))
            {
                Damage(100);
            }
            if (GUI.Button(new Rect(410, 80, 80, 20), "Restore 100"))
            {
                hp += 100;
                HPGaugeManager.Restore(100);
            }
        }
    }
    

}
