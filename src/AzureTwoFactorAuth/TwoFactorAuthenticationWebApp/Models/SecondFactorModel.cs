using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwoFactorAuthenticationWebApp.Models
{
    /// <summary>
    /// The second factor model.
    /// </summary>
    public class SecondFactorModel
    {
        /// <summary>
        /// The CorrelationId
        /// </summary>
        public string PartitionKey { get; set; }
        
        /// <summary>
        /// The CorrelationId
        /// </summary>
        public string RowKey { get; set; }

        /// <summary>
        /// The CorrelationId
        /// </summary>
        public string CorrelationId { get; set; }

        /// <summary>
        /// The CreatedSecurityCode
        /// </summary>
        public string CreatedSecurityCode { get; set; }

        /// <summary>
        /// The RetrievedSecurityCode
        /// </summary>
        public string RetrievedSecurityCode { get; set; }

        /// <summary>
        /// The RequestCreationTimestamp
        /// </summary>
        public string RequestCreationTimestamp { get; set; }

        /// <summary>
        /// The RequestValidatedTimestamp
        /// </summary>
        public string RequestValidatedTimestamp { get; set; }
    }
}
