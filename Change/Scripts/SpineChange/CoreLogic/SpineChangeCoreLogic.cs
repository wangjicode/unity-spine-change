using System;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using Spine.Unity.Modules.AttachmentTools;
using UnityEngine;
namespace DancingWord.SpineChange {
    public class SpineChangeCoreLogic {

        private UnityEngine.Object roleObject;
        private SkeletonAnimation skeletonAnimation;
        private RoleEquipmentData equipmentData;
        private Dictionary<int, List<string>> slotAttachmentDic;

        private static SpineChangeCoreLogic _instance;
        public static SpineChangeCoreLogic Instance {
            get {
                if (_instance == null) {
                    _instance = new SpineChangeCoreLogic ();
                }
                return _instance;
            }
        }
        private SpineChangeCoreLogic () { }
        #region 换装相关
        /// <summary>
        /// 通过Texture2D重建Spine的AtlasRegion。
        /// </summary>
        /// <returns>AtlasRegion.</returns>
        /// <param name="texture2D">Texture2D.</param>
        public AtlasRegion CreateAtlasRegionFromTexture (Texture2D texture2D) {
            AtlasRegion region = new AtlasRegion {
                width = texture2D.width,
                height = texture2D.height,
                originalWidth = texture2D.width,
                originalHeight = texture2D.height,
                rotate = false,
                page = new AtlasPage ()
            };
            region.page.name = texture2D.name;
            region.page.width = texture2D.width;
            region.page.height = texture2D.height;
            region.page.uWrap = TextureWrap.ClampToEdge;
            region.page.vWrap = TextureWrap.ClampToEdge;
            return region;
        }
        /// <summary>
        /// 通过AtlasRegion设置卡槽上的Attachment
        /// </summary>
        /// <returns><c>true</c>, if region attachment by atlas region was set, <c>false</c> otherwise.</returns>
        /// <param name="slotStr">Slot string.</param>
        /// <param name="atlasRegion">Atlas region.</param>
        public bool SetRegionAttachmentByAtlasRegion (SkeletonAnimation skeletonAnim, string slotStr, string attachmentName, AtlasRegion atlasRegion) {
            Slot slot = skeletonAnim.skeleton.FindSlot (slotStr);
            if (slot == null && atlasRegion == null) {
                return false;
            }
            RegionAttachment attachment = SpineCreateAttachment.Instance.CreateRegionAttachmentByAtlasRegion (skeletonAnim, attachmentName, atlasRegion) as RegionAttachment;
            slot.Attachment = attachment;
            return false;
        }
        /// <summary>
        /// 通过AtlasRegion设置卡槽上的Attachment
        /// </summary>
        /// <returns><c>true</c>, if mesh attachment by atlas region was set, <c>false</c> otherwise.</returns>
        /// <param name="slotStr">Slot string.</param>
        /// <param name="atlasRegion">Atlas region.</param>
        public bool SetMeshAttachmentByAtlasRegion (SkeletonAnimation skeletonAnim, string slotStr, string attachmentName, AtlasRegion atlasRegion) {
            Slot slot = skeletonAnim.skeleton.FindSlot (slotStr);
            if (slot == null && atlasRegion == null) {
                return false;
            }
            MeshAttachment attachment = SpineCreateAttachment.Instance.CreateMeshAttachmentByAtlasRegion (skeletonAnim, attachmentName, atlasRegion) as MeshAttachment;
            slot.Attachment = attachment;
            return true;
        }
        /// <summary>
        /// 通过skeletonDataAsset创建角色
        /// </summary>
        /// <returns><c>true</c>, if spine role was created, <c>false</c> otherwise.</returns>
        /// <param name="skeletonDataAsset">Skeleton data asset.</param>
        /// <param name="animationName">Animation name.</param>
        /// <param name="skinName">Skin name.</param>
        /// <param name="pos">Position.</param>
        private System.Collections.IEnumerator Create (SpineAvatarType avatarType, EquipmentObject eo, SkeletonDataAsset skeletonDataAsset, string roleGameObjectName, string animationName, string skinName, bool loop, RoleEquipmentData data, Action<Avatar> createSucCallBack, Action<string> failCallBack, Action<bool> createQueueCallBack, bool isTP) {
            if (skeletonDataAsset == null) {
                if (failCallBack != null) {
                    failCallBack.Invoke ("<-创建角色失败->");
                }
                yield break;
            }
            GameObject roleSpine = new GameObject {
                name = roleGameObjectName
            };
            roleObject = roleSpine;
            skeletonAnimation = SkeletonAnimation.AddToGameObject (roleSpine, skeletonDataAsset);
            skeletonAnimation.Initialize (false);
            skeletonAnimation.Skeleton.SetSkin (skinName);
            skeletonAnimation.state.SetAnimation (SpineConstValue.TrackIndex, animationName, loop);
            CreateSlotAttachmentDic (skeletonAnimation.Skeleton.Data.DefaultSkin.Attachments);
            if (null == data) {
                equipmentData = new RoleEquipmentData ();
                equipmentData.RoleEquipmentItems = new List<RoleEquipmentItem> ();
            } else {
                equipmentData = data;
            }
            CombineTextureData combineTextureData = new CombineTextureData ();
            combineTextureData.resultData = new CombineFinishedData ();
            RoleAnaimationData rad = new RoleAnaimationData { roleJsonStr = eo.equipmentJsonAsset.text };
            Avatar avatar = new SpineAvatar (equipmentData, skeletonAnimation, slotAttachmentDic, rad, combineTextureData, avatarType, roleObject, eo.equipmentJsonAsset.text);
            combineTextureData.resultData.self = avatar;
            if (createSucCallBack != null) {
                if(null != roleSpine){
                    roleSpine.SetActive (false);
                    createSucCallBack.Invoke (avatar);
                }
            }
            //初始化角色的所有服装信息
            if (equipmentData.RoleEquipmentItems.Count > 0) {
                if(null != combineTextureData.initRoleCoroutine){
                    GameManager.Instance.StopCoroutine(combineTextureData.initRoleCoroutine);
                }
                combineTextureData.initRoleCoroutine = GameManager.Instance.StartCoroutine (InitRoleEquipment (rad, skeletonAnimation, equipmentData, combineTextureData, slotAttachmentDic, avatar.GetDestoryTextureList ,roleSpine.SetActive, null, isTP));
                yield return combineTextureData.initRoleCoroutine;
            } else {
                roleSpine.SetActive (true);
            }
            if (null != createQueueCallBack) {
                createQueueCallBack.Invoke (true);
            }
        }
        /// <summary>
        /// 创建slot对应的attachment的字典(一个slot有可能对应多个attachment)
        /// </summary>
        /// <param name="attachments">Attachments.</param>
        private void CreateSlotAttachmentDic (Dictionary<Skin.AttachmentKeyTuple, Attachment> attachments) {
            slotAttachmentDic = new Dictionary<int, List<string>> ();
            foreach (KeyValuePair<Skin.AttachmentKeyTuple, Attachment> keys in attachments) {
                if (slotAttachmentDic.ContainsKey (keys.Key.slotIndex)) {
                    slotAttachmentDic[keys.Key.slotIndex].Add (keys.Value.Name);
                } else {
                    List<string> list = new List<string> ();
                    list.Add (keys.Value.Name);
                    slotAttachmentDic.Add (keys.Key.slotIndex, list);
                }
            }
        }
        /// <summary>
        /// 通过slotindex和attachment的名称组合获取旧的attachment
        /// </summary>
        /// <returns>The attachment form role.</returns>
        /// <param name="allAttachmentName">All attachment name.</param>
        public Attachment GetAttachmentFormRole (SkeletonAnimation skeletonAnim, string allAttachmentName) {
            int key = SpineChangeDataLogic.Instance.GetSlotIndexFromAllAttachmentName (allAttachmentName);
            string value = SpineChangeDataLogic.Instance.GetAttachmentFromAllAttachmnetName (allAttachmentName);
            foreach (KeyValuePair<Skin.AttachmentKeyTuple, Attachment> keys in skeletonAnim.Skeleton.Data.DefaultSkin.Attachments) {
                if (keys.Key.slotIndex == key && keys.Value.Name == value) {
                    return keys.Value;
                }
            }
            return null;
        }
        /// <summary>
        /// 根据slotIndex获取slot的名字
        /// </summary>
        /// <returns>The slot name by slot index.</returns>
        /// <param name="slotIndex">Slot index.</param>
        public string GetSlotNameBySlotIndex (SkeletonAnimation skeletonAnim, int slotIndex) {
            if(null == skeletonAnim) return null;
            for (int i = 0; i < skeletonAnim.Skeleton.Data.Slots.Items.Length; i++) {
                if (skeletonAnim.Skeleton.Data.Slots.Items[i].Index == slotIndex) {
                    return skeletonAnim.Skeleton.Data.Slots.Items[i].Name;
                }
            }
            return null;
        }
        /// <summary>
        /// 从slot对应的attachment的字典中取出某一个attachment对应的slotindex集合
        /// </summary>
        /// <returns>The slot list for slot attachment dic.</returns>
        /// <param name="attachmentName">Attachment name.</param>
        public List<int> GetSlotListForSlotAttachmentDic (Dictionary<int, List<string>> tempSlotAttachmentDic, string attachmentName) {
            List<int> slotList = new List<int> ();
            foreach (KeyValuePair<int, List<string>> keys in tempSlotAttachmentDic) {
                if (keys.Value.Contains (attachmentName)) {
                    slotList.Add (keys.Key);
                }
            }
            return slotList;
        }
        /// <summary>
        /// 通过Spine生成的json，atlas文件创建角色
        /// </summary>
        /// <param name="skeletonJson">Skeleton json.</param>
        /// <param name="atlasText">Atlas text.</param>
        /// <param name="textures">Textures.</param>
        /// <param name="material">Material.</param>
        /// <param name="initialize">If set to <c>true</c> initialize.</param>
        /// <param name="skinName">Skin name.</param>
        /// <param name="animationName">Animation name.</param>
        /// <param name="loop">If set to <c>true</c> loop.</param>
        public void Create (SpineAvatarType avatarType, EquipmentObject eo, Material material, bool initialize, string name, string skinName, string animationName, bool loop, RoleEquipmentData data, Action<Avatar> createSucCallBack, Action<string> failCallBack, Action<bool> createQueueCallBack, bool isTP) {
            #if UNITY_ANDROID
            GameManager.Instance.StartCoroutine (Create (avatarType, eo, SkeletonDataAsset.CreateRuntimeInstance (eo.equipmentJsonAsset, AtlasAsset.CreateRuntimeInstance (eo.equipmentTxtAsset, eo.equipmentTexture_Alpha,eo.equipmentTexture_RGB, material, initialize,true), initialize), name, animationName, skinName, loop, data, createSucCallBack, failCallBack, createQueueCallBack, isTP));
            #elif UNITY_IPHONE || UNITY_EDITOR
            GameManager.Instance.StartCoroutine (Create (avatarType, eo, SkeletonDataAsset.CreateRuntimeInstance (eo.equipmentJsonAsset, AtlasAsset.CreateRuntimeInstance (eo.equipmentTxtAsset, new Texture2D[] { eo.equipmentTexture }, material, initialize), initialize), name, animationName, skinName, loop, data, createSucCallBack, failCallBack, createQueueCallBack, isTP));
            #endif
            
        }
        /// <summary>
        /// 初始化角色的插槽附件
        /// </summary>
        /// <returns><c>true</c>, if role slot attachment info was inited, <c>false</c> otherwise.</returns>
        public System.Collections.IEnumerator InitRoleEquipment (RoleAnaimationData animationData, SkeletonAnimation skeletonAnim, RoleEquipmentData tempData, CombineTextureData combineTextureData, Dictionary<int, List<string>> TempSlotAttachmentDic, List<Texture2D> destoryTextureList ,Action<bool> setRoleStatus, Action<string> failCallBack, bool isTP) {
            if (skeletonAnim == null) {
                if (null != failCallBack) {
                    failCallBack.Invoke ("<-初始化角色的插槽和附件信息失败->");
                }
                yield break;
            }
            System.DateTime time1 = System.DateTime.Now;
            for (int i = 0; i < tempData.RoleEquipmentItems.Count; i++) {
                // if(tempData.RoleEquipmentItems[i].EquipmentType == RoleEquipmentType.RoleHat || tempData.RoleEquipmentItems[i].EquipmentType == RoleEquipmentType.RoleGlasses)
                //     continue;
                EquipmentRes res = SpineChangeDataLogic.Instance.GetRoleEquipmentObject (tempData.RoleEquipmentItems[i], null);
                while (!res.isDone) {
                    yield return null;
                }
                combineTextureData.changeRoleCoroutine = GameManager.Instance.StartCoroutine (Change (animationData, skeletonAnim, combineTextureData, TempSlotAttachmentDic, res.GetRes (), isTP, destoryTextureList ,setRoleStatus, failCallBack, true));
                yield return combineTextureData.changeRoleCoroutine;
            }
            System.DateTime time2 = System.DateTime.Now;
            if (null != GameObject.FindObjectOfType<ChangeTest> ())
                GameObject.FindObjectOfType<ChangeTest> ().text.text += "创建角色并初始化用时：" + (time2 - time1).TotalMilliseconds + "\n";
            if(null != roleObject){
                setRoleStatus.Invoke (true);
            }
            if (isTP) {
                //需要合图集
                UpdateRoleEquipment (skeletonAnim, combineTextureData);
            } else {
                DestroyResources (combineTextureData);
            }
        }
        /// <summary>
        /// 换装
        /// </summary>
        /// <returns>The change.</returns>
        /// <param name="item">装备数据</param>
        public System.Collections.IEnumerator Change (RoleAnaimationData animationData, SkeletonAnimation skeletonAnim, CombineTextureData combineTextureData, Dictionary<int, List<string>> slotAttachmentDic, EquipmentObject eo, bool isTexturePackage, List<Texture2D> destoryTextureList ,Action<bool> setRoleStatus, Action<string> failCallBack, bool isCreateInit = false) {
            #if UNITY_ANDROID
            destoryTextureList.Add(eo.equipmentTexture_Alpha);
            destoryTextureList.Add(eo.equipmentTexture_RGB);
            #elif UNITY_IPHONE || UNITY_EDITOR
            destoryTextureList.Add(eo.equipmentTexture);
            #endif
            SpineReadJsonDataLogic.Instance.GetSpineJsonObjectDic (eo.equipmentJsonAsset);
            List<AtlasRegion> regions = SpineChangeDataLogic.Instance.GetEquipmentAtlasRegions (eo,SpineChangeDataLogic.Instance.GetEquipmentMaterial ());
            List<string> slotAttachmentNameList = new List<string> ();
            List<AtlasRegionData> sortData = SpineCreateAttachment.Instance.GetSortRegionList (skeletonAnim, regions, slotAttachmentDic);
            //更换皮肤数据里面的Attachment字典KeyValuePair<Skin.AttachmentKeyTuple, Attachment >(换成我们自己创建的)
            for (int i = 0; i < sortData.Count; i++) {
                // List<int> slotIndexList = GetSlotListForSlotAttachmentDic (slotAttachmentDic, regions[i].name);
                // for (int j = 0; j < slotIndexList.Count; j++) {
                //     Attachment tempattachment = GetAttachmentByType (skeletonAnim, SpineChangeDataLogic.Instance.GetAttachmentType (skeletonAnim, slotIndexList[j] + "|" + regions[i].name), slotIndexList[j] + "|" + regions[i].name, regions[i]);
                //     if (null == tempattachment) {
                //         yield return null;
                //     }
                //     skeletonAnim.Skeleton.Data.DefaultSkin.AddAttachment (slotIndexList[j], regions[i].name, tempattachment);
                //     slotAttachmentNameList.Add (SpineChangeCoreLogic.Instance.GetSlotNameBySlotIndex (skeletonAnim, slotIndexList[j]) + "$" + tempattachment.Name);
                // }
                Attachment tempattachment = GetAttachmentByType (skeletonAnim, sortData[i].type, sortData[i].allName, sortData[i].region);
                if (null == tempattachment) {
                    yield return null;
                }
                skeletonAnim.Skeleton.Data.DefaultSkin.AddAttachment (SpineChangeDataLogic.Instance.GetSlotIndexFromAllAttachmentName (sortData[i].allName), SpineChangeDataLogic.Instance.GetAttachmentFromAllAttachmnetName (sortData[i].allName), tempattachment);
                slotAttachmentNameList.Add (SpineChangeCoreLogic.Instance.GetSlotNameBySlotIndex (skeletonAnim, SpineChangeDataLogic.Instance.GetSlotIndexFromAllAttachmentName(sortData[i].allName)) + "$" + tempattachment.Name);
            }
            //替换完附件之后刷新(有的附件可以不用刷新)
            if(null == skeletonAnim) yield break;
            skeletonAnim.Skeleton.SetToSetupPose ();
            System.DateTime time1 = System.DateTime.Now;
            //重组Json
            yield return GameManager.Instance.StartCoroutine (SpineReadJsonDataLogic.Instance.CreateNewRoleJsonDic (animationData, eo.equipmentJsonAsset, slotAttachmentNameList, skeletonAnim, setRoleStatus));
            yield return null;
            System.DateTime time2 = System.DateTime.Now;
            if (null != GameObject.FindObjectOfType<ChangeTest> ())
                GameObject.FindObjectOfType<ChangeTest> ().text.text += "重组Json的时间：" + (time2 - time1).TotalMilliseconds + "\n";
            //当不是创建时change的话就判断是否合图集
            if (!isCreateInit) {
                if(null != roleObject){
                    setRoleStatus.Invoke (true);
                }
                if (isTexturePackage) {
                    UpdateRoleEquipment (skeletonAnim, combineTextureData);
                } else {
                    DestroyResources (combineTextureData);
                }
            }
        }
        /// <summary>
        /// 替换完attachment数据之后进行刷新
        /// </summary>
        private void UpdateRoleEquipment (SkeletonAnimation skeletonAnim, CombineTextureData combineTextureData) {
            //合并图集
            if(null != combineTextureData.combineCoroutine){
                GameManager.Instance.StopCoroutine(combineTextureData.combineCoroutine);
            }
            combineTextureData.combineCoroutine = GameManager.Instance.StartCoroutine (TexturePackaged (skeletonAnim, combineTextureData));
        }
        private System.Collections.IEnumerator TexturePackaged (SkeletonAnimation skeleAnim, CombineTextureData combineTextureData) {
            combineTextureData.isFinished = false;
            Skin skin = new Skin (SpineConstValue.NewSkinname);
            skin.Append (skeleAnim.skeleton.data.DefaultSkin);
            System.DateTime time1 = System.DateTime.Now;
            GetRepackedFromSkin (combineTextureData, skin, SpineConstValue.NewSkinname, Shader.Find (SpineConstValue.shaderPath));
            while (!combineTextureData.isFinished) {
                yield return null;
            }
            System.DateTime time2 = System.DateTime.Now;
            if (null != GameObject.FindObjectOfType<ChangeTest> ())
                GameObject.FindObjectOfType<ChangeTest> ().text.text += "合并图集用时：" + (time2 - time1).TotalMilliseconds + "\n";
            if (combineTextureData.isFinished) {
                skeleAnim.skeleton.SetSkin (combineTextureData.resultData.skin);
                skeleAnim.skeleton.data.defaultSkin = combineTextureData.resultData.skin;
                combineTextureData.resultData.self.DestroyEquipment();
                DestroyResources (combineTextureData);
            }
        }
        private void DestroyResources (CombineTextureData combineTextureData) {
            if(null != combineTextureData.resultData){
                if(null != combineTextureData.resultData.lastRenderTex){
                    RenderTexture.ReleaseTemporary(combineTextureData.resultData.lastRenderTex);
                    combineTextureData.resultData.lastRenderTex.Release();
                    combineTextureData.resultData.lastRenderTex = null;
                }
            }
            UnityEngine.Resources.UnloadUnusedAssets ();
        }
        /// <summary>
        /// 通过skin把所有的texture打包到一个图集
        /// </summary>
        /// <returns>The repacked from skin.</returns>
        /// <param name="skin">Skin.</param>
        /// <param name="newName">New name.</param>
        /// <param name="shader">Shader.</param>
        /// <param name="outputMaterial">Output material.</param>
        /// <param name="outputTexture">Output texture.</param>
        /// <param name="maxAtlasSize">Max atlas size.</param>
        /// <param name="padding">Padding.</param>
        /// <param name="textureFormat">Texture format.</param>
        /// <param name="mipmaps">If set to <c>true</c> mipmaps.</param>
        /// <param name="materialPropertySource">Material property source.</param>
        /// <param name="clearCache">If set to <c>true</c> clear cache.</param>
        private void GetRepackedFromSkin (CombineTextureData combineTextureData, Skin skin, string newName, Shader shader, int maxAtlasSize = SpineConstValue.smallAtlasSize, int padding = SpineConstValue.atlasPadding, TextureFormat textureFormat = SpineConstValue.textureFormat, bool mipmaps = SpineConstValue.atlasMipMaps, Material materialPropertySource = null, bool clearCache = SpineConstValue.atlasClearCache) {
            //用CPU合并图集
            //return AtlasUtilities.GetRepackedSkin(skin, newName, shader, out outputMaterial, out outputTexture);
            //用GPU合并图集
            SpineChangeCommonTools.GetSpineRepackedSkin (combineTextureData, skin, newName, shader, maxAtlasSize, textureFormat, mipmaps, materialPropertySource, clearCache);
        }
        /// <summary>
        /// 根据原始附件的类型选择生成不同类型的附件
        /// </summary>
        /// <returns>The attachment by type.</returns>
        /// <param name="type">Type.</param>
        /// <param name="attachmentName">Attachment name.</param>
        /// <param name="atlasRegion">Atlas region.</param>
        private Attachment GetAttachmentByType (Spine.Unity.SkeletonAnimation skeletonAnim, RoleAttachmentType type, string attachmentName, AtlasRegion atlasRegion) {
            switch (type) {
                //暂时只有MeshAttachment和RegionAttachment
                case RoleAttachmentType.Region:
                    return SpineCreateAttachment.Instance.CreateRegionAttachmentByAtlasRegion (skeletonAnim, attachmentName, atlasRegion);
                case RoleAttachmentType.LinkedMesh:
                case RoleAttachmentType.Mesh:
                    return SpineCreateAttachment.Instance.CreateMeshAttachmentByAtlasRegion (skeletonAnim, attachmentName, atlasRegion);
                case RoleAttachmentType.BoundingBox:
                    return SpineCreateAttachment.Instance.CreateBoundingBoxAttachmentByName (skeletonAnim, attachmentName);
                case RoleAttachmentType.Clipping:
                    return SpineCreateAttachment.Instance.CreateClippingAttachmentByName (skeletonAnim, attachmentName);
                case RoleAttachmentType.Path:
                    return SpineCreateAttachment.Instance.CreatePathAttachmentByName (skeletonAnim, attachmentName);
                case RoleAttachmentType.Point:
                    return SpineCreateAttachment.Instance.CreatePointAttachmentByName (skeletonAnim, attachmentName);
                default:
                    return null;
            }
        }
        /// <summary>
        /// 获取附件的类型
        /// </summary>
        /// <returns>The role attachment type.</returns>
        /// <param name="attachmentName">Attachment name.</param>
        public RoleAttachmentType GetRoleAttachmentType (SkeletonAnimation skeletonAnim, string attachmentName) {
            return SpineChangeDataLogic.Instance.GetAttachmentType (skeletonAnim, attachmentName);
        }
        #endregion
        #region Animation相关
        /// <summary>
        /// 通过Animation的名字获取Animation
        /// </summary>
        /// <returns>The animation.</returns>
        /// <param name="animationName">Animation name.</param>
        public Spine.Animation GetAnimation (SkeletonAnimation animation, string animationName) {
            ExposedList<TrackEntry> trackEntries = animation.state.Tracks;
            for (int i = 0; i < trackEntries.Count; i++) {
                if (trackEntries.Items[i].Animation.Name.Equals (animationName)) {
                    return trackEntries.Items[i].Animation;
                }
            }
            return new Spine.Animation ("<empty>", new ExposedList<Timeline> (), 0);
        }
        /// <summary>
        /// 通过trackIndex清除Animation
        /// </summary>
        /// <param name="trackIndex">Track index.</param>
        public void ClearAnimation (SkeletonAnimation animation, int trackIndex = 0) {
            animation.state.ClearTrack (trackIndex);
        }
        /// <summary>
        /// 清除所有的Animation
        /// </summary>
        public void ClearAllAnimation (SkeletonAnimation animation) {
            animation.state.ClearTracks ();
        }
        /// <summary>
        /// 获取当前的TrackEntry
        /// </summary>
        /// <returns>The current track entry.</returns>
        public TrackEntry GetCurrentTrackEntry (SkeletonAnimation animation, int trackIndex) {
            return animation.state.GetCurrent (trackIndex);
        }
        /// <summary>
        /// 设置空Animation
        /// </summary>
        /// <param name="mixDuration">Mix duration.</param>
        public void SetEmptyAnimation (SkeletonAnimation animation, float mixDuration, int trackIndex) {
            animation.state.SetEmptyAnimation (trackIndex, mixDuration);
        }
        /// <summary>
        /// 添加一个空的Animation
        /// </summary>
        /// <param name="trackIndex">Track index.</param>
        /// <param name="mixDuration">Mix duration.</param>
        /// <param name="delay">Delay.</param>
        public void AddEmptyAnimation (SkeletonAnimation animation, int trackIndex, float mixDuration, float delay) {
            animation.state.AddEmptyAnimation (trackIndex, mixDuration, delay);
        }
        /// <summary>
        /// 通过trackIndex animationName loop 设置Animation
        /// </summary>
        /// <returns>The animation.</returns>
        /// <param name="trackIndex">Track index.</param>
        /// <param name="animationName">Animation name.</param>
        /// <param name="loop">If set to <c>true</c> loop.</param>
        public TrackEntry SetAnimation (SkeletonAnimation anim, int trackIndex, string animationName, bool loop) {
            if (string.IsNullOrEmpty (animationName)) {
                return null;
            }
            Spine.Animation animation = anim.state.Data.SkeletonData.FindAnimation (animationName);
            //skeletonAnimation.Skeleton.SetToSetupPose();
            //skeletonAnimation.state.ClearTracks();
            return anim.state.SetAnimation (trackIndex, animation, loop);
        }
        /// <summary>
        /// 添加一个Animation
        /// </summary>
        /// <returns>The animation.</returns>
        /// <param name="trackIndex">Track index.</param>
        /// <param name="animationName">Animation name.</param>
        /// <param name="loop">If set to <c>true</c> loop.</param>
        /// <param name="delay">Delay.</param>
        public TrackEntry AddAnimation (SkeletonAnimation tempAnimation, int trackIndex, string animationName, bool loop, float delay) {
            if (string.IsNullOrEmpty (animationName)) {
                return null;
            }
            Spine.Animation animation = tempAnimation.state.Data.SkeletonData.FindAnimation (animationName);
            return tempAnimation.state.AddAnimation (trackIndex, animation, loop, delay);
        }
        #endregion
    }
}