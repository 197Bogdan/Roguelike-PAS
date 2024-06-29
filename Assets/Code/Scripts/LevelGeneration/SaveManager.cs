using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager(List<Node> nodes, GameObject player, GameObject[] enemies)
{
    public List<RoomNode> rooms;
    public List<CorridorNode> corridors;
    public List<CharacterSaveData> characters;

    public void Save()
    {
        rooms = new List<RoomNode>();
        corridors = new List<CorridorNode>();
        characters = new List<CharacterSaveData>();

        foreach (Node node in nodes)
        {
            if (node is RoomNode room)
                rooms.Add(room);
            else if (node is CorridorNode corridor)
                corridors.Add(corridor);
        }

        CharacterSaveData playerData = new CharacterSaveData();
        playerData.position = player.transform.position;
        playerData.rotation = player.transform.rotation;
        playerData.health = player.GetComponent<PlayerStats>().GetHealth();
        playerData.mana = player.GetComponent<PlayerStats>().GetMana();
        playerData.exp = player.GetComponent<PlayerStats>().GetExp();
        playerData.level = player.GetComponent<PlayerStats>().GetLevel();
        characters.Add(playerData);

        foreach (GameObject enemy in enemies)
        {
            CharacterSaveData enemyData = new CharacterSaveData();
            enemyData.position = enemy.transform.position;
            enemyData.rotation = enemy.transform.rotation;
            enemyData.health = enemy.GetComponent<EnemyStats>().GetHealth();
            enemyData.mana = enemy.GetComponent<EnemyStats>().GetMana();
            enemyData.exp = enemy.GetComponent<EnemyStats>().GetExp();
            enemyData.level = enemy.GetComponent<EnemyStats>().GetLevel();
            characters.Add(enemyData);
        }

        string json = JsonUtility.ToJson(this);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/save.json", json);
    }

}

public class CharacterSaveData
{
    public Vector3 position;
    public Quaternion rotation;
    public int health;
    public int mana;
    public int exp;
    public int level;
}