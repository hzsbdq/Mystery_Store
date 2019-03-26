using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpongeBob_Mall.DAL
{
    public class DbInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<MySqlContext>
    {
        protected override void Seed(MySqlContext context)
        {
            //var students = new list<student>
            //{
            //new student{firstmidname="carson",lastname="alexander",enrollmentdate=datetime.parse("2005-09-01")},
            //new student{firstmidname="meredith",lastname="alonso",enrollmentdate=datetime.parse("2002-09-01")},
            //new student{firstmidname="arturo",lastname="anand",enrollmentdate=datetime.parse("2003-09-01")},
            //new student{firstmidname="gytis",lastname="barzdukas",enrollmentdate=datetime.parse("2002-09-01")},
            //new student{firstmidname="yan",lastname="li",enrollmentdate=datetime.parse("2002-09-01")},
            //new student{firstmidname="peggy",lastname="justice",enrollmentdate=datetime.parse("2001-09-01")},
            //new student{firstmidname="laura",lastname="norman",enrollmentdate=datetime.parse("2003-09-01")},
            //new student{firstmidname="nino",lastname="olivetto",enrollmentdate=datetime.parse("2005-09-01")}
            //};

            //students.foreach(s => context.students.add(s));
            //context.savechanges();
            
        }
    }
}