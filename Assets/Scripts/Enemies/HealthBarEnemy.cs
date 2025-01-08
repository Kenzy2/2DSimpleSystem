using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HealthBarEnemy : MonoBehaviour
{
    public Transform enemy;
    public RectTransform healthBar;
    public Vector3 offset;

    [SerializeField] private GameObject enemyInstance;
    // Update is called once per frame
    void Start()
    {
        
    }
    void Update()
    {
        if (!enemy.IsDestroyed())
        {
            Vector3 pos = Camera.main.WorldToScreenPoint(enemy.position + offset);
            healthBar.position = pos;
        }
    }
}
