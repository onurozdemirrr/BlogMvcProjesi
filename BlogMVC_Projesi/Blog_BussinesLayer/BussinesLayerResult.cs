using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog_BussinesLayer
{
    public class BussinesLayerResult<T> where T : class
    {
        // Hata mesajlarını saklayan bir List tanımlıyorum.
        public List<string> Errors { get; set; }
        // Eğer hata mesajımız yok ise aşağıdaki nesneyi geriye döndüreceğiz..
        public T Result { get; set; }

        public BussinesLayerResult()
        {
            Errors = new List<string>();
        }
    }
}
