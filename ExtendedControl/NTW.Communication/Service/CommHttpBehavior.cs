using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;

namespace NTW.Communication.Service
{
    class CommHttpBehavior: WebHttpBehavior
    {
        protected override QueryStringConverter GetQueryStringConverter(OperationDescription operationDescription)
        {
            return new CommQueryStringConverter();
        }
    }
}
