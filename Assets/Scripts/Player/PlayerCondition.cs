using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface IDamagalbe
{
    void TakePysicalDamage(int damage);
}
public class PlayerCondition : MonoBehaviour, IDamagalbe
{
    public UICondition uiCondition;
    
    Condition health { get { return uiCondition.health; } }
    Condition mana { get { return uiCondition.mana; } }
    Condition hunger { get { return uiCondition.hunger; } }
    Condition stamina { get { return uiCondition.stamina; } }

    public float noHungerHealthDecay;

    public event Action onTakeDamage;

    void Update()
    {
        UpdatePassive();
    }

    public void UpdatePassive()
    {
        mana.Add(mana.passiveValue * Time.deltaTime);
        hunger.Add(hunger.passiveValue * Time.deltaTime);
        stamina.Add(stamina.passiveValue * Time.deltaTime);
        

        if (hunger.curValue > hunger.maxValue / 2)
        {
            health.Add(health.passiveValue * Time.deltaTime);
        }
        else if (hunger.curValue == 0f)
        {
            health.Add(noHungerHealthDecay * Time.deltaTime);
        }

        if (health.curValue == 0f)
        {
            Die();
        }
    }

    public void Heal(float amout)
    {
        health.Add(amout);
    }

    public void Eat(float amount)
    {
        hunger.Add(amount);
    }

    public void Die()
    {
        Debug.Log("ав╬З╢ы.");
    }

    public void TakePysicalDamage(int damage)
    {
        health.Add(-damage);
        onTakeDamage?.Invoke();
    }
}
