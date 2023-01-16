using System.Text;
using System.Net;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace HTTPServerFromScratch.ItSelf
{
    public class ControllerHandler : IHandler
    {
        private readonly Dictionary<string, Func<object?>> _routes;

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
    }
}
