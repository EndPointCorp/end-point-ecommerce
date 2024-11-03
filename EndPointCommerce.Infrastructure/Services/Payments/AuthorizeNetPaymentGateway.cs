using Microsoft.Extensions.Configuration;
using AuthorizeNet.Api.Controllers.Bases;
using AuthorizeNet.Api.Contracts.V1;
using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Domain.Interfaces;

namespace EndPointCommerce.Infrastructure.Services.Payments
{
    /// <summary>
    /// Service class that implements Authorize.net
    /// </summary>
    public class AuthorizeNetPaymentGateway : IPaymentGateway
    {
        private readonly IConfiguration _configuration;

        public AuthorizeNetPaymentGateway(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Calls the Authorize.net API to submit a payment transaction
        /// </summary>
        public IPaymentGateway.PaymentTransactionResult CreatePaymentTransaction(Order order)
        {
            PrepareConnection();
            return AuthorizeNetCreateTransaction.Run(order);
        }

        private void PrepareConnection()
        {
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthNetRunEnvironment;
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = AuthNetMerchantAuthentication;
        }

        private AuthorizeNet.Environment AuthNetRunEnvironment =>
            _configuration["AuthNetEnvironment"] == "Production" ?
                AuthorizeNet.Environment.PRODUCTION :
                AuthorizeNet.Environment.SANDBOX;

        private merchantAuthenticationType AuthNetMerchantAuthentication =>
             new()
             {
                 name = _configuration["AuthNetLoginId"],
                 ItemElementName = ItemChoiceType.transactionKey,
                 Item = _configuration["AuthNetTransactionKey"],
             };
    }
}
