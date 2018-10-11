using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NTW.Data.Context //.Presentation 
    {
    public partial class User {
        private string _ruleName;
        public string RuleName {
            get { return _ruleName ?? (_ruleName = GetRuleName()); }
        }

        private string _settingname;
        public string SettingName {
            get { return _settingname ?? (_settingname = GetSettingName()); }
        }


        #region Helps
        protected string GetRuleName() {
            string result = null;
            using (DBContext context = new DBContext(false)) 
                result = context.Roles.FirstOrDefault(x => x.ID == this.IdRole).RoleName;
            return result;
        }

        protected string GetSettingName() {
            string result = null;
            using (DBContext context = new DBContext(false))
                result = context.Settings.FirstOrDefault(x => x.ID == this.IdSetting).Caption;
            return result;
        }
        #endregion
    }
}
