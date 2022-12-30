using System;
using System.Collections.Generic;
using System.Text;

namespace MXFaceAPIOneToNCall.Model.FaceIndentity
{
    public class AddFaceRequest
    {
        public string Encoded_Image { get; set; }
        public float ConfidenceThreshold { get; set; }
        /// <summary>
        /// Optional integer value between 21 and 100. If this parameter added in request then uploaded faces
        /// quality will be compared from request qualityThreshold value, otherwise quality check as per defined
        /// MXFace standard.
        /// </summary>
        public int? QualityThreshold { get; set; } = null;
    }
}
