#r "Newtonsoft.Json"

using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

public static async Task<Response> Run(HttpRequest req, ILogger log)
{
    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
    dynamic data = JsonConvert.DeserializeObject(requestBody);

    double x = data.x;       // x protein concentration in uM (scalar)
    double y = data.y;       // y protein concentration in uM (scalar)
    double z = data.z;       // z protein concentration in uM (scalar)
    double Pxy = data.Pxy;   // production parameter
    double Pxz = data.Pxz;   // production parameter
    double Pyz = data.Pyz;   // production parameter
    double Px = data.Px;     // degradation parameter
    double Py = data.Py;     // degradation parameter
    double Pz = data.Pz;     // degradation parameter

    y = Pxy*x + Py*y;        // difference equation
    z = Pxz*x + Pyz*y + Pz*z;
    log.LogInformation($"The new y protein concentration is {y}");
    log.LogInformation($"The new z protein concentration is {z}");

    // // return x, y and z (return multiple data)
    Response r = new Response();
    r.x = x;
    r.y = y;
    r.z = z;
    return r;
}

public class Response
{
    public double x;  // x protein (scalar)
    public double y;  // y protein (scalar)
    public double z;  // z protein (scalar)
}