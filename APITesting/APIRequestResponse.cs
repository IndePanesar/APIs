// Author       : Inde Panesar
// Date         : Feb 2017
// Description  : Request Response for API testing
using RestSharp;
using System;
using TechTalk.SpecFlow;
using FluentAssertions;
using System.Linq;
using System.Net;
using System.Xml;
using NUnit.Framework;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Configuration;


namespace APITesting
{
    public class APIRequestResponse
    {
        /// <summary>
        /// Submits a request to the API end point
        /// </summary>
        /// <param name="p_Method"></param>
        /// <param name="p_ApiPoint"></param>
        /// <returns></returns>
        public IRestResponse SubmitSOAPRequest(string p_Method, string p_ApiPoint)
        {
            try
            {
                // Map the method to the HTTP Method enum
                var method = (Method)Enum.Parse(typeof(Method), p_Method, true);
                var urlEndpoint = (string)ScenarioContext.Current["API_URL"];
                var apiPoint = p_ApiPoint;
                var parameters = (string)ScenarioContext.Current["API_PARAMETERS"];
                var client = new RestClient($"http://www.{urlEndpoint}/{p_ApiPoint}?{parameters}");

                // www.webservicex.net/uklocation.asmx/GetUKLocationByCounty?County=Hampshire&Town=Slough");
                var request = new RestRequest(method);
                request.AddHeader("Cache-Control", "no-cache");
                request.AddHeader("Content-Type", "application/xml");
                return client.Execute(request);
            }
            catch (Exception exc)
            {
                
                Assert.Fail($"Failed to execute HTTP {p_Method} request to endpoint {p_ApiPoint} error {exc}");
            }
            return null;
        }

        public IRestResponse SubmitLocalHostRequest(string p_Method, string p_ApiPoint, string p_Port)
        {
            try
            {
                // Map the method to the HTTP Method enum
                var method = (Method)Enum.Parse(typeof(Method), p_Method, true);
                var parameters = "";
                var urlEndpoint = $"localhost:{p_Port}";
                if (ScenarioContext.Current.ContainsKey("FIBONACCI_INDEX"))
                    parameters = (string)ScenarioContext.Current["FIBONACCI_INDEX"];
                else
                    parameters = $"?{(string)ScenarioContext.Current["FIBONACCI_RANGE"]}";

                var client = new RestClient($"http://{urlEndpoint}/{p_ApiPoint}/{parameters}");

                // www.webservicex.net/uklocation.asmx/GetUKLocationByCounty?County=Hampshire&Town=Slough");
                var request = new RestRequest(method);
                request.AddHeader("Cache-Control", "no-cache");
                request.AddHeader("Content-Type", "application/json");
                return client.Execute(request);
            }
            catch (Exception exc)
            {
                Assert.Fail($"Failed to execute HTTP {p_Method} request to endpoint {p_ApiPoint} error {exc}");
            }
            return null;
        }
    }

    public class ConnectToApi
    {
        private readonly string _apiurl;
        public ConnectToApi()
        { _apiurl = ConfigurationManager.AppSettings["ApiUrl"];     }

        public IRestResponse ExecuteApiRequest(string pensionRequest, string xOptions)
        {
            var request = new RestRequest(Method.POST);
            var client = new RestClient(_apiurl);
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicy) => true;

            var requestjson = SerializeMappedRequest(pensionRequest);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("x-options", $"{{\"oan\":\"{xOptions}\" }}");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/json");
            
            // adding an authorization bearer token
            request.AddHeader("Authorization", "Bearer token_id");
            
            // basic username/password
            request.AddHeader("Authorization", "Basic dXNlcm5hbWU6cGFzc3dvcmQ=");


            // add authorization basic


            //request.AddBody(pensionRequest);
            request.AddParameter("application/json; charset=utf-8", requestjson, ParameterType.RequestBody);

            return client.Execute(request);
        }

