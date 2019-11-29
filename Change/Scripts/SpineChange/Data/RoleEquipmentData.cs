using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DancingWord.SpineChange {
    //创建角色需要的数据
    public class AvatarData {
        //塑体Id
        public string plastomerId;
        //角色的类型
        public SpineAvatarType spineAvatarType;
        //角色游戏物体的名字
        public string avatarName;
        //默认皮肤名
        public string avatarSkinName;
        //初始动画名
        public string animationName;
        //是否开始循环播放动画
        public bool animationLoop;
        //是否合并图集
        public bool isTexturePackage;
        //装备数据
        public RoleEquipmentData equipmentData;
        //gameobject创建成功的回调
        public Action<Avatar> createSucCallBack;
        //穿戴成功之后的回调
        public Action<bool> wearSucCallBack;
        //创建失败的回调
        public Action<string> failCallBack;
    }
    /// <summary>
    /// 所有装备数据
    /// </summary>
    [Serializable]
    public class RoleEquipmentData {
        public string RolePlastomerId;
        public List<RoleEquipmentItem> RoleEquipmentItems;
    }
    /// <summary>
    /// 装备数据
    /// </summary>
    [Serializable]
    public class RoleEquipmentItem {
        public int EquipmentId;
        public RoleEquipmentType EquipmentType;
    }
    /// <summary>
    /// 装备的类型
    /// </summary>
    [Serializable]
    public enum RoleEquipmentType {
        /// <summary>
        /// 帽子类型
        /// </summary>
        RoleHat = 101,
        /// <summary>
        /// 眼镜类型
        /// </summary>
        RoleGlasses = 102,
        /// <summary>
        /// 发型类型
        /// </summary>
        RoleHair = 103,
        /// <summary>
        /// 衣服类型
        /// </summary>
        RoleClothe = 104,
        /// <summary>
        /// 表情类型
        /// </summary>
        RoleFace = 105,
        /// <summary>
        /// 皮肤和手类型
        /// </summary>
        RoleHand = 106,
        //----------------悬浮类----------------//
        /// <summary>
        /// 翅膀类型
        /// </summary>
        RoleWing = 111,
        /// <summary>
        /// 悬浮类型
        /// </summary>
        RoleFollow = 112,
        /// <summary>
        /// 左手持类型
        /// </summary>
        RoleLeftInHand = 113,
        /// <summary>
        /// 右手持类型
        /// </summary>
        RoleRightInHand = 114,
        /// <summary>
        /// 双手持类型
        /// </summary>
        RoleInHand = 115,
        //----------------塑体----------------//
        RolePlastomer = 888,
    }
    //角色的动画数据json
    public class RoleAnaimationData {
        public string roleJsonStr;
    }
    public enum RoleAttachmentType {
        Region,
        BoundingBox,
        Mesh,
        LinkedMesh,
        Path,
        Point,
        Clipping,
        None,
    }
    //排序会用的数据
    public class AtlasRegionData {
        //插槽索引和附件名
        public string allName;
        public Spine.AtlasRegion region;
        public RoleAttachmentType type;
    }
}