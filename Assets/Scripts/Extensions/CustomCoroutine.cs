using System.Threading.Tasks;
using UnityEngine;

//생성자로 받은 함수가 끝날 때 까지 대기하는 코루틴용 반환 값
//이미 코루틴 내부에 있는 상황에서 yield return하는 값임
public class WaitForFunction : CustomYieldInstruction
{
    bool isWaiting;

    public override bool keepWaiting => isWaiting;

    public WaitForFunction(System.Action wantFunction)
    {
        isWaiting = true;

        Run(wantFunction);
    }

    //비동기함수를 만들고 갈게요!
    async void Run(System.Action wantFunction)
    {
        await Task.Run(wantFunction);
        isWaiting = false;
    }
}