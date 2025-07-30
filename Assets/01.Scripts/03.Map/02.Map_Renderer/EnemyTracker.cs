using UnityEngine;
using System.Collections.Generic;
using System;

public class EnemyTracker : MonoBehaviour
{
    private List<GameObject> enemies = new();

    public event Action OnAllEnemiesDefeated;

    public void RegisterEnemy(GameObject enemy)
    {
        if (!enemies.Contains(enemy))
        {
            enemies.Add(enemy);
            EnemyDeathWatcher watcher = enemy.AddComponent<EnemyDeathWatcher>();
            watcher.OnDeath += () => UnregisterEnemy(enemy);
        }
    }

    private void UnregisterEnemy(GameObject enemy)
    {
        enemies.Remove(enemy);

        if (enemies.Count == 0)
        {
            OnAllEnemiesDefeated?.Invoke();
        }
    }

    // 테스트용, 즉시 클리어 이벤트 발생
    public void ForceClear()
    {
        enemies.Clear();
        OnAllEnemiesDefeated?.Invoke();
    }
}
