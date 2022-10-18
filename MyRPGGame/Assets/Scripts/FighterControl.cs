using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterControl : MonoBehaviour
{
    // 이동 관련
    private Transform myTransform = null;
    private CharacterController myCharacterController = null;
    private Transform cameraTransform = null;

    public float moveSpeed = 2.0f;
    public float runSpeed = 3.5f;
    public float rotateSpeed = 100.0f;
    public float velocitySpeed = 0.1f;

    public Vector3 moveDirection = Vector3.zero;

    private CollisionFlags collisionFlags = CollisionFlags.None;

    private Vector3 velocity = Vector3.zero;

    public float bodyRotationNormaltime = 0.5f;

    // animation
    private Animation myAnimation = null;

    public AnimationClip idleClip = null;
    public AnimationClip walkClip = null;
    public AnimationClip runClip = null;
    public AnimationClip attack01Clip = null;
    public AnimationClip attack02Clip = null;
    public AnimationClip attack03Clip = null;

    public enum ActionState
    {
        None = -1,
        Idle = 0,
        Walk = 1,
        Run = 2,
        Attack,
        Skill,
        Die
    }

    public ActionState myState = ActionState.None;

    public enum AttackState
    {
        None = -1,
        Attack1,
        Attack2,
        Attack3
    }

    public AttackState attackState = AttackState.None;
    public bool isOnNextAttack = false;

    public const int LeftMouseButton = 0;
    public const int RightMouseButton = 1;

    private float verticalSpeed = 0.0f;
    private float gravity = 9.8f;

    void ApplyGravity()
    {
        if((collisionFlags & CollisionFlags.Below) != 0)
        {
            verticalSpeed = 0.0f;
        }
        else
        {
            verticalSpeed -= gravity * Time.deltaTime;
        }
    }

    void InputControl()
    {
        if(Input.GetMouseButtonDown(LeftMouseButton) == true)
        {
            if(myState != ActionState.Attack)
            {
                myState = ActionState.Attack;
                attackState = AttackState.Attack1;
            }
            else
            {
                switch (attackState)
                {
                    case AttackState.Attack1:
                        if (myAnimation[attack01Clip.name].normalizedTime > 0.1f)
                        {
                            isOnNextAttack = true;
                        }
                        break;
                    case AttackState.Attack2:
                        if(myAnimation[attack02Clip.name].normalizedTime > 0.1f)
                        {
                            isOnNextAttack = true;
                        }
                        break;
                    case AttackState.Attack3:
                        if(myAnimation[attack03Clip.name].normalizedTime > 0.1f)
                        {
                            isOnNextAttack = true;
                        }
                        break;
                }
            }
        }
    }

    void ControlAttackAnimation()
    {
        switch (attackState)
        {
            case AttackState.Attack1:
                PlayAnimation(attack01Clip, false);
                break;
            case AttackState.Attack2:
                PlayAnimation(attack02Clip, false);
                break;
            case AttackState.Attack3:
                PlayAnimation(attack03Clip, false);
                break;
        }
    }

    void OnAttackAnimationEvent()
    {
        if(isOnNextAttack == false)
        {
            myState = ActionState.Idle;
            attackState = AttackState.Attack1;
        }
        else
        {
            isOnNextAttack = false;
            switch (attackState)
            {
                case AttackState.Attack1:
                    attackState = AttackState.Attack2;
                    break;
                case AttackState.Attack2:
                    attackState = AttackState.Attack3;
                    break;
                case AttackState.Attack3:
                    attackState = AttackState.Attack1;
                    break;
            }
        }
    }


    void ControlAnimation()
    {
        switch (myState)
        {
            case ActionState.Idle:
                PlayAnimation(idleClip);
                break;
            case ActionState.Walk:
                PlayAnimation(walkClip);
                break;
            case ActionState.Run:
                PlayAnimation(runClip);
                break;
            case ActionState.Attack:
                ControlAttackAnimation();
                break;
        }
    }

    void PlayAnimation(AnimationClip clip, bool isBlending = true)
    {
        myAnimation.clip = clip;
        if (isBlending)
        {
            myAnimation.CrossFade(clip.name);
        }
        else
        {
            myAnimation.Play(clip.name);
        }
    }

    void CheckState()
    {
        float moveSpeed = GetVelocitySpeed();
        switch (myState)
        {
            case ActionState.Idle:
                if(moveSpeed > 0.1f)
                {
                    myState = ActionState.Walk;
                }
                break;
            case ActionState.Walk:
                if(moveSpeed > 1.8f)
                {
                    myState = ActionState.Run;
                }

                if(moveSpeed < 0.1f)
                {
                    myState = ActionState.Idle;
                }
                break;
            case ActionState.Run:
                if(moveSpeed < 1.8f)
                {
                    myState = ActionState.Walk;
                }
                
                if(moveSpeed < 0.1f)
                {
                    myState = ActionState.Idle;
                }
                break;
            case ActionState.Attack:
                break;
        }
    }

    float GetVelocitySpeed()
    {
        if(myCharacterController.velocity == Vector3.zero)
        {
            velocity = Vector3.zero;
            velocitySpeed = 0.0f;
        }
        else
        {
            Vector3 currentVelocity = myCharacterController.velocity;
            currentVelocity.y = 0.0f;
            velocitySpeed += Time.fixedDeltaTime * 2.0f;
            velocity = Vector3.Lerp(velocity, currentVelocity, velocitySpeed);
        }

        return velocity.magnitude;
    }

    void BodyDirectionChange()
    {
        if(myCharacterController.velocity.magnitude > 0.1f)
        {
            bodyRotationNormaltime += Time.fixedDeltaTime * 0.5f;
        }
        else
        {
            bodyRotationNormaltime = 0.5f;
        }

        Vector3 newForward = myCharacterController.velocity;
        newForward.y = 0.0f;
        newForward = newForward.normalized;

        myTransform.forward = Vector3.Lerp(myTransform.forward, newForward, bodyRotationNormaltime);
    }

    void Move()
    {
        if(myState == ActionState.Attack)
        {
            return;
        }

        Vector3 forward = cameraTransform.TransformDirection(Vector3.forward);
        forward.y = 0.0f;
        forward = forward.normalized;
        Vector3 right = new Vector3(forward.z, 0.0f, -forward.x);

        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        Vector3 targetDirection = horizontal * right + vertical * forward;

        moveDirection = Vector3.RotateTowards(moveDirection, targetDirection, rotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000.0f);
        moveDirection = moveDirection.normalized;

        float speed = moveSpeed;
        if(myState == ActionState.Run)
        {
            speed = runSpeed;
        }

        Vector3 gravityVertical = new Vector3(0.0f, verticalSpeed, 0.0f);
        Vector3 moveAmount = (moveDirection * speed * Time.deltaTime) + gravityVertical;

        collisionFlags = myCharacterController.Move(moveAmount);
    }

    
    void AddAnimationEvent(string funcName, AnimationClip clip)
    {
        AnimationEvent newEvent = new AnimationEvent();
        newEvent.functionName = funcName;
        newEvent.time = clip.length - 0.1f;
        clip.AddEvent(newEvent);
    }

    // Start is called before the first frame update
    void Start()
    {
        myTransform = GetComponent<Transform>();
        myCharacterController = GetComponent<CharacterController>();
        myAnimation = GetComponent<Animation>();
        cameraTransform = Camera.main.transform;

        myAnimation.playAutomatically = false;
        myAnimation.Stop();

        myAnimation[idleClip.name].wrapMode = WrapMode.Loop;
        myAnimation[walkClip.name].wrapMode = WrapMode.Loop;
        myAnimation[runClip.name].wrapMode = WrapMode.Loop;
        myAnimation[attack01Clip.name].wrapMode = WrapMode.Once;
        myAnimation[attack02Clip.name].wrapMode = WrapMode.Once;
        myAnimation[attack03Clip.name].wrapMode = WrapMode.Once;

        AddAnimationEvent("OnAttackAnimationEvent", attack01Clip);
        AddAnimationEvent("OnAttackAnimationEvent", attack02Clip);
        AddAnimationEvent("OnAttackAnimationEvent", attack03Clip);

        myState = ActionState.Idle;

    }

    // Update is called once per frame
    void Update()
    {
        ApplyGravity();

        Move();

        BodyDirectionChange();

        CheckState();

        ControlAnimation();

        InputControl();

    }
}
