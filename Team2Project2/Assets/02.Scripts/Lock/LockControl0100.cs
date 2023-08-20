﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockControl0100 : MonoBehaviour
{
    private int[] result, correctCombination;
    public bool isOpened;

    private SoundManager soundManager;

    private void Start()
    {
        soundManager = SoundManager.instance;
        result = new int[]{0,0,0,0};
        correctCombination = new int[] { 0, 1, 0, 0 };
        isOpened = false;
        Rotate.Rotated += CheckResults;
    }

    private void CheckResults(string wheelName, int number)
    {
        switch (wheelName)
        {
            case "WheelOne":
                result[0] = number;
                break;

            case "WheelTwo":
                result[1] = number;
                break;

            case "WheelThree":
                result[2] = number;
                break;

            case "WheelFour":
                result[3] = number;
                break;
        }

        if (result[0] == correctCombination[0] && result[1] == correctCombination[1]
            && result[2] == correctCombination[2] && result[3] == correctCombination[3] && !isOpened)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.03f, transform.position.z);
            isOpened = true;
            soundManager.PlaySound(soundManager.sfxPlayer, soundManager.sfx, "LockOpen");
        }
    }

    private void OnDestroy()
    {
        Rotate.Rotated -= CheckResults;
    }
}
