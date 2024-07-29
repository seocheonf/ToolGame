using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ResourceManager : Manager
{
    private Dictionary<ResourceEnum.Prefab, GameObject> prefabDictionary;
    private Dictionary<ResourceEnum.BGM, AudioClip> bgmDictionary;
    private Dictionary<ResourceEnum.SFX, AudioClip> sfxDictionary;
    // ����� �ͼ��� �ϳ��� �����Ͽ�, �� �ȿ��� bgm�� sfx�� �����ϸ� ���ӳ� ���� ������ �����Ѵ�.
    private AudioMixer loadedMixer;
    public AudioMixer LoadedMixer => loadedMixer;


    int totalResourceAmount = 0;
    int currentResourceComplete = 0;


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

        //�� �ε��� ���ҽ��� ���� 
        totalResourceAmount += ResourcePath.prefabPathArray.Length;
        totalResourceAmount += ResourcePath.bgmPathArray.Length;
        totalResourceAmount += ResourcePath.sfxPathArray.Length;

        //����� �ͼ� �ε�
        GameManager.TurnOnBasicLoadingCavnas("Audio Mixer Loading...");
        loadedMixer = Load<AudioMixer>(ResourcePath.audioMixerPath);
        yield return null;

        //������ ���ҽ� �ε�
        //�� �κ��� �ε��� ���� ������ ���
        //�ε� �߿��� �ε�â �����ֱ� ���� �ڷ�ƾ ó��
        yield return Load(ResourcePath.prefabPathArray, prefabDictionary);
        yield return Load(ResourcePath.bgmPathArray, bgmDictionary);
        yield return Load(ResourcePath.sfxPathArray, sfxDictionary);

        GameManager.TurnOnBasicLoadingCavnas("Resource Loading...");


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
    /// <param name="filePath">���ҽ� ���� ���</param>
    /// <param name="resourceDictionary">���ҽ��� ����� Dictionary</param>
    /// <returns></returns>
    private bool Load<key, value>(string filePath, Dictionary<key, value> resourceDictionary) where key : Enum where value : UnityEngine.Object
    {
        string fileName = GetFileNmae(filePath);
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


    IEnumerator Load<key, value>(string[] filePathArray, Dictionary<key, value> resourceDictionary) where key : Enum where value : UnityEngine.Object
    {
        for (int index = 0; index < filePathArray.Length; index++)
        {
            GameManager.TurnOnBasicLoadingCavnas($"{filePathArray[index]} Loading... [{currentResourceComplete}/{totalResourceAmount}]");
            bool result = false;
            //�ȵǸ� result�ʿ� Load �ֱ�
            yield return new WaitForFunction(() => { result = Load(filePathArray[index], resourceDictionary); });
            if(result)
            {
                currentResourceComplete++;
            }
            yield return null;
        }
    }

    private string GetFileNmae(string filePath)
    {
        //���� ��ΰ� ���� ���
        if (filePath == null || filePath.Length == 0)
        {
            return "";
        }
        //�� ������ '/'�� ���� index. '/'�� ���� ���� 0�� index�� ����ų ��.
        int index = filePath.LastIndexOf('/') + 1;
        
        //���� ��� ���� '/'�� �� �������� ���, �� �κи� ���� �ٽ� �˻��Ѵ�.
        if (index >= filePath.Length)
        {
            string tempt = filePath.Substring(0, index - 1);
            return GetFileNmae(tempt);
        }

        return filePath.Substring(index);
    }
    

}
