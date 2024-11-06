using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public string CharacterName;
    public float maxHp;
    public float curHp;
    public float damage;
    public float Cost;
    public float attackDelay;
    public float moveSpeed;
    public float attackRadius;
    public float spawnCoolTime;
    public bool isDead;
    public bool isMoved; // �̵� Ž��
    public bool canAttack; // ���� ���� ����
    public GameObject[] attackTarget = new GameObject[2];   // Ž���� Ÿ�� (�ִ� 2��)

    public LayerMask TargetLayer;
    protected Vector2 CenterPos;

    protected Rigidbody2D rb;
    protected Collider2D col;
    protected Animator animator;
    [HideInInspector]public SpriteRenderer spriteRenderer;
    public GameObject DeadParticle;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    protected void Start()
    {
        isMoved = true; // �ʱ� ���¸� �̵� �������� ����
    }

    protected void Update()
    {
        if (isDead) return;

        if (isMoved && !canAttack)
        {
            Move();
        }

        AttackRange(attackRadius);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("GameCleaner"))
        {
            Dead();
        }
    }

    protected virtual void Move()
    {
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        rb.velocity = new Vector2(-moveSpeed, rb.velocity.y); // �������� �̵�
    }

    protected virtual void AttackRange(float radius)
    {
        Bounds bounds = col.bounds;
        CenterPos = new Vector2(bounds.center.x, bounds.center.y);

        Collider2D[] attackRange = Physics2D.OverlapCircleAll(CenterPos, radius, TargetLayer);
        bool enemyDetected = false; // �� Ž�� ���� ����

        // attackTarget �迭 �ʱ�ȭ
        ClearAttackTargets();

        int targetCount = 0; // Ž���� ���� ��

        foreach (Collider2D target in attackRange)
        {
            if(target.gameObject.activeSelf==false)
            {
                RemoveTarget(target.gameObject);
            }
            if ((target.CompareTag("Enemy") || target.CompareTag("EnemyTower")) && targetCount < 2)
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

    public virtual void ClearAttackTargets()
    {
        for (int i = 0; i < attackTarget.Length; i++)
        {
            attackTarget[i] = null;
        }
    }

    protected virtual void AddTarget(GameObject target)
    {
        for (int i = 0; i < attackTarget.Length; i++)
        {
            if (attackTarget[i] == null)
            {
                attackTarget[i] = target;
                break;
            }
            else if(attackTarget[i].activeSelf == false)
            {
                // ���� Ÿ���̶�� ���� �ٲ���
                ClearAttackTargets();
                break;
            }
        }
    }

    protected virtual IEnumerator AttackTargets()
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

    protected virtual void Attack(Collider2D target)
    {
        if (target == null) return;

        if (target.CompareTag("Enemy"))
        {
            EnemyUnit enemy = target.GetComponent<EnemyUnit>(); // ���� ��ũ��Ʈ ����

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log($"{CharacterName}�� {enemy.name}���� {damage}�� ���ظ� �������ϴ�.");

                if (enemy.curHp <= 0)
                {
                    RemoveTarget(enemy.gameObject); // Ÿ�� ����
                    enemy.curHp = 0;
                }
            }
        }
        else if (target.CompareTag("EnemyTower"))
        {
            if (GameManager.Instance.enemyTowerCurHp <= 0)
            {
                Debug.Log("���� ��ü�� �ı��߽��ϴ�.");
                GameManager.Instance.isVictory = true;
            }
            else
            {
                GameManager.Instance.enemyTowerCurHp -= damage;
                Debug.Log("���� ��ü�� �������Դϴ�.");
            }
        }
    }

    protected void RemoveTarget(GameObject target)
    {
        for (int i = 0; i < attackTarget.Length; i++)
        {
            if (attackTarget[i] == target)
            {
                attackTarget[i] = null;
                break;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        animator.SetTrigger("Hit");
        curHp -= damage;

        if (curHp <= 0)
        {
            Dead();
        }
    }

    public virtual void Dead()
    {
        isDead = true;
        ClearAttackTargets(); // ������ �� �ִ� Ÿ�� ����Ʈ�� ��� �ʱ�ȭ
        gameObject.tag = "Untagged";
        gameObject.layer = 0;
        GameObject AcensionObj = Instantiate(GameManager.Instance.AcensionPrefab, transform.position, transform.rotation);
        //animator.SetBool("Dead", isDead);
        //Destroy(gameObject, 0.5f); // ��� �ִϸ��̼� ����
    }
}
