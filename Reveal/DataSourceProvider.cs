using Reveal.Sdk;
using Reveal.Sdk.Data;
using Reveal.Sdk.Data.MySql;

namespace RevealSdk.Server.Reveal

{
    internal class DataSourceProvider : IRVDataSourceProvider
    {
        public Task<RVDataSourceItem> ChangeDataSourceItemAsync(IRVUserContext userContext, string dashboardId, RVDataSourceItem dataSourceItem)
        {
            if (dataSourceItem is RVMySqlDataSourceItem sqlDsi)
            {
                ChangeDataSourceAsync(userContext, sqlDsi.DataSource);

                switch (sqlDsi.Id)
                {
                    // ****
                    // Example of an Stored Procedure with a customerId parameter from the userContext
                    // ****
                    case "sp_customer_orders":
                        sqlDsi.Procedure = sqlDsi.Id;
                        sqlDsi.ProcedureParameters = new Dictionary<string, object> { { "customer", userContext.UserId } };
                        break;
                    // ****
                    // Example of an ad-hoc query with a customerId parameter from the userContext
                    // ****
                    case "customer_orders":
                        sqlDsi.CustomQuery = $"select * from {sqlDsi.Id} where customer_id = {userContext.UserId}";
                        break;
                    // ****
                    // Example of a ad-hoc query to a View with a orderId parameter from the userContext
                    // ****
                    case "customer_orders_details":
                        sqlDsi.CustomQuery = $"select * from {sqlDsi.Id} where order_id = {userContext.Properties["OrderId"]}";
                        break;
                    // ****
                    // This assumes the Data Sources Dialog table / object is selected
                    // you can check the incoming request of function, table to customize the query
                    // ****
                    default:
                        if (sqlDsi.Table == "customers")
                        { 
                            sqlDsi.CustomQuery = $"select * from {sqlDsi.Table} where id = {userContext.UserId}";
                        }
                        break;
                }
            }
            return Task.FromResult(dataSourceItem);
        }

        public Task<RVDashboardDataSource> ChangeDataSourceAsync(IRVUserContext userContext, RVDashboardDataSource dataSource)
        {
            if (dataSource is RVMySqlDataSource sqlDs)
            {
                sqlDs.Host = "infragistics.local";
                sqlDs.Database = "northwind";             
            }
            return Task.FromResult(dataSource);
        }
    }
}