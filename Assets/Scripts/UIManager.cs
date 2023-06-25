using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance = null;
    [Header("Start UI")]
    [SerializeField] private GameObject startTextObj;
    [SerializeField] private GameObject contactTextObj;
    private bool isContactPos = false;

    [Header("Reward Text Event")]
    [SerializeField] private GameObject[] rewardTextObj;
    [SerializeField] private GameObject rewardPrefab;

    [Header("Cart Count")]
    [SerializeField] private Text cartCountText;
    
    [Header("Coin UI")]
    [SerializeField] private GameObject panelObj;
    [SerializeField] private GameObject coinPrefab;
    private RectTransform coinRectTr;
    private bool isCreateCoinUI = false;
    private Text       coinText;
    private GameObject coinObj;
    private int coinCount;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this.gameObject);
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartTextUI());
        StartCoroutine(StartContactTextUI());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator StartTextUI()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.3f);
            

            if(Input.GetMouseButton(0))
            {
                startTextObj.SetActive(false);
                Destroy(startTextObj);
                yield break;
            }
        }
    }

    IEnumerator StartContactTextUI()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.3f);

            if(isContactPos)
            {
                contactTextObj.SetActive(true);
                yield return new WaitForSeconds(3.5f);
                Destroy(contactTextObj);
                yield break;
            }
        }
    }

    public void AddCoinUi()
    {
        if(!isCreateCoinUI)
        {
            isCreateCoinUI = true;
            coinObj = Instantiate(coinPrefab, panelObj.transform.position, Quaternion.identity);
            coinRectTr = coinObj.GetComponent<RectTransform>();
            Image coinImage = coinObj.transform.GetChild(0).GetComponent<Image>();
            coinText = coinObj.GetComponentInChildren<Text>();
            coinObj.transform.parent = panelObj.transform;
            coinCount = 100;
        }
        else
        {
            coinCount += 100;
            coinText.text = $"{coinCount}";
        }
    }

    public void VisibleReward(int count)
    {
        if(count < 10)
        {
            StartCoroutine(VisibleTimer(0));
        }
        else
        {
            StartCoroutine(VisibleTimer(1));
        }
    }

    public void SetCartCountUI(int cnt, int maxCnt)
    {
        if(cnt >= maxCnt)
        {
            cartCountText.text = $"Max!!";
        }
        else
        {
            cartCountText.text = $"{cnt}";
        }
        
    }

    IEnumerator VisibleTimer(int idx)
    {
        GameObject rewardObj = Instantiate(rewardPrefab, rewardTextObj[idx].transform.position, Quaternion.identity);
        rewardTextObj[idx].SetActive(true);
        yield return new WaitForSeconds(2.0f);
        rewardTextObj[idx].SetActive(false);
        Destroy(rewardObj);
    }


    public RectTransform GetCoinRectTr()
    {
        return coinRectTr;
    }

    public void SetIsContact(bool _contact)
    {
        isContactPos = _contact;
    }


}
