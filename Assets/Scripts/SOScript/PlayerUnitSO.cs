using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="PlayerUnit", menuName ="ScriptableObject/PlayerUnitData", order =0)]
public class PlayerUnitSO : ScriptableObject
{

    [Header("UnitData")]
    public string UnitName;
    public int UsedCost;
    public float UnitDamage;
    public float MaxHp;
    public float AttackDelay;
    public float MoveSpeed;
    public float AttackRadius;
    public float SpawnCoolTime;

    public GameObject UnitProjectile;

}
