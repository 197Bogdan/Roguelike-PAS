using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager: MonoBehaviour
{
    public DungeonCreator dungeonData;

    public void Save()
    {
        GameData gameData = new GameData();
        
        gameData.rooms = new List<RoomNode>();
        gameData.corridors = new List<CorridorNode>();
        gameData.enemies = new List<CharacterSaveData>();

        foreach (Node node in dungeonData.dungeonRooms)
        {
            if (node is RoomNode room)
                gameData.rooms.Add(room);
            else if (node is CorridorNode corridor)
                gameData.corridors.Add(corridor);
        }

        GameObject player = dungeonData.player;
        CharacterSaveData playerData = new CharacterSaveData();
        playerData.position = player.transform.position;
        playerData.rotation = player.transform.rotation;
        playerData.health = player.GetComponent<PlayerStats>().GetHealth();
        playerData.mana = player.GetComponent<PlayerStats>().GetMana();
        playerData.exp = player.GetComponent<PlayerStats>().GetExp();
        playerData.level = player.GetComponent<PlayerStats>().GetLevel();
        gameData.player = playerData;

        List<GameObject> enemies = dungeonData.enemies;
        foreach (GameObject enemy in enemies)
        {
            CharacterSaveData enemyData = new CharacterSaveData();
            if(enemy == null) 
                continue; // enemy was killed (destroyed)

            enemyData.position = enemy.transform.position;
            enemyData.rotation = enemy.transform.rotation;
            enemyData.health = enemy.GetComponent<EnemyStats>().GetHealth();
            enemyData.mana = enemy.GetComponent<EnemyStats>().GetMana();
            enemyData.exp = enemy.GetComponent<EnemyStats>().GetExp();
            enemyData.level = enemy.GetComponent<EnemyStats>().GetLevel();
            enemyData.type = enemy.GetComponent<EnemyStats>().GetEnemyType();
            gameData.enemies.Add(enemyData);
        }

        string json = JsonUtility.ToJson(gameData);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/save.json", json);
    }

    public void DeleteSave()
    {
        System.IO.File.Delete(Application.persistentDataPath + "/save.json");
    }

    public bool IsSavedGame()
    {
        return System.IO.File.Exists(Application.persistentDataPath + "/save.json");
    }

    public void LoadSave()
    {
        string json = System.IO.File.ReadAllText(Application.persistentDataPath + "/save.json");
        GameData gameData = JsonUtility.FromJson<GameData>(json);

        foreach(RoomNode room in gameData.rooms)
            dungeonData.dungeonRooms.Add(room);
        foreach(CorridorNode corridor in gameData.corridors)
            dungeonData.dungeonRooms.Add(corridor);
        dungeonData.playerSaveData = gameData.player;
        dungeonData.enemySaveData = gameData.enemies;
    }

}

[Serializable]
public class CharacterSaveData
{
    public Vector3 position;
    public Quaternion rotation;
    public int health;
    public int mana;
    public int exp;
    public int level;
    public int type; // enemy prefab
}

[Serializable]
public class GameData
{
    public List<RoomNode> rooms;
    public List<CorridorNode> corridors;
    public List<CharacterSaveData> enemies;
    public CharacterSaveData player;
}