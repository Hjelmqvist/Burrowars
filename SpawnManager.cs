using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance = null;

    [Header("Cinemachine Target Group")]
    [SerializeField] CinemachineTargetGroup targetGroup = null;
    [SerializeField] float weight = 40, radius = 5;

    #region Players 

    [Header("Player characters")]
    [Space(20)]
    [SerializeField] PlayersInformation playerInformation = null;

    [System.Serializable]
    class PlayersInformation
    {
        public float secondsBeforePlayersSpawns = 1;
        public float secondsBetweenPlayers = 0.5f;
        public float playerLockedTime = 1;
        public List<Transform> playerSpawnpoints = new List<Transform>();
        public List<Transform> playerRespawnPoints = new List<Transform>();
    }

    public static List<Character> characters = new List<Character>();
    public static List<Player> players = new List<Player>();
    [SerializeField] List<Player> testPlayers = new List<Player>();
    [SerializeField] Character[] characterTypes = null;

    #endregion 

    #region Enemies

    [Header("Enemies")]
    [Space(20)]
    [SerializeField] TextMeshProUGUI enemiesLeftText = null;
    [SerializeField] TextMeshProUGUI waveText = null;
    [SerializeField] WavesInfo waveInfo = null;
    int enemyCount = 0;
    int waveCount = 0;

    List<EnemyInfo> enemiesToSpawn = new List<EnemyInfo>();
    List<Enemy> aliveEnemies = new List<Enemy>();

    [System.Serializable]
    class WavesInfo //General settings for all waves.
    {
        public int secondsBeforeEnemiesSpawns = 30;
        public TwodimensionalArray[] enemySpawnpoints = null;
        public Wave[] waves = null;
    }

    [System.Serializable]
    class TwodimensionalArray //Workaround to get multidimensional arrays in the inspector
    {
        public Transform[] transforms = null; 
    }

    [System.Serializable] 
    class Wave
    {
        public int secondsToKillAllEnemies = 120;
        public int secondsBetweenWavesFailed = 15;
        public int secondsBetweenWavesSucceeded = 30;
        public float secondsBetweenEnemies = 0.5f;
        public List<EnemyInfo> enemies = null;
    } //Might want some different settings for certain waves

    [System.Serializable]
    class EnemyInfo //Information about the enemies to spawn
    {
        public Enemy enemy = null;
        public int minNumber = 20, maxNumber = 25;
        public EnemySize enemySize = EnemySize.Small;
    } 

    enum EnemySize { Small, Medium, Large, Boss }

    #endregion

    /// <summary>
    /// If wave started or ended.
    /// </summary>
    public delegate void WaveChange(bool started);
    public static event WaveChange OnWaveChange;

    void Start()
    {
        Cursor.visible = false;
        OnWaveChange += SpawnManager_OnWaveChange;
        characters.Clear();

        if (Instance == null)
            Instance = this;

        if (players.Count > 0) //Spawn characters from the players list if its not empty
            StartCoroutine(SpawnPlayers(players));
        else
            StartCoroutine(SpawnPlayers(testPlayers));
            
        StartCoroutine(SpawnEnemies());
    }

    private void SpawnManager_OnWaveChange(bool started)
    {
        //Respawn dead and move all characters to the respawn points
        if (!started)
        {
            for (int i = 0; i < characters.Count; i++)
            {
                Vector3 pos = playerInformation.playerRespawnPoints[Random.Range(0, playerInformation.playerRespawnPoints.Count)].position;
                if (characters[i].IsDead)
                {
                    characters[i].Respawn(pos);
                    targetGroup.AddMember(characters[i].transform, weight, radius);
                }   
                else
                    characters[i].transform.position = pos;
            }
        }
    }

    IEnumerator SpawnPlayers(List<Player> players)
    {
        //Create characters for every player and hide them
        foreach (Player p in players)
        {
            Transform spawnpoint = playerInformation.playerSpawnpoints[Random.Range(0, playerInformation.playerSpawnpoints.Count)];
            playerInformation.playerSpawnpoints.Remove(spawnpoint);

            Character c = Instantiate(characterTypes[(int)p.CharacterType], spawnpoint.position, spawnpoint.rotation);
            c.OnCharacterDeath += PlayerDied;
            c.SetupPlayer(p);
            c.LockMovement(true);
            characters.Add(c);
            c.gameObject.SetActive(false);
        }

        IngamePanelManager.Instance.AddCharacters(characters);
        yield return new WaitForSeconds(playerInformation.secondsBeforePlayersSpawns);

        foreach (Character c in characters) //Show characters
        {
            c.gameObject.SetActive(true);
            targetGroup.AddMember(c.transform, weight, radius);
            yield return new WaitForSeconds(playerInformation.secondsBetweenPlayers);
        }

        yield return new WaitForSeconds(playerInformation.playerLockedTime);
        foreach (Character c in characters) //Let characters move
            c.LockMovement(false);
    }

    IEnumerator SpawnEnemies()
    {
        yield return new WaitForSeconds(waveInfo.secondsBeforeEnemiesSpawns);
        foreach (Wave wave in waveInfo.waves)
        {
            OnWaveChange?.Invoke(true);
            waveText.text = string.Format("Wave {0}", (++waveCount).ToString());

            //Add all enemies to spawn for current wave
            foreach (EnemyInfo enemy in wave.enemies)
            {
                int number = Random.Range(enemy.minNumber, enemy.maxNumber);
                for (int i = 0; i < number; i++)
                    enemiesToSpawn.Add(enemy);
            }

            enemyCount = enemiesToSpawn.Count;

            //Spawn enemies
            for (int i = enemiesToSpawn.Count - 1; i >= 0; i--)
            {
                yield return new WaitForSeconds(wave.secondsBetweenEnemies);

                EnemyInfo enemy = enemiesToSpawn[Random.Range(0, enemiesToSpawn.Count)];
                enemiesToSpawn.Remove(enemy);
                Transform spawnpoint = waveInfo.enemySpawnpoints[(int)enemy.enemySize].
                                       transforms[Random.Range(0, waveInfo.enemySpawnpoints[(int)enemy.enemySize].transforms.Length)];
                Enemy e = Instantiate(enemy.enemy, spawnpoint.position, spawnpoint.rotation).GetComponent<Enemy>();
                aliveEnemies.Add(e);
                e.Agent.Warp(spawnpoint.position);
                e.OnEnemyDeath += EnemyDied;

                enemiesLeftText.text = string.Format("Enemies left: {0}/{1}", aliveEnemies.Count, enemyCount);
            }

            //Done spawning enemies
            //Wait until all enemies are dead or wave time is over
            float time = Time.time;
            yield return new WaitUntil(() => aliveEnemies.Count == 0 || Time.time >= time + wave.secondsToKillAllEnemies);
            
            //Killed all enemies on time
            if (aliveEnemies.Count == 0)
            {
                OnWaveChange?.Invoke(false);
                yield return new WaitForSeconds(wave.secondsBetweenWavesSucceeded);
            }  
            else
            {
                yield return new WaitUntil(() => aliveEnemies.Count == 0);
                OnWaveChange?.Invoke(false);
                yield return new WaitForSeconds(wave.secondsBetweenWavesFailed);
            }      
        }

        yield return new WaitUntil(() => aliveEnemies.Count == 0);
        IngamePanelManager.Instance?.Victory();
    }

    void PlayerDied(Character player)
    {
        bool lastPlayerDied = characters.TrueForAll((c) => c.IsDead);

        if (!lastPlayerDied)
        {
            foreach (CinemachineTargetGroup.Target target in targetGroup.m_Targets)
                targetGroup.RemoveMember(target.target);
            foreach (Character c in characters)
            {
                if (!c.IsDead)
                    targetGroup.AddMember(c.transform, weight, radius);
            }
        }
        else
            IngamePanelManager.Instance?.Defeat();
    }

    void EnemyDied(Enemy enemy)
    {
        enemy.OnEnemyDeath -= EnemyDied;
        aliveEnemies.Remove(enemy);
        enemiesLeftText.text = string.Format("Enemies left: {0}/{1}", aliveEnemies.Count, enemyCount);
    }
}
