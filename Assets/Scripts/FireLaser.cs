using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireLaser : MonoBehaviour
{
    [SerializeField] private float defDistanceRay = 100f;
    public Transform laserFirePoint;
    public LineRenderer m_lineRenderer;
    public Transform ExplosionPos;

    public GameObject ExplosionParticle;

    // 레이저가 충돌할 레이어
    public LayerMask platformLayer;

    // 발사 각도 (도 단위)
    [SerializeField] private float angle = 60f;

    // 폭발 범위의 콜라이더
    private BoxCollider2D explosionCollider;
    private Animator animator;
    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        // 라인의 굵기 설정
        m_lineRenderer.startWidth = 0.1f;
        m_lineRenderer.endWidth = 0.1f;

        m_lineRenderer.enabled = false;

        // 폭발 콜라이더 가져오기
        explosionCollider = ExplosionPos.GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        //ShootLaser();
    }

    public IEnumerator ShootLaser()
    {
        m_lineRenderer.enabled = true;
        Debug.Log("레이저 발사!!");
        animator.SetTrigger("Fire");
        // 발사 방향 계산
        Vector2 direction = Quaternion.Euler(0, 0, angle) * -transform.right;

        // 레이저가 Platform 레이어에만 충돌하도록 설정
        RaycastHit2D _hit = Physics2D.Raycast(laserFirePoint.position, direction, defDistanceRay, platformLayer);

        if (_hit)
        {
            Draw2DRay(laserFirePoint.position, _hit.point); // 충돌 지점
            DetectEnemies(_hit.point); // 적 탐지
            GameObject ExplosionVfx = Instantiate(ExplosionParticle, _hit.point, Quaternion.identity, null);
            Destroy(ExplosionVfx, 2f);
            yield return new WaitForSeconds(0.5f);
            m_lineRenderer.enabled = false;
        }
        else
        {
            Draw2DRay(laserFirePoint.position, laserFirePoint.position + (-laserFirePoint.transform.right * defDistanceRay)); // 왼쪽 끝점
        }
    }

    private void Draw2DRay(Vector2 startPos, Vector2 endPos)
    {
        m_lineRenderer.SetPosition(0, startPos);
        m_lineRenderer.SetPosition(1, endPos);
    }

    private void DetectEnemies(Vector2 explosionPoint)
    {
        // 폭발 범위 내의 적 탐지
        if (explosionCollider != null)
        {
            Vector2 explosionSize = explosionCollider.size; // 폭발 범위의 콜라이더 크기
            Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(explosionPoint, explosionSize, 0f); // 0도 회전

            foreach (Collider2D enemy in hitEnemies)
            {
                // 적을 탐지한 경우
                if (enemy.CompareTag("Enemy")) // 적의 태그가 "Enemy"인 경우
                {
                    Debug.Log("적 탐지됨: " + enemy.name);
                    // 탐지된 모든 적 처치
                    enemy.GetComponent<EnemyUnit>().Dead();
                
                }
            }
        }
        else
        {
            Debug.LogWarning("ExplosionPos에 BoxCollider2D가 없습니다.");
        }
    }

    private void OnDrawGizmos()
    {
        // 폭발 범위를 시각적으로 표시
        Gizmos.color = Color.red; // 색상 설정
        Vector2 explosionCenter = ExplosionPos ? (Vector2)ExplosionPos.position : (Vector2)transform.position; // 폭발 중심
        if (explosionCollider != null)
        {
            Gizmos.DrawWireCube(explosionCenter, explosionCollider.size); // 폭발 범위를 박스 형태로 그리기
        }
    }
}
