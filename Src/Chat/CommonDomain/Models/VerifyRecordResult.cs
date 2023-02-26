using CommonDomainLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonDomain.Models
{
    public struct VerifyRecordResult
    {
        public bool Success { get; set; }
        public ErrorMessageEnum MessageId { get; set; }
        public Exception Exception { get; set; }
        public string Message { get; set; }
    }
}
