using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VRFPSHealthCharacter : MonoBehaviour
{
    [SerializeField] public float health;
    [SerializeField] private float maxHealth;

    [SerializeField] private bool updateColorAdjustmentOnDamage;
    [SerializeField] private Volume volume;
    [SerializeField] private Color fullHealthColor;
    [SerializeField] private Color noHealthColor;

    [SerializeField] private bool makeObjectVisibleOnDamage;
    [SerializeField] private GameObject objVisible;
    [SerializeField] private float dmgPercentToBeVisible;
    [SerializeField] private bool visibleOnAllDamage;

    [SerializeField] private bool playSoundOnDeath;
    [SerializeField] private AudioSource deathAudio;

    [SerializeField] private bool invincible;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    void Respawn()
    {
        health = maxHealth;

        if (makeObjectVisibleOnDamage)
        {
            objVisible.SetActive(false);
        }
    }

    void ApplyDamage(VRFPSDamageInfo dmgInfo)
    {
        if (!invincible && health > 0)
        {
            health -= dmgInfo.amt;

            if(health <= 0 && playSoundOnDeath)
            {
                deathAudio.Play();
            }
        }

        if (volume != null && maxHealth > 0)
        {
            if (updateColorAdjustmentOnDamage)
            {
                if(volume.profile.TryGet(out ColorAdjustments colorAdjust)) {
                    colorAdjust.colorFilter = new ColorParameter(Color.Lerp(fullHealthColor, noHealthColor, health / maxHealth));
                }
            }
        }

        if(makeObjectVisibleOnDamage && maxHealth > 0 && ((visibleOnAllDamage && health != maxHealth) || health / maxHealth <= (dmgPercentToBeVisible / 100)))
        {
            objVisible.SetActive(true);
        }
        else if(makeObjectVisibleOnDamage)
        {
            objVisible.SetActive(false);
        }
    }
}
