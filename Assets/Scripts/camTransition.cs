using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CamTransition : MonoBehaviour
{
    public float moveSpeed = 5f; // 이동 속도
    private CinemachineVirtualCamera vCam;
    public float originalOrthoSize;
    public bool isExpanded; // 확대 상태
    public bool isCollapsed; // 축소 상태

    private void Awake()
    {
        vCam = GetComponent<CinemachineVirtualCamera>();
        originalOrthoSize = 15f;
    }

    void Update()
    {
        Vector3 movement = Vector3.zero;

        // 카메라를 왼쪽으로 이동
        if (Input.GetKey(KeyCode.A))
        {
            movement += Vector3.left;
        }
        // 카메라를 오른쪽으로 이동
        else if (Input.GetKey(KeyCode.D))
        {
            movement += Vector3.right;
        }

        if (Input.GetKeyDown(KeyCode.W) && isExpanded == false && isCollapsed == false)
        {
            isExpanded = true;
            vCam.m_Lens.OrthographicSize = 35;
        }
        else if(Input.GetKeyDown(KeyCode.W) && isExpanded == true && isCollapsed == false)
        {
            isExpanded = false;
            vCam.m_Lens.OrthographicSize = originalOrthoSize;
        }

        if (Input.GetKeyDown(KeyCode.S) && isExpanded == false && isCollapsed == false)
        {
            isCollapsed = true;
            vCam.m_Lens.OrthographicSize = 5;
        }
        else if(Input.GetKeyDown(KeyCode.S) && isExpanded == false && isCollapsed == true)
        {
            isCollapsed = false;
            vCam.m_Lens.OrthographicSize = originalOrthoSize;
        }


        // 카메라의 현재 위치를 부드럽게 이동
        Vector3 newPosition = transform.position + movement * moveSpeed * Time.deltaTime;

        // x축 좌표 제한
        newPosition.x = Mathf.Clamp(newPosition.x, -45f, 45f);

        // 카메라 위치 업데이트
        transform.position = newPosition;
    }
}
