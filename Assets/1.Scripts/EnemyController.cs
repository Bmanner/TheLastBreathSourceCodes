using UnityEngine;
using System.Collections;

public class EnemyController : CharacterBase
{
    // vigilance = 경계
    public enum MonsterState { idle, vigilance, trace, attack, die };
    public MonsterState monsterState = MonsterState.idle;

    public GameObject bloodEffect;
    public GameObject bloodDecal;

    public float traceDist = 10.0f;
    public float attackDist = 2.0f;
    //public float VigilanceDist = 10.0f;

    public int damagePower = 10;
    public int AggrssvContinuanceSec = 20;
    public bool isAggressive = false;

    dyingMotion dieMotion = dyingMotion.FallingBack;

    GameObject attackImpactArea;
    Transform monsterTr;
    Transform playerTr;
    UnityEngine.AI.NavMeshAgent nvAgent;
    Animator animator;

    Coroutine aggrssvCounterCoroutine;

    bool isDie = false;

    enum dyingMotion
    {
        FallingBack = 1,
        FallingForward,
        ZombieDying
    }

    void Awake()
    {   // Awake로 변경하는 이유 - 책 390쪽 참조. 
        attackImpactArea = transform.Find("AttackArea").gameObject;
        monsterTr = this.gameObject.GetComponent<Transform>();
        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();
        nvAgent = this.gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
        animator = this.gameObject.GetComponent<Animator>();
    }
    // 이벤트 발생 시 수행할 함수 연결
    void OnEnable()
    {
        //PlayerCtrl.OnPlayerDie += this.OnPlayerDie;
        hp = Health;
        dieMotion = (dyingMotion)Random.Range(1, 4);

        StartCoroutine(this.CheckMonsterState());
        StartCoroutine(this.MonsterAction());
    }
    // 해제
    void onDisable()
    {
        //PlayerCtrl.OnPlayerDie -= this.OnPlayerDie;
    }

    public override void Damage(int dmgAmount)
    {
        isAggressive = true;

        if (aggrssvCounterCoroutine != null)
            StopCoroutine(aggrssvCounterCoroutine);
        aggrssvCounterCoroutine = StartCoroutine(AggressiveCount());

        hp -= dmgAmount;
        Debug.Log("적 남은 체력 : " + hp);
        if (hp <= 0)
        {
            MonsterDie();
        }
        else
        {
            if (Random.Range(3, 4) == 3)
            {
                animator.SetTrigger("IsHit");
            }
        }
    }

    IEnumerator CheckMonsterState()
    {
        while (!isDie)
        {
            yield return new WaitForSeconds(0.2f);

            float dist = Vector3.Distance(playerTr.position, monsterTr.position);

            if (dist <= attackDist)
            {
                if (isAggressive)
                {
                    monsterState = MonsterState.attack;
                }
                else
                {
                    monsterState = MonsterState.vigilance;
                    attackImpactArea.SetActive(false);
                }
            }
            else if (dist <= traceDist)
            {
                attackImpactArea.SetActive(false);

                if (isAggressive)
                    monsterState = MonsterState.trace;
                else
                    monsterState = MonsterState.vigilance;
            }
            else
            {
                // TODO : Roaming arround
                monsterState = MonsterState.idle;
                attackImpactArea.SetActive(false);
            }
        }
    }

    IEnumerator MonsterAction()
    {
        while (!isDie)
        {
            switch (monsterState)
            {
                case MonsterState.idle:
                    nvAgent.isStopped = true;
                    animator.SetBool("IsTrace", false);
                    animator.SetBool("IsVigilance", false);
                    break;
                case MonsterState.vigilance:
                    transform.rotation = Quaternion.LookRotation(playerTr.position - transform.position);
                    nvAgent.isStopped = true;
                    animator.SetBool("IsAttack", false);
                    animator.SetBool("IsTrace", false);
                    animator.SetBool("IsVigilance", true);
                    break;
                case MonsterState.trace:
                    nvAgent.destination = playerTr.position;
                    if (animator.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
                        nvAgent.isStopped = true;
                    else
                        nvAgent.isStopped = false;
                    animator.SetBool("IsAttack", false);
                    animator.SetBool("IsTrace", true);
                    animator.SetBool("IsVigilance", false);
                    break;
                case MonsterState.attack:
                    transform.rotation = Quaternion.LookRotation(playerTr.position - transform.position);
                    nvAgent.isStopped = true;
                    animator.SetBool("IsAttack", true); // TODO : Known issue - normalizedTime은 계속 증가함. 이 아이는 SMB에서 직접 컨트롤해야할듯.
                    if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") &&
                        animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 15f / 46f &&
                        animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 17f / 46f)
                    {
                        attackImpactArea.SetActive(true);
                    }
                    else
                    {
                        attackImpactArea.SetActive(false);
                    }
                    break;
            }
            yield return null;
        }
    }

    IEnumerator AggressiveCount()
    {
        int aggressiveCnt = AggrssvContinuanceSec;

        while(aggressiveCnt > 0)
        {
            --aggressiveCnt;
            yield return new WaitForSeconds(1f);
            Debug.Log("뭐지");
        }

        isAggressive = false;

        yield return null;
    }

    void CreateBloodEffect(Vector3 pos)
    {
        GameObject blood1 = (GameObject)Instantiate(bloodEffect, pos, Quaternion.identity);

        Vector3 decalPos = monsterTr.position + (Vector3.up * 0.05f);
        Quaternion decalRot = Quaternion.Euler(90, 0, Random.Range(0, 360));

        GameObject blood2 = (GameObject)Instantiate(bloodDecal, decalPos, decalRot);

        float scale = Random.Range(1.5f, 3.5f);
        blood2.transform.localScale = Vector3.one * scale;

        Destroy(blood1, 2.0f); // 2초 뒤 삭제
        Destroy(blood2, 30.0f);
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
        monsterState = MonsterState.die;
        //nvAgent.Stop();   // Obsolete. So changed to code below.nvAgent.Stop();
        nvAgent.isStopped = true;
        animator.SetInteger("DyingMotion", (int)dieMotion);
        animator.SetTrigger("IsDie");

        gameObject.GetComponent<CapsuleCollider>().enabled = false;
        /*
        foreach (Collider coll in gameObject.GetComponentsInChildren<SphereCollider>())
        {
            coll.enabled = false;
        }
        */
        StartCoroutine(this.PushObjectPool());
    }

    IEnumerator PushObjectPool()
    {
        yield return new WaitForSeconds(8.0f);

        isDie = false;
        hp = Health;
        gameObject.tag = "Enemy";
        monsterState = MonsterState.idle;

        gameObject.GetComponent<CapsuleCollider>().enabled = true;
        /*
        foreach (Collider coll in gameObject.GetComponentsInChildren<SphereCollider>())
        {
            coll.enabled = true;
        }
        */
        gameObject.SetActive(false);
    }

    public int getDamagePower()
    {
        return damagePower;
    }
}
