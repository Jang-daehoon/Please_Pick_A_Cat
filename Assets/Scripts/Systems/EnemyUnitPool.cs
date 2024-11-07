using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnitPool : MonoBehaviour
{
    public static EnemyUnitPool pool;

    public EnemyUnit unitPrefab;

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
    private Queue<EnemyUnit> poolQueue = new Queue<EnemyUnit>(); // Queue�� ����Ͽ� �� ȿ�������� ����

    /// <summary>
    ///  �� ���� ��������
    /// </summary>
    /// <returns>������ �� ����</returns>
    public EnemyUnit Pop()
    {
        if (poolQueue.Count <= 0) // Ǯ�� ���� ��ü�� ������ ���� ����
        {
            Push(Instantiate(unitPrefab));
        }

        EnemyUnit enemyUnit = poolQueue.Dequeue(); // ť���� ������

        enemyUnit.gameObject.SetActive(true);
        enemyUnit.transform.SetParent(null); // ���� �ֻ����� ������
        return enemyUnit;
    }

    /// <summary>
    /// ����� �� ���� ����ֱ�
    /// </summary>
    /// <param name="enemyUnit">����� �÷��̾� ����</param>
    public void Push(EnemyUnit enemyUnit)
    {
        poolQueue.Enqueue(enemyUnit); // ť�� �߰�
        enemyUnit.gameObject.SetActive(false); // ��Ȱ��ȭ
        enemyUnit.transform.SetParent(transform, false); // �θ� ����
    }

    /// <summary>
    /// ����� �� ���� ����ֱ� (�����ð� ����)
    /// </summary>
    /// <param name="enemyUnit">����� �÷��̾� ����</param>
    /// <param name="delay">�����ð�</param>
    public void Push(EnemyUnit enemyUnit, float delay)
    {
        StartCoroutine(PushCoroutine(enemyUnit, delay));
    }

    // �����ð��� �ΰ� ������ Ǯ�� ����ִ� �ڷ�ƾ
    private IEnumerator PushCoroutine(EnemyUnit enemyUnit, float delay)
    {
        yield return new WaitForSeconds(delay);
        Push(enemyUnit);
    }
}
