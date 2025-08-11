using UnityEngine;
using MyGame.Map;

public class EnemyTracker : MonoBehaviour
{
    private RoomData room;
    private RoomRenderer roomrenderer;
    private int enemyCount = 0;

    public void Initialize(RoomData room, RoomRenderer renderer)
    {
        this.room = room;
        this.roomrenderer = renderer;
    }

    public void RegisterEnemy(GameObject enemy)
    {
        enemyCount++;

        enemy.GetComponent<EnemyDeathWatcher>().OnDeath += HandleEnemyDeath;
    }

    private void HandleEnemyDeath()
    {
        enemyCount--;

        if (enemyCount <= 0)
        {
            room.state = RoomState.Cleared;
            roomrenderer.OpenAllDoors();
        }
    }
}
