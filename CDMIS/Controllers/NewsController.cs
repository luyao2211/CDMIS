using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CDMIS.Models;
using CDMIS.ViewModels;
using CDMIS.ServiceReference;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using CDMIS.OtherCs;

namespace CDMIS.Controllers
{
    [UserAuthorize]
    public class NewsController : Controller
    {
        public static ServicesSoapClient _ServicesSoapClient = new ServicesSoapClient();//WebService
        string hostAddress = "";
        string hostport = "";
        //
        // GET: /News/
        #region <ActionResult>
        public ActionResult Index()
        {
            HealthEducationList HElist = new HealthEducationList();
            DataSet info = _ServicesSoapClient.GetAddressByTypeList(HElist.selectedModuleId, 5);
            foreach (DataRow row in info.Tables[0].Rows)
            {
                HealthEducation news = new HealthEducation();
                news.Module = row[0].ToString();
                news.ModuleName = row[1].ToString();
                news.Id = row[2].ToString();
                news.Type = Convert.ToInt32(row[3].ToString());
                news.FileName = row[5].ToString();
                news.Path = row[6].ToString();
                news.Title = row[7].ToString();
                news.CreateDateTime = row[8].ToString();
                news.Author = row[9].ToString();
                news.AuthorName = row[10].ToString();

                HElist.HEList.Add(news);
            }
            return View(HElist);
        }

        public ActionResult Create()
        {
            NewHealthEducationFile nhe = new NewHealthEducationFile();
            return View(nhe);
        }

        [HttpPost]
        public ActionResult Create(NewHealthEducationFile newhe)
        {
            if (ModelState.IsValid)
            {
                string dir = Server.MapPath("/") + "HealthEducation\\";
                var user = Session["CurrentUser"] as UserAndRole;
                string servertime = _ServicesSoapClient.GetServerTime();
                newhe.news.FileName = newhe.selectedModuleId + "_" + servertime.Replace(':', '_') + ".html";
                //newhe.news.Path = dir + newhe.news.FileName;
                StreamReader sr = new StreamReader(dir + "head.txt", Encoding.Default);
                string head, temp;
                head = "";
                while ((temp = sr.ReadLine()) != null)
                {
                    head = head + temp;
                }
                temp = head + newhe.news.htmlContent + "</body></html>";
                sr.Close();

                System.IO.File.WriteAllText(dir + newhe.news.FileName, temp, Encoding.GetEncoding("GB2312"));
                newhe.news.Author = user.UserId;
                //

                //保存数据
                hostAddress = Request.ServerVariables.Get("Local_Addr").ToString();
                if (hostAddress == "::1")
                {
                    hostAddress = "127.0.0.1";
                }
                hostport = Request.ServerVariables.Get("Server_Port").ToString();
                //newhe.news.Path = "http://" + hostAddress + ":" + hostport + "/HealthEducation/" + newhe.news.FileName;
                newhe.news.Path = "/HealthEducation/" + newhe.news.FileName;
                if (newhe.news.Title == null || newhe.news.Title == "")
                {
                    newhe.news.Title = "无主题";
                }
                bool flag = _ServicesSoapClient.SetHealthEducation(newhe.selectedModuleId, "0", 5, newhe.news.FileName, newhe.news.Path, newhe.news.Title, servertime, newhe.news.Author, user.TerminalName, user.TerminalIP, user.DeviceType);
                if (flag)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(newhe);
                }
            }
            return View(newhe);
        }

        public ActionResult Edit(string Module, string Id)
        {
            DataSet info = _ServicesSoapClient.GetAll(Module, Id);
            HealthEducation news = new HealthEducation();
            if (info.Tables[0].Rows.Count > 0)
            {
                DataRow row = info.Tables[0].Rows[0];
                news.Module = Module;
                news.Id = Id;
                news.Type = Convert.ToInt32(row[2].ToString());
                news.FileName = row[4].ToString();
                news.Path = row[5].ToString();
                news.Title = row[6].ToString();
                news.CreateDateTime = row[7].ToString();
                news.Author = row[8].ToString();
                news.AuthorName = row[9].ToString();

                string dir = Server.MapPath("/") + "HealthEducation\\";
                StreamReader sr = new StreamReader(dir + news.FileName, Encoding.GetEncoding("GB2312"));

                string temp;
                news.htmlContent = "";
                if ((temp = sr.ReadLine()) != null)
                {
                    Regex reg = new Regex(@"<body>([\s\S]*)</body>", RegexOptions.IgnoreCase);
                    MatchCollection mc = reg.Matches(temp);
                    news.htmlContent = mc[0].Value;
                    news.htmlContent = news.htmlContent.Substring(6, news.htmlContent.Length - 13);
                }
                sr.Close();
            }
            NewHealthEducationFile nhe = new NewHealthEducationFile();
            nhe.selectedModuleId = Module;
            nhe.news = news;
            return View(nhe);
        }

        [HttpPost]
        public ActionResult Edit(NewHealthEducationFile newhe)
        {
            if (ModelState.IsValid)
            {
                string dir = Server.MapPath("/") + "HealthEducation\\";
                var user = Session["CurrentUser"] as UserAndRole;
                string servertime = _ServicesSoapClient.GetServerTime();
                //newhe.news.FileName = newhe.selectedModuleId + "_" + servertime + ".html";
                //newhe.news.Path = dir + newhe.news.FileName;
                StreamReader sr = new StreamReader(dir + "head.txt", Encoding.Default);
                string head, temp;
                head = "";
                while ((temp = sr.ReadLine()) != null)
                {
                    head = head + temp;
                }
                temp = head + newhe.news.htmlContent + "</body></html>";
                sr.Close();

                if (System.IO.File.Exists(dir + newhe.news.FileName))
                {
                    System.IO.File.Delete(dir + newhe.news.FileName);
                }

                System.IO.File.WriteAllText(dir + newhe.news.FileName, temp, Encoding.GetEncoding("GB2312"));
                newhe.news.Author = user.UserId;
                //
                //保存数据
                hostAddress = Request.ServerVariables.Get("Local_Addr").ToString();
                if (hostAddress == "::1")
                {
                    hostAddress = "127.0.0.1";
                }
                hostport = Request.ServerVariables.Get("Server_Port").ToString();
                //newhe.news.Path = "http://" + hostAddress + ":" + hostport + "/HealthEducation/" + newhe.news.FileName;
                newhe.news.Path = "/HealthEducation/" + newhe.news.FileName;
                if (newhe.news.Title == null || newhe.news.Title == "")
                {
                    newhe.news.Title = "无主题";
                }
                bool flag = _ServicesSoapClient.SetHealthEducation(newhe.selectedModuleId, newhe.news.Id, 5, newhe.news.FileName, newhe.news.Path, newhe.news.Title, servertime, newhe.news.Author, user.TerminalName, user.TerminalIP, user.DeviceType);
                if (flag)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(newhe);
                }
            }
            return View(newhe);
        }

        public JsonResult DeleteHealthEducation(string Module, string Id)
        {
            var res = new JsonResult();
            int flag = _ServicesSoapClient.DeleteHealthEducationInfo(Module, Id);
            if (flag == 1)
            {
                res.Data = true;
            }
            else
            {
                res.Data = false;
            }
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }
        #endregion

        #region function
        ////从Cm.MstHealthEducation表中读取所有健康教育资料的列表（倒序）
        //public DataTable GetHEFileList(string moduleId, string fileType)
        //{
        //    DataTable filedt = new DataTable();

        //    return filedt;
        //}
        #endregion
    }
}
