using System;
using System.Collections;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEngine;
namespace DancingWord.SpineChange {
    public class SpineChangeDataLogic {
        /// <summary>
        /// 材质
        /// </summary>
        private Material equipmentMaterial;

        private static SpineChangeDataLogic _instance;
        public static SpineChangeDataLogic Instance {
            get {
                if (_instance == null) {
                    _instance = new SpineChangeDataLogic ();
                }
                return _instance;
            }
        }
        private SpineChangeDataLogic () { }
        /// <summary>
        /// 从服务器或者本地获取角色的服装数据
        /// </summary>
        /// <returns>The role equipment form server or local.</returns>
        public RoleEquipmentData GetRoleEquipmentFormServerOrLocal () {
            //TODO
            return null;
        }
        //根据装备类型和装备名称ID 组装的数据类 获取装备资源
        public EquipmentRes GetRoleEquipmentObject (RoleEquipmentItem equipmentItem, Action<UnityEngine.Object> action) {
            string resName = equipmentItem.EquipmentType + "_" + equipmentItem.EquipmentId;
            return ResourcesManager.Instance.LoadResAsyn<EquipmentRes> (resName, ResLoadType.AssetBundleLoad, action) as EquipmentRes;
        }
        /// <summary>
        /// 保存角色的服装数据到服务器和本地
        /// </summary>
        /// <returns><c>true</c>, if role equipment to server and local was saved, <c>false</c> otherwise.</returns>
        /// <param name="equipmentData">Equipment data.</param>
        public bool SaveRoleEquipmentToServerAndLocal (RoleEquipmentData equipmentData, RoleEquipmentItem item) {
            //item更新到equipmentData里面。
            for (int i = 0; i < equipmentData.RoleEquipmentItems.Count; i++) {
                if (item.EquipmentType.Equals (equipmentData.RoleEquipmentItems[i].EquipmentType)) {
                    equipmentData.RoleEquipmentItems[i] = item;
                    break;
                }
            }
            //新的equipmentData持久化存储。
            //TODO
            return true;
        }
        //Resources加载文件夹中的所有文件。
        // public EquipmentObject ResourcesLoadAllEquipmentRes (string resName) {
        //     EquipmentObject eo = new EquipmentObject ();
        //     UnityEngine.Object[] objs = UnityEngine.Resources.LoadAll (resName);
        //     for (int i = 0; i < objs.Length; i++) {
        //         if (objs[i].GetType ().Equals (typeof (UnityEngine.Texture2D))) {
        //             if(objs[i].name.Contains("alpha"))
        //             eo.equipmentTexture_Alpha = objs[i] as Texture2D;
        //             if(objs[i].name.Contains("rgb"))
        //             eo.equipmentTexture_RGB = objs[i] as Texture2D;
        //         } else if (objs[i].GetType ().Equals (typeof (UnityEngine.TextAsset))) {
        //             if (objs[i].name.Contains (".atlas")) {
        //                 eo.equipmentTxtAsset = objs[i] as TextAsset;
        //             } else {
        //                 eo.equipmentJsonAsset = objs[i] as TextAsset;
        //             }
        //         }
        //     }
        //     return eo;
        // }
        /// <summary>
        /// 通过角色的服装Id读取.atlas文件
        /// </summary>
        /// <returns>The equipment atlas asset.</returns>
        /// <param name="equipmentId">Equipment identifier.</param>
        // public TextAsset GetEquipmentAtlasAsset (int equipmentId) {
        //     if (equipmentId == 0) {
        //         equipmentAtlasAsset = UnityEngine.Resources.Load<TextAsset> ("RoleRes/G2/G2.atlas");
        //     } else if (equipmentId == -1) {
        //         equipmentAtlasAsset = UnityEngine.Resources.Load<TextAsset> ("RoleRes/G1/G1.atlas");
        //     } else {
        //         //TODO
        //         equipmentAtlasAsset = null;
        //     }
        //     return equipmentAtlasAsset;
        // }
        /// <summary>
        /// 通过角色的服装Id读取.json
        /// </summary>
        /// <returns>The equipment json asset.</returns>
        /// <param name="equipmentId">Equipment identifier.</param>
        // public TextAsset GetEquipmentJsonAsset (int equipmentId) {
        //     if (equipmentId == 0) {
        //         equipmentJsonAsset = UnityEngine.Resources.Load<TextAsset> ("RoleRes/G2/G2");
        //     } else if (equipmentId == -1) {
        //         equipmentJsonAsset = UnityEngine.Resources.Load<TextAsset> ("RoleRes/G1/G1");
        //     } else {
        //         equipmentJsonAsset = null;
        //     }
        //     return equipmentJsonAsset;
        // }
        /// <summary>
        /// 通过角色的服装Id读取Texture2D文件
        /// </summary>
        /// <returns>The equipment texture.</returns>
        /// <param name="equipmentId">Equipment identifier.</param>
        // public Texture2D[] GetEquipmentTexture (int equipmentId) {
        //     if (equipmentId == 0) {
        //         equipmenttexture = new Texture2D[1];
        //         equipmenttexture[0] = UnityEngine.Resources.Load<Texture2D> ("RoleRes/G2/G2");
        //     } else if (equipmentId == -1) {
        //         equipmenttexture = new Texture2D[1];
        //         equipmenttexture[0] = UnityEngine.Resources.Load<Texture2D> ("RoleRes/G1/G1");
        //     } else {
        //         //TODO
        //         equipmenttexture = null;
        //     }
        //     return equipmenttexture;
        // }
        /// <summary>
        /// 返回默认的材质
        /// </summary>
        /// <returns>The equipment material.</returns>
        /// <param name="eqipmentId">Eqipment identifier.</param>
        public Material GetEquipmentMaterial (int eqipmentId = -1) {
            string shaderPath = null;
            #if UNITY_ANDROID
            shaderPath = "Unlit/Transparent Colored ETC1";
            #elif UNITY_IPHONE || UNITY_EDITOR
            shaderPath = SpineConstValue.shaderPath;
            #endif
            equipmentMaterial = new Material (Shader.Find (shaderPath));
            return equipmentMaterial;
        }
        /// <summary>
        /// 通过TextAsset，Texture2D[]，Material 获取AtlasRegion集合
        /// </summary>
        /// <returns>The equipment atlas regions.</returns>
        /// <param name="textAsset">Text asset.</param>
        /// <param name="textures">Textures.</param>
        /// <param name="material">Material.</param>

