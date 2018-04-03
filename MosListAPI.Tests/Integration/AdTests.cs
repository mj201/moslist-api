using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using ddbModel = Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Integration
{
    public class AdTests
    {
        private static HttpClient _client;
        static AdTests(){
            // Initialize Http Client
            _client = new HttpClient();
            _client.BaseAddress = new Uri("http://localhost:5000");
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

       public object GetSampleAdRequestObject(string body = "sample body", DateTime postDate = default(DateTime), string state = "ACTIVE", string title = "sample title", string userId = "default guid")
        {
            if (postDate == default(DateTime))
                postDate = DateTime.UtcNow;
            if (userId == "default guid")
                userId = Guid.NewGuid().ToString();
            return new {
                Body = body,
                PostDate = postDate,
                State = state,
                Title = title,
                UserId = userId
            };
        }

        [Fact]
        public async void Test_Post_Ad_Returns_Correct_Data()
        {
            // Arrange
            var ad = GetSampleAdRequestObject() as dynamic;
            var content = JsonConvert.SerializeObject(ad);
            HttpContent httpContent = new StringContent(content, System.Text.Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/ad", httpContent);

            // Assert
            // Make sure Status Code is OK
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Check that returned information is the same as submitted information
            var returnedContent = await response.Content.ReadAsStringAsync();
            var returnedAd = JsonConvert.DeserializeObject(returnedContent) as dynamic;
            AssertAreEqual(returnedAd, ad);

            // Make sure that additional fields are available in the response
            AssertIsValidAd(returnedAd);
        }

        [Fact]
        public async void Test_Post_Ad_Body_Is_Required()
        {
            // Arrange
            var ad1 = GetSampleAdRequestObject(body : null);
            var ad2 = GetSampleAdRequestObject(body : string.Empty);
            var ad3 = GetSampleAdRequestObject(body : "      ");

            var content1 = JsonConvert.SerializeObject(ad1);
            var content2 = JsonConvert.SerializeObject(ad2);
            var content3 = JsonConvert.SerializeObject(ad2);
            HttpContent httpContent1 = new StringContent(
                content1, System.Text.Encoding.UTF8, "application/json");
            HttpContent httpContent2 = new StringContent(
                content2, System.Text.Encoding.UTF8, "application/json");
            HttpContent httpContent3 = new StringContent(
                content3, System.Text.Encoding.UTF8, "application/json");

            // Act
            var response1 = await _client.PostAsync("/ad", httpContent1);
            var response2 = await _client.PostAsync("/ad", httpContent2);
            var response3 = await _client.PostAsync("/ad", httpContent2);

            // Assert
            // Make sure Status Code is Bad Request
            Assert.Equal(response1.StatusCode, HttpStatusCode.BadRequest);
            Assert.Equal(response2.StatusCode, HttpStatusCode.BadRequest);
            Assert.Equal(response3.StatusCode, HttpStatusCode.BadRequest);

            // Check that an error message is returned
            var responseContent1 = JsonConvert.DeserializeObject(await response1.Content.ReadAsStringAsync()) as dynamic;
            var responseContent2 = JsonConvert.DeserializeObject(await response2.Content.ReadAsStringAsync()) as dynamic;
            var responseContent3 = JsonConvert.DeserializeObject(await response3.Content.ReadAsStringAsync()) as dynamic;
            string errorMessage = null;
            var exception = Record.Exception(() => errorMessage = responseContent1.Body.ToString());
            Assert.Null(exception);
            Assert.False(string.IsNullOrEmpty(errorMessage));
            exception = Record.Exception(() => errorMessage = responseContent2.Body.ToString());
            Assert.Null(exception);
            Assert.False(string.IsNullOrEmpty(errorMessage));
            exception = Record.Exception(() => errorMessage = responseContent3.Body.ToString());
            Assert.Null(exception);
            Assert.False(string.IsNullOrEmpty(errorMessage));
        }
        
        [Fact]
        public async void Test_Post_Ad_State_Is_Required()
        {
            // Arrange
            var ad1 = GetSampleAdRequestObject(state : null);
            var ad2 = GetSampleAdRequestObject(state : string.Empty);
            var ad3 = GetSampleAdRequestObject(state : "junky type that is not a state");

            var content1 = JsonConvert.SerializeObject(ad1);
            var content2 = JsonConvert.SerializeObject(ad2);
            var content3 = JsonConvert.SerializeObject(ad2);
            HttpContent httpContent1 = new StringContent(
                content1, System.Text.Encoding.UTF8, "application/json");
            HttpContent httpContent2 = new StringContent(
                content2, System.Text.Encoding.UTF8, "application/json");
            HttpContent httpContent3 = new StringContent(
                content3, System.Text.Encoding.UTF8, "application/json");

            // Act
            var response1 = await _client.PostAsync("/ad", httpContent1);
            var response2 = await _client.PostAsync("/ad", httpContent2);
            var response3 = await _client.PostAsync("/ad", httpContent2);

            // Assert
            // Make sure Status Code is Bad Request
            Assert.Equal(response1.StatusCode, HttpStatusCode.BadRequest);
            Assert.Equal(response2.StatusCode, HttpStatusCode.BadRequest);
            Assert.Equal(response3.StatusCode, HttpStatusCode.BadRequest);

            // Check that an error message is returned
            var responseContent1 = JsonConvert.DeserializeObject(await response1.Content.ReadAsStringAsync()) as dynamic;
            var responseContent2 = JsonConvert.DeserializeObject(await response2.Content.ReadAsStringAsync()) as dynamic;
            var responseContent3 = JsonConvert.DeserializeObject(await response3.Content.ReadAsStringAsync()) as dynamic;
            string errorMessage = null;
            var exception = Record.Exception(() => errorMessage = responseContent1.State.ToString());
            Assert.Null(exception);
            Assert.False(string.IsNullOrEmpty(errorMessage));
            exception = Record.Exception(() => errorMessage = responseContent2.State.ToString());
            Assert.Null(exception);
            Assert.False(string.IsNullOrEmpty(errorMessage));
            exception = Record.Exception(() => errorMessage = responseContent3.State.ToString());
            Assert.Null(exception);
            Assert.False(string.IsNullOrEmpty(errorMessage));
        }
        
        [Fact]
        public async void Test_Post_Ad_Title_Is_Required()
        {
            // Arrange
            var ad1 = GetSampleAdRequestObject(title : null);
            var ad2 = GetSampleAdRequestObject(title : string.Empty);
            var ad3 = GetSampleAdRequestObject(title : "    ");

            var content1 = JsonConvert.SerializeObject(ad1);
            var content2 = JsonConvert.SerializeObject(ad2);
            var content3 = JsonConvert.SerializeObject(ad2);
            HttpContent httpContent1 = new StringContent(
                content1, System.Text.Encoding.UTF8, "application/json");
            HttpContent httpContent2 = new StringContent(
                content2, System.Text.Encoding.UTF8, "application/json");
            HttpContent httpContent3 = new StringContent(
                content3, System.Text.Encoding.UTF8, "application/json");

            // Act
            var response1 = await _client.PostAsync("/ad", httpContent1);
            var response2 = await _client.PostAsync("/ad", httpContent2);
            var response3 = await _client.PostAsync("/ad", httpContent2);

            // Assert
            // Make sure Status Code is Bad Request
            Assert.Equal(response1.StatusCode, HttpStatusCode.BadRequest);
            Assert.Equal(response2.StatusCode, HttpStatusCode.BadRequest);
            Assert.Equal(response3.StatusCode, HttpStatusCode.BadRequest);

            // Check that an error message is returned
            var responseContent1 = JsonConvert.DeserializeObject(await response1.Content.ReadAsStringAsync()) as dynamic;
            var responseContent2 = JsonConvert.DeserializeObject(await response2.Content.ReadAsStringAsync()) as dynamic;
            var responseContent3 = JsonConvert.DeserializeObject(await response3.Content.ReadAsStringAsync()) as dynamic;
            string errorMessage = null;
            var exception = Record.Exception(() => errorMessage = responseContent1.Title.ToString());
            Assert.Null(exception);
            Assert.False(string.IsNullOrEmpty(errorMessage));
            exception = Record.Exception(() => errorMessage = responseContent2.Title.ToString());
            Assert.Null(exception);
            Assert.False(string.IsNullOrEmpty(errorMessage));
            exception = Record.Exception(() => errorMessage = responseContent3.Title.ToString());
            Assert.Null(exception);
            Assert.False(string.IsNullOrEmpty(errorMessage));
        }     

        [Fact]
        public async void Test_Post_Ad_UserId_Is_Required()
        {
            // Arrange
            var ad1 = GetSampleAdRequestObject(userId : null);
            var ad2 = GetSampleAdRequestObject(userId : string.Empty);
            var ad3 = GetSampleAdRequestObject(userId : "non guid value");

            var content1 = JsonConvert.SerializeObject(ad1);
            var content2 = JsonConvert.SerializeObject(ad2);
            var content3 = JsonConvert.SerializeObject(ad2);
            HttpContent httpContent1 = new StringContent(
                content1, System.Text.Encoding.UTF8, "application/json");
            HttpContent httpContent2 = new StringContent(
                content2, System.Text.Encoding.UTF8, "application/json");
            HttpContent httpContent3 = new StringContent(
                content3, System.Text.Encoding.UTF8, "application/json");

            // Act
            var response1 = await _client.PostAsync("/ad", httpContent1);
            var response2 = await _client.PostAsync("/ad", httpContent2);
            var response3 = await _client.PostAsync("/ad", httpContent2);

            // Assert
            // Make sure Status Code is Bad Request
            Assert.Equal(response1.StatusCode, HttpStatusCode.BadRequest);
            Assert.Equal(response2.StatusCode, HttpStatusCode.BadRequest);
            Assert.Equal(response3.StatusCode, HttpStatusCode.BadRequest);

            // Check that an error message is returned
            var responseContent1 = JsonConvert.DeserializeObject(await response1.Content.ReadAsStringAsync()) as dynamic;
            var responseContent2 = JsonConvert.DeserializeObject(await response2.Content.ReadAsStringAsync()) as dynamic;
            var responseContent3 = JsonConvert.DeserializeObject(await response3.Content.ReadAsStringAsync()) as dynamic;
            string errorMessage = null;
            var exception = Record.Exception(() => errorMessage = responseContent1.UserId.ToString());
            Assert.Null(exception);
            Assert.False(string.IsNullOrEmpty(errorMessage));
            exception = Record.Exception(() => errorMessage = responseContent2.UserId.ToString());
            Assert.Null(exception);
            Assert.False(string.IsNullOrEmpty(errorMessage));
            exception = Record.Exception(() => errorMessage = responseContent3.UserId.ToString());
            Assert.Null(exception);
            Assert.False(string.IsNullOrEmpty(errorMessage));
        }    

        
        [Fact]
        public async void Test_Get_Ad_Returns_Correct_Data()
        {
            // Arrange
            var ad = GetSampleAdRequestObject() as dynamic;
            var content = JsonConvert.SerializeObject(ad);
            // Console.WriteLine("Content Sent: " + content);
            HttpContent httpContent = new StringContent(content, System.Text.Encoding.UTF8, "application/json");
            // Post the ad so that it's in the db
            var response = await _client.PostAsync("/ad", httpContent);
            var returnedAd = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync()) as dynamic;
            var id = returnedAd.id.ToString();

            // Act
            response = await _client.GetAsync("/ad/" + id);

            // Assert
            // Make sure Status Code is OK
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Check that returned information is the same as submitted information
            var responseString = await response.Content.ReadAsStringAsync();
            // Console.WriteLine("Content Received " + responseString);
            returnedAd = JsonConvert.DeserializeObject(responseString) as dynamic;
            AssertAreEqual(returnedAd, ad);

            // Make sure that additional fields are available in the response
            AssertIsValidAd(returnedAd);
        }

        [Fact]
        public async void Test_Get_All_Ads_Returns_Data()
        {
            // Act
            var response = await _client.GetAsync("/ad/");

            // Assert
            // Make sure Status Code is OK
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Check that returned information is the same as submitted information
            var responseString = await response.Content.ReadAsStringAsync();
            // Console.WriteLine("Content Received " + responseString);
            var returnedAds = JsonConvert.DeserializeObject(responseString) as dynamic;
            Console.WriteLine(responseString);

        }/*[Fact]
        public async void Test_Get_All_Ads_Returns_Ads()
        {
            // TODO: Delete all ads in db to check correct number of ads is returned
            // Arrange
            HttpResponseMessage response;
            for (int i = 0; i < 10; i++){
                var ad = GetSampleAdRequestObject() as dynamic;
                var content = JsonConvert.SerializeObject(ad);
                HttpContent httpContent = new StringContent(content, System.Text.Encoding.UTF8, "application/json");
                // Post the ad so that it's in the db
                response = await _client.PostAsync("/ad", httpContent);
            }
            
            // Act
            response = await _client.GetAsync("/ad/");

            // Assert
            // Make sure Status Code is OK
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var returnedAds = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync()) as dynamic;
            // Make sure multiple ads are returned

        }*/
        public void AssertAreEqual(dynamic ad1, dynamic ad2){
            Assert.Equal(ad2.Body, ad1.body.ToString());
            Assert.Equal((int)(ad2.PostDate.ToUniversalTime() - ((DateTime)ad1.postDate).ToUniversalTime()).TotalSeconds, 0);
            Assert.Equal(ad2.State, ad1.state.ToString());
            Assert.Equal(ad2.Title, ad1.title.ToString());
            Assert.Equal(ad2.UserId.ToString(), ad1.userId.ToString());
        }

        public static void AssertIsValidAd(dynamic ad){
            var exception = Record.Exception(() => DateTime.Parse(ad.createDate.ToString()));
            Assert.Null(exception);
            exception = Record.Exception(() => Guid.Parse(ad.id.ToString()));
            Assert.Null(exception);
            exception = Record.Exception(() => DateTime.Parse(ad.updateDate.ToString()));
            Assert.Null(exception);
        }
    }
}
