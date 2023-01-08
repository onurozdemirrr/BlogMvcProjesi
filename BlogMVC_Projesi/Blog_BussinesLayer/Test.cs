using Blog_DataAccessLayer.EtityFrameworkSQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog_BussinesLayer
{
    public class Test
    {
        public Test()
        {
            BlogContext db = new BlogContext();
            db.BlogUsers.ToList();
            //db.Database.CreateIfNotExists();
        }
    }
}



