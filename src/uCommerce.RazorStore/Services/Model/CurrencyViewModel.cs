using System;
using System.Linq;
using System.Linq.Expressions;
using ServiceStack.DataAccess;

namespace UCommerce.RazorStore.Services.Model
{
    public class CurrencyViewModel
    {
        public virtual int CurrencyId { get;  set; }
        public virtual bool Deleted { get; set; }
        public virtual int ExchangeRate { get; set; }
        public virtual Guid Guid { get; set; }
        public virtual int Id { get; set; }
        public virtual string ISOCode { get; set; }
        public virtual string Name { get; set; }

    }
}