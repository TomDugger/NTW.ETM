using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace NTW.Communication.Services
{
    [ServiceContract(SessionMode = SessionMode.NotAllowed)]
    public interface ICommandService
    {
        [OperationContract]
        [WebInvoke(Method = "GET",
            BodyStyle = WebMessageBodyStyle.WrappedResponse,
            RequestFormat = WebMessageFormat.Xml,
            ResponseFormat = WebMessageFormat.Xml,
            UriTemplate = "com?t={commandType}&i={index}&ai={parametry}")]
        void SetCommand(int commandType, int index, int[] parametry);
    }
}
