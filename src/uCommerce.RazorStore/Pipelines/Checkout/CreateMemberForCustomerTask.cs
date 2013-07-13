using System.Globalization;
using UCommerce.Infrastructure.Globalization;
using UCommerce.Transactions;

namespace UCommerce.RazorStore.Pipelines.Checkout
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mail;
    using UCommerce.EntitiesV2;
    using UCommerce.Infrastructure;
    using UCommerce.Infrastructure.Configuration;
    using UCommerce.Infrastructure.Logging;
    using UCommerce.Pipelines;
    using UCommerce.Security;

    /// <summary>
    /// Custom pipeline task that creates a member and emails them their password that was created during the checkout process.
    /// </summary>
    /// <remarks>
    /// To use it, wire up the custom component within your umbraco\ucommerce\pipelines\checkout.config settings file -you should remove the default CreateMemberForCustomer task
    /// 
    /// <component id="Checkout.CreateMemberForCustomer"
    /// 			service="UCommerce.Pipelines.IPipelineTask`1[[UCommerce.EntitiesV2.PurchaseOrder, UCommerce]], UCommerce"
    /// 			type="UCommerce.RazorStore.Pipelines.Checkout.CreateMemberForCustomerTask, UCommerce.RazorStore">
    ///     <parameters>
    ///         <emailTypeName>NewUserAccount</emailTypeName>
    ///         <!-- By prefixing the key with _, it won't display in the admin section -->
    ///         <passwordKey>_password</passwordKey>
    ///     </parameters>
    /// </component>
    /// </remarks>
    public class CreateMemberForCustomerTask : IPipelineTask<PurchaseOrder>
    {
        private readonly string _emailTypeName;
        private readonly string _passwordKey;

        private readonly CommerceConfigurationProvider _commerceConfiguration;
        private readonly ILoggingService _loggingService;
        private readonly IEmailService _emailService;
        private readonly IMemberService _memberService;

        private static readonly Random _random = new Random(Environment.TickCount);

        public CreateMemberForCustomerTask(string emailTypeName, string passwordKey, IMemberService memberService, ILoggingService loggingService, IEmailService emailService, CommerceConfigurationProvider commerceConfiguration)
        {
            _emailTypeName = emailTypeName;
            _passwordKey = passwordKey;
            _commerceConfiguration = commerceConfiguration;
            _loggingService = loggingService;
            _emailService = emailService;
            _memberService = memberService;
        }

        public PipelineExecutionResult Execute(PurchaseOrder purchaseOrder)
        {
            var email = getCustomersEmailAddress(purchaseOrder);

            if (String.IsNullOrWhiteSpace(email))
                return clearPasswordAndReturn(purchaseOrder, PipelineExecutionResult.Warning);

            var group = purchaseOrder.ProductCatalogGroup;

            if (group == null)
                return clearPasswordAndReturn(purchaseOrder, PipelineExecutionResult.Warning);

            var memberGroupId = group.MemberGroupId;
            var memberTypeId = group.MemberTypeId;

            Guard.Against.NullArgument(memberGroupId, String.Format("No member group configured for product catalog group {0}", group.Name));
            Guard.Against.NullArgument(memberTypeId, String.Format("No member type configured for product catalog group {0}", group.Name));

            if (!purchaseOrder.ProductCatalogGroup.CreateCustomersAsUmbracoMembers || String.IsNullOrWhiteSpace(getPasswordFromOrder(purchaseOrder)))
                return clearPasswordAndReturn(purchaseOrder, PipelineExecutionResult.Success);

            var customer = purchaseOrder.Customer;
            var password = getPasswordFromOrder(purchaseOrder);
            var existingMember = !String.IsNullOrWhiteSpace(customer.MemberId);

            if (existingMember)
                return clearPasswordAndReturn(purchaseOrder, PipelineExecutionResult.Success);

            if (String.IsNullOrWhiteSpace(password))
                password = generatePassword();

            var memberId = createUmbracoMember(memberTypeId, memberGroupId, email, password);

            if (String.IsNullOrWhiteSpace(memberId))
                return clearPasswordAndReturn(purchaseOrder, PipelineExecutionResult.Warning);

            storeMemberIdOnCustomer(customer, memberId);

            var customerId = customer.CustomerId;
            var orderNumber = purchaseOrder.OrderNumber;
            var orderGuid = purchaseOrder.OrderGuid.ToString();
            var cultureCode = purchaseOrder.CultureCode;
            var emailProfile = purchaseOrder.ProductCatalogGroup.EmailProfile;

            sendCustomerWelcomeEmail(email, password, customerId, memberId, orderNumber, orderGuid, cultureCode, emailProfile);

            return clearPasswordAndReturn(purchaseOrder, PipelineExecutionResult.Success);
        }

        private PipelineExecutionResult clearPasswordAndReturn(PurchaseOrder purchaseOrder, PipelineExecutionResult statusToReturn)
        {
            clearPasswordFromOrder(purchaseOrder);
            return statusToReturn;
        }

        private string getCustomersEmailAddress(PurchaseOrder purchaseOrder)
        {
            var customer = purchaseOrder.Customer;

            if (customer != null && !String.IsNullOrWhiteSpace(customer.EmailAddress))
                return customer.EmailAddress;

            var billingAddress = purchaseOrder.GetBillingAddress();

            if (billingAddress != null && !String.IsNullOrWhiteSpace(billingAddress.EmailAddress))
                return billingAddress.EmailAddress;

            return String.Empty;
        }

        private string createUmbracoMember(string memberTypeId, string memberGroupId, string email, string password)
        {
            var member = createOrGetUmbracoMember(email, password, memberGroupId, memberTypeId);
            return member == null ? String.Empty : member.MemberId;
        }

        private void storeMemberIdOnCustomer(Customer customer, string memberId)
        {
            customer.MemberId = memberId;
            customer.Save();
        }

        private string getPasswordFromOrder(PurchaseOrder purchaseOrder)
        {
            return purchaseOrder[_passwordKey];
        }

        private void clearPasswordFromOrder(PurchaseOrder purchaseOrder)
        {
            purchaseOrder[_passwordKey] = String.Empty;
            var prop = purchaseOrder.OrderProperties.FirstOrDefault(p => p.Key == _passwordKey);
            purchaseOrder.RemoveOrderProperty(prop);
            purchaseOrder.Save();
        }

        private void sendCustomerWelcomeEmail(string email, string password, int customerId, string memberId, string orderNumber, string orderGuid, string cultureCode, EmailProfile emailProfile)
        {
            var queryStringParameters = new Dictionary<string, string> 
                { 
                    {"customerId", customerId.ToString()},
                    {"memberId", memberId},
                    {"orderNumber",orderNumber},
			        {"orderGuid",orderGuid},
					{"User.Username", email},
					{"User.Password", password}
                };

            try
            {
                var customGlobalization = new CustomGlobalization(_commerceConfiguration);
                customGlobalization.SetCulture(new CultureInfo(String.IsNullOrWhiteSpace(cultureCode) ? customGlobalization.DefaultCulture.ToString() : cultureCode));
                _emailService.Send(customGlobalization, emailProfile, _emailTypeName, new MailAddress(email), queryStringParameters);
            }
            catch (SmtpException exception)
            {
                _loggingService.Log<CreateMemberForCustomerTask>(exception);
            }
        }

        private Member createOrGetUmbracoMember(string email, string password, string memberGroupId, string memberTypeId)
        {
            if (_memberService.IsLoggedIn())
                return _memberService.GetCurrentMember();

            if (_memberService.IsMember(email))
                return _memberService.GetMemberFromEmail(email);

            return _memberService.MakeNew(email, password, email, new MemberType(memberTypeId), new MemberGroup(memberGroupId));
        }

        private string generatePassword()
        {
            var firstWords = new[]
			{
				"Absolutely",
				"Amazing",
				"Approved",
				"Attractive",
				"Authentic",
				"Bargain",
				"Beautiful",
				"Better",
				"Big",
				"Colorful",
				"Colossal",
				"Complete",
				"Confidential",
				"Crammed",
				"Delivered",
				"Direct",
				"Discount",
				"Easily",
				"Endorsed",
				"Enormous",
				"Excellent",
				"Exciting",
				"Exclusive",
				"Expert",
				"Famous",
				"Fascinating",
				"Fortune",
				"Free",
				"Full",
				"Genuine",
				"Gift",
				"Gigantic",
				"Greatest",
				"Guaranteed",
				"Helpful",
				"Highest",
				"Huge",
				"Immediately",
				"Improved",
				"Informative",
				"Instructive",
				"Interesting",
				"Largest",
				"Latest",
				"Lavishly",
				"Liberal",
				"Lifetime",
				"Limited",
				"Lowest",
				"Magic",
				"Mammoth",
				"Miracle",
				"Noted",
				"Odd",
				"Outstanding",
				"Personalized",
				"Popular",
				"Powerful",
				"Practical",
				"Professional",
				"Profitable",
				"Profusely",
				"Proven",
				"Quality",
				"Quickly",
				"Rare",
				"Reduced",
				"Refundable",
				"Remarkable",
				"Reliable",
				"Revealing",
				"Revolutionary",
				"Scarce",
				"Secrets",
				"Security",
				"Selected",
				"Sensational",
				"Simplified",
				"Sizable",
				"Special",
				"Startling",
				"Strange",
				"Strong",
				"Sturdy",
				"Successful",
				"Superior",
				"Surprise",
				"Terrific",
				"Tested",
				"Tremendous",
				"Unconditional",
				"Unique",
				"Unlimited",
				"Unparalleled",
				"Unsurpassed",
				"Unusual",
				"Useful",
				"Valuable",
				"Wealth",
				"Weird",
				"Wonderful"
			};
            var secondWords = new[]
			{
				"Bark",
				"Blend",
				"Blast",
				"Boast",
				"Bump",
				"Chase",
				"Chap",
				"Climb",
				"Crawl",
				"Cry",
				"Dream",
				"Faint",
				"Float",
				"Fly",
				"Frown",
				"Groan",
				"Hide",
				"Hike",
				"Hop",
				"Joke",
				"Jump",
				"Melt",
				"Paddle",
				"Pretend",
				"Pull",
				"Push",
				"Race",
				"Ride",
				"Roll",
				"Row",
				"Rub",
				"Sail",
				"Search",
				"Shake",
				"Shout",
				"Sing",
				"Smash",
				"Spoil",
				"Spread",
				"Spray",
				"Stalk",
				"Stamp",
				"Step",
				"Stroll",
				"Stuff",
				"Swim",
				"Tag",
				"Tickle",
				"Travel",
				"Trip",
				"Vote",
				"Wag",
				"Whirl",
				"Wish",
				"Bake",
				"Bang",
				"Beep",
				"Blink",
				"Boil",
				"Broil",
				"Buzz",
				"Cackle",
				"Caw",
				"Chatter",
				"Cheep",
				"Chime",
				"Clang",
				"Clap",
				"Click",
				"Clash",
				"Crush",
				"Cut",
				"Dash",
				"Follow",
				"Frighten",
				"Fry",
				"Giggle",
				"Growl",
				"Heat",
				"Hiss",
				"Hoot",
				"Hum",
				"Juggle",
				"Laugh",
				"Leap",
				"Mix",
				"Pass",
				"Poach",
				"Purr",
				"Rattle",
				"Ring",
				"Roar",
				"Roast",
				"Rush",
				"Scramble",
				"Scream",
				"Screech",
				"Shiver",
				"Sink",
				"Slide",
				"Snap",
				"Sob",
				"Speed",
				"Stumble",
				"Swing",
				"Thump",
				"Toast",
				"Toss",
				"Wail",
				"Wave",
				"Weep",
				"Whip",
				"Whisper",
				"Wrestle"
			};

            var random = _random;

            return string.Concat(new object[]
			{
				firstWords[random.Next(0, firstWords.Length - 1)],
				random.Next(0, 99),
				secondWords[random.Next(0, secondWords.Length - 1)],
				random.Next(0, 99)
			});
        }
    }
}