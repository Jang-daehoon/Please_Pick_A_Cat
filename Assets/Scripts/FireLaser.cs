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

    // �������� �浹�� ���̾�
    public LayerMask platformLayer;

    // �߻� ���� (�� ����)
    [SerializeField] private float angle = 60f;

    // ���� ������ �ݶ��̴�
    private BoxCollider2D explosionCollider;
    private Animator animator;
    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        // ������ ���� ����
        m_lineRenderer.startWidth = 0.1f;
        m_lineRenderer.endWidth = 0.1f;

        m_lineRenderer.enabled = false;

        // ���� �ݶ��̴� ��������
        explosionCollider = ExplosionPos.GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        //ShootLaser();
    }

    public IEnumerator ShootLaser()
    {
        m_lineRenderer.enabled = true;
        Debug.Log("������ �߻�!!");
        animator.SetTrigger("Fire");
        // �߻� ���� ���
        Vector2 direction = Quaternion.Euler(0, 0, angle) * -transform.right;

        // �������� Platform ���̾�� �浹�ϵ��� ����
        RaycastHit2D _hit = Physics2D.Raycast(laserFirePoint.position, direction, defDistanceRay, platformLayer);

        if (_hit)
        {
            Draw2DRay(laserFirePoint.position, _hit.point); // �浹 ����
            DetectEnemies(_hit.point); // �� Ž��
            GameObject ExplosionVfx = Instantiate(ExplosionParticle, _hit.point, Quaternion.identity, null);
            Destroy(ExplosionVfx, 2f);
            yield return new WaitForSeconds(0.5f);
            m_lineRenderer.enabled = false;
        }
        else
        {
            Draw2DRay(laserFirePoint.position, laserFirePoint.position + (-laserFirePoint.transform.right * defDistanceRay)); // ���� ����
        }
    }

    private void Draw2DRay(Vector2 startPos, Vector2 endPos)
    {
        m_lineRenderer.SetPosition(0, startPos);
        m_lineRenderer.SetPosition(1, endPos);
    }

    private void DetectEnemies(Vector2 explosionPoint)
    {
        // ���� ���� ���� �� Ž��
        if (explosionCollider != null)
        {
            Vector2 explosionSize = explosionCollider.size; // ���� ������ �ݶ��̴� ũ��
            Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(explosionPoint, explosionSize, 0f); // 0�� ȸ��

            foreach (Collider2D enemy in hitEnemies)
            {
                // ���� Ž���� ���
                if (enemy.CompareTag("Enemy")) // ���� �±װ� "Enemy"�� ���
                {
                    Debug.Log("�� Ž����: " + enemy.name);
                    // Ž���� ��� �� óġ
                    enemy.GetComponent<EnemyUnit>().Dead();
                
                }
            }
        }
        else
        {
            Debug.LogWarning("ExplosionPos�� BoxCollider2D�� �����ϴ�.");
        }
    }

    private void OnDrawGizmos()
    {
        // ���� ������ �ð������� ǥ��
        Gizmos.color = Color.red; // ���� ����
        Vector2 explosionCenter = ExplosionPos ? (Vector2)ExplosionPos.position : (Vector2)transform.position; // ���� �߽�
        if (explosionCollider != null)
        {
            Gizmos.DrawWireCube(explosionCenter, explosionCollider.size); // ���� ������ �ڽ� ���·� �׸���
        }
    }
}
