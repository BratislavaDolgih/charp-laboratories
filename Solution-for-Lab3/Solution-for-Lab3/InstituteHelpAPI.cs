using InstituteCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solution_for_Lab3
{
    internal interface InstituteHelpAPI
    {
        string Naming { get; set; }
        void AddCourse(Institute.Course courseName);
        bool RemoveCourse(Institute.Course courseName);
    }
}
