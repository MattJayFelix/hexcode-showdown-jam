using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameDriverPhases
{
    Initial, // Opening titles
    MainMenu, // Press Start
    Transition, // Shows next opponent
    Action
}
public class GamePhase : MonoBehaviour
{
    protected GameDriver gameDriver;
    public VoxelBuffer voxelBuffer { get { return gameDriver.voxelBuffer; } }
    public EntityRegistry entityRegistry { get { return gameDriver.entityRegistry; } }
    public GameData gameData { get { return gameDriver.gameData; } }

    public void SetGameDriver(GameDriver g)
    {
        this.gameDriver = g;
    }
    public virtual void StartPhase()
    {

    }
    public virtual void EndPhase()
    {
        Destroy(this.gameObject);
    }
}
public class GameDriver : MonoBehaviour
{
    public Camera mainCamera;
    public Light mainLight;
    public Material paletteMaterial;
    public SprigganLibrary sprigganLibrary;
    public VoxelBuffer voxelBuffer;
    public RasterScanner rasterScanner;
    public EntityRegistry entityRegistry;
    public GameData gameData;
    public EnemyLibrary enemyLibrary;

    public GamePhase currentPhase; 

    void Start()
    {
        StartPhase(GameDriverPhases.Initial);
    }
    public void StartPhase(GameDriverPhases phase)
    {
        if (currentPhase != null)
        {
            currentPhase.EndPhase();
            Destroy(currentPhase.gameObject);
        }
        GameObject phaseOb = new GameObject("Game Phase");
        GamePhase nextPhase;
        switch (phase)
        {
            case GameDriverPhases.MainMenu:
                nextPhase = phaseOb.AddComponent<MainMenuGamePhase>();
                break;
            case GameDriverPhases.Transition:
                nextPhase = phaseOb.AddComponent<TransitionPhase>();
                break;
            case GameDriverPhases.Action:
                nextPhase = phaseOb.AddComponent<ActionGamePhase>();
                break;
            case GameDriverPhases.Initial:
            default:
                nextPhase = phaseOb.AddComponent<InitialGamePhase>();
                break;
        }
        currentPhase = nextPhase;
        currentPhase.SetGameDriver(this);
        currentPhase.StartPhase();
    }

    public VoxelTextBox CreateTextBox()
    {
        GameObject textBoxOb = new GameObject("Voxel Text Box");
        VoxelTextBox result = textBoxOb.AddComponent<VoxelTextBox>();
        result.sprigganLibrary = sprigganLibrary;
        return result;
    }

    public HealthBar CreateHealthBar(string name = "Health Bar")
    {
        GameObject healthBarOb = new GameObject(name);
        HealthBar result = healthBarOb.AddComponent<HealthBar>();
        result.sprigganLibrary = sprigganLibrary;
        result.SetUp();
        return result;
    }

    public Entity CreateHero()
    {
        Entity result = entityRegistry.SpawnEntity("Hero", "orca");
        result.StartAnimation(0); // Neutral animation
        return result;
    }
    public Entity CreateEnemy()
    {
        Entity result = entityRegistry.SpawnEntity("Enemy", gameData.enemyKey);
        
        result.StartAnimation(0); // Neutral animation
        return result;
    }

    public void Update()
    {
        if (Input.GetButton("PrtSc"))
        {
            string path = Application.persistentDataPath + "/" + GetNextScreenshotFilename();
            ScreenCapture.CaptureScreenshot(path);
            Debug.Log("Screenshot written to " + path);
        }
    }

    private string lastScreenshotDateTimeString;
    private int subsecondCounter;
    public string GetNextScreenshotFilename()
    {
        string dateTimeString = System.DateTime.Now.ToString("MMddHHmmss");
        if (dateTimeString == lastScreenshotDateTimeString)
        {
            subsecondCounter++;
        }
        else
        {
            lastScreenshotDateTimeString = dateTimeString;
            subsecondCounter = 0;
        }
        return string.Format("screenshot{0}{1}.png", dateTimeString, subsecondCounter);
    }
}
