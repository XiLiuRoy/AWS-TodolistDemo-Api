using System.Collections.Generic;

using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using System.Net;
using Newtonsoft.Json;
using TodolistDemo.Domain;
using TodolistDemo.ViewModel;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;
using System.Threading.Tasks;

namespace TodolistDemo.LambdaFunctions
{
    public class TodoListFunction
    {
        private IDynamoDBContext _dbContext;
        public TodoListFunction()
        {
            var config = new DynamoDBContextConfig { Conversion = DynamoDBEntryConversion.V2 };
            _dbContext = new DynamoDBContext(new AmazonDynamoDBClient(), config);
        }

        /// <summary>
        /// A Lambda function to respond to HTTP Get methods from API Gateway
        /// </summary>
        /// <param name="request"></param>
        /// <returns>The list of blogs</returns>
        public APIGatewayProxyResponse Get(APIGatewayProxyRequest request, ILambdaContext context)
        {
            context.Logger.LogLine("Get Request\n");
            var todolistDemo = new List<string>
            {
                "Go to Gym",
                "Buy soy source"
            };

            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = JsonConvert.SerializeObject(todolistDemo),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };

            return response;
        }

        public async Task<APIGatewayProxyResponse> Put(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var vm = JsonConvert.DeserializeObject<TodoItemViewModel>(request?.Body);

            var todoItem = new TodoItem(vm.Name,vm.Description, vm.DueDate);

            context.Logger.LogLine($"Saving todo item with id {todoItem.Id}");
            await _dbContext.SaveAsync<TodoItem>(todoItem);

            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = todoItem.Id.ToString(),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
            return response;
        }

    }
}
