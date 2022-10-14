using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterControl : MonoBehaviour
{
    // 이동 관련
    private Transform myTransform = null;
    private CharacterController myCharacterController = null;

    public float moveSpeed = 2.0f;
    public float runSpeed = 3.5f;
    public float rotateSpeed = 100.0f;
    public float velocitySpeed = 0.1f;

    public Vector3 moveDirection = Vector3.zero;

    private CollisionFlags collisionFlags = CollisionFlags.None;

    private Vector3 velocity = Vector3.zero;

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
        Die,
    }

    public ActionState myState = ActionState.None;

    void OnAttackAnimationEvent()
    {

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
        
    }
}
