using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum NPC_EnemyState { IDLE_STATIC, IDLE_ROAMER, IDLE_PATROL, INSPECT, ATTACK, FIND_WEAPON, KNOCKED_OUT, FOLLOW_PLAYER, DEAD, NONE }
public enum NPC_WeaponType { KNIFE, RIFLE, SHOTGUN }

public class NPC_Base : MonoBehaviour
{
    public UnityEngine.AI.NavMeshAgent navMeshAgent;
    public Animator npcAnimator;
    protected int hashSpeed;
    public NPC_EnemyState idleState = NPC_EnemyState.IDLE_ROAMER;
    NPC_EnemyState currentState = NPC_EnemyState.NONE;

    protected delegate void InitState();
    protected delegate void UpdateState();
    protected delegate void EndState();
    protected InitState _initState;
    protected InitState _updateState;
    protected InitState _endState;

    public NPC_WeaponType weaponType = NPC_WeaponType.KNIFE;
    protected Vector3 targetPos, startingPos;
    public LayerMask hitTestLayer;
    protected float weaponRange;
    public Transform weaponPivot;
    protected float weaponContact, weaponReload;

    public float inspectTimeout; //Once the npc reaches the destination, how much time unitl in goes back.

    public GameObject proyectilePrefab;
    public NPC_PatrolNode patrolNode;

    protected virtual void Start()
    {
        startingPos = transform.position;
        hashSpeed = Animator.StringToHash("Speed");
        SetWeapon(weaponType);
        SetState(idleState);
    }
    protected void SetWeapon(NPC_WeaponType newWeapon)
    {
        weaponType = newWeapon;
        npcAnimator.SetTrigger("WeaponChange");
        npcAnimator.SetInteger("WeaponType", (int)newWeapon);
        switch (newWeapon)
        {
            case NPC_WeaponType.KNIFE:
                weaponRange = 1.0f;
                weaponContact = 0.2f;
                weaponReload = 0.4f;
                break;
            case NPC_WeaponType.RIFLE:
                weaponRange = 20.0f;
                weaponContact = .05f;
                weaponReload = .5f;
                break;
            case NPC_WeaponType.SHOTGUN:
                weaponRange = 20.0f;
                weaponContact = 0.35f;
                weaponReload = 0.75f;
                break;
        }
    }
    public void SetState(NPC_EnemyState newState)
    {
        if (currentState != newState)
        {
            if (_endState != null)
                _endState();
            switch (newState)
            {
                case NPC_EnemyState.IDLE_STATIC: _initState = StateInit_IdleStatic; _updateState = StateUpdate_IdleStatic; _endState = StateEnd_IdleStatic; break;
                case NPC_EnemyState.IDLE_ROAMER: _initState = StateInit_IdleRoamer; _updateState = StateUpdate_IdleRoamer; _endState = StateEnd_IdleRoamer; break;
                case NPC_EnemyState.IDLE_PATROL: _initState = StateInit_IdlePatrol; _updateState = StateUpdate_IdlePatrol; _endState = StateEnd_IdlePatrol; break;
                case NPC_EnemyState.INSPECT: _initState = StateInit_Inspect; _updateState = StateUpdate_Inspect; _endState = StateEnd_Inspect; break;
                case NPC_EnemyState.ATTACK: _initState = StateInit_Attack; _updateState = StateUpdate_Attack; _endState = StateEnd_Attack; break;
                case NPC_EnemyState.FOLLOW_PLAYER: _initState = StateInit_Help; _updateState = StateUpdate_Help; _endState = StateEnd_Help; break;
            }
            _initState();
            currentState = newState;
        }
    }

    void UpdateSensors()
    {

    }
    protected void Update()
    {
        if (_updateState != null)
            _updateState();

        npcAnimator.SetFloat(hashSpeed, navMeshAgent.velocity.magnitude);
        npcAnimator.SetBool("Walking", navMeshAgent.velocity.magnitude > 0);
    }

    ///////////////////////////////////////////////////////// STATE: IDLE STATIC


    void StateInit_IdleStatic()
    {
        navMeshAgent.SetDestination(startingPos);
        navMeshAgent.isStopped = false;
    }
    void StateUpdate_IdleStatic()
    {


    }
    void StateEnd_IdleStatic()
    {
    }
    ///////////////////////////////////////////////////////// STATE: IDLE PATROL


    void StateInit_IdlePatrol()
    {
        navMeshAgent.speed = 6.0f;
        navMeshAgent.SetDestination(patrolNode.GetPosition());
    }
    void StateUpdate_IdlePatrol()
    {
        if (HasReachedMyDestination(1))
        {
            patrolNode = patrolNode.nextNode;
            navMeshAgent.SetDestination(patrolNode.GetPosition());
        }

    }
    void StateEnd_IdlePatrol()
    {
    }

    ///////////////////////////////////////////////////////// STATE: IDLE ROAMER


