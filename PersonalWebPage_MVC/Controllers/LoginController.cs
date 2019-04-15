using PersonalWebPage_MVC.Db.Concrete;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;

namespace PersonalWebPage_MVC.Controllers
{
    [Authorize]
    public class LoginController : Controller
    {
        #region Login,Logout
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public string ValidateLoginIfo(string userName, string pwd, string isRemember = "false")
        {
            string returnValue;
            try
            {
                TblLoginInfo tblLoginInfo = new TblLoginInfo();

                string currentEncryptePwd = tblLoginInfo.EncryptSha(tblLoginInfo.EncryptMd5(HttpUtility.UrlDecode(pwd)));
                string currentUserName = HttpUtility.UrlDecode(userName).Trim();

                string dbEncryptePwd = tblLoginInfo.GetAll().OrderBy(a => a.ID).FirstOrDefault().Password.ToString();
                string dbUserName = tblLoginInfo.GetAll().OrderBy(a => a.ID).FirstOrDefault().UserName.ToString();

                if (!String.IsNullOrEmpty(currentEncryptePwd) && !String.IsNullOrEmpty(currentUserName) && !String.IsNullOrEmpty(dbEncryptePwd) && !String.IsNullOrEmpty(dbUserName))
                {
                    if (currentUserName == dbUserName)
                    {
                        if (currentEncryptePwd == dbEncryptePwd)
                        {
                            if (isRemember == "true")
                            {
                                FormsAuthentication.RedirectFromLoginPage(userName, true);
                            }
                            else
                            {
                                FormsAuthentication.RedirectFromLoginPage(userName, false);
                            }
                            returnValue = "true";

                            return returnValue;
                        }
                        else
                        {
                            returnValue = "false";
                            return returnValue;
                        }
                    }
                    else
                    {
                        returnValue = "false";
                        return returnValue;
                    }
                }
                else
                {
                    returnValue = "false";
                    return returnValue;
                }
            }
            catch
            {
                returnValue = "false";
                return returnValue;
            }
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");
        } 
        #endregion

        #region About View
        public ActionResult About()
        {
            ViewBag.About = "active";
            TblPerson tblPerson = new TblPerson();
            var person = tblPerson.GetAll().OrderBy(a => a.ID).FirstOrDefault();
            return View(person);
        }

        [HttpPost]
        public JsonResult UploadPhoto()
        {
            try
            {
                var requestFile = Request.Files[0];
                //var id = Request.Params["id"];
                string uploadFolder = "/Files/Images/";
                FileInfo fileInfo = new FileInfo(requestFile.FileName);
                string fileExtension = fileInfo.Extension;
                string fileName = fileInfo.ToString().Substring(0, fileInfo.ToString().Length - fileExtension.Length);
                Guid g = Guid.NewGuid();
                string shortGuid = g.ToString().Substring(0, 8);
                StringBuilder filePath = new StringBuilder();
                filePath.Append(uploadFolder);
                filePath.Append(fileName);
                filePath.Append("-");
                filePath.Append(shortGuid);
                filePath.Append(fileExtension);
                requestFile.SaveAs(Server.MapPath(filePath.ToString()));
                PhotoPathUpdate(filePath.ToString());
                return Json(new { IsUploaded = true, photoPath = filePath.ToString() });
            }
            catch
            {
                return Json(new { IsUploaded = false });
            }
        }

        public void PhotoPathUpdate(string photoPath)
        {
            TblPerson tblPerson = new TblPerson();
            string sqlStr = String.Format("update People set Photo='{0}' where ID= (select top 1 ID from People order by ID)", photoPath);
            int status = tblPerson.ExecuteQuery(sqlStr);
        }

