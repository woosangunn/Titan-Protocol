using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerUi : MonoBehaviour
{
    public TMP_Text hpText;
    public TMP_Text ammoText;
    public TMP_Text chainText;

    private PlayerStatus status;
    private PlayerGrappleHandler grapple;

    private Coroutine ammoRefillRoutine;
    private Coroutine hpEffectRoutine;

    void Awake()
    {
        Debug.Log("PlayerUi Awake 호출됨");
        status = Object.FindFirstObjectByType<PlayerStatus>();
        if (status == null)
            Debug.LogError("PlayerStatus를 찾지 못했습니다!");

        grapple = Object.FindFirstObjectByType<PlayerGrappleHandler>();
    }

    void Update()
    {
        if (status != null)
        {
            hpText.text = $"HP: {status.CurrentHealth} / {status.MaxHealth}";
            ammoText.text = $"Ammo: {status.CurrentAmmo} / {status.MaxAmmo}";
        }

        if (grapple != null && grapple.IsAttached)
        {
            float remain = grapple.RemainingChainTime();
            chainText.text = $"Chain: {remain:F1}s / {grapple.maxAttachTime:F1}s";
        }
        else
        {
            chainText.text = "Chain: -";
        }
    }

    public void OnHPChanged()
    {
        if (hpEffectRoutine != null)
            StopCoroutine(hpEffectRoutine);

        hpEffectRoutine = StartCoroutine(HPEffectRoutine());
    }

    IEnumerator HPEffectRoutine()
    {
        hpText.text = "HP: #@!$%";
        yield return new WaitForSeconds(0.2f);
        hpText.text = $"HP: {status.CurrentHealth} / {status.MaxHealth}";
    }

    public void OnAmmoChanged()
    {
        ammoText.text = $"Ammo: {status.CurrentAmmo} / {status.MaxAmmo}";
    }

    public void StartAmmoRefill()
    {
        if (ammoRefillRoutine != null)
            StopCoroutine(ammoRefillRoutine);

        ammoRefillRoutine = StartCoroutine(AmmoRefillRoutine());
    }

    IEnumerator AmmoRefillRoutine()
    {
        int current = 0;
        while (current <= status.MaxAmmo)
        {
            status.CurrentAmmo = current;
            ammoText.text = $"Ammo: {current} / {status.MaxAmmo}";
            current++;
            yield return new WaitForSeconds(0.03f); // 속도 조절 가능
        }
    }
}
