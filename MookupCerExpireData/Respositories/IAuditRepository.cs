using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MookupCerExpireData
{
    public interface IAuditRepository
    {
        IList<Audits> SelectAll();        
    }
}