        [HttpPost]
        public ActionResult PersonelInfoUpdate(Models.Person person)
        {
            TblPerson tblPerson = new TblPerson();
            string sqlStr = String.Format("update People set Name='{0}',Surname='{1}',Address='{2}',Mail='{3}',Description='{4}',Linkedin='{5}',Github='{6}',Twitter='{7}',Facebook='{8}',Youtube='{9}',Instagram='{10}' where ID='{11}'",
                HttpUtility.UrlDecode(person.Name),
                HttpUtility.UrlDecode(person.Surname),
                HttpUtility.UrlDecode(person.Address),
                HttpUtility.UrlDecode(person.Mail),
                HttpUtility.UrlDecode(person.Description),
                HttpUtility.UrlDecode(person.Linkedin),
                HttpUtility.UrlDecode(person.Github),
                HttpUtility.UrlDecode(person.Twitter),
                HttpUtility.UrlDecode(person.Facebook),
                HttpUtility.UrlDecode(person.Youtube),
                HttpUtility.UrlDecode(person.Instagram),
               person.ID
                );
            int status = tblPerson.ExecuteQuery(sqlStr);
            return RedirectToAction("About");
        }
        #endregion

        #region Experience View
        public ActionResult Experience()
        {
            ViewBag.Experience = "active";
            return View();
        }
        public PartialViewResult GetExperiences()
        {
            var tblExperience = new TblExperience();
            return PartialView("_PartialExperience", tblExperience.GetAll());
        }
        [HttpPost]
        public PartialViewResult InsertExperience(Models.Experience data)
        {
            var experience = new Models.Experience()
            {
                Position = HttpUtility.UrlDecode(data.Position),
                Corporation = HttpUtility.UrlDecode(data.Corporation),
                Date = HttpUtility.UrlDecode(data.Date),
                Description = HttpUtility.UrlDecode(data.Description)
            };
            var tblExperience = new TblExperience();
            tblExperience.Add(experience);
            return PartialView("_PartialExperience", tblExperience.GetAll());
        }
        [HttpPost]
        public PartialViewResult UpdateExperience(Models.Experience data)
        {
            var experience = new Models.Experience()
            {
                ID = data.ID,
                Position = HttpUtility.UrlDecode(data.Position),
                Corporation = HttpUtility.UrlDecode(data.Corporation),
                Date = HttpUtility.UrlDecode(data.Date),
                Description = HttpUtility.UrlDecode(data.Description)
            };
            var tblExperience = new TblExperience();
            tblExperience.Update(experience);
            return PartialView("_PartialExperience", tblExperience.GetAll());
        }
        [HttpPost]
        public PartialViewResult RemoveExperience(int id)
        {
            var experience = new Models.Experience() { ID = id };
            var tblExperience = new TblExperience();
            tblExperience.Delete(experience);
            return PartialView("_PartialExperience", tblExperience.GetAll());
        }
        #endregion

        #region Educations View
        public ActionResult Education()
        {
            ViewBag.Education = "active";
            return View();
        }
        public PartialViewResult GetEducations()
        {
            var tblEdu = new TblEducation();
            return PartialView("_PartialEducation", tblEdu.GetAll());
        }
        [HttpPost]
        public PartialViewResult InsertEducation(Models.Education e)
        {
            var edu = new Models.Education() { University = HttpUtility.UrlDecode(e.University), Faculty = HttpUtility.UrlDecode(e.Faculty), Department = HttpUtility.UrlDecode(e.Department), Date = HttpUtility.UrlDecode(e.Date) };
            var tblEdu = new TblEducation();
            tblEdu.Add(edu);
            return PartialView("_PartialEducation", tblEdu.GetAll());
        }
        [HttpPost]
        public PartialViewResult UpdateEducation(Models.Education e)
        {
            var edu = new Models.Education() { ID = e.ID, University = HttpUtility.UrlDecode(e.University), Faculty = HttpUtility.UrlDecode(e.Faculty), Department = HttpUtility.UrlDecode(e.Department), Date = HttpUtility.UrlDecode(e.Date) };
            var tblEdu = new TblEducation();
            tblEdu.Update(edu);
            return PartialView("_PartialEducation", tblEdu.GetAll());
        }
        [HttpPost]
        public PartialViewResult RemoveEducation(int id)
        {
            var edu = new Models.Education() { ID = id };
            var tblEdu = new TblEducation();
            tblEdu.Delete(edu);
            return PartialView("_PartialEducation", tblEdu.GetAll());
        }
        #endregion

