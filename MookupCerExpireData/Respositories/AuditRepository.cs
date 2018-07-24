using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MookupCerExpireData
{
    public class AuditRepository: IAuditRepository
    {
        protected List<Audits> _table = null;

        public IList<Audits> SelectAll()
        {
            //return _table.AsNoTracking().ToList();
            return _table.ToList();
        }        
    }
}
