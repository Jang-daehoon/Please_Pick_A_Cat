using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnitPool : MonoBehaviour
{
    public static EnemyUnitPool pool;

    public EnemyUnit unitPrefab;

    private void Awake()
    {
        // pool이 아직 null일 경우에만 초기화
        if (pool == null)
        {
            pool = this;
        }
        else if (pool != this)
        {
            Destroy(gameObject);  // 중복 초기화 방지
        }
    }

    [SerializeField]
    private Queue<EnemyUnit> poolQueue = new Queue<EnemyUnit>(); // Queue를 사용하여 더 효율적으로 관리

    /// <summary>
    ///  적 유닛 꺼내오기
    /// </summary>
    /// <returns>꺼내온 적 유닛</returns>
    public EnemyUnit Pop()
    {
        if (poolQueue.Count <= 0) // 풀에 꺼낼 객체가 없으면 새로 생성
        {
            Push(Instantiate(unitPrefab));
        }

        EnemyUnit enemyUnit = poolQueue.Dequeue(); // 큐에서 꺼내기

        enemyUnit.gameObject.SetActive(true);
        enemyUnit.transform.SetParent(null); // 월드 최상위로 꺼내옴
        return enemyUnit;
    }

    /// <summary>
    /// 사망한 적 유닛 집어넣기
    /// </summary>
    /// <param name="enemyUnit">사망한 플레이어 유닛</param>
    public void Push(EnemyUnit enemyUnit)
    {
        poolQueue.Enqueue(enemyUnit); // 큐에 추가
        enemyUnit.gameObject.SetActive(false); // 비활성화
        enemyUnit.transform.SetParent(transform, false); // 부모 변경
    }

    /// <summary>
    /// 사망한 적 유닛 집어넣기 (지연시간 있음)
    /// </summary>
    /// <param name="enemyUnit">사망한 플레이어 유닛</param>
    /// <param name="delay">지연시간</param>
    public void Push(EnemyUnit enemyUnit, float delay)
    {
        StartCoroutine(PushCoroutine(enemyUnit, delay));
    }

    // 지연시간을 두고 유닛을 풀에 집어넣는 코루틴
    private IEnumerator PushCoroutine(EnemyUnit enemyUnit, float delay)
    {
        yield return new WaitForSeconds(delay);
        Push(enemyUnit);
    }
}
