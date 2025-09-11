using log4net;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Mobs : Character
{
    //��������� �����������
    private static readonly ILog log = LogManager.GetLogger(typeof(Mobs));
    // ������ ��� �������� ����� � ����������������
    [SerializeField] private MobStats mobStatsPrefab;
    // ���������� ��� �������������, ����� ��� ����� ���������� � ������ stats
    public MobStats mobStats => stats as MobStats; 



    private Rigidbody2D rb;
    private DropManager dropManagerScript;

    private Transform player; // ������ �� ������

    

    protected override void Awake()
    {
        // ������ ����� ��������� MobStats
        stats = Instantiate(mobStatsPrefab);

        if (mobStats != null)
        {
            log.Debug($"��� ��������������� � ID: {mobStats.IdMob} � �������� �� ������: {mobStats.DeathPrice}");
        }

        InitializeCharacteristics(EnemyTypeCode.UnitTest);
        RedistributeShotPointsInRange(270, 90, 8);
        base.Awake();
        mobStats.IsEnemy = true; // ��� �������� ������

        dropManagerScript = GetComponent<DropManager>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (player != null)
        {
            RotateTowardsPlayer();
            base.Shoot();
        }

        base.HandlingAppliedDoTEffects();
    }

    void RotateTowardsPlayer()
    {
        // ��������� ����������� � ������
        Vector2 direction = player.position - transform.position;
        direction.Normalize();

        // ��������� ���� ��������
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // ������� ������� �������
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

        // ������ ������������ ������
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, mobStats.RotateSpeed * Time.deltaTime);
    }

    public override void SetStat(string statName, float? value)
    {
        if (mobStats == null)
        {
            log.Debug("Stats �� ���������!");
            return;
        }

        if (value == null)
        {
            log.Debug($"�������� ��� {statName} �� �������!");
            return;
        }

        // �������� ������������� statName � enum CharacterStatCode
        if (Enum.TryParse<CharacterStatCode>(statName, ignoreCase: true, out CharacterStatCode statCode))
        {
            mobStats.SetStat(statName, value);
            ApplySpecialEffects(statName);
        }
    }

    //����� �������������� ���������, ������ ����� ����������
    protected override void ApplySpecialEffects(string statName)
    {
         CharacterStatCode characterCode = (CharacterStatCode)Enum.Parse(typeof(CharacterStatCode), statName);
        if (characterCode == CharacterStatCode.MoveSpeed)
        {
            EnemyTestAI mobsMoveScript = gameObject.GetComponent<EnemyTestAI>();
            mobsMoveScript.MoveSpeed = mobStats.MoveSpeed;
            log.Debug($"DropRadius �������: {mobStats.MoveSpeed}");
        }
    }


    // ������������� �������������
    private void InitializeCharacteristics(EnemyTypeCode enemyType)
    {
        var enemyData = EnemyTypesDictionary.GetCharacteristic(enemyType);
        float mobsMultiplier = GameGeneration.MobsMultipier;

        if (enemyData != null && mobStats != null)
        {
            // �������� ��� ������������� ���������� �� ItemEnemyTypesDictionary � ��������������� �������� public � ������������� �������, �.�. �� �����������
            var enemyFields = typeof(ItemEnemyTypesDictionary).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            foreach (var field in enemyFields)
            {
                if (field.PropertyType == typeof(int) || field.PropertyType == typeof(float))
                {
                    string fieldName = field.Name; // �������� ��� ����������
                    string statName = char.ToLower(fieldName[0]) + fieldName.Substring(1); // ������ ������� 1 ����� ����� �� ������

                    // �������� ��������������� ��� ���������� �� ������ Mob 
                    var mobField = typeof(MobStats).GetField(statName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                    if (mobField != null)
                    {
                        // ������������ �������� �� float
                        var value = Convert.ToSingle(field.GetValue(enemyData));
                        float? newValue = fieldName == "DeathPrice" ? value : (float)Math.Round(value * mobsMultiplier); // �������� ��������, ������� ����� �����������
                        mobStats.SetStat(fieldName, newValue); // ����������� ���������� ����� ��������
                    }
                }
            }

            for (int j = 0; j < enemyData.CountOfDots; j++)
            {
                ItemDot itemDot = DotsDictionary.GetRandomDot();
                mobStats.SetUsableDots((new UsableDotEffect(itemDot.Code, itemDot.BaseDotDmg, itemDot.BaseDotDuration)));
            }
        

            log.Debug($"InitializeCharacteristics. �������������� ���� ���������������� - DeathPrice: {mobStats.DeathPrice}");
        }
        else
        {
            if (enemyData == null)
            {
                log.Warn($"������ ����� ��� {enemyType} �� ������� � EnemyTypesDictionary.");
            }
            if (mobStats == null)
            {
                log.Warn("mobStats �� ���������������.");
            }
        }
    }


    protected override void TakeDamage(int damage, TypeOfDamage typeDamage)
    {
        log.Debug("TakeDamage. Die. idMob = " + mobStats.IdMob);
        base.TakeDamage(damage, typeDamage);
    }

    protected override void Die()
    {
        Destroy(GetComponent<BoxCollider2D>());
        if (mobStats.IsCanDie)
        {
            mobStats.IsCanDie = false;
            log.Debug("Die. idMob = " + mobStats.IdMob);
            dropManagerScript.DropLoss();
            Observer.IncrementCountKill(mobStats.DeathPrice);
            base.Die();
        }
        
    }
}
