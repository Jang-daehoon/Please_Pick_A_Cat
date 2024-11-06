using UnityEngine;
using UnityEngine.U2D;

public class PlayerUnit : Character
{
    [SerializeField] PlayerUnitSO playerUnitSo;

    private void OnEnable()
    {
        Unitinit(playerUnitSo);
        ResetUnit(); // 추가: 유닛의 전투 관련 상태 초기화
    }

    public void Unitinit(PlayerUnitSO playerUnitSo)
    {
        name = playerUnitSo.name;
        isDead = false;
        maxHp = playerUnitSo.MaxHp;
        curHp = maxHp;
        gameObject.tag = "Player";
        gameObject.layer = 3;
        CharacterName = playerUnitSo.UnitName;
        Cost = playerUnitSo.UsedCost;
        damage = playerUnitSo.UnitDamage;
        attackDelay = playerUnitSo.AttackDelay;
        moveSpeed = playerUnitSo.MoveSpeed;
        attackRadius = playerUnitSo.AttackRadius;
        spawnCoolTime = playerUnitSo.SpawnCoolTime;

    }

    // 추가: 유닛의 전투 관련 상태를 초기화하는 메서드
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
        }
        // SpriteRenderer 색상 초기화 (피격 후 색상 변경 방지)
        SpriteRenderer spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;  // 피격 후 색상 초기화
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

        GameObject AcensionObj = Instantiate(GameManager.Instance.AcensionPrefab, transform.position, transform.rotation, null);
        GameObject DeadParticleActive = Instantiate(DeadParticle, transform.position, transform.rotation, null);
        Destroy(DeadParticleActive, 2f);
        transform.position = new Vector2(100, 100);
        // 현재 실행 중인 코루틴들을 모두 중지
        StopAllCoroutines();
        // SpriteRenderer 색상 초기화 (피격 후 색상 변경 방지)
        SpriteRenderer spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;  // 피격 후 색상 초기화
        }
        PlayerUnitPool.pool.Push(this, 1f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}