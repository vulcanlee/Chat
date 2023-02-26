using CommonDomainLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonDomain.Models
{
    public struct VerifyRecordResult<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }
        public T Result { get; set; }
    }
}
