using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class ResourceManager : Manager
{
    // 편리하게 가져오게 하기 위해 static으로 선언. 아래의 GetResource 계열 함수를 통해 이 내부 값들을 찾아 가져올 수 있음.
    private static Dictionary<ResourceEnum.Prefab, GameObject> prefabDictionary;
    private static Dictionary<ResourceEnum.BGM, AudioClip> bgmDictionary;
    private static Dictionary<ResourceEnum.SFX, AudioClip> sfxDictionary;
    private static Dictionary<ResourceEnum.Mesh, Mesh> meshDictionary;
    private static Dictionary<ResourceEnum.Material, Material> materialDictionary;
    // 오디오 믹서는 하나를 생성하여, 그 안에서 bgm과 sfx를 관리하며 게임내 사운드 전반을 관리한다.
    private static AudioMixer mainMixer;
    public static AudioMixer MainMixer => mainMixer;

    private int totalResourceAmount = 0;
    private int currentResourceComplete = 0;

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
        meshDictionary = new Dictionary<ResourceEnum.Mesh, Mesh>();
        materialDictionary = new Dictionary<ResourceEnum.Material, Material>();

        //총 로드할 리소스의 개수 
        totalResourceAmount += ResourcePath.prefabPathArray.Length;
        totalResourceAmount += ResourcePath.bgmPathArray.Length;
        totalResourceAmount += ResourcePath.sfxPathArray.Length;
        totalResourceAmount += ResourcePath.meshPathArray.Length;
        totalResourceAmount += ResourcePath.materialPathArray.Length;

        //오디오 믹서 로딩
        GameManager.TurnOnBasicLoadingCavnas("Audio Mixer Loading...");
        mainMixer = Load<AudioMixer>(ResourcePath.audioMixerPath);
        yield return null;

        //나머지 리소스 로딩
        //한 부분의 로딩이 끝날 때까지 대기
        //로딩 중에도 로딩창 보여주기 위해 코루틴 처리
        GameManager.TurnOnBasicLoadingCavnas("Resource Loading...");
        yield return Load(ResourcePath.prefabPathArray, prefabDictionary);
        yield return Load(ResourcePath.bgmPathArray, bgmDictionary);
        yield return Load(ResourcePath.sfxPathArray, sfxDictionary);
        yield return Load(ResourcePath.meshPathArray, meshDictionary);
        yield return Load(ResourcePath.materialPathArray, materialDictionary);

        //로딩 완료 정보 띄우기
        GameManager.TurnOnBasicLoadingCavnas($"Resource Loaded Completely ({currentResourceComplete} / {totalResourceAmount})");

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
    /// <typeparam name="key">로드될 리소스의 분류</typeparam>
    /// <typeparam name="value">로드될 리소스의 타입</typeparam>
    /// <param name="filePath">타입에 해당하는 리소스 파일 경로</param>
    /// <param name="resourceDictionary">타입에 해당하는 리소스가 저장될 Dictionary</param>
    /// <returns>리소스 로드 성공 여부</returns>
    private bool Load<key, value>(string filePath, Dictionary<key, value> resourceDictionary) where key : Enum where value : UnityEngine.Object
    {
        string fileName = GetFileName(filePath);
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
    /// <summary>
    /// 특정 타입의 파일 경로 배열에 있는 각 파일 경로에 있는 리소스를 로드하여 해당 타입의 Dictionary에 보관하는 함수
    /// </summary>
    /// <typeparam name="key">로드될 리소스의 분류</typeparam>
    /// <typeparam name="value">로드될 리소스의 타입</typeparam>
    /// <param name="filePathArray">타입에 해당하는 리소스 파일 경로 배열</param>
    /// <param name="resourceDictionary">타입에 해당하는 리소스가 저장될 Dictionary</param>
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
    /// 파일 경로를 받아와 파일의 이름을 반환하는 함수
    /// </summary>
    /// <param name="filePath">파일 경로</param>
    /// <returns>파일 이름</returns>
    private string GetFileName(string filePath)
    {
        //파일 경로가 없을 경우
        if (filePath == null || filePath.Length <= 0)
        {
            return "";
        }
        //맨 마지막 '/'의 다음 index. '/'가 없을 경우는 0번 index를 가리킬 것.
        int index = filePath.LastIndexOf('/') + 1;
        
        //파일 경로 구분 '/'가 맨 마지막인 경우, 그 부분만 빼고 다시 검색한다.
        if (index >= filePath.Length)
        {
            string tempt = filePath.Substring(0, index - 1);
            return GetFileName(tempt);
        }

        return filePath.Substring(index);
    }
    
    /// <summary>
    /// Prefab 리소스를 불러오는 함수
    /// </summary>
    /// <param name="targetPrefab">불러오고자 하는 Prefab 리소스 정보(Enum에 기록된 이름)</param>
    /// <param name="result">Prefab 리소스 인스턴스</param>
    /// <returns>리소스를 가져오는 것 성공 여부</returns>
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
    /// Prefab 리소스를 불러오는 함수
    /// </summary>
    /// <param name="targetPrefab">불러오고자 하는 Prefab 리소스 정보(Enum에 기록된 이름)</param>
    /// <returns>Prefab 리소스 인스턴스</returns>
    public static GameObject GetResource(ResourceEnum.Prefab targetPrefab)
    {
        return prefabDictionary[targetPrefab];
    }

    /// <summary>
    /// Bgm 리소스를 불러오는 함수
    /// </summary>
    /// <param name="targetBgm">불러오고자 하는 Bgm 리소스 정보(Enum에 기록된 이름)</param>
    /// <param name="result">Bgm 리소스 인스턴스</param>
    /// <returns>리소스를 가져오는 것 성공 여부</returns>
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
    /// Bgm 리소스를 불러오는 함수
    /// </summary>
    /// <param name="targetBgm">불러오고자 하는 Bgm 리소스 정보(Enum에 기록된 이름)</param>
    /// <returns>Bgm 리소스 인스턴스</returns>
    public static AudioClip GetResource(ResourceEnum.BGM targetBgm)
    {
        return bgmDictionary[targetBgm];
    }

    /// <summary>
    /// SFX 리소스를 불러오는 함수
    /// </summary>
    /// <param name="targetSFX">불러오고자 하는 SFX 리소스 정보(Enum에 기록된 이름)</param>
    /// <param name="result">SFX 리소스 인스턴스</param>
    /// <returns>리소스를 가져오는 것 성공 여부</returns>
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
    /// SFX 리소스를 불러오는 함수
    /// </summary>
    /// <param name="targetSFX">불러오고자 하는 SFX 리소스 정보(Enum에 기록된 이름)</param>
    /// <returns>SFX 리소스 인스턴스</returns>
    public static AudioClip GetResource(ResourceEnum.SFX targetSFX)
    {
        return sfxDictionary[targetSFX];
    }


    /// <summary>
    /// Mesh 리소스를 불러오는 함수
    /// </summary>
    /// <param name="targetMesh">불러오고자 하는 Mesh 리소스 정보(Enum에 기록된 이름)</param>
    /// <param name="result">Mesh 리소스 인스턴스</param>
    /// <returns>리소스를 가져오는 것 성공 여부</returns>
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
    /// Mesh 리소스를 불러오는 함수
    /// </summary>
    /// <param name="targetMesh">불러오고자 하는 Mesh 리소스 정보(Enum에 기록된 이름)</param>
    /// <returns>Mesh 리소스 인스턴스</returns>
    public static Mesh GetResource(ResourceEnum.Mesh targetMesh)
    {
        return meshDictionary[targetMesh];
    }

    /// <summary>
    /// Material 리소스를 불러오는 함수
    /// </summary>
    /// <param name="targetMaterial">불러오고자 하는 Material 리소스 정보(Enum에 기록된 이름)</param>
    /// <param name="result">Material 리소스 인스턴스</param>
    /// <returns>리소스를 가져오는 것 성공 여부</returns>
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
    /// Material 리소스를 불러오는 함수
    /// </summary>
    /// <param name="targetMaterial">불러오고자 하는 Material 리소스 정보(Enum에 기록된 이름)</param>
    /// <returns>Material 리소스 인스턴스</returns>
    public static Material GetResource(ResourceEnum.Material targetMaterial)
    {
        return materialDictionary[targetMaterial];
    }




}
