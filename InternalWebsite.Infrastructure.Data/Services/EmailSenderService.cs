using InternalWebsite.Application.Utils;
using InternalWebsite.Infrastructure.Data.Context;
using InternalWebsite.Infrastructure.Data.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using InternalWebsite.Core.Entities;
using InternalWebsite.ViewModel.Models;
using Dapper;
using InternalWebsite.ViewModel.DTOs;
using Microsoft.AspNetCore.Components.Forms;
using Org.BouncyCastle.Utilities;
using DevExpress.Compatibility.System.Windows.Forms;
using System.Drawing;
using System.Text.RegularExpressions;

namespace InternalWebsite.Infrastructure.Data.Services
{
    public class EmailSenderService : ResponseHelper, IEmailSenderService
    {
        //private readonly IEmailSenderService _userRepository;
        private readonly IConfiguration _configuration;
        private readonly ClCongDbContext _context;

        public EmailSenderService(IConfiguration configuration,
            ClCongDbContext context) : base(configuration)
        {
            _configuration = configuration;
            _context = context;
        }

        private SqlConnection SqlConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        }

        private IDbConnection CreateConnection()
        {
            var conn = SqlConnection();
            conn.Open();
            return conn;
        }

        public bool SendEmailAsync(string email, string subject, string message)
        {
            return this.SendEmailUsingSMTP(email, subject, message);
        }
        //public static bool SendEmailAsyncId(string email, string subject, string message)
        //{
        //    return this.SendEmailUsingSMTP(email, subject, message);
        //}

        public bool SendEmailByTemplate(string template, string to)
        {
            return this.SendEmailUsingSmtpTemplate(template, to);
        }

