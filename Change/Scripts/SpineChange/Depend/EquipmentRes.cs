using System;
using System.Collections;
using System.Collections.Generic;
using DancingWord.Resources;
using UnityEngine;
public class EquipmentRes : ResBase {
    private EquipmentObject mObject = new EquipmentObject();
    public EquipmentRes (string resName, LoadType loadtype, ResLoadType mResLoadType, Action<UnityEngine.Object> func) : base (resName, loadtype, mResLoadType, func) { 
        loadABType = LoadABType.WholePackage;
        LoadResByType<EquipmentRes>("/Equipments/" + mResName,loadABType);
    }
    public EquipmentObject GetRes(){
        return mObject;
    }
    protected override void Callback (Dictionary<string, UnityEngine.Object> objs) {
        if(objs.Count == 0) return;
        #if UNITY_ANDROID
        foreach(KeyValuePair<string,UnityEngine.Object> obj in objs){
            if(obj.Key.EndsWith(".json")){
                mObject.equipmentJsonAsset = obj.Value as TextAsset;
            }else if(obj.Key.EndsWith(".txt")){
                mObject.equipmentTxtAsset = obj.Value as TextAsset;
            }else if(obj.Key.EndsWith(".png")){
                if(obj.Key.Contains("alpha"))
                mObject.equipmentTexture_Alpha = obj.Value as Texture2D;
                if(obj.Key.Contains("rgb"))
                mObject.equipmentTexture_RGB = obj.Value as Texture2D;
            }
        }
        #elif UNITY_IPHONE || UNITY_EDITOR
        foreach(KeyValuePair<string,UnityEngine.Object> obj in objs){
            if(obj.Key.EndsWith(".json")){
                mObject.equipmentJsonAsset = obj.Value as TextAsset;
            }else if(obj.Key.EndsWith(".txt")){
                mObject.equipmentTxtAsset = obj.Value as TextAsset;
            }else if(obj.Key.EndsWith(".png")){
                mObject.equipmentTexture = obj.Value as Texture2D;
            }
        }
        #endif
        
        isDone = true;
        if(mAct != null) mAct(mObject);
    }
    private void InterruptDown(){
        mLoader.InterruptLoad("/equipments/" + mResName + Tools.ExtName);
    }
    public override void ClearData(){
        if(mObject == null){
            InterruptDown();
        }else{
            Destroy(mObject);
            mObject = null;
        }
        base.ClearData();
    }
}