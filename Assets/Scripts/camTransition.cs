using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CamTransition : MonoBehaviour
{
    public float moveSpeed = 5f; // �̵� �ӵ�
    private CinemachineVirtualCamera vCam;
    public float originalOrthoSize;
    public bool isExpanded; // Ȯ�� ����
    public bool isCollapsed; // ��� ����

    private void Awake()
    {
        vCam = GetComponent<CinemachineVirtualCamera>();
        originalOrthoSize = 15f;
    }

    void Update()
    {
        Vector3 movement = Vector3.zero;

        // ī�޶� �������� �̵�
        if (Input.GetKey(KeyCode.A))
        {
            movement += Vector3.left;
        }
        // ī�޶� ���������� �̵�
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


        // ī�޶��� ���� ��ġ�� �ε巴�� �̵�
        Vector3 newPosition = transform.position + movement * moveSpeed * Time.deltaTime;

        // x�� ��ǥ ����
        newPosition.x = Mathf.Clamp(newPosition.x, -45f, 45f);

        // ī�޶� ��ġ ������Ʈ
        transform.position = newPosition;
    }
}
