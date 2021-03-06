﻿using Microsoft.Extensions.Configuration;
using PostmarkDotNet;
using Xunit;

namespace PasswordPostgres.Web.UnitTests
{
    public class EmailTests
    {
        [Fact]
        public async void ShouldBeAbleToSendAnEmail()
        {
            var postmarkMessage = new PostmarkMessage
            {
                To = "devtest@hmsoftware.co.uk",
                From = "dave@hmsoftware.co.uk", // has to be a Sender Signature on postmark account
                //TrackOpens = true,
                Subject = "from tests",
                //Subject = $"A complex email {time}",
                TextBody = "hello world from the body",
                //TextBody = "Plain Text Body - hello world",
                //HtmlBody = "<html><body><img src=\"cid:embed_name.jpg\"/></body></html>",
                HtmlBody = "<html><body><p>Hello world from the body</p></body></html>",
                //Tag = "business-message",
                //Headers = new HeaderCollection{
                //    {"X-CUSTOM-HEADER", "Header content"}
                //}
            };
            //var imageContent = System.IO.File.ReadAllBytes("test.jpg");
            //message.AddAttachment(imageContent, "test.jpg", "image/jpg", "cid:embed_name.jpg");

            // this will load a dummy token which would fail if mal-formed email
            var postmarkServerToken = AppConfiguration.LoadFromEnvironment().PostmarkServerToken;

            var sendResult = await Email.Send(postmarkServerToken, postmarkMessage);

            Assert.Equal(PostmarkStatus.Success, sendResult.Status);
        }
    }
}
