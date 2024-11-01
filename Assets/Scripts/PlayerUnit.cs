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
        Gizmos.color = Color.red; // 원의 색상 설정
        Gizmos.DrawWireSphere(transform.position, attackRadius); // 공격 범위를 원으로 표시
    }
}
