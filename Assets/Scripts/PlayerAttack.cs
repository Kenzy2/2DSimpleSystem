using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using AudioClip = UnityEngine.AudioClip;

public class PlayerAttack : MonoBehaviour
{
    public PlayerController playerAttack;
    [SerializeField] float swordRadius;
    public LayerMask enemyLayers;
    [SerializeField] private TextMeshProUGUI comboDamageText;
    
    
    [SerializeField] private AudioSource swordSound;
    
    [SerializeField] private AudioClip[] attackClips;
    [SerializeField] private List<AudioClip> _audioClips = new List<AudioClip>();

    [SerializeField] private AudioClip airAttack;
    public void ActivateAttackTrigger()
    {
        var AttackingEnemy = Physics2D.OverlapCircle(transform.position, swordRadius, playerAttack.enemyLayer);
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
            comboDamageText.text = "Dano: " + playerAttack.playerDamage;
            AttackingEnemy.GetComponent<EnemyController>().takeDamage(playerAttack.playerDamage);
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