        public List<AtlasRegion> GetEquipmentAtlasRegions (EquipmentObject eo, Material material) {
            #if UNITY_ANDROID
            return AtlasAsset.CreateRuntimeInstance (eo.equipmentTxtAsset, eo.equipmentTexture_Alpha,eo.equipmentTexture_RGB, material, true,true).GetAtlas (true).AtlasRegionList;
            #elif UNITY_IPHONE || UNITY_EDITOR
            return AtlasAsset.CreateRuntimeInstance (eo.equipmentTxtAsset, new Texture2D[]{eo.equipmentTexture}, material, true).GetAtlas ().AtlasRegionList;
            #endif
        }
        /// <summary>
        /// 通过Attachment获取SlotName
        /// </summary>
        /// <returns>The slot name by attachment name.</returns>
        /// <param name="attachmentName">Attachment name.</param>
        public string GetSlotNameByAttachmentName (string attachmentName) {
            return attachmentName.Split ('-') [0];
        }
        /// <summary>
        /// 从名称组合中拿出sortindex
        /// </summary>
        /// <returns>The slot index from all attachment name.</returns>
        /// <param name="allAttachmentName">All attachment name.</param>
        public int GetSlotIndexFromAllAttachmentName (string allAttachmentName) {
            return int.Parse (allAttachmentName.Split ('|') [0]);
        }
        /// <summary>
        /// 从名称组合中拿出attachment
        /// </summary>
        /// <returns>The attachment from all attachmnet name.</returns>
        /// <param name="allAttachmentName">All attachment name.</param>
        public string GetAttachmentFromAllAttachmnetName (string allAttachmentName) {
            return allAttachmentName.Split ('|') [1];
        }
        /// <summary>
        /// 根据附件的名称获取附件的类型
        /// </summary>
        /// <returns>The attachment type.</returns>
        /// <param name="allAttachmentName">Attachment name.</param>
        public RoleAttachmentType GetAttachmentType (SkeletonAnimation skeletonAnim, string allAttachmentName) {
            Dictionary<String, System.Object> root = SpineReadJsonDataLogic.Instance.GetTargetObjectDic (SpineConstValue.defaultSkinName,
                SpineChangeCoreLogic.Instance.GetSlotNameBySlotIndex (skeletonAnim,
                    GetSlotIndexFromAllAttachmentName (allAttachmentName)),
                GetAttachmentFromAllAttachmnetName (allAttachmentName));
            if (null == root) {
                return RoleAttachmentType.None;
            }
            string type = SpineReadJsonDataLogic.Instance.GetString (root, "type", "region");
            return (RoleAttachmentType) Enum.Parse (typeof (RoleAttachmentType), type, true);
        }
    }
}