using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireLaser : MonoBehaviour
{
    [Header("LaserInfo")]
    [SerializeField] private float LaserCoolTime = 48;
    [SerializeField] private float LaserCurTime = 0;
    private bool isReloading = false; // �ڷ�ƾ ���� ������ Ȯ���ϴ� ����

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

        LaserCoolTime = 48;

        // ������ ���� ����
        m_lineRenderer.startWidth = 0.1f;
        m_lineRenderer.endWidth = 0.1f;

        m_lineRenderer.enabled = false;

        // ���� �ݶ��̴� ��������
        explosionCollider = ExplosionPos.GetComponent<BoxCollider2D>();
    }
    private void OnEnable()
    {
        isReloading = false;
        LaserCurTime = 0;
    }
    private void Update()
    {
        //������ ��Ȱ��ȭ ������ ��� ������ ����
        if (isReloading == false)
            StartCoroutine(ReloadingLaser());
    }
    private IEnumerator ReloadingLaser()
    {
        if (isReloading) yield break; // �̹� ���� ���̸� ����

        UiManager.Instance.TowerReloadParticle.SetActive(true);
        UiManager.Instance.RealodingParticle.Play();
        isReloading = true;
        GameManager.Instance.isReloadingDone = false;
        UiManager.Instance.ReadyText.SetActive(false);

        while (LaserCurTime < LaserCoolTime)
        {
            LaserCurTime += Time.deltaTime;
            UiManager.Instance.ReloadingImage.fillAmount = LaserCurTime / LaserCoolTime; // ��Ÿ�� �� ������Ʈ
            yield return null; // ���� �����ӱ��� ���
        }

        // ��Ÿ�� �Ϸ� ��
        GameManager.Instance.isReloadingDone = true;
        UiManager.Instance.RealodingParticle.Stop();
        UiManager.Instance.ReloadingDoneParticle.Play();
        UiManager.Instance.TowerReloadParticle.SetActive(false);
        UiManager.Instance.ReadyText.SetActive(true);
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
    public void LaserFire()
    {
        if (GameManager.Instance.isReloadingDone == true)
        {
            //������ �߻� ȿ���� ���
            AudioManager.Instance.PlayEffectSound(AudioManager.Instance.FireLaserClip);
            //�߻�
            UiManager.Instance.ReloadingDoneParticle.Stop();
            isReloading = false;
            GameManager.Instance.isReloadingDone = false;
            LaserCurTime = 0;   //��Ÿ�� �ʱ�ȭ
            UiManager.Instance.ReloadingImage.fillAmount = 0;
            UiManager.Instance.ReadyText.SetActive(false);
            Debug.Log("������ �߻�!!");
            StartCoroutine(ShootLaser());
        }
        else
        {
            UiManager.Instance.LaserNotice.GetComponent<Animator>().SetTrigger("LaserNoticeOn");
            Debug.Log("�������� ���� �ʾ� �߻翡 �����߽��ϴ�.");
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
