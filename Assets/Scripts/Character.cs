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
    public bool isMoved; // 이동 탐지
    public bool canAttack; // 공격 가능 여부

    public List<GameObject> Targets = new List<GameObject>(); // 탐지된 적 저장 리스트

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
        isMoved = true; // 초기 상태를 이동 가능으로 설정
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
        rb.velocity = new Vector2(-moveSpeed, rb.velocity.y); // 왼쪽으로 이동
    }

    protected virtual void AttackRange(float radius)
    {
        Bounds bounds = col.bounds;
        CenterPos = new Vector2(bounds.center.x, bounds.center.y);

        Collider2D[] attackRange = Physics2D.OverlapCircleAll(CenterPos, radius, TargetLayer);
        bool enemyDetected = false; // 적 탐지 여부 변수

        Targets.Clear(); // 매 프레임마다 타겟 리스트 초기화

        foreach (Collider2D target in attackRange)
        {
            if (target.CompareTag("Enemy") || target.CompareTag("EnemyTower"))
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

    protected virtual void AddTarget(GameObject target)
    {
        // 이미 추가된 적인지 확인
        if (!Targets.Contains(target))
        {
            Targets.Add(target);
        }
    }

    protected virtual IEnumerator AttackTargets()
    {
        canAttack = true; // 공격 가능 상태로 변경
        // 최대 2명의 적에게 공격
        for (int i = 0; i < Mathf.Min(2, Targets.Count); i++)
        {
            Attack(Targets[i].GetComponent<Collider2D>());
            animator.SetTrigger("Attack");
            yield return new WaitForSeconds(attackDelay); // 공격 간의 지연 시간
        }
        canAttack = false; // 공격 완료 후 공격 불가능 상태로 변경
        isMoved = true; // 공격 후 다시 이동 가능
    }

    protected virtual void Attack(Collider2D target)
    {
        // 공격 로직
        if (target != null && target.CompareTag("Enemy"))
        {
            EnemyUnit enemy = target.GetComponent<EnemyUnit>(); // 적의 스크립트 참조
            
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log(CharacterName + "가 " + enemy.name + "에게 " + damage + "의 피해를 입혔습니다.");

                if (enemy.curHp <= 0)
                {
                    // 리스트에서 제거
                    Targets.Remove(enemy.gameObject);
                    enemy.curHp = 0;
                }
            }
        }
        else if(target != null && target.CompareTag("EnemyTower"))
        { 
            if(GameManager.Instance.enemyTowerCurHp <= 0)
            {
                Debug.Log("적의 성체를 파괴했습니다.");
                GameManager.Instance.isVictory = true;
            }
            else
            {
                GameManager.Instance.enemyTowerCurHp -= damage;
                Debug.Log("적의 성체를 공격중입니다.");
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
        // 사망 애니메이션 실행
    }
}
