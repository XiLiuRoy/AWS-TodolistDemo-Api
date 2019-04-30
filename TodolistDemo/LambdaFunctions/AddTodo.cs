using System.Net;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using TodolistDemo.Domain;
using TodolistDemo.ViewModel;

namespace TodolistDemo.LambdaFunctions
{
    public class AddTodo
    {
        private readonly IDynamoDBContext _dbContext;

        public AddTodo()
        {
            AWSConfigsDynamoDB.Context.TypeMappings[typeof(TodoItem)] = new Amazon.Util.TypeMapping(typeof(TodoItem), Functions.Table);
            var config = new DynamoDBContextConfig { Conversion = DynamoDBEntryConversion.V2 };
            _dbContext = new DynamoDBContext(new AmazonDynamoDBClient(), config);
        }

        /// <summary>
        /// A Lambda function to respond to HTTP Post methods from API Gateway
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<APIGatewayProxyResponse> Post(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var vm = JsonConvert.DeserializeObject<TodoItemViewModel>(request?.Body);

            var todoItem = new TodoItem(vm.Name, vm.Description, vm.DueDate);

            context.Logger.LogLine($"Saving todo item with id {todoItem.Id}");
            await _dbContext.SaveAsync(todoItem);

            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = JsonConvert.SerializeObject(new TodoItemViewModel(todoItem)),
                Headers = Functions.LambdaHeader
            };
            return response;
        }
    }
}
