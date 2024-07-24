using System.Collections;
using System.Collections.Generic;

public abstract class Manager
{
    //�����ڸ� ���� �ʴ� ������, Manager�Լ��� �ִ� Ÿ�̹��� ��������Ʈ�� ��ŭ ������ �߿���.
    //�׷��ٺ��� �˾Ƽ� ���� �ϰ� ���� ���� �ְڴٸ�, �׸�ŭ ������ �߿��ϱ� ������ ���α׷��Ӱ� ���� ������ �ٱ����� ������ �� �ְ� �ϱ� ����.
    //Initiate�� ���� ������ �ϴ� ����.

    //Manager�� �����Ǿ��� �� �ؾ��� �ϵ�.
    //�ڷ�ƾ���μ�, ��ũ��Ʈ�� ����Ǵ� ���� ���ü��� ����Ǿ�� �ϴ� ��(�����ɸ��� ��)���� ���� ���ɼ��� �����.
    //�ε�ȭ�� ������ ���� �ڷ�ƾ���� �����Ѵٰ� �� �� ����.
    public virtual IEnumerator Initiate()
    {
        yield return null;
    }

    //Manager�� �������� ���� �����ϱ� ���� �ؾ��� ��.
    //����� ��������. �����÷��� �߰��� ������ Manager�� ���� ���, ���� ó���� ���� GameManager�� Update�� �����ϱ� ������.
    //Update�� ���� ���� �ݵ�� �ѹ� ó���ϰ� �Ѿ ����� ������
    public virtual void ManagerStart() { }
    //Manager�� Update�ܿ��� ���������� �ؾ��� ��
    public virtual void ManagerUpdate(float deltaTime) { }
    //Manager�� FixedUpdate�ܿ��� ���������� �ؾ��� ��
    public virtual void ManagerFixedUpdate(float fixedDeltaTime) { }
    //Manager�� �ı��Ǿ��� �� �ѹ� �ؾ��� ��
    public virtual void ManagerDestroy() { }
}
