## GameManager

> 게임 시스템에 대한 전반적인 기능들이 정의되어 있는 매니저

<br>

- 싱글톤으로 생성되어, 언제든지 접근할 수 있습니다.
- 다른 매니저로의 접근이나, 게임 시스템에 대한 변수에 접근할 수 있습니다.
- 게임 중 일반적으로(보편적으로) 할 수 있을 법한 일들이 정의되어 실행시킬 수 있습니다.
 
<br>

- 유니티의 Start, Update, FixedUpdate, Destroy함수를 일반적으로 이곳에서 수행해야 합니다.
- 타이밍과 대상에 알맞은 델리게이트에 함수를 등록하거나 해제하여야 합니다.

<br>

### 변수

---

#### 일반변수

1. Instance
    - 게임 매니저 객체입니다.
    - 일반적으로, 게임 매니저 내부 정보는 이 값을 통해 불러올 수 있습니다.

2. controller, currentworld, option, resource, save, sound, ui
    - 기타 매니저 객체입니다.
  
3. IsScriptEntireUpdateStop
    - 델리게이트 변수 전체의 수행을 멈춥니다.

5. IsScriptManagersUpdateStop
    - ManagersUpdate와 ManagersFixedUpdate의 수행을 멈춥니다.

7. IsScriptObjectsUpdateStop
    - ObjectsUpdate와 ObjectsFixedUpdate의 수행을 멈춥니다.

#### 델리게이트 변수

    1. 각 타이밍에 해야할 일들을 각 델리게이트를 통해 수행할 수 있습니다.
    
    2. '+=' 연산을 통해 해야할 일들을 등록할 수 있습니다.
    
    3. [ Start 함수 ] => [ Update 함수 ] / [ FixedUpdate 함수 ] => [ Destroy 함수 ] 순으로 수행합니다.

###### [ Start 함수 ]

1. ManagersStart
    - 매니저들이 나타났을 때 해야할 일들
    - 1회 수행하며, 이 함수가 전부 실행되지 않으면 ObjectsStart와 각종 Update/FixedUpdate를 수행할 수 없습니다.

2. ObjectsStart
    - 일반적인 게임 오브젝트들이 나타났을 때 해야할 일들
    - 1회 수행합니다.

###### [ Update 함수 ]

3. ManagersUpdate
    - 매니저들이 1 프레임마다 해야할 일들
    - 1 프레임 마다 지속적으로 수행합니다.
    - 해야할 일의 등록과 해제를 직접 해주어야 합니다.

4. ObjectsUpdate
    - 일반적인 게임 오브젝트들이 1 프레임마다 해야할 일들
    - 1 프레임 마다 지속적으로 수행합니다.
    - 해야할 일의 등록과 해제를 직접 해주어야 합니다.

###### [ FixedUpdate 함수 ]

5. ManagersFixedUpdate
    - 매니저들이 1 Fixed 프레임마다 해야할 일들
    - 1 Fixed 프레임 마다 지속적으로 수행합니다.
    - 해야할 일의 등록과 해제를 직접 해주어야 합니다.

6. ObjectsFixedUpdate
    - 일반적인 게임 오브젝트들이 1 Fixed 프레임마다 해야할 일들
    - 1 Fixed 프레임 마다 지속적으로 수행합니다.
    - 해야할 일의 등록과 해제를 직접 해주어야 합니다.

###### [ Destroy 함수 ]

7. ManagersDestroy
    - 매니저들이 사라질 때 해야할 일들
    - 1회 수행합니다.

8. ObjectsDestroy
    - 일반적인 게임 오브젝트들이 사라질 때 해야할 일들
    - 1회 수행합니다.

<br>
  
### 함수

---

1. TurnOnBasicLoadingCavnas

2. TurnOffBasicLoadingCavnas
