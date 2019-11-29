using System;
using System.Collections;
using System.Collections.Generic;
using DancingWord.SpineChange;
using Spine;
using Spine.Unity;
using UnityEngine;

public class Avatar {
	private RoleEquipmentData equipmentData;
	private RoleAnaimationData animationData;
	private SkeletonAnimation skeletonAnimation;
	private CombineTextureData combineTextureData;
	private List<Texture2D> destoryTextureList;
	public List<Texture2D> GetDestoryTextureList{
		get{
			return destoryTextureList;
		}
	}
	public SkeletonAnimation GetSkeletonAnimation {
		get { return skeletonAnimation; }
	}
	//初始化完成或者换装完成回调
	public Action<bool> wearSucCallBack = null;
	private Dictionary<int, List<string>> slotAttachmentDic;
	public Avatar (RoleEquipmentData equipmentData, SkeletonAnimation skeletonAnimation, Dictionary<int, List<string>> slotAttachmentDic, RoleAnaimationData animationData, CombineTextureData combineTextureData) {
		this.equipmentData = equipmentData;
		this.skeletonAnimation = skeletonAnimation;
		this.slotAttachmentDic = slotAttachmentDic;
		this.animationData = animationData;
		this.combineTextureData = combineTextureData;
		this.destoryTextureList = new List<Texture2D>();
	}
	//播放动作
	public virtual void PlayAnimation (int trackIndex, string animationName, bool loop, Action<string> sucCallBack = null, Action<string> failCallBack = null) {
		SpineChangeManager.Instance.SpineSetAnimation (skeletonAnimation, trackIndex, animationName, loop, sucCallBack, failCallBack);
	}
	//播放动作
	public virtual void PlayEmptyAnimation (int trackIndex, float mixDuration) {
		SpineChangeManager.Instance.SpineSetEmptyAnimation (skeletonAnimation, mixDuration, trackIndex);
	}
	//添加动作
	public virtual void AddAnimation (int trackIndex, string animationName, bool loop, float delay, Action<string> sucCallBack = null, Action<string> failCallBack = null) {
		SpineChangeManager.Instance.SpineAddAnimation (skeletonAnimation, trackIndex, animationName, loop, delay, sucCallBack, failCallBack);
	}
	//添加空动作
	public virtual void AddEmptyAnimation (int trackIndex, float mixDuration, float delay, Action<string> sucCallBack = null, Action<string> failCallBack = null) {
		SpineChangeManager.Instance.SpineAddEmptyAnimation (skeletonAnimation, trackIndex, mixDuration, delay);
	}
	//局部换装
	public virtual void ChangeEquipment (string equipmentId, bool isTexturePackaged, Action<bool> wearSucCallBack = null, Action<string> failCallBack = null) {
		StopChangeCoroutine();
		this.wearSucCallBack = wearSucCallBack;
		SpineChangeManager.Instance.SpineRoleChange (animationData, skeletonAnimation, combineTextureData, slotAttachmentDic, equipmentData, RoleEquipmentDataTools.CreateRoleEquipmentItemById (equipmentId), isTexturePackaged, destoryTextureList, SetRoleActivity, failCallBack);
	}
	//初始化装备信息
	public virtual void InitEquipment (bool isTP, RoleEquipmentData equipmentData = null,bool isHide = true, Action<bool> wearSucCallBack = null, Action<string> failCallBack = null) {
		StopChangeCoroutine();
		this.wearSucCallBack = wearSucCallBack;
		RoleEquipmentData data = equipmentData == null?this.equipmentData : equipmentData;
		SpineChangeManager.Instance.SpineInitRoleSlotAttachment (animationData, skeletonAnimation, data, combineTextureData, slotAttachmentDic, destoryTextureList ,SetRoleActivity, isTP, isHide, failCallBack);
	}
	//获取animation
	public virtual Spine.Animation GetAnimation (string animationName) {
		return SpineChangeManager.Instance.SpineGetAnimation (skeletonAnimation, animationName);
	}
	//清空清空TrackIndex下的所有Animation
	public virtual void ClearAnimation (int trackIndex) {
		SpineChangeManager.Instance.SpineClearAnimationByTrackIndex (skeletonAnimation, trackIndex);
	}
	//清空所有的Animation
	public virtual void ClearAllAnimation () {
		SpineChangeManager.Instance.SpineClearAllAnimation (skeletonAnimation);
	}
	//获取当前TrackTrackEntry(可拿到Animation)
	public virtual TrackEntry GetTrackEntry (int trackIndex) {
		return SpineChangeManager.Instance.SpineGetCurrentTrackEntry (skeletonAnimation, trackIndex);
	}
	//设置颜色
	public virtual void SetColor (Color color) {
		Spine.Unity.SkeletonExtensions.SetColor (skeletonAnimation.Skeleton, color);
	}
	//设置角色的显隐状态
	public void SetRoleActivity (bool isActiv) {
		skeletonAnimation.gameObject.SetActive (isActiv);
		if (null != wearSucCallBack && isActiv) {
			wearSucCallBack.Invoke (isActiv);
		}
	}
	//销毁资源
	public void DestroyEquipment (bool isDestroy = false) {
		if (null == skeletonAnimation || null == skeletonAnimation.skeletonDataAsset || skeletonAnimation.skeletonDataAsset.atlasAssets.Length <= 0 || skeletonAnimation.skeletonDataAsset.atlasAssets[0].materials.Length <= 0) {
			LogManager.LogWarning("Avatar角色已经被清除！");
		}else{
			#if UNITY_ANDROID
			destoryTextureList.Add(skeletonAnimation.skeletonDataAsset.atlasAssets[0].materials[0].GetTexture("_MainTex") as Texture2D);
			destoryTextureList.Add(skeletonAnimation.skeletonDataAsset.atlasAssets[0].materials[0].GetTexture("_MainTex_A") as Texture2D);
			#elif UNITY_IPHONE || UNITY_EDITOR
			destoryTextureList.Add(skeletonAnimation.skeletonDataAsset.atlasAssets[0].materials[0].mainTexture as Texture2D);
			#endif
		}
		if(null != destoryTextureList){
			for(int i=0;i<destoryTextureList.Count;i++){
				Resources.UnloadAsset(destoryTextureList[i]);
			}
		}
		if(isDestroy){
			if(null != combineTextureData.resultData.currentRenderTex){
				RenderTexture.ReleaseTemporary(combineTextureData.resultData.currentRenderTex);
				combineTextureData.resultData.currentRenderTex.Release();
				combineTextureData.resultData.currentRenderTex = null;
			}
			skeletonAnimation = null;
			destoryTextureList = null;
			slotAttachmentDic.Clear ();
		}
		Resources.UnloadUnusedAssets();
		StopChangeCoroutine();
	}
	//停止协程
	public void StopChangeCoroutine(){
		if(null != combineTextureData.combineCoroutine){
			GameManager.Instance.StopCoroutine(combineTextureData.combineCoroutine);
		}
		if(null != combineTextureData.combineCoroutineBig){
			GameManager.Instance.StopCoroutine(combineTextureData.combineCoroutineBig);
		}
		if(null != combineTextureData.combineCoroutineSmall){
			GameManager.Instance.StopCoroutine(combineTextureData.combineCoroutineSmall);
		}
		if(null != combineTextureData.changeRoleCoroutine){
			GameManager.Instance.StopCoroutine(combineTextureData.changeRoleCoroutine);
		}
		if(null != combineTextureData.initRoleCoroutine){
			GameManager.Instance.StopCoroutine(combineTextureData.initRoleCoroutine);
		}
	}
}