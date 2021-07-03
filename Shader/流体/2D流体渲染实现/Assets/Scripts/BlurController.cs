///-----------------------------------------------------------------
///   Class:          BlurController
///   Description:    Created by Unity, edited by VC.
///   Author:         VueCode
///   GitHub:         https://github.com/ivuecode/
///-----------------------------------------------------------------
using UnityEngine;

[ExecuteInEditMode]
public class BlurController : MonoBehaviour
{
    [Header("Blue Settings")]
    public int iterations = 3;                   // ģ������-Խ���������ζ��Խģ����
    public float blurSpread = 0.6f;              // ÿ�ε�����ģ����ɢ��ֵԽ�ͣ�ģ��Ч��Խ�á�
    static Material m_Material = null;
    protected Material material { get { if (m_Material == null) { m_Material = new Material(blurShader) { hideFlags = HideFlags.DontSave }; } return m_Material; } }

    public Shader blurShader = null;             // ��ģ����������blur iteration����ɫ������ȡ4����������������ǽ���ƽ����
                                                 // ͨ������Ӧ�úͷֲ�����λ��
                                                 // ���ǵõ��˸�˹ģ�����ơ�


    // ִ��һ��ģ��������
    public void FourTapCone(RenderTexture source, RenderTexture dest, int iteration)
    {
        float off = 0.5f + iteration * blurSpread;
        Graphics.BlitMultiTap(source, dest, material,
                               new Vector2(-off, -off),
                               new Vector2(-off, off),
                               new Vector2(off, off),
                               new Vector2(off, -off));
    }

    //�������������ķ�֮һ�ֱ��ʡ�
    private void DownSample4x(RenderTexture source, RenderTexture dest)
    {
        float off = 1.0f;
        Graphics.BlitMultiTap(source, dest, material,
                               new Vector2(-off, -off),
                               new Vector2(-off, off),
                               new Vector2(off, off),
                               new Vector2(off, -off));
    }

    // �����������Ӧ��ͼ��Ч��
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        int rtW = source.width / 4;
        int rtH = source.height / 4;
        RenderTexture buffer = RenderTexture.GetTemporary(rtW, rtH, 0);

        // ��Դ���Ƶ�4x4��С������
        DownSample4x(source, buffer);

        //ģ��С����
        for (int i = 0; i < iterations; i++)
        {
            RenderTexture buffer2 = RenderTexture.GetTemporary(rtW, rtH, 0);
            FourTapCone(buffer, buffer2, i);
            RenderTexture.ReleaseTemporary(buffer);
            buffer = buffer2;
        }
        Graphics.Blit(buffer, destination);
        RenderTexture.ReleaseTemporary(buffer);
    }
}