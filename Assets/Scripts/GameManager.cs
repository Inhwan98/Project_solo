using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inan.NPC;
public class GameManager : MonoBehaviour
{
    static public GameManager instance = null;
    
    [Header("NPC Destination")]
    [SerializeField] private Transform cartGroupTr;
    [SerializeField] private Transform startTr;
    [SerializeField] private GameObject npcPrefab;

    [SerializeField] private Transform martInTr;
    [SerializeField] private Transform[] randTr;

    [SerializeField] private Transform[] payTr;
    [SerializeField] private Transform exitTrGroup;
    [SerializeField] private Transform finalTr;

    [SerializeField] private Transform angryTr;

    private int curCartCount; // 현재 모든 카트의 수

    private bool isAngryNpc;
    private GameObject angryObj;
    private NPC_Controller angryCtr;

    private bool[,] isUsedPos; // 누가 머무르고 있나
    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this.gameObject);
        }    
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        curCartCount = cartGroupTr.childCount;
        isUsedPos = new bool[randTr.Length, 14];

        StartCoroutine(CheckCartUse());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator CheckCartUse()
    {
        while(true)
        {
            yield return new WaitForSeconds(5.0f);
            if (cartGroupTr.childCount > 0)
            {
                if (isAngryNpc)
                {
                    isAngryNpc = false;
                    for(int i = 0; i < curCartCount; i++)
                    {
                        Transform cartTr = cartGroupTr.GetChild(0);
                        if (cartTr != null)
                        {
                            cartTr.SetParent(null);
                            angryCtr.SetCartDestination(cartTr);

                            break;
                        }
                    }
                    angryCtr = null;
                    angryObj = null;
                    continue;
                }

                GameObject npcObj = Instantiate(npcPrefab, startTr.position, startTr.rotation);
                NPC_Controller npcCtr = npcObj.GetComponent<NPC_Controller>();

                for(int i = 0; i < curCartCount; i++)
                {
                    Transform cartTr = cartGroupTr.GetChild(0);


                    if (cartTr != null)
                    {
                        cartTr.SetParent(null);
                        npcCtr.SetCartDestination(cartTr);
                        break;
                    }
                }
            }
            else
            {
                if (!isAngryNpc)
                {
                    angryObj = Instantiate(npcPrefab, startTr.position, startTr.rotation);
                    angryCtr = angryObj.GetComponent<NPC_Controller>();
                    angryCtr.SetNPCState(NPCState.ANGRY);
                    angryCtr.SetDestination(angryTr);

                    isAngryNpc = true;
                }

            }

        }
        
    }

    public Transform GetStartMartTr()
    {
        return martInTr;
    }

    public Transform GetRandTr()
    {
        int randIdx = Random.Range(0, randTr.Length);

        int childCnt = randTr[randIdx].childCount;
        int childIdx = Random.Range(0, childCnt);

        if (isUsedPos[randIdx, childIdx] == false)
        {
            isUsedPos[randIdx, childIdx] = true;
        }
        else
        {
            while (isUsedPos[randIdx, childIdx] != true)
            {
                randIdx = Random.Range(0, randTr.Length);
                childCnt = randTr[randIdx].childCount;
                childIdx = Random.Range(0, childCnt);
            }
            isUsedPos[randIdx, childIdx] = true;
        }

        return randTr[randIdx].GetChild(childIdx);
    }

    public void SetRandTr(Transform _tr)
    {
        for(int i = 0; i < randTr.Length; i++)
        {
            for(int j = 0; j < randTr[randTr.Length-1].childCount; j++)
            {
                if(randTr[i].GetChild(j) == _tr)
                {
                    isUsedPos[i, j] = false;
                }
            }
        }
    }


    public Transform GetPayTr()
    {
        return payTr[Random.Range(0,2)];
    }

    public Transform GetExitTr()
    {
        int childCount = exitTrGroup.childCount - 1;
        int randIdx = Random.Range(0, childCount);

        return exitTrGroup.GetChild(randIdx);
    }

    public Transform GetFinalTr()
    {
        return finalTr;
    }

    public void SetCartGroup(List<Transform> _cartTrs)
    {
        for(int i = 0; i< _cartTrs.Count; i++)
        {
            _cartTrs[i].parent = cartGroupTr;
        }
    }

    public int GetCartGroupChildCount()
    {
        return cartGroupTr.childCount;
    }


}
