using System;
using System.Collections.Generic;
using System.IO;
using LitJson;
using Spine;

namespace DancingWord.SpineChange {
    public class SpineReadJsonDataLogic {
        /// <summary>
        /// 从json文件加载的数据
        /// </summary>
        private Dictionary<String, Object> root = null;
        private static SpineReadJsonDataLogic _instance;
        public static SpineReadJsonDataLogic Instance {
            get {
                if (_instance == null) {
                    _instance = new SpineReadJsonDataLogic ();
                }
                return _instance;
            }
        }
        private SpineReadJsonDataLogic () { }
        /// <summary>
        /// 读取json文件的数据并存入内存
        /// </summary>
        /// <returns>The spine json object dic.</returns>
        /// <param name="jsonFile">Json file.</param>
        public Dictionary<String, Object> GetSpineJsonObjectDic (UnityEngine.TextAsset jsonFile) {
            var input = new StringReader (jsonFile.text);
            return ReadSkeletonData (input);
        }
        /// <summary>
        /// 读取数据
        /// </summary>
        /// <returns>The skeleton data.</returns>
        /// <param name="reader">Reader.</param>
        private Dictionary<String, Object> ReadSkeletonData (TextReader reader) {
            if (reader == null) {
                throw new NullReferenceException ("reader == null");
            }
            root = Json.Deserialize (reader) as Dictionary<String, Object>;
            return root;
        }
        /// <summary>
        /// 获取json文件中的skins下的数据
        /// </summary>
        /// <returns>The target object dic.</returns>
        /// <param name="skinName">Skin name.</param>
        /// <param name="slotName">Slot name.</param>
        /// <param name="attachmentName">Attachment name.</param>
        public Dictionary<String, Object> GetTargetObjectDic (string skinName, string slotName, string attachmentName) {
            Dictionary<String, Object> o = GetDictionaryTypeObject (GetDictionaryTypeObject (root[SpineConstValue.rootKey]) [skinName]);
            if(null == slotName) return GetDictionaryTypeObject (null);
            if (o.ContainsKey (slotName)) {
                return GetDictionaryTypeObject (GetDictionaryTypeObject (o[slotName]) [attachmentName]);
            } else {
                return GetDictionaryTypeObject (null);
            }
        }
        //重组Json
        private bool isFinshedThread;
        private object lockd = new object ();
        private System.Threading.Thread thread;
        private RoleAnaimationData animationData;
        private string equipmentJson;
        private List<string> slotAttachmentNameList;
        private Spine.Unity.SkeletonAnimation skeletonAnim;
        private Action<bool> setRoleStatus;
        public System.Collections.IEnumerator CreateNewRoleJsonDic (RoleAnaimationData animationData, UnityEngine.TextAsset equipmentJson, List<string> slotAttachmentNameList, Spine.Unity.SkeletonAnimation skeletonAnim, Action<bool> setRoleStatus, bool isCreateNewJson = false) {
            if (isCreateNewJson) {
                #region 注释掉了
                // //重新绑定引用（更换attachment之后顶点动画引用丢失的情况）
                // //1.把当前装备所替换的attachment保存起来，再分别找到各个attachment在当前装备json中的顶点动画数据。
                // //2.把各个attachment的顶点动画数据复制到roleJson中。
                // //3.利用新的roleJson生成新的动画并和新的attachment进行绑定。(可以用litjson插件替换json里面的数据)
                // Dictionary<string, JsonData> dic = new Dictionary<string, JsonData> ();
                // JsonData allDataFromEquipment = JsonMapper.ToObject (equipmentJson.text);
                // JsonData animDataFromEquipment = allDataFromEquipment.TryGetData ("animations");
                // string[] animNamesFromEquipment = new string[animDataFromEquipment.Count];
                // animDataFromEquipment.Keys.CopyTo (animNamesFromEquipment, 0);
                // //1.先从当前装备的json中拿出来
                // for (int i = 0; i < slotAttachmentNameList.Count; i++) {
                //     for (int j = 0; j < animDataFromEquipment.Count; j++) {
                //         JsonData deformData = animDataFromEquipment[j].TryGetData ("deform");
                //         if (null != deformData) {
                //             JsonData slotData = deformData.TryGetData (SpineConstValue.defaultSkinName).TryGetData (slotAttachmentNameList[i].Split ('$') [0]);
                //             if (null != slotData) {
                //                 JsonData attachmentData = slotData.TryGetData (slotAttachmentNameList[i].Split ('$') [1]);
                //                 dic.Add (animNamesFromEquipment[j] + "$deform$" + SpineConstValue.defaultSkinName + "$" + slotAttachmentNameList[i].Split ('$') [0] + "$" + slotAttachmentNameList[i].Split ('$') [1], attachmentData);
                //             }
                //         }
                //         yield return null;
                //     }
                // }
                // //2.重新组装到roleJson中
                // JsonData allDataFromRoleJson = JsonMapper.ToObject (animationData.roleJsonStr);
                // foreach (KeyValuePair<string, JsonData> keyValue in dic) {
                //     JsonData animsData = allDataFromRoleJson.TryGetData ("animations");
                //     string[] keys = keyValue.Key.Split ('$');
                //     int length = keys.Length;
                //     if (null != animsData) {
                //         for (int i = 0; i < length; i++) {
                //             animsData = animsData.TryGetData (keys[i]);
                //             if (null != animsData) {
                //                 if (i >= length - 1) {
                //                     string newJsonStr = keyValue.Value.ToJson ().Substring (1, keyValue.Value.ToJson ().Length - 2);
                //                     animsData.Add (JsonMapper.ToObject (newJsonStr));
                //                     yield return null;
                //                 }
                //                 continue;
                //             } else {
                //                 break;
                //             }
                //         }
                //     }
                // }
                // //3.刷新json
                // animationData.roleJsonStr = allDataFromRoleJson.ToJson ().ToString ();
                #endregion
                skeletonAnim.skeleton.data.animations.Clear (true);
                this.animationData = animationData;
                this.equipmentJson = equipmentJson.text;
                this.slotAttachmentNameList = slotAttachmentNameList;
                this.skeletonAnim = skeletonAnim;
                this.setRoleStatus = setRoleStatus;
                thread = new System.Threading.Thread (new System.Threading.ThreadStart (CopyJsonData));
                thread.IsBackground = false;
                thread.Start ();
                while (!isFinshedThread) {
                    yield return null;
                }
                var input = new StringReader (this.animationData.roleJsonStr);
                Dictionary<String, System.Object> tempDic = GetDictionaryTypeObject ((Json.Deserialize (input) as Dictionary<String, Object>) ["animations"]);
                if (null != tempDic) {
                    foreach (KeyValuePair<string, System.Object> entry in tempDic) {
                        try {
                            SpineCreateAttachment.Instance.ReadAnimation ((Dictionary<string, System.Object>) entry.Value, entry.Key, skeletonAnim.Skeleton.Data);
                        } catch (Exception e) {
                            throw new Exception ("Error reading animation: " + entry.Key, e);
                        }
                    }
                    skeletonAnim.AnimationState.SetAnimation (SpineConstValue.TrackIndex, skeletonAnim.AnimationState.GetCurrent (SpineConstValue.TrackIndex).animation.name, true);
                }
            }
        }
        private void CopyJsonData () {
            //重新绑定引用（更换attachment之后顶点动画引用丢失的情况）
            //1.把当前装备所替换的attachment保存起来，再分别找到各个attachment在当前装备json中的顶点动画数据。
            //2.把各个attachment的顶点动画数据复制到roleJson中。
            //3.利用新的roleJson生成新的动画并和新的attachment进行绑定。(可以用litjson插件替换json里面的数据)
            while (true) {
                lock (lockd) {
                    isFinshedThread = false;
                    Dictionary<string, JsonData> dic = new Dictionary<string, JsonData> ();
                    JsonData allDataFromEquipment = JsonMapper.ToObject (equipmentJson);
                    JsonData animDataFromEquipment = allDataFromEquipment.TryGetData ("animations");
                    string[] animNamesFromEquipment = new string[animDataFromEquipment.Count];
                    animDataFromEquipment.Keys.CopyTo (animNamesFromEquipment, 0);
                    //1.先从当前装备的json中拿出来
                    for (int i = 0; i < slotAttachmentNameList.Count; i++) {
                        for (int j = 0; j < animDataFromEquipment.Count; j++) {
                            JsonData deformData = animDataFromEquipment[j].TryGetData ("deform");
                            if (null != deformData) {
                                JsonData slotData = deformData.TryGetData (SpineConstValue.defaultSkinName).TryGetData (slotAttachmentNameList[i].Split ('$') [0]);
                                if (null != slotData) {
                                    JsonData attachmentData = slotData.TryGetData (slotAttachmentNameList[i].Split ('$') [1]);
                                    dic.Add (animNamesFromEquipment[j] + "$deform$" + SpineConstValue.defaultSkinName + "$" + slotAttachmentNameList[i].Split ('$') [0] + "$" + slotAttachmentNameList[i].Split ('$') [1], attachmentData);
                                }
                            }
                        }
                    }
                    //2.重新组装到roleJson中
                    JsonData allDataFromRoleJson = JsonMapper.ToObject (animationData.roleJsonStr);
                    foreach (KeyValuePair<string, JsonData> keyValue in dic) {
                        JsonData animsData = allDataFromRoleJson.TryGetData ("animations");
                        string[] keys = keyValue.Key.Split ('$');
                        int length = keys.Length;
                        if (null != animsData) {
                            for (int i = 0; i < length; i++) {
                                animsData = animsData.TryGetData (keys[i]);
                                if (null != animsData) {
                                    if (i >= length - 1) {
                                        //string newJsonStr = keyValue.Value.ToJson ().Substring (1, keyValue.Value.ToJson ().Length - 2);
                                        animsData.Clear ();
                                        for (int j = 0; j < keyValue.Value.Count; j++) {
                                            animsData.Add (keyValue.Value[j]);
                                        }
                                        //animsData.Add (keyValue.Value.TryGetData());
                                    }
                                    continue;
                                } else {
                                    break;
                                }
                            }
                        }
                    }
                    //3.刷新json
                    animationData.roleJsonStr = allDataFromRoleJson.ToJson ();
                    isFinshedThread = true;
                    break;
                }
            }
        }
        /// <summary>
        /// 类型转换
        /// </summary>
        /// <returns>The dic.</returns>
        /// <param name="o">O.</param>
        private Dictionary<String, Object> GetDictionaryTypeObject (Object o) {
            if (null == o) {
                return null;
            }
            return (Dictionary<String, Object>) o;
        }
        /// <summary>
        /// 从json数据中获取float[]
        /// </summary>
        /// <returns>The float array.</returns>
        /// <param name="map">Map.</param>
        /// <param name="name">Name.</param>
        /// <param name="scale">Scale.</param>
        public float[] GetFloatArray (Dictionary<String, Object> map, String name, float scale) {
            var list = (List<Object>) map[name];
            var values = new float[list.Count];
            if (scale == 1) {
                for (int i = 0, n = list.Count; i < n; i++)
                    values[i] = (float) list[i];
            } else {
                for (int i = 0, n = list.Count; i < n; i++)
                    values[i] = (float) list[i] * scale;
            }
            return values;
        }
        /// <summary>
        /// 从json数据中获取int[]
        /// </summary>
        /// <returns>The int array.</returns>
        /// <param name="map">Map.</param>
        /// <param name="name">Name.</param>
        public int[] GetIntArray (Dictionary<String, Object> map, String name) {
            var list = (List<Object>) map[name];
            var values = new int[list.Count];
            for (int i = 0, n = list.Count; i < n; i++)
                values[i] = (int) (float) list[i];
            return values;
        }
        /// <summary>
        /// 从json数据中获取int
        /// </summary>
        /// <returns>The float.</returns>
        /// <param name="map">Map.</param>
        /// <param name="name">Name.</param>
        /// <param name="defaultValue">Default value.</param>
        public float GetFloat (Dictionary<String, Object> map, String name, float defaultValue) {
            if (!map.ContainsKey (name))
                return defaultValue;
            return (float) map[name];
        }
        /// <summary>
        /// 从json数据中获取int
        /// </summary>
        /// <returns>The int.</returns>
        /// <param name="map">Map.</param>
        /// <param name="name">Name.</param>
        /// <param name="defaultValue">Default value.</param>
        public int GetInt (Dictionary<String, Object> map, String name, int defaultValue) {
            if (!map.ContainsKey (name))
                return defaultValue;
            return (int) (float) map[name];
        }
        /// <summary>
        /// 从json数据中获取bool
        /// </summary>
        /// <returns><c>true</c>, if boolean was gotten, <c>false</c> otherwise.</returns>
        /// <param name="map">Map.</param>
        /// <param name="name">Name.</param>
        /// <param name="defaultValue">If set to <c>true</c> default value.</param>
        public bool GetBoolean (Dictionary<String, Object> map, String name, bool defaultValue) {
            if (!map.ContainsKey (name))
                return defaultValue;
            return (bool) map[name];
        }
        /// <summary>
        /// 从json数据中获取string.
        /// </summary>
        /// <returns>The string.</returns>
        /// <param name="map">Map.</param>
        /// <param name="name">Name.</param>
        /// <param name="defaultValue">Default value.</param>
        public String GetString (Dictionary<String, Object> map, String name, String defaultValue) {
            if (!map.ContainsKey (name))
                return defaultValue;
            return (String) map[name];
        }
        /// <summary>
        /// Tos the color.
        /// </summary>
        /// <returns>The color.</returns>
        /// <param name="hexString">Hex string.</param>
        /// <param name="colorIndex">Color index.</param>
        /// <param name="expectedLength">Expected length.</param>
        public float ToColor (String hexString, int colorIndex, int expectedLength = 8) {
            if (hexString.Length != expectedLength)
                throw new ArgumentException ("Color hexidecimal length must be " + expectedLength + ", recieved: " + hexString, "hexString");
            return Convert.ToInt32 (hexString.Substring (colorIndex * 2, 2), 16) / (float) 255;
        }
        /// <summary>
        /// 获取attachment的Vertices
        /// </summary>
        /// <param name="map">Map.</param>
        /// <param name="attachment">Attachment.</param>
        /// <param name="verticesLength">Vertices length.</param>
        public void ReadVertices (Dictionary<String, Object> map, VertexAttachment attachment, int verticesLength) {
            attachment.WorldVerticesLength = verticesLength;
            float[] vertices = GetFloatArray (map, "vertices", 1);
            float scale = SpineConstValue.scale;
            if (verticesLength == vertices.Length) {
                if (scale != 1) {
                    for (int i = 0; i < vertices.Length; i++) {
                        vertices[i] *= scale;
                    }
                }
                attachment.vertices = vertices;
                return;
            }
            ExposedList<float> weights = new ExposedList<float> (verticesLength * 3 * 3);
            ExposedList<int> bones = new ExposedList<int> (verticesLength * 3);
            for (int i = 0, n = vertices.Length; i < n;) {
                int boneCount = (int) vertices[i++];
                bones.Add (boneCount);
                for (int nn = i + boneCount * 4; i < nn; i += 4) {
                    bones.Add ((int) vertices[i]);
                    weights.Add (vertices[i + 1] * scale);
                    weights.Add (vertices[i + 2] * scale);
                    weights.Add (vertices[i + 3]);
                }
            }
            attachment.bones = bones.ToArray ();
            attachment.vertices = weights.ToArray ();
        }
    }
}