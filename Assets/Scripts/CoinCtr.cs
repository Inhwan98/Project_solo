using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinCtr : MonoBehaviour
{
    UIManager uimgr;
    [SerializeField] float moveSpeed = 5f; // 이동 속도
    private RectTransform uiRectTransform; // UI 오브젝트의 RectTransform
    Vector3 screenPos;

    private Vector3 targetPosition; // 대상의 목표 위치

    private void Start()
    {
        screenPos = Camera.main.WorldToScreenPoint(transform.position);
    }

    private void Update()
    {
        // 대상의 현재 위치에서 목표 위치까지 부드럽게 이동
        transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    public void SetCoinRectTr(RectTransform _coinRectTr)
    {
        uiRectTransform = _coinRectTr;
        targetPosition = uiRectTransform.position;
    }
}
