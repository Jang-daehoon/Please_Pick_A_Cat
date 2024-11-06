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
        // ������ ���� ���� ���·� �ʱ�ȭ
        isDead = false;

        // enemyUnitSO���� ���� ������ ������ �ʱ�ȭ
        CharacterName = enemyUnit.unitName; // ���� �̸� �Ҵ�
        damage = enemyUnit.unitDamage; // ������ ������ �� �Ҵ�
        maxHp = enemyUnit.maxHp; // �ִ� ü�� �� �Ҵ�
        // ���� ü���� �ִ� ü������ �ʱ�ȭ
        curHp = maxHp;
        attackDelay = enemyUnit.attackDelay; // ���� ������ �� �Ҵ�
        moveSpeed = enemyUnit.moveSpeed; // �̵� �ӵ� �� �Ҵ�
        attackRadius = enemyUnit.attackRadius; // ���� ���� �� �Ҵ�
        spawnCoolTime = enemyUnit.spawnCoolTime; // ���� ��Ÿ�� �� �Ҵ�
        Cost = enemyUnit.usedCost;  //���Ǵ� �ڽ�Ʈ


        // ������ ���� ������Ʈ �±׿� ���̾� ���� (�ʿ�� ����)
        gameObject.tag = "Enemy"; // �±׸� "Enemy"�� ����
        gameObject.layer = 6; // ���̾ ������ ���̾� ������ ���� (���⼭�� 6�� ����)

        // �ִϸ����Ϳ� "Dead" ���¸� �ݿ�
        animator.SetBool("Dead", isDead); // "Dead" �ִϸ��̼� ���¸� false�� ����
    }
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
            animator.ResetTrigger("Dead");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue; // ���� ���� ����
        Gizmos.DrawWireSphere(transform.position, attackRadius); // ���� ������ ������ ǥ��
    }

    protected override void Move()
    {
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        rb.velocity = new Vector2(moveSpeed, rb.velocity.y); // �̵�
    }

    protected override void AttackRange(float radius)
    {
        Bounds bounds = col.bounds;
        CenterPos = new Vector2(bounds.center.x, bounds.center.y);

        Collider2D[] attackRange = Physics2D.OverlapCircleAll(CenterPos, radius, TargetLayer);
        bool enemyDetected = false; // �� Ž�� ���� ����

        // ���� Ÿ�� �迭 �ʱ�ȭ
        ClearAttackTargets();

        int targetCount = 0; // Ž���� ���� ��

        foreach (Collider2D target in attackRange)
        {
            if (target.gameObject.activeSelf == false)
            {
                // ��Ȱ��ȭ�� Ÿ���� �迭���� ����
                RemoveTarget(target.gameObject);
            }

            // "Player"�� "PlayerTower"�� ���� Ÿ�ٸ� ���� ����Ʈ�� �߰�
            if ((target.CompareTag("Player") || target.CompareTag("PlayerTower")) && target.gameObject.activeSelf == true && targetCount < 2)
            {
                // Ÿ���� �迭�� ���� ���ٸ� �߰�
                AddTarget(target.gameObject);
                targetCount++; // Ž���� ���� �� ����
                enemyDetected = true; // ���� Ž����
            }
        }

        if (enemyDetected)
        {
            isMoved = false;
            if (!canAttack) // ���� ���°� �ƴ� ���� ���� ����
            {
                StartCoroutine(AttackTargets()); // ���� ����
            }
            rb.velocity = Vector2.zero;
            animator.SetFloat("Speed", 0);
        }
        else
        {
            // ���� Ž������ �ʾ��� ��� �ٽ� �̵� ����
            isMoved = true;
            canAttack = false;
        }
    }

    protected override void AddTarget(GameObject target)
    {
        // ������ �� �ִ� �ִ� 2���� Ÿ�ٸ� �߰�
        for (int i = 0; i < attackTarget.Length; i++)
        {
            if (attackTarget[i] == null)
            {
                attackTarget[i] = target;
                break;
            }
            else if (attackTarget[i].activeSelf == false)
            {
                // ���� Ÿ���̶�� ���� �ٲ���
                ClearAttackTargets();
                break;
            }
        }
    }

    protected override IEnumerator AttackTargets()
    {
        canAttack = true; // ���� ���� ���·� ����

        // ������ Ÿ���� ������ ����
        for (int i = 0; i < attackTarget.Length; i++)
        {
            if (attackTarget[i] != null && attackTarget[i].activeSelf == true)
            {
                Attack(attackTarget[i].GetComponent<Collider2D>());
            }
        }

        animator.SetTrigger("Attack"); // �ִϸ��̼� Ʈ���Ŵ� ���� �ٱ����� �� ���� ȣ��

        yield return new WaitForSeconds(attackDelay); // ���� ���� ���� �ð�

        canAttack = false; // ���� �Ϸ� �� ���� �Ұ��� ���·� ����
        isMoved = true; // ���� �� �ٽ� �̵� ����
    }

    protected override void Attack(Collider2D target)
    {
        if (target == null) return;

        if (target.CompareTag("Player"))
        {
            PlayerUnit playerUnit = target.GetComponent<PlayerUnit>(); // ���� ��ũ��Ʈ ����

            if (playerUnit != null)
            {
                playerUnit.TakeDamage(damage);
                Debug.Log($"{CharacterName}�� {playerUnit.name}���� {damage}�� ���ظ� �������ϴ�.");

                if (playerUnit.curHp <= 0)
                {
                    RemoveTarget(playerUnit.gameObject); // Ÿ�� ����
                    playerUnit.curHp = 0;
                }
            }
        }
        else if (target.CompareTag("PlayerTower"))
        {
            if (GameManager.Instance.playerTowerCurHp <= 0)
            {
                Debug.Log("�Ʊ��� ��ü�� �ı��Ǿ����ϴ�.");
                GameManager.Instance.isDefeat = true;
            }
            else
            {
                GameManager.Instance.playerTowerCurHp -= damage;
                Debug.Log("�Ʊ� Ÿ���� ���ݹް��ֽ��ϴ�.");
            }
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
        animator.SetTrigger("Dead");
        GameObject AcensionObj = Instantiate(GameManager.Instance.AcensionPrefab, transform.position, transform.rotation, null);

        transform.position = new Vector2(-100, -100);

        // ���� ���� ���� �ڷ�ƾ���� ��� ����
        StopAllCoroutines();
        // �ִϸ����� �Ķ���� �ʱ�ȭ
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
        // Ÿ���� �׾��� ��, ���� �������� �ش� Ÿ���� ����
        RemoveTarget(gameObject);
    }
}
