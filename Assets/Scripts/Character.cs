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
    public GameObject[] attackTarget = new GameObject[2];   // 탐지된 타겟 (최대 2명)

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
        isMoved = true; // 초기 상태를 이동 가능으로 설정
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
        rb.velocity = new Vector2(-moveSpeed, rb.velocity.y); // 왼쪽으로 이동
    }

    protected virtual void AttackRange(float radius)
    {
        Bounds bounds = col.bounds;
        CenterPos = new Vector2(bounds.center.x, bounds.center.y);

        Collider2D[] attackRange = Physics2D.OverlapCircleAll(CenterPos, radius, TargetLayer);
        bool enemyDetected = false; // 적 탐지 여부 변수

        // attackTarget 배열 초기화
        ClearAttackTargets();

        int targetCount = 0; // 탐지된 적의 수

        foreach (Collider2D target in attackRange)
        {
            if(target.gameObject.activeSelf==false)
            {
                RemoveTarget(target.gameObject);
            }
            if ((target.CompareTag("Enemy") || target.CompareTag("EnemyTower")) && targetCount < 2)
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
                // 죽은 타겟이라면 새로 바꿔줌
                ClearAttackTargets();
                break;
            }
        }
    }

    protected virtual IEnumerator AttackTargets()
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

    protected virtual void Attack(Collider2D target)
    {
        if (target == null) return;

        if (target.CompareTag("Enemy"))
        {
            EnemyUnit enemy = target.GetComponent<EnemyUnit>(); // 적의 스크립트 참조

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log($"{CharacterName}가 {enemy.name}에게 {damage}의 피해를 입혔습니다.");

                if (enemy.curHp <= 0)
                {
                    RemoveTarget(enemy.gameObject); // 타겟 제거
                    enemy.curHp = 0;
                }
            }
        }
        else if (target.CompareTag("EnemyTower"))
        {
            if (GameManager.Instance.enemyTowerCurHp <= 0)
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
        ClearAttackTargets(); // 공격할 수 있는 타겟 리스트를 모두 초기화
        gameObject.tag = "Untagged";
        gameObject.layer = 0;
        GameObject AcensionObj = Instantiate(GameManager.Instance.AcensionPrefab, transform.position, transform.rotation);
        //animator.SetBool("Dead", isDead);
        //Destroy(gameObject, 0.5f); // 사망 애니메이션 실행
    }
}
