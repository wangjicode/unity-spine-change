using System;
using System.Collections;
using System.Collections.Generic;
using Spine;
using UnityEngine;

namespace DancingWord.SpineChange {
    public class SpineCreateAttachment : SpineAttachment {

        private static SpineCreateAttachment _instance;
        public static SpineCreateAttachment Instance {
            get {
                if (_instance == null) {
                    _instance = new SpineCreateAttachment ();
                }
                return _instance;
            }
        }
        private SpineCreateAttachment () { }
        /// <summary>
        /// 通过AtlasRegion重建MeshAttachment
        /// </summary>
        /// <returns>The mesh attachment by atlas region.</returns>
        /// <param name="allAttachmentName">Attachment name.</param>
        /// <param name="atlasRegion">Atlas region.</param>
        public override Attachment CreateMeshAttachmentByAtlasRegion (Spine.Unity.SkeletonAnimation skeletonAnim, string allAttachmentName, AtlasRegion atlasRegion) {
            if (atlasRegion == null) {
                throw new NullReferenceException ("atlasRegion == null");
            }
            string attachmentName = SpineChangeDataLogic.Instance.GetAttachmentFromAllAttachmnetName (allAttachmentName);
            MeshAttachment meshAttachment = new MeshAttachment (attachmentName);
            meshAttachment.RendererObject = atlasRegion;
            meshAttachment.RegionU = atlasRegion.u;
            meshAttachment.RegionV = atlasRegion.v;
            meshAttachment.RegionU2 = atlasRegion.u2;
            meshAttachment.RegionV2 = atlasRegion.v2;
            meshAttachment.RegionRotate = atlasRegion.rotate;
            meshAttachment.regionOffsetX = atlasRegion.offsetX;
            meshAttachment.regionOffsetY = atlasRegion.offsetY;
            meshAttachment.regionWidth = atlasRegion.width;
            meshAttachment.regionHeight = atlasRegion.height;
            meshAttachment.regionOriginalWidth = atlasRegion.originalWidth;
            meshAttachment.regionOriginalHeight = atlasRegion.originalHeight;
            Dictionary<String, System.Object> root = SpineReadJsonDataLogic.Instance.GetTargetObjectDic (SpineConstValue.defaultSkinName,
                SpineChangeCoreLogic.Instance.GetSlotNameBySlotIndex (skeletonAnim,
                    SpineChangeDataLogic.Instance.GetSlotIndexFromAllAttachmentName (
                        allAttachmentName)),
                attachmentName);
            SetMeshAttachmentFromJsonFile (meshAttachment, root);
            String parent = SpineReadJsonDataLogic.Instance.GetString (root, "parent", null);
            if (null != parent) {
                MeshAttachment ma = (MeshAttachment) skeletonAnim.skeleton.GetAttachment (SpineChangeCoreLogic.Instance.GetSlotNameBySlotIndex (skeletonAnim,
                    SpineChangeDataLogic.Instance.GetSlotIndexFromAllAttachmentName (
                        allAttachmentName)), parent);
                if (null != ma) {
                    meshAttachment.ParentMesh = ma;
                }
            }
            meshAttachment.UpdateUVs ();
            return meshAttachment;
        }
        /// <summary>
        /// 把除AtlasRegion外的信息通过读取json中的数据赋值给attachment
        /// </summary>
        /// <param name="mesh">Mesh.</param>
        /// <param name="map">Map.</param>
        private void SetMeshAttachmentFromJsonFile (MeshAttachment mesh, Dictionary<String, System.Object> map) {
            if (mesh == null) return;
            string name = SpineReadJsonDataLogic.Instance.GetString (map, "name", mesh.Name);
            mesh.Path = SpineReadJsonDataLogic.Instance.GetString (map, "path", name);
            if (map.ContainsKey ("color")) {
                var color = (String) map["color"];
                mesh.r = SpineReadJsonDataLogic.Instance.ToColor (color, 0);
                mesh.g = SpineReadJsonDataLogic.Instance.ToColor (color, 1);
                mesh.b = SpineReadJsonDataLogic.Instance.ToColor (color, 2);
                mesh.a = SpineReadJsonDataLogic.Instance.ToColor (color, 3);
            }

            mesh.Width = SpineReadJsonDataLogic.Instance.GetFloat (map, "width", 0) * SpineConstValue.scale;
            mesh.Height = SpineReadJsonDataLogic.Instance.GetFloat (map, "height", 0) * SpineConstValue.scale;

            String parent = SpineReadJsonDataLogic.Instance.GetString (map, "parent", null);
            if (parent != null) {
                mesh.InheritDeform = SpineReadJsonDataLogic.Instance.GetBoolean (map, "deform", true);
                return;
            }

            float[] uvs = SpineReadJsonDataLogic.Instance.GetFloatArray (map, "uvs", 1);
            SpineReadJsonDataLogic.Instance.ReadVertices (map, mesh, uvs.Length);
            mesh.triangles = SpineReadJsonDataLogic.Instance.GetIntArray (map, "triangles");
            mesh.regionUVs = uvs;
            if (map.ContainsKey ("hull")) mesh.HullLength = SpineReadJsonDataLogic.Instance.GetInt (map, "hull", 0) * 2;
            if (map.ContainsKey ("edges")) mesh.Edges = SpineReadJsonDataLogic.Instance.GetIntArray (map, "edges");
        }
        /// <summary>
        /// 把除AtlasRegion外的信息通过读取json中的数据赋值给attachment
        /// </summary>
        /// <param name="region">Region.</param>
        /// <param name="map">Map.</param>
        private void SetRegionAttachmentFromJsonFile (RegionAttachment region, Dictionary<String, System.Object> map) {
            if (region == null) return;
            string name = SpineReadJsonDataLogic.Instance.GetString (map, "name", region.Name);
            region.Path = SpineReadJsonDataLogic.Instance.GetString (map, "path", name);
            region.x = SpineReadJsonDataLogic.Instance.GetFloat (map, "x", 0) * SpineConstValue.scale;
            region.y = SpineReadJsonDataLogic.Instance.GetFloat (map, "y", 0) * SpineConstValue.scale;
            region.scaleX = SpineReadJsonDataLogic.Instance.GetFloat (map, "scaleX", 1);
            region.scaleY = SpineReadJsonDataLogic.Instance.GetFloat (map, "scaleY", 1);
            region.rotation = SpineReadJsonDataLogic.Instance.GetFloat (map, "rotation", 0);
            region.width = SpineReadJsonDataLogic.Instance.GetFloat (map, "width", 32) * SpineConstValue.scale;
            region.height = SpineReadJsonDataLogic.Instance.GetFloat (map, "height", 32) * SpineConstValue.scale;
            region.UpdateOffset ();

            if (map.ContainsKey ("color")) {
                var color = (String) map["color"];
                region.r = SpineReadJsonDataLogic.Instance.ToColor (color, 0);
                region.g = SpineReadJsonDataLogic.Instance.ToColor (color, 1);
                region.b = SpineReadJsonDataLogic.Instance.ToColor (color, 2);
                region.a = SpineReadJsonDataLogic.Instance.ToColor (color, 3);
            }
        }
        /// <summary>
        /// 通过Texture2D重建MeshAttachment
        /// </summary>
        /// <returns>The mesh attachment by texture.</returns>
        /// <param name="slotName">Slot name.</param>
        /// <param name="texture">Texture.</param>
        public override Attachment CreateMeshAttachmentByTexture (Spine.Unity.SkeletonAnimation sAnim, string slotName, Texture2D texture) {
            Slot slot = sAnim.skeleton.FindSlot (slotName);
            if (slot == null && texture == null) {
                throw new System.NullReferenceException ("slot == null || texture == null");
            }
            MeshAttachment oldMeshAttachment = slot.Attachment as MeshAttachment;
            if (oldMeshAttachment == null) {
                return null;
            }
            if (oldMeshAttachment == null) {
                throw new System.NullReferenceException ("oldMeshAttachment == null");
            }
            MeshAttachment newMeshAttachment = new MeshAttachment (oldMeshAttachment.Name);
            newMeshAttachment.RendererObject = CreateAtlasRegionFromTexture (texture);
            newMeshAttachment.Path = oldMeshAttachment.Path;
            newMeshAttachment.Bones = oldMeshAttachment.Bones;
            newMeshAttachment.Edges = oldMeshAttachment.Edges;
            newMeshAttachment.Triangles = oldMeshAttachment.Triangles;
            newMeshAttachment.Vertices = oldMeshAttachment.Vertices;
            newMeshAttachment.WorldVerticesLength = oldMeshAttachment.WorldVerticesLength;
            newMeshAttachment.HullLength = oldMeshAttachment.HullLength;
            newMeshAttachment.RegionRotate = false;
            newMeshAttachment.RegionU = 0f;
            newMeshAttachment.RegionV = 1f;
            newMeshAttachment.RegionU2 = 1f;
            newMeshAttachment.RegionV2 = 0f;
            newMeshAttachment.RegionUVs = oldMeshAttachment.RegionUVs;
            Material material = new Material (Shader.Find (SpineConstValue.shaderPath));
            material.mainTexture = texture;
            (newMeshAttachment.RendererObject as AtlasRegion).page.rendererObject = material;
            newMeshAttachment.UpdateUVs ();
            return newMeshAttachment;
        }
        /// <summary>
        /// 通过AtlasRegion重建RegionAttachment
        /// </summary>
        /// <returns>The region attachment by atlas region.</returns>
        /// <param name="allAttachmentName">Attachment name.</param>
        /// <param name="atlasRegion">Atlas region.</param>
        public override Attachment CreateRegionAttachmentByAtlasRegion (Spine.Unity.SkeletonAnimation skeletonAnim, string allAttachmentName, AtlasRegion atlasRegion) {
            if (atlasRegion == null) {
                throw new NullReferenceException ("slot == null || texture == null");
            }
            string attachmentName = SpineChangeDataLogic.Instance.GetAttachmentFromAllAttachmnetName (allAttachmentName);
            RegionAttachment regionAttachment = new RegionAttachment (attachmentName);
            regionAttachment.RendererObject = atlasRegion;
            regionAttachment.SetUVs (atlasRegion.u, atlasRegion.v, atlasRegion.u2, atlasRegion.v2, atlasRegion.rotate);
            regionAttachment.regionOffsetX = atlasRegion.offsetX;
            regionAttachment.regionOffsetY = atlasRegion.offsetY;
            regionAttachment.regionWidth = atlasRegion.width;
            regionAttachment.regionHeight = atlasRegion.height;
            regionAttachment.regionOriginalWidth = atlasRegion.originalWidth;
            regionAttachment.regionOriginalHeight = atlasRegion.originalHeight;
            Dictionary<String, System.Object> root = SpineReadJsonDataLogic.Instance.GetTargetObjectDic (SpineConstValue.defaultSkinName,
                SpineChangeCoreLogic.Instance.GetSlotNameBySlotIndex (skeletonAnim,
                    SpineChangeDataLogic.Instance.GetSlotIndexFromAllAttachmentName (
                        allAttachmentName)),
                attachmentName);
            SetRegionAttachmentFromJsonFile (regionAttachment, root);
            regionAttachment.UpdateOffset ();
            return regionAttachment;
        }
        /// <summary>
        /// 通过Texture2D重建RegionAttachment
        /// </summary>
        /// <returns>The region attachment by texture.</returns>
        /// <param name="slotName">Slot name.</param>
        /// <param name="texture">Texture.</param>
        public override Attachment CreateRegionAttachmentByTexture (Spine.Unity.SkeletonAnimation sAnim, string slotName, Texture2D texture) {
            Slot slot = sAnim.skeleton.FindSlot (slotName);
            if (slot == null && texture == null) {
                throw new NullReferenceException ("slot == null || texture == null");
            }
            RegionAttachment regionAttachment = slot.Attachment as RegionAttachment;
            if (regionAttachment == null) {
                throw new NullReferenceException ("regionAttachment == null");
            }
            regionAttachment.RendererObject = CreateAtlasRegionFromTexture (texture);
            regionAttachment.SetUVs (0f, 1f, 1f, 0f, false);
            Material material = new Material (Shader.Find (SpineConstValue.shaderPath));
            material.mainTexture = texture;
            (regionAttachment.RendererObject as AtlasRegion).page.rendererObject = material;
            regionAttachment.UpdateOffset ();
            return regionAttachment;
        }
        /// <summary>
        /// 创建BoundingBoxattachment
        /// </summary>
        /// <returns>The bounding box attachment by name.</returns>
        /// <param name="allAttachmentName">All attachment name.</param>
        public override Attachment CreateBoundingBoxAttachmentByName (Spine.Unity.SkeletonAnimation skeletonAnim, string allAttachmentName) {
            if (string.IsNullOrEmpty (allAttachmentName)) {
                throw new NullReferenceException ();
            }
            BoundingBoxAttachment boundingBoxAttachment = new BoundingBoxAttachment (SpineChangeDataLogic.Instance.GetAttachmentFromAllAttachmnetName (allAttachmentName));
            Dictionary<String, System.Object> root = SpineReadJsonDataLogic.Instance.GetTargetObjectDic (SpineConstValue.defaultSkinName,
                SpineChangeCoreLogic.Instance.GetSlotNameBySlotIndex (skeletonAnim,
                    SpineChangeDataLogic.Instance.GetSlotIndexFromAllAttachmentName (
                        allAttachmentName)),
                SpineChangeDataLogic.Instance.GetAttachmentFromAllAttachmnetName (allAttachmentName));
            SetBoundingBoxAttachmentFromJsonFile (boundingBoxAttachment, root);
            return boundingBoxAttachment;
        }
        /// <summary>
        /// 获取json中的数据到BoundingBoxattachment
        /// </summary>
        /// <param name="box">Box.</param>
        /// <param name="map">Map.</param>
        private void SetBoundingBoxAttachmentFromJsonFile (BoundingBoxAttachment box, Dictionary<String, System.Object> map) {
            if (box == null) return;
            SpineReadJsonDataLogic.Instance.ReadVertices (map, box, SpineReadJsonDataLogic.Instance.GetInt (map, "vertexCount", 0) << 1);
        }
        /// <summary>
        /// 创建ClippingAttachment
        /// </summary>
        /// <returns>The clipping attachment by name.</returns>
        /// <param name="allAttachmentName">All attachment name.</param>
        public override Attachment CreateClippingAttachmentByName (Spine.Unity.SkeletonAnimation skeletonAnim, string allAttachmentName) {
            if (string.IsNullOrEmpty (allAttachmentName)) {
                throw new NullReferenceException ();
            }
            ClippingAttachment clippingAttachment = new ClippingAttachment (SpineChangeDataLogic.Instance.GetAttachmentFromAllAttachmnetName (allAttachmentName));
            Dictionary<String, System.Object> root = SpineReadJsonDataLogic.Instance.GetTargetObjectDic (SpineConstValue.defaultSkinName,
                SpineChangeCoreLogic.Instance.GetSlotNameBySlotIndex (skeletonAnim,
                    SpineChangeDataLogic.Instance.GetSlotIndexFromAllAttachmentName (
                        allAttachmentName)),
                SpineChangeDataLogic.Instance.GetAttachmentFromAllAttachmnetName (allAttachmentName));
            SetClippingAttachmentFromJsonFile (clippingAttachment, root);
            return clippingAttachment;
        }
        /// <summary>
        /// 获取json中的数据到ClippingAttachment
        /// </summary>
        /// <param name="clip">Clip.</param>
        /// <param name="map">Map.</param>
        private void SetClippingAttachmentFromJsonFile (ClippingAttachment clip, Dictionary<String, System.Object> map) {
            //暂时先注释掉 SpineChangeCoreLogic.Instance.skeletonAnimation 报错
            // if (clip == null) return;

            // string end = SpineReadJsonDataLogic.Instance.GetString (map, "end", null);
            // if (end != null) {
            //     SlotData slot = SpineChangeCoreLogic.Instance.skeletonAnimation.Skeleton.Data.FindSlot (end);
            //     if (slot == null) throw new Exception ("Clipping end slot not found: " + end);
            //     clip.EndSlot = slot;
            // }
            // SpineReadJsonDataLogic.Instance.ReadVertices (map, clip, SpineReadJsonDataLogic.Instance.GetInt (map, "vertexCount", 0) << 1);
        }
        /// <summary>
        /// 创建PathAttachment
        /// </summary>
        /// <returns>The path attachment by name.</returns>
        /// <param name="allAttachmentName">All attachment name.</param>
        public override Attachment CreatePathAttachmentByName (Spine.Unity.SkeletonAnimation skeletonAnim, string allAttachmentName) {
            if (string.IsNullOrEmpty (allAttachmentName)) {
                throw new NullReferenceException ();
            }
            PathAttachment pathAttachment = new PathAttachment (SpineChangeDataLogic.Instance.GetAttachmentFromAllAttachmnetName (allAttachmentName));
            Dictionary<String, System.Object> root = SpineReadJsonDataLogic.Instance.GetTargetObjectDic (SpineConstValue.defaultSkinName,
                SpineChangeCoreLogic.Instance.GetSlotNameBySlotIndex (skeletonAnim,
                    SpineChangeDataLogic.Instance.GetSlotIndexFromAllAttachmentName (
                        allAttachmentName)),
                SpineChangeDataLogic.Instance.GetAttachmentFromAllAttachmnetName (allAttachmentName));
            SetPathAttachmentFromJsonFile (pathAttachment, root);
            return pathAttachment;
        }
        /// <summary>
        /// 获取json中的数据到PathAttachment
        /// </summary>
        /// <param name="pathAttachment">Path attachment.</param>
        /// <param name="map">Map.</param>
        private void SetPathAttachmentFromJsonFile (PathAttachment pathAttachment, Dictionary<String, System.Object> map) {
            if (pathAttachment == null) return;
            pathAttachment.closed = SpineReadJsonDataLogic.Instance.GetBoolean (map, "closed", false);
            pathAttachment.constantSpeed = SpineReadJsonDataLogic.Instance.GetBoolean (map, "constantSpeed", true);
            int vertexCount = SpineReadJsonDataLogic.Instance.GetInt (map, "vertexCount", 0);
            SpineReadJsonDataLogic.Instance.ReadVertices (map, pathAttachment, vertexCount << 1);
            pathAttachment.lengths = SpineReadJsonDataLogic.Instance.GetFloatArray (map, "lengths", SpineConstValue.scale);
        }
        /// <summary>
        /// 创建PointAttachment
        /// </summary>
        /// <returns>The point attachment by name.</returns>
        /// <param name="allAttachmentName">All attachment name.</param>
        public override Attachment CreatePointAttachmentByName (Spine.Unity.SkeletonAnimation skeletonAnim, string allAttachmentName) {
            if (string.IsNullOrEmpty (allAttachmentName)) {
                throw new NullReferenceException ();
            }
            PointAttachment pointAttachment = new PointAttachment (SpineChangeDataLogic.Instance.GetAttachmentFromAllAttachmnetName (allAttachmentName));
            Dictionary<String, System.Object> root = SpineReadJsonDataLogic.Instance.GetTargetObjectDic (SpineConstValue.defaultSkinName,
                SpineChangeCoreLogic.Instance.GetSlotNameBySlotIndex (skeletonAnim,
                    SpineChangeDataLogic.Instance.GetSlotIndexFromAllAttachmentName (
                        allAttachmentName)),
                SpineChangeDataLogic.Instance.GetAttachmentFromAllAttachmnetName (allAttachmentName));
            SetPointAttachmentFromJsonFile (pointAttachment, root);
            return new PointAttachment (allAttachmentName);
        }
        /// <summary>
        /// 获取json中的数据到PointAttachment
        /// </summary>
        /// <param name="point">Point.</param>
        /// <param name="map">Map.</param>
        private void SetPointAttachmentFromJsonFile (PointAttachment point, Dictionary<String, System.Object> map) {
            if (point == null) return;
            point.x = SpineReadJsonDataLogic.Instance.GetFloat (map, "x", 0) * SpineConstValue.scale;
            point.y = SpineReadJsonDataLogic.Instance.GetFloat (map, "y", 0) * SpineConstValue.scale;
            point.rotation = SpineReadJsonDataLogic.Instance.GetFloat (map, "rotation", 0);
        }
        /// <summary>
        /// 通过Texture2D重建AtlasRegion
        /// </summary>
        /// <returns>The atlas region from texture.</returns>
        /// <param name="texture2D">Texture2 d.</param>
        private AtlasRegion CreateAtlasRegionFromTexture (Texture2D texture2D) {
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

        public void ReadAnimation (Dictionary<string, System.Object> map, string name, SkeletonData skeletonData) {
            var scale = 0.01f;
            var timelines = new ExposedList<Timeline> ();
            float duration = 0;

            // Slot timelines.
            if (map.ContainsKey ("slots")) {
                foreach (KeyValuePair<string, System.Object> entry in (Dictionary<string, System.Object>) map["slots"]) {
                    string slotName = entry.Key;
                    int slotIndex = skeletonData.FindSlotIndex (slotName);
                    var timelineMap = (Dictionary<string, System.Object>) entry.Value;
                    foreach (KeyValuePair<string, System.Object> timelineEntry in timelineMap) {
                        var values = (List<System.Object>) timelineEntry.Value;
                        var timelineName = (string) timelineEntry.Key;
                        if (timelineName == "attachment") {
                            var timeline = new AttachmentTimeline (values.Count);
                            timeline.slotIndex = slotIndex;

                            int frameIndex = 0;
                            foreach (Dictionary<string, System.Object> valueMap in values) {
                                float time = (float) valueMap["time"];
                                timeline.SetFrame (frameIndex++, time, (string) valueMap["name"]);
                            }
                            timelines.Add (timeline);
                            duration = Math.Max (duration, timeline.frames[timeline.FrameCount - 1]);

                        } else if (timelineName == "color") {
                            var timeline = new ColorTimeline (values.Count);
                            timeline.slotIndex = slotIndex;

                            int frameIndex = 0;
                            foreach (Dictionary<string, System.Object> valueMap in values) {
                                float time = (float) valueMap["time"];
                                string c = (string) valueMap["color"];
                                timeline.SetFrame (frameIndex, time, SpineReadJsonDataLogic.Instance.ToColor (c, 0), SpineReadJsonDataLogic.Instance.ToColor (c, 1), SpineReadJsonDataLogic.Instance.ToColor (c, 2), SpineReadJsonDataLogic.Instance.ToColor (c, 3));
                                ReadCurve (valueMap, timeline, frameIndex);
                                frameIndex++;
                            }
                            timelines.Add (timeline);
                            duration = Math.Max (duration, timeline.frames[(timeline.FrameCount - 1) * ColorTimeline.ENTRIES]);

                        } else if (timelineName == "twoColor") {
                            var timeline = new TwoColorTimeline (values.Count);
                            timeline.slotIndex = slotIndex;

                            int frameIndex = 0;
                            foreach (Dictionary<string, System.Object> valueMap in values) {
                                float time = (float) valueMap["time"];
                                string light = (string) valueMap["light"];
                                string dark = (string) valueMap["dark"];
                                timeline.SetFrame (frameIndex, time, SpineReadJsonDataLogic.Instance.ToColor (light, 0), SpineReadJsonDataLogic.Instance.ToColor (light, 1), SpineReadJsonDataLogic.Instance.ToColor (light, 2), SpineReadJsonDataLogic.Instance.ToColor (light, 3),
                                    SpineReadJsonDataLogic.Instance.ToColor (dark, 0, 6), SpineReadJsonDataLogic.Instance.ToColor (dark, 1, 6), SpineReadJsonDataLogic.Instance.ToColor (dark, 2, 6));
                                ReadCurve (valueMap, timeline, frameIndex);
                                frameIndex++;
                            }
                            timelines.Add (timeline);
                            duration = Math.Max (duration, timeline.frames[(timeline.FrameCount - 1) * TwoColorTimeline.ENTRIES]);

                        } else
                            throw new Exception ("Invalid timeline type for a slot: " + timelineName + " (" + slotName + ")");
                    }
                }
            }

            // Bone timelines.
            if (map.ContainsKey ("bones")) {
                foreach (KeyValuePair<string, System.Object> entry in (Dictionary<string, System.Object>) map["bones"]) {
                    string boneName = entry.Key;
                    int boneIndex = skeletonData.FindBoneIndex (boneName);
                    if (boneIndex == -1) throw new Exception ("Bone not found: " + boneName);
                    var timelineMap = (Dictionary<string, System.Object>) entry.Value;
                    foreach (KeyValuePair<string, System.Object> timelineEntry in timelineMap) {
                        var values = (List<System.Object>) timelineEntry.Value;
                        var timelineName = (string) timelineEntry.Key;
                        if (timelineName == "rotate") {
                            var timeline = new RotateTimeline (values.Count);
                            timeline.boneIndex = boneIndex;

                            int frameIndex = 0;
                            foreach (Dictionary<string, System.Object> valueMap in values) {
                                timeline.SetFrame (frameIndex, (float) valueMap["time"], (float) valueMap["angle"]);
                                ReadCurve (valueMap, timeline, frameIndex);
                                frameIndex++;
                            }
                            timelines.Add (timeline);
                            duration = Math.Max (duration, timeline.frames[(timeline.FrameCount - 1) * RotateTimeline.ENTRIES]);

                        } else if (timelineName == "translate" || timelineName == "scale" || timelineName == "shear") {
                            TranslateTimeline timeline;
                            float timelineScale = 1;
                            if (timelineName == "scale")
                                timeline = new ScaleTimeline (values.Count);
                            else if (timelineName == "shear")
                                timeline = new ShearTimeline (values.Count);
                            else {
                                timeline = new TranslateTimeline (values.Count);
                                timelineScale = scale;
                            }
                            timeline.boneIndex = boneIndex;

                            int frameIndex = 0;
                            foreach (Dictionary<string, System.Object> valueMap in values) {
                                float time = (float) valueMap["time"];
                                float x = SpineReadJsonDataLogic.Instance.GetFloat (valueMap, "x", 0);
                                float y = SpineReadJsonDataLogic.Instance.GetFloat (valueMap, "y", 0);
                                timeline.SetFrame (frameIndex, time, x * timelineScale, y * timelineScale);
                                ReadCurve (valueMap, timeline, frameIndex);
                                frameIndex++;
                            }
                            timelines.Add (timeline);
                            duration = Math.Max (duration, timeline.frames[(timeline.FrameCount - 1) * TranslateTimeline.ENTRIES]);

                        } else
                            throw new Exception ("Invalid timeline type for a bone: " + timelineName + " (" + boneName + ")");
                    }
                }
            }

            // IK constraint timelines.
            if (map.ContainsKey ("ik")) {
                foreach (KeyValuePair<string, System.Object> constraintMap in (Dictionary<string, System.Object>) map["ik"]) {
                    IkConstraintData constraint = skeletonData.FindIkConstraint (constraintMap.Key);
                    var values = (List<System.Object>) constraintMap.Value;
                    var timeline = new IkConstraintTimeline (values.Count);
                    timeline.ikConstraintIndex = skeletonData.ikConstraints.IndexOf (constraint);
                    int frameIndex = 0;
                    foreach (Dictionary<string, System.Object> valueMap in values) {
                        float time = (float) valueMap["time"];
                        float mix = SpineReadJsonDataLogic.Instance.GetFloat (valueMap, "mix", 1);
                        bool bendPositive = SpineReadJsonDataLogic.Instance.GetBoolean (valueMap, "bendPositive", true);
                        timeline.SetFrame (frameIndex, time, mix, bendPositive ? 1 : -1);
                        ReadCurve (valueMap, timeline, frameIndex);
                        frameIndex++;
                    }
                    timelines.Add (timeline);
                    duration = Math.Max (duration, timeline.frames[(timeline.FrameCount - 1) * IkConstraintTimeline.ENTRIES]);
                }
            }

            // Transform constraint timelines.
            if (map.ContainsKey ("transform")) {
                foreach (KeyValuePair<string, System.Object> constraintMap in (Dictionary<string, System.Object>) map["transform"]) {
                    TransformConstraintData constraint = skeletonData.FindTransformConstraint (constraintMap.Key);
                    var values = (List<System.Object>) constraintMap.Value;
                    var timeline = new TransformConstraintTimeline (values.Count);
                    timeline.transformConstraintIndex = skeletonData.transformConstraints.IndexOf (constraint);
                    int frameIndex = 0;
                    foreach (Dictionary<string, System.Object> valueMap in values) {
                        float time = (float) valueMap["time"];
                        float rotateMix = SpineReadJsonDataLogic.Instance.GetFloat (valueMap, "rotateMix", 1);
                        float translateMix = SpineReadJsonDataLogic.Instance.GetFloat (valueMap, "translateMix", 1);
                        float scaleMix = SpineReadJsonDataLogic.Instance.GetFloat (valueMap, "scaleMix", 1);
                        float shearMix = SpineReadJsonDataLogic.Instance.GetFloat (valueMap, "shearMix", 1);
                        timeline.SetFrame (frameIndex, time, rotateMix, translateMix, scaleMix, shearMix);
                        ReadCurve (valueMap, timeline, frameIndex);
                        frameIndex++;
                    }
                    timelines.Add (timeline);
                    duration = Math.Max (duration, timeline.frames[(timeline.FrameCount - 1) * TransformConstraintTimeline.ENTRIES]);
                }
            }

            // Path constraint timelines.
            if (map.ContainsKey ("paths")) {
                foreach (KeyValuePair<string, System.Object> constraintMap in (Dictionary<string, System.Object>) map["paths"]) {
                    int index = skeletonData.FindPathConstraintIndex (constraintMap.Key);
                    if (index == -1) throw new Exception ("Path constraint not found: " + constraintMap.Key);
                    PathConstraintData data = skeletonData.pathConstraints.Items[index];
                    var timelineMap = (Dictionary<string, System.Object>) constraintMap.Value;
                    foreach (KeyValuePair<string, System.Object> timelineEntry in timelineMap) {
                        var values = (List<System.Object>) timelineEntry.Value;
                        var timelineName = (string) timelineEntry.Key;
                        if (timelineName == "position" || timelineName == "spacing") {
                            PathConstraintPositionTimeline timeline;
                            float timelineScale = 1;
                            if (timelineName == "spacing") {
                                timeline = new PathConstraintSpacingTimeline (values.Count);
                                if (data.spacingMode == SpacingMode.Length || data.spacingMode == SpacingMode.Fixed) timelineScale = scale;
                            } else {
                                timeline = new PathConstraintPositionTimeline (values.Count);
                                if (data.positionMode == PositionMode.Fixed) timelineScale = scale;
                            }
                            timeline.pathConstraintIndex = index;
                            int frameIndex = 0;
                            foreach (Dictionary<string, System.Object> valueMap in values) {
                                timeline.SetFrame (frameIndex, (float) valueMap["time"], SpineReadJsonDataLogic.Instance.GetFloat (valueMap, timelineName, 0) * timelineScale);
                                ReadCurve (valueMap, timeline, frameIndex);
                                frameIndex++;
                            }
                            timelines.Add (timeline);
                            duration = Math.Max (duration, timeline.frames[(timeline.FrameCount - 1) * PathConstraintPositionTimeline.ENTRIES]);
                        } else if (timelineName == "mix") {
                            PathConstraintMixTimeline timeline = new PathConstraintMixTimeline (values.Count);
                            timeline.pathConstraintIndex = index;
                            int frameIndex = 0;
                            foreach (Dictionary<string, System.Object> valueMap in values) {
                                timeline.SetFrame (frameIndex, (float) valueMap["time"], SpineReadJsonDataLogic.Instance.GetFloat (valueMap, "rotateMix", 1), SpineReadJsonDataLogic.Instance.GetFloat (valueMap, "translateMix", 1));
                                ReadCurve (valueMap, timeline, frameIndex);
                                frameIndex++;
                            }
                            timelines.Add (timeline);
                            duration = Math.Max (duration, timeline.frames[(timeline.FrameCount - 1) * PathConstraintMixTimeline.ENTRIES]);
                        }
                    }
                }
            }

            // Deform timelines.
            if (map.ContainsKey ("deform")) {
                foreach (KeyValuePair<string, System.Object> deformMap in (Dictionary<string, System.Object>) map["deform"]) {
                    Skin skin = skeletonData.FindSkin (deformMap.Key);
                    foreach (KeyValuePair<string, System.Object> slotMap in (Dictionary<string, System.Object>) deformMap.Value) {
                        int slotIndex = skeletonData.FindSlotIndex (slotMap.Key);
                        if (slotIndex == -1) throw new Exception ("Slot not found: " + slotMap.Key);
                        foreach (KeyValuePair<string, System.Object> timelineMap in (Dictionary<string, System.Object>) slotMap.Value) {
                            var values = (List<System.Object>) timelineMap.Value;
                            VertexAttachment attachment = (VertexAttachment) skin.GetAttachment (slotIndex, timelineMap.Key);
                            if (attachment == null) throw new Exception ("Deform attachment not found: " + timelineMap.Key);
                            bool weighted = attachment.bones != null;
                            float[] vertices = attachment.vertices;
                            int deformLength = weighted ? vertices.Length / 3 * 2 : vertices.Length;

                            var timeline = new DeformTimeline (values.Count);
                            timeline.slotIndex = slotIndex;
                            timeline.attachment = attachment;

                            int frameIndex = 0;
                            foreach (Dictionary<string, System.Object> valueMap in values) {
                                float[] deform;
                                if (!valueMap.ContainsKey ("vertices")) {
                                    deform = weighted ? new float[deformLength] : vertices;
                                } else {
                                    deform = new float[deformLength];
                                    int start = SpineReadJsonDataLogic.Instance.GetInt (valueMap, "offset", 0);
                                    float[] verticesValue = SpineReadJsonDataLogic.Instance.GetFloatArray (valueMap, "vertices", 1);
                                    Array.Copy (verticesValue, 0, deform, start, verticesValue.Length);
                                    if (scale != 1) {
                                        for (int i = start, n = i + verticesValue.Length; i < n; i++)
                                            deform[i] *= scale;
                                    }

                                    if (!weighted) {
                                        for (int i = 0; i < deformLength; i++)
                                            deform[i] += vertices[i];
                                    }
                                }

                                timeline.SetFrame (frameIndex, (float) valueMap["time"], deform);
                                ReadCurve (valueMap, timeline, frameIndex);
                                frameIndex++;
                            }
                            timelines.Add (timeline);
                            duration = Math.Max (duration, timeline.frames[timeline.FrameCount - 1]);
                        }
                    }
                }
            }

            // Draw order timeline.
            if (map.ContainsKey ("drawOrder") || map.ContainsKey ("draworder")) {
                var values = (List<System.Object>) map[map.ContainsKey ("drawOrder") ? "drawOrder" : "draworder"];
                var timeline = new DrawOrderTimeline (values.Count);
                int slotCount = skeletonData.slots.Count;
                int frameIndex = 0;
                foreach (Dictionary<string, System.Object> drawOrderMap in values) {
                    int[] drawOrder = null;
                    if (drawOrderMap.ContainsKey ("offsets")) {
                        drawOrder = new int[slotCount];
                        for (int i = slotCount - 1; i >= 0; i--)
                            drawOrder[i] = -1;
                        var offsets = (List<System.Object>) drawOrderMap["offsets"];
                        int[] unchanged = new int[slotCount - offsets.Count];
                        int originalIndex = 0, unchangedIndex = 0;
                        foreach (Dictionary<string, System.Object> offsetMap in offsets) {
                            int slotIndex = skeletonData.FindSlotIndex ((string) offsetMap["slot"]);
                            if (slotIndex == -1) throw new Exception ("Slot not found: " + offsetMap["slot"]);
                            // Collect unchanged items.
                            while (originalIndex != slotIndex)
                                unchanged[unchangedIndex++] = originalIndex++;
                            // Set changed items.
                            int index = originalIndex + (int) (float) offsetMap["offset"];
                            drawOrder[index] = originalIndex++;
                        }
                        // Collect remaining unchanged items.
                        while (originalIndex < slotCount)
                            unchanged[unchangedIndex++] = originalIndex++;
                        // Fill in unchanged items.
                        for (int i = slotCount - 1; i >= 0; i--)
                            if (drawOrder[i] == -1) drawOrder[i] = unchanged[--unchangedIndex];
                    }
                    timeline.SetFrame (frameIndex++, (float) drawOrderMap["time"], drawOrder);
                }
                timelines.Add (timeline);
                duration = Math.Max (duration, timeline.frames[timeline.FrameCount - 1]);
            }

            // Event timeline.
            if (map.ContainsKey ("events")) {
                var eventsMap = (List<System.Object>) map["events"];
                var timeline = new EventTimeline (eventsMap.Count);
                int frameIndex = 0;
                foreach (Dictionary<string, System.Object> eventMap in eventsMap) {
                    EventData eventData = skeletonData.FindEvent ((string) eventMap["name"]);
                    if (eventData == null) throw new Exception ("Event not found: " + eventMap["name"]);
                    var e = new Spine.Event ((float) eventMap["time"], eventData);
                    e.Int = SpineReadJsonDataLogic.Instance.GetInt (eventMap, "int", eventData.Int);
                    e.Float = SpineReadJsonDataLogic.Instance.GetFloat (eventMap, "float", eventData.Float);
                    e.String = SpineReadJsonDataLogic.Instance.GetString (eventMap, "string", eventData.String);
                    timeline.SetFrame (frameIndex++, e);
                }
                timelines.Add (timeline);
                duration = Math.Max (duration, timeline.frames[timeline.FrameCount - 1]);
            }
            timelines.TrimExcess ();
            skeletonData.animations.Add (new Spine.Animation (name, timelines, duration));
        }
        private void ReadCurve (Dictionary<string, System.Object> valueMap, CurveTimeline timeline, int frameIndex) {
            if (!valueMap.ContainsKey ("curve"))
                return;
            System.Object curveObject = valueMap["curve"];
            if (curveObject.Equals ("stepped"))
                timeline.SetStepped (frameIndex);
            else {
                var curve = curveObject as List<System.Object>;
                if (curve != null)
                    timeline.SetCurve (frameIndex, (float) curve[0], (float) curve[1], (float) curve[2], (float) curve[3]);
            }
        }
        //对attachment通过是否有parent排序
        public List<AtlasRegionData> GetSortRegionList (Spine.Unity.SkeletonAnimation skeletonAnim, List<AtlasRegion> atlasRegions, Dictionary<int, List<string>> slotAttachmentDic) {
            List<AtlasRegionData> haveParent = new List<AtlasRegionData> ();
            List<AtlasRegionData> nothingParent = new List<AtlasRegionData> ();
            for (int i = 0; i < atlasRegions.Count; i++) {
                List<int> slotIndexList = SpineChangeCoreLogic.Instance.GetSlotListForSlotAttachmentDic (slotAttachmentDic, atlasRegions[i].name);
                for (int j = 0; j < slotIndexList.Count; j++) {
                    RoleAttachmentType tempType = SpineChangeDataLogic.Instance.GetAttachmentType (skeletonAnim, slotIndexList[j] + "|" + atlasRegions[i].name);
                    if (tempType.Equals (RoleAttachmentType.None)) {
                        continue;
                    }
                    Dictionary<String, System.Object> root = SpineReadJsonDataLogic.Instance.GetTargetObjectDic (
                        SpineConstValue.defaultSkinName,
                        SpineChangeCoreLogic.Instance.GetSlotNameBySlotIndex (skeletonAnim, SpineChangeDataLogic.Instance.GetSlotIndexFromAllAttachmentName (slotIndexList[j] + "|" + atlasRegions[i].name)), atlasRegions[i].name);
                    string parent = SpineReadJsonDataLogic.Instance.GetString (root, "parent", null);
                    if (string.IsNullOrEmpty (parent)) {
                        nothingParent.Add (new AtlasRegionData { allName = slotIndexList[j] + "|" + atlasRegions[i].name, region = atlasRegions[i], type = tempType });
                    } else {
                        haveParent.Add (new AtlasRegionData { allName = slotIndexList[j] + "|" + atlasRegions[i].name, region = atlasRegions[i], type = tempType });
                    }
                }
            }
            for (int i = 0; i < haveParent.Count; i++) {
                nothingParent.Add (haveParent[i]);
            }
            //测试
            // for (int i = 0; i < nothingParent.Count; i++) {
            //     Debug.LogError (nothingParent[i].name);
            // }
            return nothingParent;
        }
    }
}