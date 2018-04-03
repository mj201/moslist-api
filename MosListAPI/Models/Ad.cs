using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using System;
using System.ComponentModel.DataAnnotations;
using MosListAPI.Enum;
using MosListAPI.Attributes;

namespace MosListAPI.Models {
    [DynamoDBTable("Ads")]
    public class Ad{
        private DateTime createDate;
        private DateTime postDate;
        private DateTime updateDate;
        #region " Properties "
            [DynamoDBProperty]
            public string Body { get; set;}
            [DynamoDBProperty]
            public DateTime CreateDate { get{return createDate.ToUniversalTime();} private set{createDate = value;} }
            [DynamoDBHashKey]
            public string Id { get; private set; }
            [DynamoDBProperty]
            public DateTime PostDate { get{return postDate.ToUniversalTime();} set{postDate = value;} }
            [DynamoDBProperty]
            public State State { get; set; }
            [DynamoDBProperty]
            public string Title { get; set;}
            [DynamoDBProperty]
            public DateTime UpdateDate { get{return updateDate.ToUniversalTime();} private set{updateDate = value;} }
            [DynamoDBProperty]
            public string UserId { get; set; }
        #endregion

        #region " Constructors "
        public Ad(){
            Initialize();
        }
        public Ad(AdPostRequest adPostRequest){
            Initialize();
            Body = adPostRequest.Body;
            PostDate = adPostRequest.PostDate;
            State = adPostRequest.State;
            Title = adPostRequest.Title;
            UserId = adPostRequest.UserId;
        }
        #endregion

        #region " Methods "
        private void Initialize(){
            CreateDate = DateTime.UtcNow;
            Id = Guid.NewGuid().ToString().ToUpper();
            UpdateDate = DateTime.UtcNow;
        }
        #endregion
    }
    public class AdPostRequest {
        #region " Properties "
        [Required]
        public string Body { get; set;}
        public DateTime PostDate { get; set; }
        [Required]
        [EnumDataType(typeof(State))]
        public State State { get; set; }
        [Required]
        public string Title { get; set;}
        [Required]
        [NonDefault(typeof(Guid))]
        public string UserId { get; set; }
        #endregion
    }

    public class AdPostResponse {
        #region " Properties "
            public string Body { get; private set;}
            public DateTime CreateDate { get; private set; }
            public string Id { get; private set; }
            public DateTime PostDate { get; private set; }
            public State? State { get; private set; }
            public string Title { get; private set;}
            public DateTime UpdateDate { get; private set; }
            public string UserId { get; private set; }
        #endregion

        #region " Constructors "
            public AdPostResponse(Ad ad){
                Body = ad.Body;
                CreateDate = ad.CreateDate;
                Id = ad.Id;
                PostDate = ad.PostDate;
                State = ad.State;
                Title = ad.Title;
                UpdateDate = ad.UpdateDate;
                UserId = ad.UserId;
            }
        #endregion 
    }
}