using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ResourceManager : Manager
{

    private Dictionary<ResourceEnum.Prefab, GameObject> prefabDictionary;
    private Dictionary<ResourceEnum.BGM, AudioClip> bgmDictionary;
    private Dictionary<ResourceEnum.SFX, AudioClip> sfxDictionary;

    private AudioMixer loadedMixer;
    public AudioMixer LoadedMixer => loadedMixer;

    int resourceAmount;
    int resourceComplete;


    public override IEnumerator Initiate()
    {
        yield return base.Initiate();

        // Ȥ�ø� �ߺ� ���� ����
        if (prefabDictionary != null)   yield break;
        if (bgmDictionary != null)      yield break;
        if (sfxDictionary != null)      yield break;

        prefabDictionary = new Dictionary<ResourceEnum.Prefab, GameObject>();
        bgmDictionary = new Dictionary<ResourceEnum.BGM, AudioClip>();
        sfxDictionary = new Dictionary<ResourceEnum.SFX, AudioClip>();

        resourceAmount += ResourcePath.prefabPathArray.Length;
        resourceAmount += ResourcePath.bgmPathArray.Length;
        resourceAmount += ResourcePath.sfxPathArray.Length;

        


        //�ӽÿ�
        //UIManager���� �θ��� �����ؾ� ��.
        GameManager.TurnOnBasicLoadingCavnas("Reslurce Loading...");

        //��� �׽�Ʈ2

    }


    T Load<T>(string filePath) where T : UnityEngine.Object
    {

        return null;
    }



}