    Misc_Timer idleTimer = new Misc_Timer();
    Misc_Timer idleRotateTimer = new Misc_Timer();
    bool idleWaiting, idleMoving;
    void StateInit_IdleRoamer()
    {
        navMeshAgent.speed = 7.0f;

        idleTimer.StartTimer(Random.Range(2.0f, 4.0f));
        RandomRotate();
        AdvanceIdle();
        idleWaiting = false;
        idleMoving = true;

    }
    void StateUpdate_IdleRoamer()
    {

        idleTimer.UpdateTimer();

        if (idleMoving)
        {
            if (HasReachedMyDestination(0))
            {
                AdvanceIdle();

            }
        }
        else if (idleWaiting)
        {
            idleRotateTimer.UpdateTimer();
            if (idleRotateTimer.IsFinished())
            {
                RandomRotate();
                idleRotateTimer.StartTimer(Random.Range(1.5f, 3.25f));
            }

        }
        if (idleTimer.IsFinished())
        {
            if (idleMoving)
            {
                navMeshAgent.isStopped = true;
                float waitTime = Random.Range(2.5f, 6.5f);
                float randomTurnTime = waitTime / 2.0f;
                idleRotateTimer.StartTimer(randomTurnTime);
                idleTimer.StartTimer(waitTime);


            }
            else if (idleWaiting)
            {
                idleTimer.StartTimer(Random.Range(2.0f, 4.0f));

                AdvanceIdle();
            }

            idleMoving = !idleMoving;
            idleWaiting = !idleMoving;

        }

    }
    void StateEnd_IdleRoamer()
    {
    }
    void AdvanceIdle()
    {

        RaycastHit hit = new RaycastHit();
        Physics.Raycast(transform.position, transform.forward * 5.0f, out hit, 50.0f, hitTestLayer);
        //Debug.DrawRay (transform.position, transform.forward, Color.red);

        if (hit.distance < 3.0f)
        {
            Vector3 dir = hit.point - transform.position;
            Vector3 reflectedVector = Vector3.Reflect(dir, hit.normal);
            Physics.Raycast(transform.position, reflectedVector, out hit, 50.0f, hitTestLayer);
        }

        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(hit.point);


    }
    ///////////////////////////////////////////////////////// STATE: HELP!
    protected virtual void StateInit_Help()
    {
    }
    protected virtual void StateUpdate_Help()
    {
    }
    protected virtual void StateEnd_Help()
    {
    }
    ///////////////////////////////////////////////////////// STATE: INSPECT
    protected Misc_Timer inspectTimer = new Misc_Timer();
    protected Misc_Timer inspectTurnTimer = new Misc_Timer();
    protected bool inspectWait;
    void StateInit_Inspect()
    {
        navMeshAgent.speed = 16.0f;
        navMeshAgent.isStopped = false;
        inspectTimer.StopTimer();
        inspectWait = false;
    }
    void StateUpdate_Inspect()
    {
        if (HasReachedMyDestination(1.5f) && !inspectWait)
        {
            inspectWait = true;
            inspectTimer.StartTimer(2.0f);
            inspectTurnTimer.StartTimer(1.0f);
        }
        navMeshAgent.SetDestination(targetPos);
        TargetCheck();

        if (inspectWait)
        {
            inspectTimer.UpdateTimer();
            inspectTurnTimer.UpdateTimer();
            if (inspectTurnTimer.IsFinished())
            {
                RandomRotate();
                inspectTurnTimer.StartTimer(Random.Range(0.5f, 1.25f));
            }
            if (inspectTimer.IsFinished())
                SetState(idleState);
        }
    }
    protected virtual void TargetCheck()
    {
    }
    void StateEnd_Inspect()
    {
    }

    ///////////////////////////////////////////////////////// STATE: ATTACK
    Misc_Timer attackActionTimer = new Misc_Timer();
    bool actionDone;
    void StateInit_Attack()
    {
        if (!actionDone) return;
        navMeshAgent.isStopped = true;
        navMeshAgent.velocity = Vector3.zero;
        npcAnimator.SetBool("Attack", true);
        //CancelInvoke("AttackAction");
        Invoke("AttackAction", weaponContact);
        attackActionTimer.StartTimer(weaponReload);

        actionDone = false;
    }
    void StateUpdate_Attack()
    {
        attackActionTimer.UpdateTimer();
        if (!actionDone && attackActionTimer.IsFinished())
        {
            EndAttack();
            actionDone = true;
        }
    }
    void StateEnd_Attack()
    {
        npcAnimator.SetBool("Attack", false);
    }
    void EndAttack()
    {
        SetState(NPC_EnemyState.INSPECT);
    }
    protected virtual void AttackAction()
    {
    }
    ////////////////////////// MISC FUNCTIONS //////////////////////////

    void RandomRotate()
    {
        float randomAngle = Random.Range(45, 180);
        float randomSign = Random.Range(0, 2);
        if (randomSign == 0)
            randomAngle *= -1;

        transform.Rotate(0, randomAngle, 0);
    }
    /*float randomMoveInnerRadius=0.5f, randomMoveOuterRadius=10.0f;
	private Vector3 GetRandomPoint(){	
		Vector3 newPos;
		//do{
			newPos=Random.insideUnitSphere * randomMoveOuterRadius;
		//}while(newPos.x <randomMoveInnerRadius && newPos.y<randomMoveInnerRadius);
		Vector3 finalPos = transform.position + newPos;

		return finalPos;
	}*/
    public bool HasReachedMyDestination(float range)
    {
        float dist = Vector3.Distance(transform.position, navMeshAgent.destination);
        if (dist <= range)
        {
            return true;
        }

        return false;
    }
    ////////////////////////// PUBLIC FUNCTIONS //////////////////////////
    public void SetAlertPos(Vector3 newPos)
    {
        if (idleState != NPC_EnemyState.IDLE_STATIC)
        {
            SetTargetPos(newPos);
        }
    }
    public void SetTargetPos(Vector3 newPos)
    {
        targetPos = newPos;
        if (currentState != NPC_EnemyState.ATTACK)
        {
            SetState(NPC_EnemyState.INSPECT);
        }
    }
    public virtual void Damage()
    {
        navMeshAgent.velocity = Vector3.zero;
        //navMeshAgent.Stop ();
        npcAnimator.SetBool("Dead", true);
        GameManager.AddScore(100);
        npcAnimator.transform.parent = null;
        Vector3 pos = npcAnimator.transform.position;
        pos.y = 0.2f;
        npcAnimator.transform.position = pos;
        GameManager.RemoveEnemy();
        Destroy(gameObject);
    }
}
