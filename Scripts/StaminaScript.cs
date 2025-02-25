using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaScript : MonoBehaviour
{
    [SerializeField] int maxStamina = 3;
    [SerializeField] int currentStamina;
    [SerializeField] float staminaRegenRate = 2f;
    [SerializeField] Image[] staminaBar;
    public bool isDashing = false;
    // Start is called before the first frame update
    void Start()
    {
        currentStamina = maxStamina;
        StartCoroutine(RegenerateStamina());
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < staminaBar.Length; i++)
        {
            if (i < currentStamina)
            {
                staminaBar[i].enabled = true;
            }
            else
            {
                staminaBar[i].enabled = false;
            }
        }

        Debug.Log(isDashing);
    }

    public bool ConsumeStamina(int amount)
    {
        if (currentStamina >= amount)
        {
            currentStamina -= amount;
            staminaBar[currentStamina].enabled = false;
            return true;
        }
        return false;
    }

    public IEnumerator RegenerateStamina()
    {
        while (true)
        {
            if(!isDashing)
            {
                yield return null;
            }

            yield return new WaitForSeconds(staminaRegenRate);
            if (!isDashing && currentStamina < maxStamina)
            {
                staminaBar[currentStamina].enabled = true;
                currentStamina++;
            }
        }
    }
}
