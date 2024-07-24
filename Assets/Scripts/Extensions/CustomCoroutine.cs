using System.Threading.Tasks;
using UnityEngine;

//�����ڷ� ���� �Լ��� ���� �� ���� ����ϴ� �ڷ�ƾ�� ��ȯ ��
//�̹� �ڷ�ƾ ���ο� �ִ� ��Ȳ���� yield return�ϴ� ����
public class WaitForFunction : CustomYieldInstruction
{
    bool isWaiting;

    public override bool keepWaiting => isWaiting;

    public WaitForFunction(System.Action wantFunction)
    {
        isWaiting = true;

        Run(wantFunction);
    }

    //�񵿱��Լ��� ����� ���Կ�!
    async void Run(System.Action wantFunction)
    {
        await Task.Run(wantFunction);
        isWaiting = false;
    }
}