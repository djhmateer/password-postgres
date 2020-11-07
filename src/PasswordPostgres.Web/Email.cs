using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using PostmarkDotNet;

namespace PasswordPostgres.Web
{
    public static class Email
    {
        public static async Task<PostmarkResponse?> Send(string postmarkServerToken, PostmarkMessage postmarkMessage)
        {
            if (string.IsNullOrWhiteSpace(postmarkServerToken)) throw new ArgumentNullException(nameof(postmarkServerToken));
            if (postmarkMessage == null) throw new ArgumentNullException(nameof(postmarkMessage));

            var client = new PostmarkClient(postmarkServerToken);
            var sendResult = await client.SendMessageAsync(postmarkMessage);

            return sendResult;

        }
    }
}
