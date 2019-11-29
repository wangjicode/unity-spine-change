using System;
using System.Collections;
using System.Collections.Generic;
using DancingWord.Resources;
using DancingWord.SpineChange;
using LitJson;
using Spine;
using Spine.Unity;
using Spine.Unity.Modules.AttachmentTools;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ChangeTest : MonoBehaviour {
    public Button buttonAnimation;
    public Button buttonDestory;
    public Button buttonInit;
    public Text text;
    public SpineAvatar middle;
    public Image image;
    // private IEnumerator Update() {
    //     if(isStart){
    //         image.transform.localPosition += new Vector3(10f,0f,0f);
    //         yield return new WaitForSeconds(0.09f);
    //     }
    //     Debug.Log("OK");
    // }
    private IEnumerator test(){
        while(true){
            image.transform.localPosition += new Vector3(10f,0f,0f);
            yield return new WaitForSeconds(0.001f);
        }
    }
    private bool isStart = false;
    private void Start () {
        buttonAnimation.onClick.AddListener (() => {
            //isStart = true;
            // middle.PlayAnimation (1, "attack_01", false, Debug.Log, Debug.Log);
            // middle.AddEmptyAnimation (1, 1f, 0.1f, Debug.Log, Debug.Log);
            // middle.SetColor (Color.white);
            //image.transform.DOMoveX(0,0.3f,true);
            StartCoroutine(test());
        });
        buttonDestory.onClick.AddListener (() => {
            text.text = "";
            middle.DestroySpineAvatar ();
        });
        buttonInit.onClick.AddListener (() => {
            RoleEquipmentData equipmentData1 = new RoleEquipmentData ();
            equipmentData1.RoleEquipmentItems = new List<RoleEquipmentItem> ();
            equipmentData1.RoleEquipmentItems.Add (new RoleEquipmentItem { EquipmentId = 300001, EquipmentType = RoleEquipmentType.RoleHair }); //7
            equipmentData1.RoleEquipmentItems.Add (new RoleEquipmentItem { EquipmentId = 400001, EquipmentType = RoleEquipmentType.RoleClothe }); //8
            equipmentData1.RoleEquipmentItems.Add (new RoleEquipmentItem { EquipmentId = 500001, EquipmentType = RoleEquipmentType.RoleFace }); //9
            equipmentData1.RoleEquipmentItems.Add (new RoleEquipmentItem { EquipmentId = 200001, EquipmentType = RoleEquipmentType.RoleGlasses }); //5
            equipmentData1.RoleEquipmentItems.Add (new RoleEquipmentItem { EquipmentId = 600008, EquipmentType = RoleEquipmentType.RoleHand }); //11
            equipmentData1.RoleEquipmentItems.Add (new RoleEquipmentItem { EquipmentId = 100001, EquipmentType = RoleEquipmentType.RoleHat }); //5
            middle.InitEquipment (true, equipmentData1,true, (bool b) => { LogManager.LogError ("穿戴所有装备完成。"); });
        });
    }
    public void Change (string id) {
        if (null != middle) {
            LogManager.Log ("middle != null");
            middle.ChangeEquipment (id, true, (bool b) => { LogManager.LogError ("穿戴局部装备完成。"); }, Debug.Log);
        } else {
            LogManager.Log ("middle == null");
        }
    }
    public void CreateMiddle () {
        RoleEquipmentData equipmentData = new RoleEquipmentData ();
        equipmentData.RolePlastomerId = "88888888";
        equipmentData.RoleEquipmentItems = new List<RoleEquipmentItem> ();
        equipmentData.RoleEquipmentItems.Add (new RoleEquipmentItem { EquipmentId = 300007, EquipmentType = RoleEquipmentType.RoleHair }); //7
        equipmentData.RoleEquipmentItems.Add (new RoleEquipmentItem { EquipmentId = 400008, EquipmentType = RoleEquipmentType.RoleClothe }); //8
        equipmentData.RoleEquipmentItems.Add (new RoleEquipmentItem { EquipmentId = 500009, EquipmentType = RoleEquipmentType.RoleFace }); //9
        equipmentData.RoleEquipmentItems.Add (new RoleEquipmentItem { EquipmentId = 200005, EquipmentType = RoleEquipmentType.RoleGlasses }); //5
        equipmentData.RoleEquipmentItems.Add (new RoleEquipmentItem { EquipmentId = 600009, EquipmentType = RoleEquipmentType.RoleHand }); //11
        equipmentData.RoleEquipmentItems.Add (new RoleEquipmentItem { EquipmentId = 100005, EquipmentType = RoleEquipmentType.RoleHat }); //5
        AvatarCreateQueue.Instance.CreateAvatar(equipmentData,SpineAvatarType.Middle,"jump_lv1_01",true,true,(Avatar a) => {
            middle = a as SpineAvatar;
            GameObject role = middle.avatarObject as GameObject;
            role.transform.position = new Vector3 (2.5f, -2f, 0);
            role.transform.localScale = Vector3.one;
            LogManager.Log ("角色gameobject完成。");
        },(bool b) => { LogManager.Log ("穿戴所有装备完成。"); },null);
    }
}