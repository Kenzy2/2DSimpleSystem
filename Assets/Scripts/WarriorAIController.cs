using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum WarriorState
{
    Idle, Walk, Attack, Dead
}

public class WarriorAIController : MonoBehaviour
{
    [SerializeField] float walkSpeed;
    [SerializeField] float distanceDetect;
    [SerializeField] float distanceAttack = 3;
    [SerializeField] float timeToAttack;
    [SerializeField] float attackRange;

    [SerializeField] int warriorDamage = 35;

    [SerializeField] bool canAttack;

    [SerializeField] LayerMask wallLayer;
    [SerializeField] LayerMask playerLayer;

    [SerializeField] Vector2 direction;

    [SerializeField] Transform attackPoint;
    Transform playerTransform;

    private WarriorState warriorState;

    private Coroutine attackCooldownCoroutine;

    Animator warriorAnimator;

    private PlayerController playerController;
    private EnemyController _enemyController;

    [SerializeField] private AudioSource warriorAttack;
    [SerializeField] private AudioClip[] attacks;
    
    void Awake()
    {
        playerController = FindObjectOfType<PlayerController>();
        _enemyController = GetComponent<EnemyController>();
        warriorAnimator = GetComponent<Animator>();
        playerTransform = FindObjectOfType<PlayerController>().transform;
    }

    void Start()
    {
        warriorState = WarriorState.Walk;
    }
    private void Update()
    {
        IsNearToAttack();
        if (_enemyController.life <= 0)
        {
            warriorState = WarriorState.Dead;
        }
        switch (warriorState)
        {
            case WarriorState.Idle:
                WarriorIdle();
                break;
            case WarriorState.Walk:
                WarriorWalk();
                break;
            case WarriorState.Attack:
                WarriorAttack();
                break;
            case WarriorState.Dead:
                WarriorDead();
                break;
        }
    }
    void ChangeState(WarriorState state)
    {
        warriorState = state;
        warriorAnimator.SetBool("isRunning", state == WarriorState.Walk);
    }

    void WarriorDead()
    {
        warriorAnimator.Play("Death");
    }
    
    void WarriorIdle()
    {
        warriorAnimator.SetBool("isRunning", false);
    }
    void WarriorWalk()
    {
        warriorAnimator.SetBool("isRunning", true);
        if (IsFaceWall())
        {
            direction.x *= -1;
        }
        Vector2 playerDirection = (playerTransform.position - transform.position).normalized;
        direction = new Vector2(playerDirection.x, direction.y);
        ChangePlayerDirection(direction);
        transform.Translate(direction * (walkSpeed * Time.deltaTime));
    }
    public bool IsFaceWall()
    {
        return Physics2D.Raycast(transform.position, direction, distanceDetect, wallLayer);
    }

    public void IsNearToAttack()
    {
        if (Vector2.Distance(transform.position, playerTransform.transform.position) <= distanceAttack)
        {
            ChangeState(WarriorState.Attack);
        }
        else
        {
            if(canAttack)
                ChangeState(WarriorState.Walk);
            //warriorAnimator.SetBool("Attack", false);
        }
    }

    void WarriorAttack()
    {
        if (canAttack)
        {
            Vector2 dir = (playerTransform.position - transform.position).normalized;
            ChangePlayerDirection(dir);
            
            canAttack = false;
            warriorAnimator.SetBool("Attack", true);
            StartCoroutine(AttackCooldown());
        }
    }

    IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(timeToAttack);
        warriorAnimator.SetBool("Attack", false);
        canAttack = true;
    }

    void ChangePlayerDirection(Vector2 positionX)
    {
        if (positionX.x <= 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    void ActivateAttackTrigger()
    {
        var circleTrigger = Physics2D.OverlapCircle(attackPoint.position, attackRange, playerLayer);
        if (circleTrigger)
        {
            playerController.PlayerTakeDamage(warriorDamage);
            circleTrigger.GetComponent<PlayerController>();
            warriorAttack.PlayOneShot(attacks[1]);
        }
        else
        {
            warriorAttack.PlayOneShot(attacks[0]);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}