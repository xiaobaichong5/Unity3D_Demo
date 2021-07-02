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
    public int iterations = 3;                   // 模糊迭代-越大的数字意味着越模糊。
    public float blurSpread = 0.6f;              // 每次迭代的模糊扩散。值越低，模糊效果越好。
    static Material m_Material = null;
    protected Material material { get { if (m_Material == null) { m_Material = new Material(blurShader) { hideFlags = HideFlags.DontSave }; } return m_Material; } }

    public Shader blurShader = null;             // “模糊迭代”（blur iteration）着色器仅获取4个纹理采样并对它们进行平均。
                                                 // 通过反复应用和分布样本位置
                                                 // 我们得到了高斯模糊近似。


    // 执行一次模糊迭代。
    public void FourTapCone(RenderTexture source, RenderTexture dest, int iteration)
    {
        float off = 0.5f + iteration * blurSpread;
        Graphics.BlitMultiTap(source, dest, material,
                               new Vector2(-off, -off),
                               new Vector2(-off, off),
                               new Vector2(off, off),
                               new Vector2(off, -off));
    }

    //将纹理降采样到四分之一分辨率。
    private void DownSample4x(RenderTexture source, RenderTexture dest)
    {
        float off = 1.0f;
        Graphics.BlitMultiTap(source, dest, material,
                               new Vector2(-off, -off),
                               new Vector2(-off, off),
                               new Vector2(off, off),
                               new Vector2(off, -off));
    }

    // 由相机调用以应用图像效果
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        int rtW = source.width / 4;
        int rtH = source.height / 4;
        RenderTexture buffer = RenderTexture.GetTemporary(rtW, rtH, 0);

        // 将源复制到4x4较小的纹理。
        DownSample4x(source, buffer);

        //模糊小纹理
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