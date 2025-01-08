using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] public int life = 100;
    [SerializeField] float deathAnimationTime;
    [SerializeField] SpriteRenderer thisEnemySpriteRender;
    public GameObject FloatingTextPrefab;

    [SerializeField] private ParticleSystem damageParticle;
    [SerializeField] private Font fonte;
    [SerializeField] public AudioSource attackSoundEffect;
    
    [SerializeField] private Image healthBar;

    private void Awake()
    {
        attackSoundEffect = GetComponent<AudioSource>();
    }
    private void Update()
    {
        if(life <= 0)
        {
            Destroy(this.gameObject, deathAnimationTime);
            thisEnemySpriteRender.color = Color.red;
        }
    }

    public void takeDamage(int playerDamage)
    {
        life -= playerDamage;
        thisEnemySpriteRender.color = Color.red;
        StartCoroutine("ChangeColor");
        
        // trigger floating text
        ShowFloatingText(playerDamage);

        damageParticle.Play();
        healthBar.fillAmount = life / 100f;
    }
    private void ShowFloatingText(int lastPlayerDamage)
    {
        GameObject textDamage = Instantiate(FloatingTextPrefab, transform.position, Quaternion.identity);
        textDamage.GetComponent<TextMeshPro>().text = lastPlayerDamage.ToString();
    }

    private IEnumerator ChangeColor()
    {
        yield return new WaitForSeconds(0.2f);
        thisEnemySpriteRender.color = Color.white;
    }
}
