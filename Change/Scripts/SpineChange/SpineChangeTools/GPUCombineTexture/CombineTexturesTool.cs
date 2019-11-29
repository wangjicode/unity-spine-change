using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DancingWord.SpineChange {
    public class CombineTexturesTool {
        private CombineTexturesTool () { }
        //图集的个数
        //private int index;
        /// <summary>
        /// 开始坐标
        /// </summary>
        private int endIndex;
        /// <summary>
        /// 结束坐标
        /// </summary>
        private int beginIndex;
        /// <summary>
        /// 图集的最大尺寸
        /// </summary>
        private int currentAtlasSize;
        /// <summary>
        /// 当前行所有Texture2D的总宽度
        /// </summary>
        private float currentAllWidth;
        /// <summary>
        /// 当前所有行里面最高的Texture2D的高度之和
        /// </summary>
        private float currentAllHeight;
        /// <summary>
        /// 散图
        /// </summary>
        //private List<TextureClass> textureList;
        //private List<TextureClass> currentTextureList;
        /// <summary>
        /// 所有行中最高的Texture2D高的集合
        /// </summary>
        public List<float> lineHeightList = new List<float> ();

        private static CombineTexturesTool _instance;
        public static CombineTexturesTool Instance {
            get {
                if (_instance == null) {
                    _instance = new CombineTexturesTool ();
                }
                return _instance;
            }
        }
        /// <summary>
        /// 将散图打包成图集并带出Rect集合(弃用)
        /// </summary>
        /// <returns>The render texture by textures.</returns>
        /// <param name="textures">Textures.</param>
        /// <param name="renderTextureSize">Render texture size.</param>
        /// <param name="textureClassList">textureClassList.</param>
        // public RenderTexture GetRenderTextureByTextures (List<Texture2D> textures, int renderTextureSize, out List<TextureClass> textureClassList) {
        //     //ResetValue ();
        //     currentAtlasSize = renderTextureSize;
        //     textureList = GetTextureClassList (textures);
        //     //排序(弃用)
        //     //textureList = SortList(GetTextureClassList(textures));
        //     currentTextureList = new List<TextureClass> ();
        //     Material material = InitMaterial ();
        //     MaxRectsBinPack maxRectsBinPack = new MaxRectsBinPack (currentAtlasSize, currentAtlasSize, false);
        //     RenderTexture render = RenderTexture.GetTemporary (currentAtlasSize, currentAtlasSize);
        //     Graphics.Blit (null, null, material, 0);
        //     for (int i = 0; i < textureList.Count; i++) {
        //         //打包算法(弃用)
        //         //Rect value = GetTextureScaleAndOffset(renderTextureSize, textureList[i].texture.width, textureList[i].texture.height);
        //         Rect posRect = maxRectsBinPack.Insert (textureList[i].texture.width, textureList[i].texture.height, MaxRectsBinPack.FreeRectChoiceHeuristic.RectBestAreaFit);
        //         Rect scaleRect = GetScaleRect (posRect, renderTextureSize, renderTextureSize);
        //         material.SetTexture ("_MainTex", textureList[i].texture);
        //         material.SetFloat ("_ScaleX", scaleRect.width);
        //         material.SetFloat ("_ScaleY", scaleRect.height);
        //         material.SetFloat ("_OffsetX", scaleRect.x);
        //         material.SetFloat ("_OffsetY", scaleRect.y);
        //         Graphics.Blit (null, render, material, 1);
        //         textureList[i].rect = GetUVRect (scaleRect, textureList[i].texture, renderTextureSize, renderTextureSize);
        //         currentTextureList.Add (textureList[i]);
        //     }
        //     Graphics.Blit (null, null, material, 0);
        //     textureClassList = currentTextureList;
        //     return render;
        // }
        // private Dictionary<int, TextureSize[]> textureSizes = new Dictionary<int, TextureSize[]> {
        //     {
        //     512,
        //     new TextureSize[] {
        //     new TextureSize { x = 512, y = 512 },
        //     new TextureSize { x = 1024, y = 512 },
        //     new TextureSize { x = 1536, y = 512 },
        //     new TextureSize { x = 2048, y = 512 },
        //     new TextureSize { x = 2048, y = 1024 },
        //     new TextureSize { x = 2048, y = 1024 },
        //     new TextureSize { x = 2048, y = 1024 },
        //     new TextureSize { x = 2048, y = 1024 },
        //     new TextureSize { x = 2048, y = 1536 },
        //     new TextureSize { x = 2048, y = 1536 },
        //     new TextureSize { x = 2048, y = 1536 },
        //     new TextureSize { x = 2048, y = 1536 },
        //     new TextureSize { x = 2048, y = 2048 },
        //     new TextureSize { x = 2048, y = 2048 },
        //     new TextureSize { x = 2048, y = 2048 },
        //     new TextureSize { x = 2048, y = 2048 }
        //     }
        //     },
        //     {
        //     1024,
        //     new TextureSize[] {
        //     new TextureSize { x = 1024, y = 1024 },
        //     new TextureSize { x = 2048, y = 1024 },
        //     new TextureSize { x = 2048, y = 2048 },
        //     new TextureSize { x = 2048, y = 2048 }
        //     }
        //     },
        // };
        private TextureSize GetAtlasSize (int smallAtlasNumber) {
            int lineNumber = SpineConstValue.atlasMaxSize / SpineConstValue.smallAtlasSize;
            if (smallAtlasNumber >= (lineNumber * lineNumber)) {
                throw new System.NotSupportedException ("[--图集超出 4096*4096 ,请修改散图的大小--]");
            }
            int lines = smallAtlasNumber / lineNumber;
            int x = 0, y = 0;
            if (lines <= 0) {
                x = (smallAtlasNumber + 1) * SpineConstValue.smallAtlasSize;
                y = SpineConstValue.smallAtlasSize;
            } else {
                x = lineNumber * SpineConstValue.smallAtlasSize;
                y = (lines + 1) * SpineConstValue.smallAtlasSize;
            }
            return new TextureSize {
                x = x,
                y = y
            };
        }
        private bool IsSuitableTexture (RenderTexture tex) {
            if (tex.width > currentAtlasSize || tex.height > currentAtlasSize) {
                return false;
            }
            return true;
        }
        public IEnumerator GetRenderTextureByTexturesOptimize (CombineTextureData combineTextureData, List<Spine.AtlasRegion> regions, int renderTextureSize, Action<PackagedTextureResult> finishCallBack) {
            ResetValue (combineTextureData);
            currentAtlasSize = renderTextureSize;
            List<TextureClass> textureList = new List<TextureClass>();
            //GetTextureClassList (textures);
            List<TextureClass> currentTextureList = new List<TextureClass> ();
            Material material = InitMaterial ();
            MaxRectsBinPack maxRectsBinPack = new MaxRectsBinPack (currentAtlasSize, currentAtlasSize, false);
            RenderTexture render = RenderTexture.GetTemporary (currentAtlasSize, currentAtlasSize,0,RenderTextureFormat.ARGB4444);
            Dictionary<RenderTexture, TextureClass[]> dic = new Dictionary<RenderTexture, TextureClass[]> ();
            Graphics.Blit (null, null, material, 0);
            for (int i = 0; i < regions.Count; i++) {
                UnityEngine.Profiling.Profiler.BeginSample("SpineChange_ToTexture");
                RenderTexture regionTex = Spine.Unity.Modules.AttachmentTools.AtlasUtilities.ToTexture(regions[i]);
                UnityEngine.Profiling.Profiler.EndSample();
                textureList.Add(new TextureClass{index = i,texture = regionTex,textureSize = new TextureSize{x = regionTex.width,y = regionTex.height}});
                if (!IsSuitableTexture (textureList[i].texture)) {
                    throw new System.NotSupportedException ("[--散图 " + textureList[i].texture.name + " 的大小超出单位图集的大小,请修改散图或单位图集的大小(SpineConstValue.smallAtlasSize)--]");
                }
                Rect posRect = maxRectsBinPack.Insert (textureList[i].texture.width, textureList[i].texture.height, MaxRectsBinPack.FreeRectChoiceHeuristic.RectBestAreaFit);
                if (posRect.height <= 0) {
                    //保存本次的图集和散图的位置信息
                    TextureClass[] array1 = new TextureClass[currentTextureList.Count];
                    currentTextureList.CopyTo (array1);
                    dic.Add (render, array1);
                    currentTextureList.Clear ();
                    Graphics.Blit (null, null, material, 0);
                    //初始化参数进行下一次的合图
                    material = InitMaterial ();
                    maxRectsBinPack = new MaxRectsBinPack (currentAtlasSize, currentAtlasSize, false);
                    render = RenderTexture.GetTemporary (currentAtlasSize, currentAtlasSize,0,RenderTextureFormat.ARGB4444);
                    Graphics.Blit (null, null, material, 0);
                    textureList.RemoveAt(i);
                    --i;
                    combineTextureData.smallAtlasNumber++;
                    continue;
                }
                Rect scaleRect = GetScaleRect (posRect, renderTextureSize, renderTextureSize);
                material.SetTexture ("_MainTex", textureList[i].texture);
                material.SetFloat ("_ScaleX", scaleRect.width);
                material.SetFloat ("_ScaleY", scaleRect.height);
                material.SetFloat ("_OffsetX", scaleRect.x);
                material.SetFloat ("_OffsetY", scaleRect.y);
                Graphics.Blit (null, render, material, 1);
                textureList[i].rect = posRect;
                currentTextureList.Add (textureList[i]);
                //卸载散图资源
                // textureList[i].texture = null;
                // UnityEngine.Resources.UnloadUnusedAssets();
                RenderTexture.ReleaseTemporary(textureList[i].texture);
                textureList[i].texture.Release();
                textureList[i].texture = null;
                yield return null;
            }
            Graphics.Blit (null, null, material, 0);
            TextureClass[] array2 = new TextureClass[currentTextureList.Count];
            currentTextureList.CopyTo (array2);
            dic.Add (render, array2);
            if(null != combineTextureData.combineCoroutineBig){
                GameManager.Instance.StopCoroutine(combineTextureData.combineCoroutineBig);
            }
            combineTextureData.combineCoroutineBig = GameManager.Instance.StartCoroutine (FinallyPackage (combineTextureData, dic, finishCallBack));
        }
        //合并小的图集为一张大图
        private IEnumerator FinallyPackage (CombineTextureData combineTextureData, Dictionary<RenderTexture, TextureClass[]> dic, Action<PackagedTextureResult> finishCallBack) {
            PackagedTextureResult result = new PackagedTextureResult ();
            TextureSize size = GetAtlasSize (combineTextureData.smallAtlasNumber);
            MaxRectsBinPack maxRectsBinPack = new MaxRectsBinPack (size.x, size.y, false);
            Material material = InitMaterial ();
            RenderTexture render = RenderTexture.GetTemporary (size.x, size.y,0,RenderTextureFormat.ARGB4444);
            List<TextureClass> tc = new List<TextureClass> ();
            Graphics.Blit (null, null, material, 0);
            foreach (KeyValuePair<RenderTexture, TextureClass[]> keys in dic) {
                RenderTexture tempTexture = keys.Key;
                Rect rect = maxRectsBinPack.Insert (tempTexture.width, tempTexture.height, MaxRectsBinPack.FreeRectChoiceHeuristic.RectBestAreaFit);
                //重新计算位置
                for (int i = 0; i < keys.Value.Length; i++) {
                    keys.Value[i].rect = new Rect (rect.x + keys.Value[i].rect.x, rect.y + keys.Value[i].rect.y, keys.Value[i].rect.width, keys.Value[i].rect.height);
                    tc.Add (keys.Value[i]);
                }
                Rect scaleRect = GetScaleRect (rect, size.x, size.y);
                material.SetTexture ("_MainTex", tempTexture);
                material.SetFloat ("_ScaleX", scaleRect.width);
                material.SetFloat ("_ScaleY", scaleRect.height);
                material.SetFloat ("_OffsetX", scaleRect.x);
                material.SetFloat ("_OffsetY", scaleRect.y);
                Graphics.Blit (null, render, material, 1);
                //归还小图集RenderTexture
                RenderTexture.ReleaseTemporary(tempTexture);
                tempTexture.Release();
                tempTexture = null;
                yield return null;
            }
            Graphics.Blit (null, null, material, 0);
            for (int i = 0; i < tc.Count; i++) {
                tc[i].rect = GetUVRect (GetScaleRect (tc[i].rect, size.x, size.y), tc[i].textureSize, size.x, size.y);
            }
            result.renderTexture = render;
            result.textureInfo = tc.ToArray ();
            if (null != finishCallBack) {
                finishCallBack.Invoke (result);
            }
        }
        private Rect GetScaleRect (Rect rect, int width, int height) {
            float offsetX = ((-1) * rect.x) / rect.width;
            float offsetY = ((-1) * rect.y) / rect.height;
            float scaleX = (width * 1.0f) / rect.width;
            float scaleY = (height * 1.0f) / rect.height;
            Rect tempRect = new Rect (offsetX, offsetY, scaleX, scaleY);
            return tempRect;
        }
        private List<TextureClass> GetTextureClassList (List<RenderTexture> textures) {
            List<TextureClass> temp = new List<TextureClass> ();
            for (int i = 0; i < textures.Count; i++) {
               TextureSize tempTextureSize = new TextureSize();
               tempTextureSize.x = textures[i].width;
               tempTextureSize.y = textures[i].height;
                temp.Add (new TextureClass { index = i, texture = textures[i],textureSize = tempTextureSize });
            }
            return temp;
        }
        /// <summary>
        /// 初始化材质
        /// </summary>
        /// <returns>The material.</returns>
        private Material InitMaterial () {
            return new Material (Shader.Find (SpineConstValue.combineTextureShaderPath));
        }
        /// <summary>
        /// 重置
        /// </summary>
        private void ResetValue (CombineTextureData combineTextureData) {
            endIndex = 0;
            beginIndex = 0;
            currentAllWidth = 0;
            currentAllHeight = 0;
            lineHeightList = new List<float> ();
            combineTextureData.smallAtlasNumber = 0;
        }
        /// <summary>
        /// 设置图集中散图的位置和缩放
        /// </summary>
        /// <returns>The texture scale and offset.</returns>
        /// <param name="renderTextureSize">Render texture size.</param>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        // private Rect GetTextureScaleAndOffset (int renderTextureSize, float width, float height) {
        //     //Debug.LogError(renderTextureSize + "---" + width + "---" + height);
        //     //合并算法
        //     if (currentTextureList.Count == 0) {
        //         return new Rect (0f, 0f, (float) renderTextureSize / width, (float) renderTextureSize / height);
        //     }
        //     endIndex = currentTextureList.Count - 1;
        //     currentAllWidth = GetAllWidth (beginIndex, endIndex);
        //     if (currentAllWidth + width > renderTextureSize) {
        //         currentAllHeight = GetMaxHeight (beginIndex, endIndex);
        //         beginIndex = endIndex + 1;
        //         currentAllWidth = GetAllWidth (beginIndex, endIndex);
        //     }
        //     return new Rect ((-1) * (currentAllWidth / width), (-1) * currentAllHeight / height, renderTextureSize / width, renderTextureSize / height);
        // }
        /// <summary>
        /// 计算当前行中散图的宽度之和
        /// </summary>
        /// <returns>The all width.</returns>
        /// <param name="bIndex">B index.</param>
        /// <param name="eIndex">E index.</param>
        // private float GetAllWidth (int bIndex, int eIndex) {
        //     float allWidth = 0;
        //     for (int i = bIndex; i <= eIndex; i++) {
        //         allWidth += textureList[i].texture.width;
        //     }
        //     return allWidth;
        // }
        /// <summary>
        /// 计算当前所有行中最高的散图的高度之和
        /// </summary>
        /// <returns>The max height.</returns>
        /// <param name="bIndex">B index.</param>
        /// <param name="eIndex">E index.</param>
        // private float GetMaxHeight (int bIndex, int eIndex) {
        //     float maxHeight = int.MinValue * 1.0f;
        //     for (int i = bIndex; i <= eIndex; i++) {
        //         if (maxHeight < textureList[i].texture.height) {
        //             maxHeight = textureList[i].texture.height;
        //         }
        //     }
        //     lineHeightList.Add (maxHeight);
        //     float allheight = 0;
        //     for (int j = 0; j < lineHeightList.Count; j++) {
        //         allheight += lineHeightList[j];
        //     }
        //     return allheight;
        // }
        /// <summary>
        /// 给散图排序
        /// </summary>
        /// <returns>The list.</returns>
        /// <param name="textures">Textures.</param>
        private List<TextureClass> SortList (List<TextureClass> textures) {
            List<TextureClass> tempList = textures;
            for (int i = 0; i < tempList.Count - 1; i++) {
                for (int j = 0; j < tempList.Count - 1 - i; j++) {
                    if (GetAreaByTexture (tempList[j].texture) > GetAreaByTexture (tempList[j + 1].texture)) {
                        TextureClass tempTexture = tempList[j];
                        tempList[j] = tempList[j + 1];
                        tempList[j + 1] = tempTexture;
                    }
                }
            }
            return tempList;
        }
        /// <summary>
        /// 单个散图的面积
        /// </summary>
        /// <returns>The area by texture.</returns>
        /// <param name="texture">Texture.</param>
        private float GetAreaByTexture (RenderTexture texture) {
            return texture.width * texture.height;
        }
        /// <summary>
        /// 获取图集中散图的大小和位置信息
        /// </summary>
        /// <returns>The UVR ect.</returns>
        /// <param name="rect">Rect.</param>
        /// <param name="texture">Texture.</param>
        private Rect GetUVRect (Rect rect, TextureSize texture, int atlasWidth, int atlasHeight) {
            float x = (((-1) * rect.x) * texture.x) * 1.0f / atlasWidth;
            float y = (((-1) * rect.y) * texture.y) * 1.0f / atlasHeight;
            float width = (float) texture.x / atlasWidth;
            float height = (float) texture.y / atlasHeight;

            float xMin = x;
            float yMin = y;
            float xMax = x + width;
            float yMax = y + height;
            return new Rect (x, y, width, height) {
                xMin = xMin,
                    yMin = yMin,
                    xMax = xMax,
                    yMax = yMax
            };
        }
    }
}