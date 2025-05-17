using UnityEngine;
using System.Collections;
public class NPC_Enemy : NPC_Base
{
    // Use this for initialization

    protected override void Start()
    {
        base.Start();
        GameManager.AddToEnemyCount();
    }
    protected override void TargetCheck()
    {
        RaycastHit hit = new RaycastHit();
        Physics.Raycast(transform.position, transform.forward, out hit, weaponRange, hitTestLayer);

        if (hit.collider != null && hit.collider.tag == "Player")
        {
            SetState(NPC_EnemyState.ATTACK);
        }
    }
    protected override void AttackAction()
    {
        switch (weaponType)
        {
            case NPC_WeaponType.KNIFE:
                RaycastHit[] hits = Physics.SphereCastAll(weaponPivot.position, 2.0f, weaponPivot.forward);
                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider != null && hit.collider.tag == "Player")
                    {
                        hit.collider.GetComponent<PlayerBehavior>().DamagePlayer();
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
}