        public bool SendSingUpGroupEmail(EmailSendDto emailSendDto)
        {
            string to = emailSendDto.Email; string subject = emailSendDto.EmailTitle; string userName = emailSendDto.UserName;
            string template = emailSendDto.EmailType; string url = emailSendDto.CallBackUrl; string pwd = emailSendDto.OTP;
            string firstName = emailSendDto.FirstName;
            string lastName = emailSendDto.LastName;

            //EmailTemplate templateData;
            //using (var db = _context)
            //{
            //    templateData =
            //        db.EmailTemplates
            //            .FirstOrDefault(c => c.Name.ToLower() == template.ToLower()
            //                                 && c.IsActive == true);
            //}

            if (template?.ToLower() == "singup")
            {
                var body = "";
                if (string.IsNullOrEmpty(body))

                    #region body

                    body =
                        $"<html xmlns='http://www.w3.org/1999/xhtml'> <head> <meta http-equiv='Content-Type' content='text/html; charset=utf-8' /> <title>Hi! {userName} Welcome Email from Balke</title> </head> <body> <table width='100%' border='0' cellspacing='0' cellpadding='0'> <tr> <td align='center' valign='top' bgcolor='#ffe77b' style='background-color:#ffe77b;'> <br> <br> <table width='600' border='0' cellspacing='0' cellpadding='0'> <tr> <td height='70' align='left' valign='middle'></td> </tr> <tr> <td align='left' valign='top'><img src='http://localhost:2131/Templates/EmailTemplate/images/top.png' width='600' height='13' style='display:block;'></td> </tr> <tr> <td align='left' valign='top' bgcolor='#ffffff' style='background-color:#ffffff;'> <table width='100%' border='0' cellspacing='0' cellpadding='0'> <tr> <td align='center' valign='middle' style='padding:10px; color:#564319; font-size:28px; font-family:Georgia, 'Times New Roman', Times, serif;'> Welcom to Balke App! <small>.</small> </td> </tr> <tr> <td align='center' valign='middle' style='padding:10px; color:#564319; font-size:28px; font-family:Georgia, 'Times New Roman', Times, serif;'> Please click here to Confirm your account Confimation link your <br><a href='{url}'> clicking here</a>.! <small>.</small> </td> </tr> </table> <table width='95%' border='0' align='center' cellpadding='0' cellspacing='0'> <tr> <td align='left' valign='middle' style='color:#525252; font-family:Arial, Helvetica, sans-serif; padding:10px;'> <div style='font-size:16px;'> </div> <div style='font-size:12px;'> Thank you for showing your interest in our website. All you need to do is click the button below (it only takes a few seconds). You won’t be asked to log in to your account – we're simply verifying ownership of this email address. <hr> </div> </td> </tr> </table> </td> </tr> </table> <br> <br> </td> </tr> </table> </body> </html>";

                #endregion

                return EmailSingUpGroupData(to, subject, body);
            }

            if (template?.ToLower() == "reset")
            {
                var body = "";
                if (string.IsNullOrEmpty(body))

                    #region b

                    body =
                        $"<!DOCTYPE html> <html lang='en'> <head> <meta charset='UTF-8'> <meta http-equiv='X-UA-Compatible' content='IE=edge'> <meta name='viewport' content='width=device-width, initial-scale=1.0'> <title>Reset Your Balke Tech Password</title> <style> body {{ font-family: system-ui, -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, 'Open Sans', 'Helvetica Neue', sans-serif; margin: 0; padding: 0; background-color: #f7f7f7; /* Background color for the whole email */ }} #mailSection{{ display: flex; align-items: center; height: 100vh; }} .container {{ max-width: 500px; margin: 0 auto; padding: 20px; border: 1px solid #ccc; border-radius: 5px; background-color: #ffffff; /* Background color for the white box inside the container */ }} .header {{ text-align: center; padding: 3px 0; background-color: #2E64DB; /* Background color behind the Balke Tech logo */ border-top-left-radius: 5px; border-top-right-radius: 5px; }} .header img {{ width: 150px; height: auto; background-color: #2E64DB; /* Background color behind the Balke Tech logo */ padding: 20px; border-radius: 5px; }} .title {{ font-weight: 630; /* Semi-bold */ color: #2E64DB; position: relative; }} .line {{ width: 80%; border-top: 1px solid #ccc; padding-bottom: 6px; margin: 0 auto; /* Center the line horizontally */ }} .image {{ text-align: center; }} .image img {{ width: 60px; height: auto; padding: 10px; }} .button {{ display: block; width: 200px; margin: 20px auto; padding: 10px; background-color: #2E64DB; color: white !important; text-align: center; text-decoration: none; border: none; border-radius: 5px; cursor: pointer; }} .button:hover {{ background-color: #1c4a9f; color: white !important; }} .text {{ margin: 10px 20px; }} .blue-text {{ color: #1c4a9f; text-decoration: underline; }} @media (max-width: 600px) {{ #mailSection{{ height: auto; }} .container {{ padding: 0; border: none; }} .header {{ border-top-left-radius: 0px; border-top-right-radius: 0px; }} }} </style> </head> <body> <div id='mailSection'> <div class='container'> <div class='header'> <img src='https://api.balke.pro/Resources/file/WhiteBalkeLogo.png' alt='Balke Tech Logo'> </div> <h2 class='title' style='text-align: center;'>Reset Your Balke Tech Password</h2> <div class='line'></div> <p class='text'>Hi {firstName} {lastName},</p> <div class='image'> <img src='https://api.balke.pro/Resources/file/ResetPassword.png' alt='Balke Tech Logo'> </div> <p class='text'>It looks like you requested to reset your password. No worries, we’ve got you covered!</p> <p class='text'>To reset your password, please click the button below:</p> <a class='button' href='{url}'>Reset Password</a> <p class='text'>If the button above doesn’t work, copy and paste the following link into your browser:</p> <!-- <p class='text'>[Reset Link]</p> --> <a class='text blue-text' href='{url}' target='_blank'>{url}</a> <p class='text'>For your security, this link will expire in 24 hours. If you didn’t request a password reset, please ignore this email or contact our support team at <a href='mailto:support@balke.tech' class='title'>support@balke.tech</a></p> <p class='text'>Thank you for using Balke Tech!</p> <p class='text'>Best regards,</p> <p class='text title'>The Balke Tech Team</p> </div> </div> </body> </html>";

                #endregion

                return EmailSingUpGroupData(to, subject, body);
            }
            if (template?.ToLower() == "adminreset")
            {
                var body = "";
                if (string.IsNullOrEmpty(body))

                    #region b

                    body =
                        $"<!DOCTYPE html> <html> <head> <title>Admin Email Template</title> <style> main {{width: 700px; }} .header {{background: #2E64DB; height: 160px; width: 100%; display: flex; justify-content: center; align-items: center; }} .header img {{height: 32px; }} .content {{padding: 24px; background: white; border: 1px solid #2E64DB; }} p, a {{font - family: Roboto; font-size: 16px; font-weight: 400; line-height: 19.2px; letter-spacing: 0.0075em; text-align: left; color: black; }} .mb-24 {{margin - bottom: 24px; }} a {{color: #0086C9; }} .note {{font - style: italic; }} </style> </head> <body> <main> <div class='header'> <img src='https://i.ibb.co/59Lt8QF/1.png' alt='' /> </div> <div class='content'> <p class='mb-24'>Hello [Admin's Name],</p> <p class='mb-24'> We received a request to reset your password. If you made this request, please click the link below to reset your password:</p> <a class='mb-24'  target='_blank'>{url}</a> <p class='mb-24'><span class='note'>Note:</span> This link will be active for the next 24 hours.</p> <p class='mb-24'>If you did not request a password reset, please ignore this email. <br /> Your password will remain unchanged. </p> <p> <span class='regards'> Best regards,</span> <br /> Balke Tech Team</p> </div> </main> </body> </html>";

                #endregion

                return EmailSingUpGroupData(to, subject, body);
            }
            if (template?.ToLower() == "signin")
            {
                var body = "";
                if (string.IsNullOrEmpty(body))

                    #region b

                    body =
                        $"<html xmlns='http://www.w3.org/1999/xhtml'> <head> <title>Welcome Email from Balke</title> </head> <body> <table width='100%' border='0' cellspacing='0' cellpadding='0'> <tr> <td align='center' valign='top' bgcolor='#ffe77b' style='background-color:#ffe77b;'> <br> <br> <table width='600' border='0' cellspacing='0' cellpadding='0'> <tr> <td height='70' align='left' valign='middle'></td> </tr> <tr> <td align='left' valign='top'><img src='http://localhost:2131/Templates/EmailTemplate/images/top.png' width='600' height='13' style='display:block;'></td> </tr> <tr> <td align='left' valign='top' bgcolor='#564319' style='background-color:#564319; font-family:Arial, Helvetica, sans-serif; padding:10px;'> <div style='font-size:36px; color:#ffffff;'> <b>{userName}</b> </div> <div style='font-size:13px; color:#a29881;'> <b> Hi ! {userName} : Balke App</b> </div> </td> </tr> <tr> <td align='left' valign='top' bgcolor='#ffffff' style='background-color:#ffffff;'> <table width='100%' border='0' cellspacing='0' cellpadding='0'> <tr> <td align='center' valign='middle' style='padding:10px; color:#564319; font-size:28px; font-family:Georgia, 'Times New Roman', Times, serif;'> Welcom to Balke App! <b> your temporary passowrd is : {pwd}</b> </td> </tr> </table> <table width='95%' border='0' align='center' cellpadding='0' cellspacing='0'> <tr> <td align='left' valign='middle' style='color:#525252; font-family:Arial, Helvetica, sans-serif; padding:10px;'> <div style='font-size:16px;'> Dear {userName}, </div> <div style='font-size:12px;'> Thank you for showing your interest  in  our website.> clicking here</a>.. You won’t be asked to log in to your account – we're simply verifying ownership of this email address. <hr> </div> </td> </tr> </table> </td> </tr> </table> <br> <br> </td> </tr> </table> </body> </html>";

                #endregion

                var a = EmailSingUpGroupData(to, subject, body);
                return true;
            }
            if (template?.ToLower() == "emailconfirm")
            {
                var body = "";
                if (string.IsNullOrEmpty(body))


                    body = $"<!DOCTYPE html> <html lang='en'> <head> <meta charset='UTF-8'> <meta http-equiv='X-UA-Compatible' content='IE=edge'> <meta name='viewport' content='width=device-width, initial-scale=1.0'> <title>Reset Your Balke Tech Password</title> <style> body {{ font-family: system-ui, -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, 'Open Sans', 'Helvetica Neue', sans-serif; margin: 0; padding: 0; background-color: #f7f7f7; /* Background color for the whole email */ }} #mailSection{{ display: flex; align-items: center; height: 100vh; }} .container {{ max-width: 500px; margin: 0 auto; padding: 20px; border: 1px solid #ccc; border-radius: 5px; background-color: #ffffff; /* Background color for the white box inside the container */ }} .header {{ text-align: center; padding: 3px 0; background-color: #2E64DB; /* Background color behind the Balke Tech logo */ border-top-left-radius: 5px; border-top-right-radius: 5px; }} .header img {{ width: 150px; height: auto; padding: 20px; border-radius: 5px; }} .title {{ font-weight: 630; /* Semi-bold */ color: #2E64DB; position: relative; }} .line {{ width: 80%; border-top: 1px solid #ccc; padding-bottom: 6px; margin: 0 auto; /* Center the line horizontally */ }} .button {{ display: block; width: 200px; margin: 20px auto; padding: 10px; background-color: #2E64DB; color: white !important; text-align: center; text-decoration: none; border: none; border-radius: 5px; cursor: pointer; }} .button:hover {{ background-color: #1c4a9f !important; color: white !important; }} .text {{ margin: 10px 20px; }} .blue-text {{ color: #1c4a9f; text-decoration: underline; }} @media (max-width: 600px) {{ #mailSection{{ height: auto; }} .container {{ padding: 0; border: none; }} .header {{ border-top-left-radius: 0px; border-top-right-radius: 0px; }} }} </style> </head> <body> <div id='mailSection'> <div class='container'> <div class='header'> <img src='https://api.balke.pro/Resources/file/WhiteBalkeLogo.png' alt='Balke Tech Logo'> </div> <h2 class='title' style='text-align: center;'>Verify Your Email for Balke Tech</h2> <div class='line'></div> <p class='text'>Hi {firstName} {lastName},</p> <p class='text'>Welcome to Balke Tech!</p> <p class='text'>Thank you for signing up. To get started with launching your ad campaigns, please verify your email address by clicking the button below:</p> <a class='button' href='{url}'>Verify Email</a> <p class='text'>If the button above doesn’t work, copy and paste the following link into your browser:</p> <!-- <p class='text'>[Reset Link]</p> --> <a class='text blue-text' href='{url}' target='_blank'>verification.link</a> <p class='text'>If you have any questions or need assistance, feel free to contact our support team at <a href='mailto:support@balke.tech' class='title'>support@balke.tech</a></p> <p class='text'>Thank you for choosing Balke Tech. We're excited to help you launch successful ad campaigns!</p> <p class='text'>Best regards,</p> <p class='text title'>The Balke Tech Team</p> </div> </div> </body> </html>";


                var a = EmailSingUpGroupData(to, subject, body);
                return true;
            }
            if (template?.ToLower() == "changeemail")
            {
                var body = "";
                if (string.IsNullOrEmpty(body))

                    #region b

                    body =
                        $"<!DOCTYPE html> <html lang='en'> <head> <meta charset='UTF-8'> <meta name='viewport' content='width=device-width, initial-scale=1.0'> <title>Your OTP Code</title> </head> <body style='font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0;'> <div style='width: 100%; padding: 20px; background-color: #f4f4f4; display: flex; justify-content: center;'> <div style='background-color: #ffffff; padding: 20px; max-width: 600px; width: 100%; border-radius: 8px; box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);'> <div style='font-size: 24px; color: #333333; margin-bottom: 20px; text-align: center;'> Your OTP Code </div> <div style='font-size: 16px; color: #555555; line-height: 1.5; text-align: center;'> Hi <b> {firstName} {lastName}</b>, <br><br> Use the following One-Time Password (OTP) to complete your transaction: <br><br> <div style='font-size: 32px; font-weight: bold; color: #3498db; margin: 20px 0;'> {pwd} </div> <br> Please do not share this code with anyone. It is valid for the next 10 minutes. <br><br> If you didn’t request this OTP, please ignore this email or contact our support team at <a href='mailto:support@balke.tech' style='color: #3498db; text-decoration: none;'>support@balke.tech</a>. <br><br> Thank you for using Balke Tech! </div> <div style='font-size: 14px; color: #999999; margin-top: 20px; text-align: center;'> Best regards,<br> The Balke Tech Team </div> </div> </div> </body> </html>";

                #endregion

                var a = EmailSingUpGroupData(to, subject, body);
                return true;
            }
            if (template?.ToLower() == "passwordset")
            {
                var body = "";
                if (string.IsNullOrEmpty(body))

                    #region b

                    body =
                        $"<!DOCTYPE html> <html lang='en'> <head> <meta charset='UTF-8'> <meta http-equiv='X-UA-Compatible' content='IE=edge'> <meta name='viewport' content='width=device-width, initial-scale=1.0'> <title>Reset Your Balke Tech Password</title> <style> body {{ font-family: system-ui, -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, 'Open Sans', 'Helvetica Neue', sans-serif; margin: 0; padding: 0; background-color: #f7f7f7; /* Background color for the whole email */ }} #mailSection{{ display: flex; align-items: center; height: 100vh; }} .container {{ max-width: 500px; margin: 0 auto; padding: 20px; border: 1px solid #ccc; border-radius: 5px; background-color: #ffffff; /* Background color for the white box inside the container */ }} .header {{ text-align: center; padding: 3px 0; background-color: #2E64DB; /* Background color behind the Balke Tech logo */ border-top-left-radius: 5px; border-top-right-radius: 5px; }} .header img {{ width: 150px; height: auto; padding: 20px; border-radius: 5px; }} .title {{ font-weight: 630; /* Semi-bold */ color: #2E64DB; position: relative; }} .line {{ width: 80%; border-top: 1px solid #ccc; padding-bottom: 6px; margin: 0 auto; /* Center the line horizontally */ }} .button {{ display: block; width: 200px; margin: 20px auto; padding: 10px; background-color: #2E64DB; color: white !important; text-align: center; text-decoration: none; border: none; border-radius: 5px; cursor: pointer; }} .button:hover {{ background-color: #1c4a9f !important; color: white !important; }} .text {{ margin: 10px 20px; }} .blue-text {{ color: #1c4a9f; text-decoration: underline; }} @media (max-width: 600px) {{ #mailSection{{ height: auto; }} .container {{ padding: 0; border: none; }} .header {{ border-top-left-radius: 0px; border-top-right-radius: 0px; }} }} </style> </head> <body> <div id='mailSection'> <div class='container'> <div class='header'> <img src='https://api.balke.pro/Resources/file/WhiteBalkeLogo.png' alt='Balke Tech Logo'> </div> <h2 class='title' style='text-align: center;'>Set Your Password for Balke Tech</h2> <div class='line'></div> <p class='text'>Hi {firstName} {lastName},</p> <p class='text'>Welcome to Balke Tech!</p> <p class='text'>Thank you for signing up. To get started with launching your ad campaigns, please set your password by clicking the button below:</p> <a class='button' href='{url}'>Set Password</a> <p class='text'>If the button above doesn’t work, copy and paste the following link into your browser:</p> <!-- <p class='text'>[Reset Link]</p> --> <a class='text blue-text' href='{url}' target='_blank'>verification.link</a> <p class='text'>If you have any questions or need assistance, feel free to contact our support team at <a href='mailto:support@balke.tech' class='title'>support@balke.tech</a></p> <p class='text'>Thank you for choosing Balke Tech. We're excited to help you launch successful ad campaigns!</p> <p class='text'>Best regards,</p> <p class='text title'>The Balke Tech Team</p> </div> </div> </body> </html>";

                #endregion

                var a = EmailSingUpGroupData(to, subject, body);
                return true;
            }


            return true;
        }
        public bool MarketingEmailSend(EmailSendDto emailSendDto, Marketing marketing)
        {
            string to = emailSendDto.Email; string subject = emailSendDto.EmailTitle; string userName = emailSendDto.UserName;
            string template = emailSendDto.EmailType; string url = emailSendDto.CallBackUrl; string pwd = emailSendDto.OTP;
            string firstName = emailSendDto.FirstName;
            string lastName = emailSendDto.LastName;

            if (template?.ToLower() == "marketing")
            {
                var body = "";
                if (string.IsNullOrEmpty(body))
                {
                    var items = Regex.Split(marketing.Solution, @",(?!\s)")
                                 .Select(item => item.Trim()) // Remove leading/trailing whitespace
                                 .ToArray();
                    #region b
                    body =
                    $"<!DOCTYPE html> <html lang=\"en\"> <head> <meta charset=\"UTF-8\"> <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\"> <title>Document</title> <style> body {{ font-family: system-ui, -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, 'Open Sans', 'Helvetica Neue', sans-serif;}} .container-content{{background-color:#fff;max-width:500px;width:100%;border:1px solid #2e64db;border-radius:10px;overflow:hidden;margin:0 auto;box-shadow:0 4px 6px rgba(0,0,0,.1)}}.header{{background-color:#2e64db;text-align:center;padding:20px}}.logo{{display:block;margin:0 auto;height:60px}}.content{{padding:20px}}.content p{{font-family:Outfit;font-size:14px;font-weight:600;line-height:22px;letter-spacing:.025em;text-align:left;margin-bottom:12px;color:#2e64db}}.content p span{{color:#000}}.content h2{{font-family:Outfit;font-size:20px;font-weight:600;line-height:28px;letter-spacing:.001em;text-align:center;margin-bottom:12px;text-decoration:underline}}h1{{color:#2e64db;margin-bottom:12px;font-family:Outfit;font-size:24px;font-weight:600;line-height:32px;letter-spacing:.001em;text-align:left}}.authorized-user{{color:#000;margin-left:10px}}.section h3{{color:#2e64db;margin-bottom:12px;font-family:Outfit;font-size:16px;font-weight:600;line-height:25px;text-align:left}}.section p{{font-family:Outfit !important;font-size:14px !important;font-weight:400 !important;line-height:22px !important;letter-spacing:.025em !important;text-align:left !important;color:#000 !important}}.section ul{{padding-left:20px;list-style-type:decimal}}.section ul li{{font-family:Outfit;font-size:14px;font-weight:400;line-height:22px;letter-spacing:.025em;text-align:left;margin-bottom:12px}}.footer-text{{font-weight:bold !important;color:#000 !important;margin-top:35px !important;font-family:Outfit;font-size:12px !important;font-weight:400 !important;line-height:16px !important;letter-spacing:.025em !important;text-align:left !important}}.footer-text span{{color:#2e64db !important;font-weight:600 !important}} </style> </head> <body style='font-family: Arial, sans-serif;'> <div class=\"container-content\"> <!-- Header Section --> <div class=\"header\"> <!-- <img src=\"https://api.balke.pro/Resources/file/WhiteBalkeLogo.png\" alt=\"Logo\" class=\"logo\"> --> <img src='https://api.balke.pro/Resources/file/WhiteBalkeLogo.png' alt=\"Logo\" class=\"logo\"> </div> <!-- Content Section --> <div class='content'> <h1>Marketing Service Request</h1> <p>Received From: <span class='authorized-user'>{to}</span></p> <h2>Marketing Service Request Summary</h2> <div class='section'> <h3>Business Details:</h3> <p><strong>Business Name: </strong>{marketing.BusinessName}</p> <p><strong>Owner Name: </strong>{marketing.OwnerName}</p> <p><strong>Phone Number: </strong>{marketing.PhoneNumber}</p> </div> <div class='section'> <h3>Project Details:</h3> <p><strong>Objectives:</strong></p> <p>“{marketing.Objective}”</p> <p><strong>Project Summary:</strong></p> <p>“{marketing.Summary}”</p> <p><strong>Duration: </strong>{marketing.Duration}</p> <p><strong>Project Estimated Budget: </strong>{marketing.Budget} USD</p> <p><strong>Solution:</strong></p> <ul>";

                    #endregion
                    // Loop through each item in the array and create a list item for it
                    foreach (string item in items)
                    {
                        body += $"<li>{item}</li>";
                    }

                    // Close the <ul> tag
                    body += $"</ul> </div> <div class='section'> <p><strong>Comments:</strong></p> <p>{marketing.Details}</p> </div> <p class='footer-text'>Best regards,<br> <span >The Balke Tech Team</span></p> </div> </div> </body> </html>";
                }
                var a = EmailInfoGroupData(to,subject, body);
                return true;
            }
            return true;
        }
        private bool SendEmailUsingSmtpTemplate(string template, string to)
        {
            try
            {
                LookUpValueDto lookupData;
                EmailTemplate templateData;
                var emailListData = new List<LookUpValueDto>();
                List<TemplateField> templateFieldData;
                using (var connection = CreateConnection())
                {
                    const string sqlQuery = "Get_LookUp_Value";
                    var sqlParam = new
                    {
                        category = template
                    };
                    lookupData = connection.Query<LookUpValueDto>(sqlQuery, sqlParam,
                    commandType: CommandType.StoredProcedure, buffered: true, commandTimeout: 3600).Distinct()
                    .FirstOrDefault();
                }
                if (!string.IsNullOrEmpty(to))
                {
                    lookupData.To = to;
                    using (var db = new ClCongDbContext())
                    {
                        templateData = db.EmailTemplates.FirstOrDefault(c =>
                        c.Name.ToLower() == template.ToLower() && c.IsActive == true);
                        templateFieldData =
                        db.TemplateFields.Where(c => c.EmailTemplateId == templateData.EmailTemplateId).ToList();
                        //templateFeildData = db.TemplateFeild.Include(x => x.EmailTemplate).Where(x => x.EmailTemplate.Name == "groupalert").ToList();
                        emailListData = GetUserInfo(to, "user");
                    }
                    return SendEmail(lookupData, emailListData, templateData, templateFieldData);
                }
                else
                {
                    using (var db = new ClCongDbContext())
                    {
                        templateData = db.EmailTemplates.FirstOrDefault(c => c.Name == template);
                        templateFieldData =
                        db.TemplateFields.Where(c => c.EmailTemplateId == templateData.EmailTemplateId).ToList();
                    }
                    if (lookupData.To.ToLower().Contains("group"))
                        emailListData = GroupAndUser(lookupData);
                    else if (lookupData.To.ToLower().Contains("user"))
                        emailListData = GroupAndUser(lookupData);
                    return SendEmail(lookupData, emailListData, templateData, templateFieldData);
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        private bool SendSingUpGroupEmailData(string to, string subject, string body)
        {
            try
            {
                return EmailSingUpGroupData(to, subject, body);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        private List<LookUpValueDto> GroupAndUser(LookUpValueDto lookupData)
        {
            string[] splitted = lookupData.To.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            var name = splitted[1];
            var contractEmails = new List<LookUpValueDto>();
            contractEmails = GetGroupContactEmail(name, splitted[0]);
            //lookupData.To = contractEmails.EmailTo;
            return contractEmails;
        }
        private List<LookUpValueDto> GetUserInfo(string id, string type)
        {
            var contractEmails = new List<LookUpValueDto>();
            contractEmails = GetGroupContactEmail(id, type);
            //lookupData.To = contractEmails.EmailTo;
            return contractEmails;
        }
        private List<LookUpValueDto> GetGroupContactEmail(string name, string type)
        {
            List<LookUpValueDto> LookupData;
            using (var connection = CreateConnection())
            {
                var sqlQuery = type.ToLower() == "group" ? "Get_Contract_Emails_By_GroupName" : "Get_Emails_By_User";
                var sqlParam = new
                {
                    name = name
                };
                LookupData = connection.Query<LookUpValueDto>(sqlQuery, sqlParam, commandType: CommandType.StoredProcedure,
                buffered: true, commandTimeout: 3600).Distinct().ToList();
            }
            return LookupData;
        }
        private bool SendPasswordEmail(LookUpValueDto lookUp, List<LookUpValueDto> lookUpData, EmailTemplate templateData,
        List<TemplateField> templateFeildData)
        {
            foreach (var item in lookUpData)
            {
                using (MailMessage messageObj = new MailMessage
                {
                    IsBodyHtml = true,
                    From = new MailAddress("arfanali.cloud@gmail.com", "Balke App")
                })
                {
                    string[] Multi = item.EmailTo.Split(',');
                    foreach (var Multiemailid in Multi)
                    {
                        messageObj.To.Add(new MailAddress(Multiemailid));
                    }
                    messageObj.Subject = lookUp.Subject;
                    //string s = "Name:{0} {1}, Location:{2}, Age:{3}";

                    #region MyRegion

                    string combindedString = string.Join(",", item.FirstName);
                    //List<int> numbers = new List<int> { 1, 2, 3 };


                    var EmailBody = string.Format(templateData.Content, item.FirstName, item.LastName, 100);

                    #endregion

                    messageObj.Body = EmailBody; // templateData.Content.Replace("{0}", lookupData
                    using (var client = new SmtpClient("smtp.gmail.com", 587))
                    {
                        client.UseDefaultCredentials = false;
                        client.Credentials =
                        new NetworkCredential("arfanali.cloud@gmail.com", "ibifodcrfbavneht");
                        client.EnableSsl = true;
                        try
                        {
                            client.Send(messageObj);
                            return true;
                        }
                        catch (Exception ex)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        private bool SendEmail(LookUpValueDto lookUp, List<LookUpValueDto> lookUpData, EmailTemplate templateData,
        List<TemplateField> templateFieldData)
        {
            foreach (var item in lookUpData)
            {
                using (var messageObj = new MailMessage
                {
                    IsBodyHtml = true,
                    From = new MailAddress("arfanali.cloud@gmail.com", "Balke App")
                })
                {
                    var multi = item.EmailTo.Split(',');
                    foreach (var multiEmailId in multi)
                    {
                        messageObj.To.Add(new MailAddress(multiEmailId));
                    }
                    messageObj.Subject = lookUp.Subject;
                    //string s = "Name:{0} {1}, Location:{2}, Age:{3}";

                    #region MyRegion

                    string combindedString = string.Join(",", item.FirstName);
                    //List<int> numbers = new List<int> { 1, 2, 3 };


                    var EmailBody = string.Format(templateData.Content, item.FirstName, item.LastName, 100);

                    #endregion

                    messageObj.Body = EmailBody; // templateData.Content.Replace("{0}", lookupData
                    using (var client = new SmtpClient("smtp.gmail.com", 587))
                    {
                        client.UseDefaultCredentials = false;
                        client.Credentials =
                        new NetworkCredential("arfanali.cloud@gmail.com", "ibifodcrfbavneht");
                        client.EnableSsl = true;
                        try
                        {
                            client.Send(messageObj);
                            return true;
                        }
                        catch (Exception ex)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        public bool SendWinEmail(string email, string template)
        {
            using (var messageObj = new MailMessage
            {
                IsBodyHtml = true,
                From = new MailAddress("arfanali.cloud@gmail.com", "Balke App")
            })
            {
                messageObj.To.Add(new MailAddress(email));
                messageObj.Subject = "Quiz";

                #region MyRegion

                var EmailBody = string.Format(template, 100);

                #endregion

                messageObj.Body = EmailBody; // templateData.Content.Replace("{0}", lookupData
                using (var client = new SmtpClient("smtp.gmail.com", 587))
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials =
                    new NetworkCredential("arfanali.cloud@gmail.com", "ibifodcrfbavneht");
                    client.EnableSsl = true;
                    try
                    {
                        client.Send(messageObj);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        private bool EmailSingUpGroupData(string to, string subject, string body)
        {
            using (MailMessage messageObj = new MailMessage
            {
                IsBodyHtml = true,
                From = new MailAddress("support@balke.tech", "Balke App")
            })
            {
                string[] Multi = to.Split(',');
                foreach (var Multiemailid in Multi)
                {
                    messageObj.To.Add(new MailAddress(Multiemailid));
                }
                messageObj.Subject = subject;
                //string s = "Name:{0} {1}, Location:{2}, Age:{3}";

                #region MyRegion

                //List<int> numbers = new List<int> { 1, 2, 3 };


                //var emailBody = string.Format(body ?? "", "", 100);

                #endregion

                messageObj.Body = body; // templateData.Content.Replace("{0}", lookupData
                using (var client = new SmtpClient("smtp-mail.outlook.com", 587))
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials =
                    new NetworkCredential("support@balke.tech", "Balke@102");
                    client.EnableSsl = true;
                    try
                    {
                        client.Send(messageObj);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
            }
        }
        private bool EmailInfoGroupData(string to, string subject, string body)
        {
            to = "info@balke.tech";
            using (MailMessage messageObj = new MailMessage
            {
                IsBodyHtml = true,
                From = new MailAddress("info@balke.tech", "Balke App")
            })
            {
                string[] Multi = to.Split(',');
                foreach (var Multiemailid in Multi)
                {
                    messageObj.To.Add(new MailAddress(Multiemailid));
                }
                messageObj.Subject = subject;
                //string s = "Name:{0} {1}, Location:{2}, Age:{3}";

                #region MyRegion

                //List<int> numbers = new List<int> { 1, 2, 3 };


                //var emailBody = string.Format(body ?? "", "", 100);

                #endregion

                messageObj.Body = body; // templateData.Content.Replace("{0}", lookupData
                using (var client = new SmtpClient("smtp-mail.outlook.com", 587))
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials =
                    new NetworkCredential("info@balke.tech", "Balke@103");
                    client.EnableSsl = true;
                    try
                    {
                        client.Send(messageObj);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
            }
        }
        private bool SendEmailUsingSMTP(string toEmail, string subject, string message)
        {
            using (MailMessage messageObj = new MailMessage
            {
                IsBodyHtml = true,
                From = new MailAddress("arfanali.cloud@gmail.com", "Balke App")
            }
            )
            {
                string[] Multi = toEmail.Split(',');
                foreach (var Multiemailid in Multi)
                {
                    messageObj.To.Add(new MailAddress(Multiemailid));
                }
                messageObj.Subject = subject;
                messageObj.Body = message;
                using (var client = new SmtpClient("smtp.gmail.com", 587))
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials =
                    new NetworkCredential("arfanali.cloud@gmail.com", "ibifodcrfbavneht");
                    client.EnableSsl = true;
                    try
                    {
                        client.Send(messageObj);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
            }
        }
        public bool SendEmailForTimeSheet(string to, string subject)
        {
            using (MailMessage messageObj = new MailMessage
            {
                IsBodyHtml = true,
                From = new MailAddress("arfanali.cloud@gmail.com", "Balke App")
            })
            {
                string[] Multi = to.Split(',');
                foreach (var Multiemailid in Multi)
                {
                    messageObj.To.Add(new MailAddress(Multiemailid));
                }
                messageObj.Subject = subject;
                messageObj.Body = "Time Sheet"; // templateData.Content.Replace("{0}", lookupData
                using (var client = new SmtpClient("smtp.gmail.com", 587))
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials =
                        new NetworkCredential("arfanali.cloud@gmail.com", "ibifodcrfbavneht");
                    client.EnableSsl = true;
                    try
                    {
                        client.Send(messageObj);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
            }
        }
    }
}
