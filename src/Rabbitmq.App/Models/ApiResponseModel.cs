using System.Net;

namespace Rabbitmq.App.Models;

 public class ApiResponseModel
 {
     public ApiResponseModel()
     {
         StatusCode = HttpStatusCode.ServiceUnavailable;
         MessageOutput = new MessageOutputModel();
     }

     public ApiResponseModel(HttpStatusCode statusCode, string messageOutputContent)
     {
         StatusCode = statusCode;
         MessageOutput = new MessageOutputModel(messageOutputContent);
     }

     public HttpStatusCode StatusCode { get; set; }
     public MessageOutputModel MessageOutput { get; set; }
 }