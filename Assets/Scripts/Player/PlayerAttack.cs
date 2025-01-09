using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using AudioClip = UnityEngine.AudioClip;

public class PlayerAttack : MonoBehaviour
{
    public PlayerCombatController playerCombatController;
    [SerializeField] float swordRadius;
    public LayerMask enemyLayers;
    [SerializeField] private TextMeshProUGUI comboDamageText;
    
    
    [SerializeField] private AudioSource swordSound;
    
    [SerializeField] private AudioClip[] attackClips;
    [SerializeField] private List<AudioClip> _audioClips = new List<AudioClip>();

    [SerializeField] private AudioClip airAttack;

    public void ActivateAttackTrigger()
    {
        var AttackingEnemy = Physics2D.OverlapCircle(transform.position, swordRadius, enemyLayers);
        if (AttackingEnemy)
        {
            if (_audioClips.Count == 0)
            {
                _audioClips.AddRange(attackClips);
            }
            if (_audioClips.Count > 0)
            {
                swordSound.clip = _audioClips[0];
                swordSound.Play();
                _audioClips.RemoveAt(0);
            }
            comboDamageText.text = "Dano: " + playerCombatController.playerDamage;
            AttackingEnemy.GetComponent<EnemyController>().takeDamage(playerCombatController.playerDamage);
        }
        else
        {
            swordSound.PlayOneShot(airAttack);
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, swordRadius);
    }
}