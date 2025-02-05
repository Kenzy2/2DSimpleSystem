using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCombatController : MonoBehaviour
{

    [SerializeField] private float[] duringAttack;
    [SerializeField] private int[] bonusDamage;

    public Animator playerAnimator;
    [SerializeField] private PlayerMovement playerMovement;

    [SerializeField] public bool isAttacking = false;
    [SerializeField] public bool canAttack = true;
    [SerializeField] public bool isPlayerAlive = true;


    [SerializeField] public int playerLife = 100;
    [SerializeField] public int playerDamage;
    [SerializeField] int currentCombo = 1;

    [SerializeField] private Image playerHealthBar;
    //[SerializeField] private TextMeshProUGUI enemyDamageText;
    [SerializeField] private GameObject damagePrefab;

    void Update()
    {
        OnAttack();
        if (isAttacking)
        {
            playerMovement.canMovement = false;
            if (playerMovement.isJumping)
            {
                playerMovement.canMovement = true;
                playerAnimator.SetBool(AnimatorNames.isWalking, false);
            }
            else
            {
                playerAnimator.SetBool(AnimatorNames.isWalking, false);
            }
        }
        else
        {
            playerMovement.canMovement = true;
            //playerAnimator.SetBool(AnimatorNames.isWalking, true);
        }

        playerHealthBar.fillAmount = playerLife / 100f;
    }

    public void PlayerTakeDamage(int damage)
    {
        if(!playerAnimator.GetBool(AnimatorNames.playerBlock))
        {
            ShowFloatingText(damage);
            playerLife -= damage;
        }
        if(playerLife <= 0)
        {
            playerMovement.canMovement = false;
            playerAnimator.Play("HeroKnight_Death");
            StartCoroutine(PlayerDeath());  
        }
        playerMovement.PlayerKnockback();
    }

    void ShowFloatingText(int lastEnemyDamage)
    {
        GameObject enemyDamageTextPrefab = Instantiate(damagePrefab, transform.position, Quaternion.identity);
        enemyDamageTextPrefab.GetComponent<TextMeshPro>().text = lastEnemyDamage.ToString();
    }

    void OnAttack()
    {
        if (Input.GetButtonDown("Fire1") && !isAttacking && currentCombo == 1 && canAttack)
        {
            canAttack = false;
            SetPlayerAttack(AnimatorNames.isAttackingOne, 0, bonusDamage[0]);
            playerDamage = 10;
        }
        else if (Input.GetButtonDown("Fire1") && !isAttacking && currentCombo == 2 && canAttack)
        {
            canAttack = false;
            SetPlayerAttack(AnimatorNames.isAttackingTwo, 0, bonusDamage[1]);
        }
        else if (Input.GetButtonDown("Fire1") && !isAttacking && currentCombo == 3 && canAttack)
        {
            canAttack = false;
            SetPlayerAttack(AnimatorNames.isAttackingThree, 0, bonusDamage[2]);
        }
        if (currentCombo > 3)
        {
            currentCombo = 1;
        }
        playerMovement.canMovement = false;
    }

    void SetPlayerAttack(string animName, int currentAttack, int playerBonusDamage)
    {
        playerDamage += UnityEngine.Random.Range(playerDamage, playerBonusDamage);
        playerAnimator.SetTrigger(animName);
        StartCoroutine(ResetAttack(animName, currentAttack));
        canAttack = true;
        currentCombo++;
        playerMovement.canMovement = true;
    }

    IEnumerator ResetAttack(string animationForDisable, int currentAttack)
    {
        isAttacking = true;
        yield return new WaitForSeconds(duringAttack[currentAttack]);
        isAttacking = false;
        //playerAnimator.SetTrigger(animationForDisable);
    }

    IEnumerator PlayerDeath()
    {
        yield return new WaitForSeconds(.9f);
        isPlayerAlive = false;
    }

    public class AnimatorNames
    {
        public const string isAttackingOne = "Attack";
        public const string isAttackingTwo = "AttackTwo";
        public const string isAttackingThree = "AttackThree";
        public const string isWalking = "playerWalk";
        public const string playerBlock = "playerBlock";
    }
}