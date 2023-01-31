using IFoundBackend.Model.Abstracts;
using IFoundBackend.Model.Posts;
using IFoundBackend.MxFace;
using MXFaceAPIOneToNCall.Model.FaceIndentity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Buffers.Text;
using System.Reflection.Metadata;
using System.Runtime.ConstrainedExecution;
using System.Security.Principal;
using System.Text.RegularExpressions;

namespace IFoundBackend.Areas.MxFaceManager
{
    public class FaceIdentityManager
    {
        private MXFaceIdentityAPI _mxFaceIdentityAPI;
        public FaceIdentityManager()
        {
            string _subscriptionKey = "XYiyrB4lAxfLx8F4o8-nAjyNS0wKw1148";
            MXFaceIdentityAPI mxFaceIdentityAPI = new MXFaceIdentityAPI("https://faceapi.mxface.ai/api/v2/", _subscriptionKey);
            _mxFaceIdentityAPI= mxFaceIdentityAPI;
        }
 
       
        
        //groupId Int Array IDs of any groups that the new identities belong to.
        //encoded_Image String  Base64 encoded string of image.
        //externalId String  Unqiue External ID of the identity.
        //confidenceThreshold Int Integer value between 0 and 100. If greater than zero, the identities will not be created if the given face match an existing identity in the database with a confidence greater than this confidenceThreshold.
        //qualityThreshold Int Optional integer value between 21 and 100. If this parameter added in request then uploaded faces quality will be compared from request qualityThreshold value, otherwise quality check as per defined MXFace standard.
        public void createFaceIdentity(List<int> groupids,string encoded,int externalID, int confidenceThreshold, int qualityThreshold=70)
        {

        }
        public void deleteFaceIdentity(int faceIdentityID)
        {

        }
    }
}
