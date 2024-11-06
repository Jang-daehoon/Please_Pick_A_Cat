using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : Character
{
    [SerializeField] EnemyUnitSO enemyUnitSO;

    public void OnEnable()
    {
        UnitInit(enemyUnitSO);
        ResetUnit();
    }

    public void UnitInit(EnemyUnitSO enemyUnit)
    {
        name = enemyUnit.name;
        // 유닛이 죽지 않은 상태로 초기화
        isDead = false;

        // enemyUnitSO에서 유닛 정보를 가져와 초기화
        CharacterName = enemyUnit.unitName; // 유닛 이름 할당
        damage = enemyUnit.unitDamage; // 유닛의 데미지 값 할당
        maxHp = enemyUnit.maxHp; // 최대 체력 값 할당
        // 현재 체력은 최대 체력으로 초기화
        curHp = maxHp;
        attackDelay = enemyUnit.attackDelay; // 공격 딜레이 값 할당
        moveSpeed = enemyUnit.moveSpeed; // 이동 속도 값 할당
        attackRadius = enemyUnit.attackRadius; // 공격 범위 값 할당
        spawnCoolTime = enemyUnit.spawnCoolTime; // 생성 쿨타임 값 할당
        Cost = enemyUnit.usedCost;  //사용되는 코스트


        // 유닛의 게임 오브젝트 태그와 레이어 설정 (필요시 수정)
        gameObject.tag = "Enemy"; // 태그를 "Enemy"로 설정
        gameObject.layer = 6; // 레이어를 적합한 레이어 값으로 설정 (여기서는 6번 예시)

        // 애니메이터에 "Dead" 상태를 반영
        animator.SetBool("Dead", isDead); // "Dead" 애니메이션 상태를 false로 설정
    }
    public void ResetUnit()
    {
        // 전투 관련 상태 초기화
        isMoved = true;
        canAttack = false;
        ClearAttackTargets();

        // 물리 관련 초기화
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }

        // 애니메이터 파라미터 초기화
        if (animator != null)
        {
            animator.SetFloat("Speed", 0);
            animator.ResetTrigger("Attack");
            animator.ResetTrigger("Hit");
            animator.ResetTrigger("Dead");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue; // 원의 색상 설정
        Gizmos.DrawWireSphere(transform.position, attackRadius); // 공격 범위를 원으로 표시
    }

    protected override void Move()
    {
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        rb.velocity = new Vector2(moveSpeed, rb.velocity.y); // 이동
    }

    protected override void AttackRange(float radius)
    {
        Bounds bounds = col.bounds;
        CenterPos = new Vector2(bounds.center.x, bounds.center.y);

        Collider2D[] attackRange = Physics2D.OverlapCircleAll(CenterPos, radius, TargetLayer);
        bool enemyDetected = false; // 적 탐지 여부 변수

        // 공격 타겟 배열 초기화
        ClearAttackTargets();

        int targetCount = 0; // 탐지된 적의 수

        foreach (Collider2D target in attackRange)
        {
            if (target.gameObject.activeSelf == false)
            {
                // 비활성화된 타겟은 배열에서 제거
                RemoveTarget(target.gameObject);
            }

            // "Player"나 "PlayerTower"와 같은 타겟만 공격 리스트에 추가
            if ((target.CompareTag("Player") || target.CompareTag("PlayerTower")) && target.gameObject.activeSelf == true && targetCount < 2)
            {
                // 타겟이 배열에 아직 없다면 추가
                AddTarget(target.gameObject);
                targetCount++; // 탐지된 적의 수 증가
                enemyDetected = true; // 적이 탐지됨
            }
        }

        if (enemyDetected)
        {
            isMoved = false;
            if (!canAttack) // 공격 상태가 아닐 때만 공격 시작
            {
                StartCoroutine(AttackTargets()); // 공격 수행
            }
            rb.velocity = Vector2.zero;
            animator.SetFloat("Speed", 0);
        }
        else
        {
            // 적이 탐지되지 않았을 경우 다시 이동 가능
            isMoved = true;
            canAttack = false;
        }
    }

    protected override void AddTarget(GameObject target)
    {
        // 공격할 수 있는 최대 2명의 타겟만 추가
        for (int i = 0; i < attackTarget.Length; i++)
        {
            if (attackTarget[i] == null)
            {
                attackTarget[i] = target;
                break;
            }
            else if (attackTarget[i].activeSelf == false)
            {
                // 죽은 타겟이라면 새로 바꿔줌
                ClearAttackTargets();
                break;
            }
        }
    }

    protected override IEnumerator AttackTargets()
    {
        canAttack = true; // 공격 가능 상태로 변경

        // 공격할 타겟이 있으면 공격
        for (int i = 0; i < attackTarget.Length; i++)
        {
            if (attackTarget[i] != null && attackTarget[i].activeSelf == true)
            {
                Attack(attackTarget[i].GetComponent<Collider2D>());
            }
        }

        animator.SetTrigger("Attack"); // 애니메이션 트리거는 루프 바깥에서 한 번만 호출

        yield return new WaitForSeconds(attackDelay); // 공격 간의 지연 시간

        canAttack = false; // 공격 완료 후 공격 불가능 상태로 변경
        isMoved = true; // 공격 후 다시 이동 가능
    }

    protected override void Attack(Collider2D target)
    {
        if (target == null) return;

        if (target.CompareTag("Player"))
        {
            PlayerUnit playerUnit = target.GetComponent<PlayerUnit>(); // 적의 스크립트 참조

            if (playerUnit != null)
            {
                playerUnit.TakeDamage(damage);
                Debug.Log($"{CharacterName}가 {playerUnit.name}에게 {damage}의 피해를 입혔습니다.");

                if (playerUnit.curHp <= 0)
                {
                    RemoveTarget(playerUnit.gameObject); // 타겟 제거
                    playerUnit.curHp = 0;
                }
            }
        }
        else if (target.CompareTag("PlayerTower"))
        {
            if (GameManager.Instance.playerTowerCurHp <= 0)
            {
                Debug.Log("아군의 성체가 파괴되었습니다.");
                GameManager.Instance.isDefeat = true;
            }
            else
            {
                GameManager.Instance.playerTowerCurHp -= damage;
                Debug.Log("아군 타워가 공격받고있습니다.");
            }
        }
    }
    public override void Dead()
    {
        isDead = true;
        ClearAttackTargets(); // 기존 메서드 사용
        // 전투 상태 초기화
        canAttack = false;
        isMoved = false;

        gameObject.tag = "Untagged";
        gameObject.layer = 0;
        animator.SetTrigger("Dead");
        GameObject AcensionObj = Instantiate(GameManager.Instance.AcensionPrefab, transform.position, transform.rotation, null);

        transform.position = new Vector2(-100, -100);

        // 현재 실행 중인 코루틴들을 모두 중지
        StopAllCoroutines();
        // 애니메이터 파라미터 초기화
        if (animator != null)
        {
            animator.SetFloat("Speed", 0);
            animator.ResetTrigger("Attack");
            animator.ResetTrigger("Hit");
            animator.ResetTrigger("Dead");
        }
        EnemyUnitPool.pool.Push(this, 0.5f);
    }
    public void OnDeath()
    {
        // 타겟이 죽었을 때, 공격 범위에서 해당 타겟을 제거
        RemoveTarget(gameObject);
    }
}
