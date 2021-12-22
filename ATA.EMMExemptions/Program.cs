using Microsoft.SharePoint.Client;
using System;
using System.Configuration;
using System.Net;

namespace ATA.EMMExemptions
{
    class Program
    {
        private static void Main(string[] args)
        {
            using (OutputLog log = new OutputLog())
            {
                log.LogInfo(string.Format("Beginning to Check EMM list for exemptions", (object)DateTime.Now.ToString()));
                Program.readExemptionsList(log);
            }
        }

        private static void readExemptionsList(OutputLog log)
        {
            string appSetting1 = ConfigurationManager.AppSettings["ATAEMMExemptionSiteURL"];
            string appSetting2 = ConfigurationManager.AppSettings["ATAEMMExemptionListName"];
            string Username = ConfigurationManager.AppSettings["ATAMembersSiteUrlUser"];
            string Password = ConfigurationManager.AppSettings["ATAMembersSiteUrlPassword"];
            int num1 = 0;
            try
            {
                using (ClientContext clientContext = new ClientContext(appSetting1))
                {
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;

                    clientContext.Credentials = new NetworkCredential(Username, Password);
                    clientContext.ExecutingWebRequest += new EventHandler<WebRequestEventArgs>(clientContext_ExecutingWebRequest);
                    List spList = clientContext.Web.Lists.GetByTitle(appSetting2);
                    var query = new CamlQuery()
                    {
                        ViewXml = "<OrderBy><FieldRef Name='Title' Ascending='FALSE' /></OrderBy><FieldRef Name='ID'/></IsNotNull></Where></Query></View>"
                    };
                    ListItemCollection results = spList.GetItems(query);
                    clientContext.Load(results);
                    clientContext.ExecuteQuery();

                    foreach (var item in results)
                    {
                        DateTime result1;
                        DateTime.TryParse(Program.EnsureTextValue(item, "Expiration_x0020_Date0", log), out result1);
                        string str1 = Program.EnsureTextValue(item, "Archive_x0020_Status", log);
                        string str2 = Program.EnsureTextValue(item, "Document_x0020_Type", log);
                        string str3 = Program.EnsureTextValue(item, "Title", log);
                        string str4 = Program.EnsureTextValue(item, "Description0", log);
                        log.LogInfo("Item - " + (object)num1 + ".  " + str3);
                        num1 = 1;
                        int num2;
                        if (!(result1 >= DateTime.Now))
                        {
                            int day1 = result1.Day;
                            DateTime now = DateTime.Now;
                            int day2 = now.Day;
                            if (day1 == day2)
                            {
                                int month1 = result1.Month;
                                now = DateTime.Now;
                                int month2 = now.Month;
                                if (month1 == month2)
                                {
                                    int year1 = result1.Year;
                                    now = DateTime.Now;
                                    int year2 = now.Year;
                                    num2 = year1 != year2 ? 1 : 0;
                                    goto label_11;
                                }
                            }
                            num2 = 1;
                        }
                        else
                            num2 = 0;
                        label_11:
                        if (num2 == 0)
                        {
                            TimeSpan timeSpan = result1 - DateTime.Now;
                            int result2;
                            int.TryParse(timeSpan.TotalDays.ToString(), out result2);
                            num1 = 2;
                            result2 = (int)result1.Subtract(DateTime.Now).TotalDays;
                            num1 = 3;
                            OutputLog outputLog = log;
                            string str5 = str2.Trim().ToLower().Contains("exemption").ToString();
                            object[] objArray1 = new object[10];
                            objArray1[0] = (object)"totaldays";
                            object[] objArray2 = objArray1;
                            timeSpan = result1 - DateTime.Now;
                            string str6 = timeSpan.TotalDays.ToString();
                            objArray2[1] = (object)str6;
                            objArray1[2] = (object)" Days: ";
                            objArray1[3] = (object)result2;
                            objArray1[4] = (object)"Archive Status: ";
                            objArray1[5] = (object)Program.EnsureTextValue(item, "Archive_x0020_Status", log);
                            objArray1[6] = (object)"Document_x0020_Type: ";
                            objArray1[7] = (object)Program.EnsureTextValue(item, "Document_x0020_Type", log);
                            objArray1[8] = (object)"Expiration Date :";
                            objArray1[9] = (object)Program.EnsureTextValue(item, "Expiration_x0020_Date0", log);
                            string str7 = string.Format(string.Concat(objArray1));
                            string info = "DocumentType.Contains('Exemption')" + str5 + str7;
                            outputLog.LogInfo(info);
                            string Body = "This is an automatically generated notice the Exemption " + str3 + " - " + str4 + " Expires on " + (object)result1 + "<br /><br /><a href=\"https://portal.airlines.org/os/emmc/Regulatory Exemptions/Forms/Current.aspx\">Link to Member Portal Regulatory Exemptions</a>";
                            num1 = 4;
                            if (str1 == "No" && str2.Contains("Exemption"))
                            {
                                num1 = 5;
                                switch (result2)
                                {
                                    case 0:
                                        ErrorEmailer.SendEmail(Body, "Exemption expired" + str3, log, 0);
                                        log.LogInfo("In switch for Totaldays 0");
                                        break;
                                    case 30:
                                        ErrorEmailer.SendEmail(Body, "Exemption will expire in 30 days" + str3, log, 30);
                                        break;
                                    case 60:
                                        ErrorEmailer.SendEmail(Body, "Exemption will expire in 60 days" + str3, log, 60);
                                        break;
                                    case 90:
                                        ErrorEmailer.SendEmail(Body, "Exemption will expire in 90 days" + str3, log, 90);
                                        break;
                                    case 120:
                                        ErrorEmailer.SendEmail(Body, "Exemption will expire in 120 days" + str3, log, 120);
                                        break;
                                    case 150:
                                        ErrorEmailer.SendEmail(Body, "Exemption will expire in 150 days" + str3, log, 150);
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogInfo("");
                log.LogInfo("Error @ count " + (object)num1 + ": " + ex.Message);
            }
        }
        private static void clientContext_ExecutingWebRequest(object sender, WebRequestEventArgs e)
        {
            try
            {
                e.WebRequestExecutor.WebRequest.Headers.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f");
            }
            catch
            { throw; }
        }

        private static string EnsureTextValue(ListItem item, string param, OutputLog log)
        {
            string empty = string.Empty;
            try
            {
                if(item.FieldValues.TryGetValue(param,out object value))
                {
                    if (value != null)
                    {
                        empty = value.ToString();
                    }
                }
                else
                {
                    log.LogInfo("");
                    log.LogInfo("List Key" + param + "not found");
                }

            }
            catch (Exception ex)
            {
                log.LogInfo("");
                log.LogInfo("Error in : EnsureTextValue for Param:" + param + ex.Message);
            }
            return empty;
        }
    }
}
