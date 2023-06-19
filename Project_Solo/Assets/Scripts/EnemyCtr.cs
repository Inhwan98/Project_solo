using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyCtr : MonoBehaviour
{
    private readonly int hashHit = Animator.StringToHash("OnHit");
    private readonly int hashDie = Animator.StringToHash("IsDie");
    private readonly int hashDieNum = Animator.StringToHash("DieNum");

    private int RandNum;
    [SerializeField] private float hp;

    private Animator anim;
    private NavMeshAgent agent;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        RandNum = Random.Range(0,2);
    }
    private void OnCollisionEnter(Collision coll)
    {
        if(coll.gameObject.layer == LayerMask.NameToLayer("PlayerWeapon"))
        {
            Destroy(coll.gameObject);
            hp -= 60.0f;
            if(hp <= 0f)
            {
                anim.SetInteger(hashDieNum, RandNum);
                anim.SetBool(hashDie, true);
                this.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

                foreach(Transform childTr in transform)
                {
                    childTr.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                    foreach(Transform childTr2 in childTr)
                    {
                        childTr2.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                    }
                }
                //Destroy(this.gameObject, 4.0f);
            }
            else
            {
                anim.SetTrigger(hashHit);
            }
            
        }
    }
}
