using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PMAudioManager : MonoBehaviour
{
    [SerializeField] private GameModeManager gameModeManager;
    [SerializeField] private AudioSource introAudio;
    [SerializeField] private AudioSource mainAudio;
    [SerializeField] private AudioSource scaredAudio;
    [SerializeField] private AudioSource victoryAudio;
    

    private float elapsedTime;
    private int playingSong;

    // Start is called before the first frame update
    void Start()
    {
        introAudio.Play();
        playingSong = 0;
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;

        if (gameModeManager.ghostAttackTimer > 0)
        {
            if (playingSong != 1)
            {
                mainAudio.Stop();
                victoryAudio.Stop();
                scaredAudio.Play();
                playingSong = 1;
            }
        }
        else if(gameModeManager.itemsLeft <= 0)
        {
            if (playingSong != 2)
            {
                mainAudio.Stop();
                victoryAudio.Play();
                scaredAudio.Stop();
                playingSong = 2;
            }
        }
        else if(elapsedTime > 4)
        {
            if (playingSong != 3)
            {
                mainAudio.Play();
                victoryAudio.Stop();
                scaredAudio.Stop();
                playingSong = 3;
            }
        }
    }
}
