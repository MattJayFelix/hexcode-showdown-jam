using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialGamePhase : GamePhase
{
    public const int environmentColor = 4;

    public Entity pressureSpikeText;
    public Entity pressureSpikeSpike;

    public Entity forTheDSOP;
    public Entity hexcodeShowdownJam;

    public Entity funkyFuture;
    public Entity eight;

    public int screenIndex;

    public float advanceTimer = 2.0f;

    public override void StartPhase()
    {
        CreateEnvironment();
        screenIndex = 0;
        gameDriver.rasterScanner.finalChunkFlag = false;
    }

    public void ShowScreen(int s)
    {
        if (s == 0)
        {
        }
        else if (s == 1)
        {
            advanceTimer = 2.0f;
            pressureSpikeSpike = entityRegistry.SpawnEntity("Spike", "pressureSpikeSpike");
            pressureSpikeText = entityRegistry.SpawnEntity("Text", "pressureSpikeText");
            pressureSpikeSpike.transform.position = new Vector3(128.0f, 52.0f, 64.0f);
            pressureSpikeText.transform.position = new Vector3(128.0f, 48.0f, 63.0f);
        }
        else if (s == 2)
        {
            advanceTimer = 2.0f;
            pressureSpikeSpike.gameObject.SetActive(false);
            pressureSpikeText.gameObject.SetActive(false);
            forTheDSOP = entityRegistry.SpawnEntity("For The DSOP", "forTheDSOP");
            hexcodeShowdownJam = entityRegistry.SpawnEntity("Hexcode Showdown Jam", "hexcodeShowdownJam");
            forTheDSOP.transform.position = new Vector3(128.0f, 65.0f, 64.0f);
            hexcodeShowdownJam.transform.position = new Vector3(128.0f, 48.0f, 63.0f);
        }
        else if (s == 3)
        {
            advanceTimer = 2.0f;
            forTheDSOP.gameObject.SetActive(false);
            hexcodeShowdownJam.gameObject.SetActive(false);
            funkyFuture = entityRegistry.SpawnEntity("Funky Future", "funkyFuture");
            eight = entityRegistry.SpawnEntity("Eight", "eight");
            funkyFuture.transform.position = new Vector3(120.0f, 60.0f, 40.0f);
            eight.transform.position = new Vector3(150.0f, 52.0f, 40.0f);
        }
        else
        {
            gameDriver.StartPhase(GameDriverPhases.MainMenu);
        }
    }

    public void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            ShowScreen(999);
            return;
        }
        if (Input.GetButtonDown("Fire1"))
        {
            AdvanceScreen();
        }
        if (screenIndex > 0)
        {
            advanceTimer -= Time.deltaTime;
            if (advanceTimer < 0.0f)
            {
                AdvanceScreen();
            }
        }
        else
        {
            if (gameDriver.rasterScanner.finalChunkFlag)
            {
                AdvanceScreen();
            }
        }
    }

    public void AdvanceScreen()
    {
        screenIndex++;
        ShowScreen(screenIndex);
    }

    public override void EndPhase()
    {
        entityRegistry.Clear();
        voxelBuffer.Clear();
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
            {;
                voxelBuffer.Set(new IntVectorXYZ(0, j, k), environmentColor);
                voxelBuffer.Set(new IntVectorXYZ(VoxelBuffer.sizeX-1, j, k), environmentColor);
            }
        }
    }
}
