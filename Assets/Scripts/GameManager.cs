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

    private bool[] isCartUse; // 사용중인 카트가 있다면 true 없다면 false
    private bool isCart = false; // 사용할 카트가 있는가?
    private int curCartCount; // 현재 모든 카트의 수

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
        isCartUse = new bool[curCartCount];
        isUsedPos = new bool[randTr.Length, 14];

        StartCoroutine(CheckNPCStart()); // Cart Count Check
        StartCoroutine(CheckCartUse());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator CheckNPCStart()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.3f);
            foreach(bool check in isCartUse)
            {
                //check가 true면 사용중이다.
                // 하나라도 false면 사용 가능한것
                if(check == false)
                {
                    isCart = true;
                    break;
                }
                else
                {
                    isCart = false;
                }
            }

        }
    }

    IEnumerator CheckCartUse()
    {
        while(true)
        {
            
            if(cartGroupTr.childCount != 0)
            {
                curCartCount = cartGroupTr.childCount;
                GameObject npcObj = Instantiate(npcPrefab, startTr.position, startTr.rotation);
                NPC_Controller npcCtr = npcObj.GetComponent<NPC_Controller>();
                for(int i = 0; i < curCartCount; i++)
                {
                    Transform cartTr = cartGroupTr.GetChild(i);
                    if(cartTr != null)
                    {
                        npcCtr.SetCartDestination(cartTr);
                        isCartUse[i] = true; // 해당 카트 사용중
                        break;
                    }
                }
                
            }
            yield return new WaitForSeconds(5.0f);
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
        foreach(Transform cartTr in _cartTrs)
        {
            cartTr.parent = cartGroupTr;
        }
    }


}
