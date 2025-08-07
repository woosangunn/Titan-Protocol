using UnityEngine;
using System;

public class EnemyDeathWatcher : MonoBehaviour
{
    public event Action OnDeath;

    // �� ������Ʈ�� �ı��� �� ȣ���
    private void OnDestroy()
    {
        OnDeath?.Invoke();
    }
}
