using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnitPool : MonoBehaviour
{
    public static PlayerUnitPool pool;

    public PlayerUnit unitPrefab;

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
    private Queue<PlayerUnit> poolQueue = new Queue<PlayerUnit>(); // Queue를 사용하여 더 효율적으로 관리

    /// <summary>
    /// 아군 유닛 꺼내오기
    /// </summary>
    /// <returns>꺼내온 아군 유닛</returns>
    public PlayerUnit Pop()
    {
        if (poolQueue.Count <= 0) // 풀에 꺼낼 객체가 없으면 새로 생성
        {
            Push(Instantiate(unitPrefab));
        }

        PlayerUnit playerUnit = poolQueue.Dequeue(); // 큐에서 꺼내기

        playerUnit.gameObject.SetActive(true);
        playerUnit.transform.SetParent(null); // 월드 최상위로 꺼내옴
        return playerUnit;
    }

    /// <summary>
    /// 사망한 플레이어 유닛 집어넣기
    /// </summary>
    /// <param name="playerUnit">사망한 플레이어 유닛</param>
    public void Push(PlayerUnit playerUnit)
    {
        poolQueue.Enqueue(playerUnit); // 큐에 추가
        playerUnit.gameObject.SetActive(false); // 비활성화
        playerUnit.transform.SetParent(transform, false); // 부모 변경
    }

    /// <summary>
    /// 사망한 플레이어 유닛 집어넣기 (지연시간 있음)
    /// </summary>
    /// <param name="playerUnit">사망한 플레이어 유닛</param>
    /// <param name="delay">지연시간</param>
    public void Push(PlayerUnit playerUnit, float delay)
    {
        StartCoroutine(PushCoroutine(playerUnit, delay));
    }

    // 지연시간을 두고 유닛을 풀에 집어넣는 코루틴
    private IEnumerator PushCoroutine(PlayerUnit playerUnit, float delay)
    {
        yield return new WaitForSeconds(delay);
        Push(playerUnit);
    }
}
