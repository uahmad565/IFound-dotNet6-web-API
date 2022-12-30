using System;
using System.Collections.Generic;
using System.Text;

namespace MXFaceAPIOneToNCall.Model
{
    public class BaseResponse
    {
        public int? ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public BaseResponse()
        {

        }
        public BaseResponse(int code, string message)
        {
            ErrorCode = code;
            ErrorMessage = message;
        }
    }
}
