using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebsite.ViewModel.Models
{
    
        public class MessageRequest
        {
            public string Title { get; set; }
            public string Body { get; set; }
            public string DeviceToken { get; set; }
            // Add more properties as needed based on your notification requirements
        }

    public class NotificationModel
    {
       
        public string DeviceId { get; set; }
      
        public bool IsAndroiodDevice { get; set; }
      
        public string Body { get; set; }
        public string BodyImage { get; set; }

        public string Title { get; set; }
        public string Abstract { get; set; }
        public string Content { get; set; }
        public string TopicName { get; set; }
        public string SubtopicName { get; set; }
        public List<string> ObjKeywords { get; set; }

    }

    public class GoogleNotification
    {
        public class DataPayload
        {
           
            public string Title { get; set; }
          
            public string Body { get; set; }
        }
        
        public string Priority { get; set; } = "high";
      
        public DataPayload Data { get; set; }
     
        public DataPayload Notification { get; set; }
    }
    public class ResponseModel
    {
      
        public bool IsSuccess { get; set; }
       
        public string Message { get; set; }
    }
    public class FcmNotificationSetting
    {
        public string SenderId { get; set; }
        public string ServerKey { get; set; }
    }
}
