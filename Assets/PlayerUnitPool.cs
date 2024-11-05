using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnitPool : MonoBehaviour
{
    public static PlayerUnitPool pool;

    public PlayerUnit unitPrefab;

    private void Awake()
    {
        // pool�� ���� null�� ��쿡�� �ʱ�ȭ
        if (pool == null)
        {
            pool = this;
        }
        else if (pool != this)
        {
            Destroy(gameObject);  // �ߺ� �ʱ�ȭ ����
        }
    }

    [SerializeField]
    private Queue<PlayerUnit> poolQueue = new Queue<PlayerUnit>(); // Queue�� ����Ͽ� �� ȿ�������� ����

    /// <summary>
    /// �Ʊ� ���� ��������
    /// </summary>
    /// <returns>������ �Ʊ� ����</returns>
    public PlayerUnit Pop()
    {
        if (poolQueue.Count <= 0) // Ǯ�� ���� ��ü�� ������ ���� ����
        {
            Push(Instantiate(unitPrefab));
        }

        PlayerUnit playerUnit = poolQueue.Dequeue(); // ť���� ������

        playerUnit.gameObject.SetActive(true);
        playerUnit.transform.SetParent(null); // ���� �ֻ����� ������
        return playerUnit;
    }

    /// <summary>
    /// ����� �÷��̾� ���� ����ֱ�
    /// </summary>
    /// <param name="playerUnit">����� �÷��̾� ����</param>
    public void Push(PlayerUnit playerUnit)
    {
        poolQueue.Enqueue(playerUnit); // ť�� �߰�
        playerUnit.gameObject.SetActive(false); // ��Ȱ��ȭ
        playerUnit.transform.SetParent(transform, false); // �θ� ����
    }

    /// <summary>
    /// ����� �÷��̾� ���� ����ֱ� (�����ð� ����)
    /// </summary>
    /// <param name="playerUnit">����� �÷��̾� ����</param>
    /// <param name="delay">�����ð�</param>
    public void Push(PlayerUnit playerUnit, float delay)
    {
        StartCoroutine(PushCoroutine(playerUnit, delay));
    }

    // �����ð��� �ΰ� ������ Ǯ�� ����ִ� �ڷ�ƾ
    private IEnumerator PushCoroutine(PlayerUnit playerUnit, float delay)
    {
        yield return new WaitForSeconds(delay);
        Push(playerUnit);
    }
}
