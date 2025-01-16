using DealerNetAPI.Common;
using DealerNetAPI.DomainObject;
using DealerNetAPI.ResourceAccess.Interface;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace DealerNetAPI.ResourceAccess
{
    public class MenuAccess : IMenuAccess
    {
        private readonly ICommonDB _commonDB = null;
        public MenuAccess(ICommonDB commonDB)
        {
            _commonDB = commonDB;
        }

        public async Task<APIResponse> SaveMenu(Menu menu)
        {
            APIResponse apiResponse = new APIResponse();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[13];
                arrParams[0] = new OracleParameter("P_MENU_ID", OracleDbType.Varchar2);
                if (menu.MenuId == 0 || string.IsNullOrEmpty(Convert.ToString(menu.MenuId)))
                {
                    arrParams[0].Value = DBNull.Value;
                }
                else
                {
                    arrParams[0].Value = menu.MenuId;
                }
                arrParams[1] = new OracleParameter("P_MENU_NAME", OracleDbType.Varchar2);
                arrParams[1].Value = menu.MenuName;
                arrParams[2] = new OracleParameter("P_DESCRIPTION", OracleDbType.Varchar2);
                arrParams[2].Value = menu.MenuDesc;
                arrParams[3] = new OracleParameter("P_PARENT_MENU_ID", OracleDbType.Varchar2);
                arrParams[3].Value = menu.MenuParentId;
                arrParams[4] = new OracleParameter("P_MENU_LEVEL", OracleDbType.Varchar2);
                arrParams[4].Value = menu.MenuLevel;
                arrParams[5] = new OracleParameter("P_MENU_URL", OracleDbType.Varchar2);
                arrParams[5].Value = menu.MenuURL;
                arrParams[6] = new OracleParameter("P_DISPLAY_SEQ", OracleDbType.Varchar2);
                arrParams[6].Value = menu.DisplaySeq;
                arrParams[7] = new OracleParameter("P_STATUS", OracleDbType.Varchar2);
                arrParams[7].Value = menu.Status;
                arrParams[8] = new OracleParameter("P_TARGET", OracleDbType.Varchar2);
                arrParams[8].Value = menu.Target;
                arrParams[9] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2);
                arrParams[9].Value = menu.CreatedBy;
                arrParams[10] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 500);
                arrParams[10].Direction = ParameterDirection.Output;
                arrParams[11] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                arrParams[11].Direction = ParameterDirection.Output;
                arrParams[12] = new OracleParameter("P_ROUTE_LINK_VISIBLE", OracleDbType.Varchar2,100);
                arrParams[12].Value = menu.RouteLinkVisible;

                DataTable dtInvData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DNET.Menus.INSERT_UPDATE, arrParams);
                apiResponse.Status = arrParams[10].Value.ToString();
                apiResponse.StatusDesc = arrParams[11].Value.ToString();
            }
            catch (Exception ex)
            {
                apiResponse.Status = Utilities.ERROR;
                apiResponse.StatusDesc = ex.ToString();
                throw;
            }
            return apiResponse;
        }

        public async Task<List<Menu>> ReadMenu(Menu menu)
        {
            List<Menu> lstMenu = new List<Menu>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[2];
                arrParams[0] = new OracleParameter("P_MENU_ID", OracleDbType.Varchar2);
                if (menu.MenuId == 0 || string.IsNullOrEmpty(Convert.ToString(menu.MenuId)))
                {
                    arrParams[0].Value = DBNull.Value;
                }
                else
                {
                    arrParams[0].Value = menu.MenuId;
                }
                arrParams[1] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[1].Direction = ParameterDirection.Output;

                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DNET.Menus.READ, arrParams);
                foreach (DataRow row in dtData.Rows)
                {
                    Menu _menu = new Menu();
                    _menu.MenuId = SafeTypeHandling.ConvertStringToInt32(row["MENU_ID"]);
                    _menu.MenuName = SafeTypeHandling.ConvertToString(row["MENU_NAME"]);
                    _menu.MenuDesc = SafeTypeHandling.ConvertToString(row["DESCRIPTION"]);
                    _menu.MenuParentId = SafeTypeHandling.ConvertStringToInt32(row["PARENT_MENU_ID"]);
                    _menu.MenuParentName = SafeTypeHandling.ConvertToString(row["PARENT_MENU_NAME"]);
                    _menu.MenuLevel = SafeTypeHandling.ConvertToString(row["MENU_LEVEL"]);
                    _menu.MenuURL = SafeTypeHandling.ConvertToString(row["MENU_URL"]);
                    _menu.DisplaySeq = SafeTypeHandling.ConvertStringToInt32(row["DISPLAY_SEQ"]);
                    _menu.Target = SafeTypeHandling.ConvertToString(row["TARGET"]);
                    _menu.Status = SafeTypeHandling.ConvertToString(row["STATUS"]);
                    _menu.RouteLinkVisible = SafeTypeHandling.ConvertToString(row["ROUTE_LINK_VISIBLE"]);
                    _menu.CreatedByName = SafeTypeHandling.ConvertToString(row["CREATED_BY"]);
                    _menu.CreatedDate = SafeTypeHandling.ConvertToDateTime(row["CREATION_DATE"]);
                    _menu.LastUpdatedByName = SafeTypeHandling.ConvertToString(row["LAST_UPDATED_BY"]);
                    _menu.LastUpdatedDate = SafeTypeHandling.ConvertToDateTime(row["LAST_UPDATED_DATE"]);
                    lstMenu.Add(_menu);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstMenu;
        }

        public async Task<APIResponse> SaveTaskMapping(List<Menu> tasks)
        {
            APIResponse apiResponse = new APIResponse();
            try
            {
                int counter = 0;
                foreach (Menu task in tasks)
                {
                    OracleParameter[] arrParams = new OracleParameter[7];
                    arrParams[0] = new OracleParameter("P_ROLEID", OracleDbType.Varchar2);
                    arrParams[0].Value = task.RoleId;
                    arrParams[1] = new OracleParameter("P_USERID", OracleDbType.Varchar2);
                    if (task.UserId == 0 || string.IsNullOrEmpty(Convert.ToString(task.UserId)))
                    {
                        arrParams[1].Value = DBNull.Value;
                    }
                    else
                    {
                        arrParams[1].Value = task.UserId;
                    }
                    arrParams[2] = new OracleParameter("P_TASK_ID", OracleDbType.Varchar2);
                    arrParams[2].Value = task.MenuId;
                    arrParams[3] = new OracleParameter("P_STATUS", OracleDbType.Varchar2);
                    arrParams[3].Value = task.TaskStatus;
                    arrParams[4] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2);
                    arrParams[4].Value = task.CreatedBy;
                    arrParams[5] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 500);
                    arrParams[5].Direction = ParameterDirection.Output;
                    arrParams[6] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                    arrParams[6].Direction = ParameterDirection.Output;

                    DataTable dtInvData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DNET.Menus.INSERT_UPDATE_TASK_MAPPING, arrParams);
                    counter++;
                }

                if (counter == 0)
                {
                    apiResponse.Status = Utilities.ERROR;
                    apiResponse.StatusDesc = "Record does not submitted";
                }
                else if (counter != tasks.Count)
                {
                    apiResponse.Status = Utilities.SUCCESS;
                    apiResponse.StatusDesc = "Record Partially Updated Successfully";
                }
                else
                {
                    apiResponse.Status = Utilities.SUCCESS;
                    apiResponse.StatusDesc = "Record Updated Successfully";
                }
            }
            catch (Exception ex)
            {
                apiResponse.Status = Utilities.ERROR;
                apiResponse.StatusDesc = ex.ToString();
                throw;
            }
            return apiResponse;
        }

        public async Task<List<Menu>> ReadTaskMapping(Menu menu)
        {
            List<Menu> lstTasks = new List<Menu>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[3];
                arrParams[0] = new OracleParameter("P_ROLEID", OracleDbType.Varchar2);
                arrParams[0].Value = menu.RoleId;
                arrParams[1] = new OracleParameter("P_USERID", OracleDbType.Varchar2);
                if (menu.UserId == 0 || string.IsNullOrEmpty(Convert.ToString(menu.UserId)))
                {
                    arrParams[1].Value = DBNull.Value;
                }
                else
                {
                    arrParams[1].Value = menu.UserId;
                }
                arrParams[2] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[2].Direction = ParameterDirection.Output;

                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DNET.Menus.READ_TASK_MAPPING, arrParams);
                foreach (DataRow row in dtData.Rows)
                {
                    Menu _menu = new Menu();
                    _menu.TaskMapId = SafeTypeHandling.ConvertStringToInt32(row["TASK_MAP_ID"]);
                    _menu.MenuId = SafeTypeHandling.ConvertStringToInt32(row["TASK_ID"]);
                    _menu.UserId = SafeTypeHandling.ConvertStringToInt32(row["USER_ID"]);
                    _menu.TaskStatus = SafeTypeHandling.ConvertToString(row["TASK_STATUS"]);
                    _menu.MenuName = SafeTypeHandling.ConvertToString(row["MENU_NAME"]);
                    _menu.MenuURL = SafeTypeHandling.ConvertToString(row["MENU_URL"]);
                    _menu.MenuParentId = SafeTypeHandling.ConvertStringToInt32(row["PARENT_MENU_ID"]);
                    _menu.RouteLinkVisible = SafeTypeHandling.ConvertToString(row["ROUTE_LINK_VISIBLE"]);
                    _menu.CreatedByName = SafeTypeHandling.ConvertToString(row["CREATED_BY"]);
                    _menu.CreatedDate = SafeTypeHandling.ConvertToDateTime(row["CREATED_DATE"]);
                    _menu.LastUpdatedByName = SafeTypeHandling.ConvertToString(row["LAST_UPDATE_BY"]);
                    _menu.LastUpdatedDate = SafeTypeHandling.ConvertToDateTime(row["LAST_UPDATE_DATE"]);
                    _menu.SubMenuCount = SafeTypeHandling.ConvertStringToInt32(row["SUB_MENU_COUNT"]);
                    lstTasks.Add(_menu);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstTasks;
        }

        public async Task<List<Menu>> ReadUserTaskAssign(Menu menu)
        {
            List<Menu> lstTasks = new List<Menu>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[3];
                arrParams[0] = new OracleParameter("P_ROLEID", OracleDbType.Varchar2);
                arrParams[0].Value = menu.RoleId;
                arrParams[1] = new OracleParameter("P_USERID", OracleDbType.Varchar2);
                if (menu.UserId == 0 || string.IsNullOrEmpty(Convert.ToString(menu.UserId)))
                {
                    arrParams[1].Value = DBNull.Value;
                }
                else
                {
                    arrParams[1].Value = menu.UserId;
                }
                arrParams[2] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[2].Direction = ParameterDirection.Output;

                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DNET.Menus.USER_TASK_ASSIGN_READ, arrParams);
                foreach (DataRow row in dtData.Rows)
                {
                    Menu _menu = new Menu();
                    _menu.TaskMapId = SafeTypeHandling.ConvertStringToInt32(row["TASK_MAP_ID"]);
                    _menu.MenuId = SafeTypeHandling.ConvertStringToInt32(row["TASK_ID"]);
                    _menu.UserId = SafeTypeHandling.ConvertStringToInt32(row["USER_ID"]);
                    _menu.TaskStatus = SafeTypeHandling.ConvertToString(row["TASK_STATUS"]);
                    _menu.MenuName = SafeTypeHandling.ConvertToString(row["MENU_NAME"]);
                    _menu.MenuURL = SafeTypeHandling.ConvertToString(row["MENU_URL"]);
                    _menu.MenuParentId = SafeTypeHandling.ConvertStringToInt32(row["PARENT_MENU_ID"]);
                    _menu.RouteLinkVisible = SafeTypeHandling.ConvertToString(row["ROUTE_LINK_VISIBLE"]);
                    _menu.CreatedByName = SafeTypeHandling.ConvertToString(row["CREATED_BY"]);
                    _menu.CreatedDate = SafeTypeHandling.ConvertToDateTime(row["CREATED_DATE"]);
                    _menu.LastUpdatedByName = SafeTypeHandling.ConvertToString(row["LAST_UPDATE_BY"]);
                    _menu.LastUpdatedDate = SafeTypeHandling.ConvertToDateTime(row["LAST_UPDATE_DATE"]);
                    _menu.SubMenuCount = SafeTypeHandling.ConvertStringToInt32(row["SUB_MENU_COUNT"]);
                    _menu.isChecked = SafeTypeHandling.ConvertStringToBoolean(row["IS_SELECTED"]);
                    lstTasks.Add(_menu);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstTasks;
        }


        public async Task<APIResponse> MenuUtilization(Menu menu)
        {
            APIResponse apiResponse = new APIResponse();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[8];
                arrParams[0] = new OracleParameter("OUT_ID", OracleDbType.Int32);
                arrParams[0].Direction = ParameterDirection.Output;
                arrParams[1] = new OracleParameter("P_APPLICATION", OracleDbType.Varchar2, 50);
                arrParams[1].Value = menu.ApplicationName;
                arrParams[2] = new OracleParameter("P_MENU", OracleDbType.Varchar2, 200);
                arrParams[2].Value = menu.MenuName;
                arrParams[3] = new OracleParameter("P_SUBMENU", OracleDbType.Varchar2, 200);
                arrParams[3].Value = menu.SubMenuName;
                arrParams[4] = new OracleParameter("P_INFOPAGE", OracleDbType.Varchar2, 1000);
                arrParams[4].Value = menu.MenuURL;
                arrParams[5] = new OracleParameter("P_ACCESSED_BY", OracleDbType.Varchar2, 100);
                arrParams[5].Value = menu.LoginID;
                arrParams[6] = new OracleParameter("P_CLIENT_IP", OracleDbType.Varchar2);
                arrParams[6].Value = menu.LocalIP;
                arrParams[7] = new OracleParameter("P_CREATED_BY", OracleDbType.Int32);
                arrParams[7].Value = menu.CreatedBy;

                DataTable dtInvData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DNET.Menus.MENU_UTILIZATION_LOG_INSERT, arrParams);
                apiResponse.Status = Utilities.SUCCESS;
                apiResponse.StatusDesc = "Menu utilization log generated successfully.";
            }
            catch (Exception ex)
            {
                apiResponse.Status = Utilities.ERROR;
                apiResponse.StatusDesc = ex.ToString();
                throw;
            }
            return apiResponse;
        }

    }
}
