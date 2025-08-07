using UnityEngine;
using System;

public class EnemyDeathWatcher : MonoBehaviour
{
    public event Action OnDeath;

    // 적 오브젝트가 파괴될 때 호출됨
    private void OnDestroy()
    {
        OnDeath?.Invoke();
    }
}
