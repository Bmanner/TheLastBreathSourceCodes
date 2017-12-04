using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO : 루트모션 사용 검토. 왜 안썼지?

public class PlayerMovement : MonoBehaviour
{
    public static bool IsCombat;

    public GameObject Neck;
    public float WalkingSpeed = 2f;            // The speed that the player will move at.
    public float RunningSpeed = 4f;
    [Tooltip("앞으로 가다가 고개가 몇도 이상 꺾어지면 몸체를 돌릴지에 대한 각도")]
    public float BodyRotateAngle = 82f;
    [Tooltip("몸체 돌아갈 때 얼마나 부드럽게 할지에 대한 값. 값이 작을수록 부드럽습니다.")]
    public float BodyRotationSmoothing = 7f;
    [HideInInspector] public bool MoveSlower = false;
    [HideInInspector] public bool IsDodging { get { return isDodging; } set { isDodging = value; } }
    [HideInInspector] public bool DodgeIsCool { set { dodgeIsCooldown = value; } }

    Quaternion bodyRotDir;
    Quaternion neckRotDir;

    Vector3 movement;                   // The vector to store the direction of the player's movement.
    Animator anim;                      // Reference to the animator component.
    Rigidbody playerRigidbody;          // Reference to the player's rigidbody.
    Collider capsuleCollider;
    PlayerAttackController attackCon;

    float moveSlower = 0.3f;

    float rawH;
    float rawV;
    float h;
    float v;
    bool isRunning;
    bool isRayHit;
    bool dodgeIsCooldown;
    bool isDodging;

    bool dodgeSetDirFlag = false;

    public void GetHit()
    {
        anim.SetTrigger("GetHit");
    }

    void Awake()
    {
        // Set up references.
        anim = GetComponent<Animator>();
        playerRigidbody = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        attackCon = GetComponent<PlayerAttackController>();
    }

    void OnEnable()
    {
        isRunning = false;
        IsCombat = anim.GetBool("IsCombat");
        rawH = 0f;
        rawV = 0f;
        h = 0f;
        v = 0f;
    }

    void FixedUpdate()
    {
        // Store the input axes.
        rawH = Input.GetAxisRaw("Horizontal");
        rawV = Input.GetAxisRaw("Vertical");
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        if (!isDodging)
        {
            CalcPlayerRotation();
            // Move the player around the scene.
            Move();
        }
        // Comment : 유니티 함수 사이클을 봐도 잘 이해가 안되는데, 어떤 연유에서인지 모르겠으나 Fixed Update가 아닌 다른 함수에서
        // 아래의 코드를 실행하면 제대로 적용이 안됨. 간헐적으로 되거나(LateUpdate) 아예 안됨(Update) ...아 아닌가? 암튼 안됨
        if (dodgeSetDirFlag)
        {
            movement.Set(rawH, 0f, rawV);
            playerRigidbody.MoveRotation(Quaternion.LookRotation(movement));
            dodgeSetDirFlag = false;
        }
    }

    void Update()
    { 
        if (Input.GetKeyDown(Shortcuts.Sprint))
        {
            isRunning = true;
        }
        if(Input.GetKeyUp(Shortcuts.Sprint))
        {
            isRunning = false;
        }
        if (Input.GetKeyDown(Shortcuts.CombatMode))
        {
            IsCombat = !IsCombat;
            anim.SetBool("IsCombat", IsCombat);
            attackCon.enabled = IsCombat;
        }
        if(IsCombat && !dodgeIsCooldown && Input.GetKeyDown(Shortcuts.Dodge))
        {
            isDodging = true;
            dodgeIsCooldown = true;
            attackCon.enabled = false;
            anim.applyRootMotion = true;
            anim.SetTrigger("Dodge");

            dodgeSetDirFlag = true;

            //StartCoroutine(ImmortalForSeconds(0.4f));
        }
    }
    
    void LateUpdate()
    {
        if (!isDodging)
        {
            Animating();
            Rotation();
        }
    }

