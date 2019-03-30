using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CellScriptY : MonoBehaviour
{
    float x;        // x protein concentration in uM (scalar)
    float y;        // y protein concentration in uM (scalar)
    float z;        // z protein concentration in uM (scalar)
    float Pxy;      // production parameter
    float Pxz;      // production parameter
    float Pyz;      // production parameter
    float Px;       // degradation parameter
    float Py;       // degradation parameter
    float Pz;       // degradation parameter
    float g;        // green color intensity representing y protein concentration (0 - 1)
    float r;        // red color intensity representing z protein concentration (0 - 1)
    float yMax;     // maximal y protein concentration in uM
    float zmax;     // maximal z protein concentration in uM
    string url = "https://threegenenetwork.azurewebsites.net/api/ThreeGeneNetworkNanoService"; // service URL


    // Start is called before the first frame update
    void Start()
    {
        Pxy = 0.8f;
        Pxz = 0.8f;
        Pyz = 0.8f;
        Px = 0.95f;
        Py = 0.95f;
        Pz = 0.95f;
        x = 10.0f;    // constant x protein concentration (= 10 uM)
        y = 0.0f;     // initial y protein concentration (= 0 uM)
        z = 0.0f;     // initial z protein concentration (= 0 uM)
        yMax = 2 * Pxy * x / (1 - Py);  // assume yMax equals 2 times steady state level
        zmax = 2 * Pxz * x / (1 - Pz);  // assume zMax equals 2 times steady state level
    }

    // FixedUpdate is called once per frame.
    // 0.02 seconds (50 calls per second) is the default time between calls.
    async Task FixedUpdate()
    {
        Time.fixedDeltaTime = 0.05f;  // the time interval in seconds at which physics and other fixed
                                      // fixed frame updates are performed.

        // y = Pxy * x + Py * y;  // difference equation

        // data to be sent to the server
        RequestData requestData = new RequestData();
        requestData.x = x;
        requestData.y = y;
        requestData.z = z;
        requestData.Pxy = Pxy;
        requestData.Pxz = Pxz;
        requestData.Pyz = Pyz;
        requestData.Px = Px;
        requestData.Py = Py;
        requestData.Pz = Pz;

        using (HttpClient client = new HttpClient())
        {
            var values = await PostAsync(client, requestData, url); // make a web service request
            y = values.y;
            z = values.z;
        }

        g = y / yMax;          // compute g
        r = z / zmax;          // compute r
        GetComponent<Renderer>().material.color = new Color(0, g, 0); // update the green color intensity
        //GetComponent<Renderer>().material.color = new Color(r, 0, 0); // update the red color intensity
        Debug.Log("y: " + y);  // display y value in the Unity console window
        Debug.Log("g: " + g);  // display g value in the Unity console window
        //Debug.Log("z: " + z);  // display z value in the Unity console window
        //Debug.Log("r: " + r);  // display r value in the Unity console window
    }

    static async Task<ResponseData> PostAsync(HttpClient client, RequestData data, string url)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, url);
        string jsonRequestBody = JsonConvert.SerializeObject(data);
        request.Content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");
        var response = await client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        ResponseData responseData = JsonConvert.DeserializeObject<ResponseData>(content);
        return responseData;
    }
}

public class RequestData
{
    public float x;
    public float y;
    public float z;
    public float Pxy;
    public float Pxz;
    public float Pyz;
    public float Px;
    public float Py;
    public float Pz;
}

// server returns x, y and z
public class ResponseData
{
    public float x;
    public float y;
    public float z;
}
