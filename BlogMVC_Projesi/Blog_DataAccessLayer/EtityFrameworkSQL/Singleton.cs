﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog_DataAccessLayer.EtityFrameworkSQL
{
    public class Singleton
    {
        protected static BlogContext _context;
        private static object _lock = new object();
        protected Singleton()
        {
            CreateContext();
        }
        private static void CreateContext()
        {
            if (_context == null)
            {
                // bazı uygulamalarda (multitrade uygulamalarda), aynı anda 2 tane istek if bloğuna girebilir. Bu gibi durumları kontrol etmek için, lock ile kilitleme yapılır.. Yani lock aynı anda 2 tane isteğin ya da trade'in çalıştırılmayacağını söyler.
                lock (_lock)
                {
                    if (_context == null)
                    {
                        _context = new BlogContext();
                    }
                    
                }
                
            }     
           // return _context;
        }
    }
}
