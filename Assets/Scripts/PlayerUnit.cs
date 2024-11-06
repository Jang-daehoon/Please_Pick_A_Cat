using UnityEngine;
using UnityEngine.U2D;

public class PlayerUnit : Character
{
    [SerializeField] PlayerUnitSO playerUnitSo;

    private void OnEnable()
    {
        Unitinit(playerUnitSo);
        ResetUnit(); // �߰�: ������ ���� ���� ���� �ʱ�ȭ
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

    // �߰�: ������ ���� ���� ���¸� �ʱ�ȭ�ϴ� �޼���
    public void ResetUnit()
    {
        // ���� ���� ���� �ʱ�ȭ
        isMoved = true;
        canAttack = false;
        ClearAttackTargets();

        // ���� ���� �ʱ�ȭ
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }

        // �ִϸ����� �Ķ���� �ʱ�ȭ
        if (animator != null)
        {
            animator.SetFloat("Speed", 0);
            animator.ResetTrigger("Attack");
            animator.ResetTrigger("Hit");
        }
        // SpriteRenderer ���� �ʱ�ȭ (�ǰ� �� ���� ���� ����)
        SpriteRenderer spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;  // �ǰ� �� ���� �ʱ�ȭ
        }
    }

    public override void Dead()
    {
        isDead = true;
        ClearAttackTargets(); // ���� �޼��� ���
        // ���� ���� �ʱ�ȭ
        canAttack = false;
        isMoved = false;

        gameObject.tag = "Untagged";
        gameObject.layer = 0;

        GameObject AcensionObj = Instantiate(GameManager.Instance.AcensionPrefab, transform.position, transform.rotation, null);
        GameObject DeadParticleActive = Instantiate(DeadParticle, transform.position, transform.rotation, null);
        Destroy(DeadParticleActive, 2f);
        transform.position = new Vector2(100, 100);
        // ���� ���� ���� �ڷ�ƾ���� ��� ����
        StopAllCoroutines();
        // SpriteRenderer ���� �ʱ�ȭ (�ǰ� �� ���� ���� ����)
        SpriteRenderer spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;  // �ǰ� �� ���� �ʱ�ȭ
        }
        PlayerUnitPool.pool.Push(this, 1f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}