using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Inan.NPC
{
    public class NPC_Controller : MonoBehaviour
    {
        private readonly int hashWalk = Animator.StringToHash("IsWalk");
        private readonly int hashThink = Animator.StringToHash("IsThink");

        [Header("NPC State")]
        [SerializeField] NPCState npcState = NPCState.CATCHCART;
        private NavMeshAgent agent;


        private Animator anim;
        private Transform destTr; // agent의 목적지. 처음 목적지는 카트
        [SerializeField] private Transform cartTr;

        private float time; //layer 바꿔주는 time
        private bool isCatchCart = false;
        GameManager gmr;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            anim = GetComponent<Animator>();
        }
        void Start()
        {
            
            StartCoroutine(CheckState());
            StartCoroutine(CheckAnimState());
        }

        // Update is called once per frame
        void Update()
        {
            gmr = GameManager.instance;
            //Cart 잡기
            ActionCatchCart();
            
        }

        void ActionCatchCart()
        {

            if (isCatchCart)
            {
                if (time <= 0.75)
                {
                    time += Time.deltaTime * 3f;
                }
                anim.SetLayerWeight(1, time);
            }
            else
            {
                if (time > 0)
                {
                    time -= Time.deltaTime * 2f;
                }
                anim.SetLayerWeight(1, time);
            }
        }

        IEnumerator CheckState()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.3f);
                switch (npcState)
                {
                    //카트에 근접하면 카트 잡는 레이어 활성화
                    case NPCState.CATCHCART:
                        if(Vector3.Distance(transform.position, destTr.position) <= 4.1f)
                        {
                            Debug.Log("Catch");
                            transform.LookAt(destTr.position + (Vector3.right));

                            agent.isStopped = true;
                            yield return new WaitForSeconds(0.3f);
                            destTr.parent = transform;
                            cartTr = destTr;
                            isCatchCart = true;

                            //catch후 첫 입장
                            destTr = gmr.GetRandTr();
                            agent.SetDestination(destTr.position);
                            agent.isStopped = false;

                            npcState = NPCState.MARTIN;
                        }
                        break;

                    case NPCState.MARTIN:
                        if(agent.remainingDistance <= 0.1f)
                        {
                            agent.isStopped = true;
                            
                            cartTr.SetParent(null);

                            Debug.Log("자식 분리");
                            anim.SetBool(hashThink, true);
                            isCatchCart = false;

                            
                        }
                        break;
                }
            }
        }

        IEnumerator CheckAnimState()
        {
            while(true)
            {
                yield return new WaitForSeconds(0.3f);
                if(agent.velocity.magnitude > 0.1f)
                {
                    anim.SetBool("IsWalk", true);
                }
                else
                {
                    anim.SetBool(hashWalk, false);
                }
            }
        }

        public void SetCartDestination(Transform _destTr)
        {
            agent.SetDestination(_destTr.position + (-_destTr.right * 4.0f));
            destTr = _destTr;
        }

        public void SetDestination(Transform _destTr)
        {
            agent.SetDestination(_destTr.position);
            destTr = _destTr;
        }
    }

    }
