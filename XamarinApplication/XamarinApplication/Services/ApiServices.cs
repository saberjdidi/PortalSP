using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.Connectivity;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Xamarin.Forms;
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.Views;

namespace XamarinApplication.Services
{
    public class ApiServices
    {
        public INavigation Navigation { get; set; }
        public async Task<Response> CheckConnection()
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = "Please turn on your internet settings.",
                };
            }

            var isReachable = await CrossConnectivity.Current.IsRemoteReachable(
                "google.com");
            if (!isReachable)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = "Check your internet connection.",
                };
            }

            return new Response
            {
                IsSuccess = true,
                Message = "Ok",
            };
        }

        public async Task<Response> GetList<T>(
            string urlBase,
            string servicePrefix,
            string controller)
        {
            try
            {
                var client = new HttpClient();
                //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                client.BaseAddress = new Uri(urlBase);
                var url = string.Format("{0}{1}", servicePrefix, controller);
                Debug.WriteLine("********url*************");
                Debug.WriteLine(url);
                var response = await client.GetAsync(url);
                var result = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("********result*************");
                Debug.WriteLine(result);
                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = result,
                    };
                }

                var list = JsonConvert.DeserializeObject<List<T>>(result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                return new Response
                {
                    IsSuccess = true,
                    Message = "Ok",
                    Result = list,
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }
        public async Task<Response> GetListWithCoockie<T>(
            string urlBase,
            string servicePrefix,
            string controller,
            string cookie)
        {
            try
            {
                var cookieContainer = new CookieContainer();
                var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
                var client = new HttpClient(handler);
                client.BaseAddress = new Uri(urlBase);
                var url = string.Format("{0}{1}", servicePrefix, controller);
                Debug.WriteLine("********url*************");
                Debug.WriteLine(url);
                cookieContainer.Add(client.BaseAddress, new Cookie("JSESSIONID", cookie));
                var response = await client.GetAsync(url);
                var result = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("********result*************");
                Debug.WriteLine(result);
                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = result,
                    };
                }

                var list = JsonConvert.DeserializeObject<List<T>>(result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                Debug.WriteLine("+++++++++++++++++++++++++list++++++++++++++++++++++++");
                Debug.WriteLine(list);
                return new Response
                {
                    IsSuccess = true,
                    Message = "Ok",
                    Result = list,
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }
        public async Task<Response> GetAttachmentWithCoockie<T>(
            string urlBase,
            string servicePrefix,
            string controller,
            string cookie)
        {
            try
            {
                var cookieContainer = new CookieContainer();
                var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
                var client = new HttpClient(handler);
                client.BaseAddress = new Uri(urlBase);
                var url = string.Format("{0}{1}", servicePrefix, controller);
                Debug.WriteLine("********url*************");
                Debug.WriteLine(url);
                cookieContainer.Add(client.BaseAddress, new Cookie("JSESSIONID", cookie));
                var response = await client.GetAsync(url);
                var result = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("********result*************");
                Debug.WriteLine(result);
                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = result,
                    };
                }

                var list = JsonConvert.DeserializeObject<T>(result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                Debug.WriteLine("********list*************");
                Debug.WriteLine(list);
                return new Response
                {
                    IsSuccess = true,
                    Message = "Ok",
                    Result = list,
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }
        public async Task<Response> ListFromXmlToJson<T>(
            string urlBase,
            string servicePrefix,
            string controller,
            string cookie)
        {
            try
            {
                var cookieContainer = new CookieContainer();
                var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
                var client = new HttpClient(handler);
                client.BaseAddress = new Uri(urlBase);
                var url = string.Format("{0}{1}", servicePrefix, controller);
                Debug.WriteLine("********url*************");
                Debug.WriteLine(url);
                cookieContainer.Add(client.BaseAddress, new Cookie("JSESSIONID", cookie));
                var response = await client.GetAsync(url);
                var result = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("********result*************");
                Debug.WriteLine(result);
                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = result,
                    };
                }

                var xml = "@" + result;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(result);
                var list = JsonConvert.SerializeXmlNode(doc);
                Debug.WriteLine("+++++++++++++++++++++++++list++++++++++++++++++++++++");
                Debug.WriteLine(list);
                var res = list.Remove(0, 27);
                Debug.WriteLine("+++++++++++++++++++++++++res++++++++++++++++++++++++");
                Debug.WriteLine(res);
                var output = res.Substring(0, res.Length - 2);
                Debug.WriteLine("+++++++++++++++++++++++++output++++++++++++++++++++++++");
                Debug.WriteLine(output);
                //var json = JsonConvert.DeserializeObject<T>(list);
                var json = JsonConvert.DeserializeObject<List<T>>(output, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                Debug.WriteLine("+++++++++++++++++++++++++json++++++++++++++++++++++++");
                Debug.WriteLine(json);
                return new Response
                {
                    IsSuccess = true,
                    Message = "Ok",
                    Result = json,
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }
        public async Task<Response> PostRequest<T>(
           string urlBase,
           string servicePrefix,
           string controller,
           string cookie,
           SearchModel searchModel)
        {
            try
            {
                var request = JsonConvert.SerializeObject(searchModel);
                Debug.WriteLine("********request*************");
                Debug.WriteLine(request);
                var content = new StringContent(
                    request,
                    Encoding.UTF8,
                    "application/json");
                var cookieContainer = new CookieContainer();
                var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
                var client = new HttpClient(handler);
                client.BaseAddress = new Uri(urlBase);
                var url = string.Format("{0}{1}", servicePrefix, controller);
                cookieContainer.Add(client.BaseAddress, new Cookie("JSESSIONID",cookie));
                var response = await client.PostAsync(url, content);
                Debug.WriteLine("********response*************");
                Debug.WriteLine(response);
                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = response.StatusCode.ToString(),
                    };
                }

                var result = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("********result*************");
                Debug.WriteLine(result);
                //var newRecord = JsonConvert.DeserializeObject<T>(result);
                var list = JsonConvert.DeserializeObject<List<T>>(result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                Debug.WriteLine("+++++++++++++++++++++++++list++++++++++++++++++++++++");
                Debug.WriteLine(list);
                return new Response
                {
                    IsSuccess = true,
                    Message = "OK",
                    Result = list,
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }
        public async Task<Response> PostRequest<T>(
           string urlBase,
           string servicePrefix,
           string controller,
           int pageIndex,
           int pageSize,
           string cookie,
           SearchModel searchModel)
        {
            try
            {
                var request = JsonConvert.SerializeObject(searchModel);
                Debug.WriteLine("********request*************");
                Debug.WriteLine(request);
                var content = new StringContent(
                    request,
                    Encoding.UTF8,
                    "application/json");
                var cookieContainer = new CookieContainer();
                var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
                var client = new HttpClient(handler);
                client.BaseAddress = new Uri(urlBase);
                var url = string.Format("{0}{1}", servicePrefix, controller);
                cookieContainer.Add(client.BaseAddress, new Cookie("JSESSIONID", cookie));
                var response = await client.PostAsync(url, content);
                Debug.WriteLine("********response*************");
                Debug.WriteLine(response);
                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = response.StatusCode.ToString(),
                    };
                }

                var result = await response.Content.ReadAsStringAsync();
                //var newRecord = JsonConvert.DeserializeObject<T>(result);
                var list = JsonConvert.DeserializeObject<List<T>>(result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                await Task.Delay(1000);
                Debug.WriteLine("+++++++++++++++++++++++++list++++++++++++++++++++++++");
                Debug.WriteLine(list);
                return new Response
                {
                    IsSuccess = true,
                    Message = "OK",
                    Result = list.Skip(pageIndex * pageSize).Take(pageSize).ToList(),
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }
        public async Task<Response> GetEventPatient<T>(
           string urlBase,
           string servicePrefix,
           string controller,
           long id,
           string cookie)
        {
            try
            {
                var cookieContainer = new CookieContainer();
                var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
                var client = new HttpClient(handler);
                client.BaseAddress = new Uri(urlBase);
                var url = string.Format("{0}{1}", servicePrefix, controller, id);
                //var url = $"{servicePrefix}{controller}/{id}";
                Debug.WriteLine("********url*************");
                Debug.WriteLine(url);
                cookieContainer.Add(client.BaseAddress, new Cookie("JSESSIONID", cookie));
                var response = await client.GetAsync(url);
                Debug.WriteLine("********response*************");
                Debug.WriteLine(response);
                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = response.StatusCode.ToString(),
                    };
                }

                var result = await response.Content.ReadAsStringAsync();
                //var newRecord = JsonConvert.DeserializeObject<T>(result);
                var list = JsonConvert.DeserializeObject<List<T>>(result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                Debug.WriteLine("+++++++++++++++++++++++++list++++++++++++++++++++++++");
                Debug.WriteLine(list);
                return new Response
                {
                    IsSuccess = true,
                    Message = "OK",
                    Result = list,
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }
        //Attachment list
        public async Task<Response> PostAttachment<T>(
           string urlBase,
           string servicePrefix,
           string controller,
           string cookie,
           SearchAttachment search)
        {
            try
            {
                var request = JsonConvert.SerializeObject(search);
                Debug.WriteLine("********request*************");
                Debug.WriteLine(request);
                var content = new StringContent(
                    request,
                    Encoding.UTF8,
                    "application/json");
                var cookieContainer = new CookieContainer();
                var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
                var client = new HttpClient(handler);
                client.BaseAddress = new Uri(urlBase);
                var url = string.Format("{0}{1}", servicePrefix, controller);
                cookieContainer.Add(client.BaseAddress, new Cookie("JSESSIONID", cookie));
                var response = await client.PostAsync(url, content);
                Debug.WriteLine("********response*************");
                Debug.WriteLine(response);
                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = response.StatusCode.ToString(),
                    };
                }

                var result = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("********result*************");
                Debug.WriteLine(result);
                //var newRecord = JsonConvert.DeserializeObject<T>(result);
                var list = JsonConvert.DeserializeObject<List<T>>(result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                Debug.WriteLine("+++++++++++++++++++++++++list++++++++++++++++++++++++");
                Debug.WriteLine(list);
                return new Response
                {
                    IsSuccess = true,
                    Message = "OK",
                    Result = list,
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }
        //Execution Report list
        public async Task<Response> PostExecutionReport<T>(
           string urlBase,
           string servicePrefix,
           string controller,
           string cookie,
           SearchExecutionReport search)
        {
            try
            {
                var request = JsonConvert.SerializeObject(search);
                Debug.WriteLine("********request*************");
                Debug.WriteLine(request);
                var content = new StringContent(
                    request,
                    Encoding.UTF8,
                    "application/json");
                var cookieContainer = new CookieContainer();
                var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
                var client = new HttpClient(handler);
                client.BaseAddress = new Uri(urlBase);
                var url = string.Format("{0}{1}", servicePrefix, controller);
                cookieContainer.Add(client.BaseAddress, new Cookie("JSESSIONID", cookie));
                var response = await client.PostAsync(url, content);
                Debug.WriteLine("********response*************");
                Debug.WriteLine(response);
                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = response.StatusCode.ToString(),
                    };
                }

                var result = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("********result*************");
                Debug.WriteLine(result);
                //var newRecord = JsonConvert.DeserializeObject<T>(result);
                var list = JsonConvert.DeserializeObject<List<T>>(result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                Debug.WriteLine("+++++++++++++++++++++++++list++++++++++++++++++++++++");
                Debug.WriteLine(list);
                return new Response
                {
                    IsSuccess = true,
                    Message = "OK",
                    Result = list,
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }
        public async Task<Response> PostRequestCatalog<T>(
           string urlBase,
           string servicePrefix,
           string controller,
           string cookie,
           SearchRequestCatalog search)
        {
            try
            {
                var request = JsonConvert.SerializeObject(search);
                Debug.WriteLine("********request*************");
                Debug.WriteLine(request);
                var content = new StringContent(
                    request,
                    Encoding.UTF8,
                    "application/json");
                var cookieContainer = new CookieContainer();
                var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
                var client = new HttpClient(handler);
                client.BaseAddress = new Uri(urlBase);
                var url = string.Format("{0}{1}", servicePrefix, controller);
                cookieContainer.Add(client.BaseAddress, new Cookie("JSESSIONID", cookie));
                var response = await client.PostAsync(url, content);
                Debug.WriteLine("********response*************");
                Debug.WriteLine(response);
                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = response.StatusCode.ToString(),
                    };
                }

                var result = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("********result*************");
                Debug.WriteLine(result);
                //var newRecord = JsonConvert.DeserializeObject<T>(result);
                var list = JsonConvert.DeserializeObject<List<T>>(result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                Debug.WriteLine("+++++++++++++++++++++++++list++++++++++++++++++++++++");
                Debug.WriteLine(list);
                return new Response
                {
                    IsSuccess = true,
                    Message = "OK",
                    Result = list,
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }
        public async Task<Response> PostRequestInstrument<T>(
           string urlBase,
           string servicePrefix,
           string controller,
           string cookie,
           SearchInstrument search)
        {
            try
            {
                var request = JsonConvert.SerializeObject(search);
                Debug.WriteLine("********request Instrument*************");
                Debug.WriteLine(request);
                var content = new StringContent(
                    request,
                    Encoding.UTF8,
                    "application/json");
                var cookieContainer = new CookieContainer();
                var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
                var client = new HttpClient(handler);
                client.BaseAddress = new Uri(urlBase);
                var url = string.Format("{0}{1}", servicePrefix, controller);
                cookieContainer.Add(client.BaseAddress, new Cookie("JSESSIONID", cookie));
                var response = await client.PostAsync(url, content);
                Debug.WriteLine("********response Instrument*************");
                Debug.WriteLine(response);
                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = response.StatusCode.ToString(),
                    };
                }

                var result = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("********result Instrument*************");
                Debug.WriteLine(result);
                //var newRecord = JsonConvert.DeserializeObject<T>(result);
                var list = JsonConvert.DeserializeObject<List<T>>(result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                Debug.WriteLine("+++++++++++++++++++++++++list++++++++++++++++++++++++");
                Debug.WriteLine(list);
                return new Response
                {
                    IsSuccess = true,
                    Message = "OK",
                    Result = list,
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }
        //save
        public async Task<Response> Save<T>(
            string urlBase,
            string servicePrefix,
            string controller,
            string cookie,
            T model)
        {
            try
            {
                var request = JsonConvert.SerializeObject(model);
                Debug.WriteLine("********request*************");
                Debug.WriteLine(request);
                var content = new StringContent(
                    request,
                    Encoding.UTF8,
                    "application/json");
                var cookieContainer = new CookieContainer();
                var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
                var client = new HttpClient(handler);
                client.BaseAddress = new Uri(urlBase);
                var url = string.Format("{0}{1}", servicePrefix, controller);
                cookieContainer.Add(client.BaseAddress, new Cookie("JSESSIONID", cookie));
                var response = await client.PostAsync(url, content);
                Debug.WriteLine("********response*************");
                Debug.WriteLine(response);
                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = response.StatusCode.ToString(),
                    };
                }

                var result = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("********result*************");
                Debug.WriteLine(result);
                var newRecord = JsonConvert.DeserializeObject<T>(result);
                //var newRecord = JsonConvert.DeserializeObject<List<T>>(result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                Debug.WriteLine("********newRecord*************");
                Debug.WriteLine(newRecord);
                return new Response
                {
                    IsSuccess = true,
                    Message = "Record added OK",
                    Result = newRecord,
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }
        //RoomHistoric
        public async Task<Response> RoomHistoric<T>(
            string urlBase,
            string servicePrefix,
            string controller,
            string cookie,
            Room model)
        {
            try
            {
                var request = JsonConvert.SerializeObject(model);
                Debug.WriteLine("********request*************");
                Debug.WriteLine(request);
                var content = new StringContent(
                    request,
                    Encoding.UTF8,
                    "application/json");
                var cookieContainer = new CookieContainer();
                var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
                var client = new HttpClient(handler);
                client.BaseAddress = new Uri(urlBase);
                var url = string.Format("{0}{1}", servicePrefix, controller);
                cookieContainer.Add(client.BaseAddress, new Cookie("JSESSIONID", cookie));
                var response = await client.PostAsync(url, content);
                Debug.WriteLine("********response*************");
                Debug.WriteLine(response);
                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = response.StatusCode.ToString(),
                    };
                }

                var result = await response.Content.ReadAsStringAsync();
                // var newRecord = JsonConvert.DeserializeObject<T>(result);
                var newRecord = JsonConvert.DeserializeObject<List<T>>(result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                Debug.WriteLine("********newRecord*************");
                Debug.WriteLine(newRecord);
                return new Response
                {
                    IsSuccess = true,
                    Message = "Record added OK",
                    Result = newRecord,
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }
        //InstrumentHistoric
        public async Task<Response> InstrumentHistoric<T>(
            string urlBase,
            string servicePrefix,
            string controller,
            string cookie,
            Instrument model)
        {
            try
            {
                var request = JsonConvert.SerializeObject(model);
                Debug.WriteLine("********request*************");
                Debug.WriteLine(request);
                var content = new StringContent(
                    request,
                    Encoding.UTF8,
                    "application/json");
                var cookieContainer = new CookieContainer();
                var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
                var client = new HttpClient(handler);
                client.BaseAddress = new Uri(urlBase);
                var url = string.Format("{0}{1}", servicePrefix, controller);
                cookieContainer.Add(client.BaseAddress, new Cookie("JSESSIONID", cookie));
                var response = await client.PostAsync(url, content);
                Debug.WriteLine("********response*************");
                Debug.WriteLine(response);
                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = response.StatusCode.ToString(),
                    };
                }

                var result = await response.Content.ReadAsStringAsync();
                // var newRecord = JsonConvert.DeserializeObject<T>(result);
                var newRecord = JsonConvert.DeserializeObject<List<T>>(result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                Debug.WriteLine("********newRecord*************");
                Debug.WriteLine(newRecord);
                return new Response
                {
                    IsSuccess = true,
                    Message = "Record added OK",
                    Result = newRecord,
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }
        //put
        public async Task<Response> PutPatient<T>(
           string urlBase,
           string servicePrefix,
           string controller,
           string cookie,
           T model)
        {
            try
            {
                var request = JsonConvert.SerializeObject(model);
                Debug.WriteLine("********request*************");
                Debug.WriteLine(request);
                var content = new StringContent(
                    request,
                    Encoding.UTF8,
                    "application/json");
                var cookieContainer = new CookieContainer();
                var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
                var client = new HttpClient(handler);
                client.BaseAddress = new Uri(urlBase);
                var url = string.Format("{0}{1}", servicePrefix, controller);
                cookieContainer.Add(client.BaseAddress, new Cookie("JSESSIONID", cookie));
                var response = await client.PutAsync(url, content);
                Debug.WriteLine("********response*************");
                Debug.WriteLine(response);
                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = response.StatusCode.ToString(),
                    };
                }

                var result = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("********result*************");
                Debug.WriteLine(result);
                var newRecord = JsonConvert.DeserializeObject<T>(result);
                Debug.WriteLine("********newRecord*************");
                Debug.WriteLine(newRecord);
                return new Response
                {
                    IsSuccess = true,
                    Message = "Record added OK",
                    Result = newRecord,
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }
        public async Task<Response> Put<T>(
            string urlBase,
            string servicePrefix,
            string controller,
            string cookie,
            T model)
        {
            try
            {
                var request = JsonConvert.SerializeObject(model);
                Debug.WriteLine("********request*************");
                Debug.WriteLine(request);
                var content = new StringContent(
                    request,
                    Encoding.UTF8,
                    "application/json");
                var cookieContainer = new CookieContainer();
                var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
                var client = new HttpClient(handler);
                client.BaseAddress = new Uri(urlBase);
                var url = string.Format("{0}{1}", servicePrefix, controller);
                cookieContainer.Add(client.BaseAddress, new Cookie("JSESSIONID", cookie));
                var response = await client.PutAsync(url, content);
                Debug.WriteLine("********response*************");
                Debug.WriteLine(response);
                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = response.StatusCode.ToString(),
                    };
                }

                var result = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("********result*************");
                Debug.WriteLine(result);
                var newRecord = JsonConvert.DeserializeObject<T>(result);
                Debug.WriteLine("********newRecord*************");
                Debug.WriteLine(newRecord);
                return new Response
                {
                    IsSuccess = true,
                    Message = "Record updated OK",
                    Result = newRecord,
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }
        //Put User
        public async Task<Response> PutUser<T>(
            string urlBase,
            string servicePrefix,
            string controller,
            string cookie,
            T model)
        {
            try
            {
                var request = JsonConvert.SerializeObject(model);
                Debug.WriteLine("********request*************");
                Debug.WriteLine(request);
                PopuPage page1 = new PopuPage();
                await PopupNavigation.Instance.PushAsync(page1);
                var content = new StringContent(
                    request,
                    Encoding.UTF8,
                    "application/json");
                var cookieContainer = new CookieContainer();
                var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
                var client = new HttpClient(handler);
                client.BaseAddress = new Uri(urlBase);
                var url = string.Format("{0}{1}", servicePrefix, controller);
                cookieContainer.Add(client.BaseAddress, new Cookie("JSESSIONID", cookie));
                var response = await client.PutAsync(url, content);
                Debug.WriteLine("********response*************");
                Debug.WriteLine(response);
                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = response.StatusCode.ToString(),
                    };
                }

                var result = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("********result*************");
                Debug.WriteLine(result);
                await App.Current.MainPage.Navigation.PopPopupAsync(true);
                var newRecord = JsonConvert.DeserializeObject<T>(result);
                Debug.WriteLine("********newRecord*************");
                Debug.WriteLine(newRecord);
                return new Response
                {
                    IsSuccess = true,
                    Message = "Record updated OK",
                    Result = newRecord,
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }
        //delete
        public async Task<Response> Delete<T>(
            string urlBase,
            string servicePrefix,
            string controller,
            string cookie)
        {
            try
            {
                var cookieContainer = new CookieContainer();
                var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
                var client = new HttpClient(handler);
                client.BaseAddress = new Uri(urlBase);
                /*var url = string.Format(
                    "{0}{1}/{2}",
                    servicePrefix,
                    controller,
                    id);*/
                var url = $"{servicePrefix}{controller}";
                cookieContainer.Add(client.BaseAddress, new Cookie("JSESSIONID", cookie));
                var response = await client.DeleteAsync(url);
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    var error = JsonConvert.DeserializeObject<Response>(result);
                    error.IsSuccess = false;
                    return error;
                }

                return new Response
                {
                    IsSuccess = true,
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }
        //Change Checked
        public async Task<Response> CheckBoxCheckChanged<T>(
          string urlBase,
          string servicePrefix,
          string controller,
          string cookie)
        {
            try
            {
                var cookieContainer = new CookieContainer();
                var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
                var client = new HttpClient(handler);
                client.BaseAddress = new Uri(urlBase);
                var url = string.Format("{0}{1}", servicePrefix, controller);
                cookieContainer.Add(client.BaseAddress, new Cookie("JSESSIONID", cookie));
                //var request = new HttpRequestMessage(HttpMethod.Post, url);
                //var response = await client.SendAsync(request);
                var response = await client.PostAsync(url, null);
                Debug.WriteLine("********response*************");
                Debug.WriteLine(response);
                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = response.StatusCode.ToString(),
                    };
                }

                var result = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("********result*************");
                Debug.WriteLine(result);
                var newRecord = JsonConvert.DeserializeObject<T>(result);
                //var list = JsonConvert.DeserializeObject<List<T>>(result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                return new Response
                {
                    IsSuccess = true,
                    Message = "OK",
                    Result = newRecord,
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }
        //Role Users
        public async Task<Response> PostRole<T>(
           string urlBase,
           string servicePrefix,
           string controller,
           string cookie,
           Role model)
        {
            try
            {
                var request = JsonConvert.SerializeObject(model);
                Debug.WriteLine("********request*************");
                Debug.WriteLine(request);
                var content = new StringContent(
                    request,
                    Encoding.UTF8,
                    "application/json");
                var cookieContainer = new CookieContainer();
                var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
                var client = new HttpClient(handler);
                client.BaseAddress = new Uri(urlBase);
                var url = string.Format("{0}{1}", servicePrefix, controller);
                cookieContainer.Add(client.BaseAddress, new Cookie("JSESSIONID", cookie));
                var response = await client.PostAsync(url, content);
                Debug.WriteLine("********response*************");
                Debug.WriteLine(response);
                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = response.StatusCode.ToString(),
                    };
                }

                var result = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("********result*************");
                Debug.WriteLine(result);
                //var newRecord = JsonConvert.DeserializeObject<T>(result);
                var list = JsonConvert.DeserializeObject<List<T>>(result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                Debug.WriteLine("+++++++++++++++++++++++++list++++++++++++++++++++++++");
                Debug.WriteLine(list);
                return new Response
                {
                    IsSuccess = true,
                    Message = "OK",
                    Result = list,
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }
        /*
        public async Task<Response> Post<T>(
            string urlBase,
            string servicePrefix,
            string controller,
            string tokenType,
            string accessToken,
            SearchModel searchModel)
        {
            try
            {
                var request = JsonConvert.SerializeObject(searchModel);
                var content = new StringContent(
                    request, Encoding.UTF8,
                    "application/json");
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(tokenType, accessToken);
                client.BaseAddress = new Uri(urlBase);
                var url = string.Format("{0}{1}", servicePrefix, controller);
                var response = await client.PostAsync(url, content);
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    var error = JsonConvert.DeserializeObject<Response>(result);
                    error.IsSuccess = false;
                    return error;
                }

                var newRecord = JsonConvert.DeserializeObject<T>(result);

                return new Response
                {
                    IsSuccess = true,
                    Message = "Record added OK",
                    Result = newRecord,
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }


        
          public async Task GetRequestsSearch(SearchModel searchModel)
        {
            var client = new HttpClient();

            var json = JsonConvert.SerializeObject(searchModel);
            HttpContent content = new StringContent(json);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await client.PostAsync("https://portalesp.smart-path.it/Portalesp/request/searchByExample", content);

           // var response = await client.PostAsync("https://portalesp.smart-path.it/Portalesp/request/searchByExample", content);
           // var ideas = JsonConvert.DeserializeObject<List<Request>>(response);
           // return response;
        }
        public async Task<List<Request>> SearchRequestsAsync(SearchModel searchModel)
        {
            var client = new HttpClient();

            var json = await client.GetStringAsync("https://portalesp.smart-path.it/Portalesp/request/searchByExample" + searchModel);

            var requests = JsonConvert.DeserializeObject<List<Request>>(json);

            return requests;
        }
          
          public async Task PostIdeaAsync(SearchModel searchModel, string accessToken)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var json = JsonConvert.SerializeObject(searchModel);
            HttpContent content = new StringContent(json);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await client.PostAsync("https://portalesp.smart-path.it/Portalesp/request/searchByExample", content);
            if (response.IsSuccessStatusCode)
            {
                //await Navigation.PopAsync();
                await Application.Current.MainPage.DisplayAlert("Message", "Idea Added", "ok");
            }
            else
            {
                //DisplayAlert("Message", "Data Failed To Save", "Ok");
                await Application.Current.MainPage.DisplayAlert("Message", "Failed to add Idea", "ok");
            }
            
        }
        */
    }
}
