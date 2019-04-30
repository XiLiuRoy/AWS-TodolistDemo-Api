using System;
using System.Net;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using TodolistDemo.Domain;

namespace TodolistDemo.LambdaFunctions
{
    public class DeleteTodo
    {
        private readonly IDynamoDBContext _dbContext;

        public DeleteTodo()
        {
            AWSConfigsDynamoDB.Context.TypeMappings[typeof(TodoItem)] = new Amazon.Util.TypeMapping(typeof(TodoItem), Functions.Table);
            var config = new DynamoDBContextConfig { Conversion = DynamoDBEntryConversion.V2 };
            _dbContext = new DynamoDBContext(new AmazonDynamoDBClient(), config);
        }

        /// <summary>
        /// A Lambda function to respond to HTTP Delete methods from API Gateway
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<APIGatewayProxyResponse> Delete(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var id = string.Empty;
            if (request.PathParameters != null && request.PathParameters.ContainsKey(Functions.QueryParameterId))
                id = request.PathParameters[Functions.QueryParameterId];
            else if (request.QueryStringParameters != null && request.QueryStringParameters.ContainsKey(Functions.QueryParameterId))
                id = request.QueryStringParameters[Functions.QueryParameterId];

            if (string.IsNullOrWhiteSpace(id))
                throw new ApplicationException($"{nameof(id)} cannot be null or empty.");

            context.Logger.LogLine($"Deleting todo item with id {id}");

            await _dbContext.DeleteAsync<TodoItem>(id);
            return new APIGatewayProxyResponse()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Headers = Functions.LambdaHeader
            };

        }
    }
}
