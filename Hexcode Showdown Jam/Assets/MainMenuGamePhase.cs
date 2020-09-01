using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuGamePhase : GamePhase
{
    public const int environmentColor = 5;

    public Entity culler;
    public Entity wail;

    public GameObject promptsContainer;

    public VoxelTextBox startGame;
    public VoxelTextBox instructionsPrompt;
    public VoxelTextBox quit;

    public VoxelTextBox highScore;

    public VoxelTextBox controlsBox;

    public int selectionIndex = 0;

    public int screenIndex;

    public float menuCycle = 0.0f; // 0 to 2.0f
    public float menuPeriod = 2.0f;
    
    public override void StartPhase()
    {
        Time.timeScale = 1.0f;
        CreateEnvironment();
        gameDriver.rasterScanner.finalChunkFlag = false;
        gameDriver.rasterScanner.ResetChunkPtr();
        CreateMenu();
        ShowScreen(0);
    }

    public void CreateMenu()
    {
        // SCREEN 0
        culler = entityRegistry.SpawnEntity("Culler", "cullerText");
        wail = entityRegistry.SpawnEntity("Wail", "wailText");
        culler.transform.position = new Vector3(92.0f, 70.0f, 80.0f);
        wail.transform.position = new Vector3(110.0f, 49.0f, 80.0f);

        startGame = gameDriver.CreateTextBox();
        startGame.ShowText("START GAME");
        instructionsPrompt = gameDriver.CreateTextBox();
        instructionsPrompt.ShowText("CONTROLS");
        quit = gameDriver.CreateTextBox();
        quit.ShowText("QUIT");

        highScore = gameDriver.CreateTextBox();
        int highScoreValue = UnityEngine.PlayerPrefs.GetInt("highscore");
        if (highScoreValue > 0) highScore.ShowText("Longest run - " + highScoreValue);
        highScore.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        highScore.transform.localPosition = new Vector3(96.0f, 8.0f, 16.0f);
        highScore.transform.localRotation = Quaternion.Euler(70.0f, 0.0f, 0.0f);

        promptsContainer = new GameObject("Text Container");
        startGame.transform.parent = promptsContainer.transform;
        instructionsPrompt.transform.parent = promptsContainer.transform;
        quit.transform.parent = promptsContainer.transform;

        promptsContainer.transform.position = new Vector3(136.0f, 64.0f, 60.0f);
        startGame.transform.localPosition = Vector3.zero;
        instructionsPrompt.transform.localPosition = new Vector3(0.0f, -6.0f, 0.0f);
        quit.transform.localPosition = new Vector3(0.0f, -12.0f, 0.0f);

        // Initial sizes
        startGame.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
        instructionsPrompt.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
        quit.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);

        // SCREEN 1
        controlsBox = gameDriver.CreateTextBox();
        controlsBox.ShowText("CONTROLS\nWASD - MOVE\nSPACE OR ENTER - FIRE\nESCAPE - QUIT");
        controlsBox.transform.localPosition = new Vector3(96.0f, 62.0f, 78.0f);
        controlsBox.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
    }

    public void ShowScreen(int s)
    {
        screenIndex = s;
        if (s == 0)
        {
            culler.gameObject.SetActive(true);
            wail.gameObject.SetActive(true);
            promptsContainer.gameObject.SetActive(true);
            controlsBox.gameObject.SetActive(false);
            highScore.gameObject.SetActive(true);
        }
        else if (s == 1)
        {
            culler.gameObject.SetActive(false);
            wail.gameObject.SetActive(false);
            promptsContainer.gameObject.SetActive(false);
            controlsBox.gameObject.SetActive(true);
            highScore.gameObject.SetActive(false);
        }
    }

    public void AdjustSizes()
    {
        startGame.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
        instructionsPrompt.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
        quit.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);

        menuCycle += Time.deltaTime;
        while (menuCycle > menuPeriod) menuCycle -= menuPeriod;

        float specialScale;
        if (menuCycle < menuPeriod / 2.0f)
        {
            float lerp = menuCycle / (menuPeriod / 2.0f);
            specialScale = Mathf.Lerp(0.25f, 0.30f, lerp);
        }
        else
        {
            float lerp = (menuCycle - (menuPeriod / 2.0f)) / (menuPeriod / 2.0f);
            specialScale = Mathf.Lerp(0.30f, 0.25f, lerp);
        }

        Transform selectedTransform;
        if (selectionIndex == 0) selectedTransform = startGame.transform;
        else if (selectionIndex == 1) selectedTransform = instructionsPrompt.transform;
        else /*if (selectionIndex == 2)*/ selectedTransform = quit.transform;

        selectedTransform.transform.localScale = new Vector3(specialScale, specialScale, specialScale);
    }

    public void DoQuit()
    {
        if (!Application.isEditor) Application.Quit();
        else gameDriver.StartPhase(GameDriverPhases.Initial);
    }
    public void Update()
    {
        if (screenIndex == 0)
        {
            AdjustSizes();
            if (Input.GetButtonDown("Cancel"))
            {
                DoQuit();
                //gameDriver.StartPhase(GameDriverPhases.Initial);
                return;
            }
            if (Input.GetButtonDown("Vertical"))
            {
                if (Input.GetAxisRaw("Vertical") > 0) MoveSelection(-1);
                else if (Input.GetAxisRaw("Vertical") < 0) MoveSelection(1);
                //            AdvanceScreen();
            }
            if (Input.GetButtonDown("Fire1"))
            {
                if (selectionIndex == 0)
                {
                    gameData.Reset();
                    gameDriver.StartPhase(GameDriverPhases.Transition);
                }
                else if (selectionIndex == 1)
                {
                    ShowScreen(1);
                }
                else
                {
                    DoQuit();
                }
            }
        }
        else if (screenIndex == 1)
        {
            if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Cancel"))
            {
                ShowScreen(0);
            }
        }
    }

    public void MoveSelection(int value)
    {
        selectionIndex += value;
        if (selectionIndex < 0) selectionIndex = 2;
        if (selectionIndex > 2) selectionIndex = 0;
    }

    public override void EndPhase()
    {
        voxelBuffer.Clear();
        entityRegistry.Clear();
        gameDriver.rasterScanner.ResetChunkPtr();

        Destroy(startGame.gameObject);
        Destroy(instructionsPrompt.gameObject);
        Destroy(quit.gameObject);
        Destroy(controlsBox.gameObject);
        Destroy(highScore.gameObject);
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
