using System.Collections;
using System.Collections.Generic;
using Spine;
using UnityEngine;

namespace DancingWord.SpineChange {
    public abstract class SpineAttachment {
        /// <summary>
        /// 通过AtlasRegion重建RegionAttachment
        /// </summary>
        /// <returns>The region attachment by atlas region.</returns>
        /// <param name="allAttachmentName">Attachment name.</param>
        /// <param name="atlasRegion">Atlas region.</param>
        public abstract Attachment CreateRegionAttachmentByAtlasRegion (Spine.Unity.SkeletonAnimation skeletonAnim,string allAttachmentName, AtlasRegion atlasRegion);
        /// <summary>
        /// 通过AtlasRegion重建MeshAttachment
        /// </summary>
        /// <returns>The mesh attachment by atlas region.</returns>
        /// <param name="allAttachmentName">Attachment name.</param>
        /// <param name="atlasRegion">Atlas region.</param>
        public abstract Attachment CreateMeshAttachmentByAtlasRegion (Spine.Unity.SkeletonAnimation skeletonAnim,string allAttachmentName, AtlasRegion atlasRegion);
        /// <summary>
        /// 通过Texture重建RegionAttachment
        /// </summary>
        /// <returns>The region attachment by texture.</returns>
        /// <param name="slotName">Slot name.</param>
        /// <param name="texture">Texture.</param>
        public abstract Attachment CreateRegionAttachmentByTexture (Spine.Unity.SkeletonAnimation sAnim, string slotName, Texture2D texture);
        /// <summary>
        /// 通过Texture重建MeshAttachment
        /// </summary>
        /// <returns>The mesh attachment by texture.</returns>
        /// <param name="slotName">Slot name.</param>
        /// <param name="texture">Texture.</param>
        public abstract Attachment CreateMeshAttachmentByTexture (Spine.Unity.SkeletonAnimation sAnim, string slotName, Texture2D texture);
        /// <summary>
        /// 重建BoundingBoxAttachment
        /// </summary>
        /// <returns>The bounding box attachment by name.</returns>
        /// <param name="allAttachmentName">All attachment name.</param>
        public abstract Attachment CreateBoundingBoxAttachmentByName (Spine.Unity.SkeletonAnimation skeletonAnim,string allAttachmentName);
        /// <summary>
        /// 重建PathAttachment
        /// </summary>
        /// <returns>The path attachment by name.</returns>
        /// <param name="allAttachmentName">All attachment name.</param>
        public abstract Attachment CreatePathAttachmentByName (Spine.Unity.SkeletonAnimation skeletonAnim,string allAttachmentName);
        /// <summary>
        /// 重建PointAttachment
        /// </summary>
        /// <returns>The point attachment by name.</returns>
        /// <param name="allAttachmentName">All attachment name.</param>
        public abstract Attachment CreatePointAttachmentByName (Spine.Unity.SkeletonAnimation skeletonAnim,string allAttachmentName);
        /// <summary>
        /// 重建ClippingAttachment
        /// </summary>
        /// <returns>The clipping attachment by name.</returns>
        /// <param name="allAttachmentName">All attachment name.</param>
        public abstract Attachment CreateClippingAttachmentByName (Spine.Unity.SkeletonAnimation skeletonAnim,string allAttachmentName);
    }
}