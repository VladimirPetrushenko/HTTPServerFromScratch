using System.Text;
using System.Net;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Net.Sockets;
using System.Linq.Expressions;

namespace HTTPServerFromScratch.ItSelf
{
    public class ControllerHandler : IHandler
    {
        private readonly Dictionary<string, Func<object?>> _routes;
        private readonly Dictionary<Type, Func<Task, object?>> _extractors = new Dictionary<Type, Func<Task, object?>>();

        public ControllerHandler(Assembly assembly)
        {
            _routes = assembly.GetTypes()
                .Where(x => typeof(IController).IsAssignableFrom(x))
                .SelectMany(controller => controller.GetMethods().Select(method => new { controller, method }))
                .ToDictionary(
                    key => GetPath(key.controller, key.method), 
                    value => GetEndpointMethod(value.controller, value.method));

        }

        private static Func<object?> GetEndpointMethod(Type controller, MethodInfo method)
        {
            return () => method.Invoke(Activator.CreateInstance(controller), Array.Empty<object>());
        }

        private static string GetPath(Type controller, MethodInfo method)
        {
            var name = controller.Name;
            if (name.EndsWith("controller", StringComparison.InvariantCultureIgnoreCase))
            {
                name = name.Substring(0, name.Length - "controller".Length);
            }

            if (method.Name.Equals("Index", StringComparison.InvariantCultureIgnoreCase))
            {
                return name;
            }

            return name + "/" + method.Name;
        }

        public void Handle(Stream networkStream, Request request)
        {
            if(!_routes.TryGetValue(request.Path, out var func))
            {
                ResponseWriter.WriteStatus(HttpStatusCode.NotFound, networkStream);
            }
            else
            {
                ResponseWriter.WriteStatus(HttpStatusCode.OK, networkStream);
                WriteControllerResponse(func(), networkStream);
            }
        }

        public async Task HandleAsync(Stream networkStream, Request request)
        {
            if (!_routes.TryGetValue(request.Path, out var func))
            {
                await ResponseWriter.WriteStatusAsync(HttpStatusCode.NotFound, networkStream);
            }
            else
            {
                await ResponseWriter.WriteStatusAsync(HttpStatusCode.OK, networkStream);
                await WriteControllerResponseAsync(func(), networkStream);
            }
        }

        private void WriteControllerResponse(object? response, Stream networkStream)
        {
            if(response is string str)
            {
                using var writer = new StreamWriter(networkStream, leaveOpen: true);
                writer.Write(str);
            }
            else if(response is byte[] bytes)
            {
                networkStream.Write(bytes, 0, bytes.Length);
            }
            else
            {
                WriteControllerResponse(JsonSerializer.Serialize(response), networkStream);
            }
        }

        private async Task WriteControllerResponseAsync(object? response, Stream networkStream)
        {
            if (response is string str)
            {
                using var writer = new StreamWriter(networkStream, leaveOpen: true);
                await writer.WriteAsync(str);
            }
            else if (response is byte[] bytes)
            {
                await networkStream.WriteAsync(bytes, 0, bytes.Length);
            }
            else if(response is Task task)
            {
                await task;
                await WriteControllerResponseAsync(ExtractValue(task), networkStream);
                //await WriteControllerResponseAsync(task.GetType().GetProperty("Result")?.GetValue(task), networkStream);
            }
            else
            {
                await WriteControllerResponseAsync(JsonSerializer.Serialize(response), networkStream);
            }
        }
        
        private object? ExtractValue(Task task)
        {
            var taskType = task.GetType();
            if (!taskType.IsGenericType)
            {
                return null;
            }

            if(!_extractors.TryGetValue(taskType, out var extractor))
            {
                _extractors.Add(taskType, extractor = CreateExtractor(taskType));
            }

            return extractor(task);
        }

        private Func<Task, object?> CreateExtractor(Type taskType)
        {
            var param = Expression.Parameter(typeof(Task));
            return (Func<Task, object?>)Expression.Lambda(typeof(Func<Task, object?>), 
                Expression.Convert(
                    Expression.Property(
                        Expression.Convert(param, taskType), 
                        "Result"), 
                    typeof(object)),
                param)
                .Compile();
        }
    }
}
