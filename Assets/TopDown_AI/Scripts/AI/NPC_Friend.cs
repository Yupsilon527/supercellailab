using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Friend : NPC_Base
{
    public static NPC_Friend main;

    public ParticleSystem leroyParticle;
    public ParticleSystem helpParticle;
    private void Awake()
    {
        main = this;
    }
    protected override void AttackAction()
    {
        switch (weaponType)
        {
            case NPC_WeaponType.KNIFE:
                RaycastHit[] hits = Physics.SphereCastAll(weaponPivot.position, 2.0f, weaponPivot.forward);
                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider != null && hit.collider.tag == "Enemy")
                    {
                        hit.collider.GetComponent<NPC_Base>().Damage();
                    }
                }
                break;
            case NPC_WeaponType.RIFLE:
                GameObject bullet = GameObject.Instantiate(proyectilePrefab, weaponPivot.position, weaponPivot.rotation) as GameObject;
                bullet.transform.Rotate(0, Random.Range(-7.5f, 7.5f), 0);
                break;
            case NPC_WeaponType.SHOTGUN:
                for (int i = 0; i < 5; i++)
                {
                    GameObject birdshot = GameObject.Instantiate(proyectilePrefab, weaponPivot.position, weaponPivot.rotation) as GameObject;
                    birdshot.transform.Rotate(0, Random.Range(-15, 15), 0);
                }
                break;
        }
    }
    protected override void TargetCheck()
    {
        RaycastHit hit = new RaycastHit();
        Physics.Raycast(transform.position, transform.forward, out hit, weaponRange, hitTestLayer);

        if (hit.collider != null && hit.collider.tag == "Enemy")
        {
            SetState(NPC_EnemyState.ATTACK);
        }
    }
    public override void Damage()
    {
        Debug.Log("Ouch");
    }

    public void Attack()
    {
        if (leroyParticle != null)
            leroyParticle.Play();

        List<NPC_Enemy> found = new();
        foreach (var hit in Physics.SphereCastAll(transform.position, 100, Vector3.one))
        {
            if (hit.collider.CompareTag("Enemy") && hit.collider.TryGetComponent(out NPC_Enemy enemy))
            {
                found.Add(enemy);
            }
        }
        Debug.Log("FOUND TARGETS: " + found.Count);
        if (found.Count > 0)
        {
            found.Sort((NPC_Enemy a, NPC_Enemy b) => (a.transform.position - transform.position).sqrMagnitude.CompareTo((b.transform.position - transform.position).sqrMagnitude));

            Debug.Log(name +" ENGAGE TARGET " + found[0].name);
            targetPos = found[0].transform.position;
            idleState = NPC_EnemyState.IDLE_STATIC;
            SetState(NPC_EnemyState.INSPECT);
        }
        
    }
    public void Help()
    {
        idleState = NPC_EnemyState.FOLLOW_PLAYER;
        SetState(idleState);
    }
    protected override void StateInit_Help()
    {
        navMeshAgent.speed = 16.0f;
    }
    protected override void StateUpdate_Help()
    {
        if (PlayerBehavior.instance != null)
            navMeshAgent.SetDestination(PlayerBehavior.instance.transform.position);

        if (HasReachedMyDestination(5) && !inspectWait)
        {
            inspectWait = true;
            inspectTimer.StartTimer(.5f);
            navMeshAgent.isStopped = true;

            if (PlayerBehavior.instance.damaged)
            {
                PlayerBehavior.instance.Heal();
                npcAnimator.SetTrigger("Heal");
                if (helpParticle != null)
                    helpParticle.Play();
            }
        }
        else if (inspectWait)
        {
            inspectTimer.UpdateTimer();
            if (inspectTimer.IsFinished())
            {
                inspectWait = false;
                if (!HasReachedMyDestination(5))
                    navMeshAgent.isStopped = false;
            }
        }

    }
    protected virtual void StateEnd_Help()
    {
    }
}
