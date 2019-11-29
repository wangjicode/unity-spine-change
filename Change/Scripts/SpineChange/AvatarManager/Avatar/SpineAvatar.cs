using System.Collections;
using System.Collections.Generic;
using DancingWord.SpineChange;
using Spine.Unity;
using UnityEngine;
public enum SpineAvatarType {
    Middle, //舞台中间
    Left, //舞台左边
    Right, //舞台右边
    Challange, //挑战者
    Other, //其他
}
public class SpineAvatar : Avatar {
    public Object avatarObject;
    public SpineAvatarType avatarType;
    public AvatarFollow avatarFollow;
    public SpineAvatar (RoleEquipmentData equipmentData, SkeletonAnimation skeletonAnimation, Dictionary<int, List<string>> slotAttachmentDic, RoleAnaimationData animationData, CombineTextureData combineTextureData, SpineAvatarType avatarType, Object avatarObject, string roleJsonStr) : base (equipmentData, skeletonAnimation, slotAttachmentDic, animationData, combineTextureData) {
        this.avatarObject = avatarObject;
        this.avatarType = avatarType;
    }
    //销毁avatar
    public void DestroySpineAvatar () {
        base.DestroyEquipment (true);
        GameObject.Destroy (avatarObject);
        if (null != avatarFollow) {
            GameObject.Destroy (avatarFollow);
        }
    }
    //跟随骨骼移动
    public void FollowBoneMove (SpineAvatar otherSpine, string boneName) {
        GameObject selfFollowGameObject = new GameObject (avatarObject.name);
        avatarFollow = selfFollowGameObject.AddComponent<AvatarFollow> ();
        BoneFollower boneFollower = selfFollowGameObject.AddComponent<BoneFollower> ();
        boneFollower.SkeletonRenderer = GetSkeletonAnimation;
        boneFollower.SetBone (boneName);
        avatarFollow.StartFollow (selfFollowGameObject, otherSpine);
    }
}