using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionPhase : GamePhase
{
    public const int environmentColor = 3;

    public VoxelTextBox matchCounter;
    public VoxelTextBox heroText;
    public VoxelTextBox versusText;
    public VoxelTextBox enemyText;

    public Entity heroSpriggan;
    public Entity enemySpriggan;

    public float timeout;

    public override void StartPhase()
    {
        CreateEnvironment();
        gameDriver.rasterScanner.finalChunkFlag = false;
        gameDriver.rasterScanner.ResetChunkPtr();
        timeout = 6.0f;
        gameData.ReadyNextMatch();
        CreateText();
        CreateEntities();
    }

    public void CreateText()
    {
        matchCounter = gameDriver.CreateTextBox();
        matchCounter.ShowText("Match " + gameData.roundNumber);
        matchCounter.transform.localPosition = new Vector3(96.0f, 70.0f, 78.0f);

        heroText = gameDriver.CreateTextBox();
        heroText.ShowText("Lisa the Orca");
        heroText.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        heroText.transform.localPosition = new Vector3(62.0f, 52.0f, 78.0f);

        versusText = gameDriver.CreateTextBox();
        versusText.ShowText("VS");
        versusText.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        versusText.transform.localPosition = new Vector3(120.0f, 48.0f, 80.0f);

        enemyText = gameDriver.CreateTextBox();
        enemyText.ShowText(gameData.enemyName);
        enemyText.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        enemyText.transform.localPosition = new Vector3(132.0f, 52.0f, 78.0f);
    }

    public void CreateEntities()
    {
        heroSpriggan = gameDriver.CreateHero();
        heroSpriggan.transform.position = new Vector3(80.0f, 32.0f, 64.0f);
        enemySpriggan = gameDriver.CreateEnemy();
        enemySpriggan.transform.position = new Vector3(176.0f, 32.0f, 64.0f);
    }
    
    public void Update()
    {
        timeout -= Time.unscaledDeltaTime;
        if (timeout <= 0.0f || Input.GetButtonDown("Fire1") || Input.GetButtonDown("Cancel"))
        {
            Advance();
        }
    }

    public void Advance()
    {
        gameDriver.StartPhase(GameDriverPhases.Action);
    }

    public override void EndPhase()
    {
        voxelBuffer.Clear();
        entityRegistry.Clear();
        gameDriver.rasterScanner.ResetChunkPtr();

        Destroy(matchCounter.gameObject);
        Destroy(heroText.gameObject);
        Destroy(versusText.gameObject);
        Destroy(enemyText.gameObject);
    }

    public void CreateEnvironment()
    {
        // Back wall
        for (int i = 0; i < VoxelBuffer.sizeX; i++)
        {
            for (int j = 0; j < VoxelBuffer.sizeY; j++)
            {
                voxelBuffer.Set(new IntVectorXYZ(i, j, VoxelBuffer.sizeZ - 1), environmentColor);
            }
        }
        // Floor
        for (int i = 0; i < VoxelBuffer.sizeX; i++)
        {
            for (int k = 0; k < VoxelBuffer.sizeZ - 1; k++)
            {
                voxelBuffer.Set(new IntVectorXYZ(i, 0, k), environmentColor);
            }
        }
        // Side walls
        for (int j = 0; j < VoxelBuffer.sizeY; j++)
        {
            for (int k = 0; k < VoxelBuffer.sizeZ - 1; k++)
            {
                ;
                voxelBuffer.Set(new IntVectorXYZ(0, j, k), environmentColor);
                voxelBuffer.Set(new IntVectorXYZ(VoxelBuffer.sizeX - 1, j, k), environmentColor);
            }
        }
    }

}
