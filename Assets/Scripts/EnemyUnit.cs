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
        Gizmos.color = Color.blue; // ���� ���� ����
        Gizmos.DrawWireSphere(transform.position, attackRadius); // ���� ������ ������ ǥ��
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
        bool enemyDetected = false; // �� Ž�� ���� ����
        Targets.Clear(); // �� �����Ӹ��� Ÿ�� ����Ʈ �ʱ�ȭ
        foreach (Collider2D target in attackRange)
        {
            if (target.CompareTag("Player") || target.CompareTag("PlayerTower"))
            {
                AddTarget(target.gameObject);
                enemyDetected = true; // ���� Ž����
            }
        }
        if (enemyDetected)
        {
            isMoved = false;
            if (!canAttack) // ���� ���°� �ƴ� ���� ����
            {
                StartCoroutine(AttackTargets()); // �� ����
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
    //�� �߰����´� PlayerUnit�� ������ �������� Ž��
    //���ݷ������� �����ϴ�. �θ��� ���ݷ����� �״�� ���
    protected override void Attack(Collider2D target)
    {
        // ���� ���� 
        if (target != null && target.CompareTag("Player"))
        {
            PlayerUnit playerUnit = target.GetComponent<PlayerUnit>(); // ���� ��ũ��Ʈ ����
            if (playerUnit != null)
            {
                if (playerUnit.curHp <= 0)
                {
                    // ����Ʈ���� ����
                    Targets.Remove(playerUnit.gameObject);
                }
                playerUnit.TakeDamage(damage);
                Debug.Log(CharacterName + "�� " + playerUnit.name + "���� " + damage + "�� ���ظ� �������ϴ�.");
            }
        }
        else if(target != null && target.CompareTag("PlayerTower"))
        {
            if(GameManager.Instance.playerTowerCurHp <=0)
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
}
