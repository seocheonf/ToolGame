namespace ToolGame
{

    public enum ForceType
    {
        VelocityForce,
        ImpulseForce,
        DurationForce,
        UnityDuration
    }

    public enum CrowdControlState
    {
        ElectricShcok,
        Stun,
        Length
    }

    public enum GeneralState
    {
        Normal,
        CrowdControl,
        Action,
        Length
    }

    public enum CameraViewType
    {
        Default,
        FirstView,
        ThirdView
    }

    public enum OuterKeyCode
    {
        
        #region About Playable
        /// <summary>
        /// �÷��̾��� ���õ� �Է�. �׻� ���� �ִٰ� �����ؾ� ��.
        /// </summary>
        
        //��
        Forward,
        //��
        Backward,
        //��(ward���� ������ �ܼ� �����¿�� ���������� ���ϱ� ����)
        Leftward,
        //��
        Rightward,

        //����
        Jump,
        //���
        Dash,
        //�ɱ�
        Sit,
        //����
        Rush,
        //���� ���ϱ� (���, ����)
        TakeTool,
        //�ܺ� ��ȣ�ۿ�
        OuterFunc,

        #endregion

        #region About Tool

        /// <summary>
        /// ������ ���õ� �Է�. �����ִ���, �ƴ��� ��Ȳ�� ���� �ľ��ؾ� ��.
        /// </summary>

        //������
        Reverse,
        //���/������/�ֵθ���/��ġ�� ��� ���� �׼Ǽ��� ���� �͵�
        Action,
        //��-�� ȸ����
        Rot_Forward_Left,
        //��-�� ȸ����
        Rot_Backward_Right,

        #endregion

        //���̿�
        Length
    }

}