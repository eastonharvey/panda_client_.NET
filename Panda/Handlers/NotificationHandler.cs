using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using System.Collections.Specialized;

namespace Panda.Handlers
{

    //Step 1: Create your own GenericHandler.ashx and replace the bass class with this one [NotificationHandler]
    //Step 2: Override the abstract methods below in your new GenericHandler.ashx. 
                //Be sure to set: HttpContext.Current.Response.Status = "200"; for each one
    //Setp 3: Direct notifications traffic under the notifications tab of your Panda Stream Cloud Control Panel to your new GenericHandler.ashx

    public abstract class NotificationHandler : IHttpHandler
    {
        #region IHttpHandler Members

        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            var streamReader = new StreamReader(context.Request.InputStream);
            string rawRequest = streamReader.ReadToEnd();
            if (!string.IsNullOrEmpty(rawRequest))
            {
                // parse query string params into collection
                NameValueCollection parameters = HttpUtility.ParseQueryString(rawRequest);

                // retrieve the event name
                string notificationEvent = parameters["event"];

                // route event details to the appropriate method
                if (!string.IsNullOrEmpty(notificationEvent))
                {
                    switch (notificationEvent)
                    {
                        case "video-created":
                            OnVideoCreated(parameters["video_id"], GetEncodingIds(parameters));
                            break;
                        case "video-encoded":
                            OnVideoEncoded(parameters["video_id"], GetEncodingIds(parameters));
                            break;
                        case "encoding-progress":
                            OnEncodingProgress(parameters["encoding_id"], parameters["progress"]);
                            break;
                        case "encoding-completed":
                            OnEncodingCompleted(parameters["encoding_id"]);
                            break;
                    }
                }
            }
        }

        #endregion

        private List<string> GetEncodingIds(NameValueCollection parameters)
        {
            List<string> encodingIds = new List<string>();
            foreach (var key in parameters.AllKeys)
                if (key.Contains("encoding_ids"))
                    encodingIds.Add(parameters[key]);
            return encodingIds;
        }

        public abstract void OnVideoCreated(string videoId, List<string> encodingIds);
        public abstract void OnVideoEncoded(string videoId, List<string> encodingIds);
        public abstract void OnEncodingProgress(string encodingId, string progress);
        public abstract void OnEncodingCompleted(string encodingId);
    }
}