        #region Skills View
        public ActionResult Skills()
        {
            ViewBag.Skills = "active";
            return View();
        }
        //tech
        public PartialViewResult GetTechSkills()
        {
            var tblSkill = new TblSkill();
            return PartialView("_PartialTechSkills", tblSkill.GetAll().Where(a => a.EntryType == "tech").ToList());
        }
        public PartialViewResult RemoveTechSkill(int id)
        {
            var skillTech = new Models.Skill() { ID = id };
            var tblSkillTech = new TblSkill();
            tblSkillTech.Delete(skillTech);
            return PartialView("_PartialTechSkills", tblSkillTech.GetAll().Where(a => a.EntryType == "tech").ToList());
        }
        public PartialViewResult InsertTechSkill(string desc)
        {
            var skillTech = new Models.Skill() { Description = HttpUtility.UrlDecode(desc), EntryType = "tech" };
            var tblSkillTech = new TblSkill();
            tblSkillTech.Add(skillTech);
            return PartialView("_PartialTechSkills", tblSkillTech.GetAll().Where(a => a.EntryType == "tech").ToList());
        }
        //work
        public PartialViewResult GetWorkSkills()
        {
            var tblSkill = new TblSkill();
            return PartialView("_PartialWorkSkills", tblSkill.GetAll().Where(a => a.EntryType == "work").ToList());
        }
        [HttpPost]
        public PartialViewResult InsertWorkSkill(string desc)
        {
            var skillWork = new Models.Skill() { Description = HttpUtility.UrlDecode(desc), EntryType = "work" };
            var tblSkillWork = new TblSkill();
            tblSkillWork.Add(skillWork);
            return PartialView("_PartialWorkSkills", tblSkillWork.GetAll().Where(a => a.EntryType == "work").ToList());
        }
        [HttpPost]
        public PartialViewResult UpdateWorkSkill(int id, string desc)
        {
            var skillWork = new Models.Skill() { ID = id, Description = HttpUtility.UrlDecode(desc), EntryType = "work" };
            var tblSkillWork = new TblSkill();
            tblSkillWork.Update(skillWork);
            return PartialView("_PartialWorkSkills", tblSkillWork.GetAll().Where(a => a.EntryType == "work").ToList());
        }
        [HttpPost]
        public PartialViewResult RemoveWorkSkill(int id)
        {
            var skillWork = new Models.Skill() { ID = id };
            var tblSkillWork = new TblSkill();
            tblSkillWork.Delete(skillWork);
            return PartialView("_PartialWorkSkills", tblSkillWork.GetAll().Where(a => a.EntryType == "work").ToList());
        }
        #endregion

        #region Interest View
        public ActionResult Interests()
        {
            ViewBag.Interests = "active";
            return View();
        }
        public PartialViewResult GetInterests()
        {
            var tblInterest = new TblInterest();
            return PartialView("_PartialInterest", tblInterest.GetAll());
        }
        [HttpPost]
        public PartialViewResult InsertInterest(string desc)
        {
            var interest = new Models.Interest() { Description = HttpUtility.UrlDecode(desc) };
            var tblInterest = new TblInterest();
            tblInterest.Add(interest);
            return PartialView("_PartialInterest", tblInterest.GetAll());
        }
        [HttpPost]
        public PartialViewResult UpdateInterest(int id, string desc)
        {
            var interest = new Models.Interest() { ID = id, Description = HttpUtility.UrlDecode(desc) };
            var tblInterest = new TblInterest();
            tblInterest.Update(interest);
            return PartialView("_PartialInterest", tblInterest.GetAll());
        }
        [HttpPost]
        public PartialViewResult RemoveInterest(int id)
        {
            var interest = new Models.Interest() { ID = id };
            var tblInterest = new TblInterest();
            tblInterest.Delete(interest);
            return PartialView("_PartialInterest", tblInterest.GetAll());
        }
        #endregion

