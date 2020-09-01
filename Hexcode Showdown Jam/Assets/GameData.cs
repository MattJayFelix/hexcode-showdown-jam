using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public GameDriver gameDriver;

    public const int cycleLength = 8;
    public int cyclesCompleted;
    public int roundNumber; // Starts at 1 and increments, this is the score
    public const float speedIncreasePerCycle = 0.25f;

    public int heroHealth;
    public int heroMaxHealth = 20;

    // Set by action phase
    public Entity heroEntity;
    public Entity enemyEntity;

    public string enemyName;
    public string enemyKey;
    public int enemyHealth;
    public int enemyMaxHealth;
    public EnemySheet enemySheet;
    public EnemyAI enemyAI;

    public int backAccentColor;
    public int backAccentChance;

    public int sideAccentColor;
    public int sideAccentChance;

    public int floorAccentColor;
    public int floorAccentChance;

    public IntVectorXYZ heroSpace;
    public IntVectorXYZ enemySpace;


    [System.NonSerialized]
    public string[] names = null;

    public void Reset()
    {
        if (names == null) ReadNames();
        cyclesCompleted = 0;
        roundNumber = 0;
        heroHealth = heroMaxHealth;
        RefreshTimeScale();
    }

    public void RefreshTimeScale()
    {
        Time.timeScale = 1.0f + speedIncreasePerCycle * (float)cyclesCompleted;
    }

    public static char[] splitChars = { '\n' };
    public void ReadNames()
    {
        TextAsset ta = Resources.Load("Names") as TextAsset;
        string fullNamesText = ta.text;
        names = fullNamesText.Split(splitChars);
        for (int i = 0; i < names.Length;i++)
        {
            names[i] = names[i].Substring(0, names[i].Length - 1); // Cut \n
        }
    }
    public string DrawName()
    {
        return names[Random.Range(0, names.Length)];
    }

    public void ReadyNextMatch()
    {
        roundNumber++;
        enemyKey = ChooseEnemyKey();
        enemySheet = gameDriver.enemyLibrary.enemySheets[enemyKey];
        CreateEnemyAI();
        enemyName = DrawName() + " The " + enemySheet.species;
        enemyHealth = enemySheet.health;
        enemyMaxHealth = enemySheet.health;
        PrepareArena();
        heroSpace = new IntVectorXYZ(1, 0, 1);
        enemySpace = new IntVectorXYZ(4, 0, 1);
    }

    public string ChooseEnemyKey()
    {
        int roundInCycle = (roundNumber - 1) % cycleLength;
        if (roundInCycle == 0)
        {
            return "guppy";
        }
        else
        {
            return "guppy";
        }
    }

    public void PrepareArena()
    {
        int roundInCycle = (roundNumber - 1) % cycleLength;
        switch (roundInCycle + Random.Range(0, 2))
        {
            case 0: backAccentColor = 4; break;
            case 1: backAccentColor = 6; break;
            case 2: backAccentColor = 5; break;
            case 3: backAccentColor = 8; break;
            case 4: backAccentColor = 7; break;
            case 5: backAccentColor = 3; break;
            case 6: backAccentColor = 2; break;
            case 7: backAccentColor = 1; break;
            default: backAccentColor = Random.Range(1, 9); break;
        }

        switch (roundInCycle + Random.Range(0,2))
        {
            case 0: sideAccentColor = 8; break;
            case 1: sideAccentColor = 7; break;
            case 2: sideAccentColor = 4; break;
            case 3: sideAccentColor = 5; break;
            case 4: sideAccentColor = 6; break;
            case 5: sideAccentColor = 3; break;
            case 6: sideAccentColor = 2; break;
            case 7: sideAccentColor = 1; break;
            default: sideAccentColor = Random.Range(1, 9); break;
        }

        switch (roundInCycle + Random.Range(0, 2))
        {
            case 0: floorAccentColor = 6; break;
            case 1: floorAccentColor = 4; break;
            case 2: floorAccentColor = 5; break; 
            case 3: floorAccentColor = 1; break;
            case 4: floorAccentColor = 2; break;
            case 5: floorAccentColor = 3; break;
            case 6: floorAccentColor = 8; break;
            case 7: floorAccentColor = 7; break;
            default: floorAccentColor = Random.Range(1, 9); break;
        }
        backAccentColor = 5 + roundInCycle;
        floorAccentChance = 3 + roundNumber;
        sideAccentChance = 2 + roundInCycle;

        /*
         *     public int backAccentColor;
    public int backAccentChance;

    public int sideAccentColor;
    public int sideAccentChance;

    public int floorAccentColor;
    public int floorAccentChance;
    */
    }

    public void CreateEnemyAI()
    {
        if (enemyAI != null)
        {
            Destroy(enemyAI);
        }
        switch (enemySheet.ai)
        {
            default:
                enemyAI = gameObject.AddComponent<GuppyAI>();
                break;
        }
        enemyAI.gameData = this;
    }

}
