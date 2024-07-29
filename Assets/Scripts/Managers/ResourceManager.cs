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
    // 오디오 믹서는 하나를 생성하여, 그 안에서 bgm과 sfx를 관리하며 게임내 사운드 전반을 관리한다.
    private AudioMixer loadedMixer;
    public AudioMixer LoadedMixer => loadedMixer;


    int totalResourceAmount = 0;
    int currentResourceComplete = 0;


    public override IEnumerator Initiate()
    {
        yield return base.Initiate();

        // 혹시모를 중복 생성 방지
        if (prefabDictionary != null)   yield break;
        if (bgmDictionary != null)      yield break;
        if (sfxDictionary != null)      yield break;

        // 리소스 보관장소 마련
        prefabDictionary = new Dictionary<ResourceEnum.Prefab, GameObject>();
        bgmDictionary = new Dictionary<ResourceEnum.BGM, AudioClip>();
        sfxDictionary = new Dictionary<ResourceEnum.SFX, AudioClip>();

        //총 로드할 리소스의 개수 
        totalResourceAmount += ResourcePath.prefabPathArray.Length;
        totalResourceAmount += ResourcePath.bgmPathArray.Length;
        totalResourceAmount += ResourcePath.sfxPathArray.Length;

        //오디오 믹서 로딩
        GameManager.TurnOnBasicLoadingCavnas("Audio Mixer Loading...");
        loadedMixer = Load<AudioMixer>(ResourcePath.audioMixerPath);
        yield return null;

        //나머지 리소스 로딩
        //한 부분의 로딩이 끝날 때까지 대기
        //로딩 중에도 로딩창 보여주기 위해 코루틴 처리
        yield return Load(ResourcePath.prefabPathArray, prefabDictionary);
        yield return Load(ResourcePath.bgmPathArray, bgmDictionary);
        yield return Load(ResourcePath.sfxPathArray, sfxDictionary);

        GameManager.TurnOnBasicLoadingCavnas("Resource Loading...");


    }


    /// <summary>
    /// 파일 경로에 있는 리소스를 UnityEngine.Object 타입으로 메모리에 불러오는 함수
    /// </summary>
    /// <typeparam name="T">UnityEngine.Object를 상속받는 클래스. 일반적으로 유니티에서 활용되는 오브젝트 타입으로 작성하면 된다.</typeparam>
    /// <param name="filePath">리소스의 파일 경로</param>
    /// <returns>타입에 해당하는 리소스 반환</returns>
    private T Load<T>(string filePath) where T : UnityEngine.Object
    {
        //리소스 폴더로부터 리소스 로드
        T loadData = Resources.Load<T>(filePath);
        //리소스가 없는 경우 디버깅용
        if(loadData == null)
        {
            Debug.LogError($"Resource Load has faild : {filePath}");
        }
        return loadData;
    }

    /// <summary>
    /// 파일 경로에 있는 리소스를 로드하여 Dictionary에 보관하는 함수
    /// </summary>
    /// <typeparam name="key">로드된 리소스의 분류</typeparam>
    /// <typeparam name="value">로드된 리소스의 타입</typeparam>
    /// <param name="filePath">리소스 파일 경로</param>
    /// <param name="resourceDictionary">리소스가 저장될 Dictionary</param>
    /// <returns></returns>
    private bool Load<key, value>(string filePath, Dictionary<key, value> resourceDictionary) where key : Enum where value : UnityEngine.Object
    {
        string fileName = GetFileNmae(filePath);
        //Enum 타입을 받아, 그 안에 문자열에 매칭되는 값이 있다면, 그 Enum 값을 반환
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
        else //파일 명하고, Enum명하고 일치하지 않을 경우 디버깅용
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
            //안되면 result쪽에 Load 넣기
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
        //파일 경로가 없을 경우
        if (filePath == null || filePath.Length == 0)
        {
            return "";
        }
        //맨 마지막 '/'의 다음 index. '/'가 없을 경우는 0번 index를 가리킬 것.
        int index = filePath.LastIndexOf('/') + 1;
        
        //파일 경로 구분 '/'가 맨 마지막인 경우, 그 부분만 빼고 다시 검색한다.
        if (index >= filePath.Length)
        {
            string tempt = filePath.Substring(0, index - 1);
            return GetFileNmae(tempt);
        }

        return filePath.Substring(index);
    }
    

}
