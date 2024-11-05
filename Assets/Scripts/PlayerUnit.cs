using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : Character
{
    [SerializeField] PlayerUnitSO playerUnitSo;
    private void OnEnable()
    {
        Unitinit();
    }
    private void Unitinit()
    {
        CharacterName = playerUnitSo.UnitName;
        Cost = playerUnitSo.UsedCost;
        damage = playerUnitSo.UnitDamage;
        maxHp = playerUnitSo.MaxHp;
        attackDelay = playerUnitSo.AttackDelay;
        moveSpeed = playerUnitSo.MoveSpeed;
        attackRadius = playerUnitSo.AttackRadius;
        spawnCoolTime = playerUnitSo.SpawnCoolTime;

        isDead = false;
        curHp = maxHp;
        gameObject.tag = "Player";
        animator.SetBool("Dead", isDead);
        gameObject.layer = 3;

    }
    public override void Dead()
    {
        isDead = true;

        if (curHp < 0)
        {
            curHp = 0;
        }
        gameObject.tag = "Untagged";
        gameObject.layer = 0;
        GameObject AcensionObj = Instantiate(GameManager.Instance.AcensionPrefab, transform.position, transform.rotation, null);
        animator.SetTrigger("Dead");
        //PlayerUnitPool.pool.Push(this, 0.5f); //������ƮǮ��
        Destroy(gameObject, 0.5f);

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red; // ���� ���� ����
        Gizmos.DrawWireSphere(transform.position, attackRadius); // ���� ������ ������ ǥ��
    }
}
