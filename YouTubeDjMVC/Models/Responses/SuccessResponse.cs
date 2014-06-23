using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YouTubeDjMVC.Models.Responses
{
    public class SuccessResponse:Response
    {
        public SuccessResponse()
        {
            Success = true;
        }
    }
}