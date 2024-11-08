using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "EnemyUnit", menuName = "ScriptableObject/EnemyUnitData", order = 0)]
public class EnemyUnitSO : ScriptableObject
{
    [Header("UnitData")]
    public string unitName;
    public float unitDamage;
    public float maxHp;
    public float attackDelay;
    public float moveSpeed;
    public float spawnCoolTime;
    public float attackRadius;
    public float usedCost;  //소모되는 Cost

    public GameObject unitProjectile;
}