    void Move()
    {
        // Set the movement vector based on the axis input.
        movement.Set(rawH, 0f, rawV);

        // Normalise the movement vector and make it proportional to the speed per second.
        movement = movement.normalized * Time.deltaTime * (isRunning ? RunningSpeed : WalkingSpeed);

        if (IsCombat)
        {
            movement *= 0.78f;

            if (MoveSlower)
                movement *= moveSlower;
        }

        // Move the player to it's current position plus the movement.
        playerRigidbody.MovePosition(transform.position + movement);
    }

    void CalcPlayerRotation()
    {
        // Perform the raycast and if it hits something on the floor layer...
        if (MouseRaycaster.Instance.IsHit)
        {
            isRayHit = true;
            // Create a vector from the player to the point on the floor the raycast from the mouse hit.
            Vector3 playerToMouse = MouseRaycaster.Instance.RayHit.point - Neck.transform.position;

            // Neck rotation direction.
            neckRotDir = Quaternion.LookRotation(playerToMouse);

            // Ensure the vector is entirely along the floor plane.
            playerToMouse.y = 0f;

            // Create a quaternion (rotation) based on looking down the vector from the player to the mouse.
            if (v == 0 && h == 0)
                bodyRotDir = Quaternion.LookRotation(playerToMouse);
            else
            {
                var dot = Vector3.Dot(movement.normalized, playerToMouse.normalized);
                var angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

                if (angle > BodyRotateAngle)
                    bodyRotDir = Quaternion.LookRotation(playerToMouse);
                else
                    bodyRotDir = Quaternion.LookRotation(movement);

            }
        }
        else
            isRayHit = false;
    }

    void Rotation()
    {
        if (!isRayHit)
            return;

        var targetDir = Quaternion.Lerp(transform.rotation, isRunning && !(v == 0 && h == 0)? Quaternion.LookRotation(movement) : bodyRotDir, Time.deltaTime * BodyRotationSmoothing);

        if (v == 0 && h == 0)
            playerRigidbody.MoveRotation(bodyRotDir);
        else
            playerRigidbody.MoveRotation(targetDir);

        if(!isRunning)
            Neck.transform.rotation = neckRotDir;
    }

    void Animating()
    {
        // Create a boolean that is true if either of the input axes is non-zero.
        //bool walking = h != 0f || v != 0f;

        // Tell the animator whether or not the player is walking.
        //anim.SetBool("IsWalking", walking);

        //anim.SetFloat("Horizontal", h);
        //anim.SetFloat("Vertical", v);
        if(isRunning)
        {
            //anim.SetFloat("Vertical", 2);
            anim.SetFloat("Vertical", rawH == 0 && rawV == 0 ? 0 : 2); // 2 is animator Blend Tree posY max value
            anim.SetFloat("Horizontal", 0);
        }
        else
        {
            var blendDir = transform.rotation * Vector3.ClampMagnitude(new Vector3(v, 0, h), 1f);

            anim.SetFloat("Horizontal", blendDir.z);
            anim.SetFloat("Vertical", blendDir.x);
        }
        
    }

    IEnumerator ImmortalForSeconds(float seconds)
    {
        capsuleCollider.enabled = false;

        yield return new WaitForSeconds(seconds);

        capsuleCollider.enabled = true;
    }
}




/* LEGACY */
/*
[DisallowMultipleComponent]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerMovement : MonoBehaviour
{
    Animator anim;

    bool isWalking = false;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Turning();
        Walking();
        Move();
    }

    void Turning()
    {
        //anim.SetFloat("Turn", Input.GetAxis("Horizontal"));
    }

    void Walking()
    {
        if (Input.GetKeyDown(Shortcuts.Sprint))
        {
            isWalking = !isWalking;
            anim.SetBool("Walk", isWalking);
        }
    }

    void Move()
    {
        anim.SetFloat("MoveZ", Input.GetAxis("MoveZ"));
        anim.SetFloat("MoveX", Input.GetAxis("MoveX"));
    }
}
*/