        public static string SerializeMappedRequest(string p_Resquest)
        {
            var camelCaseSetting = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            return JsonConvert.SerializeObject(p_Resquest, camelCaseSetting);
        }
    }

    // SoapAPITests.Steps

    [Binding]
    public class SoapUIStesps
    {
        private IRestResponse _curr_response, _prev_response;
        private APIRequestResponse _api_requestresponse;

        [Given(@"I have a local api end point at port '(.*)'")]
        public void GivenIHaveALocalApiEndPointAtPort(string p_Port)
        {
            _api_requestresponse = new APIRequestResponse();

        }

        [Given(@"I have an api end point at '(.*)'")]
        public void GivenIHaveAnApiEndPointAt(string p_EndPoint)
        {
            // Save the end point in the Scenario context
            if (ScenarioContext.Current.ContainsKey("API_URL"))
                ScenarioContext.Current.Remove("API_URL");
            ScenarioContext.Current["API_URL"] = p_EndPoint;
            _api_requestresponse = new APIRequestResponse();
        }

        [Given(@"I want the Fibonacci number at index '(.*)'")]
        public void GivenIWantTheFibonacciNumberAtIndex(string p_Index)
        {
            // Save the parameters in the Scenario context
            if (ScenarioContext.Current.ContainsKey("FIBONACCI_INDEX"))
                ScenarioContext.Current.Remove("FIBONACCI_INDEX");
            ScenarioContext.Current["FIBONACCI_INDEX"] = p_Index;
        }

        [Given(@"I want the Fibonacci numbers from '(.*)' to '(.*)'")]
        public void GivenIWantTheFibonacciNumbersFromTo(string p_StartIndex, string p_FinishIndex)
        {
            // Save the parameters in the Scenario context
            if (ScenarioContext.Current.ContainsKey("FIBONACCI_RANGE"))
                ScenarioContext.Current.Remove("FIBONACCI_RANGE");
            ScenarioContext.Current["FIBONACCI_RANGE"] = $"startIndex={p_StartIndex}&finishIndex={p_FinishIndex}";
        }

        [When(@"I submit request for Fibonacci number")]
        public void WhenISubmitRequestForFibonacciNumber()
        {
            _curr_response = _api_requestresponse.SubmitLocalHostRequest("GET", "fib", "7003");
        }

        [When(@"I submit request for Fibonacci range")]
        public void WhenISubmitRequestForFibonacciRange()
        {
            _curr_response = _api_requestresponse.SubmitLocalHostRequest("GET", "fib/range", "7003");
        }

        [Then(@"the value should be '(.*)'")]
        public void ThenTheValueShouldBe(string p_Fibonacci)
        {
            _curr_response.Content.Should().Contain(p_Fibonacci);
        }

        ////////////////////////////////////////////////////////////////////
        [Given(@"I want to get uk location by county '(.*)'")]
        public void GivenIWantToGetUkLocationByCounty(string p_County)
        {
            // Save the parameters in the Scenario context
            if (ScenarioContext.Current.ContainsKey("API_PARAMETERS"))
                ScenarioContext.Current.Remove("API_PARAMETERS");
            ScenarioContext.Current["API_PARAMETERS"] = $"County={p_County}";
        }

        [When(@"I submit an http request of type '(.*)' to '(.*)'")]
        public void WhenISubmitAnHttpRequestOfTypeTo(string p_Method, string p_ApiPoint)
        {
            // Save the API point in  ScenarioContext
            if (ScenarioContext.Current.ContainsKey("API_POINT"))
                ScenarioContext.Current.Remove("API_POINT");
            ScenarioContext.Current["API_POINT"] = p_ApiPoint;

            _curr_response = _api_requestresponse.SubmitSOAPRequest(p_Method, p_ApiPoint);
        }

        [Then(@"I get response status of OK")]
        public void ThenIGetResponseStatusOfOK()
        {
            _curr_response.StatusCode.Should().Be(HttpStatusCode.OK, "Expected stus code of 200 OK");
        }

        [Then(@"the response body of locations by county '(.*)'")]
        public void ThenTheResponseBodyOfLocationsByCounty(string p_County)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(WebUtility.HtmlDecode(_curr_response.Content));
            XmlNodeList countyList = xmlDoc.GetElementsByTagName("Table");
            var countyNames = countyList.Cast<XmlNode>().Where(p_XML => p_XML["County"].InnerText == p_County)
                                                        .Select(p_Node => p_Node["County"].InnerText)
                                                        .ToList();

            // We should find the number of counties found be the size of the response table entries
            countyNames.Should().NotBeNullOrEmpty($"Expect to find county {p_County}");
            countyNames.Count.Should().Be(countyList.Count);
        }

        [When(@"I submit request '(.*)' to get uk location by county '(.*)' and town '(.*)'")]
        public void WhenISubmitRequestToGetUkLocationByCountyAndTown(string p_Method, string p_County, string p_Town)
        {
            if (ScenarioContext.Current.ContainsKey("API_PARAMETERS"))
                ScenarioContext.Current.Remove("API_PARAMETERS");
            ScenarioContext.Current["API_PARAMETERS"] = $"County={p_County}&Town={p_Town}";
            _prev_response = _curr_response;
            _curr_response = _api_requestresponse.SubmitSOAPRequest(p_Method, (string)ScenarioContext.Current["API_POINT"]);
        }

        [Then(@"the response body is the same as previous")]
        public void ThenTheResponseBodyIsTheSameAsPrevious()
        {
            _curr_response.Should().NotBeNull("Expected a response object to be non null");
            var xmlCurrDoc = new XmlDocument();
            xmlCurrDoc.LoadXml(WebUtility.HtmlDecode(_curr_response.Content));

            var xmlPrevDoc = new XmlDocument();
            xmlPrevDoc.LoadXml(WebUtility.HtmlDecode(_prev_response.Content));

            xmlPrevDoc.Should().BeEquivalentTo(xmlCurrDoc, "Expected the two XML respnses to be the same");
        }
    }
}
