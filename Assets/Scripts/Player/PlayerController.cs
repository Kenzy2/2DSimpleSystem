using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // Movement variables
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rollSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float groundCheckHeight = 0.5f;  // Tamanho da cápsula
    [SerializeField] private float groundCheckWidth = 0.2f;   // Largura da cápsula
    [SerializeField] private float distanceDetect;

    [SerializeField] private int life;
    public int playerDamage = 35;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private Image healthBar;

    private bool hasJumped = false;
    private bool canCoyoteJump = false;
    public bool isPlayerAlive = true;
    [SerializeField] private float coyoteJumpDelay;

    //[SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] public Animator playerAnimator;
    bool isGrounded;

    // Combat variables
    [SerializeField] private float duringAttack = 0.05f;

    public LayerMask enemyLayer;
    public PlayerAttack triggerAttack;
    [SerializeField] private bool canSecondAttack = false;
    [SerializeField] private bool canThirdAttack;
    [SerializeField] private bool canRoll;

    // Wall detect variables
    public LayerMask wallLayer;
    [SerializeField] private Transform pointRaycast;

    private RaycastHit2D wallLeft;
    private RaycastHit2D wallRight;
    
    [SerializeField] AnimatorStateInfo stateinfo;

    private void Awake()
    {
        playerSprite = GetComponent<SpriteRenderer>();
        isPlayerAlive = true;
    }

    public class AnimatorStateInfo
    {
        public const string playerIdleToWalking = "Idle To Walking";
        public const string playerAttack = "Attack";
        public const string playerJump = "Jumping";
        public const string playerDeath = "HeroKnight_Death";
        public const string playerRoll = "PlayerRoll";
        public const string playerAttackTwo = "AttackTwo";
        public const string playerAttackThree = "AttackThree";
    }
    
    void Update() 
    {
        // Desenhando o OverlapCircle
        DrawCapsule(groundCheck.position, groundCheckWidth, groundCheckHeight); // Visualiza a área de verificação
        //OnAttack();
        wallCheck();
    }

    private void PlayerLifeCheck()
    {
        if(life <= 0)
        {
            playerAnimator.Play(AnimatorStateInfo.playerDeath);
            StartCoroutine(PlayerDeath());
        }
    }

    private IEnumerator PlayerDeath()
    {
        yield return new WaitForSeconds(1f);
        isPlayerAlive = false;
    }

    public void PlayerTakeDamage(int enemyDamage)
    {
        life -= enemyDamage;
        playerSprite.color = Color.red;
        StartCoroutine(ChangePlayerColor());
        healthBar.fillAmount = life / 100f;
        PlayerLifeCheck();
    }

    IEnumerator ChangePlayerColor()
    {
        yield return new WaitForSeconds(0.3f);
        playerSprite.color = Color.white;
    }
    //Cronometro para desativar a animação e o bonus de speed do rolamento
    private IEnumerator PlayerRollDisable()
    {
        yield return new WaitForSeconds(0.6f);
        playerAnimator.SetBool(AnimatorStateInfo.playerRoll, false);
        moveSpeed -= rollSpeed;
        canRoll = true;
    }
    
    void ApplyJumpForce()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        canCoyoteJump = false;
    }
    // Comando manipulavel via método para evitar repetição (Altera a direção e o estado de movimento do player)
    void ChangePlayerStateMovement(int playerDirection, bool isIdle)
    {
        playerAnimator.SetBool(AnimatorStateInfo.playerIdleToWalking, isIdle);
        transform.localScale = new Vector3(playerDirection,1,1);
    }

    public int IncresePlayerDamage(int a, int b)
    {
        int playerDamageCritical = Random.Range(a, b);
        return playerDamageCritical;
    }
    
    // método para ser chamado no animator e possibilitar o ataque numero 2    
    public void CanComboOne() 
    {
        canSecondAttack = true;
    }

    // método para ser chamado no animator e possibilitar o ataque numero 3
    public void CanComboTwo()
    {
        canThirdAttack = true;
    }

    // método para ser chamado no animator e impossibilitar o player de combar sem atacar na hora certa
    public void CantCombo()
    {
        canSecondAttack = false;
        canThirdAttack = false;
        playerDamage = 10;
    }

    // ativando o trigger para colidir com inimigos e causar dano
    public void playerAttackTrigger()
    {
        triggerAttack.ActivateAttackTrigger();
    }
    
    // adicionando o tempo que o player poderá dar o salto "coyote"
    private IEnumerator StartCoyoteTime()
    {
        yield return new WaitForSeconds(coyoteJumpDelay);
        canCoyoteJump = false;
    }

    //cronometro para desativar os efeitos do primeiro ataque
    private IEnumerator AttackTime()
    {
        yield return new WaitForSeconds(duringAttack);
        playerAnimator.SetBool(AnimatorStateInfo.playerAttack, false);
        canSecondAttack = false;
        canThirdAttack = false;
    }

    //cronometro para encerrar os efeitos do segundo ataque
    private IEnumerator SecondAttackTime()
    {
        playerDamage = IncresePlayerDamage(30, 35);
        yield return new WaitForSeconds(.5f);
        playerAnimator.SetBool(AnimatorStateInfo.playerAttackTwo, false);
    }

    // cronometro para encerrar os efeitos do terceiro ataque
    private IEnumerator ThirdAttackTime()
    {
        yield return new WaitForSeconds(.7f);
        playerAnimator.SetBool(AnimatorStateInfo.playerAttackThree, false);
        canThirdAttack = false;
        playerDamage = 10;
    }

    // Função para desenhar um círculo (útil para mostrar o raio do OverlapCircle)
    void DrawCapsule(Vector2 position, float width, float height)
    {
        float angleStep = 360f / 30;  // Resolução da cápsula (quantos segmentos)
        for (int i = 0; i < 30; i++)
        {
            float angle1 = i * angleStep * Mathf.Deg2Rad;
            float angle2 = (i + 1) * angleStep * Mathf.Deg2Rad;

            // Calculando as posições dos pontos para desenhar a cápsula
            Vector2 point1 = position + new Vector2(Mathf.Cos(angle1) * width, Mathf.Sin(angle1) * height);
            Vector2 point2 = position + new Vector2(Mathf.Cos(angle2) * width, Mathf.Sin(angle2) * height);

            // Desenhando as linhas que formam a cápsula
            Debug.DrawLine(point1, point2, Color.green);
        }
    }
    //soltando raycast horizontalmente para detectar paredes
    private void wallCheck()
    {
        wallLeft = Physics2D.Raycast(pointRaycast.position, Vector2.left, distanceDetect, wallLayer);
        wallRight = Physics2D.Raycast(pointRaycast.position, Vector2.right, distanceDetect, wallLayer);
    }

    //desenhando as linhas de detecção de parede
    private void OnDrawGizmosSelected()
    {
        Debug.DrawRay(pointRaycast.position, Vector2.left * distanceDetect, Color.red);
        Debug.DrawRay(pointRaycast.position, Vector2.right * distanceDetect, Color.green);
    }
}
