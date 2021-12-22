using System;
using System.Configuration;
using System.Net.Mail;

namespace ATA.EMMExemptions
{
    internal class ErrorEmailer
    {
        private static string _email = "errors@ailrines.org";
        private static string _errorEmail = "errors@ailrines.org";
        private static string _bccEmail = (string)null;
        private static string _errorEmailTitle = "Error on Airlines.org";
        private static string _errorEmailFromAddress = "errors@Fuels.airlines.org";
        private static string _EmailToAddress = "IT@airlines.org";
        private static string _EmailFromAddress = "errors@Fuels.airlines.org";
        private static string _errorEmailTitle0 = "Your exemption has expired!";
        private static string _errorEmailTitle30 = "Exemption will expire in 30 days!";
        private static string _errorEmailTitle60 = "Exemption will expire in 60 days!";
        private static string _errorEmailTitle90 = "Exemption will expire in 90 days!";
        private static string _errorEmailTitle120 = "Exemption will expire in 120 days!";
        private static string _errorEmailTitle150 = "Exemption will expire in 150 days!";
        private static string _sMTPServer = (string)null;
        private static bool initialized = false;

        internal static void SendErrorEmail(string error, OutputLog log)
        {
            try
            {
                ErrorEmailer.Initialize();
                MailMessage message = new MailMessage(new MailAddress(ErrorEmailer._errorEmailFromAddress), new MailAddress(ErrorEmailer._errorEmail));
                if (!string.IsNullOrEmpty(ErrorEmailer._bccEmail))
                    message.CC.Add(new MailAddress(ErrorEmailer._bccEmail));
                message.IsBodyHtml = false;
                message.Subject = ErrorEmailer._errorEmailTitle;
                message.Body = error;
                new SmtpClient(ErrorEmailer._sMTPServer).Send(message);
                message.Dispose();
            }
            catch (Exception ex)
            {
                log.LogError("Error in sending error email because " + ex.ToString());
            }
        }

        internal static void SendEmail(string Body, string Title, OutputLog log, int days)
        {
            try
            {
                ErrorEmailer.Initialize();
                MailMessage message = new MailMessage(new MailAddress(ErrorEmailer._errorEmailFromAddress), new MailAddress(ErrorEmailer._email));
                if (!string.IsNullOrEmpty(ErrorEmailer._bccEmail))
                    message.CC.Add(new MailAddress(ErrorEmailer._bccEmail));
                message.IsBodyHtml = false;
                message.To.Add(ErrorEmailer._EmailToAddress);
                switch (days)
                {
                    case 0:
                        message.Subject = ErrorEmailer._errorEmailTitle0;
                        break;
                    case 30:
                        message.Subject = ErrorEmailer._errorEmailTitle30;
                        break;
                    case 60:
                        message.Subject = ErrorEmailer._errorEmailTitle60;
                        break;
                    case 90:
                        message.Subject = ErrorEmailer._errorEmailTitle90;
                        break;
                    case 120:
                        message.Subject = ErrorEmailer._errorEmailTitle120;
                        break;
                    case 150:
                        message.Subject = ErrorEmailer._errorEmailTitle150;
                        break;
                    default:
                        message.Subject = Title;
                        break;
                }
                message.IsBodyHtml = true;
                message.Body = Body;
                SmtpClient smtpClient = new SmtpClient(ErrorEmailer._sMTPServer);
                log.LogError("Sending Email with Title:" + Title);
                smtpClient.Send(message);
                message.Dispose();
            }
            catch (Exception ex)
            {
                log.LogError("Error in sending error email because " + ex.ToString());
            }
        }

        private static void Initialize()
        {
            if (ErrorEmailer.initialized)
                return;
            ErrorEmailer._email = ErrorEmailer.RetrieveValueFromConf("EmailToAddress", ErrorEmailer._email);
            ErrorEmailer._errorEmail = ErrorEmailer.RetrieveValueFromConf("ReportLogErrorTo", ErrorEmailer._email);
            ErrorEmailer._bccEmail = ErrorEmailer.RetrieveValueFromConf("BccLogErrorTo", ErrorEmailer._bccEmail);
            ErrorEmailer._errorEmailTitle = ErrorEmailer.RetrieveValueFromConf("ErrorEmailTitle", ErrorEmailer._errorEmailTitle);
            ErrorEmailer._errorEmailTitle0 = ErrorEmailer.RetrieveValueFromConf("ErrorEmailTitle0", ErrorEmailer._errorEmailTitle0);
            ErrorEmailer._errorEmailTitle30 = ErrorEmailer.RetrieveValueFromConf("ErrorEmailTitle30", ErrorEmailer._errorEmailTitle30);
            ErrorEmailer._errorEmailTitle60 = ErrorEmailer.RetrieveValueFromConf("ErrorEmailTitle60", ErrorEmailer._errorEmailTitle60);
            ErrorEmailer._errorEmailTitle90 = ErrorEmailer.RetrieveValueFromConf("ErrorEmailTitle90", ErrorEmailer._errorEmailTitle90);
            ErrorEmailer._errorEmailTitle120 = ErrorEmailer.RetrieveValueFromConf("ErrorEmailTitle120", ErrorEmailer._errorEmailTitle120);
            ErrorEmailer._errorEmailTitle150 = ErrorEmailer.RetrieveValueFromConf("ErrorEmailTitle150", ErrorEmailer._errorEmailTitle150);
            ErrorEmailer._EmailToAddress = ErrorEmailer.RetrieveValueFromConf("EmailToList", ErrorEmailer._EmailToAddress);
            ErrorEmailer._errorEmailFromAddress = ErrorEmailer.RetrieveValueFromConf("ErrorEmailFromAddress", ErrorEmailer._errorEmailFromAddress);
            ErrorEmailer._sMTPServer = ErrorEmailer.RetrieveValueFromConf("SMTPServer", ErrorEmailer._sMTPServer);
            if (ErrorEmailer._sMTPServer == null)
                throw new InvalidOperationException("SMTPServer key is missing in AppSettings");
            ErrorEmailer.initialized = true;
        }

        private static string RetrieveValueFromConf(string key, string defaultValue)
        {
            return string.IsNullOrEmpty(ConfigurationManager.AppSettings[key]) ? defaultValue : ConfigurationManager.AppSettings[key];
        }
    }
}
