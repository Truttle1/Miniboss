using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterHealthbar : MonoBehaviour
{
    public GameObject healthBar;
    public GameObject heatlhBarContainer;
    [SerializeField] private float healthBarMaxSize = 2.5f;
    private Monster monster;
    void Start()
    {
        //healthBarMaxSize = healthBar.transform.localScale.x;
        monster = GetComponent<Monster>();
    }

    private void Update()
    {
        if (monster.getTarget() || monster.getHovered())
        {
            heatlhBarContainer.SetActive(true);
        }
        else
        {
            heatlhBarContainer.SetActive(false);
        }
    }

    public void setHP(int HP, int maxHP)
    {
        healthBar.transform.localScale = new Vector3(((HP * 1.0f) / maxHP) * healthBarMaxSize, healthBar.transform.localScale.y, healthBar.transform.localScale.z);
    }
}
