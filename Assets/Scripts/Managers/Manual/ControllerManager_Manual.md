## ControllerManager

> 입력 시스템에 대한 매니저

<br>

- 입력 시스템을 다루는 매니저입니다.
- 이 게임에서의 모든 입력 장치에 대한 행동은 이 매니저를 통해 이루어져야 합니다.
- 키 - 기능 쌍을 한번 등록하면, 이후 키 입력과 그에 대한 기능을 수행할 수 있습니다.
- 해제하면 이후 키 입력과 그에 대한 기능을 수행할 수 없습니다.

<br>

### 변수

---

1. MouseMovement
    - 마우스의 위치 변화량에 대한 변수입니다.
    - 마우스를 움직였을 때, 그 움직임 자체를 감지하여 x, y 값 형태로 반환합니다
  
### 함수

---

1. AddInputFuncInteraction(FuncInteractionData funcInteractionData)
    - 키보드 키 입력에 대응하는 기능을 등록하는 함수입니다.
    - 주어진 구조체에 맞추어 함수를 수행할 시, 컨트롤러 매니저에 해당 키 - 기능 쌍이 등록됩니다.
    - 이 후, 그 키를 누를 시 기능이 수행됩니다.

2. RRemoveInputFuncInteraction(FuncInteractionData funcInteractionData)
    - 키보드 키 입력에 대응하는 기능을 해제하는 함수입니다.
    - 자신이 등록했을 때 사용했던 구조체를 매개변수로 넣어, 컨트롤러 매니저의 해당 키 - 기능 쌍을 해제합니다.
    - 더 이상 쓰지 않는 키 - 기능 쌍은 반드시 해제해 주어야 합니다.
  
3. AddInputFuncInteraction(List<FuncInteractionData> funcInteractionData)
    - 키보드 키 입력에 대응하는 기능을 등록하는 함수입니다.
    - 주어진 구조체에 맞추어 함수를 수행할 시, 컨트롤러 매니저에 해당 키 - 기능 쌍이 등록됩니다.
    - 구조체가 List로 주어져, List를 순회하며 키 - 기능 쌍을 등록합니다.

4. RRemoveInputFuncInteraction(List<FuncInteractionData> funcInteractionData)
    - 키보드 키 입력에 대응하는 기능을 해제하는 함수입니다.
    - 자신이 등록했을 때 사용했던 구조체를 매개변수로 넣어, 컨트롤러 매니저의 해당 키 - 기능 쌍을 해제합니다.
    - 구조체가 List로 주어져, 자신이 등록했을 때 사용했던 구조체가 List에 담겨있어야 합니다.
    - 더 이상 쓰지 않는 키 - 기능 쌍은 반드시 해제해 주어야 합니다.

### 기타

---

1. FuncInteractionFunction (delegate)
    - 수행할 기능의 구조
    - 반환값이 없고, 매개변수가 없는 형태로 기능을 받습니다.
    - 기본 형태이기에 빈 함수로 감싸서 등록할 수 있습니다. ( () => { 기능 ;} )

<br>
<br>

## FuncInteractionData

> 입력 시스템에 사용되는 키 - 기능 쌍을 위한 구조체

<br>

### 변수

---

1. keyCode
    - 입력할 키 입니다.
  
2. description
    - 수행할 기능에 대한 설명입니다.
  
3. OnFuncInteraction
    - 입력받는 순간의 기능입니다.

5. DurationFuncInteraction
    - 입력받는 동안의 기능입니다.

6. OffFuncInteraction
    - 입력이 종료됐을 때의 기능입니다.













