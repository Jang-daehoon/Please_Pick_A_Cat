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

        curHp = maxHp;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red; // ���� ���� ����
        Gizmos.DrawWireSphere(transform.position, attackRadius); // ���� ������ ������ ǥ��
    }
}
