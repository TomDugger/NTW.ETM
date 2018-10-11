using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NTW.Data.Context//.Manipulation
{
    public partial class Project
    {
        public static int CountTaskInLine(string nameProject) {
            int result = 4;
            using (DBContext context = new DBContext(true)) {
                var project = context.Projects.FirstOrDefault(x => x.Caption == nameProject);
                foreach (var item in project.Tasks) {

                }
                result = project.Tasks.Count;
            }
            return result;
        }
    }
}
