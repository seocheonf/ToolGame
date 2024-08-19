using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class ResourceManager : Manager
{
    // ���ϰ� �������� �ϱ� ���� static���� ����. �Ʒ��� GetResource �迭 �Լ��� ���� �� ���� ������ ã�� ������ �� ����.
    private static Dictionary<ResourceEnum.Prefab, GameObject> prefabDictionary;
    private static Dictionary<ResourceEnum.BGM, AudioClip> bgmDictionary;
    private static Dictionary<ResourceEnum.SFX, AudioClip> sfxDictionary;
    private static Dictionary<ResourceEnum.Mesh, Mesh> meshDictionary;
    private static Dictionary<ResourceEnum.Material, Material> materialDictionary;
    // ����� �ͼ��� �ϳ��� �����Ͽ�, �� �ȿ��� bgm�� sfx�� �����ϸ� ���ӳ� ���� ������ �����Ѵ�.
    private static AudioMixer mainMixer;
    public static AudioMixer MainMixer => mainMixer;

    private int totalResourceAmount = 0;
    private int currentResourceComplete = 0;

    public override IEnumerator Initiate()
    {
        yield return base.Initiate();

        // Ȥ�ø� �ߺ� ���� ����
        if (prefabDictionary != null)   yield break;
        if (bgmDictionary != null)      yield break;
        if (sfxDictionary != null)      yield break;

        // ���ҽ� ������� ����
        prefabDictionary = new Dictionary<ResourceEnum.Prefab, GameObject>();
        bgmDictionary = new Dictionary<ResourceEnum.BGM, AudioClip>();
        sfxDictionary = new Dictionary<ResourceEnum.SFX, AudioClip>();
        meshDictionary = new Dictionary<ResourceEnum.Mesh, Mesh>();
        materialDictionary = new Dictionary<ResourceEnum.Material, Material>();

        //�� �ε��� ���ҽ��� ���� 
        totalResourceAmount += ResourcePath.prefabPathArray.Length;
        totalResourceAmount += ResourcePath.bgmPathArray.Length;
        totalResourceAmount += ResourcePath.sfxPathArray.Length;
        totalResourceAmount += ResourcePath.meshPathArray.Length;
        totalResourceAmount += ResourcePath.materialPathArray.Length;

        //����� �ͼ� �ε�
        GameManager.TurnOnBasicLoadingCavnas("Audio Mixer Loading...");
        mainMixer = Load<AudioMixer>(ResourcePath.audioMixerPath);
        yield return null;

        //������ ���ҽ� �ε�
        //�� �κ��� �ε��� ���� ������ ���
        //�ε� �߿��� �ε�â �����ֱ� ���� �ڷ�ƾ ó��
        GameManager.TurnOnBasicLoadingCavnas("Resource Loading...");
        yield return Load(ResourcePath.prefabPathArray, prefabDictionary);
        yield return Load(ResourcePath.bgmPathArray, bgmDictionary);
        yield return Load(ResourcePath.sfxPathArray, sfxDictionary);
        yield return Load(ResourcePath.meshPathArray, meshDictionary);
        yield return Load(ResourcePath.materialPathArray, materialDictionary);

        //�ε� �Ϸ� ���� ����
        GameManager.TurnOnBasicLoadingCavnas($"Resource Loaded Completely ({currentResourceComplete} / {totalResourceAmount})");

    }


    /// <summary>
    /// ���� ��ο� �ִ� ���ҽ��� UnityEngine.Object Ÿ������ �޸𸮿� �ҷ����� �Լ�
    /// </summary>
    /// <typeparam name="T">UnityEngine.Object�� ��ӹ޴� Ŭ����. �Ϲ������� ����Ƽ���� Ȱ��Ǵ� ������Ʈ Ÿ������ �ۼ��ϸ� �ȴ�.</typeparam>
    /// <param name="filePath">���ҽ��� ���� ���</param>
    /// <returns>Ÿ�Կ� �ش��ϴ� ���ҽ� ��ȯ</returns>
    private T Load<T>(string filePath) where T : UnityEngine.Object
    {
        //���ҽ� �����κ��� ���ҽ� �ε�
        T loadData = Resources.Load<T>(filePath);
        //���ҽ��� ���� ��� ������
        if(loadData == null)
        {
            Debug.LogError($"Resource Load has faild : {filePath}");
        }
        return loadData;
    }
    /// <summary>
    /// ���� ��ο� �ִ� ���ҽ��� �ε��Ͽ� Dictionary�� �����ϴ� �Լ�
    /// </summary>
    /// <typeparam name="key">�ε�� ���ҽ��� �з�</typeparam>
    /// <typeparam name="value">�ε�� ���ҽ��� Ÿ��</typeparam>
    /// <param name="filePath">Ÿ�Կ� �ش��ϴ� ���ҽ� ���� ���</param>
    /// <param name="resourceDictionary">Ÿ�Կ� �ش��ϴ� ���ҽ��� ����� Dictionary</param>
    /// <returns>���ҽ� �ε� ���� ����</returns>
    private bool Load<key, value>(string filePath, Dictionary<key, value> resourceDictionary) where key : Enum where value : UnityEngine.Object
    {
        string fileName = GetFileName(filePath);
        //Enum Ÿ���� �޾�, �� �ȿ� ���ڿ��� ��Ī�Ǵ� ���� �ִٸ�, �� Enum ���� ��ȯ
        if(Enum.TryParse(typeof(key), fileName, out object filekey))
        {
            value loadData = Load<value>(filePath);
            if(loadData == null)
            {
                return false;
            }
            resourceDictionary.Add((key)filekey, loadData);
            return true;
        }
        else //���� ���ϰ�, Enum���ϰ� ��ġ���� ���� ��� ������
        {
            Debug.LogError($"File name \"{fileName}\" has not matched enum value in {typeof(key)}");
            return false;
        }
    }
    /// <summary>
    /// Ư�� Ÿ���� ���� ��� �迭�� �ִ� �� ���� ��ο� �ִ� ���ҽ��� �ε��Ͽ� �ش� Ÿ���� Dictionary�� �����ϴ� �Լ�
    /// </summary>
    /// <typeparam name="key">�ε�� ���ҽ��� �з�</typeparam>
    /// <typeparam name="value">�ε�� ���ҽ��� Ÿ��</typeparam>
    /// <param name="filePathArray">Ÿ�Կ� �ش��ϴ� ���ҽ� ���� ��� �迭</param>
    /// <param name="resourceDictionary">Ÿ�Կ� �ش��ϴ� ���ҽ��� ����� Dictionary</param>
    /// <returns></returns>
    private IEnumerator Load<key, value>(string[] filePathArray, Dictionary<key, value> resourceDictionary) where key : Enum where value : UnityEngine.Object
    {
        for (int index = 0; index < filePathArray.Length; index++)
        {
            GameManager.TurnOnBasicLoadingCavnas($"{filePathArray[index]} Now Loading... [{currentResourceComplete}/{totalResourceAmount}]");
            if(Load(filePathArray[index], resourceDictionary))
            {
                currentResourceComplete++;
            }
            yield return null;
        }
    }

    /// <summary>
    /// ���� ��θ� �޾ƿ� ������ �̸��� ��ȯ�ϴ� �Լ�
    /// </summary>
    /// <param name="filePath">���� ���</param>
    /// <returns>���� �̸�</returns>
    private string GetFileName(string filePath)
    {
        //���� ��ΰ� ���� ���
        if (filePath == null || filePath.Length <= 0)
        {
            return "";
        }
        //�� ������ '/'�� ���� index. '/'�� ���� ���� 0�� index�� ����ų ��.
        int index = filePath.LastIndexOf('/') + 1;
        
        //���� ��� ���� '/'�� �� �������� ���, �� �κи� ���� �ٽ� �˻��Ѵ�.
        if (index >= filePath.Length)
        {
            string tempt = filePath.Substring(0, index - 1);
            return GetFileName(tempt);
        }

        return filePath.Substring(index);
    }
    
    /// <summary>
    /// Prefab ���ҽ��� �ҷ����� �Լ�
    /// </summary>
    /// <param name="targetPrefab">�ҷ������� �ϴ� Prefab ���ҽ� ����(Enum�� ��ϵ� �̸�)</param>
    /// <param name="result">Prefab ���ҽ� �ν��Ͻ�</param>
    /// <returns>���ҽ��� �������� �� ���� ����</returns>
    public static bool TryGetResource(ResourceEnum.Prefab targetPrefab, out GameObject result)
    {
        result = null;
        if (prefabDictionary.TryGetValue(targetPrefab, out result))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    /// <summary>
    /// Prefab ���ҽ��� �ҷ����� �Լ�
    /// </summary>
    /// <param name="targetPrefab">�ҷ������� �ϴ� Prefab ���ҽ� ����(Enum�� ��ϵ� �̸�)</param>
    /// <returns>Prefab ���ҽ� �ν��Ͻ�</returns>
    public static GameObject GetResource(ResourceEnum.Prefab targetPrefab)
    {
        return prefabDictionary[targetPrefab];
    }

    /// <summary>
    /// Bgm ���ҽ��� �ҷ����� �Լ�
    /// </summary>
    /// <param name="targetBgm">�ҷ������� �ϴ� Bgm ���ҽ� ����(Enum�� ��ϵ� �̸�)</param>
    /// <param name="result">Bgm ���ҽ� �ν��Ͻ�</param>
    /// <returns>���ҽ��� �������� �� ���� ����</returns>
    public static bool TryGetResource(ResourceEnum.BGM targetBgm, out AudioClip result)
    {
        result = null;
        if(bgmDictionary.TryGetValue(targetBgm, out result))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    /// <summary>
    /// Bgm ���ҽ��� �ҷ����� �Լ�
    /// </summary>
    /// <param name="targetBgm">�ҷ������� �ϴ� Bgm ���ҽ� ����(Enum�� ��ϵ� �̸�)</param>
    /// <returns>Bgm ���ҽ� �ν��Ͻ�</returns>
    public static AudioClip GetResource(ResourceEnum.BGM targetBgm)
    {
        return bgmDictionary[targetBgm];
    }

    /// <summary>
    /// SFX ���ҽ��� �ҷ����� �Լ�
    /// </summary>
    /// <param name="targetSFX">�ҷ������� �ϴ� SFX ���ҽ� ����(Enum�� ��ϵ� �̸�)</param>
    /// <param name="result">SFX ���ҽ� �ν��Ͻ�</param>
    /// <returns>���ҽ��� �������� �� ���� ����</returns>
    public static bool TryGetResource(ResourceEnum.SFX targetSFX, out AudioClip result)
    {
        result = null;
        if (sfxDictionary.TryGetValue(targetSFX, out result))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    /// <summary>
    /// SFX ���ҽ��� �ҷ����� �Լ�
    /// </summary>
    /// <param name="targetSFX">�ҷ������� �ϴ� SFX ���ҽ� ����(Enum�� ��ϵ� �̸�)</param>
    /// <returns>SFX ���ҽ� �ν��Ͻ�</returns>
    public static AudioClip GetResource(ResourceEnum.SFX targetSFX)
    {
        return sfxDictionary[targetSFX];
    }


    /// <summary>
    /// Mesh ���ҽ��� �ҷ����� �Լ�
    /// </summary>
    /// <param name="targetMesh">�ҷ������� �ϴ� Mesh ���ҽ� ����(Enum�� ��ϵ� �̸�)</param>
    /// <param name="result">Mesh ���ҽ� �ν��Ͻ�</param>
    /// <returns>���ҽ��� �������� �� ���� ����</returns>
    public static bool TryGetResource(ResourceEnum.Mesh targetMesh, out Mesh result)
    {
        result = null;
        if (meshDictionary.TryGetValue(targetMesh, out result))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    /// <summary>
    /// Mesh ���ҽ��� �ҷ����� �Լ�
    /// </summary>
    /// <param name="targetMesh">�ҷ������� �ϴ� Mesh ���ҽ� ����(Enum�� ��ϵ� �̸�)</param>
    /// <returns>Mesh ���ҽ� �ν��Ͻ�</returns>
    public static Mesh GetResource(ResourceEnum.Mesh targetMesh)
    {
        return meshDictionary[targetMesh];
    }

    /// <summary>
    /// Material ���ҽ��� �ҷ����� �Լ�
    /// </summary>
    /// <param name="targetMaterial">�ҷ������� �ϴ� Material ���ҽ� ����(Enum�� ��ϵ� �̸�)</param>
    /// <param name="result">Material ���ҽ� �ν��Ͻ�</param>
    /// <returns>���ҽ��� �������� �� ���� ����</returns>
    public static bool TryGetResource(ResourceEnum.Material targetMaterial, out Material result)
    {
        result = null;
        if (materialDictionary.TryGetValue(targetMaterial, out result))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    /// <summary>
    /// Material ���ҽ��� �ҷ����� �Լ�
    /// </summary>
    /// <param name="targetMaterial">�ҷ������� �ϴ� Material ���ҽ� ����(Enum�� ��ϵ� �̸�)</param>
    /// <returns>Material ���ҽ� �ν��Ͻ�</returns>
    public static Material GetResource(ResourceEnum.Material targetMaterial)
    {
        return materialDictionary[targetMaterial];
    }




}
