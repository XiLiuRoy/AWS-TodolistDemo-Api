using System;
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
using Amazon;

namespace TodolistDemo.LambdaFunctions
{
    public class TodoListFunction
    {
        private readonly IDynamoDBContext _dbContext;
        private static string Table => "TodoList";
        public const string QueryParameterId = "Id";

        public TodoListFunction()
        {
            AWSConfigsDynamoDB.Context.TypeMappings[typeof(TodoItem)] =new Amazon.Util.TypeMapping(typeof(TodoItem), Table);
            var config = new DynamoDBContextConfig { Conversion = DynamoDBEntryConversion.V2};
            _dbContext = new DynamoDBContext(new AmazonDynamoDBClient(), config);
        }

        /// <summary>
        /// A Lambda function to respond to HTTP Get methods from API Gateway
        /// </summary>
        /// <param name="request"></param>
        /// <returns>The list of blogs</returns>
        public async Task<APIGatewayProxyResponse> Get(APIGatewayProxyRequest request, ILambdaContext context)
        {
            context.Logger.LogLine("Get Request\n");
            var todoItemsScan = _dbContext.ScanAsync<TodoItem>(null);
            var pageItems = await todoItemsScan.GetNextSetAsync();

            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = JsonConvert.SerializeObject(pageItems),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };

            return response;
        }

        /// <summary>
        /// A Lambda function to respond to HTTP put methods from API Gateway
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<APIGatewayProxyResponse> Put(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var vm = JsonConvert.DeserializeObject<TodoItemViewModel>(request?.Body);

            var todoItem = new TodoItem(vm.Name,vm.Description, vm.DueDate);

            context.Logger.LogLine($"Saving todo item with id {todoItem.Id}");
            await _dbContext.SaveAsync(todoItem);

            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = todoItem.Id,
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
            return response;
        }

        public async Task<APIGatewayProxyResponse> Delete(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var id = string.Empty;
            if (request.PathParameters != null && request.PathParameters.ContainsKey(QueryParameterId))
                id = request.PathParameters[QueryParameterId];
            else if (request.QueryStringParameters != null && request.QueryStringParameters.ContainsKey(QueryParameterId))
                id = request.QueryStringParameters[QueryParameterId];

            if(string.IsNullOrWhiteSpace(id))
                throw new ApplicationException($"{nameof(id)} cannot be null or empty.");

            context.Logger.LogLine($"Deleting todo item with id {id}");

            await _dbContext.DeleteAsync<TodoItem>(id);
            return new APIGatewayProxyResponse()
            {
                StatusCode = (int) HttpStatusCode.OK
            };

        }

    }
}
