using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionGamePhase : GamePhase
{   
    public override void StartPhase()
    {
        InitWalls();
        InitRing();
        InitEntities();
        gameDriver.rasterScanner.finalChunkFlag = false;
        gameDriver.rasterScanner.ResetChunkPtr();
    }

    public int ringColor = 6;

    public int ringXSize = VoxelBuffer.sizeX - 32;
    public int ringZSize = VoxelBuffer.sizeZ - 32;
    public int ringYOffset = 5;

    public const float zVerticalOffsetStep = 8.0f;
    public const float spaceYOffset = 16.0f;

    public GameObject[,] spaces;

    public Entity hero;
    public Entity enemy;

    public IntVectorXYZ heroSpace { get { return gameData.heroSpace; } }
    public IntVectorXYZ enemySpace { get { return gameData.enemySpace; } }

    public EnemyAI enemyAI { get { return gameData.enemyAI; } }

    public HealthBar heroHealth;
    public HealthBar enemyHealth;

    public VoxelTextBox heroText;
    public VoxelTextBox enemyText;

    public VoxelTextBox matchResultText;

    public bool matchStarted { get { return gameData.matchStarted; } set { gameData.matchStarted = value; } }

    public float heroFireCooldown = 0.1f;
    public float heroFireCooldownRemaining = 0.0f;

    public float heroHitstunCooldown = 0.5f; // Simultaneously invulnerability
    public float heroHitstunCooldownRemaining = 0.0f;

    public const float matchEndingTime = 2.5f;
    public float matchEndingTimer = matchEndingTime;

    private float savedTimeScale; // For when game's over

    public GameObject GetSpace(IntVectorXYZ coords)
    {
        return spaces[coords.x, coords.z];
    }

    public void InitWalls()
    {
        // Back wall
        for (int i = 0; i < VoxelBuffer.sizeX; i++)
        {
            for (int j = 0; j < VoxelBuffer.sizeY; j++)
            {
                for (int k = VoxelBuffer.sizeZ - 4; k < VoxelBuffer.sizeZ; k++)
                {
                    int value = DrawColor(gameData.backAccentColor, gameData.backAccentChance, k == VoxelBuffer.sizeZ - 1);
                    IntVectorXYZ coords = new IntVectorXYZ(i, j, k);
                    voxelBuffer.Set(coords, value);
                }
            }
        }
        // Floor
        for (int i = 0; i < VoxelBuffer.sizeX; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                for (int k = 0; k < VoxelBuffer.sizeZ - 1; k++)
                {
                    int value = DrawColor(gameData.floorAccentColor, gameData.floorAccentChance, j == 0);
                    IntVectorXYZ coords = new IntVectorXYZ(i, j, k);
                    voxelBuffer.Set(coords, value);
                }
            }
        }
        // Side walls
        for (int i = 0; i < VoxelBuffer.sizeX; i++)
        {
            if (i > 4 && i < VoxelBuffer.sizeX - 5) continue;
            for (int j = 0; j < VoxelBuffer.sizeY; j++)
            {
                for (int k = 0; k < VoxelBuffer.sizeZ - 1; k++)
                {
                    int value = DrawColor(gameData.sideAccentColor, gameData.sideAccentChance, i == 0 || i == VoxelBuffer.sizeX - 1);
                    IntVectorXYZ coords = new IntVectorXYZ(i, j, k);
                    voxelBuffer.Set(coords, value);
                }
            }
        }
    }
    public void InitRing()
    {
        int leftoverSpaceX = VoxelBuffer.sizeX - ringXSize;
        int leftoverSpaceZ = VoxelBuffer.sizeZ - ringZSize;

        int spaceXSize = ringXSize / 6;
        int spaceZSize = ringZSize / 3;

        spaces = new GameObject[6, 3];

        for (int spaceX = 0; spaceX < 6; spaceX++)
        {
            for (int spaceZ = 0; spaceZ < 3; spaceZ++)
            {
                int startX = leftoverSpaceX / 2 + spaceX * spaceXSize;
                int startZ = leftoverSpaceZ / 2 + spaceZ * spaceZSize;

                for (int i = startX; i < startX + spaceXSize; i++)
                {
                    voxelBuffer.Set(new IntVectorXYZ(i, ringYOffset, startZ), ringColor);
                }
                for (int j = startZ; j < startZ + spaceZSize; j++)
                {
                    voxelBuffer.Set(new IntVectorXYZ(startX, ringYOffset, j), ringColor);
                }

                spaces[spaceX, spaceZ] = new GameObject("Space " + spaceX + ", " + spaceZ);
                spaces[spaceX, spaceZ].transform.position = new Vector3(startX + spaceXSize / 2, ringYOffset + spaceYOffset + zVerticalOffsetStep * spaceZ, startZ + spaceZSize / 2);
                spaces[spaceX, spaceZ].transform.parent = gameObject.transform;

                if (spaceX == 5)
                {
                    // Final X space: draw a Z line at the end
                    for (int j = startZ; j <= startZ + spaceZSize; j++)
                    {
                        voxelBuffer.Set(new IntVectorXYZ(startX + spaceXSize, ringYOffset, j), ringColor);
                    }
                }
                if (spaceZ == 2)
                {
                    // Final Z space: draw a X line at the end
                    for (int i = startX; i <= startX + spaceXSize; i++)
                    {
                        voxelBuffer.Set(new IntVectorXYZ(i, ringYOffset, startZ + spaceZSize), ringColor);
                    }
                }
            }
        }
    }

    public void InitEntities()
    {
        hero = gameDriver.CreateHero();
        gameData.heroEntity = hero;
        hero.gameObject.SetActive(false);

        enemy = gameDriver.CreateEnemy();
        gameData.enemyEntity = enemy;
        enemy.gameObject.SetActive(false);

        heroHealth = gameDriver.CreateHealthBar("Hero Health");
        heroHealth.Show(gameData.heroHealth, gameData.heroMaxHealth);
        heroHealth.transform.position = new Vector3(28.0f, 92.0f, 120.0f);

        enemyHealth = gameDriver.CreateHealthBar("Enemy Health");
        enemyHealth.rightToLeft = true;
        enemyHealth.Show(gameData.enemyHealth, gameData.enemyMaxHealth);
        enemyHealth.transform.position = new Vector3(140.0f, 92.0f, 120.0f);

        heroHealth.gameObject.SetActive(false);
        enemyHealth.gameObject.SetActive(false);

        heroText = gameDriver.CreateTextBox();
        heroText.ShowText("Lisa the Orca");
        heroText.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        heroText.transform.localPosition = new Vector3(57.0f, 80.0f, 120.0f);

        enemyText = gameDriver.CreateTextBox();
        enemyText.ShowText(gameData.enemyName);
        enemyText.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        enemyText.transform.localPosition = new Vector3(144.0f, 80.0f, 120.0f);

    }

    public int DrawColor(int accentColor, int accentChance, bool atEnd)
    {
        int rand = Random.Range(0, 100);
        int result = 0;
        if (rand < accentChance) result = accentColor;
        else if (rand < accentChance + 20) result = 7; // Dark blue
        else if (rand < accentChance + 25) result = 8; // Light blue
        else
        {
            if (atEnd) result = 7; // Back wall's always dark blue
            else result = 0; // Otherwise transparent
        }
        return result;
    }

    public void PositionEntities()
    {
        hero.transform.position = spaces[heroSpace.x, heroSpace.z].transform.position;
        enemy.transform.position = spaces[enemySpace.x, enemySpace.z].transform.position;
    }

    private void Update()
    {
        if (!matchStarted)
        {
            if (gameDriver.rasterScanner.finalChunkFlag)
            {
                hero.gameObject.SetActive(true);
                enemy.gameObject.SetActive(true);
                heroHealth.gameObject.SetActive(true);
                enemyHealth.gameObject.SetActive(true);

                PositionEntities();
                matchStarted = true;
            }
            return;
        }
        CheckHeroShotHits();
        CheckEnemyShotHits();
        if (gameData.matchResult == 0)
        {
            // Hero controls
            ProcessPlayerInput();
            ProcessEnemyAI();
            PositionEntities();
            heroHealth.Show(gameData.heroHealth, gameData.heroMaxHealth);
            enemyHealth.Show(gameData.enemyHealth, gameData.enemyMaxHealth);
            CheckMatchOver();
        }
        if (gameData.matchResult != 0)
        {
            matchEndingTimer -= Time.unscaledDeltaTime;
            matchResultText.transform.position = matchResultText.transform.position + new Vector3(0.0f, 32.0f * Time.unscaledDeltaTime, 0.0f);
            if (matchEndingTimer <= 0.0f)
            {
                Time.timeScale = savedTimeScale;
                if (gameData.matchResult == 1) gameDriver.StartPhase(GameDriverPhases.Transition);
                else if (gameData.matchResult == -1) FinishGame();
            }

        }
    }

    public void ProcessPlayerInput()
    {
        if (heroFireCooldownRemaining > 0.0f)
        {
            heroFireCooldownRemaining -= Time.deltaTime;
        }
        if (heroHitstunCooldownRemaining > 0.0f)
        {
            heroHitstunCooldownRemaining -= Time.deltaTime;
        }

        if (hero.currentAnimation != null && hero.currentAnimation.index != 0 && heroFireCooldownRemaining <= 0.0f && heroHitstunCooldownRemaining <= 0.0f)
        {
            hero.StartAnimation(0);
        }

        if (hero.currentAnimation != null && hero.currentAnimation.index == 0)
        {
            if (Input.GetButtonDown("Horizontal"))
            {
                if (Input.GetAxisRaw("Horizontal") > 0)
                {
                    if (heroSpace.x < 2) gameData.heroSpace = new IntVectorXYZ(heroSpace.x + 1, 0, heroSpace.z);
                }
                else
                {
                    if (heroSpace.x > 0) gameData.heroSpace = new IntVectorXYZ(heroSpace.x - 1, 0, heroSpace.z);
                }
            }
            if (Input.GetButtonDown("Vertical"))
            {
                if (Input.GetAxisRaw("Vertical") > 0)
                {
                    if (heroSpace.z < 2) gameData.heroSpace = new IntVectorXYZ(heroSpace.x, 0, heroSpace.z + 1);
                }
                else
                {
                    if (heroSpace.z > 0) gameData.heroSpace = new IntVectorXYZ(heroSpace.x, 0, heroSpace.z - 1);
                }
            }
            if (Input.GetButtonDown("Fire1") && HeroShotSheet.activeShotCount < 3 && heroFireCooldownRemaining <= 0.0f)
            {
                // Fire!
                Entity heroShot = gameDriver.CreateShot("hero");
                heroShot.transform.position = hero.transform.position + heroShot.GetComponentInChildren<ShotSheet>().GetFireOffset();
                heroFireCooldownRemaining = heroFireCooldown;
                hero.StartAnimation(1);
            }
        }

        if (Input.GetButtonDown("Cancel"))
        {
            FinishGame();
        }
    }

    public void ProcessEnemyAI()
    {
        if (enemyAI.moveFlag)
        {
            enemyAI.moveFlag = false;
            gameData.enemySpace = enemyAI.moveDestination;
        }
    }

    public void CheckHeroShotHits()
    {
        for (int i=0;i < HeroShotSheet.activeShots.Length;i++)
        {
            HeroShotSheet thisShot = HeroShotSheet.activeShots[i];
            if (thisShot == null) continue;
            bool shotHit = thisShot.CheckForHit(enemy);
            if (shotHit)
            {
                Destroy(thisShot.shotEntity.gameObject);
                if (enemyAI != null && !enemyAI.invulnerable)
                {
                    if (!enemyAI.invulnerableDuringHitstun || enemyAI.hitStunRemaining <= 0.0f) gameData.enemyHealth -= thisShot.damage;
                    enemyAI.OnWasHit();
                }
            }
        }
    }

    public void CheckEnemyShotHits()
    {
        if (enemyAI == null) return;
        for (int i = 0; i < enemyAI.activeShots.Length; i++)
        {
            ShotSheet thisShot = enemyAI.activeShots[i];
            if (thisShot == null) continue;
            bool shotHit = thisShot.CheckForHit(hero);
            if (shotHit)
            {
                Destroy(thisShot.shotEntity.gameObject);
                if (hero.currentAnimation != null && hero.currentAnimation.index != 2)
                {
                    heroHitstunCooldownRemaining = heroHitstunCooldown;
                    gameData.heroHealth -= thisShot.damage;                    
                    hero.StartAnimation(2);
                }
            }
        }
    }

    public void CheckMatchOver()
    {
        if (gameData.heroHealth <= 0)
        {
            gameData.matchResult = -1;
            matchEndingTimer = matchEndingTime;
            matchResultText = gameDriver.CreateTextBox();
            ShowMatchResultText(gameData.enemyName + " Wins");
            savedTimeScale = Time.timeScale;
            Time.timeScale = 0.1f;
            //Debug.Log("CLEARING ENEMY AI");
            gameData.ClearEnemyAI();
        }
        if (gameData.enemyHealth <= 0)
        {
            gameData.matchResult = 1;
            matchEndingTimer = matchEndingTime;
            ShowMatchResultText("Lisa The Orca Wins");
            savedTimeScale = Time.timeScale;
            Time.timeScale = 0.25f;
            Debug.Log("CLEARING ENEMY AI");
            gameData.ClearEnemyAI();
        }
    }

    public void ShowMatchResultText(string text)
    {
        matchResultText = gameDriver.CreateTextBox();
        matchResultText.ShowText(text);
        matchResultText.transform.position = new Vector3(87.0f, 8.0f, 21.0f);
        matchResultText.transform.localScale = new Vector3(0.25f, 0.25f, 0.1f);
        matchResultText.transform.rotation = Quaternion.Euler(45.0f, 0.0f, 0.0f);
    }

    public void FinishGame()
    {
        int currentHighScoreValue = UnityEngine.PlayerPrefs.GetInt("highscore");
        int gameScore = gameData.roundNumber;
        if (currentHighScoreValue < gameScore)
        {
            UnityEngine.PlayerPrefs.SetInt("highscore", gameScore);
        }
        gameDriver.StartPhase(GameDriverPhases.MainMenu);
    }


    public override void EndPhase()
    {
        Destroy(heroHealth.gameObject);
        Destroy(enemyHealth.gameObject);
        Destroy(heroText.gameObject);
        Destroy(enemyText.gameObject);
        if (matchResultText != null) Destroy(matchResultText.gameObject);
        voxelBuffer.Clear();
        entityRegistry.Clear();
        gameDriver.rasterScanner.ResetChunkPtr();
    }
}
