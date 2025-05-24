using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : SingletonMono<SFXManager>
{
    [Header("SFX 配置列表")]
    public List<SFX> sfxList;

    // 内部字典：根据 SFX 名称快速查找对应的 SFX 数据（预制体等）
    private Dictionary<SFXName, SFX> sfxDict = new Dictionary<SFXName, SFX>();
    // 对象池字典：每种 SFX 对应一个对象池（使用 Queue 管理）
    private Dictionary<SFXName, Queue<ParticleSystem>> pool = new Dictionary<SFXName, Queue<ParticleSystem>>();

    protected void Awake()
    {
        InitializeSfxDictionary();
    }

    /// <summary>
    /// 初始化 SFX 字典，方便后续通过名称查找对应的 SFX 配置
    /// </summary>
    private void InitializeSfxDictionary()
    {
        foreach (var sfx in sfxList)
        {
            if (sfxDict.ContainsKey(sfx.name))
            {
                Debug.LogWarning("发现重复的 SFX 名称: " + sfx.name);
            }
            else if (sfx.prefab == null)
            {
                Debug.LogWarning("SFX [" + sfx.name + "] 的 prefab 为空！");
            }
            else
            {
                sfxDict.Add(sfx.name, sfx);
            }
        }
    }
    /// <summary>
    /// 播放指定 SFX，指定持续时间（秒）后自动停止并回收，
    /// 不修改任何 Burst 数量设置。
    /// </summary>
    /// <param name="sfxName">SFX 名称</param>
    /// <param name="position">播放位置</param>
    /// <param name="duration">持续时间（秒）——小于等于 0 时按预制体自身时长播放</param>
    /// <param name="color">可选：覆盖粒子 StartColor</param>
    public ParticleSystem PlaySfx(SFXName sfxName, Vector3 position, float duration, Color color = default)
    {
        if (!sfxDict.TryGetValue(sfxName, out var sfxData))
        {
            Debug.LogError($"无法找到 SFX: {sfxName}");
            return null;
        }

        ParticleSystem psInstance = GetFromPool(sfxName, sfxData.prefab);
        if (psInstance == null)
        {
            Debug.LogError($"实例化 SFX 失败: {sfxName}");
            return null;
        }

        psInstance.transform.position = position;
        if (!color.Equals(default)) psInstance.startColor = color;

        psInstance.gameObject.SetActive(true);
        psInstance.Play();

        // 若 duration <= 0，则继续用预制体原本的生命周期
        if (duration > 0f)
            StartCoroutine(StopAndReturnAfterDuration(psInstance, sfxName, duration));
        else
            StartCoroutine(ReturnToPoolWhenFinished(psInstance, sfxName));

        return psInstance;
    }

    /// <summary>
    /// 协程：在指定持续时间后停止粒子播放并回收到对象池
    /// </summary>
    private IEnumerator StopAndReturnAfterDuration(ParticleSystem psInstance, SFXName sfxName, float duration)
    {
        yield return new WaitForSeconds(duration);

        // 停止发射并清除现有粒子
        psInstance.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        // 等待彻底停止（含子粒子）后回收
        yield return new WaitUntil(() => !psInstance.IsAlive(true));
        psInstance.gameObject.SetActive(false);
        ReturnToPool(sfxName, psInstance);
    }
    /// <summary>
    /// 播放指定名称的 SFX，并在播放结束后将实例回收到对象池中
    /// </summary>
    /// <param name="sfxName">SFX 名称，对应 sfxList 中的项</param>
    /// <param name="position">播放位置</param>
    public ParticleSystem PlaySfx(SFXName sfxName, Vector3 position, Color color = default, int count = -1)
    {
        if (!sfxDict.ContainsKey(sfxName))
        {
            Debug.LogError("无法找到 SFX: " + sfxName);
            return null;
        }

        SFX sfxData = sfxDict[sfxName];
        ParticleSystem psInstance = GetFromPool(sfxName, sfxData.prefab);
        if (psInstance == null)
        {
            Debug.LogError("实例化 SFX 失败: " + sfxName);
            return null;
        }

        // 设置播放位置，并激活对象
        psInstance.transform.position = position;
        if(!color.Equals(default))psInstance.startColor = color;

        var emission = psInstance.emission;
        ParticleSystem.Burst[] bursts = new ParticleSystem.Burst[emission.burstCount];
        emission.GetBursts(bursts);
        if (bursts.Length > 0 && count>0)
        {
            // 修改 Burst Count
            bursts[0].count = count;

            // 重新设置 Burst
            emission.SetBursts(bursts);
        }

        psInstance.gameObject.SetActive(true);
        psInstance.Play();

        // 启动协程，监测该粒子系统何时播放结束，从而回收到对象池中
        StartCoroutine(ReturnToPoolWhenFinished(psInstance, sfxName));
        return psInstance;
    }

    /// <summary>
    /// 从对象池中获取一个 ParticleSystem 实例；如果没有可用实例，则新建一个
    /// </summary>
    /// <param name="sfxName">SFX 名称</param>
    /// <param name="prefab">对应的预制体</param>
    /// <returns>ParticleSystem 实例</returns>
    private ParticleSystem GetFromPool(SFXName sfxName, ParticleSystem prefab)
    {
        if (!pool.ContainsKey(sfxName))
        {
            pool[sfxName] = new Queue<ParticleSystem>();
        }

        if (pool[sfxName].Count > 0)
        {
            return pool[sfxName].Dequeue();
        }
        else
        {
            // 新建实例，并将其设置为 SFXManager 的子对象，便于层级管理
            ParticleSystem instance = Instantiate(prefab, transform);
            return instance;
        }
    }

    /// <summary>
    /// 协程：等待粒子系统播放结束后，将其实例回收到对象池中
    /// </summary>
    /// <param name="psInstance">粒子系统实例</param>
    /// <param name="sfxName">对应的 SFX 名称</param>
    /// <returns></returns>
    private IEnumerator ReturnToPoolWhenFinished(ParticleSystem psInstance, SFXName sfxName)
    {
        // 等待直到粒子系统完全停止（包括所有子粒子）
        yield return new WaitUntil(() => !psInstance.IsAlive(true));
        // 隐藏对象并归还到池中
        psInstance.gameObject.SetActive(false);
        ReturnToPool(sfxName, psInstance);
    }

    /// <summary>
    /// 将粒子系统实例归还到对象池中
    /// </summary>
    /// <param name="sfxName">对应的 SFX 名称</param>
    /// <param name="psInstance">粒子系统实例</param>
    private void ReturnToPool(SFXName sfxName, ParticleSystem psInstance)
    {
        if (!pool.ContainsKey(sfxName))
        {
            pool[sfxName] = new Queue<ParticleSystem>();
        }
        pool[sfxName].Enqueue(psInstance);
    }
}

[Serializable]
public struct SFX
{
    public SFXName name;
    public ParticleSystem prefab;
}

public enum SFXName
{
    Food
}
