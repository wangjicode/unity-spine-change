using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DancingWord.SpineChange {
    //记录合图集的状态和最终的新数据
    public class CombineTextureData {
        public int smallAtlasNumber;
        public bool isFinished = false;
        public Coroutine combineCoroutine;
        public Coroutine initRoleCoroutine;
        public Coroutine combineCoroutineBig;
        public Coroutine changeRoleCoroutine;
        public Coroutine combineCoroutineSmall;
        public CombineFinishedData resultData;
    }
    public class SpriteClass {
        public string name;
        public Sprite sprite;
    }
    public class TextureClass {
        public int index;
        public Rect rect;
        public RenderTexture texture;
        public TextureSize textureSize;
    }
    public class TextureSize {
        public int x;
        public int y;
    }
    //合并图集时的过程数据
    public class PackagedTextureResult {
        public RenderTexture renderTexture;
        public TextureClass[] textureInfo;
    }
    //合并完图集的返回数据
    public class CombineFinishedData {
        public Spine.Skin skin;
        public Material material;
        public RenderTexture currentRenderTex;
        public RenderTexture lastRenderTex;
        //临时先加在这里
        public Avatar self;
    }
}