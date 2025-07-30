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

    // �׽�Ʈ��, ��� Ŭ���� �̺�Ʈ �߻�
    public void ForceClear()
    {
        enemies.Clear();
        OnAllEnemiesDefeated?.Invoke();
    }
}
