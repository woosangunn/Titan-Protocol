using UnityEngine;
using System;

public class EnemyDeathWatcher : MonoBehaviour
{
    public event Action OnDeath;

    private void OnDestroy()
    {
        OnDeath?.Invoke();
    }
}
