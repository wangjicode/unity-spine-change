using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DancingWord.SpineChange {
    public class AtlasPackageTools {
        /// <summary>
        /// 打包图集
        /// </summary>
        /// <returns>The package.</returns>
        /// <param name="textureName">Texture name.</param>
        /// <param name="textures">Textures.</param>
        /// <param name="padding">Padding.</param>
        /// <param name="atlasSize">Atlas size.</param>
        /// <param name="textureFormat">Texture format.</param>
        /// <param name="mipMaps">If set to <c>true</c> mip maps.</param>
        /// <param name="isReadEnable">If set to <c>true</c> is read enable.</param>
        /// <param name="atlasTextures">Atlas textures.</param>
        public static Dictionary<string, Rect> TexturesPackage (string textureName, List<Texture2D> textures, int padding, int atlasSize, TextureFormat textureFormat, bool mipMaps, bool isReadEnable, out Texture2D atlasTextures) {
            Dictionary<string, Rect> spriteNameRectDic = new Dictionary<string, Rect> ();
            Texture2D texture = new Texture2D (atlasSize, atlasSize, textureFormat, mipMaps);
            texture.name = textureName;
            Rect[] rects = texture.PackTextures (textures.ToArray (), padding, atlasSize, isReadEnable);
            for (int i = 0; i < rects.Length; i++) {
                spriteNameRectDic.Add (textures[i].name, rects[i]);
            }
            atlasTextures = texture;
            return spriteNameRectDic;
        }
        /// <summary>
        /// 拆解图集
        /// </summary>
        /// <returns>The textures sprite dic.</returns>
        /// <param name="texture">Texture.</param>
        /// <param name="rectDic">Rect dic.</param>
        public static Dictionary<string, Sprite> GetTexturesSpriteDic (Texture2D texture, Dictionary<string, Rect> rectDic) {
            Rect rect;
            Sprite sprite;
            Dictionary<string, Sprite> spriteNameDic = new Dictionary<string, Sprite> ();
            foreach (KeyValuePair<string, Rect> keys in rectDic) {
                rect = SpineChangeCommonTools.GetTextureRectByUVRect (keys.Value, texture.width, texture.height);
                sprite = Sprite.Create (texture, rect, Vector2.zero);
                spriteNameDic.Add (keys.Key, sprite);
            }
            return spriteNameDic;
        }
        /// <summary>
        /// 字典转list集合
        /// </summary>
        /// <returns>The sprite class list.</returns>
        /// <param name="dic">Dic.</param>
        public static List<SpriteClass> GetSpriteClassList (Dictionary<string, Sprite> dic) {
            List<SpriteClass> list = new List<SpriteClass> ();
            foreach (KeyValuePair<string, Sprite> keys in dic) {
                list.Add (new SpriteClass {
                    name = keys.Key,
                        sprite = keys.Value
                });
            }
            return list;
        }
    }
}