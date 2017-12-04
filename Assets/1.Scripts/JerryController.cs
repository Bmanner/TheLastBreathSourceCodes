using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JerryController : MonoBehaviour {

    // vigilance = 경계
    public enum JerryStateEnum { idle, scared, backAway, trace, traceFast, die };
    [SerializeField] public JerryStateEnum jerryState = JerryStateEnum.idle;

    public float TraceFastDist = 4.0f;
    public float traceDist = 2.0f;
    public float BackAwayDist = 3.0f;
    //public float VigilanceDist = 10.0f;

    public int damagePower = 10;
    public int AggrssvContinuanceSec = 20;

    dyingMotion dieMotion = dyingMotion.FallingBack;

    GameObject attackImpactArea;
    Transform playerTr;
    UnityEngine.AI.NavMeshAgent nvAgent;
    Animator animator;

    Coroutine aggrssvCounterCoroutine;

    float originalSpeed;
    bool isDie = false;
    bool sawFight = false;
    bool backAwayDone = false;

    /// <summary> 플레이어의 달리기 시 속도 증가값 </summary>
    readonly float RUNNING_SPEED_MULTIPLYER = 1.6f;

    enum dyingMotion
    {
        FallingBack = 1,
        FallingForward,
        ZombieDying
    }

    void Awake()
    {   // Awake로 변경하는 이유 - 책 390쪽 참조. 
        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();
        nvAgent = this.gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
        animator = this.gameObject.GetComponent<Animator>();

        originalSpeed = nvAgent.speed;
    }
    // 이벤트 발생 시 수행할 함수 연결
    void OnEnable()
    {
        StartCoroutine(this.CheckMonsterState());
        StartCoroutine(this.MonsterAction());
    }
    // 해제
    void onDisable()
    {
        //PlayerCtrl.OnPlayerDie -= this.OnPlayerDie;
    }

    IEnumerator CheckMonsterState()
    {
        while (!isDie)
        {
            yield return new WaitForSeconds(0.2f);

            float dist = Vector3.Distance(playerTr.position, transform.position);

            if (!PlayerMovement.IsCombat)
            {
                if (dist > TraceFastDist)
                    jerryState = JerryStateEnum.traceFast;
                else if (dist >= traceDist)
                    jerryState = JerryStateEnum.trace;
                else
                    jerryState = JerryStateEnum.idle;

                sawFight = false;
                backAwayDone = false;
            }
            else
            {
                if (!sawFight && dist > TraceFastDist)
                    jerryState = JerryStateEnum.traceFast;
                else if (!sawFight && dist >= traceDist)
                    jerryState = JerryStateEnum.trace;
                else if (!backAwayDone)
                {
                    jerryState = JerryStateEnum.backAway;
                    StartCoroutine(BackAwaySeq());
                    sawFight = true;
                }
                else
                {
                    jerryState = JerryStateEnum.scared;
                }
            }
        }
    }

    IEnumerator MonsterAction()
    {
        //TODO : 스테이트 변동 시에만 아래 루프 돌도록 수정.
        //TODO : 코드 정리 필요.
        while (!isDie)
        {
            nvAgent.updateRotation = true;
            switch (jerryState)
            {
                case JerryStateEnum.idle:
                    nvAgent.isStopped = true;
                    animator.SetBool("IsTrace", false);
                    animator.SetBool("IsScared", false);
                    animator.SetBool("IsBackAway", false);
                    break;
                case JerryStateEnum.scared:
                    nvAgent.isStopped = true;
                    nvAgent.updateRotation = true;
                    animator.SetBool("IsScared", true);
                    animator.SetBool("IsTrace", true);
                    animator.SetBool("IsBackAway", false);
                    break;
                case JerryStateEnum.backAway:
                    nvAgent.destination = transform.position - (playerTr.position - transform.position);
                    nvAgent.isStopped = false;
                    nvAgent.updateRotation = false;
                    animator.SetBool("IsBackAway", true);
                    break;
                case JerryStateEnum.trace:
                    nvAgent.destination = playerTr.position;
                    nvAgent.isStopped = false;
                    nvAgent.speed = originalSpeed;
                    animator.SetBool("IsTrace", true);
                    animator.SetBool("IsScared", false);
                    animator.SetBool("IsBackAway", false);
                    break;
                case JerryStateEnum.traceFast:
                    nvAgent.destination = playerTr.position;
                    nvAgent.isStopped = false;
                    nvAgent.speed = originalSpeed * RUNNING_SPEED_MULTIPLYER;
                    animator.SetBool("IsTrace", true);
                    animator.SetBool("IsScared", false);
                    animator.SetBool("IsBackAway", false);
                    break;
            }
            yield return null;
        }
    }

    IEnumerator BackAwaySeq()
    {
        float time = 1f;
        while(time > 0f)
        {
            time -= Time.deltaTime;
            yield return null;
        }
        backAwayDone = true;
    }

    void OnPlayerDie()
    {
        StopAllCoroutines();
        //nvAgent.Stop();   // Obsolete. So changed to code below.
        nvAgent.isStopped = true;
        animator.SetTrigger("IsPlayerDie");
    }

    void MonsterDie()
    {
        gameObject.tag = "Untagged";

        StopAllCoroutines();

        isDie = true;
        jerryState = JerryStateEnum.die;
        //nvAgent.Stop();   // Obsolete. So changed to code below.nvAgent.Stop();
        nvAgent.isStopped = true;
        animator.SetInteger("DyingMotion", (int)dieMotion);
        animator.SetTrigger("IsDie");

        gameObject.GetComponent<CapsuleCollider>().enabled = false;
    }

    public int getDamagePower()
    {
        return damagePower;
    }
}
