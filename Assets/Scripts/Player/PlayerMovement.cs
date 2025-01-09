using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private PlayerCombatController playerCombatController;
    [SerializeField] private WarriorAIController warriorAIController;

    public bool isJumping = false;
    public bool canRoll = true;
    public bool canMovement = true;
    public bool isRight = true;
    public bool isKnockbackActive = false;

    [SerializeField] public float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float rollTimer;
    [SerializeField] private float speedMultiplier;
    [SerializeField] private float knockbackForce;
    [SerializeField] private float knockbackTimer;
    [SerializeField] private float knockbackDuration;
    public float moveX;

    void Update()
    {
        PlayerMovementSystem();
        PlayerJump();
    }

    void PlayerMovementSystem()
    {
        if (isKnockbackActive)
        {
            knockbackTimer -= Time.deltaTime;
            if(knockbackTimer <= 0)
            {
                isKnockbackActive = false;
                canMovement = true;
            }
            return;
        }
        if (!canMovement) // Impede o movimento
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            SetPlayerState(PlayerStatement.playerWalk, false);
            return;
        }
        moveX = Input.GetAxisRaw("Horizontal");

        if (!playerCombatController.isAttacking && canMovement && !isKnockbackActive)
        {
            rb.velocity = new Vector2(moveX * moveSpeed, rb.velocity.y);
            if (moveX > 0)
            {
                playerTransform.localScale = new Vector3(moveX, 1, 1);
                SetPlayerState(PlayerStatement.playerWalk, true);
                PlayerRoll();
                isRight = true;
            }
            else if (moveX < 0)
            {
                playerTransform.localScale = new Vector3(moveX, 1, 1);
                SetPlayerState(PlayerStatement.playerWalk, true);
                PlayerRoll();
                isRight = false;
            }
            else
            {
                SetPlayerState(PlayerStatement.playerWalk, false);
            }
        }
        else
        {
            canRoll = false;
        }
    }

    public void PlayerKnockback()
    {
        isKnockbackActive = true;
        canMovement = false;
        knockbackTimer = knockbackDuration;
        if(warriorAIController.isRight)
            rb.AddForce(Vector2.right * knockbackForce, ForceMode2D.Impulse);
        else 
            rb.AddForce(Vector2.left * knockbackForce, ForceMode2D.Impulse);
    }

    void PlayerJump()
    {
        if (Input.GetButtonDown("Jump") && !isJumping)
        {
            isJumping = true;
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            SetPlayerState(PlayerStatement.playerJump, true);
        }
    }

    bool PlayerRoll()
    {
        if (!isJumping && canRoll && Input.GetButtonDown("Fire3"))
        {
            playerCombatController.canAttack = false;
            moveSpeed *= speedMultiplier;
            canRoll = false;
            SetPlayerState(PlayerStatement.playerRoll, true);
            StartCoroutine(SetPlayerRoll());
            return true;
        }
        return false;
    }

    private IEnumerator SetPlayerRoll()
    {
        yield return new WaitForSeconds(rollTimer);
        SetPlayerState(PlayerStatement.playerRoll, false);
        moveSpeed /= speedMultiplier;
        playerCombatController.canAttack = true;
        canRoll = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            SetPlayerState(PlayerStatement.playerJump, false);
            isJumping = false;
            if (playerCombatController.canAttack) 
            {
                canRoll = true;
            }
        }
    }

    void SetPlayerState(string animName, bool setStatement)
    {
        playerAnimator.SetBool(animName, setStatement);
    }

    public class PlayerStatement
    {
        public const string playerWalk = "playerWalk";
        public const string playerRoll = "playerRoll";
        public const string playerJump = "playerJump";
    }
}
