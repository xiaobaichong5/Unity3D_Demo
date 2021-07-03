using System;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
    [ExecuteInEditMode]
    public class MetaballCameraEffect : MonoBehaviour
    {
        /// 模糊迭代-越大的数字意味着越模糊。
        public int iterations = 3;

        /// 每次迭代的模糊扩散。较低的值
        /// 提供更好的模糊效果，但需要更多的迭代来
        /// 得到大的模糊。值通常介于0.5和1.0之间。
        public float blurSpread = 0.6f;


        // 模糊迭代着色器。
        // 基本上它只需要4个纹理样本和平均值。
        // 通过反复应用和分布样本位置
        // 我们得到了高斯模糊近似。

        public Shader blurShader = null;

        static Material m_Material = null;
        protected Material material
        {
            get
            {
                if (m_Material == null)
                {
                    m_Material = new Material(blurShader);
                    m_Material.hideFlags = HideFlags.DontSave;
                }
                return m_Material;
            }
        }

        public Material cutOutMaterial;

        public Camera bgCamera;


        RenderTexture bgTargetTexture;

        private void OnEnable()
        {
			if (Screen.width > 0 && Screen.height > 0) {
				bgTargetTexture = new RenderTexture (Screen.width, Screen.height, 16);
				bgCamera.targetTexture = bgTargetTexture;
			}
        }

        protected void OnDisable()
        {
            if (m_Material)
            {
                DestroyImmediate(m_Material);
            }
        }

        protected void Start()
        {
            // 禁用，如果我们不支持图像效果
            if (!SystemInfo.supportsImageEffects)
            {
                enabled = false;
                return;
            }
            // 如果着色器无法在用户图形卡上运行，请禁用
            if (!blurShader || !material.shader.isSupported)
            {
                enabled = false;
                return;
            }

            // 如果着色器无法在用户图形卡上运行，请禁用
            if (!cutOutMaterial.shader.isSupported)
            {
                enabled = false;
                return;
            }

        }

        // 执行一次模糊迭代。
        public void FourTapCone(RenderTexture source, RenderTexture dest, int iteration)
        {
            float off = 0.5f + iteration * blurSpread;
            Graphics.BlitMultiTap(source, dest, material,
                                   new Vector2(-off, -off),
                                   new Vector2(-off, off),
                                   new Vector2(off, off),
                                   new Vector2(off, -off)
                );
        }

        // 将纹理降采样到四分之一分辨率。
        private void DownSample4x(RenderTexture source, RenderTexture dest)
        {
            float off = 1.0f;
            Graphics.BlitMultiTap(source, dest, material,
                                   new Vector2(-off, -off),
                                   new Vector2(-off, off),
                                   new Vector2(off, off),
                                   new Vector2(off, -off)
                );
        }


        // 由相机调用以应用图像效果
        RenderTexture buffer;
		RenderTexture buffer3;
		RenderTexture buffer2;
        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
			//if(!Application.isPlaying)
			//	return;

            int rtW = source.width / 4;
            int rtH = source.height / 4;
            buffer = RenderTexture.GetTemporary(rtW, rtH, 0);
			// Copy source to the 4x4 smaller texture.
			DownSample4x(source, buffer);


            // 模糊小纹理
            for (int i = 0; i < iterations; i++)
            {
					buffer2 = RenderTexture.GetTemporary (rtW, rtH, 0);
					FourTapCone (buffer, buffer2, i);
					RenderTexture.ReleaseTemporary (buffer);
					buffer = buffer2;
            }


     
            Graphics.Blit(bgTargetTexture, destination); // background
			Graphics.Blit(buffer, destination, cutOutMaterial); // water
            RenderTexture.ReleaseTemporary(buffer);


        }
    }
}
