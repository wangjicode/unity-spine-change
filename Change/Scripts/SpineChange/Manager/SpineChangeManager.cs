using System;
using System.Collections;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace DancingWord.SpineChange {
    public class SpineChangeManager {
        #region 字段
        private static SpineChangeManager _instance;
        public static SpineChangeManager Instance {
            get {
                if (_instance == null) {
                    _instance = new SpineChangeManager ();
                }
                return _instance;
            }
        }
        private SpineChangeCoreLogic spineChangeCoreLogic;
        #endregion
        #region 构造函数
        private SpineChangeManager () {
            spineChangeCoreLogic = SpineChangeCoreLogic.Instance;
        }
        #endregion
        #region 换装相关
        /// <summary>
        /// 创建角色
        /// </summary>
        /// <param name="skeletonJson">Skeleton json.</param>
        /// <param name="atlasText">Atlas text.</param>
        /// <param name="textures">Textures.</param>
        /// <param name="material">Material.</param>
        /// <param name="initialize">If set to <c>true</c> initialize.</param>
        /// <param name="skinName">Skin name.</param>
        /// <param name="animationName">Animation name.</param>
        /// <param name="loop">If set to <c>true</c> loop.</param>
        /// <param name="pos">Position.</param>
        /// <param name="successfulCallBack">Successful call back.</param>
        /// <param name="failCallBack">Fail call back.</param>
        public void SpineCreateRole (SpineAvatarType avatarType, RoleEquipmentItem equipmentItem, Material material, bool initialize, string name, string skinName, string animationName, bool loop, RoleEquipmentData data, Action<Avatar> createSucCallBack, Action<string> failCallBack, Action<bool> createQueueCallBack, bool isTP) {
            SpineChangeDataLogic.Instance.GetRoleEquipmentObject (equipmentItem, (UnityEngine.Object o) => {
                spineChangeCoreLogic.Create (avatarType, o as EquipmentObject, material, initialize, name, skinName, animationName, loop, data, createSucCallBack, failCallBack, createQueueCallBack, isTP);
            });
        }
        /// <summary>
        /// 角色换装
        /// </summary>
        /// <param name="slotStr">Slot string.</param>
        /// <param name="texture2D">Texture2 d.</param>
        /// <param name="successfulCallBack">Successful call back.</param>
        /// <param name="failCallBack">Fail call back.</param>
        public void SpineRoleChange (RoleAnaimationData animationData, SkeletonAnimation skeletonAnim, CombineTextureData combineTextureData, Dictionary<int, List<string>> slotAttachmentDic, RoleEquipmentData equipmentData, RoleEquipmentItem equipmentItem, bool isTexturePackaged, List<Texture2D> destoryTextureList ,Action<bool> setRoleStatus, Action<string> failCallBack = null) {
            if (null != setRoleStatus) {
                setRoleStatus.Invoke (false);
            }
            System.DateTime time1 = System.DateTime.Now;
            SpineChangeDataLogic.Instance.GetRoleEquipmentObject (equipmentItem, (UnityEngine.Object o) => {
                //SpineChangeDataLogic.Instance.SaveRoleEquipmentToServerAndLocal (equipmentData, equipmentItem);
                if(null != combineTextureData.changeRoleCoroutine){
                    GameManager.Instance.StopCoroutine(combineTextureData.changeRoleCoroutine);
                }
                combineTextureData.changeRoleCoroutine = GameManager.Instance.StartCoroutine (spineChangeCoreLogic.Change (animationData, skeletonAnim, combineTextureData, slotAttachmentDic, o as EquipmentObject, isTexturePackaged, destoryTextureList ,setRoleStatus, failCallBack));
                System.DateTime time2 = System.DateTime.Now;
                if (null != GameObject.FindObjectOfType<ChangeTest> ())
                    GameObject.FindObjectOfType<ChangeTest> ().text.text += "加载AB用时：" + (time2 - time1).TotalMilliseconds + "\n";
            });
        }
        /// <summary>
        /// 获取AttachmentType
        /// </summary>
        /// <returns>The get role attachment type.</returns>
        /// <param name="attachmentName">Attachment name.</param>
        /// <param name="successfulCallBack">Successful call back.</param>
        /// <param name="failCallBack">Fail call back.</param>
        public RoleAttachmentType SpineGetRoleAttachmentType (SkeletonAnimation skeletonAnim, string attachmentName, Action<string> successfulCallBack, Action<string> failCallBack) {
            return spineChangeCoreLogic.GetRoleAttachmentType (skeletonAnim, attachmentName);
        }
        /// <summary>
        /// 初始化角色的插槽和附件信息
        /// </summary>
        /// <param name="successfulCallBack">Successful call back.</param>
        /// <param name="failCallBack">Fail call back.</param>
        public void SpineInitRoleSlotAttachment (RoleAnaimationData animationData, SkeletonAnimation skeletonAnim, RoleEquipmentData tempData, CombineTextureData combineTextureData, Dictionary<int, List<string>> TempSlotAttachmentDic, List<Texture2D> destoryTextureList ,Action<bool> setRoleStatus, bool isTP, bool isHide, Action<string> failCallBack = null) {
            if (null != setRoleStatus && isHide) {
                setRoleStatus.Invoke (false);
            }
            if(null != combineTextureData.initRoleCoroutine){
                GameManager.Instance.StopCoroutine(combineTextureData.initRoleCoroutine);
            }
            combineTextureData.initRoleCoroutine = GameManager.Instance.StartCoroutine (spineChangeCoreLogic.InitRoleEquipment (animationData, skeletonAnim, tempData, combineTextureData, TempSlotAttachmentDic, destoryTextureList ,setRoleStatus, failCallBack, isTP));
        }
        #endregion
        #region Animation相关
        /// <summary>
        /// 通过animationName获取Animation
        /// </summary>
        /// <returns>The get animation.</returns>
        /// <param name="animationName">Animation name.</param>
        public Spine.Animation SpineGetAnimation (SkeletonAnimation animation, string animationName) {
            return spineChangeCoreLogic.GetAnimation (animation, animationName);
        }
        /// <summary>
        /// 清空TrackIndex下的所有Animation
        /// </summary>
        /// <param name="trackIndex">Track index.</param>
        public void SpineClearAnimationByTrackIndex (SkeletonAnimation animation, int trackIndex = 0) {
            spineChangeCoreLogic.ClearAnimation (animation, trackIndex);
        }
        /// <summary>
        /// 清空所有的Animation
        /// </summary>
        public void SpineClearAllAnimation (SkeletonAnimation animation) {
            spineChangeCoreLogic.ClearAllAnimation (animation);
        }
        /// <summary>
        /// 获取当前TrackTrackEntry(可拿到Animation)
        /// </summary>
        /// <returns>The get current track entry.</returns>
        public TrackEntry SpineGetCurrentTrackEntry (SkeletonAnimation animation, int trackIndex) {
            return spineChangeCoreLogic.GetCurrentTrackEntry (animation, trackIndex);
        }
        /// <summary>
        /// 设置当前角色的动作为空动作
        /// </summary>
        /// <param name="mixDuration">Mix duration.</param>
        public void SpineSetEmptyAnimation (SkeletonAnimation animation, float mixDuration, int trackIndex) {
            spineChangeCoreLogic.SetEmptyAnimation (animation, mixDuration, trackIndex);
        }
        /// <summary>
        /// 添加一个空的Animation
        /// </summary>
        /// <param name="trackIndex">Track index.</param>
        /// <param name="mixDuration">过渡时间</param>
        /// <param name="delay">Delay.</param>
        public void SpineAddEmptyAnimation (SkeletonAnimation animation, int trackIndex, float mixDuration, float delay) {
            spineChangeCoreLogic.AddEmptyAnimation (animation, trackIndex, mixDuration, delay);
        }
        /// <summary>
        /// 切换动画
        /// </summary>
        /// <returns>The set animation.</returns>
        /// <param name="trackIndex">Track index.</param>
        /// <param name="animationName">Animation name.</param>
        /// <param name="loop">If set to <c>true</c> loop.</param>
        /// <param name="successfulCallBack">Successful call back.</param>
        /// <param name="failCallBack">Fail call back.</param>
        public TrackEntry SpineSetAnimation (SkeletonAnimation animation, int trackIndex, string animationName, bool loop, Action<string> sucCallBack = null, Action<string> failCallBack = null) {
            TrackEntry trackEntry = spineChangeCoreLogic.SetAnimation (animation, trackIndex, animationName, loop);
            if (null != trackEntry) {
                if (sucCallBack != null)
                    sucCallBack.Invoke ("设置Animation：" + animationName + "成功！");
            } else {
                if (failCallBack != null)
                    failCallBack.Invoke ("设置Animation：" + animationName + "失败！");
            }
            return trackEntry;
        }
        /// <summary>
        /// 添加动画
        /// </summary>
        /// <returns>The add animation.</returns>
        /// <param name="trackIndex">Track index.</param>
        /// <param name="animationName">Animation name.</param>
        /// <param name="loop">If set to <c>true</c> loop.</param>
        /// <param name="delay">Delay.</param>
        /// <param name="successfulCallBack">Successful call back.</param>
        /// <param name="failCallBack">Fail call back.</param>
        public TrackEntry SpineAddAnimation (SkeletonAnimation animation, int trackIndex, string animationName, bool loop, float delay, Action<string> sucCallBack = null, Action<string> failCallBack = null) {
            TrackEntry trackEntry = spineChangeCoreLogic.AddAnimation (animation, trackIndex, animationName, loop, delay);
            if (null != trackEntry) {
                if (sucCallBack != null)
                    sucCallBack.Invoke ("添加Animation：" + animationName + "成功！");
            } else {
                if (failCallBack != null)
                    failCallBack.Invoke ("添加Animation：" + animationName + "失败！");
            }
            return trackEntry;
        }
        #endregion
    }
}