using Reveal.Sdk;
using System.Text.RegularExpressions;

namespace RevealSdk.Server.Reveal
{
    public class UserContextProvider : IRVUserContextProvider
    {
        IRVUserContext IRVUserContextProvider.GetUserContext(HttpContext aspnetContext)
        {

            // ****
            // In this case, there are 2 headers sent in clear text to the server
            // Normally, you'd be accepting your token or other secrets that you'd use 
            // for the security context of your data requests,
            // or you would be passing query parameters for custom queries, etc.
            // ****

            var userIdString = aspnetContext.Request.Headers["x-header-one"];
            var orderId = aspnetContext.Request.Headers["x-header-two"];

            int userId;

            if (string.IsNullOrEmpty(userIdString) || userIdString == "0")
            {
                // this is for testing purposes only to ensure there is a valid userId
                userId = 3; // Assign the default value, this is an Admin user in the ObjectFilter
            }
            else if (!int.TryParse(userIdString, out userId) || !IsValidCustomerId(userId))
            {
                throw new ArgumentException("Invalid CustomerID format. CustomerID must be a valid integer between 1 and 30.");
            }

            // At this point, 'userId' will be either the parsed valid ID or the default value of 3.
            // You can now use the 'userId' variable in the rest of your code.

            // Proceed with valid userId

            // ****
            // Set up Roles based on the incoming user id.  In a real app, this would be set up to match
            // your scenario and be dynamically loaded
            // ****
            string role = "User";
            if (userIdString == "3" || userIdString == "11")
            {
                role = "Admin";
            }

            // ****
            // Create an array of properties that can be used in other Reveal functions
            // ****
            var props = new Dictionary<string, object>() {
                    { "OrderId", orderId },
                    { "Role", role } };

            return new RVUserContext(userIdString, props);
        }

        private static bool IsValidCustomerId(int customerId) => customerId >= 1 && customerId <= 30;
    }
}