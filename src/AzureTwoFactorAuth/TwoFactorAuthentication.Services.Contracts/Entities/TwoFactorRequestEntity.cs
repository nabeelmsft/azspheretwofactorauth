
namespace TwoFactorAuthentication.Services.Contracts.Entities
{
    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    /// The TwoFactorRequestEntity
    /// </summary>
    public class TwoFactorRequestEntity : TableEntity
    {
        public TwoFactorRequestEntity(string uniqueId, string userName, string correlationId)
        {
            this.PartitionKey = userName;
            this.RowKey = uniqueId;
            this.CorrelationId = correlationId;
        }

        public TwoFactorRequestEntity()
        {

        }

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
