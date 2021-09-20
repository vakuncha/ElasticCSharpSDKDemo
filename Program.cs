using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Management.Elastic;
using Azure.Identity;
using Microsoft.Rest;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Azure.Management.Elastic.Models;
using Microsoft.Azure.Management.ResourceManager.Fluent;

namespace ElasticSDKDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {

            string subscriptionId = Environment.GetEnvironmentVariable("AZURE_SUBSCRIPTION_ID");

            string tenantId = Environment.GetEnvironmentVariable("AZURE_TENANT_ID");
            string clientId = Environment.GetEnvironmentVariable("AZURE_CLIENT_ID");
            string clientSecret = Environment.GetEnvironmentVariable("AZURE_CLIENT_SECRET");
            string resourceGroup = "vakuncha-test-rg";

            var credentials = SdkContext.AzureCredentialsFactory
                               .FromServicePrincipal(clientId,clientSecret,tenantId,AzureEnvironment.AzureGlobalCloud);


            var microsoftElasticClient = new MicrosoftElasticClient(credentials);

            microsoftElasticClient.SubscriptionId = subscriptionId;

            var elasticResources = microsoftElasticClient.Monitors.ListByResourceGroup(resourceGroup);

            if (elasticResources.Any())
            {
                var resourceToBeDeleted = elasticResources.First().Name;
                Console.WriteLine("Deleting Resource:" + resourceToBeDeleted);
                microsoftElasticClient.Monitors.Delete(resourceGroup,resourceToBeDeleted);
            }
           
            ElasticMonitorResource elasticMonitorResource= new ElasticMonitorResource();

            elasticMonitorResource.Location="eastus2euap";
            elasticMonitorResource.Sku = new ResourceSku("ess-monthly-consumption_Monthly");
            elasticMonitorResource.Properties = new MonitorProperties();

            elasticMonitorResource.Properties.UserInfo = new Microsoft.Azure.Management.Elastic.Models.UserInfo();
            elasticMonitorResource.Properties.UserInfo.CompanyInfo = new CompanyInfo();
            elasticMonitorResource.Properties.UserInfo.FirstName = "varun";
            elasticMonitorResource.Properties.UserInfo.LastName = "kunchakuri";
            elasticMonitorResource.Properties.UserInfo.EmailAddress = "sdkdemo@mpliftrelastic20210901outlo.onmicrosoft.com";
            elasticMonitorResource.Properties.UserInfo.CompanyInfo.Business = "cloud";
            elasticMonitorResource.Properties.UserInfo.CompanyInfo.Country = "India";
            elasticMonitorResource.Properties.UserInfo.CompanyInfo.Domain = "software";
            elasticMonitorResource.Properties.UserInfo.CompanyInfo.State = "andhrapradesh";


            string resourceName = "csharpsdkCreate-" + new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();

            Console.WriteLine("Creating Resource:" + resourceName);
            microsoftElasticClient.Monitors.Create(resourceGroup, resourceName, elasticMonitorResource);

            MonitoringTagRulesProperties monitoringTagRulesProperties = new MonitoringTagRulesProperties();
            monitoringTagRulesProperties.LogRules = new LogRules();
            monitoringTagRulesProperties.LogRules.SendSubscriptionLogs = true;
            monitoringTagRulesProperties.LogRules.SendActivityLogs = true;
            Console.WriteLine("Updating Tag rules for Resource:" + resourceName);
            microsoftElasticClient.TagRules.CreateOrUpdate(resourceGroup,resourceName, "default",monitoringTagRulesProperties);
    
        }

    }
}
