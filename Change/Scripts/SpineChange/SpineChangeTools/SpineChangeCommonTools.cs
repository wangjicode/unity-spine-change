using System.Collections;
using System.Collections.Generic;
using Spine;
using Spine.Unity.Modules.AttachmentTools;
using UnityEngine;

namespace DancingWord.SpineChange {
    public static class SpineChangeCommonTools {
        /// <summary>
        /// 通过Sprite创建Texture2D
        /// </summary>
        /// <returns>The texture by sprite.</returns>
        /// <param name="sprite">Sprite.</param>
        /// <param name="apply">If set to <c>true</c> apply.</param>
        /// <param name="textureFormat">Texture format.</param>
        /// <param name="mipMaps">If set to <c>true</c> mip maps.</param>
        public static Texture2D GetTextureBySprite (Sprite sprite, bool apply = true, TextureFormat textureFormat = TextureFormat.RGBA32, bool mipMaps = false) {
            Texture2D texture2D = sprite.texture;
            Rect rect = sprite.textureRect;
            Color[] spritePixels = texture2D.GetPixels ((int) rect.x, (int) rect.y, (int) rect.width, (int) rect.height);
            Texture2D newTexture = new Texture2D ((int) rect.width, (int) rect.height, textureFormat, mipMaps);
            newTexture.SetPixels (spritePixels);
            if (apply) {
                newTexture.Apply ();
            }
            return newTexture;
        }
        /// <summary>
        /// 通过Texture2D克隆Texture2D
        /// </summary>
        /// <returns>The clone by texture.</returns>
        /// <param name="texture">Texture.</param>
        /// <param name="apply">If set to <c>true</c> apply.</param>
        /// <param name="textureFormat">Texture format.</param>
        /// <param name="mipMaps">If set to <c>true</c> mip maps.</param>
        public static Texture2D GetCloneByTexture (Texture2D texture, bool apply = true, TextureFormat textureFormat = TextureFormat.RGBA32, bool mipMaps = false) {
            Color[] spritePixels = texture.GetPixels (0, 0, texture.width, texture.height);
            Texture2D newTexture = new Texture2D (texture.width, texture.height, textureFormat, mipMaps);
            newTexture.SetPixels (spritePixels);
            if (apply) {
                newTexture.Apply ();
            }
            return newTexture;
        }
        /// <summary>
        /// 通过AtlasRegion创建Sprite
        /// </summary>
        /// <returns>The sprite by atlas region.</returns>
        /// <param name="atlasRegion">Atlas region.</param>
        /// <param name="pixelsPerUnit">Pixels per unit.</param>
        public static Sprite GetSpriteByAtlasRegion (AtlasRegion atlasRegion, float pixelsPerUnit = 100) {
            return Sprite.Create (GetMainTexture (atlasRegion), GetUnityRect (atlasRegion), new Vector2 (0.5f, 0.5f), pixelsPerUnit);
        }
        /// <summary>
        /// 通过AtlasRegion创建Texture2D
        /// </summary>
        /// <returns>The texture by atlas region.</returns>
        /// <param name="atlasRegion">Atlas region.</param>
        /// <param name="apply">If set to <c>true</c> apply.</param>
        /// <param name="textureFormat">Texture format.</param>
        /// <param name="mipMaps">If set to <c>true</c> mip maps.</param>
        public static Texture2D GetTextureByAtlasRegion (AtlasRegion atlasRegion, bool apply = true, TextureFormat textureFormat = TextureFormat.RGBA32, bool mipMaps = false) {
            Texture2D outTexture;
            Texture2D source = GetMainTexture (atlasRegion);
            Rect rect = SpineUnityFlipRect (GetSpineAtlasRect (atlasRegion), source.height);
            int width = (int) rect.width;
            int height = (int) rect.height;
            outTexture = new Texture2D (width, height, textureFormat, mipMaps);
            outTexture.name = atlasRegion.name;
            Color[] pixelBuffer = source.GetPixels ((int) atlasRegion.x, (int) atlasRegion.y, width, height);
            outTexture.SetPixels (pixelBuffer);
            if (apply) {
                outTexture.Apply ();
            }
            return outTexture;
        }
        /// <summary>
        /// 获取AtlasRegion的Texture2D
        /// </summary>
        /// <returns>The main texture.</returns>
        /// <param name="atlasRegion">Atlas region.</param>
        public static Texture2D GetMainTexture (AtlasRegion atlasRegion) {
            Material material = (atlasRegion.page.rendererObject as Material);
            return material.mainTexture as Texture2D;
        }
        /// <summary>
        /// 获取AtlasRegion的Rect
        /// </summary>
        /// <returns>The unity rect.</returns>
        /// <param name="atlasRegion">Atlas region.</param>
        public static Rect GetUnityRect (AtlasRegion atlasRegion) {
            return SpineUnityFlipRect (GetSpineAtlasRect (atlasRegion), atlasRegion.page.height);
        }
        public static Rect GetSpineAtlasRect (AtlasRegion region, bool includeRotate = true) {
            if (includeRotate && region.rotate) {
                return new Rect (region.x, region.y, region.height, region.width);
            } else {
                return new Rect (region.x, region.y, region.width, region.height);
            }
        }
        public static Rect SpineUnityFlipRect (Rect rect, int textureHeight) {
            rect.y = textureHeight - rect.y - rect.height;
            return rect;
        }
        /// <summary>
        /// 通过Texture2D创建AtlasRegion
        /// </summary>
        /// <returns>The atlas region by texture.</returns>
        /// <param name="texture">Texture.</param>
        /// <param name="shader">Shader.</param>
        /// <param name="scale">Scale.</param>
        /// <param name="material">Material.</param>
        public static AtlasRegion GetAtlasRegionByTexture (Texture2D texture, Shader shader, float scale, Material material = null) {
            Material tempMaterial = new Material (shader);
            if (material != null) {
                tempMaterial.CopyPropertiesFromMaterial (material);
                tempMaterial.shaderKeywords = material.shaderKeywords;
            }
            tempMaterial.mainTexture = texture;
            AtlasPage page = GetAtlasPageByMaterial (tempMaterial);
            float width = texture.width;
            float height = texture.height;
            AtlasRegion atlasRegion = new AtlasRegion ();
            atlasRegion.name = texture.name;
            atlasRegion.index = -1;
            atlasRegion.rotate = false;
            Vector2 boundsMin = Vector2.zero, boundsMax = new Vector2 (width, height) * scale;
            atlasRegion.width = (int) width;
            atlasRegion.originalWidth = (int) width;
            atlasRegion.height = (int) height;
            atlasRegion.originalHeight = (int) height;
            atlasRegion.offsetX = width * (0.5f - InverseLerp (boundsMin.x, boundsMax.x, 0));
            atlasRegion.offsetY = height * (0.5f - InverseLerp (boundsMin.y, boundsMax.y, 0));

            // Use the full area of the texture.
            atlasRegion.u = 0;
            atlasRegion.v = 1;
            atlasRegion.u2 = 1;
            atlasRegion.v2 = 0;
            atlasRegion.x = 0;
            atlasRegion.y = 0;

            atlasRegion.page = page;
            return atlasRegion;
        }
        /// <summary>
        /// 通过Material创建AtlasPage
        /// </summary>
        /// <returns>The atlas page by material.</returns>
        /// <param name="material">Material.</param>
        public static AtlasPage GetAtlasPageByMaterial (Material material) {
            AtlasPage page = new AtlasPage {
                rendererObject = material,
                name = material.name
            };
            Texture texture = material.mainTexture;
            if (texture != null) {
                page.width = texture.width;
                page.height = texture.height;
            }
            return page;
        }
        public static float InverseLerp (float a, float b, float value) {
            return (value - a) / (b - a);
        }
        /// <summary>
        /// UV坐标转Texture坐标
        /// </summary>
        /// <returns>The texture rect by UVR ect.</returns>
        /// <param name="uvRect">Uv rect.</param>
        /// <param name="texWidth">Tex width.</param>
        /// <param name="texHeight">Tex height.</param>
        public static Rect GetTextureRectByUVRect (Rect uvRect, int texWidth, int texHeight) {
            uvRect.x *= texWidth;
            uvRect.width *= texWidth;
            uvRect.y *= texHeight;
            uvRect.height *= texHeight;
            return uvRect;
        }
        /// <summary>
        /// RenderTexture转Texture2D
        /// </summary>
        /// <returns>The sprite by render texture.</returns>
        /// <param name="renderTexture">Render texture.</param>
        public static Texture2D GetTextureByRenderTexture (RenderTexture renderTexture, TextureFormat textureFormat = SpineConstValue.textureFormat) {
            Texture2D texture = new Texture2D (renderTexture.width, renderTexture.height, textureFormat, false);
            RenderTexture.active = renderTexture;
            texture.ReadPixels (new Rect (0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture.Apply ();
            RenderTexture.active = null;
            return texture;
        }
        public static void GetSpineRepackedSkin (CombineTextureData combineTextureData, Skin skin, string newName, Shader shader, int maxAtlasSize, TextureFormat textureFormat, bool mipMaps, Material materialPropertySource, bool clearCache) {
            if (skin == null) {
                throw new System.NullReferenceException ("Skin was null");
            }
            var skinAttachments = skin.Attachments;
            var newSkin = new Skin (newName);

            var existingRegions = new Dictionary<AtlasRegion, int> ();
            var regionIndexes = new List<int> ();
            var repackedAttachments = new List<Attachment> ();
            var texturesToPack = new List<Texture2D> ();
            var originalRegions = new List<AtlasRegion> ();
            int newRegionIndex = 0;
            foreach (var kvp in skinAttachments) {
                var newAttachment = kvp.Value.GetClone (true);
                if (IsRenderable (newAttachment)) {
                    var region = newAttachment.GetAtlasRegion ();
                    int existingIndex;
                    if (existingRegions.TryGetValue (region, out existingIndex)) {
                        regionIndexes.Add (existingIndex); // Store the region index for the eventual new attachment.
                    } else {
                        originalRegions.Add (region);
                        //texturesToPack.Add (region.ToTexture ()); // Add the texture to the PackTextures argument
                        existingRegions.Add (region, newRegionIndex); // Add the region to the dictionary of known regions
                        regionIndexes.Add (newRegionIndex); // Store the region index for the eventual new attachment.
                        newRegionIndex++;
                    }
                    repackedAttachments.Add (newAttachment);
                }
                var key = kvp.Key;
                newSkin.AddAttachment (key.slotIndex, key.name, newAttachment);
            }
            //合并图集
            if (null != combineTextureData.combineCoroutineSmall) {
                //region.ToTexture ()？？？
                GameManager.Instance.StopCoroutine (combineTextureData.combineCoroutineSmall);
            }
            combineTextureData.combineCoroutineSmall = GameManager.Instance.StartCoroutine (CombineTexturesTool.Instance.GetRenderTextureByTexturesOptimize (combineTextureData, originalRegions, maxAtlasSize, (PackagedTextureResult result) => {
                RenderTexture renderTexture = result.renderTexture;
                TextureClass[] outTextureClassList = result.textureInfo;
                var newMaterial = new Material (shader);
                if (materialPropertySource != null) {
                    newMaterial.CopyPropertiesFromMaterial (materialPropertySource);
                    newMaterial.shaderKeywords = materialPropertySource.shaderKeywords;
                }

                newMaterial.name = newName;
                newMaterial.mainTexture = renderTexture;
                var page = newMaterial.ToSpineAtlasPage ();
                page.name = newName;

                var repackedRegions = new List<AtlasRegion> ();
                for (int i = 0, n = originalRegions.Count; i < n; i++) {
                    var oldRegion = originalRegions[i];
                    var newRegion = UVRectToAtlasRegion (GetTargetRectByIndex (outTextureClassList, i), oldRegion.name, page, oldRegion.offsetX, oldRegion.offsetY, oldRegion.rotate);
                    repackedRegions.Add (newRegion);
                }

                for (int i = 0, n = repackedAttachments.Count; i < n; i++) {
                    var a = repackedAttachments[i];
                    a.SetRegion (repackedRegions[regionIndexes[i]]);
                }

                if (clearCache)
                    AtlasUtilities.ClearCache ();
                if (null != combineTextureData.resultData) {
                    combineTextureData.resultData.skin = null;
                    combineTextureData.resultData.material = null;
                    if(null != combineTextureData.resultData.currentRenderTex){
                        combineTextureData.resultData.lastRenderTex = combineTextureData.resultData.currentRenderTex;
                    }
                }
                combineTextureData.resultData.skin = newSkin;
                combineTextureData.resultData.material = newMaterial;
                combineTextureData.resultData.currentRenderTex = renderTexture;
                combineTextureData.isFinished = true;
            }));
        }
        public static AtlasRegion UVRectToAtlasRegion (Rect uvRect, string name, AtlasPage page, float offsetX, float offsetY, bool rotate) {
            var tr = GetTextureRectByUVRect (uvRect, page.width, page.height);
            var rr = SpineUnityFlipRect (tr, page.height);

            int x = (int) rr.x, y = (int) rr.y;
            int w, h;
            if (rotate) {
                w = (int) rr.height;
                h = (int) rr.width;
            } else {
                w = (int) rr.width;
                h = (int) rr.height;
            }

            return new AtlasRegion {
                page = page,
                    name = name,

                    u = uvRect.xMin,
                    u2 = uvRect.xMax,
                    v = uvRect.yMax,
                    v2 = uvRect.yMin,

                    index = -1,

                    width = w,
                    originalWidth = w,
                    height = h,
                    originalHeight = h,
                    offsetX = offsetX,
                    offsetY = offsetY,
                    x = x,
                    y = y,

                    rotate = rotate
            };
        }
        public static bool IsRenderable (Attachment a) {
            return a is RegionAttachment || a is MeshAttachment;
        }
        public static AtlasRegion GetAtlasRegion (this Attachment a) {
            var regionAttachment = a as RegionAttachment;
            if (regionAttachment != null)
                return (regionAttachment.RendererObject) as AtlasRegion;

            var meshAttachment = a as MeshAttachment;
            if (meshAttachment != null)
                return (meshAttachment.RendererObject) as AtlasRegion;

            return null;
        }
        /// <summary>
        /// 从集合中拿出一个TextureClass对象
        /// </summary>
        /// <returns>The target rect by index.</returns>
        /// <param name="list">List.</param>
        /// <param name="index">Index.</param>
        public static Rect GetTargetRectByIndex (TextureClass[] list, int index) {
            for (int i = 0; i < list.Length; i++) {
                if (index == list[i].index) {
                    return list[i].rect;
                }
            }
            return Rect.zero;
        }
    }
}