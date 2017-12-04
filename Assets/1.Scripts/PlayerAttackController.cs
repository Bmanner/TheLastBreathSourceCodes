using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerAttackController : MonoBehaviour {
    [Tooltip("In second.")]
    public float chargeTime = 1f;

    [HideInInspector] public bool Stun { set { isStun = value; } }

    CameraShaker camShaker;
    PlayerMovement movementScript;
    Animator animator;

    [SerializeField] GameObject weaponParent; // WeaponPos in character right hand
    GameObject attackImpactArea;
    GameObject chargeGauge;
    Transform gauge;

    float nextFire = 0.0f;
    float chargeCounter = 0f;
    bool isCharging = false;
    bool isMax = false;
    bool isAttacking = false;
    bool isStun = false;

    void Awake () {
        camShaker = Camera.main.GetComponent<CameraShaker>();
        movementScript = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();

        attackImpactArea = transform.Find("AttackArea").gameObject;
        chargeGauge = GameObject.Find("ChargeGauge");
        gauge = chargeGauge.transform.Find("Gauge");

        chargeGauge.SetActive(false);
    }
	
    void OnEnable()
    {
        weaponParent.SetActive(true);
    }

    void OnDisable()
    {
        StopAllCoroutines();

        isCharging = false;
        isAttacking = false;
        isMax = false;
        chargeCounter = 0;
        animator.speed = 1f;
        movementScript.MoveSlower = false;
        camShaker.SetBivrate(0);
        if(chargeGauge != null) chargeGauge.SetActive(false);
        animator.SetFloat("ChargeMultiplier", 1f);
        animator.Play("Idle");

        weaponParent.SetActive(false);
    }

	// Update is called once per frame
	void Update () {

        if(isStun)
        {
            StopAllCoroutines();

            isCharging = false;
            isAttacking = false;
            isMax = false;
            chargeCounter = 0;
            animator.speed = 1f;
            movementScript.MoveSlower = false;
            camShaker.SetBivrate(0);
            chargeGauge.SetActive(false);
            animator.SetFloat("ChargeMultiplier", 1f);
            animator.Play("Idle");

            return;
        }

        // TODO : 무기를 들고 있는지 체크.
        if (Input.GetButtonDown("Fire1"))
        {  
            // 애니메이션이 플레이 중이면 공격하지 않도록 isAttacking 플래그를 세워준다. (차징이 애니 끝나기 전에 들어가지 않도록 하는 코드)
            if (animator.GetCurrentAnimatorStateInfo(1).IsName("Attack"))
            {
                isAttacking = true;
                return;
            }
            else
                isAttacking = false;

            chargeCounter = 0;

            animator.Play("Attack");
            StartCoroutine(CheckExactAttackMoment());
        }

        if (!isAttacking && Input.GetButton("Fire1"))
        {
            chargeCounter += Time.deltaTime;
            if(!isCharging && chargeCounter > 0.23f && chargeCounter < chargeTime)
            {
                isCharging = true;
                StartCoroutine(AttackCharging());
            }
            else if(chargeCounter > chargeTime)
            {
                isCharging = false;
                isMax = true;
            }
        }

        if(Input.GetButtonUp("Fire1"))
        {
            isCharging = false;
            isMax = false;

            if (chargeCounter >= chargeTime)
            {
                Debug.Log("This was a charged attack");
            }
        }
    }

    IEnumerator AttackCharging()
    {
        chargeGauge.SetActive(true);
        animator.speed = 0.1f;

        movementScript.MoveSlower = true;

        //TODO : Charge UI 표시
        //TODO : 고정 프레임에 맞추어 수정.

        while (isCharging)
        {
            gauge.localScale = new Vector3(chargeCounter / chargeTime, 1, 1);

            camShaker.SetBivrate(chargeCounter / chargeTime);
            yield return null;
        }

        if (isMax)
            animator.SetFloat("ChargeMultiplier", 0f);

        while (isMax)
        {
            camShaker.SetBivrate(1f);
            yield return null;
        }

        camShaker.SetBivrate(0);
        if(chargeCounter >= chargeTime)
            camShaker.Shake();

        animator.SetFloat("ChargeMultiplier", 1f);
        animator.speed = 1f;

        movementScript.MoveSlower = false;

        chargeGauge.SetActive(false);
        chargeCounter = 0;
    }

    IEnumerator CheckExactAttackMoment()
    {
        yield return null;

        while (animator.GetCurrentAnimatorStateInfo(1).IsName("Attack")) {
            if (animator.GetCurrentAnimatorStateInfo(1).normalizedTime >= 16f / 46f)
            {
                attackImpactArea.SetActive(true);
                break;
            }
            
            yield return null;
        }

        yield return new WaitForSeconds(0.15f);

        attackImpactArea.SetActive(false);
    }
}
