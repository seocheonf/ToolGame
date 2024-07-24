using System.Collections;
using System.Collections.Generic;

public abstract class Manager
{
    //생성자를 쓰지 않는 이유는, Manager함수를 넣는 타이밍은 델리게이트인 만큼 순서가 중요함.
    //그러다보니 알아서 딱딱 하게 만들 수는 있겠다만, 그만큼 순서가 중요하기 때문에 프로그래머가 직접 순서를 바깥에서 조정할 수 있게 하기 위함.
    //Initiate를 쓰는 이유는 하단 참조.

    //Manager가 생성되었을 때 해야할 일들.
    //코루틴으로서, 스크립트가 실행되는 도중 동시성이 보장되어야 하는 일(오래걸리는 일)들이 있을 가능성에 대비함.
    //로딩화면 진행을 위해 코루틴으로 진행한다고 볼 수 있음.
    public virtual IEnumerator Initiate()
    {
        yield return null;
    }

    //Manager가 본격적인 일을 시작하기 전에 해야할 일.
    //사용이 제한적임. 게임플레이 중간에 들어오는 Manager가 있을 경우, 관련 처리를 위해 GameManager의 Update를 제어하기 위함임.
    //Update로 들어가기 전에 반드시 한번 처리하고 넘어갈 기능을 정의함
    public virtual void ManagerStart() { }
    //Manager가 Update단에서 지속적으로 해야할 일
    public virtual void ManagerUpdate(float deltaTime) { }
    //Manager가 FixedUpdate단에서 지속적으로 해야할 일
    public virtual void ManagerFixedUpdate(float fixedDeltaTime) { }
    //Manager가 파괴되었을 때 한번 해야할 일
    public virtual void ManagerDestroy() { }
}
