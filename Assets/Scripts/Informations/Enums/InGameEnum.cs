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
        Custom,
        Default,
        FirstView,
        ThirdView,
        Length
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

        //Esc
        Esc,

        //��Ī ��ȯ
        Sight,

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

        T1,
        T2,
        T3,
        T4,
        T5,
        T6,
        T7,
        T8,
        T9,

        //���� �뵵
        Length
    }


    public enum FixedUIType
    {
        FixedUITest,
        PlayableInputUI,
        PlayableInputUIBlock,

        TitleUI
    }

    public enum FloatingUIType
    {
        FloatingUITest,

        InStageOption
    }

}