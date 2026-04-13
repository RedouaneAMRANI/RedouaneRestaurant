using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RestaurantManagement
{
    public static class ActivityLogger
    {
        public static void Log(string action, string entity, int? entityId = null)
        {
            try
            {
                using (EFDBEntities db = new EFDBEntities())
                {
                    EmployeeActivity activity = new EmployeeActivity();
                    activity.CNIE = CurrentUser.CNIE;
                    activity.Action = action;
                    activity.Entity = entity;
                    activity.EntityId = entityId;
                    activity.CreatedAt = DateTime.Now;

                    db.EmployeeActivities.Add(activity);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to log activity: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
