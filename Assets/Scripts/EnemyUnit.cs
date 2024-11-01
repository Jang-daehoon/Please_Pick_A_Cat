using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : Character
{
    [SerializeField] EnemyUnitSO enemyUnitSO;

    private void OnEnable()
    {
        Unitinit();
    }
    private void Unitinit()
    {
        CharacterName = enemyUnitSO.unitName;
        damage = enemyUnitSO.unitDamage;
        maxHp = enemyUnitSO.maxHp;
        attackDelay = enemyUnitSO.attackDelay;
        moveSpeed = enemyUnitSO.moveSpeed;
        attackRadius = enemyUnitSO.attackRadius;
        spawnCoolTime = enemyUnitSO.spawnCoolTime;

        curHp = maxHp;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue; // 원의 색상 설정
        Gizmos.DrawWireSphere(transform.position, attackRadius); // 공격 범위를 원으로 표시
    }
    protected override void Move()
    {
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
    }
    protected override void AttackRange(float radius)
    {
        Bounds bounds = col.bounds;
        CenterPos = new Vector2(bounds.center.x, bounds.center.y);

        Collider2D[] attackRange = Physics2D.OverlapCircleAll(CenterPos, radius, TargetLayer);
        bool enemyDetected = false; // 적 탐지 여부 변수
        Targets.Clear(); // 매 프레임마다 타겟 리스트 초기화
        foreach (Collider2D target in attackRange)
        {
            if (target.CompareTag("Player") || target.CompareTag("PlayerTower"))
            {
                AddTarget(target.gameObject);
                enemyDetected = true; // 적이 탐지됨
            }
        }
        if (enemyDetected)
        {
            isMoved = false;
            if (!canAttack) // 공격 상태가 아닐 때만 시작
            {
                StartCoroutine(AttackTargets()); // 적 공격
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
    //적 추가상태는 PlayerUnit과 동일한 로직으로 탐색
    //공격로직또한 동일하다. 부모의 공격로직을 그대로 사용
    protected override void Attack(Collider2D target)
    {
        // 공격 로직 
        if (target != null && target.CompareTag("Player"))
        {
            PlayerUnit playerUnit = target.GetComponent<PlayerUnit>(); // 적의 스크립트 참조
            if (playerUnit != null)
            {
                if (playerUnit.curHp <= 0)
                {
                    // 리스트에서 제거
                    Targets.Remove(playerUnit.gameObject);
                }
                playerUnit.TakeDamage(damage);
                Debug.Log(CharacterName + "가 " + playerUnit.name + "에게 " + damage + "의 피해를 입혔습니다.");
            }
        }
        else if(target != null && target.CompareTag("PlayerTower"))
        {
            if(GameManager.Instance.playerTowerCurHp <=0)
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
}
