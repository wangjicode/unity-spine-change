using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DancingWord.SpineChange {
    public class SpineConstValue {
        public const float scale = 0.01f;
        public const int TrackIndex = 0;
        public const int atlasPadding = 2;
        //小图集的大小
        public const int smallAtlasSize = 1024;
        //大图集的最大尺寸
        public const int atlasMaxSize = 4096;
        //Type长度
        public const int splitTypeLength = 3;
        //Id开始的索引
        public const int splitIdStartIndex = 2;
        //Id的长度
        public const int splitIdLength = 6;
        public const string rootKey = "skins";
        public const bool atlasMipMaps = false;
        public const bool atlasClearCache = false;
        public const string defaultSkinName = "default";
        public const string NewSkinname = "GPURepackedSkin";
        public const string shaderPath = "Sprites/Default";//Sprites/Default
        /// <summary>
        /// IOS设置成RGBA PVRTC 4。(非运行时)
        /// 安卓平台设置成RGBA 16。(非运行时)
        /// </summary>
        public const TextureFormat textureFormat = TextureFormat.RGBA32;
        public const string combineTextureShaderPath = "DW/Avatar/AvatarSpineShader";
        public const string clipTextureShaderPath = "DW/Avatar/AvatarSpineShader_Clip";
    }
}