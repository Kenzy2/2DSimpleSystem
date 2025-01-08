using System.Collections;
using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    [SerializeField] private float[] duringAttack;
    [SerializeField] private float comboCooldown = 0.2f;
    public Animator playerAnimator;
    [SerializeField] private int currentCombo = 1; 
    private bool isAttacking = false; 


    void Update()
    {
        OnAttack();
    }
    
    void OnAttack()
    {
        if(Input.GetButtonDown("Fire1") && !isAttacking && currentCombo == 1)
        {
            playerAnimator.SetBool(AnimatorNames.isAttackingOne, true);
            StartCoroutine(ResetAttack(AnimatorNames.isAttackingOne, 0));
            currentCombo++;
        }
        else if (Input.GetButtonDown("Fire1") && !isAttacking && currentCombo == 2)
        {
            playerAnimator.SetBool(AnimatorNames.isAttackingTwo, true);
            StartCoroutine(ResetAttack(AnimatorNames.isAttackingTwo, 1));
            currentCombo++;
        }
        else if (Input.GetButtonDown("Fire1") && !isAttacking && currentCombo == 3)
        {
            playerAnimator.SetBool(AnimatorNames.isAttackingThree, true);
            StartCoroutine(ResetAttack(AnimatorNames.isAttackingThree, 2));
            currentCombo++;
        }
        if(currentCombo > 3)
        {
            currentCombo = 1;
        }
    }
    IEnumerator ResetAttack(string animationForDisable, int currentAttack)
    {
        isAttacking = true;
        yield return new WaitForSeconds(duringAttack[currentAttack]);
        isAttacking = false;
        playerAnimator.SetBool(animationForDisable, false);
    }

    public class AnimatorNames
    {
        public const string isAttackingOne = "Attack";
        public const string isAttackingTwo = "AttackTwo";
        public const string isAttackingThree = "AttackThree";
    }
}
