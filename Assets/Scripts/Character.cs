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

    public List<GameObject> Targets = new List<GameObject>(); // Ž���� �� ���� ����Ʈ

    public LayerMask TargetLayer;
    protected Vector2 CenterPos;

    protected Rigidbody2D rb;
    protected Collider2D col;
    protected Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
    }

    protected void Start()
    {
        isMoved = true; // �ʱ� ���¸� �̵� �������� ����
    }

    protected void Update()
    {
        if (isMoved && !canAttack)
        {
            Move();
        }
        AttackRange(attackRadius);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("GameCleaner"))
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

        Targets.Clear(); // �� �����Ӹ��� Ÿ�� ����Ʈ �ʱ�ȭ

        foreach (Collider2D target in attackRange)
        {
            if (target.CompareTag("Enemy") || target.CompareTag("EnemyTower"))
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

    protected virtual void AddTarget(GameObject target)
    {
        // �̹� �߰��� ������ Ȯ��
        if (!Targets.Contains(target))
        {
            Targets.Add(target);
        }
    }

    protected virtual IEnumerator AttackTargets()
    {
        canAttack = true; // ���� ���� ���·� ����
        // �ִ� 2���� ������ ����
        for (int i = 0; i < Mathf.Min(2, Targets.Count); i++)
        {
            Attack(Targets[i].GetComponent<Collider2D>());
            animator.SetTrigger("Attack");
            yield return new WaitForSeconds(attackDelay); // ���� ���� ���� �ð�
        }
        canAttack = false; // ���� �Ϸ� �� ���� �Ұ��� ���·� ����
        isMoved = true; // ���� �� �ٽ� �̵� ����
    }

    protected virtual void Attack(Collider2D target)
    {
        // ���� ����
        if (target != null && target.CompareTag("Enemy"))
        {
            EnemyUnit enemy = target.GetComponent<EnemyUnit>(); // ���� ��ũ��Ʈ ����
            
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log(CharacterName + "�� " + enemy.name + "���� " + damage + "�� ���ظ� �������ϴ�.");

                if (enemy.curHp <= 0)
                {
                    // ����Ʈ���� ����
                    Targets.Remove(enemy.gameObject);
                    enemy.curHp = 0;
                }
            }
        }
        else if(target != null && target.CompareTag("EnemyTower"))
        { 
            if(GameManager.Instance.enemyTowerCurHp <= 0)
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

    public void TakeDamage(float damage)
    {
        if (curHp > 0)
        {
            animator.SetTrigger("Hit");
            curHp -= damage;
        }
        else
        {
            Dead();
        }
    }

    public virtual void Dead()
    {
        isDead = true;

        if (curHp < 0)
        {
            curHp = 0;
        }
        Targets.Clear();
        gameObject.tag = "Untagged";
        gameObject.layer = 0;
        GameObject AcensionObj = Instantiate(GameManager.Instance.AcensionPrefab, transform.position, transform.rotation, null);
        animator.SetBool("Dead", isDead);
        Destroy(gameObject, 0.5f);
        // ��� �ִϸ��̼� ����
    }
}