        #region Award View
        public ActionResult Award()
        {
            ViewBag.Award = "active";
            return View();
        }
        public PartialViewResult GetAwards()
        {
            var tblAward = new TblAward();
            return PartialView("_PartialAward", tblAward.GetAll());
        }
        [HttpPost]
        public PartialViewResult InsertAward(string desc)
        {
            var award = new Models.Award() { Description = HttpUtility.UrlDecode(desc) };
            var tblAward = new TblAward();
            tblAward.Add(award);
            return PartialView("_PartialAward", tblAward.GetAll());
        }
        [HttpPost]
        public PartialViewResult UpdateAward(int id, string desc)
        {
            var award = new Models.Award() { ID = id, Description = HttpUtility.UrlDecode(desc) };
            var tblAward = new TblAward();
            tblAward.Update(award);
            return PartialView("_PartialAward", tblAward.GetAll());
        }
        [HttpPost]
        public PartialViewResult RemoveAward(int id)
        {
            var award = new Models.Award() { ID = id };
            var tblAward = new TblAward();
            tblAward.Delete(award);
            return PartialView("_PartialAward", tblAward.GetAll());
        }
        #endregion

        #region LoginInfoView
        public ActionResult LoginInfo()
        {
            ViewBag.LoginInfo = "active";
            return View();
        }
        [HttpGet]
        public JsonResult GetLoginInfo()
        {
            TblLoginInfo tblLoginInfo = new TblLoginInfo();
            Models.LoginInfo lgIn = new Models.LoginInfo()
            {
                ID = tblLoginInfo.GetAll().OrderByDescending(i => i.ID).FirstOrDefault().ID,
                UserName = tblLoginInfo.GetAll().OrderByDescending(i => i.UserName).FirstOrDefault().UserName
            };
            object loginInfo = lgIn;
            return Json(loginInfo, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLoginInfo(bool status)
        {
            TblLoginInfo tblLoginInfo = new TblLoginInfo();
            UpdateStatus lgIn = new UpdateStatus()
            {
                ID = tblLoginInfo.GetAll().OrderByDescending(i => i.ID).FirstOrDefault().ID,
                UserName = tblLoginInfo.GetAll().OrderByDescending(i => i.UserName).FirstOrDefault().UserName,
                Status = status
            };
            object loginInfo = lgIn;
            return Json(loginInfo, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult UpdateLoginInfo(int id, string name, string newPwd, string currentPwd)
        {
            TblLoginInfo tblLoginInfo = new TblLoginInfo();

            string currentEncrypte = tblLoginInfo.EncryptSha(tblLoginInfo.EncryptMd5(HttpUtility.UrlDecode(currentPwd)));
            string dbEncrypte = tblLoginInfo.GetAll().OrderBy(a => a.ID).FirstOrDefault().Password.ToString();

            if (currentEncrypte == dbEncrypte)
            {
                string newPwdEncrypte = tblLoginInfo.EncryptSha(tblLoginInfo.EncryptMd5(HttpUtility.UrlDecode(newPwd)));
                Models.LoginInfo loginInfo = new Models.LoginInfo()
                {
                    ID = id,
                    UserName = HttpUtility.UrlDecode(name),
                    Password = newPwdEncrypte,
                    TransactionDate = DateTime.Now.ToString()
                };
                tblLoginInfo.Update(loginInfo);
                return GetLoginInfo();
            }
            else
            {
                return GetLoginInfo(false);
            }
        }
        #endregion
    }
    //LoginInfo View'de, başarısız güncelleme işlemini jquery'e bildirmek için kullanıldı.
    public class UpdateStatus
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        public bool Status { get; set; }
    }
}