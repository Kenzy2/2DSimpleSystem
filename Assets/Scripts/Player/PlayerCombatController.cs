using System.Collections;
using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{

    [SerializeField] private float[] duringAttack;

    public Animator playerAnimator;
    [SerializeField] private PlayerMovement playerMovement;

    [SerializeField] private int currentCombo = 1;

    [SerializeField] public bool isAttacking = false;
    [SerializeField] public bool canAttack = true;


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
                //playerMovement.moveSpeed = 4;
            }
            else
            {
                playerAnimator.SetBool(AnimatorNames.isWalking, false);
                //playerMovement.moveSpeed *= 0;
            }
        }
        else
        {
            playerMovement.canMovement = true;
            playerAnimator.SetBool(AnimatorNames.isWalking, true);
            //playerMovement.moveSpeed = 4;
        }
    }

    void OnAttack()
    {
        if (Input.GetButtonDown("Fire1") && !isAttacking && currentCombo == 1 && canAttack)
        {
            canAttack = false;
            SetPlayerAttack(AnimatorNames.isAttackingOne, 0);
        }
        else if (Input.GetButtonDown("Fire1") && !isAttacking && currentCombo == 2 && canAttack)
        {
            canAttack = false;
            SetPlayerAttack(AnimatorNames.isAttackingTwo, 1);
        }
        else if (Input.GetButtonDown("Fire1") && !isAttacking && currentCombo == 3 && canAttack)
        {
            canAttack = false;
            SetPlayerAttack(AnimatorNames.isAttackingThree, 2);
        }
        if (currentCombo > 3)
        {
            currentCombo = 1;
        }
        playerMovement.canMovement = false;
    }

    void SetPlayerAttack(string animName, int currentAttack)
    {
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

    public class AnimatorNames
    {
        public const string isAttackingOne = "Attack";
        public const string isAttackingTwo = "AttackTwo";
        public const string isAttackingThree = "AttackThree";
        public const string isWalking = "playerWalk";
    }
}