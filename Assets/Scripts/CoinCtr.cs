using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinCtr : MonoBehaviour
{
    UIManager uimgr;
    [SerializeField] float moveSpeed = 5f; // �̵� �ӵ�
    private RectTransform uiRectTransform; // UI ������Ʈ�� RectTransform
    Vector2 screenPos;

    private Vector2 targetPosition; // ����� ��ǥ ��ġ

    private void Start()
    {
        screenPos = Camera.main.WorldToScreenPoint(transform.position);

       
    }

    private void Update()
    {
        // ����� ���� ��ġ���� ��ǥ ��ġ���� �ε巴�� �̵�
        transform.position = Vector2.Lerp(screenPos, targetPosition, moveSpeed * Time.deltaTime);
    }

    public void SetCoinRectTr(RectTransform _coinRectTr)
    {
        uiRectTransform = _coinRectTr;
        targetPosition = uiRectTransform.position;
    }
}
