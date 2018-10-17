using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Dispatcher;
using System.Text;

namespace NTW.Communication.Service
{
    class CommQueryStringConverter : QueryStringConverter
    {
        public CommQueryStringConverter() : base()
        {

        }

        public override bool CanConvert(Type type)
        {
            return type == typeof(int[]) || base.CanConvert(type);
        }

        public override object ConvertStringToValue(string parameter, Type parameterType)
        {
            if (parameterType == typeof(int[]))
            {
                if (parameter != null)
                    return parameter.Replace("[", "").Replace("]", "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => Convert.ToInt32(x)).ToArray();
                else
                    return new int[0];
            }
            else return base.ConvertStringToValue(parameter, parameterType);
        }

        public override string ConvertValueToString(object parameter, Type parameterType)
        {
            if (parameterType == typeof(int[]))
            {
                if (((int[])parameter).Length > 0)
                    return string.Format("[{0}]", ((int[])parameter).Select(x => x.ToString()).Aggregate((i, j) => i + ", " + j));
                else
                    return string.Format("[{0}]", "");
            }
            else return base.ConvertValueToString(parameter, parameterType);
        }
    }
}
