using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeManager : MonoBehaviour
{
    public int itemsPickedUp;
    public int itemsLeft;
    public float ghostAttackTimer;
    public MeshRenderer leftHand;
    public MeshRenderer rightHand;
    public Material scaredPacManMaterial;
    public Material normalPacManMaterial;
    public GameObject scaredTint;
    public VRFPSHealthCharacter healthCharacter;
    public float countdown = 4;
    public TMPro.TextMeshPro countdownText;
    public TMPro.TextMeshPro livesText;
    public TMPro.TextMeshPro coinText;
    public int lives = 3;
    public float deadForTime = 5;
    public List<GameObject> respawnLocations;
    public float ghostAttackTime = 10;

    //globals
    private float currentDeadTime = -1;

    // Start is called before the first frame update
    void Start()
    {
        livesText.text = "Lives Left: " + lives;
    }

    void RecieveBonusItem()
    {
        ghostAttackTimer += ghostAttackTime;
    }

    void RunPakmanRespawnCode()
    {
        if(healthCharacter != null && healthCharacter.health <= 0 && currentDeadTime == -1)
        {
            currentDeadTime = deadForTime;
        }

        if(currentDeadTime >= 0)
        {
            currentDeadTime -= Time.deltaTime;

            if(currentDeadTime < 0 && lives > 0)
            {
                lives--;
                livesText.text = "Lives Left: " + lives;
                currentDeadTime = -1;
                int locationRespawn = (int)(Random.Range(0, respawnLocations.Count));
                gameObject.transform.position = respawnLocations[locationRespawn].transform.position;
                healthCharacter.SendMessage("Respawn");
            }
        }

        
    }

    void RunGhostAttackHandColor()
    {
        if (ghostAttackTimer > 0)
        {
            ghostAttackTimer -= Time.deltaTime;
            if (ghostAttackTimer < 0) ghostAttackTimer = 0;

            if (ghostAttackTimer < 4 && (ghostAttackTimer % 0.25) < 0.12)
            {
                leftHand.material = normalPacManMaterial;
                rightHand.material = normalPacManMaterial;
            }
            else
            {
                leftHand.material = scaredPacManMaterial;
                rightHand.material = scaredPacManMaterial;
            }

            scaredTint.SetActive(true);
            healthCharacter.health = 100;
        }
        else
        {
            leftHand.material = normalPacManMaterial;
            rightHand.material = normalPacManMaterial;
            scaredTint.SetActive(false);
        }
    }

    void RunCountdownCode()
    {
        countdown -= Time.deltaTime;

        if (countdown > 0)
        {
            countdownText.text = System.Math.Floor(countdown).ToString();
        }
        else if(countdownText.gameObject.activeSelf)
        {
            countdownText.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        RunCountdownCode();
        RunGhostAttackHandColor();
        RunPakmanRespawnCode();
    }

    public void RecieveItem()
    {
        itemsPickedUp++;
        itemsLeft--;
        coinText.text = "Coins Left: " + itemsLeft;
    }

    public void SetGhostAttackMode()
    {
        ghostAttackTimer = 30;
    }
}
