//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Net.Mail;
//using System.Threading;
//using System.Web;
//using System.Web.Mvc;

//namespace ProjectWith.Models.ath_windows
//{
//	public class MailSender
//	{


//		internal static void SendToResponsable(Controller controller, int? Numero)
//		{


//			baseEntities db = new baseEntities();
//			EntretienAnnuel entretienAnnuel = db.EntretienAnnuel.Find(Numero);

//			EntretienAnnuel entretienAnnuelone = db.EntretienAnnuel.Where(e => e.id == Numero).SingleOrDefault();

//			var empVeri = db.Employee.Where(em => em.Matricule == entretienAnnuelone.matriculeResp).SingleOrDefault();




//			List<MailAddress> ToEmp = new List<MailAddress>();
//			List<MailAddress> CcEmp = new List<MailAddress>();

//			if (!string.IsNullOrEmpty(empVeri.email))
//			{
//				CcEmp.Add(new MailAddress(empVeri.email, empVeri.Prenom.ToString() + " " + empVeri.Nom.ToString()));
//			}



//			string MObject;
//			string MBody = "";

//			string AbsolutePath = HttpContext.Current.Request.Url.AbsolutePath;
//			string AbsoluteUri = HttpContext.Current.Request.Url.AbsoluteUri;

//			string ServerPath = AbsoluteUri.Replace(AbsolutePath, "/");

//			var EmpNom = entretienAnnuelone.Nom.ToString() + " " + entretienAnnuelone.Prenom.ToString();

//			//MObject = "Notification Entretien Annuel: Un Entretien Annuel a été rempli par votre Employé :" + EmpNom + "!";
//			MObject = "Notification Entretien Annuel !";

//			MBody = RenderStringFromView(controller, "~/Views/MailTemplate/_TempMailSendToResponsable.cshtml", entretienAnnuelone);


//			// Déclaration du thread
//			Thread ThreadModelSendMail;

//			// Instanciation du thread
//			ThreadModelSendMail = new Thread(new ThreadStart(() => ModelSendMailUniSens(CcEmp, MObject, MBody)));

//			// Lancement du thread
//			ThreadModelSendMail.Start();

//		}



//		private static void ModelSendMailUniSens(List<MailAddress> To, string Subject, string Body)
//		{

//			baseEntities db = new baseEntities();

//			string sError;
//			string mail_from_mail = db.Config.Find("mail_from_mail").Val;
//			string mail_from_name = db.Config.Find("mail_from_name").Val;
//			string mail_smtp_credentials_password = db.Config.Find("mail_smtp_credentials_password").Val;
//			string mail_smtp_credentials_username = db.Config.Find("mail_smtp_credentials_username").Val;
//			string mail_smtp_host = db.Config.Find("mail_smtp_host").Val;
//			int mail_smtp_port = Int32.Parse(db.Config.Find("mail_smtp_port").Val);
//			bool mail_smtp_enablessl;
//			if (db.Config.Find("mail_smtp_enablessl").Val == "true")
//			{
//				mail_smtp_enablessl = true;
//			}
//			else
//			{
//				mail_smtp_enablessl = false;
//			}


//			//Mail Message
//			MailMessage msg = new MailMessage();

//			//receiver email id
//			for (int i = 0; i < To.Count; i++)
//			{
//				msg.To.Add(To.ElementAt(i));
//			}




//			msg.From = new MailAddress(mail_from_mail, mail_from_name);

//			// Objet du mail
//			msg.Subject = Subject;

//			// Contenue du mail
//			msg.Body = Body;
//			msg.IsBodyHtml = true;

//			//SMTP client
//			SmtpClient client = new SmtpClient();
//			client.UseDefaultCredentials = false;
//			//credentials to login in to hotmail account
//			client.Credentials = new System.Net.NetworkCredential(mail_smtp_credentials_username, mail_smtp_credentials_password);
//			//port number for mail
//			client.Port = mail_smtp_port;
//			client.Host = mail_smtp_host;
//			client.DeliveryMethod = SmtpDeliveryMethod.Network;
//			client.EnableSsl = mail_smtp_enablessl;
//			try
//			{
//				// Send the email
//				client.Send(msg);

//			}
//			catch (Exception ex)
//			{
//				sError = ex.ToString();
//			}

//		}


//		private static string RenderStringFromView(Controller controller, string viewName, object model)
//		{

//			controller.ViewData.Model = model;

//			try
//			{
//				using (var sw = new StringWriter())
//				{
//					var viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
//					var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
//					viewResult.View.Render(viewContext, sw);
//					string yy = sw.GetStringBuilder().ToString();
//					return yy;
//				}
//			}
//			catch (Exception ex)
//			{
//				return ex.ToString();
//			}

//		}
//	}
//}