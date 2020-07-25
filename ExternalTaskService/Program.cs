using Camunda.Api.Client;
using Camunda.Api.Client.Deployment;
using Camunda.Api.Client.ExternalTask;
using Camunda.Api.Client.ProcessDefinition;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Camunda.ExternalTask.Service
{
    class Program
    {
        private static CamundaClient _client = CamundaClient.Create("http://localhost:8080/engine-rest");
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            AwaitNetExternalTaskSync().Wait();
        }

        private static async Task AwaitNetExternalTaskSync()
        {
            FetchExternalTaskTopic topic = new FetchExternalTaskTopic("address-info", 10000);

            var topics = new System.Collections.Generic.List<FetchExternalTaskTopic>();
            topics.Add(topic);
            
                FetchExternalTasks t = new FetchExternalTasks()
            {
                WorkerId = "postman",
                AsyncResponseTimeout = 30000,
                MaxTasks = 3,
                Topics = topics
            };

            var et = await _client.ExternalTasks.FetchAndLock(t);

            foreach (var task in et)
            {
                Console.WriteLine(task.Id + " " + task.TopicName);
            }
        }

        private static async Task PrintAllDeployments()
        {

            DeploymentQuery query = new DeploymentQuery()
            {
                IncludeDeploymentsWithoutTenantId = true
            };

            var deployments = await _client.Deployments.Query(query).List();

            foreach (var d in deployments)
            {
                Console.WriteLine($"{d.Id} {d.Name} { d.DeploymentTime}");
            }
        }
    }
}
