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

        public void UpdateFirebaseID(string firebaseId, string user)
        {
            var find = db.FirebaseIDSaves.Where(p => p.UserID == user).FirstOrDefault();

            if(find == null)
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
            } else
            {
                find.CreateTime = DateTime.Now;
                find.FirebaseID = firebaseId;
                db.Entry(find).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
        }

    }
}