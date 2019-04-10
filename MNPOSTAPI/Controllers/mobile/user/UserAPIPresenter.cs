using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MNPOSTCOMMON;
using MNPOSTAPI.Models;

namespace MNPOSTAPI.Controllers.mobile.user
{
    public class UserAPIPresenter
    {
        MNPOSTEntities db = new MNPOSTEntities();

        public UserInfoResult GetUserInfo(string user, string firebaseId)
        {

            var checkUser = db.BS_Employees.Where(p => p.UserLogin == user).FirstOrDefault();

            if (checkUser == null)
            {
                return new UserInfoResult()
                {
                    error = 1,
                    msg = "Tài khoản không có quyền truy cập"
                };
            }

            UpdateFirebaseID(firebaseId, user);

            return new UserInfoResult()
            {
                error = 0,
                msg = "",
                FullName = checkUser.EmployeeName,
                EmployeeCode = checkUser.EmployeeID,
                PostOfficeID = checkUser.PostOfficeID
            };
        }
        public ResultInfo GetNotice(string user)
        {
            /*
             var find = db.NoticeSaves.Where(p => p.MNUser == user).Select(p => new
            {
                id = p.Id,
                time = p.CreateTime.Value.ToString("dd/MM/yyyy"),
                title = p.Title,
                messenger = p.Content,
                content = p.Content,
                isRead = p.IsRead
            }).ToList();
            */

            var data = (from p in db.NoticeSaves
                       where p.MNUser == user
                       orderby p.CreateTime descending
                       select new
                       {
                           id = p.Id,
                           time = p.CreateTime.ToString(),
                           title = p.Title,
                           messenger = p.Content,
                           content = p.Content,
                           isRead = p.IsRead
                       }).ToList();

            return new NoticeResult()
            {
                error = 0,
                msg = "",
                data = data
            };
        }
        public void UpdateFirebaseID(string firebaseId, string user)
        {
            var find = db.FirebaseIDSaves.Where(p => p.UserID == user).FirstOrDefault();

            if (find == null)
            {
                var ins = new FirebaseIDSave()
                {
                    Id = Guid.NewGuid().ToString(),
                    CreateTime = DateTime.Now,
                    FirebaseID = firebaseId,
                    UserID = user
                };

                db.FirebaseIDSaves.Add(ins);
                db.SaveChanges();
            }
            else
            {
                find.CreateTime = DateTime.Now;
                find.FirebaseID = firebaseId;
                db.Entry(find).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
        }

    }
}