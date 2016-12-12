using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
//using System.Drawing;
using System.Globalization;
using PdfToImage;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using BAL;
using Controller;
using System.Web.Services;
using DAL;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Generic;
using System.Net;
//using System.Net.Mail;
using System.Web.Mail;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System.Reflection;
using System.Data.SqlClient;
using System.Configuration;
using System.Diagnostics;


//using Microsoft.Office.Interop.Outlook;
//using System.Reflection;     // to use Missing.Value
//using Microsoft.Office;
//using Microsoft.Office.Core;
//using System.Runtime.InteropServices;
//using Outlook = Microsoft.Office.Interop.Outlook;

public partial class default_copy : System.Web.UI.Page
{
    //Given By Mukesh on 06/07/2015
    #region [Global Declaration]
    CommonBAL objCommonBAL;
    FolderBAL objFolderBAL;
    FileUploadBAL objFileUploadBAL;
    FileUploadController objFileUploadController;
    DataSet dsExport = new DataSet();
    FolderController FolderControl;
    static int R = 0;
    static int W = 0;
    static string strFileName = string.Empty;
    DataTable dtDeniePath = new DataTable();
    OperationClass operation = new OperationClass();
    String delivery = "";
    string AccessDeniePath = "";
    #endregion




    protected void Page_PreInit(Object sender, EventArgs e)
    {
        if (Convert.ToString(Session["GroupName"]).ToLower() == "directors" || Convert.ToString(Session["Groupname"]).ToLower() == "general manager" || Convert.ToString(Session["Groupname"]).ToLower() == "senior management" || Convert.ToString(Session["Groupname"]).ToLower() == "functional management" || Convert.ToString(Session["Groupname"]).ToLower() == "others")
        {
            this.MasterPageFile = "~/MasterPage/MasterPage2.master";

        }
        else
        {
            this.MasterPageFile = "~/MasterPage/MasterPage.master";
        }


    }




    #region [Page_Load]
    protected void Page_Load(object sender, EventArgs e)
    {



        //GenericList<int> list1 = new GenericList<int>();


        //GenericList<string> list2 = new GenericList<string>();


        //var dd = typeof(CommonBAL);

        //foreach (var dddm in dd.GetMethods())    
        //{
        //   var ddee =dddm.Name;
        //}


        //Type type1 = typeof(CommonBAL);
        //object obj = Activator.CreateInstance(type1);
        //object[] mParam = new object[] { "5", 10 };
        //int res = (int)type1.InvokeMember("UpdatePwdAttempt", BindingFlags.InvokeMethod,
        //                                   null, obj, mParam);
        //Console.Write("Result: {0} \n", res);



        ///for testing github
        Response.Clear();
        objCommonBAL = new CommonBAL();
        Session["TempFileID"] = "";

        if (Session["UserName"] != null)
        {
            if (!Page.IsPostBack)
            {


                ////Stuff some test data into the session.  We will access this later to test the session.
                //Session["StartupTime"] = DateTime.Now;

                ////Show the user what we put in there.
                //lblStartupTime.Text = "Session Start Time: " + Session["StartupTime"].ToString();
                //lblCurrentTime.Text = "Current Time: " + DateTime.Now.ToString();
                //Singleton s1 = Singleton.Instance();
                //Singleton s2 = Singleton.Instance();

                //if (s1 == s2)
                //{
                //    Console.WriteLine(obj.param1 + " " + obj.param2);
                //}
                BindDropdown();
                Session["WorkFlow"] = null;
                txtSort.Text = "FileName";
                txtOder.Text = "Asc";


                try
                {
                    if (HttpContext.Current.Session["myGVPageId"] != null)
                    {
                        gvData.PageIndex = Convert.ToInt32(HttpContext.Current.Session["myGVPageId"]);
                        Session["myGVPageId"] = null;
                    }
                }
                catch (Exception ex)
                {
                    // log it
                }
                DataTable dtfiledetails1 = null;
                OperationClass operation = new OperationClass();
                string TableName = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Convert.ToInt32(Session["FolderID"])));
                dtfiledetails1 = operation.GetTable4Command(
                string.Format(@"select a.fileid,FileName,ApprovalStatus,withdrawcomments,Column0 'Itemno',column1 'Particulars',column2 'Purpose',column3 'GM',b.PageCount from {0} a,tblfile b where a.fileid =b.fileid and b.folderid={1}  and column1 is not null order by column0", TableName, Convert.ToInt32(Session["FolderID"])));

                if (dtfiledetails1 != null && dtfiledetails1.Rows.Count > 0)
                {
                    string In_Attendance = operation.ExecuteScalar4Command(string.Format(@"select Value from tblConfig where keys='In Attendance'"));
                    if (In_Attendance.ToLower() == "yes")
                    {
                        if (Convert.ToInt32(Session["ParentFolderId"].ToString()) != 1 && Request.QueryString["Value"] != "home")
                        {
                            string InAttendance = operation.ExecuteScalar4Command(string.Format(@"select InAttendance from tblFolder where FolderId={0}", Session["FolderId"].ToString()));
                            if (InAttendance != null || InAttendance != "")
                            {
                                lblinattendancelbl.Visible = true;
                                tblinattendance.Visible = true;
                                lblinattendancetext.Visible = true;
                                lblinattendancetext.Text = InAttendance;
                            }
                            else
                            {
                                lblinattendancelbl.Visible = false;
                                lblinattendancetext.Visible = false;
                            }
                            string Invitees = operation.ExecuteScalar4Command(string.Format(@"select Invitees from tblFolder where FolderId={0}", Session["FolderId"].ToString()));
                            if (Invitees != null || Invitees != "")
                            {
                                lblinviteeslbl.Visible = true;
                                tblinvitees.Visible = true;
                                lblinviteestext.Visible = true;
                                lblinviteestext.Text = Invitees;
                            }
                            else
                            {
                                lblinviteeslbl.Visible = false;
                                lblinviteestext.Visible = false;
                            }
                        }
                    }
                    else
                    {
                        lblinattendancelbl.Visible = false;
                        lblinattendancetext.Visible = false;
                        lblinviteeslbl.Visible = false;
                        lblinviteestext.Visible = false;
                    }

                }
                if (Session["FolderName"] != null)
                {

                    string commitee = Session["FolderName"].ToString();
                    if (commitee != "")
                    {
                        string[] commitees = commitee.Split('\\');
                        if (commitees.Length > 1)
                        {

                            if (commitees[1].ToLower() != "archived meetings" || commitees[1].ToLower() != "repository")
                            {
                                Session["DCommiteename"] = commitees[1].ToString();
                                if (commitees.Count() > 1)
                                {
                                    if (commitees[1] != "")
                                    {

                                        lblhdcommitee.Text = commitees[1].ToString();
                                    }

                                }
                                if (commitees.Count() > 2)
                                {
                                    if (commitees[2] != "")
                                    {
                                        Session["DMeetingdate"] = commitees[2].ToString();
                                        lblhdmeetingdate.Text = commitees[2].ToString();
                                    }
                                }
                            }
                            else if (commitees[1].ToLower() == "archived meetings" || commitees[1].ToLower() != "repository")
                            {
                                if (commitees.Count() > 2)
                                {
                                    if (commitees[2] != "")
                                    {

                                        lblhdcommitee.Text = commitees[2].ToString();
                                    }

                                }
                                if (commitees.Count() > 3)
                                {
                                    if (commitees[3] != "")
                                    {
                                        lblhdmeetingdate.Text = commitees[3].ToString();
                                    }
                                }
                            }
                        }
                    }
                }
                lblMessage.Text = "";
                Session["CHECKED_ITEMS_DocumentListing"] = null;
                if (Convert.ToString(Session["FolderID"]) != "")
                {
                    /*GetPNode function is used to get current folder id and all its child folder id
                    if a parent have less access then a child then parent access permissions will be provided to its all child
                    else if parent have full access then permissions will be provided as per child's own access permissions*/
                    //if repository folder seleted then
                    if (Convert.ToInt32(Session["FolderID"]) == 1)
                        return;

                    Session["IDS"] = Convert.ToInt32(Session["FolderID"]);

                    bool AccessRight = objCommonBAL.GetPNode(Convert.ToInt32(Session["FolderID"]));
                    if (AccessRight)
                    {
                        //btnDelete.Visible = false;
                        btnExport.Visible = false;
                        btnSMS.Visible = false;
                        //btnMoveFile.Visible = false;
                        //lblSelect.Visible = false;
                        //lnkSelectAll.Visible = false;
                        //LinkButton2.Visible = false;
                        //btnCopyFiles.Visible = false;
                        return;
                    }

                }


                //Bind grid view as per access permissions
                if (Session["FolderID"] != null && Session["ParentFolderID"] != null && Session["ParentFolderID"] != null)
                {
                    if (Convert.ToInt32(Session["FolderID"].ToString()) != 0 && Convert.ToInt32(Session["ParentFolderID"].ToString()) != 1 && Request.QueryString["Value"] != "home" && Request.QueryString["Value"] != "cancel")
                    {
                        OperationClass objOperationClass = new OperationClass();
                        string str = objOperationClass.ExecuteScalar4Command(string.Format(@"select FolderName from tblfolder where FolderId=" + Session["FolderId"].ToString()));
                        if (str.ToLower() == "mom")
                        {
                            btnDelete.Visible = true;
                            Bindgridview();
                            btnFirstSeperator.Visible = false;
                            btnLastSeperator.Visible = false;
                            txtColor.Visible = false;
                            lblRestricted.Visible = false;
                            Bindgridview();
                            btnExport.Visible = false;
                            btnSMS.Visible = false;
                            btnAuthorizeAll.Visible = false;
                            btnnotebook.Visible = false;
                            chkSelectalll.Visible = false;
                            upFileView.Visible = false;
                            btnMoveDown.Visible = false;
                            btnInvitee.Visible = false;
                            btnPublish.Visible = false;
                            txtColor.Visible = false;
                            lblRestricted.Visible = false;
                            ((Control)Master.FindControl("ddlPageNumber")).Visible = false;

                        }
                        else
                        {

                            if (operation.ExecuteScalar4Command(string.Format(@"select parentfolderid from tblFolder where folderid={0}", Convert.ToInt32(Session["FolderID"]))) == "1")
                                bindLvFileView();
                            else
                                DirectorDataBinding();
                        }
                    }
                    else
                        if (Convert.ToInt32(Session["ParentFolderID"].ToString()) == 1 && (Session["FolderName"].ToString().ToLower() == "meetings\\my briefcase" || Session["FolderName"].ToString().ToLower() == "meetings\\company info" || Session["FolderName"].ToString().ToLower() == "meetings\\shared documents"))
                        {
                            DirectorDataBinding();
                        }
                        else
                        {
                            btnExport.Visible = false;
                            btnSMS.Visible = false;
                            btnDelete.Visible = false;
                            btnAuthorizeAll.Visible = false;
                            tblMeetingInformation.Visible = false;
                            chkSelectalll.Visible = false;
                            btnnotebook.Visible = false;
                            btnMoveDown.Visible = false;
                            btnInvitee.Visible = false;
                            btnPublish.Visible = false;
                            btnFirstSeperator.Visible = false;
                            btnLastSeperator.Visible = false;
                            txtColor.Visible = false;
                            lblRestricted.Visible = false;


                        }
                }
                if (Session["ParentFolderID"] != null)
                {
                    if (Convert.ToInt32(Session["ParentFolderID"].ToString()) == 1)
                    {
                        if (Session["FolderName"].ToString().ToLower() == "meetings\\my briefcase" || Session["FolderName"].ToString().ToLower() == "meetings\\company info" || Session["FolderName"].ToString().ToLower() == "meetings\\shared documents")
                        {
                            lblblankmessage.Visible = false;
                        }
                        else
                        {
                            lblblankmessage.Visible = true;
                        }
                    }
                }

                if (Request.QueryString["Value"] == "cancel")
                {
                    pnlCancel.Visible = true;
                }



                if (Request.QueryString["Value"] != null && Request.QueryString["Value"] != "meeting")
                {
                    if (Request.QueryString["Value"].ToString().ToLower() == "my briefcase")
                    {

                        // ((Control)Master.FindControl("liUpload")).Visible = true;
                        gvData.EmptyDataText = "No Documents";
                        DirectorDataBinding();
                    }
                    if (Request.QueryString["Value"].ToString().ToLower() == "company info")
                    {
                        if (Session["GroupName"].ToString().ToLower() == "directors")
                        {
                            gvData.EmptyDataText = "No Documents";
                            //((Control)Master.FindControl("liUploadInfo")).Visible = false;
                        }
                        else
                        {
                            gvData.EmptyDataText = "No Documents";
                            //((Control)Master.FindControl("liUploadInfo")).Visible = true;
                        }

                        DirectorDataBinding();
                    }


                    if (Request.QueryString["Value"] == "home")
                    {
                        btnExport.Visible = false;
                        btnSMS.Visible = false;
                        // btnDelete.Visible = false;
                        btnAuthorizeAll.Visible = false;
                        chkSelectalll.Visible = false;
                        tblMeetingInformation.Visible = false;
                        btnnotebook.Visible = false;
                        btnMoveDown.Visible = false;
                        btnInvitee.Visible = false;
                        btnPublish.Visible = false;
                        btnFirstSeperator.Visible = false;
                        btnLastSeperator.Visible = false;
                        txtColor.Visible = false;
                        lblRestricted.Visible = false;


                    }
                }

                //Added by Kirti S Loke

                //btnCopyFiles.Visible = false;
                //btnMoveFile.Visible = false;
                //btnExport.Visible = false;
                if (Session["FolderName"].ToString().ToLower() == "meetings\\binani cement")
                {
                    if (Convert.ToString(Session["Groupname"]).ToLower().Trim() == "office secretary" || Convert.ToString(Session["Groupname"]).ToLower().Trim() == "admin")
                    {
                        GridView2.Visible = false;
                        GridView1.Visible = false;
                        lblblankmessage.Visible = false;
                        lblApproval.Visible = false;
                        btnAuthorize.Visible = false;
                    }
                    else
                    {
                        btnMoveFile.Visible = false;
                        lblblankmessage.Visible = false;
                        lblApproval.Visible = true;
                        btnAuthorize.Visible = true;
                        GetAgendaDetails();
                    }
                }
                else
                    if (Session["FolderName"].ToString().ToLower() == "meetings\\binani industries")
                    {
                        if (Convert.ToString(Session["Groupname"]).ToLower().Trim() == "office secretary" || Convert.ToString(Session["Groupname"]).ToLower().Trim() == "admin")
                        {
                            GridView2.Visible = false;
                            GridView1.Visible = false;
                            lblblankmessage.Visible = false;
                            lblApproval.Visible = false;
                            btnAuthorize.Visible = false;
                        }
                        else
                        {
                            btnMoveFile.Visible = false;
                            lblblankmessage.Visible = false;
                            lblApproval.Visible = true;
                            btnAuthorize.Visible = true;
                            GetAgendaDetailsforBoard();
                        }

                    }

                    else
                    {
                        GridView2.Visible = false;
                        GridView1.Visible = false;
                        // chkSelectalll.Visible = false;
                    }
            }
            else
            {
                if (Session["FolderName"].ToString().ToLower() == "meetings\\binani cement")
                {
                    btnMoveFile.Visible = false;
                    btnAuthorize.Visible = true;
                }
            }

        }
        else
        {

            Session.Abandon();
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Your session has expired')", true);
        }

    }
    #endregion




    protected void Bindgridview()
    {

        OperationClass operation = new OperationClass();
        string TableName = operation.ExecuteScalar4Command(string.Format("select tablename from tblFolderIndexMaster where folder_id={0}", Convert.ToInt32(Session["FolderID"])));
        string ParentFolderID = operation.ExecuteScalar4Command(string.Format("select ParentFolderID from tblFolder where folderid='{0}'", Convert.ToInt32(Session["FolderID"])));
        //DataTable dtfiledetails = operation.GetTable4Command(
        //string.Format("select a.fileid,a.DocStatus,FileName,FileSize,ApprovalStatus,withdrawcomments,Column0 'Itemno',column1 'Noteno',column2 'subject',column3 'lotno',convert(varchar(12),column4,103) 'meettingdate' from {0} a,tblfile b where a.fileid =b.fileid and b.folderid={1} and Column0 is null", TableName, Convert.ToInt16(Session["FolderID"])));
        DataTable dtfiledetails = operation.GetTable4Command(
        string.Format("select a.MId,a.FileID,d.foldername 'Meeting Name',(select foldername from tblfolder where folderid=d.parentfolderid) 'Committee Name',"
+ " a.FolderId,b.filename,b.pagecount,a.description as Description "
+ " from tblMOMApproved a inner join tblfile b on a.fileid=b.fileid inner join tblfolder d on a.folderid=d.folderid where a.ParentFolderId='{0}' order by a.FolderID desc", ParentFolderID));


        if (dtfiledetails != null)
        {
            if (dtfiledetails.Rows.Count > 0)
            {
                gvFielSearch.DataSource = null;
                gvFielSearch.DataSource = dtfiledetails;
                gvFielSearch.DataBind();

            }
        }


    }



    protected void gvFielSearch_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        //if (e.Row.RowType == DataControlRowType.DataRow)
        //{

        //    LinkButton lnkCompleted = (LinkButton)e.Row.FindControl("lnkCompleted");
        //    HiddenField hdDocstatus = (HiddenField)e.Row.FindControl("hdDocstatus");
        //    if (Session["GroupName"].ToString().ToLower() == "directors" || Convert.ToString(Session["Groupname"]).ToLower() == "general manager" || Convert.ToString(Session["Groupname"]).ToLower() == "senior management" || Convert.ToString(Session["Groupname"]).ToLower() == "functional management" || Convert.ToString(Session["Groupname"]).ToLower() == "others")
        //    {
        //        if (hdDocstatus.Value == "1")
        //        {
        //            e.Row.Visible = false;
        //        }
        //    }
        //    else
        //    {
        //        if (hdDocstatus.Value == "1")
        //        {
        //            e.Row.BackColor = System.Drawing.Color.Gray;
        //            lnkCompleted.Text = "MoM Completed";
        //        }
        //    }

        //}


    }
    public void btnnotification_Click(object sender, EventArgs e)
    {
        string Result = "";
        DataCon dtcon = new DataCon();
        string ParentFolderName = operation.ExecuteScalar4Command(string.Format(@"select FolderName  from tblFolder where folderId in (select ParentFolderId from tblFolder where folderId={0})", Convert.ToInt32(Session["FolderID"])));
        string FolderName = operation.ExecuteScalar4Command(string.Format(@"select FolderName from tblFolder where folderId={0}", Convert.ToInt32(Session["FolderID"])));
        DataTable PushWooshHWID = operation.GetTable4Command(string.Format(@"select PushWooshHWID from tblDeviceMaster where UserId in(SELECT DISTINCT USERID  FROM  tblUserAccessControl WHERE FOLDERID='{0}' and accessSymbol in('F','M','R','L') and USERID IN (SELECT DISTINCT USERID FROM tblUserMaster where status=1)) and tblDeviceMaster.Active='y'", Convert.ToInt32(Session["FolderID"])));
        string Message = "Agenda has been uploaded in the meeting of " + FolderName + " of " + ParentFolderName;
        foreach (DataRow dr in PushWooshHWID.Rows)
        {
            if (dr["PushWooshHWID"].ToString() != "")
            {
                Result = dtcon.SendPushNotifications(dr["PushWooshHWID"].ToString(), Message);
                int i = 1;
            }
        }
        if (Result != "")
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Agenda Upload Notification Sent Successfully')", true);
        }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Registered Device Not Found')", true);
        }

    }
    protected void gvFielSearch_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {

    }
    protected void gvFielSearch_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Viewmom")
        {
            //if user does not have file view permission then a message will be displayed. You don't have access to view these files.
            string[] FileNameID = e.CommandArgument.ToString().Split(new Char[] { ',' });

            Session["FileID"] = FileNameID[0].ToString();
            Session["FileName"] = FileNameID[1].ToString();
            if (Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".tif" || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".tiff"
                || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".gif" || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".bmp"
                || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".jpg" || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".jpeg" || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".png")
            {
                // StringBuilder sbwindow = new StringBuilder();
                //sbwindow.Append("window.showModalDialog('../Viewer/ImageViewer_New.aspx',null,'status:no;dialogTop:300;dialogWidth:1024px;dialogHeight:800px;dialogHide:true;help:no;scroll:yes;center:yes');");
                // sbwindow.Append("window.open('../Viewer/ImageViewer_New.aspx',null,'scrollbars:no;width=1024px, height=800px;help:no;center:yes; resizable=yes');");
                //Response.Redirect("../Viewer/ImageViewer_New.aspx", false);
                Response.Redirect("../Viewer/Thumbnailmom.aspx", false);
                //Response.Redirect("../Viewer/frmsectionviewer.aspx", false);

                //ScriptManager.RegisterClientScriptBlock(this.Page, typeof(UpdatePanel), "ImageViewer", sbwindow.ToString(), true);
                //sbwindow = null;
            }
            else if (Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".wmv")
            {
                StringBuilder sbwindow = new StringBuilder();
                sbwindow.Append("window.showModalDialog('../Viewer/WMVViwer.aspx',null,'status:no;dialogTop:300;dialogWidth:1014px;dialogHeight:700px;dialogHide:true;help:no;scroll:no;center:yes');");
                ScriptManager.RegisterClientScriptBlock(this.Page, typeof(UpdatePanel), "VideoViewer", sbwindow.ToString(), true);
                sbwindow = null;
            }
            else if (Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".pdf" || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".docx" || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".pptx" || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".xlsx")
            {
                // StringBuilder sbwindow = new StringBuilder();
                // sbwindow.Append("window.showModalDialog('../Viewer/ImageViewer_New.aspx',null,'status:no;dialogTop:300;dialogWidth:1024px;dialogHeight:800px;dialogHide:true;help:no;scroll:yes;center:yes');");
                //sbwindow.Append("window.open('../Viewer/ImageViewer_New.aspx',null,'scrollbars:no;width=1024px, height=800px;help:no;center:yes; resizable=yes');");
                //Response.Redirect("../Viewer/ImageViewer_New.aspx", false);
                Response.Redirect("../AddRenameFolder/ThumbnailMOMM.aspx?Value=12", false);
                //ScriptManager.RegisterClientScriptBlock(this.Page, typeof(UpdatePanel), "ImageViewer", sbwindow.ToString(), true);
                //sbwindow = null;
                //sbwindow.Append("window.open('PDFViewer.aspx',null,'scrollbars:no;width=1024px, height=800px;help:no;center:yes; resizable=yes');");

                //sbwindow.Append("window.showModalDialog('../PDFViewer.aspx',null,'status:no;dialogTop:300;dialogWidth:1024px;dialogHeight:800px;dialogHide:true;help:no;scroll:yes;center:yes');");
                //ScriptManager.RegisterClientScriptBlock(this.Page, typeof(UpdatePanel), "ImageViewer", sbwindow.ToString(), true);
                //sbwindow = null;
            }
            else if (Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".zip" || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".rar" || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".zipx")
            {
                ScriptManager.RegisterStartupScript(this.Page, typeof(UpdatePanel), "msg", "alert('Please download file, then view on your local')", true);
                return;
            }

            else
            {
                StringBuilder sbwindow = new StringBuilder();
                sbwindow.Append("window.open('../Viewer/OfficerViewer.aspx',null,'scrollbars:no;width=1024px, height=800px;help:no;center:yes; resizable=yes');");
                // sbwindow.Append("window.showModelessDialog('Viewer/OfficerViewer.aspx',null,'status:no;dialogTop:300;dialogWidth:1024px;dialogHeight:800px;dialogHide:true;help:no;scroll:yes;center:yes');");
                ScriptManager.RegisterClientScriptBlock(this.Page, typeof(UpdatePanel), "DocumentViewer", sbwindow.ToString(), true);
                sbwindow = null;
            }
        }
        if (e.CommandName == "Delete")
        {
            OperationClass objOperationClass = new OperationClass();
            //if user does not have file view permission then a message will be displayed. You don't have access to view these files.
            string[] FileNameID = e.CommandArgument.ToString().Split(new Char[] { ',' });


            int value = objOperationClass.ExecuteNonQuery(string.Format(@"delete from tblmomapproved where fileid='{0}'", FileNameID[0].ToString()));
            if (value == 1)
            {
                Bindgridview();
                Response.Redirect("../Default/Default.aspx");

            }


        }
        if (e.CommandName == "Completedmom")
        {

            string[] FileNameID = e.CommandArgument.ToString().Split(new Char[] { ',' });

            Session["FileID"] = FileNameID[0].ToString();
            Session["FileName"] = FileNameID[1].ToString();
            OperationClass objOperationClass = new OperationClass();
            string tablename = objOperationClass.ExecuteScalar4Command(string.Format(@"select TableName from tblFolderIndexMaster where Folder_Id={0}", Session["FolderId"].ToString()));
            int value = objOperationClass.Insert4Command(string.Format(@"update {0} set DocStatus=1 where fileid={1}", tablename, Session["FileID"]));
            if (value == 1)
            {
                ScriptManager.RegisterStartupScript(this.Page, typeof(UpdatePanel), "msg", "alert('Mom Completed')", true);
            }

        }


    }


    protected void GetAgendaDetailsforBoard()
    {

        btnMoveFile.Visible = false;
        gvData.Visible = false;
        //chkSelectEmailId.Visible = false;
        //lblChkEmailId.Visible = false;
        gvParent.Visible = false;
        DataTable dtAgenda = null;
        DataTable dtAgendatablename = null;
        DataTable dtStepsno = null;
        if (Convert.ToString(Session["Groupname"]).ToLower().Trim() == "president")
        {
            string strtablename = operation.ExecuteScalar4Command("select TableName from tblfolderindexmaster where folder_id=(select FolderID from tblfolder where foldername='binani industries')");
            dtAgenda = operation.GetTable4Command(string.Format(@"select distinct b.Id,b.FileId,d.column0 'Subject',0'stepsno',b.StepStatus,a.filename,b.reason from tblfile a inner join tblAgendaApprovalDetail b on a.fileid=b.fileid inner join tblAgendaLevelDetail c on c.folderid=b.ParentFolderId inner join {0} d on d.fileid=a.fileid where FinalAgendaApprovalStatus=1", strtablename));
            if (dtAgenda != null && dtAgenda.Rows.Count > 0)
            {
                GridView2.DataSource = dtAgenda;
                GridView2.DataBind();
                lblItems.Visible = true;
                btnDistributeAll.Visible = true;
            }
            else
            {
                GridView2.DataSource = null;
                GridView2.DataBind();
                btnDistributeAll.Visible = false;
            }

            dtStepsno = operation.GetTable4Command(string.Format(@"select stepsno from tblAgendaLevelDetail where  userid={0} and Folderid=(select Folderid from tblfolder where foldername='binani industries')", Convert.ToInt32(Session["UserID"])));
            string strStepsno = "";
            if (dtStepsno != null && dtStepsno.Rows.Count > 0)
            {
                for (int i = 0; i < dtStepsno.Rows.Count; i++)
                {

                    strStepsno = strStepsno + dtStepsno.Rows[i]["stepsno"].ToString() + ",";
                }
            }
            if (strStepsno != "")
            {

                string strAgendatablename = operation.ExecuteScalar4Command("select TableName from tblfolderindexmaster where folder_id=(select FolderID from tblfolder where foldername='binani industries')");
                //dtAgenda = operation.GetTable4Command(string.Format(@"select b.Id,b.FileId,d.column0 'Subject',c.stepsno,b.StepStatus,a.filename,c.userid,b.reason from tblfile a inner join tblAgendaApprovalDetail b on a.fileid=b.fileid   inner join tblAgendaLevelDetail c on c.folderid=b.ParentFolderId inner join {0} d on d.fileid=a.fileid where c.userid={1} and b.StepStatus in ({2}) and  b.FinalAgendaApprovalStatus is null", strtablename, Convert.ToInt32(Session["UserID"]), strStepsno.Substring(0, strStepsno.LastIndexOf(","))));
                dtAgendatablename = operation.GetTable4Command(string.Format(@"select b.Id,b.WorkFlowAgendaApproval_Id,c.WorkFlowAgenda_Id,b.FileId,d.column0 'Subject',c.stepsno,b.StepStatus,a.filename,c.userid,b.reason,b.Status from tblfile a inner join tblAgendaApprovalDetail b on a.fileid=b.fileid inner join tblAgendaLevelDetail c on c.folderid=b.ParentFolderId inner join {0} d on d.fileid=a.fileid where c.userid={1} and b.StepStatus in ({2}) and  b.FinalAgendaApprovalStatus is null and b.WorkFlowAgendaApproval_Id=c.WorkFlowAgenda_Id", strAgendatablename, Convert.ToInt32(Session["UserID"]), strStepsno.Substring(0, strStepsno.LastIndexOf(","))));

                if (dtAgendatablename != null && dtAgendatablename.Rows.Count > 0)
                {
                    GridView1.DataSource = dtAgendatablename;
                    GridView1.DataBind();
                }
                else
                {
                    GridView1.DataSource = null;
                    GridView1.DataBind();
                    btnAuthorize.Visible = false;
                }
            }
        }
        else
        {
            dtStepsno = operation.GetTable4Command(string.Format(@"select stepsno from tblAgendaLevelDetail where  userid={0} and Folderid=(select Folderid from tblfolder where foldername='binani industries')", Convert.ToInt32(Session["UserID"])));
            string strStepsno = "";
            if (dtStepsno != null && dtStepsno.Rows.Count > 0)
            {
                for (int i = 0; i < dtStepsno.Rows.Count; i++)
                {

                    strStepsno = strStepsno + dtStepsno.Rows[i]["stepsno"].ToString() + ",";
                }
            }
            if (strStepsno != "")
            {

                string strtablename = operation.ExecuteScalar4Command("select TableName from tblfolderindexmaster where folder_id=(select FolderID from tblfolder where foldername='binani industries')");
                //dtAgenda = operation.GetTable4Command(string.Format(@"select b.Id,b.FileId,d.column0 'Subject',c.stepsno,b.StepStatus,a.filename,c.userid,b.reason from tblfile a inner join tblAgendaApprovalDetail b on a.fileid=b.fileid   inner join tblAgendaLevelDetail c on c.folderid=b.ParentFolderId inner join {0} d on d.fileid=a.fileid where c.userid={1} and b.StepStatus in ({2}) and  b.FinalAgendaApprovalStatus is null", strtablename, Convert.ToInt32(Session["UserID"]), strStepsno.Substring(0, strStepsno.LastIndexOf(","))));
                dtAgenda = operation.GetTable4Command(string.Format(@"select b.Id,b.WorkFlowAgendaApproval_Id,c.WorkFlowAgenda_Id,b.FileId,d.column0 'Subject',c.stepsno,b.StepStatus,a.filename,c.userid,b.reason,b.Status from tblfile a inner join tblAgendaApprovalDetail b on a.fileid=b.fileid inner join tblAgendaLevelDetail c on c.folderid=b.ParentFolderId inner join {0} d on d.fileid=a.fileid where c.userid={1} and b.StepStatus in ({2}) and  b.FinalAgendaApprovalStatus is null and b.WorkFlowAgendaApproval_Id=c.WorkFlowAgenda_Id", strtablename, Convert.ToInt32(Session["UserID"]), strStepsno.Substring(0, strStepsno.LastIndexOf(","))));

                if (dtAgenda != null && dtAgenda.Rows.Count > 0)
                {
                    GridView1.DataSource = dtAgenda;
                    GridView1.DataBind();
                }
                else
                {
                    GridView1.DataSource = null;
                    GridView1.DataBind();
                    btnAuthorize.Visible = false;
                }
            }
        }
    }
    //public void btnAgendalevelNotebook_Click(object sender, EventArgs e)
    //{
    //    Response.Redirect("../AccessControl/AgendaLevelNoteBook.aspx", false);
    //}


    protected void btncommentsave_Click(object sender, EventArgs e)
    {

        if (ViewState["FirstSelectedd"] == null && ViewState["LastSelectedd"] == null)
        {
            GenericDAL objDAL = new GenericDAL();
            string comments = txtcomments.Text;
            int success = 0;

            OperationClass objOperationClass = new OperationClass();

            DataTable dtAttachmentIds = objOperationClass.GetTable4Command(string.Format(@"select attachmentid  from tblattachment where fileid = {0}", Convert.ToInt32(Session["FileId"])));
            string fileids = Session["FileId"].ToString();

            fileids = fileids + ",";
            if (dtAttachmentIds != null && dtAttachmentIds.Rows.Count > 0)
            {
                for (int i = 0; i < dtAttachmentIds.Rows.Count; i++)
                {
                    if (i == dtAttachmentIds.Rows.Count - 1)
                    {
                        fileids = fileids + dtAttachmentIds.Rows[i]["attachmentid"].ToString();
                    }
                    else
                    {
                        fileids = fileids + dtAttachmentIds.Rows[i]["attachmentid"].ToString() + ",";
                    }
                }
            }
            else
            {
                fileids = fileids.Substring(0, fileids.LastIndexOf(","));
            }


            success = (int)objDAL.ExecuteNonQuery("Update tblFile set ApprovalStatus='5' , withdrawcomments='" + comments + "' where FileId in (" + fileids + ")");
            if (success > 0)
            {
                Reject(comments);
                pncomment.Visible = false;
                DirectorDataBinding();

            }
        }
        else
        {
            string ItemNo = "";
            string strMeetingDate = "";
            string strrr = "";
            string TableName = "";
            DataTable dtfiledetailsnew = null;
            DateTime? MeettingDate = null;
            DataTable DtImageUploaded = null;
            if (ViewState["FirstSelectedd"] != null && W != 1)
            {

                strMeetingDate = Convert.ToString(Session["FolderName"]).Substring(Session["FolderName"].ToString().LastIndexOf("\\") + 1);
                strrr = ViewState["FirstSelectedd"].ToString();
                TableName = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Convert.ToInt32(Session["FolderID"])));
                dtfiledetailsnew = operation.GetTable4Command(string.Format(@"select  FileID,substring(Column0,0,3) as Colun,Column0 from {0} where Column0>'{1}' and Column0 not like '{2}%' and folderid={3} order by Column0", TableName, strrr, strrr, Convert.ToInt32(Session["FolderID"])));
                if (Convert.ToInt32(strrr) < 10)
                {
                    ItemNo = ("0" + (Convert.ToInt32(strrr)));
                }
                else
                {
                    ItemNo = "" + (Convert.ToInt32(strrr));
                }

                MeettingDate = DateTime.ParseExact(strMeetingDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                ViewState["FieldValues"] = "'" + txtcomments.Text.ToString().Replace("'", "''").Trim() + "','" + ItemNo + "','" + MeettingDate + "','" + "" + "'";
                ViewState["FieldNames"] = "Column1,Column0,Column4,Column3";


                DtImageUploaded = SetBussinessentityies("", "", null, "", "", "3",
                                                            "", "", 0, "no", TableName);
                W++;

            }
            if (ViewState["LastSelectedd"] != null && W != 1)
            {

                strMeetingDate = Convert.ToString(Session["FolderName"]).Substring(Session["FolderName"].ToString().LastIndexOf("\\") + 1);


                strrr = ViewState["LastSelectedd"].ToString(); ;
                TableName = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Convert.ToInt32(Session["FolderID"])));
                dtfiledetailsnew = operation.GetTable4Command(string.Format(@"select  FileID,substring(Column0,0,3) as Colun,Column0 from {0} where Column0>'{1}' and Column0 not like '{2}%' and folderid={3} order by Column0", TableName, strrr, strrr, Convert.ToInt32(Session["FolderID"])));
                if (Convert.ToInt32(strrr) < 9)
                {
                    ItemNo = ("0" + (Convert.ToInt32(strrr) + 1));
                }
                else
                {
                    ItemNo = "" + (Convert.ToInt32(strrr) + 1);
                }

                MeettingDate = DateTime.ParseExact(strMeetingDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                ViewState["FieldValues"] = "'" + txtcomments.Text.ToString().Replace("'", "''").Trim() + "','" + ItemNo + "','" + MeettingDate + "','" + "" + "'";
                ViewState["FieldNames"] = "Column1,Column0,Column4,Column3";


                DtImageUploaded = SetBussinessentityies("", "", null, "", "", "3",
                                                                    "", "", 0, "no", TableName);
                W++;

            }
            DirectorDataBinding();
            pncomment.Visible = false;
            txtcomments.Text = "";
            ViewState["FirstSelectedd"] = null;
            ViewState["LastSelectedd"] = null;
            ViewState["FileIds"] = null;
        }

    }

    public void Reject(string Comment)
    {
        DataTable dtComments = null;
        DataTable dtUserId = null;
        try
        {

            string ParentFolderName = operation.ExecuteScalar4Command(string.Format(@"select FolderName  from tblFolder where folderId in (select ParentFolderId from tblFolder where folderId={0})", Convert.ToInt32(Session["FolderID"])));
            string FolderName = operation.ExecuteScalar4Command(string.Format(@"select FolderName from tblFolder where folderId={0}", Convert.ToInt32(Session["FolderID"])));
            string FullName = operation.ExecuteScalar4Command(string.Format(@"select Firstname+' ' + Lastname as FullName from tbluserdetail where UserID={0}", Convert.ToInt32(Session["UserID"])));
            string strUserIds = "SELECT (select EmailID from tbluserdetail where userid=a.userid)'EmailId' FROM tbluserAccesscontrol a where groupid in(select groupid from dbo.tblWorkGroupMaster where  groupname in('office secretary'))and  folderid in(select parentfolderid from tblfolder where folderid='" + Convert.ToInt32(HttpContext.Current.Session["FolderID"]) + "') and AccessSymbol<>'N' and Userid!=" + HttpContext.Current.Session["UserID"].ToString() + " and (select EmailID from tbluserdetail where userid=a.userid) is not null";
            DataTable dtUserIds = operation.GetTable4Command(strUserIds);
            string emailIds = "";
            for (int i = 0; i < dtUserIds.Rows.Count; i++)
            {
                emailIds += dtUserIds.Rows[i]["EmailId"].ToString() + ",";
            }
            string[] str = HttpContext.Current.Session["FolderName"].ToString().Split('\\');
            OperationClass objOperationClass = new OperationClass();

            StringBuilder sb = new StringBuilder();
            HttpContext context = HttpContext.Current;
            sb.Append("<table>");
            sb.Append("<tr>");
            sb.Append("<td align='left'>");
            sb.Append("<b> Dear Sir/Madam,  </b> <br />");
            sb.Append(Environment.NewLine + "<br />");
            sb.Append("Rejected  By : " + FullName);
            sb.Append(Environment.NewLine + "<br />");
            sb.Append("Committee Name : " + ParentFolderName);
            sb.Append(Environment.NewLine + "<br />");
            sb.Append("Meeting Scheduled on : " + FolderName);
            sb.Append(Environment.NewLine + "<br />");
            sb.Append("Comment : " + Comment);
            sb.Append(Environment.NewLine + "<br />");

            sb.Append("</td>");

            sb.Append("</tr>");

            sb.Append("<tr>");
            sb.Append("<td align='left'>");



            sb.Append("<table  align='center' border='0' bordercolor='#00aeef' width='99%' class='reporttable1' cellspacing='0' cellpadding='0' style='font-size:16px;'>");
            sb.Append("<tr><td><b>Regards," + "</b><br />");
            sb.Append("Titan</td></tr></table>");
            sb.Append("</td></tr></table>");
            sb.Append("</td></tr></table>");

            sb.Append(Environment.NewLine + "<br />");
            sb.Append(Environment.NewLine + "<br />");


            try
            {
                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                string MailFrom = Convert.ToString(ConfigurationManager.AppSettings["EmailFrom"]);
                mail.From = new System.Net.Mail.MailAddress(MailFrom);

                mail.To.Add(emailIds);
                mail.Subject = "Rejected Agenda Status";
                mail.Body = sb.ToString();
                mail.IsBodyHtml = true;

                System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
                // client.EnableSsl = true;
                client.Send(mail);

            }
            catch (Exception excm)
            {

            }


        }
        catch
        {

        }

    }


    protected void btnSubmitSelected_Click(object sender, EventArgs e)
    {

        R = 0;
        OperationClass objOperationClass = new OperationClass();
        foreach (GridViewRow row in gvData.Rows)
        {
            CheckBox chkSelect = (CheckBox)row.FindControl("chkSelect");
            Label lblApprovalStatus = (Label)row.FindControl("lblApprovalStatus");
            if (chkSelect.Checked)
            {
                Label FileId = (Label)row.FindControl("lblFileId");
                DataTable dtAttachmentIds = objOperationClass.GetTable4Command(string.Format(@"select attachmentid  from tblattachment where fileid = {0}", Convert.ToInt32(FileId.Text.ToString())));
                string fileids = FileId.Text.ToString();

                fileids = fileids + ",";
                if (dtAttachmentIds != null && dtAttachmentIds.Rows.Count > 0)
                {
                    for (int i = 0; i < dtAttachmentIds.Rows.Count; i++)
                    {
                        if (i == dtAttachmentIds.Rows.Count - 1)
                        {
                            fileids = fileids + dtAttachmentIds.Rows[i]["attachmentid"].ToString();
                        }
                        else
                        {
                            fileids = fileids + dtAttachmentIds.Rows[i]["attachmentid"].ToString() + ",";
                        }
                    }
                }
                else
                {
                    fileids = fileids.Substring(0, fileids.LastIndexOf(","));
                }
                //int value2 = Convert.ToInt16(operation.Insert4Command(string.Format(@"Update tblfile set ApprovalStatus=1 where fileid={0}", Convert.ToInt32(FileId.Text.ToString()))));
                if (lblApprovalStatus.Text == "5")
                {

                    int value2 = Convert.ToInt16(operation.Insert4Command(string.Format(@"Update tblFile set ApprovalStatus=0,withdrawcomments=null  where fileid in ({0})", fileids)));
                    ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('Agenda Submited.')", true);
                }
            }
        }
        DirectorDataBinding();
        chkSelectalll.Checked = false;


    }

    protected void btncommentcancel_Click(object sender, EventArgs e)
    {
        pncomment.Visible = false;
        ViewState["FirstSelectedd"] = null;
        ViewState["LastSelectedd"] = null;
        ViewState["FileIds"] = null;
        ViewState["FileIds"] = null;
    }

    protected void GetAgendaDetails()
    {
        btnMoveFile.Visible = false;
        gvData.Visible = false;
        //chkSelectEmailId.Visible = false;
        //lblChkEmailId.Visible = false;
        gvParent.Visible = false;
        DataTable dtAgenda = null;
        DataTable dtAgendatablename = null;
        DataTable dtStepsno = null;
        if (Convert.ToString(Session["Groupname"]).ToLower().Trim() == "board secretariat user")
        {
            string strtablename = operation.ExecuteScalar4Command("select TableName from tblfolderindexmaster where folder_id=(select FolderID from tblfolder where foldername='binani cement')");
            dtAgenda = operation.GetTable4Command(string.Format(@"select distinct b.Id,b.FileId,d.column0 'Subject',0'stepsno',b.StepStatus,a.filename,b.reason from tblfile a inner join tblAgendaApprovalDetail b on a.fileid=b.fileid inner join tblAgendaLevelDetail c on c.folderid=b.ParentFolderId inner join {0} d on d.fileid=a.fileid where FinalAgendaApprovalStatus=1", strtablename));
            if (dtAgenda != null && dtAgenda.Rows.Count > 0)
            {
                GridView2.DataSource = dtAgenda;
                GridView2.DataBind();
                lblItems.Visible = true;
                btnDistributeAll.Visible = true;
            }
            else
            {
                GridView2.DataSource = null;
                GridView2.DataBind();
                btnDistributeAll.Visible = false;
            }

            dtStepsno = operation.GetTable4Command(string.Format(@"select stepsno from tblAgendaLevelDetail where  userid={0} and Folderid=(select Folderid from tblfolder where foldername='binani cement')", Convert.ToInt32(Session["UserID"])));
            string strStepsno = "";
            if (dtStepsno != null && dtStepsno.Rows.Count > 0)
            {
                for (int i = 0; i < dtStepsno.Rows.Count; i++)
                {

                    strStepsno = strStepsno + dtStepsno.Rows[i]["stepsno"].ToString() + ",";
                }
            }
            if (strStepsno != "")
            {

                string strAgendatablename = operation.ExecuteScalar4Command("select TableName from tblfolderindexmaster where folder_id=(select FolderID from tblfolder where foldername='binani cement')");
                //dtAgenda = operation.GetTable4Command(string.Format(@"select b.Id,b.FileId,d.column0 'Subject',c.stepsno,b.StepStatus,a.filename,c.userid,b.reason from tblfile a inner join tblAgendaApprovalDetail b on a.fileid=b.fileid   inner join tblAgendaLevelDetail c on c.folderid=b.ParentFolderId inner join {0} d on d.fileid=a.fileid where c.userid={1} and b.StepStatus in ({2}) and  b.FinalAgendaApprovalStatus is null", strtablename, Convert.ToInt32(Session["UserID"]), strStepsno.Substring(0, strStepsno.LastIndexOf(","))));
                dtAgendatablename = operation.GetTable4Command(string.Format(@"select b.Id,b.WorkFlowAgendaApproval_Id,c.WorkFlowAgenda_Id,b.FileId,d.column0 'Subject',c.stepsno,b.StepStatus,a.filename,c.userid,b.reason,b.Status from tblfile a inner join tblAgendaApprovalDetail b on a.fileid=b.fileid inner join tblAgendaLevelDetail c on c.folderid=b.ParentFolderId inner join {0} d on d.fileid=a.fileid where c.userid={1} and b.StepStatus in ({2}) and  b.FinalAgendaApprovalStatus is null and b.WorkFlowAgendaApproval_Id=c.WorkFlowAgenda_Id", strAgendatablename, Convert.ToInt32(Session["UserID"]), strStepsno.Substring(0, strStepsno.LastIndexOf(","))));

                if (dtAgendatablename != null && dtAgendatablename.Rows.Count > 0)
                {
                    GridView1.DataSource = dtAgendatablename;
                    GridView1.DataBind();
                }
                else
                {
                    GridView1.DataSource = null;
                    GridView1.DataBind();
                    btnAuthorize.Visible = false;
                }
            }
        }
        else
        {
            dtStepsno = operation.GetTable4Command(string.Format(@"select stepsno from tblAgendaLevelDetail where  userid={0} and Folderid=(select Folderid from tblfolder where foldername='binani cement')", Convert.ToInt32(Session["UserID"])));
            string strStepsno = "";
            if (dtStepsno != null && dtStepsno.Rows.Count > 0)
            {
                for (int i = 0; i < dtStepsno.Rows.Count; i++)
                {

                    strStepsno = strStepsno + dtStepsno.Rows[i]["stepsno"].ToString() + ",";
                }
            }
            if (strStepsno != "")
            {

                string strtablename = operation.ExecuteScalar4Command("select TableName from tblfolderindexmaster where folder_id=(select FolderID from tblfolder where foldername='binani cement')");
                //dtAgenda = operation.GetTable4Command(string.Format(@"select b.Id,b.FileId,d.column0 'Subject',c.stepsno,b.StepStatus,a.filename,c.userid,b.reason from tblfile a inner join tblAgendaApprovalDetail b on a.fileid=b.fileid   inner join tblAgendaLevelDetail c on c.folderid=b.ParentFolderId inner join {0} d on d.fileid=a.fileid where c.userid={1} and b.StepStatus in ({2}) and  b.FinalAgendaApprovalStatus is null", strtablename, Convert.ToInt32(Session["UserID"]), strStepsno.Substring(0, strStepsno.LastIndexOf(","))));
                dtAgenda = operation.GetTable4Command(string.Format(@"select b.Id,b.WorkFlowAgendaApproval_Id,c.WorkFlowAgenda_Id,b.FileId,d.column0 'Subject',c.stepsno,b.StepStatus,a.filename,c.userid,b.reason,b.Status from tblfile a inner join tblAgendaApprovalDetail b on a.fileid=b.fileid inner join tblAgendaLevelDetail c on c.folderid=b.ParentFolderId inner join {0} d on d.fileid=a.fileid where c.userid={1} and b.StepStatus in ({2}) and  b.FinalAgendaApprovalStatus is null and b.WorkFlowAgendaApproval_Id=c.WorkFlowAgenda_Id", strtablename, Convert.ToInt32(Session["UserID"]), strStepsno.Substring(0, strStepsno.LastIndexOf(","))));

                if (dtAgenda != null && dtAgenda.Rows.Count > 0)
                {
                    GridView1.DataSource = dtAgenda;
                    GridView1.DataBind();
                }
                else
                {
                    GridView1.DataSource = null;
                    GridView1.DataBind();
                    btnAuthorize.Visible = false;
                }
            }
        }

    }

    protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            HiddenField hdstepsno = (HiddenField)e.Row.FindControl("hdstepsno");
            HiddenField hdStepStatus = (HiddenField)e.Row.FindControl("hdStepStatus");
            Label lblReason = (Label)e.Row.FindControl("lblReason");
            Label lblStatus = (Label)e.Row.FindControl("lblStatus");
            LinkButton lnkReject = (LinkButton)e.Row.FindControl("lnkReject");
            LinkButton lnkApproveAgenda = (LinkButton)e.Row.FindControl("lnkApproveAgenda");
            LinkButton lnkDisApproveAgenda = (LinkButton)e.Row.FindControl("lnkDisApproveAgenda");
            Label lblWorkFlowAgenda_Id = (Label)e.Row.FindControl("lblWorkFlowAgenda_Id");

            if (lblWorkFlowAgenda_Id.Text == "2")
            {
                if (hdstepsno.Value != hdStepStatus.Value)
                {
                    e.Row.Visible = false;
                }
                else
                    if (hdStepStatus.Value.ToString() == "1" && Convert.ToString(lblReason.Text) == "")
                    {
                        lnkReject.Enabled = false;
                    }
                    else
                        if (Convert.ToString(lblReason.Text) != "")
                        {
                            //lnkReject.Enabled = true;
                            ////lblStatus.Text = "Rejected";
                            //lnkApproveAgenda.Enabled = false;
                            ////GridView1.Columns[8].Visible = false;
                            if (hdStepStatus.Value.ToString() == "2")
                            {
                                lnkApproveAgenda.Visible = false;
                                lnkDisApproveAgenda.Enabled = false;
                                lnkApproveAgenda.Text = "Confirm";
                                GridView1.Columns[8].Visible = false;
                                lnkReject.Text = "Delete";
                            }
                            else
                                if (hdStepStatus.Value.ToString() == "6")
                                {
                                    lnkApproveAgenda.Enabled = false;
                                    lnkDisApproveAgenda.Enabled = false;
                                }
                                else
                                    if (hdStepStatus.Value.ToString() == "3")
                                    {
                                        lnkApproveAgenda.Visible = false;
                                        GridView1.Columns[9].Visible = false;
                                    }
                                    else
                                    {
                                        e.Row.Visible = true;
                                        lnkApproveAgenda.Text = "Forward";
                                        GridView1.Columns[9].Visible = false;
                                    }
                        }
                        else
                            if (hdStepStatus.Value.ToString() == "2")
                            {
                                e.Row.Visible = true;
                                lnkApproveAgenda.Text = "Review";
                                lnkReject.Text = "Delete";
                                GridView1.Columns[8].Visible = false;
                            }
                            else
                                if (hdStepStatus.Value.ToString() == "3")
                                {
                                    e.Row.Visible = true;
                                    lnkApproveAgenda.Text = "Authorize";
                                    GridView1.Columns[9].Visible = false;
                                }
                                else
                                    if (hdStepStatus.Value.ToString() == "4")
                                    {
                                        e.Row.Visible = true;
                                        lnkApproveAgenda.Text = "Approve";
                                        GridView1.Columns[9].Visible = false;
                                    }
                                    else
                                        if (hdStepStatus.Value.ToString() == "5")
                                        {
                                            e.Row.Visible = true;
                                            lnkApproveAgenda.Text = "Forward";
                                            GridView1.Columns[9].Visible = false;
                                        }
                                        else
                                        {
                                            e.Row.Visible = true;
                                        }
            }
            else
            {
                if (hdstepsno.Value != hdStepStatus.Value)
                {
                    e.Row.Visible = false;
                }
                else
                    if (hdStepStatus.Value.ToString() == "1" && Convert.ToString(lblReason.Text) == "")
                    {
                        lnkReject.Enabled = false;
                    }
                    else
                        if (Convert.ToString(lblReason.Text) != "")
                        {

                            if (hdStepStatus.Value.ToString() == "2")
                            {
                                lnkApproveAgenda.Visible = false;
                                lnkDisApproveAgenda.Enabled = false;
                                lnkApproveAgenda.Text = "Confirm";
                                GridView1.Columns[8].Visible = false;
                                lnkReject.Text = "Delete";
                            }
                            //else
                            //    if (hdStepStatus.Value.ToString() == "6")
                            //    {
                            //        lnkApproveAgenda.Enabled = false;
                            //        lnkDisApproveAgenda.Enabled = false;
                            //    }
                            //    else
                            //        if (hdStepStatus.Value.ToString() == "3")
                            //        {
                            //            lnkApproveAgenda.Visible = false;
                            //            GridView1.Columns[9].Visible = false;
                            //        }
                            //        else
                            //        {
                            //            e.Row.Visible = true;
                            //            lnkApproveAgenda.Text = "Forward";
                            //            GridView1.Columns[9].Visible = false;
                            //        }
                        }
                        else
                            if (hdStepStatus.Value.ToString() == "2")
                            {
                                e.Row.Visible = true;
                                lnkApproveAgenda.Text = "Review";
                                lnkReject.Text = "Delete";
                                GridView1.Columns[8].Visible = false;
                            }
                            else
                                if (hdStepStatus.Value.ToString() == "3")
                                {
                                    e.Row.Visible = true;
                                    lnkApproveAgenda.Text = "Approve";
                                    GridView1.Columns[9].Visible = false;
                                }
                                else
                                {
                                    e.Row.Visible = true;
                                }

            }
            if (Session["GroupName"].ToString().ToLower() == "directors" || Session["GroupName"].ToString().ToLower() == "permanent invitees" || Session["GroupName"].ToString().ToLower() == "cfo" || Convert.ToString(Session["Groupname"]).ToLower() == "department" || Convert.ToString(Session["Groupname"]).ToLower() == "board secretariat user" || Convert.ToString(Session["Groupname"]).ToLower() == "senior management" || Convert.ToString(Session["Groupname"]).ToLower() == "functional management" || Convert.ToString(Session["Groupname"]).ToLower() == "others")
            {
                GridView1.Columns[9].Visible = false;
                GridView1.Columns[3].Visible = false;
                GridView1.Columns[4].Visible = false;
            }
            else
            {
                //GridView1.Columns[8].Visible = false;
            }
        }

    }


    protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Approve")
        {
            int strStepStatus = 0;
            string[] FileNameID = e.CommandArgument.ToString().Split(new Char[] { ',' });
            string strAgendaSteps = operation.ExecuteScalar4Command(string.Format(@"select steps from dbo.tblAgendaLevelMaster where folderid=(select ParentfolderId from dbo.tblAgendaApprovalDetail where id={0}) and WorkFlow_id=(select WorkFlowAgendaApproval_Id from dbo.tblAgendaApprovalDetail where id={1})", Convert.ToInt32(FileNameID[0].ToString()), Convert.ToInt32(FileNameID[0].ToString())));
            //if (FileNameID[1].ToString() == strAgendaSteps)
            //{
            //    //strStepStatus = Convert.ToInt32(operation.ExecuteNonQuery(string.Format(@"update tblAgendaApprovalDetail set StepStatus={0} where Id={1}", 1, Convert.ToInt32(FileNameID[0].ToString()))));
            //    strStepStatus = Convert.ToInt32(operation.ExecuteNonQuery(string.Format(@"update tblAgendaApprovalDetail set StepStatus={0} where Id={1}", 2, Convert.ToInt32(FileNameID[0].ToString()))));
            //}
            //else
            //{
            if (FileNameID[3].ToString() == "2")
            {
                if (FileNameID[1].ToString() == "2")
                {
                    strStepStatus = Convert.ToInt32(operation.ExecuteNonQuery(string.Format(@"update tblAgendaApprovalDetail set StepStatus={0},Status='{1}' where Id={2}", (Convert.ToInt32(FileNameID[1].ToString()) + 1), "For Approval", Convert.ToInt32(FileNameID[0].ToString()))));
                }
                else
                    if (FileNameID[1].ToString() == "3")
                    {
                        strStepStatus = Convert.ToInt32(operation.ExecuteNonQuery(string.Format(@"update tblAgendaApprovalDetail set StepStatus={0},Status='{1}' where Id={2}", (Convert.ToInt32(FileNameID[1].ToString()) + 1), "For Approval", Convert.ToInt32(FileNameID[0].ToString()))));
                    }
                    else
                        if (FileNameID[1].ToString() == "4")
                        {
                            strStepStatus = Convert.ToInt32(operation.ExecuteNonQuery(string.Format(@"update tblAgendaApprovalDetail set StepStatus={0},Status='{1}' where Id={2}", (Convert.ToInt32(FileNameID[1].ToString()) + 1), "Approved", Convert.ToInt32(FileNameID[0].ToString()))));
                        }
                        else
                            if (FileNameID[1].ToString() == "5")
                            {
                                strStepStatus = Convert.ToInt32(operation.ExecuteNonQuery(string.Format(@"update tblAgendaApprovalDetail set StepStatus={0},FinalAgendaApprovalStatus={1} where Id={2}", (Convert.ToInt32(FileNameID[1].ToString()) + 1), 1, Convert.ToInt32(FileNameID[0].ToString()))));
                            }
                            else
                            {

                                strStepStatus = Convert.ToInt32(operation.ExecuteNonQuery(string.Format(@"update tblAgendaApprovalDetail set StepStatus={0} where Id={1}", (Convert.ToInt32(FileNameID[1].ToString()) + 1), Convert.ToInt32(FileNameID[0].ToString()))));
                            }


                if (strStepStatus > 0)
                {
                    if (FileNameID[1].ToString() == "2")
                    {
                        ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('Agenda is Reviewed.')", true);
                    }
                    else
                        if (FileNameID[1].ToString() == "3")
                        {
                            ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('Agenda is authorized.')", true);
                        }
                        else
                            if (FileNameID[1].ToString() == "4")
                            {
                                ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('Agenda is Forwarded.')", true);
                            }
                            else
                                if (FileNameID[1].ToString() == "5")
                                {
                                    ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('Agenda is Forwarded.')", true);
                                }
                }
                GetAgendaDetails();
            }
            else
            {
                if (FileNameID[1].ToString() == "2")
                {
                    strStepStatus = Convert.ToInt32(operation.ExecuteNonQuery(string.Format(@"update tblAgendaApprovalDetail set StepStatus={0},Status='{1}' where Id={2}", (Convert.ToInt32(FileNameID[1].ToString()) + 1), "For Approval", Convert.ToInt32(FileNameID[0].ToString()))));
                }
                else
                    if (FileNameID[1].ToString() == "3")
                    {
                        strStepStatus = Convert.ToInt32(operation.ExecuteNonQuery(string.Format(@"update tblAgendaApprovalDetail set StepStatus={0},FinalAgendaApprovalStatus={1} where Id={2}", (Convert.ToInt32(FileNameID[1].ToString()) + 1), 1, Convert.ToInt32(FileNameID[0].ToString()))));
                    }



                if (strStepStatus > 0)
                {
                    if (FileNameID[1].ToString() == "2")
                    {
                        ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('Agenda is Reviewed.')", true);
                    }
                    else
                        if (FileNameID[1].ToString() == "3")
                        {
                            ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('Agenda is Forwarded.')", true);
                        }
                }
                GetAgendaDetailsforBoard();
            }

        }

        if (e.CommandName == "DownLoad")
        {
            string[] FileNameID = e.CommandArgument.ToString().Split(new Char[] { ',' });

            Session["FileID"] = FileNameID[0].ToString();
            Session["FileName"] = FileNameID[1].ToString();

            // create objects of class
            objCommonBAL = new CommonBAL();

            //visible lblmessage
            lblMessage.Visible = false;
            lblMessage.Text = "";

            //visible panel first
            Panel4.Visible = false;

            //set folder for save decrypt file.
            string ImageSavingFilePath = Convert.ToString(HttpContext.Current.Server.MapPath("~\\Repository\\Decrypt\\" + HttpContext.Current.Session["UserName"].ToString()));

            //set folder path exported file.
            string ImagesavedFilePath = Convert.ToString(HttpContext.Current.Server.MapPath("~\\ExportFile\\" + Convert.ToString(HttpContext.Current.Session["UserName"]).Trim()));

            //Set zip file name and path
            string ZipFilePath = Convert.ToString(Server.MapPath("~\\ExportFile\\".Trim() + Session["UserName"].ToString().Trim() + "\\" + string.Format("ExportedFile{0:MMM-dd-yyyy_hh-mm-ss}", System.DateTime.Now) + ".zip"));

            //set directory path for delete tepory file.
            string ZipDirectoryPath = Convert.ToString(Server.MapPath("~\\ExportFile\\".Trim() + Session["UserName"].ToString().Trim()));

            string strMessage = objCommonBAL.ExportFileOnButtonClick1(Convert.ToString(Session["FileID"]), ImageSavingFilePath, ImagesavedFilePath, ZipFilePath, ZipDirectoryPath, "");
            //string strMessage = objCommonBAL.ExportFileOnButtonClick1(Convert.ToString(Session["FileID"]), ImageSavingFilePath, ImagesavedFilePath, ZipFilePath, ZipDirectoryPath, "", Session["FolderName"].ToString());
            if (strMessage.Contains("alert"))
            {
                ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "myScr", strMessage, true);
            }
            else
            {

                FileDownLoad(strMessage);
            }
        }

        if (e.CommandName == "DisApprove")
        {
            Panel5.Visible = true;
            string[] FileNameID = e.CommandArgument.ToString().Split(new Char[] { ',' });
            ViewState["Id"] = FileNameID[0].ToString();
            ViewState["StepStatus"] = FileNameID[1].ToString();
        }

        if (e.CommandName == "Reject")
        {
            string[] FileNameID = e.CommandArgument.ToString().Split(new Char[] { ',' });

            Session["FileID"] = FileNameID[0].ToString();
            Session["FileName"] = FileNameID[1].ToString();

            int strFileApprovalStatus = Convert.ToInt32(operation.ExecuteNonQuery(string.Format(@"Delete From tblFile where FileID={0}", Convert.ToInt32(FileNameID[0].ToString()))));
            int strFileAgendaDelete = Convert.ToInt32(operation.ExecuteNonQuery(string.Format(@"Delete From tblAgendaApprovalDetail where FileID={0}", Convert.ToInt32(FileNameID[0].ToString()))));
            if (strFileApprovalStatus > 0)
            {
                if (FileNameID[2].ToString() == "2")
                {
                    ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "myScr", "alert('Agenda is Deleted.')", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "myScr", "alert('Agenda is Rejected.')", true);
                }
            }
            if (Session["FolderName"].ToString().ToLower() == "Meetings\\binani cement")
            {
                GetAgendaDetails();
            }
            else
            {
                GetAgendaDetailsforBoard();
            }
        }

        if (e.CommandName == "View")
        {

            //if user does not have file view permission then a message will be displayed. You don't have access to view these files.
            string[] FileNameID = e.CommandArgument.ToString().Split(new Char[] { ',' });

            Session["FileID"] = FileNameID[0].ToString();
            Session["FileName"] = FileNameID[1].ToString();
            Session["WorkFlow"] = "WorkFlow";
            Session["Redirect"] = "View";
            if (Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".tif" || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".tiff"
                || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".gif" || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".bmp"
                || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".jpg" || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".jpeg" || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".png")
            {
                Response.Redirect("../Viewer/Thumbnail.aspx", false);
            }
            else if (Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".wmv")
            {
                StringBuilder sbwindow = new StringBuilder();
                sbwindow.Append("window.showModalDialog('../Viewer/WMVViwer.aspx',null,'status:no;dialogTop:300;dialogWidth:1014px;dialogHeight:700px;dialogHide:true;help:no;scroll:no;center:yes');");
                ScriptManager.RegisterClientScriptBlock(this.Page, typeof(UpdatePanel), "VideoViewer", sbwindow.ToString(), true);
                sbwindow = null;
            }
            else if (Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".pdf")
            {

                Response.Redirect("../Viewer/Thumbnail.aspx", false);



            }
            else if (Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".zip" || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".rar" || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".zipx")
            {
                ScriptManager.RegisterStartupScript(this.Page, typeof(UpdatePanel), "msg", "alert('Please download file, then view on your local')", true);
                return;
            }

            else
            {
                //StringBuilder sbwindow = new StringBuilder();
                //sbwindow.Append("window.open('../Viewer/OfficerViewer.aspx',null,'scrollbars:no;width=1024px, height=800px;help:no;center:yes; resizable=yes');");
                //// sbwindow.Append("window.showModelessDialog('Viewer/OfficerViewer.aspx',null,'status:no;dialogTop:300;dialogWidth:1024px;dialogHeight:800px;dialogHide:true;help:no;scroll:yes;center:yes');");
                //ScriptManager.RegisterClientScriptBlock(this.Page, typeof(UpdatePanel), "DocumentViewer", sbwindow.ToString(), true);
                //sbwindow = null;


                StringBuilder sbwindow = new StringBuilder();
                //sbwindow.Append("window.open('../Viewer/OfficerViewer.aspx',null,'scrollbars:no;width=1024px, height=800px;help:no;center:yes; resizable=yes');");
                sbwindow.Append("window.open('../Viewer/HTML_OfficerViewer.aspx',null,'scrollbars:no;width=1024px, height=800px;help:no;center:yes; resizable=yes');");
                // sbwindow.Append("window.showModelessDialog('Viewer/OfficerViewer.aspx',null,'status:no;dialogTop:300;dialogWidth:1024px;dialogHeight:800px;dialogHide:true;help:no;scroll:yes;center:yes');");
                ScriptManager.RegisterClientScriptBlock(this.Page, typeof(UpdatePanel), "DocumentViewer", sbwindow.ToString(), true);
                sbwindow = null;
            }
        }

    }

    protected void GridView2_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DropDownList ddlCommitte = (DropDownList)e.Row.FindControl("ddlCommitte");
            //DataTable dtStepsno = operation.GetTable4Command(string.Format(@"select FolderId,FolderName from tblfolder where parentfolderid=1 and foldername!='group companies' and foldername!='Recycle Bin' and foldername!='Archive Folder' and deletestatus!=1"));
            DataTable dtStepsno = operation.GetTable4Command(string.Format(@"select distinct a.FolderName,a.folderid  from tblfolder a inner join tblUserAccesscontrol b on a.folderid=b.folderid and AccessSymbol not in('N')and foldername!='binani cement' and foldername!='Recycle Bin' and foldername!='archived meetings' and Foldername!='Repository' and deletestatus!=1 and ProposedMeetings!=1 and a.parentfolderid=1 and b.userid='{0}'", Session["UserID"]));
            if (dtStepsno != null && dtStepsno.Rows.Count > 0)
            {
                ddlCommitte.DataSource = dtStepsno;
                ddlCommitte.DataTextField = "FolderName";
                ddlCommitte.DataValueField = "FolderId";
                ddlCommitte.DataBind();
                ddlCommitte.Items.Insert(0, new System.Web.UI.WebControls.ListItem("--Select--", "0"));

            }
        }
    }

    protected void BindDropdown()
    {
        DataTable dtStepsno = operation.GetTable4Command(string.Format(@"select distinct a.FolderName,a.folderid  from tblfolder a inner join tblUserAccesscontrol b on a.folderid=b.folderid and AccessSymbol not in('N')and foldername!='binani cement' and foldername!='Recycle Bin' and foldername!='archived meetings' and Foldername!='Repository' and deletestatus!=1 and ProposedMeetings!=1 and a.parentfolderid=1 and b.userid='{0}'", Session["UserID"]));
        //DataTable dtStepsno = operation.GetTable4Command(string.Format(@"select FolderId,FolderName from tblfolder where parentfolderid=1 and foldername!='group companies' and foldername!='Recycle Bin' and foldername!='Archive Folder' and deletestatus!=1"));
        if (dtStepsno != null && dtStepsno.Rows.Count > 0)
        {
            ddlCommitteName.DataSource = dtStepsno;
            ddlCommitteName.DataTextField = "FolderName";
            ddlCommitteName.DataValueField = "FolderId";
            ddlCommitteName.DataBind();
            ddlCommitteName.Items.Insert(0, new System.Web.UI.WebControls.ListItem("--Select--", "0"));
            ddlMeetingDateNew.Items.Insert(0, new System.Web.UI.WebControls.ListItem("--Select--", "0"));
        }
    }

    protected void ddlCommitteName_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlCommitteName.SelectedIndex != 0)
        {
            DataTable dtStepsno = operation.GetTable4Command(string.Format(@"select FolderId,FolderName from tblfolder where parentfolderid={0} and FolderName!='Board & Its Committees' and meetingstatus!=1 and meetingcancelled!=1 and deletestatus!=1 and ProposedMeetings!=1", ddlCommitteName.SelectedValue));
            if (dtStepsno != null && dtStepsno.Rows.Count > 0)
            {
                ddlMeetingDateNew.Enabled = true;
                ddlMeetingDateNew.DataSource = dtStepsno;
                ddlMeetingDateNew.DataTextField = "FolderName";
                ddlMeetingDateNew.DataValueField = "FolderId";
                ddlMeetingDateNew.DataBind();

            }
            else
            {
                ddlMeetingDateNew.Items.Insert(0, new System.Web.UI.WebControls.ListItem("--Select--", "0"));
                ddlMeetingDateNew.Enabled = false;
            }
        }
        else
        {
            ddlMeetingDateNew.Items.Insert(0, new System.Web.UI.WebControls.ListItem("--Select--", "0"));
            ddlMeetingDateNew.SelectedIndex = 0;
            ddlMeetingDateNew.Enabled = false;
        }
    }

    protected void ddlCommitte_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList ddlLabTest = (DropDownList)sender;
        GridViewRow row = (GridViewRow)ddlLabTest.NamingContainer;
        DropDownList ddlAddLabTestShortName = (DropDownList)row.FindControl("ddlCommitte");
        DropDownList ddlMeeting = (DropDownList)row.FindControl("ddlMeeting");
        if (ddlAddLabTestShortName.SelectedIndex != 0)
        {
            DataTable dtStepsno = operation.GetTable4Command(string.Format(@"select FolderId,FolderName from tblfolder where parentfolderid={0} and FolderName!='Board & Its Committees' and meetingstatus!=1 and meetingcancelled!=1 and deletestatus!=1 and ProposedMeetings!=1", ddlAddLabTestShortName.SelectedValue));
            if (dtStepsno != null && dtStepsno.Rows.Count > 0)
            {
                ddlMeeting.Enabled = true;
                ddlMeeting.DataSource = dtStepsno;
                ddlMeeting.DataTextField = "FolderName";
                ddlMeeting.DataValueField = "FolderId";
                ddlMeeting.DataBind();

            }
            else
            {
                ddlMeeting.Items.Insert(0, new System.Web.UI.WebControls.ListItem("--Select--", "0"));
                ddlMeeting.Enabled = false;
            }
        }
        else
        {
            ddlMeeting.Items.Insert(0, new System.Web.UI.WebControls.ListItem("--Select--", "0"));
            ddlMeeting.SelectedIndex = 0;
            ddlMeeting.Enabled = false;
        }
    }


    protected void GridView2_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Move")
        {
            string strAgendatablename = operation.ExecuteScalar4Command(string.Format(@"select foldername from tblfolder where FolderID={0}", Convert.ToInt32(Session["FolderID"])));

            string str = "";
            if (strAgendatablename.ToLower() == "binani cement")
            {
                foreach (GridViewRow row in GridView2.Rows)
                {
                    CheckBox chkSelected = (CheckBox)row.FindControl("chkSelected");
                    if (chkSelected.Checked)
                    {
                        DropDownList ddlMeeting = (DropDownList)row.FindControl("ddlMeeting");
                        DropDownList ddlCommitte = (DropDownList)row.FindControl("ddlCommitte");
                        if (ddlCommitte.SelectedIndex != 0)
                        {
                            str = ddlMeeting.SelectedValue.ToString();
                            string[] FileNameID = e.CommandArgument.ToString().Split(new Char[] { ',' });
                            Session["FileID"] = FileNameID[0].ToString();
                            Session["FileName"] = FileNameID[1].ToString();
                            string[] filename = FileNameID[1].Split(new Char[] { '.' });
                            string name = filename[0].ToString();
                            //System.IO.File.Move(Server.MapPath("~\\Repository") + "\\" + name + "_" + Session["FolderID"].ToString() + ".enc", Server.MapPath("~\\Repository") + "\\" + name + "_" + ddlMeeting.SelectedValue.ToString() + ".enc");
                            string TableName = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Convert.ToInt32(str)));
                            string TableNameFrom = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Session["FolderID"]));
                            string value1 = operation.ExecuteScalar4Command(string.Format(@"insert into {0}(FileId,ImportedBy,ImportedOn,DocStatus,FolderId,Column1)(select FileId,ImportedBy,ImportedOn,DocStatus,FolderId,Column0 from {1} where FileId='{2}')", TableName, TableNameFrom, FileNameID[0].ToString()));
                            string Deletestatement = string.Format("delete from {0} where Fileid={1}", TableNameFrom, Convert.ToInt32(FileNameID[0].ToString()));
                            operation.Insert4Command(Deletestatement);
                            int value2 = Convert.ToInt16(operation.Insert4Command(string.Format(@"update tblfile set folderid={0} where fileid={1}", Convert.ToInt32(str), Convert.ToInt32(FileNameID[0].ToString()))));
                            string DeleteAgenda = string.Format("Update tblAgendaApprovalDetail Set FinalAgendaApprovalStatus=2 where Fileid={0}", Convert.ToInt32(FileNameID[0].ToString()));
                            operation.Insert4Command(DeleteAgenda);
                            if (value2 > 0)
                            {
                                ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('File Moved Successfully.')", true);
                            }
                        }
                    }
                }
                GetAgendaDetails();
            }
            else
            {
                foreach (GridViewRow row in GridView2.Rows)
                {
                    CheckBox chkSelected = (CheckBox)row.FindControl("chkSelected");
                    if (chkSelected.Checked)
                    {
                        DropDownList ddlMeeting = (DropDownList)row.FindControl("ddlMeeting");
                        DropDownList ddlCommitte = (DropDownList)row.FindControl("ddlCommitte");
                        if (ddlCommitte.SelectedIndex != 0)
                        {
                            str = ddlMeeting.SelectedValue.ToString();
                            string[] FileNameID = e.CommandArgument.ToString().Split(new Char[] { ',' });
                            Session["FileID"] = FileNameID[0].ToString();
                            Session["FileName"] = FileNameID[1].ToString();
                            string[] filename = FileNameID[1].Split(new Char[] { '.' });
                            string name = filename[0].ToString();
                            // System.IO.File.Move(Server.MapPath("~\\Repository") + "\\" + name + "_" + Session["FolderID"].ToString() + ".enc", Server.MapPath("~\\Repository") + "\\" + name + "_" + ddlMeeting.SelectedValue.ToString() + ".enc");
                            string TableName = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Convert.ToInt32(str)));
                            string TableNameFrom = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Session["FolderID"]));
                            string value1 = operation.ExecuteScalar4Command(string.Format(@"insert into {0}(FileId,ImportedBy,ImportedOn,DocStatus,FolderId,Column1)(select FileId,ImportedBy,ImportedOn,DocStatus,FolderId,Column0 from {1} where FileId='{2}')", TableName, TableNameFrom, FileNameID[0].ToString()));
                            string Deletestatement = string.Format("delete from {0} where Fileid={1}", TableNameFrom, Convert.ToInt32(FileNameID[0].ToString()));
                            operation.Insert4Command(Deletestatement);
                            int value2 = Convert.ToInt16(operation.Insert4Command(string.Format(@"update tblfile set folderid={0} where fileid={1}", Convert.ToInt32(str), Convert.ToInt32(FileNameID[0].ToString()))));
                            string DeleteAgenda = string.Format("Update tblAgendaApprovalDetail Set FinalAgendaApprovalStatus=2 where Fileid={0}", Convert.ToInt32(FileNameID[0].ToString()));
                            operation.Insert4Command(DeleteAgenda);
                            if (value2 > 0)
                            {
                                ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('File Moved Successfully.')", true);
                            }
                        }
                    }
                }
                GetAgendaDetailsforBoard();

            }
        }

        if (e.CommandName == "View")
        {

            //if user does not have file view permission then a message will be displayed. You don't have access to view these files.
            string[] FileNameID = e.CommandArgument.ToString().Split(new Char[] { ',' });

            Session["FileID"] = FileNameID[0].ToString();
            Session["FileName"] = FileNameID[1].ToString();
            Session["WorkFlow"] = "WorkFlow";
            Session["Redirect"] = "View";
            if (Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".tif" || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".tiff"
                || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".gif" || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".bmp"
                || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".jpg" || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".jpeg" || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".png")
            {
                Response.Redirect("../Viewer/Thumbnail.aspx", false);
            }
            else if (Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".wmv")
            {
                StringBuilder sbwindow = new StringBuilder();
                sbwindow.Append("window.showModalDialog('../Viewer/WMVViwer.aspx',null,'status:no;dialogTop:300;dialogWidth:1014px;dialogHeight:700px;dialogHide:true;help:no;scroll:no;center:yes');");
                ScriptManager.RegisterClientScriptBlock(this.Page, typeof(UpdatePanel), "VideoViewer", sbwindow.ToString(), true);
                sbwindow = null;
            }
            else if (Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".pdf")
            {

                Response.Redirect("../Viewer/Thumbnail.aspx", false);



            }
            else if (Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".zip" || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".rar" || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".zipx")
            {
                ScriptManager.RegisterStartupScript(this.Page, typeof(UpdatePanel), "msg", "alert('Please download file, then view on your local')", true);
                return;
            }

            else
            {
                //StringBuilder sbwindow = new StringBuilder();
                //sbwindow.Append("window.open('../Viewer/OfficerViewer.aspx',null,'scrollbars:no;width=1024px, height=800px;help:no;center:yes; resizable=yes');");
                //// sbwindow.Append("window.showModelessDialog('Viewer/OfficerViewer.aspx',null,'status:no;dialogTop:300;dialogWidth:1024px;dialogHeight:800px;dialogHide:true;help:no;scroll:yes;center:yes');");
                //ScriptManager.RegisterClientScriptBlock(this.Page, typeof(UpdatePanel), "DocumentViewer", sbwindow.ToString(), true);
                //sbwindow = null;


                StringBuilder sbwindow = new StringBuilder();
                //sbwindow.Append("window.open('../Viewer/OfficerViewer.aspx',null,'scrollbars:no;width=1024px, height=800px;help:no;center:yes; resizable=yes');");
                sbwindow.Append("window.open('../Viewer/HTML_OfficerViewer.aspx',null,'scrollbars:no;width=1024px, height=800px;help:no;center:yes; resizable=yes');");
                // sbwindow.Append("window.showModelessDialog('Viewer/OfficerViewer.aspx',null,'status:no;dialogTop:300;dialogWidth:1024px;dialogHeight:800px;dialogHide:true;help:no;scroll:yes;center:yes');");
                ScriptManager.RegisterClientScriptBlock(this.Page, typeof(UpdatePanel), "DocumentViewer", sbwindow.ToString(), true);
                sbwindow = null;
            }
        }

        if (e.CommandName == "DownLoad")
        {
            string[] FileNameID = e.CommandArgument.ToString().Split(new Char[] { ',' });

            Session["FileID"] = FileNameID[0].ToString();
            Session["FileName"] = FileNameID[1].ToString();

            // create objects of class
            objCommonBAL = new CommonBAL();

            //visible lblmessage
            lblMessage.Visible = false;
            lblMessage.Text = "";

            //visible panel first
            Panel4.Visible = false;

            //set folder for save decrypt file.
            string ImageSavingFilePath = Convert.ToString(HttpContext.Current.Server.MapPath("~\\Repository\\Decrypt\\" + HttpContext.Current.Session["UserName"].ToString()));

            //set folder path exported file.
            string ImagesavedFilePath = Convert.ToString(HttpContext.Current.Server.MapPath("~\\ExportFile\\" + Convert.ToString(HttpContext.Current.Session["UserName"]).Trim()));

            //Set zip file name and path
            string ZipFilePath = Convert.ToString(Server.MapPath("~\\ExportFile\\".Trim() + Session["UserName"].ToString().Trim() + "\\" + string.Format("ExportedFile{0:MMM-dd-yyyy_hh-mm-ss}", System.DateTime.Now) + ".zip"));

            //set directory path for delete tepory file.
            string ZipDirectoryPath = Convert.ToString(Server.MapPath("~\\ExportFile\\".Trim() + Session["UserName"].ToString().Trim()));

            string strMessage = objCommonBAL.ExportFileOnButtonClick1(Convert.ToString(Session["FileID"]), ImageSavingFilePath, ImagesavedFilePath, ZipFilePath, ZipDirectoryPath, "");
            //string strMessage = objCommonBAL.ExportFileOnButtonClick1(Convert.ToString(Session["FileID"]), ImageSavingFilePath, ImagesavedFilePath, ZipFilePath, ZipDirectoryPath, "", Session["FolderName"].ToString());
            if (strMessage.Contains("alert"))
            {
                ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "myScr", strMessage, true);
            }
            else
            {
                FileDownLoad(strMessage);
            }
        }
    }


    protected void btnYesnew_Click(object sender, EventArgs e)
    {
        int strStepStatus = 0;
        string strAgendatablename = operation.ExecuteScalar4Command(string.Format(@"select foldername from tblfolder where FolderID={0}", Convert.ToInt32(Session["FolderID"])));
        if (strAgendatablename.ToLower() == "binani cement")
        {

            if (ViewState["StepStatus"].ToString() == "4" || ViewState["StepStatus"].ToString() == "3" || ViewState["StepStatus"].ToString() == "5")
            {
                strStepStatus = Convert.ToInt32(operation.ExecuteNonQuery(string.Format(@"update tblAgendaApprovalDetail set Reason='{0}',StepStatus={1},Status='{2}' where Id={3}", txtReason.Text.Replace("'", "''").Trim(), (Convert.ToInt32(ViewState["StepStatus"].ToString()) - 1), "Rejected", Convert.ToInt32(ViewState["Id"].ToString()))));
            }
            else
            {
                strStepStatus = Convert.ToInt32(operation.ExecuteNonQuery(string.Format(@"update tblAgendaApprovalDetail set Reason='{0}',StepStatus={1} where Id={2}", txtReason.Text.Replace("'", "''").Trim(), (Convert.ToInt32(ViewState["StepStatus"].ToString()) - 1), Convert.ToInt32(ViewState["Id"].ToString()))));
            }
            ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('File is Disapproved.')", true);
            Panel5.Visible = false;
            if (ViewState["Id"] != "" || ViewState["StepStatus"] != "")
            {
                ViewState["Id"] = "";
                ViewState["StepStatus"] = "";
            }
            GetAgendaDetails();
        }
        else
        {

            if (ViewState["StepStatus"].ToString() == "3")
            {
                strStepStatus = Convert.ToInt32(operation.ExecuteNonQuery(string.Format(@"update tblAgendaApprovalDetail set Reason='{0}',StepStatus={1},Status='{2}' where Id={3}", txtReason.Text.Replace("'", "''").Trim(), (Convert.ToInt32(ViewState["StepStatus"].ToString()) - 1), "Rejected", Convert.ToInt32(ViewState["Id"].ToString()))));
            }

            ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('File is Disapproved.')", true);
            Panel5.Visible = false;
            if (ViewState["Id"] != "" || ViewState["StepStatus"] != "")
            {
                ViewState["Id"] = "";
                ViewState["StepStatus"] = "";
            }
            GetAgendaDetailsforBoard();

        }

        //for (int i = 0; i < GridView1.Rows.Count; i++)
        //{
        //    System.Web.UI.WebControls.CheckBox chkSelect = (System.Web.UI.WebControls.CheckBox)GridView1.Rows[i].FindControl("chkSelect");
        //    System.Web.UI.WebControls.Label lblId = (System.Web.UI.WebControls.Label)GridView1.Rows[i].FindControl("lblId");
        //    System.Web.UI.WebControls.HiddenField hdStepStatus = (System.Web.UI.WebControls.HiddenField)GridView1.Rows[i].FindControl("hdStepStatus");
        //    if (chkSelect.Checked)
        //    {
        //        if (hdStepStatus.Value == "4" || hdStepStatus.Value == "3" || hdStepStatus.Value == "5")
        //        {
        //            strStepStatus = Convert.ToInt32(operation.ExecuteNonQuery(string.Format(@"update tblAgendaApprovalDetail set Reason='{0}',StepStatus={1},Status='{2}' where Id={3}", txtReason.Text.Replace("'", "''").Trim(), (Convert.ToInt32(hdStepStatus.Value) - 1), "Rejected", Convert.ToInt32(lblId.Text))));
        //        }
        //        else
        //        {
        //            strStepStatus = Convert.ToInt32(operation.ExecuteNonQuery(string.Format(@"update tblAgendaApprovalDetail set Reason='{0}',StepStatus={1} where Id={2}", txtReason.Text.Replace("'", "''").Trim(), (Convert.ToInt32(hdStepStatus.Value) - 1), Convert.ToInt32(lblId.Text))));
        //        }
        //        ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('File is Disapproved.')", true);
        //        Panel5.Visible = false;
        //        GetAgendaDetails();
        //    }
        //}

    }
    protected void btnNOnew_Click(object sender, EventArgs e)
    {
        Panel5.Visible = false;
        if (ViewState["Id"] != "" || ViewState["StepStatus"] != "")
        {
            ViewState["Id"] = "";
            ViewState["StepStatus"] = "";
        }
    }

    #region [bindLvFileView]
    public void bindLvFileView()
    {
        DataTable dsFieldName = new DataTable();
        DataSet dsFolderAccess = new DataSet();
        DataTable dtFolderdt = new DataTable();

        try
        {
            objFolderBAL = new FolderBAL();
            objCommonBAL = new CommonBAL();

            //if fodlerid empty then return to the page without doing anythings.
            #region If folderid equal to empty
            if (Convert.ToString(Session["FolderId"]) == "")
            {
                btnComplete.Visible = false;
                //Get access details on basis of groupid.
                dsFolderAccess = objFolderBAL.GeFolderId(Convert.ToInt32(Session["GroupID"]));

                //if folder has a right to access (Symbol's like 'F','M',"R','L')
                if (dsFolderAccess.Tables[0].Rows.Count > 0)
                {
                    //get folder detail on basis of folderid.
                    dtFolderdt = objCommonBAL.GetFolderDetail(Convert.ToInt64(dsFolderAccess.Tables[0].Rows[0]["FolderID"]));
                    if (dtFolderdt.Rows.Count > 0)
                    {
                        //set session of folder id to use through out application open
                        Session["FolderID"] = dtFolderdt.Rows[0]["FolderID"].ToString();
                    }
                }
                else
                {
                    //btnDelete.Visible = false;
                    btnExport.Visible = false;
                    btnSMS.Visible = false;
                    //btnMoveFile.Visible = false;
                    //lblSelect.Visible = false;
                    //lnkSelectAll.Visible = false;
                    //LinkButton2.Visible = false;
                    //btnCopyFiles.Visible = false;
                    btnComplete.Visible = false;
                    return;
                }
            }
            else
            {
                if (Session["GroupName"].ToString().ToLower() == "admin")
                {
                    DataTable dtFolderAccess = objCommonBAL.GetMeetingStatus(Convert.ToInt32(Session["FolderID"]));
                    //int statusofPhysicalTable = objCommonBAL.GetDataStatus(Convert.ToInt32(Session["FolderID"]));
                    if (dtFolderAccess.Rows.Count > 0)
                    {
                        //if (statusofPhysicalTable > 0)
                        //{
                        btnExport.Visible = true;
                        btnSMS.Visible = true;
                        //}
                        //else
                        //{
                        //    btnExport.Visible = false;
                        //}
                        int meetingStatus = Convert.ToInt16(dtFolderAccess.Rows[0]["MeetingStatus"]);
                        if (meetingStatus == 0 && Convert.ToInt32(Session["ParentFolderId"]) != 1)// && statusofPhysicalTable>0)
                            btnComplete.Visible = true;
                        else
                            btnComplete.Visible = false;
                        Session["MeetingStatus"] = meetingStatus.ToString();
                    }
                    else
                        btnComplete.Visible = false;
                }
            }
            #endregion
            /*added by nilesh sonpasare on 04/06/2012
             *  ConfigurationManager.AppSettings["AccessType"]
             * if AccessType userwise then pass value 1 and groupwise then pass 0 to 
             * CheckAccessRight,GetParentChildFolderId methodes.
             */

            //Check folder access and then do work as per rights to the selected folder.
            //string Symbol = objCommonBAL.GetSymbolstring(Convert.ToInt32(Session["FolderID"]), Convert.ToInt32(Session["GroupID"]), Convert.ToInt32(Session["UserID"]), ConfigurationManager.AppSettings["AccessType"].ToString().ToLower() == "userwise" ? 1 : 0);
            string Symbol = objCommonBAL.GetSymbolstring(Convert.ToInt32(Session["FolderID"]), Convert.ToInt32(Session["GroupID"]), Convert.ToInt32(Session["UserID"]), ConfigurationManager.AppSettings["AccessType"].ToString().ToLower() == "userwise" ? 1 : 0);
            //if symbol not equal to empty or group have a right to access folder.
            if (Symbol != "" || Symbol.ToUpper() != "N")
            {
                //Make button visible true false as per rights to the folder.
                #region Check symbol & make button true and false .
                switch (Symbol)
                {
                    case "F":
                        if (ConfigurationManager.AppSettings["Application"].ToString() == "hindalco")
                        {
                            if (Session["GroupName"].ToString().ToLower() == "admin")
                            {
                                //btnDelete.Visible = true;
                                btnExport.Visible = true;
                                btnSMS.Visible = true;
                                //btnMoveFile.Visible = true;
                                //lblSelect.Visible = true;
                                //lnkSelectAll.Visible = true;
                                //LinkButton2.Visible = true;
                                //btnCopyFiles.Visible = true;
                            }
                            else
                            {
                                //btnDelete.Visible = false;
                                btnExport.Visible = true;
                                btnSMS.Visible = true;
                                //btnMoveFile.Visible = false;
                                //lblSelect.Visible = true;
                                //lnkSelectAll.Visible = true;
                                //LinkButton2.Visible = true;
                                //btnCopyFiles.Visible = false;
                            }
                        }
                        else
                        {
                            if (Session["GroupName"].ToString().ToLower() == "admin")
                            {
                                //btnDelete.Visible = true;
                                btnExport.Visible = true;
                                btnSMS.Visible = true;
                                //btnMoveFile.Visible = true;
                                //lblSelect.Visible = true;
                                //lnkSelectAll.Visible = true;
                                //LinkButton2.Visible = true;
                                //btnCopyFiles.Visible = true;
                            }
                            else
                            {
                                //btnDelete.Visible = false;
                                btnExport.Visible = true;
                                btnSMS.Visible = true;
                                //btnMoveFile.Visible = true;
                                //lblSelect.Visible = true;
                                //lnkSelectAll.Visible = true;
                                //LinkButton2.Visible = true;
                                //btnCopyFiles.Visible = true;
                            }
                        }
                        break;
                    case "M":
                        if (ConfigurationManager.AppSettings["Application"].ToString() == "hindalco")
                        {
                            //btnDelete.Visible = false;
                            btnExport.Visible = true;
                            btnSMS.Visible = true;
                            //btnMoveFile.Visible = false;
                            //lblSelect.Visible = true;
                            //lnkSelectAll.Visible = true;
                            //LinkButton2.Visible = true;
                            //btnCopyFiles.Visible = false;
                        }
                        else
                        {
                            //btnDelete.Visible = false;
                            btnExport.Visible = true;
                            btnSMS.Visible = true;
                            //btnMoveFile.Visible = false;
                            //lblSelect.Visible = true;
                            //lnkSelectAll.Visible = true;
                            //LinkButton2.Visible = true;
                            //btnCopyFiles.Visible = false;
                        }
                        break;
                    case "R":
                        //btnDelete.Visible = false;
                        btnExport.Visible = false;
                        btnSMS.Visible = false;
                        //btnMoveFile.Visible = false;
                        //lblSelect.Visible = false;
                        //lnkSelectAll.Visible = false;
                        //LinkButton2.Visible = false;
                        //btnCopyFiles.Visible = false;

                        break;

                    case "L":
                        btnExport.Visible = false;
                        btnSMS.Visible = false;
                        //btnMoveFile.Visible = false;
                        //lblSelect.Visible = false;
                        //lnkSelectAll.Visible = false;
                        //LinkButton2.Visible = false;
                        //btnCopyFiles.Visible = false;

                        break;
                    case "N":
                        btnExport.Visible = false;
                        btnSMS.Visible = false;

                        break;
                }
                #endregion

                if (Convert.ToString(Session["Groupname"]).ToLower() == "admin")
                {
                    btnDelete.Visible = true;
                    btnAuthorizeAll.Visible = true;
                    chkSelectalll.Visible = true;
                    btnMoveDown.Visible = true;
                    btnInvitee.Visible = true;
                    btnPublish.Visible = true;
                    btnFirstSeperator.Visible = true;
                    btnLastSeperator.Visible = true;
                    btnOCR.Visible = false;


                }
                else
                {
                    btnDelete.Visible = false;
                    btnAuthorizeAll.Visible = false;
                    chkSelectalll.Visible = false;
                    btnMoveDown.Visible = false;
                    btnInvitee.Visible = false;
                    btnPublish.Visible = false;
                    btnFirstSeperator.Visible = false;
                    btnLastSeperator.Visible = false;
                    txtColor.Visible = false;
                    lblRestricted.Visible = false;
                }

                //bind data to gridview
                #region Bind Gridview Here .
                string TableName = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Convert.ToInt32(Session["FolderID"])));
                dsFieldName = operation.GetTable4Command(string.Format(@"select ID, a.fileid 'FileID',FileName,Column0 'Itemno',column1 'Particulars',column2 'Purpose',column3 'GM' from {0} a,tblfile b where a.fileid =b.fileid and b.folderid={1} order by column0", TableName, Convert.ToInt32(Session["FolderID"])));
                if (dsFieldName != null)
                {
                    if (dsFieldName.Rows.Count > 0)
                    {
                        gvParent.DataSource = null;
                        gvParent.DataSource = dsFieldName;
                        gvParent.DataBind();
                    }
                    else
                    {
                        //lblNoMeeting.Visible = true;
                        //tblMeetingInformation.Visible = false;
                        ViewState["RecordCnt"] = 0;
                        gvParent.DataSource = new int[0];
                        gvParent.DataBind();
                        btnComplete.Visible = false;
                        //btnDelete.Visible = false;
                        btnExport.Visible = false;
                        btnSMS.Visible = false;
                        //btnMoveFile.Visible = false;
                        //lblSelect.Visible = false;
                        //lnkSelectAll.Visible = false;
                        //LinkButton2.Visible = false;
                        //btnCopyFiles.Visible = false;
                        //lblRecord.Text = "";
                    }
                }
                btnExport.Visible = false;
                btnSMS.Visible = false;
                if (Session["GroupName"].ToString().ToLower().Trim() == "board secretariat user" || Session["Designation"].ToString().ToLower() == "board secretary" || Session["GroupName"].ToString().ToLower() == "admin" || Session["GroupName"].ToString().ToLower().Trim() == "president")
                {
                    gvParent.Visible = true;
                    if (gvParent.Rows.Count > 0)
                    {
                        btnMoveFile.Visible = true;
                        btnDelete.Visible = true;
                        btnAuthorizeAll.Visible = true;
                        chkSelectalll.Visible = true;
                        btnMoveDown.Visible = true;
                        btnInvitee.Visible = true;
                        btnPublish.Visible = true;
                        btnFirstSeperator.Visible = true;
                        btnLastSeperator.Visible = true;
                        btnOCR.Visible = false;


                    }
                    else
                    {
                        btnDelete.Visible = false;
                        btnAuthorizeAll.Visible = false;
                        chkSelectalll.Visible = false;
                        btnMoveDown.Visible = false;
                        btnInvitee.Visible = false;
                        btnPublish.Visible = false;
                        btnFirstSeperator.Visible = false;
                        btnLastSeperator.Visible = false;
                        txtColor.Visible = false;
                        lblRestricted.Visible = false;
                    }
                }
                else if (Session["GroupName"].ToString().ToLower() != "directors" && Convert.ToString(Session["Groupname"]).ToLower() != "general manager" && Session["GroupName"].ToString().ToLower() != "permanent invitees" && Session["GroupName"].ToString().ToLower() != "cfo" || Convert.ToString(Session["Groupname"]).ToLower() != "senior management" || Convert.ToString(Session["Groupname"]).ToLower() != "functional management" || Convert.ToString(Session["Groupname"]).ToLower() != "others")
                {
                    gvParent.Visible = false;
                    //foreach (GridViewRow i in gvParent.Rows)
                    //{
                    //    CheckBox chkSelect = (CheckBox)i.FindControl("chkSelect");
                    //    LinkButton lnkEdit = (LinkButton)i.FindControl("lnkEdit");
                    //    lnkEdit.Visible = false;
                    //    chkSelect.Enabled = false;
                    //    gvData.Columns[0].Visible = false;

                    //}

                }
                else
                {
                    gvParent.Visible = false;

                }

                #endregion
            }
            else
            {
                //btnDelete.Visible = false;
                //btnMoveFile.Visible = false;
                //btnCopyFiles.Visible = false;
                return;
            }
            gvData.Visible = false;
        }
        catch (Exception ex)
        {
            Session["Error"] = ex.ToString();
            Response.Redirect("../ErrorMessage.aspx", false);
        }
        finally
        {
            objCommonBAL = null;
            objFolderBAL = null;
            if (Convert.ToString(Session["FolderName"]).ToLower() == "Meetings\\recycle bin")
            {
                btnAuthorizeAll.Visible = false;
                btnDelete.Visible = false;
                chkSelectalll.Visible = false;
                btnMoveDown.Visible = false;
                btnInvitee.Visible = false;
                btnPublish.Visible = false;
                btnFirstSeperator.Visible = false;
                btnLastSeperator.Visible = false;
                txtColor.Visible = false;
                lblRestricted.Visible = false;
            }
        }
    }
    #endregion

    //protected void btnSMS_Click(object sender, EventArgs e)
    //{
    //    string Response = string.Empty;
    //    if (Session["UserName"] != null)
    //    {
    //        try
    //        {
    //            OperationClass objOperationClass = new OperationClass();
    //            string parentfoldername = objOperationClass.ExecuteScalar4Command(string.Format(@"select foldername from tblfolder where folderid=(select parentfolderid from tblfolder where folderid={0})", Convert.ToInt16(Session["FolderID"])));
    //            string foldername = objOperationClass.ExecuteScalar4Command(string.Format(@"select foldername from tblfolder where folderid={0}", Convert.ToInt16(Session["FolderID"])));
    //            string TableName = objOperationClass.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Convert.ToInt32(Session["FolderID"])));
    //            DataTable dtmin = objOperationClass.GetTable4Command(string.Format(@"select min(column0) 'Min',max(column0) 'Max' from {0}", TableName));
    //            string min = "", max = "";
    //            if (dtmin != null && dtmin.Rows.Count > 0)
    //            {
    //                min = dtmin.Rows[0]["Min"].ToString();
    //                max = dtmin.Rows[0]["Max"].ToString();
    //            }
    //            string msgText = "";
    //            string userName = ConfigurationSettings.AppSettings["UserName"].ToString();
    //            string password = ConfigurationSettings.AppSettings["Password"].ToString();
    //            string From = ConfigurationSettings.AppSettings["From"].ToString();
    //            //DataTable dt = objOperationClass.GetTable4Command(string.Format(@"Select distinct b.MobileNo,b.Id from tblUserAccesscontrol a inner join tbluserdetail b on a.userid=b.userid and a.FOLDERID IN(" + Convert.ToString(Session["FolderID"]) + ") and AccessSymbol not in('N') and b.MobileNo is not null"));
    //            DataTable dt = objOperationClass.GetTable4Command(string.Format(@"Select distinct b.MobileNo,b.UID from tblUserAccesscontrol a inner join tblAddressBook b on a.userid=b.userid inner join tbluserdetail c on a.userid=c.userid and a.FOLDERID IN(" + Convert.ToString(Session["FolderID"]) + ") and AccessSymbol not in('N') and b.MobileNo is not null  and bcl=1"));


    //            //msgText = "Dear Sir/Madam, " + parentfoldername + " Agenda No. " + min + " to " + max + " updated for the Meeting dated " + foldername + " Regards , Board Secretariat.";
    //            msgText = "Dear Sir/Madam, " + parentfoldername + " Agenda are updated for the Meeting dated " + foldername + " Regards , Board Secretariat.";
    //            int length = msgText.Length;
    //            //string[] arrno = { "9867087012", "9867087012", "9867087012" };
    //            //DataTable dt = objOperationClass.GetTable4Command(string.Format(@"Select b.ContactNo from tblUserAccesscontrol a inner join tbluserdetail b on a.userid=b.userid and a.FOLDERID IN(" + Convert.ToString(Session["FolderID"]) + ") and AccessSymbol not in('N') and b.GroupId in (select GroupID from dbo.tblWorkGroupMaster where lower(groupname) !='admin')"));
    //            //string[] arrno = { ConfigurationSettings.AppSettings["Number1"].ToString(),ConfigurationSettings.AppSettings["Number2"].ToString()};
    //            //for (int i = 0; i < arrno.Length; i++)
    //            //{
    //            string no1 = "";
    //            HDFCService.Service serr = new HDFCService.Service();

    //            for (int i = 0; i < dt.Rows.Count; i++)
    //            {

    //                no1 = dt.Rows[i]["MobileNo"].ToString();
    //                Response = serr.SEND_SMS(no1, msgText, ConfigurationSettings.AppSettings["SMSUserName"].ToString(), ConfigurationSettings.AppSettings["SMSPassword"].ToString());


    //                //string no1 = arrno[i].ToString();
    //                //string URL = "http://api.myvaluefirst.com/psms/servlet/psms.Eservice2?data=<?xml%20version=\"1.0\"%20encoding=\"ISO-8859-1\"?><!DOCTYPE%20MESSAGE%20SYSTEM%20\"http://127.0.0.1:80/psms/dtd/messagev12.dtd\"%20><MESSAGE%20VER=\"1.2\"><USER%20USERNAME=\"" + userName + "\"%20PASSWORD=\"" + password + "\"/><SMS%20UDH=\"0\"%20CODING=\"1\"%20TEXT=\"" + msgText + "\"%20PROPERTY=\"0\"%20ID=\"1\"><ADDRESS%20FROM=\"" + From + "\"%20TO=\"" + no1 + "\"%20SEQ=\"1\"%20TAG=\"some%20clientside%20random%20data\"%20/></SMS></MESSAGE>&action=send";

    //                //WebRequest myWebRequest = WebRequest.Create(URL);
    //                //WebResponse myWebResponse = myWebRequest.GetResponse();
    //                //Stream ReceiveStream = myWebResponse.GetResponseStream();
    //                //Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
    //                //StreamReader readStream = new StreamReader(ReceiveStream, encode);
    //                //string strResponse = readStream.ReadToEnd();

    //                string DeliveryStatus1 = "SMS sent to " + no1 + " with Message " + msgText + "" + "(" + Response + ")";
    //                string ParentFolderNamess = operation.ExecuteScalar4Command(string.Format(@"select FolderName  from tblFolder where folderId in (select ParentFolderId from tblFolder where folderId={0})", Convert.ToInt32(Session["FolderID"])));
    //                string CommitteFname = ParentFolderNamess + "//" + Session["DMeetingdate"].ToString();
    //                DateTime sfds = DateTime.Now;
    //                string sdasd = sfds.ToString("yyyy-MM-dd HH:mm:ss");
    //                string value1 = operation.ExecuteScalar4Command(string.Format(@"insert into tblfileactions (FileName,ActionName,UserID,ActionDate,Status,FolderId) values ('{0}','{1}',{2},'{3}','{4}',{5})", CommitteFname, DeliveryStatus1, Session["UserId"], sdasd, 1, Session["FolderId"].ToString()));
    //                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Message Sent Successfully')", true);

    //                //SMSServiceClient.SMSServiceService objUserDetails = new SMSServiceClient.SMSServiceService();
    //                //int output = objUserDetails.sendSMS(ConfigurationSettings.AppSettings["ApplicationID"].ToString(), no1, msgText);
    //                //if (output == 0)
    //                //{
    //                //    string DeliveryStatus1 = "SMS sent to " + no1 + "with Message" + msgText;
    //                //    string ParentFolderNamess = operation.ExecuteScalar4Command(string.Format(@"select FolderName  from tblFolder where folderId in (select ParentFolderId from tblFolder where folderId={0})", Convert.ToInt32(Session["FolderID"])));
    //                //    string CommitteFname = ParentFolderNamess + "//" + Session["DMeetingdate"].ToString();
    //                //    DateTime sfds = DateTime.Now;
    //                //    string sdasd = sfds.ToString("yyyy-MM-dd HH:mm:ss");
    //                //    string value1 = operation.ExecuteScalar4Command(string.Format(@"insert into tblfileactions (FileName,ActionName,UserID,ActionDate,Status,FolderId) values ('{0}','{1}',{2},'{3}','{4}',{5})", CommitteFname, DeliveryStatus1, Session["UserId"], sdasd, 1, Session["FolderId"].ToString()));
    //                //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Message Sent Successfully')", true);
    //                //}
    //                //else
    //                //{

    //                //    string DeliveryStatus1 = "SMS not sent to " + no1 + " Error:" + output;
    //                //    string ParentFolderNamess = operation.ExecuteScalar4Command(string.Format(@"select FolderName  from tblFolder where folderId in (select ParentFolderId from tblFolder where folderId={0})", Convert.ToInt32(Session["FolderID"])));
    //                //    string CommitteFname = ParentFolderNamess + "//" + Session["DMeetingdate"].ToString();
    //                //    DateTime sfds = DateTime.Now;
    //                //    string sdasd = sfds.ToString("yyyy-MM-dd HH:mm:ss");
    //                //    string value1 = operation.ExecuteScalar4Command(string.Format(@"insert into tblfileactions (FileName,ActionName,UserID,ActionDate,Status,FolderId) values ('{0}','{1}',{2},'{3}','{4}',{5})", CommitteFname, DeliveryStatus1, Session["UserId"], sdasd, 1, Session["FolderId"].ToString()));
    //                //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Message not sent')", true);
    //                //}


    //            }
    //            // ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Message Sent Successfully')", true);
    //            pnlSms.Visible = false;
    //        }
    //        catch (Exception ex)
    //        {
    //            //createlog("ErrorResponse :" + Response + "_" + ex.Message);
    //        }
    //    }

    //}


    protected void btnSMS_Click(object sender, EventArgs e)
    {
        string Response = string.Empty;
        if (Session["UserName"] != null)
        {
            string no1 = "";
            try
            {
                OperationClass objOperationClass = new OperationClass();
                string parentfoldername = objOperationClass.ExecuteScalar4Command(string.Format(@"select foldername from tblfolder where folderid=(select parentfolderid from tblfolder where folderid={0})", Convert.ToInt16(Session["FolderID"])));
                string foldername = objOperationClass.ExecuteScalar4Command(string.Format(@"select foldername from tblfolder where folderid={0}", Convert.ToInt16(Session["FolderID"])));
                //string TableName = objOperationClass.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Convert.ToInt32(Session["FolderID"])));
                //DataTable dtmin = objOperationClass.GetTable4Command(string.Format(@"select min(column0) 'Min',max(column0) 'Max' from {0}", TableName));
                //string min = "", max = "";
                //if (dtmin != null && dtmin.Rows.Count > 0)
                //{
                //    min = dtmin.Rows[0]["Min"].ToString();
                //    max = dtmin.Rows[0]["Max"].ToString();
                //}
                string msgText = "";
                //string userName = ConfigurationSettings.AppSettings["UserName"].ToString();
                //string password = ConfigurationSettings.AppSettings["Password"].ToString();
                //string From = ConfigurationSettings.AppSettings["From"].ToString();
                ////DataTable dt = objOperationClass.GetTable4Command(string.Format(@"Select distinct b.MobileNo,b.Id from tblUserAccesscontrol a inner join tbluserdetail b on a.userid=b.userid and a.FOLDERID IN(" + Convert.ToString(Session["FolderID"]) + ") and AccessSymbol not in('N') and b.MobileNo is not null"));
                //DataTable dt = objOperationClass.GetTable4Command(string.Format(@"Select distinct b.MobileNo,b.UID from tblUserAccesscontrol a inner join tblAddressBook b on a.userid=b.userid inner join tbluserdetail c on a.userid=c.userid and a.FOLDERID IN(" + Convert.ToString(Session["FolderID"]) + ") and AccessSymbol not in('N') and b.MobileNo is not null  and bcl=1"));

                //msgText = "Dear Sir/Madam, " + parentfoldername + " Agenda No. " + min + " to " + max + " updated for the Meeting dated " + foldername + " Regards , Board Secretariat.";
                msgText = "Dear Sir/Madam, " + parentfoldername + " Agenda are updated for the Meeting dated " + foldername + " Regards , Board Secretariat.";
                int length = msgText.Length;
                txtsmscomment.Text = msgText;
                //for (int i = 0; i < dt.Rows.Count; i++)
                //{
                //    //string no1 = arrno[i].ToString();
                //    no1 = dt.Rows[i]["MobileNo"].ToString();

                //    //Response = serr.SEND_SMS(no1, msgText, ConfigurationSettings.AppSettings["SMSUserName"].ToString(), ConfigurationSettings.AppSettings["SMSPassword"].ToString());
                //    string URL = "http://api.myvaluefirst.com/psms/servlet/psms.Eservice2?data=<?xml%20version=\"1.0\"%20encoding=\"ISO-8859-1\"?><!DOCTYPE%20MESSAGE%20SYSTEM%20\"http://127.0.0.1:80/psms/dtd/messagev12.dtd\"%20><MESSAGE%20VER=\"1.2\"><USER%20USERNAME=\"" + userName + "\"%20PASSWORD=\"" + password + "\"/><SMS%20UDH=\"0\"%20CODING=\"1\"%20TEXT=\"" + msgText + "\"%20PROPERTY=\"0\"%20ID=\"1\"><ADDRESS%20FROM=\"" + From + "\"%20TO=\"" + no1 + "\"%20SEQ=\"1\"%20TAG=\"some%20clientside%20random%20data\"%20/></SMS></MESSAGE>&action=send";
                //    //string URL = "https://paypoint.selcommobile.com/bulksms/dispatch56.php?msisdn=" + no1 + "&" + "user=" + userName + "&" + " password= " + password + "&message= " + msgText + "";
                //    WebRequest myWebRequest = WebRequest.Create(URL);
                //    WebResponse myWebResponse = myWebRequest.GetResponse();
                //    Stream ReceiveStream = myWebResponse.GetResponseStream();
                //    Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                //    StreamReader readStream = new StreamReader(ReceiveStream, encode);
                //    string strResponse = readStream.ReadToEnd();

                //    string DeliveryStatus1 = "SMS sent to " + no1 + " with Message " + msgText;
                //    string ParentFolderNamess = operation.ExecuteScalar4Command(string.Format(@"select FolderName  from tblFolder where folderId in (select ParentFolderId from tblFolder where folderId={0})", Convert.ToInt32(Session["FolderID"])));
                //    string CommitteFname = ParentFolderNamess + "//" + Session["DMeetingdate"].ToString();
                //    DateTime sfds = DateTime.Now;
                //    string sdasd = sfds.ToString("yyyy-MM-dd HH:mm:ss");
                //    string value1 = operation.ExecuteScalar4Command(string.Format(@"insert into tblfileactions (FileName,ActionName,UserID,ActionDate,Status,FolderId) values ('{0}','{1}',{2},'{3}','{4}',{5})", CommitteFname, DeliveryStatus1, Session["UserId"], sdasd, 1, Session["FolderId"].ToString()));
                //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Message Sent Successfully')", true);

                //    //SMSServiceClient.SMSServiceService objUserDetails = new SMSServiceClient.SMSServiceService();
                //    //int output = objUserDetails.sendSMS(ConfigurationSettings.AppSettings["ApplicationID"].ToString(), no1, msgText);
                //    //if (output == 0)
                //    //{
                //    //    string DeliveryStatus1 = "SMS sent to " + no1 + "with Message" + msgText;
                //    //    string ParentFolderNamess = operation.ExecuteScalar4Command(string.Format(@"select FolderName  from tblFolder where folderId in (select ParentFolderId from tblFolder where folderId={0})", Convert.ToInt32(Session["FolderID"])));
                //    //    string CommitteFname = ParentFolderNamess + "//" + Session["DMeetingdate"].ToString();
                //    //    DateTime sfds = DateTime.Now;
                //    //    string sdasd = sfds.ToString("yyyy-MM-dd HH:mm:ss");
                //    //    string value1 = operation.ExecuteScalar4Command(string.Format(@"insert into tblfileactions (FileName,ActionName,UserID,ActionDate,Status,FolderId) values ('{0}','{1}',{2},'{3}','{4}',{5})", CommitteFname, DeliveryStatus1, Session["UserId"], sdasd, 1, Session["FolderId"].ToString()));
                //    //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Message Sent Successfully')", true);
                //    //}
                //    //else
                //    //{

                //    //    string DeliveryStatus1 = "SMS not sent to " + no1 + " Error:" + output;
                //    //    string ParentFolderNamess = operation.ExecuteScalar4Command(string.Format(@"select FolderName  from tblFolder where folderId in (select ParentFolderId from tblFolder where folderId={0})", Convert.ToInt32(Session["FolderID"])));
                //    //    string CommitteFname = ParentFolderNamess + "//" + Session["DMeetingdate"].ToString();
                //    //    DateTime sfds = DateTime.Now;
                //    //    string sdasd = sfds.ToString("yyyy-MM-dd HH:mm:ss");
                //    //    string value1 = operation.ExecuteScalar4Command(string.Format(@"insert into tblfileactions (FileName,ActionName,UserID,ActionDate,Status,FolderId) values ('{0}','{1}',{2},'{3}','{4}',{5})", CommitteFname, DeliveryStatus1, Session["UserId"], sdasd, 1, Session["FolderId"].ToString()));
                //    //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Message not sent')", true);
                //    //}
                //    //createlogsms("Response :" + strResponse);

                //}
                //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Message Sent Successfully')", true);
                pnlSms.Visible = true;

            }
            catch (Exception ex)
            {
                createlog("mail :" + Response + "," + ex.Message);
                string DeliveryStatus1 = "SMS not sent to " + no1 + " Error:" + ex.Message;
                string ParentFolderNamess = operation.ExecuteScalar4Command(string.Format(@"select FolderName  from tblFolder where folderId in (select ParentFolderId from tblFolder where folderId={0})", Convert.ToInt32(Session["FolderID"])));
                string CommitteFname = ParentFolderNamess + "//" + Session["DMeetingdate"].ToString();
                DateTime sfds = DateTime.Now;
                string sdasd = sfds.ToString("yyyy-MM-dd HH:mm:ss");
                string value1 = operation.ExecuteScalar4Command(string.Format(@"insert into tblfileactions (FileName,ActionName,UserID,ActionDate,Status,FolderId) values ('{0}','{1}',{2},'{3}','{4}',{5})", CommitteFname, DeliveryStatus1, Session["UserId"], sdasd, 1, Session["FolderId"].ToString()));
            }
        }

    }


    public static void createlog(string msg)
    {
        try
        {
            if (!Directory.Exists("C:\\DessMeetingLogs"))
            {
                Directory.CreateDirectory("C:\\DessMeetingLogs");
            }
            string logformat = System.DateTime.Now.ToString("ddMMyyyy");
            StreamWriter sw = new StreamWriter("C:\\DessMeetingLogs\\Log_" + logformat + ".txt", true);

            sw.WriteLine(msg);
            sw.Close();

        }
        catch
        {

        }

    }


    //#region [SendMail]
    //protected void btnExport_Click(object sender, EventArgs e)
    //{
    //    R = 0;
    //    if (Session["UserName"] != null)
    //    {
    //        bool sendmail = false;
    //        OperationClass objOC = new OperationClass();
    //        GenericDAL objDAL = new GenericDAL();

    //        Object[] objparam = { Convert.ToInt32(Session["FolderId"]) };
    //        string MeetingNo = objOC.ExecuteScalar4Command(string.Format(@"select MeetingNo from tblFolder where FolderId=" + Convert.ToInt32(Session["FolderId"].ToString())));

    //        OperationClass objOperationClass = new OperationClass();
    //        DataTable dtEmailIDs = objDAL.ExecuteDataTable("SPGetMailOfUsersFolderwise", objparam);
    //        string emailIds = "";
    //        OperationClass operation = new OperationClass();
    //        string ParentFolderName = operation.ExecuteScalar4Command(string.Format(@"select FolderName  from tblFolder where folderId in (select ParentFolderId from tblFolder where folderId={0})", Convert.ToInt32(Session["FolderID"])));
    //        string FolderName = operation.ExecuteScalar4Command(string.Format(@"select FolderName from tblFolder where folderId={0}", Convert.ToInt32(Session["FolderID"])));
    //        string FullFolderName = Session["FolderName"].ToString();

    //        DataTable dtLocationTime = objOperationClass.GetTable4Command("select Time,(select Fulladdress from tblLocationDetails where locationid=a.Locationid)Location from tblfolderindexmaster a where Folder_id='" + Convert.ToInt32(Session["FolderID"]) + "'");
    //        int len = FullFolderName.Length - (FullFolderName.IndexOf('\\') + 1) - (FullFolderName.Substring(FullFolderName.LastIndexOf('\\'))).Length;
    //        Session["FolderName"].ToString().Substring((Session["FolderName"].ToString().IndexOf('\\') + 1));
    //        DateTime MyDateTime = new DateTime();
    //        MyDateTime = DateTime.ParseExact(FolderName, "dd/MM/yyyy", null);


    //        string Send_DateOfMeeting = String.Format(" {0:MMMM d, yyyy }", MyDateTime);
    //        string Send_DayOfMeeting = String.Format(" {0:dddd}", MyDateTime);

    //        string CompanyName = "";
    //        foreach (DataRow rr in dtEmailIDs.Rows)
    //        {
    //            emailIds += rr[0].ToString() + ",";
    //            emailIds = rr[0].ToString() + ",";

    //            }
    //            emailIds = emailIds.Substring(0, emailIds.LastIndexOf(','));
    //            StringBuilder sb = new StringBuilder();
    //            HttpContext context = HttpContext.Current;
    //            if (ParentFolderName.Contains('-'))
    //            {
    //                if (ParentFolderName.Substring(0, ParentFolderName.LastIndexOf('-')).Trim().ToLower() == "titan")
    //                {
    //                    CompanyName = "Titan Company Limited.";
    //                     CompanyName = ConfigurationManager.AppSettings["CompanyName"].ToString();

    //                }
    //                else
    //                    if (ParentFolderName.Substring(0, ParentFolderName.LastIndexOf('-')).Trim().ToLower() == "ttpl")
    //                    {
    //                        CompanyName = "Titan TimeProducts Limited.";
    //                        CompanyName = ConfigurationManager.AppSettings["CompanyName"].ToString();

    //                    }
    //                    else
    //                    {
    //                        CompanyName = "Titan Electronics and Automation Limited.";
    //                         CompanyName = ConfigurationManager.AppSettings["CompanyName"].ToString();

    //                    }
    //            }
    //            else
    //            {
    //                 CompanyName = "Titan Company Limited.";
    //                CompanyName = ConfigurationManager.AppSettings["CompanyName"].ToString();
    //            }



    //            sb.Append("<table>");
    //            sb.Append("<tr>");
    //            sb.Append("<td align='left'>");
    //            sb.Append("Respected Sir/Madam<br />");

    //            sb.Append("<p>Agenda have been uploaded on Director's Portal by <B>Secretarial Team</B> for " +
    //             " the <B> " + ParentFolderName + "</B> scheduled on <B>" + FolderName + "</B>");

    //            sb.Append("</td>");
    //            sb.Append("</tr>");
    //            sb.Append("<tr>");
    //            sb.Append("<td align='left'>");
    //            string[] strTimeNew = dtLocationTime.Rows[0]["Time"].ToString().Split(new Char[] { ':' });
    //            sb.Append("<p>This is to bring to your notice that the Agenda for the meeting of the " + ParentFolderName + " scheduled for " + Send_DayOfMeeting + "," + Send_DateOfMeeting + "," + Convert.ToInt32(strTimeNew[0]).ToString("00") + ":" + strTimeNew[1].ToUpper() + " at " + dtLocationTime.Rows[0]["Location"].ToString() + "has been uploaded on Digital Meeting Portal.</p>");
    //            sb.Append("</td>");
    //            sb.Append("</tr>");
    //            sb.Append("<tr>");
    //            sb.Append("<td align='left'>");
    //            sb.Append("<b>Shoppers Stop Ltd. is uploaded in the Meeting Portal.</b><br/>");
    //            sb.Append("</td>");
    //            sb.Append("</tr>");
    //            sb.Append("<tr>");
    //            sb.Append("<td align='left'>");
    //            sb.Append("<br/>");
    //            sb.Append("</td>");
    //            sb.Append("</tr>");

    //            sb.Append("<tr>");
    //            sb.Append("<td>");
    //            sb.Append("</td>");
    //            sb.Append("</tr>");


    //            sb.Append("<table>");

    //            if (ParentFolderName.Contains('-'))
    //            {
    //                if (ParentFolderName.Substring(0, ParentFolderName.LastIndexOf('-')).Trim().ToLower() == "titan")
    //                {
    //                    sb.Append("<tr>");
    //                    sb.Append("<td align='left'>");
    //                    sb.Append("<b><B>Regards,</B></b>");
    //                    sb.Append("</td>");
    //                    sb.Append("</tr>");
    //                    sb.Append("<tr>");
    //                    sb.Append("<td align='left'>");
    //                    sb.Append("A R Rajaram");
    //                     sb.Append(Session["Name"].ToString());

    //                    sb.Append("</td>");
    //                    sb.Append("</tr>");
    //                    sb.Append("<tr>");
    //                    sb.Append("<td align='left'>");
    //                    sb.Append("Head – Legal & Company Secretary");
    //                    sb.Append("</td>");
    //                    sb.Append("</tr>");
    //                    sb.Append("<tr>");
    //                    sb.Append("<td align='left'>");
    //                    sb.Append("Titan Company Limited");
    //                    sb.Append(ConfigurationManager.AppSettings["CompanyName"].ToString());

    //                    sb.Append("</td>");
    //                    sb.Append("</tr>");
    //                }
    //                else
    //                    if (ParentFolderName.Substring(0, ParentFolderName.LastIndexOf('-')).Trim().ToLower() == "ttpl")
    //                    {



    //                        sb.Append("<tr>");
    //                        sb.Append("<td align='left'>");
    //                        sb.Append("Thank you.");
    //                        sb.Append("</td>");
    //                        sb.Append("</tr>");
    //                        sb.Append("<tr>");
    //                        sb.Append("<td align='left'>");
    //                        sb.Append("Yours truly,");
    //                        sb.Append("</td>");
    //                        sb.Append("</tr>");
    //                        sb.Append("<tr>");
    //                        sb.Append("<td align='left'>");
    //                        sb.Append("for  Titan TimeProducts Limited");
    //                        sb.Append(ConfigurationManager.AppSettings["CompanyName"].ToString());
    //                        sb.Append("</td>");
    //                        sb.Append("</tr>");
    //                        sb.Append("<tr>");
    //                        sb.Append("<td align='left'>");
    //                        sb.Append("A R Rajaram");
    //                        sb.Append(Session["Name"].ToString());
    //                        sb.Append("</td>");
    //                        sb.Append("</tr>");


    //                    }
    //                    else
    //                    {
    //                        sb.Append("<tr>");
    //                        sb.Append("<td align='left'>");
    //                        sb.Append("Thank you.");
    //                        sb.Append("</td>");
    //                        sb.Append("</tr>");
    //                        sb.Append("<tr>");
    //                        sb.Append("<td align='left'>");
    //                        sb.Append("Yours truly,");
    //                        sb.Append("</td>");
    //                        sb.Append("</tr>");
    //                        sb.Append("<tr>");
    //                        sb.Append("<td align='left'>");
    //                        sb.Append("for  Titan Electronics and Automation Limited");
    //                        sb.Append(ConfigurationManager.AppSettings["CompanyName"].ToString());
    //                        sb.Append("</td>");
    //                        sb.Append("</tr>");
    //                        sb.Append("<tr>");
    //                        sb.Append("<td align='left'>");
    //                        sb.Append("A R Rajaram");
    //                        sb.Append(Session["Name"].ToString());
    //                        sb.Append("</td>");
    //                        sb.Append("</tr>");


    //                    }
    //            }
    //            else
    //            {
    //                sb.Append("<tr>");
    //                sb.Append("<td align='left'>");
    //                sb.Append("<b><B>Regards,</B></b>");
    //                sb.Append("</td>");
    //                sb.Append("</tr>");
    //                sb.Append("<tr>");
    //                sb.Append("<td align='left'>");
    //                sb.Append("A R Rajaram");
    //                sb.Append(Session["Name"].ToString());
    //                sb.Append("</td>");
    //                sb.Append("</tr>");
    //                sb.Append("<tr>");
    //                sb.Append("<td align='left'>");
    //                sb.Append("Head – Legal & Company Secretary");
    //                sb.Append("</td>");
    //                sb.Append("</tr>");
    //                sb.Append("<tr>");
    //                sb.Append("<td align='left'>");
    //                sb.Append("Titan Company Limited");
    //                sb.Append(ConfigurationManager.AppSettings["CompanyName"].ToString());
    //                sb.Append("</td>");
    //                sb.Append("</tr>");
    //            }
    //            sb.Append("</table>");
    //             sb.Append("</td>");
    //             sb.Append("</tr>");

    //              sb.Append("<tr>");
    //              sb.Append("<td align='left'>");
    //             sb.Append(Environment.NewLine + "<br />");
    //              sb.Append("<tr/>");
    //            sb.Append("<tr><td><b>Regards," + "</b><br />");                
    //             sb.Append("Company Secretary.</td></tr>");
    //             sb.Append("</td></tr></table>");
    //            Boolean DeliveryStatus = false;
    //            if (ParentFolderName.Contains('-'))
    //            {
    //                DeliveryStatus = sendMail(emailIds, sb.ToString(), "Agenda for the meeting of the " + ParentFolderName.Substring(ParentFolderName.LastIndexOf('-') + 1, (ParentFolderName.Length - ParentFolderName.LastIndexOf('-')) - 1) + " of " + CompanyName + " with Serial No.(" + MeetingNo + ") scheduled to be held on " + FolderName + "");
    //            }
    //            else
    //            {
    //                DeliveryStatus = sendMail(emailIds, sb.ToString(), "Agenda for the meeting of the " + ParentFolderName + " of " + CompanyName + " with Serial No.(" + MeetingNo + ") scheduled to be held on " + FolderName + "");
    //            }

    //            if (DeliveryStatus == true)
    //            {
    //                string ParentFolderNames = operation.ExecuteScalar4Command(string.Format(@"select FolderName  from tblFolder where folderId in (select ParentFolderId from tblFolder where folderId={0})", Convert.ToInt32(Session["FolderID"])));
    //                string CommitteFname = ParentFolderNames + "//" + Session["DMeetingdate"].ToString();
    //                string delivery = "Agenda Email sent to " + emailIds + " with Subject Alert for Agenda upload";
    //                DateTime sfds = DateTime.Now;
    //                string sdasd = sfds.ToString("yyyy-MM-dd HH:mm:ss");
    //                string value1 = operation.ExecuteScalar4Command(string.Format(@"insert into tblfileactions (FileName,ActionName,UserID,ActionDate,Status,FolderId) values ('{0}','{1}',{2},'{3}','{4}',{5})", CommitteFname, delivery, Session["UserId"], sdasd, 1, Session["FolderId"].ToString()));
    //            }
    //            else
    //            {

    //                ParentFolderName = operation.ExecuteScalar4Command(string.Format(@"select FolderName  from tblFolder where folderId in (select ParentFolderId from tblFolder where folderId={0})", Convert.ToInt32(Session["FolderID"])));
    //                string CommitteFname = ParentFolderName + "//" + Session["DMeetingdate"].ToString();
    //                DateTime sfds = DateTime.Now;
    //                string sdasd = sfds.ToString("yyyy-MM-dd HH:mm:ss");
    //                string value1 = operation.ExecuteScalar4Command(string.Format(@"insert into tblfileactions (FileName,ActionName,UserID,ActionDate,Status,FolderId) values ('{0}','{1}',{2},'{3}','{4}',{5})", CommitteFname, delivery, Session["UserId"], sdasd, 1, Session["FolderId"].ToString()));
    //            }

    //        }

    //        ScriptManager.RegisterStartupScript(Page, this.GetType(), "msg", "alert('Email has been sent.')", true);


    //    }

    //#endregion


    #region [SendMail]
    protected void btnExport_Click(object sender, EventArgs e)
    {
        R = 0;
        string AgendaMailContent = "";
        string Subject = "";
        if (Session["UserName"] != null)
        {
            pnlpopup.Visible = true;
            bool sendmail = false;
            OperationClass operation = new OperationClass();
            string CompanyName = "";
            string FolderName = operation.ExecuteScalar4Command(string.Format(@"select FolderName from tblFolder where folderId={0}", Convert.ToInt32(Session["FolderID"])));
            string FullFolderName = Session["FolderName"].ToString();

            DataTable dtLocationTime = operation.GetTable4Command("select Time,(select Fulladdress from tblLocationDetails where locationid=a.Locationid)Location from tblfolderindexmaster a where Folder_id='" + Convert.ToInt32(Session["FolderID"]) + "'");
            string[] strTimeNew = dtLocationTime.Rows[0]["Time"].ToString().Split(new Char[] { ':' });
           


            DateTime MyDateTime = new DateTime();
            MyDateTime = DateTime.ParseExact(FolderName, "dd/MM/yyyy", null);

            string Send_DateOfMeeting = String.Format(" {0:MMMM d, yyyy }", MyDateTime);
            string Send_DayOfMeeting = String.Format(" {0:dddd}", MyDateTime);

            string ParentFolderName = operation.ExecuteScalar4Command(string.Format(@"select FolderName  from tblFolder where folderId in (select ParentFolderId from tblFolder where folderId={0})", Convert.ToInt32(Session["FolderID"])));
            StringBuilder sb = new StringBuilder();
            HttpContext context = HttpContext.Current;
            if (ParentFolderName.Contains('-'))
            {

            }
            else
            {
                // CompanyName = "Titan Company Limited.";
                CompanyName = ConfigurationManager.AppSettings["CompanyName"].ToString();
            }







            if (ParentFolderName.Contains('-'))
            {

            }
            else
            {
                int k = 0;
                foreach (GridViewRow i in gvData.Rows)
                {
                    CheckBox chSelect = (CheckBox)i.FindControl("chkSelect");
                    //lblApprove = (Label)i.FindControl("lblApprovalStatus");

                    if (chSelect.Checked)
                    {
                        sendmail = true;
                        Label AgendaNo = (Label)i.FindControl("LinkButton1");
                        LinkButton AgendaParticular = (LinkButton)i.FindControl("lnkOpen1");
                        if (k == 0)
                        {
                            sb.Append("<table>");
                            sb.Append("<tr>");
                            sb.Append("<td>");
                            sb.Append("Sr No.");
                            sb.Append("</td>");

                            sb.Append("<td>");
                            sb.Append("Description");
                            sb.Append("</td>");
                            sb.Append("<tr/>");
                            k++;
                        }
                        sb.Append("<tr>");
                        sb.Append("<td>");
                        sb.Append(AgendaNo.Text);
                        sb.Append("</td>");
                        sb.Append("<td>");
                        sb.Append(AgendaParticular.Text);
                        sb.Append("</td>");
                        sb.Append("<tr/>");

                    }

                }
                sb.Append("</table>");
            }
            sb.Append("</table>");

            string MeetingNo = operation.ExecuteScalar4Command(string.Format(@"select MeetingNo from tblFolder where FolderId=" + Convert.ToInt32(Session["FolderId"].ToString())));


            lblsubject.Text = operation.ExecuteScalar4Command(string.Format(@"select AgendaSubject from tblMailContent where EmailType=2")).Replace("[Committee]", ParentFolderName).Replace("[CompanyName]", CompanyName).Replace("[MeetingNo]", MeetingNo).Replace("[Date]", FolderName);

            txtsmscomment.Text = "Dear Sir/Madam, " + ParentFolderName + " Agenda are updated for the Meeting dated " + FolderName + " Regards , Board Secretariat.";

            StringBuilder strAgendaMailContent = new StringBuilder();
            strAgendaMailContent.Append(operation.ExecuteScalar4Command(string.Format(@"select AgendaMailContent from tblMailContent where EmailType=2")));

            strAgendaMailContent.Replace("[Committee]", ParentFolderName);
            strAgendaMailContent.Replace("[Date]", Send_DateOfMeeting);
            strAgendaMailContent.Replace("[Time]", Convert.ToInt32(strTimeNew[0]).ToString("00") + ":" + strTimeNew[1].ToUpper());
            strAgendaMailContent.Replace("[Location]", dtLocationTime.Rows[0]["Location"].ToString());
            strAgendaMailContent.Replace("[CompanyName]", ConfigurationManager.AppSettings["CompanyName"].ToString());
            strAgendaMailContent.Replace("[Description]", sb.ToString());
            if (ClientScript.IsStartupScriptRegistered("BindGroupName") == false)
                ClientScript.RegisterStartupScript(GetType(), "Ispostback", "javascript:ShowPopup('" + strAgendaMailContent.ToString() + "');", true);



        }

    }
    #endregion

    [WebMethod(EnableSession = true)]
    public static void InsertMeetingData(string MeetingData)
    {
        HttpContext.Current.Session["MeetingData"] = MeetingData;

    }
    protected void btnMeetingnotice_Click(object sender, EventArgs e)
    {

        OperationClass operation = new OperationClass();
        GenericDAL objDAL = new GenericDAL();

        Object[] objparam = { Convert.ToInt32(Session["FolderId"]) };

        DataTable dtEmailIDs = objDAL.ExecuteDataTable("SPGetMailOfUsersFolderwise", objparam);
        string emailIds = "";

        string ParentFolderName = operation.ExecuteScalar4Command(string.Format(@"select FolderName  from tblFolder where folderId in (select ParentFolderId from tblFolder where folderId={0})", Convert.ToInt32(Session["FolderID"])));
        string FolderName = operation.ExecuteScalar4Command(string.Format(@"select FolderName from tblFolder where folderId={0}", Convert.ToInt32(Session["FolderID"])));
        string FullFolderName = Session["FolderName"].ToString();



        foreach (DataRow rr in dtEmailIDs.Rows)
        {

            emailIds = rr[0].ToString() + ",";
            emailIds = emailIds.Substring(0, emailIds.LastIndexOf(','));
            sendMail(emailIds, Session["MeetingData"].ToString(), lblsubject.Text);
        }
        SaveAgendaEmailContaint(Session["MeetingData"].ToString(), lblsubject.Text);
        pnlpopup.Visible = false;
        ScriptManager.RegisterStartupScript(Page, this.GetType(), "msg", "alert('Mail Updated and Sent Successfully.')", true);
        DirectorDataBinding();
    }

    public void SaveAgendaEmailContaint(string AgendaMailContent, string subject)
    {
        try
        {
            OperationClass operation = new OperationClass();
            string value1 = operation.ExecuteScalar4Command(string.Format(@"update tblMailContent set AgendaMailContent='{0}',AgendaSubject='{1}' where Folderid={2} and Status={3}", AgendaMailContent, subject, Convert.ToInt32(Session["FolderId"].ToString()), 1));
        }
        catch (Exception ex)
        {

        }
    }


    protected void btnCancelling_Click(object sender, EventArgs e)
    {

        pnlpopup.Visible = false;
        DirectorDataBinding();

    }



    public Boolean sendMail(string mailid, string mailmessage, string subject)
    {
        string CommitteFname = "";
        string ParentFolderName = "";
        Boolean DeliveryStatus;
        String delivery = "";

        System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();

        try
        {
            //Button 2 = gmail 
            string MailFrom = Convert.ToString(ConfigurationManager.AppSettings["EmailFrom"]);
            mail.From = new System.Net.Mail.MailAddress(MailFrom);

            mail.To.Add(mailid);
            mail.Subject = subject;
            mail.Body = mailmessage;
            mail.IsBodyHtml = true;

            System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
            client.EnableSsl = true;
            client.Send(mail);
            //End of Button 2
            //Button 4
            //OperationClass objOperationClass = new OperationClass();
            //System.Net.Mail.MailMessage m = new System.Net.Mail.MailMessage();

            //string MailFrom = Convert.ToString(ConfigurationManager.AppSettings["EmailFrom"]);
            //string Password = Convert.ToString(ConfigurationManager.AppSettings["EmailPassword"]);
            //m.From = new System.Net.Mail.MailAddress(Convert.ToString(MailFrom));
            //m.To.Add(mailid);
            //m.Subject = subject;
            ////m.Subject = subject;
            //m.Body = mailmessage;

            ////m.To.Add("sanjeev.sengar@ashokpiramalgroup.com");          
            //m.IsBodyHtml = true;
            ////m.To.Add(new System.Net.Mail.MailAddress("nareshshriniwass@gmail.com"));
            //System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
            //smtp.Host = Convert.ToString(ConfigurationManager.AppSettings["EmailHost"]);
            //smtp.EnableSsl = true;
            //NetworkCredential authinfo = new NetworkCredential(MailFrom, Password);
            //smtp.UseDefaultCredentials = false;
            //smtp.Credentials = authinfo;
            //smtp.Send(m);
            //End of Button 4
            //mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;
            //delivery = mail.DeliveryNotificationOptions.ToString();


            // Exide code
            //MailMessage mail = new MailMessage();
            //string MailFrom = Convert.ToString(ConfigurationManager.AppSettings["EmailFrom"]);
            //mail.From = MailFrom;
            //mail.To = mailid;

            //mail.Subject = subject;
            //mail.Body = mailmessage;
            //mail.BodyFormat = MailFormat.Html;
            //string EmailHost = Convert.ToString(ConfigurationManager.AppSettings["EmailHost"]);
            //SmtpMail.SmtpServer = EmailHost;
            //SmtpMail.Send(mail);

            //mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;
            //delivery = mail.DeliveryNotificationOptions.ToString();

            //if (delivery != "OnFailure")
            //{


            //string DeliveryStatus1 = "Email send to " + mailid + "  " + delivery;
            //ParentFolderName = operation.ExecuteScalar4Command(string.Format(@"select FolderName  from tblFolder where folderId in (select ParentFolderId from tblFolder where folderId={0})", Convert.ToInt32(Session["FolderID"])));
            //CommitteFname = ParentFolderName + "//" + Session["DMeetingdate"].ToString();
            //DateTime sfds = DateTime.Now;
            //string sdasd = sfds.ToString("yyyy-MM-dd HH:mm:ss");
            //string value1 = operation.ExecuteScalar4Command(string.Format(@"insert into tblfileactions (FileName,ActionName,UserID,ActionDate,Status,FolderId) values ('{0}','{1}',{2},'{3}','{4}',{5})", CommitteFname, DeliveryStatus1, Session["UserId"], sdasd, 1, Session["FolderId"].ToString()));



            //}
            //string DeliveryStatus1 = "Email send to " + mailid + "  " + delivery;
            //ParentFolderName = operation.ExecuteScalar4Command(string.Format(@"select FolderName  from tblFolder where folderId in (select ParentFolderId from tblFolder where folderId={0})", Convert.ToInt32(Session["FolderID"])));
            //CommitteFname = ParentFolderName + "//" + Session["DMeetingdate"].ToString();
            //DateTime sfds = DateTime.Now;
            //string sdasd = sfds.ToString("yyyy-MM-dd HH:mm:ss");
            //string value1 = operation.ExecuteScalar4Command(string.Format(@"insert into tblfileactions (FileName,ActionName,UserID,ActionDate,Status,FolderId) values ('{0}','{1}',{2},'{3}','{4}',{5})", CommitteFname, DeliveryStatus1, Session["UserId"], sdasd, 1, Session["FolderId"].ToString()));


            return DeliveryStatus = true;
        }
        catch (Exception ex)
        {
            //mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
            //delivery = mail.DeliveryNotificationOptions.ToString();
            //ParentFolderName = operation.ExecuteScalar4Command(string.Format(@"select FolderName  from tblFolder where folderId in (select ParentFolderId from tblFolder where folderId={0})", Convert.ToInt32(Session["FolderID"])));
            //CommitteFname = ParentFolderName + "//" + Session["DMeetingdate"].ToString();
            //string DeliveryStatus1 = "Email not send to " + mailid + "  " + delivery;
            //DateTime sfds = DateTime.Now;
            //string sdasd = sfds.ToString("yyyy-MM-dd HH:mm:ss");
            //string value1 = operation.ExecuteScalar4Command(string.Format(@"insert into tblfileactions (FileName,ActionName,UserID,ActionDate,Status,FolderId) values ('{0}','{1}',{2},'{3}','{4}',{5})", CommitteFname, DeliveryStatus1, Session["UserId"], sdasd, 1, Session["FolderId"].ToString()));
            //return DeliveryStatus = false;

            ParentFolderName = operation.ExecuteScalar4Command(string.Format(@"select FolderName  from tblFolder where folderId in (select ParentFolderId from tblFolder where folderId={0})", Convert.ToInt32(Session["FolderID"])));
            CommitteFname = ParentFolderName + "//" + Session["DMeetingdate"].ToString();
            string DeliveryStatus1 = "Email not sent to " + mailid + "  " + delivery;
            DateTime sfds = DateTime.Now;
            string sdasd = sfds.ToString("yyyy-MM-dd HH:mm:ss");
            string value1 = operation.ExecuteScalar4Command(string.Format(@"insert into tblfileactions (FileName,ActionName,UserID,ActionDate,Status,FolderId) values ('{0}','{1}',{2},'{3}','{4}',{5})", CommitteFname, DeliveryStatus1, Session["UserId"], sdasd, 1, Session["FolderId"].ToString()));
            return DeliveryStatus = false;
        }

    }


    protected void chkSelectalll_OnCheckedChanged(object sender, EventArgs e)
    {
        if (chkSelectalll.Checked == true)
        {
            foreach (GridViewRow row in gvData.Rows)
            {
                CheckBox chkSelect = (CheckBox)row.FindControl("chkSelect");
                chkSelect.Checked = true;
            }
        }
        else
        {
            foreach (GridViewRow row in gvData.Rows)
            {
                CheckBox chkSelect = (CheckBox)row.FindControl("chkSelect");
                chkSelect.Checked = false;
            }
        }

    }


    //#region [btnDelete_Click]
    //protected void btnDelete_Click(object sender, EventArgs e)
    //{
    //    try
    //    {
    //        objCommonBAL = new CommonBAL();

    //        ViewState["Delete"] = "True";

    //        lblMessage.Text = "";
    //        string strFileId = "";
    //        string strFileIddl = "";

    //        //get chechbox value
    //        if (Convert.ToInt16(Session["ParentFolderId"]) == 1)
    //            strFileId = objCommonBAL.GetCheckBoxCheckValue(gvParent);
    //        else
    //            strFileId = objCommonBAL.GetCheckBoxCheckValue(gvData);
    //        if (strFileId != "")
    //        {
    //            //store value for get on get btnyes click.
    //            ViewState["strFileId"] = strFileId;

    //            lblRplMessage0.Text = "Are you sure you want to delete selected files ?";
    //            Panel4.Visible = true;
    //            ddlCommitteenew.Visible = true;
    //            OperationClass objOperationClass = new OperationClass();
    //            string SqlQuery;
    //            SqlQuery = "SELECT [FolderName], [FolderId] FROM [tblFolder] WHERE ([ParentFolderId] = 1) AND [MEETINGSTATUS]!=1 AND [MEETINGCANCELLED]!=1 and FolderName='Recycle Bin'";
    //            DataTable dt = objOperationClass.GetTable4Command(SqlQuery);
    //            if (dt.Rows.Count != 0)
    //            {
    //                ddlCommitteenew.DataSource = dt;
    //                ddlCommitteenew.DataTextField = "FolderName";
    //                ddlCommitteenew.DataValueField = "FolderId";
    //                ddlCommitteenew.DataBind();
    //            }

    //            btnNO.Focus();

    //            objFileUploadBAL = new FileUploadBAL();
    //            objFileUploadController = new FileUploadController();
    //            int value2;
    //            if (ViewState["strFileId"] != null)
    //            {
    //                strFileIddl = (string)ViewState["strFileId"];

    //                if (strFileIddl != "")
    //                {
    //                    try
    //                    {
    //                        string strrrr = operation.ExecuteScalar4Command(string.Format(@"select ParentFolderId from tblfolder where folderid={0}", Session["FolderID"].ToString()));
    //                        if (strrrr != "1")
    //                        {
    //                            string TableNamenew = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", ddlCommitteenew.SelectedValue));
    //                            foreach (GridViewRow row in gvData.Rows)
    //                            {
    //                                CheckBox chkSelect = (CheckBox)row.FindControl("chkSelect");
    //                                if (chkSelect.Checked)
    //                                {
    //                                    Label Id = (Label)row.FindControl("lblId");
    //                                    Label FileId = (Label)row.FindControl("lblFileId");
    //                                    Label lblApprovalStatus = (Label)row.FindControl("lblApprovalStatus");
    //                                    LinkButton FileName = (LinkButton)row.FindControl("lnkOpen");
    //                                    string[] FileNameID = FileName.CommandArgument.ToString().Split(new Char[] { ',' });
    //                                    string[] filename = FileNameID[1].Split(new Char[] { '.' });
    //                                    string name = filename[0].ToString();
    //                                    DataSet dss = new DataSet();


    //                                    if (System.IO.File.Exists(Server.MapPath("~\\Repository") + "\\" + name + "_" + Session["FolderID"].ToString() + ".enc"))
    //                                    {
    //                                        if (!System.IO.File.Exists(Server.MapPath("~\\Repository") + "\\" + name + "_" + ddlCommitteenew.SelectedValue.ToString() + ".enc"))
    //                                        {
    //                                            System.IO.File.Move(Server.MapPath("~\\Repository") + "\\" + name + "_" + Session["FolderID"].ToString() + ".enc", Server.MapPath("~\\Repository") + "\\" + name + "_" + ddlCommitteenew.SelectedValue.ToString() + ".enc");

    //                                            dss = operation.GetDataSet4Command(string.Format(@"select * from tblFolderIndexMaster where folder_id={0}", Session["FolderID"].ToString()));
    //                                            if (dss.Tables[0].Rows.Count > 0)
    //                                            {
    //                                                DataSet dsMeet = operation.GetDataSet4Command(string.Format(@"select * from {0} where FileId={1}", dss.Tables[0].Rows[0]["TableName"].ToString(), Convert.ToInt32(FileId.Text.ToString())));

    //                                                if (dsMeet.Tables[0].Rows.Count > 0)
    //                                                {

    //                                                    string ImportedBy = dsMeet.Tables[0].Rows[0]["ImportedBy"].ToString();
    //                                                    string DocStatus = dsMeet.Tables[0].Rows[0]["DocStatus"].ToString();
    //                                                    string Column3 = dsMeet.Tables[0].Rows[0]["Column3"].ToString();
    //                                                    string Column0 = dsMeet.Tables[0].Rows[0]["Column0"].ToString();
    //                                                    string Column1 = dsMeet.Tables[0].Rows[0]["Column1"].ToString();
    //                                                    string Column2 = dsMeet.Tables[0].Rows[0]["Column2"].ToString();
    //                                                    string value1 = operation.ExecuteScalar4Command(string.Format(@"insert into {0} (FileId,ImportedBy,DocStatus,FolderId,Column0,Column1,Column2,Column3) values ('" + Convert.ToInt32(FileId.Text.ToString()) + "','" + ImportedBy + "','" + DocStatus + "', '" + ddlMeetingDate.SelectedValue.ToString() + "', '" + Column0 + "','" + Column1 + "','" + Column2 + "','" + Column3 + "')", TableNamenew));
    //                                                    string Deletestatement = string.Format("delete from {0} where Fileid={1}", dss.Tables[0].Rows[0]["TableName"].ToString(), Convert.ToInt32(FileId.Text.ToString()));
    //                                                    operation.Insert4Command(Deletestatement);

    //                                                }
    //                                            }
    //                                            value2 = Convert.ToInt16(operation.Insert4Command(string.Format(@"update tblfile set folderid={0} where fileid={1}", Convert.ToInt32(ddlCommitteenew.SelectedValue.ToString()), Convert.ToInt32(FileId.Text.ToString()))));
    //                                        }

    //                                        else
    //                                        {

    //                                            dss = operation.GetDataSet4Command(string.Format(@"select * from tblFolderIndexMaster where folder_id={0}", Session["FolderID"].ToString()));
    //                                            string Deletestatement = string.Format("delete from {0} where Fileid={1}", dss.Tables[0].Rows[0]["TableName"].ToString(), Convert.ToInt32(FileId.Text.ToString()));
    //                                            operation.Insert4Command(Deletestatement);
    //                                            value2 = Convert.ToInt16(operation.Insert4Command(string.Format(@"delete from tblfile where fileid={0}", Convert.ToInt32(FileId.Text.ToString()))));
    //                                        }
    //                                        DirectorDataBinding();
    //                                        Panel4.Visible = false;
    //                                    }
    //                                    else
    //                                    {
    //                                        if (lblApprovalStatus.Text != "1")
    //                                        {
    //                                            string ItemNo;
    //                                            string strrr = FileNameID[2].ToString();
    //                                            string TableName = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Convert.ToInt32(Session["FolderID"])));
    //                                            DataTable dtfiledetailsnew = operation.GetTable4Command(string.Format(@"select  FileID,substring(Column0,0,3) as Colun,Column0 from {0} where Column0>'{1}' and Column0 not like '{2}%' and folderid={3} order by Column0", TableName, strrr, strrr, Convert.ToInt32(Session["FolderID"])));
    //                                            DataTable dtfiledetails = operation.GetTable4Command(string.Format(@"select  FileID,substring(Column0,0,3) as Colun,Column0 from {0} where Column0='{1}' and Column0  like '{2}%' and folderid={3} order by Column0", TableName, strrr, strrr, Convert.ToInt32(Session["FolderID"])));
    //                                            for (int j = 0; j < dtfiledetails.Rows.Count; j++)
    //                                            {

    //                                                int Value = operation.Insert4Command(string.Format(@"Delete From {0}  where Column0  like '{1}%'", TableName, dtfiledetails.Rows[j]["Colun"].ToString()));
    //                                                int Valueww = operation.Insert4Command(string.Format(@"update tblfileactions set ActionName='Deleted' where FileID='{0}'", dtfiledetails.Rows[j]["FileID"].ToString()));

    //                                            }
    //                                            //if (Convert.ToInt32(strrr) < 9)
    //                                            //{
    //                                            //    ItemNo = ("0" + (Convert.ToInt32(strrr) + 1));
    //                                            //}
    //                                            //else
    //                                            //{
    //                                            //    ItemNo = strrr;
    //                                            //}

    //                                            for (int j = 0; j < dtfiledetailsnew.Rows.Count; j++)
    //                                            {
    //                                                string stttttttuuut = Convert.ToString(Convert.ToInt32(dtfiledetailsnew.Rows[j]["Colun"].ToString()) - 1);
    //                                                string sttttttttw = dtfiledetailsnew.Rows[j]["Column0"].ToString();
    //                                                if (Convert.ToInt32(dtfiledetailsnew.Rows[j]["Colun"].ToString()) < 11)
    //                                                {
    //                                                    int Value = operation.Insert4Command(string.Format(@"update {0} set column0='{1}' where FileID='{2}'", TableName, ("0" + Convert.ToInt32(stttttttuuut) + sttttttttw.Substring(2, sttttttttw.Length - 2)), dtfiledetailsnew.Rows[j]["FileID"].ToString()));
    //                                                }
    //                                                else
    //                                                {
    //                                                    int Value = operation.Insert4Command(string.Format(@"update {0} set column0='{1}' where FileID='{2}'", TableName, (stttttttuuut + sttttttttw.Substring(2, sttttttttw.Length - 2)), dtfiledetailsnew.Rows[j]["FileID"].ToString()));
    //                                                }

    //                                            }

    //                                            //dss = operation.GetDataSet4Command(string.Format(@"select TableName from tblFolderIndexMaster where folder_id={0}", Session["FolderID"].ToString()));
    //                                            //string Deletestatement = string.Format("delete from {0} where Fileid={1}", dss.Tables[0].Rows[0]["TableName"].ToString(), Convert.ToInt32(FileId.Text.ToString()));
    //                                            //operation.Insert4Command(Deletestatement);

    //                                            //DataTable dtattachmentnew = operation.GetTable4Command(string.Format(@"select AttachmentID from tblattachment  where Fileid='{0}'", Convert.ToInt32(FileId.Text.ToString())));
    //                                            //for (int i = 0; i < dtattachmentnew.Rows.Count; i++)
    //                                            //{
    //                                            //    string str = string.Format("delete from {0} where Fileid={1}", dss.Tables[0].Rows[0]["TableName"].ToString(), Convert.ToInt32(dtattachmentnew.Rows[i]["AttachmentID"].ToString()));
    //                                            //    operation.Insert4Command(str);

    //                                            //}
    //                                        }
    //                                        else
    //                                        {
    //                                            ScriptManager.RegisterStartupScript(Page, this.GetType(), "msg", "alert('File is already authorized.')", true);
    //                                        }

    //                                    }

    //                                }
    //                            }
    //                            DirectorDataBinding();
    //                            Panel4.Visible = false;
    //                        }
    //                        else
    //                        {
    //                            string TableNamenew = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", ddlCommitteenew.SelectedValue));
    //                            foreach (GridViewRow row in gvParent.Rows)
    //                            {
    //                                CheckBox chkSelect = (CheckBox)row.FindControl("chkSelect");
    //                                if (chkSelect.Checked)
    //                                {
    //                                    Label Id = (Label)row.FindControl("lblId");
    //                                    Label FileId = (Label)row.FindControl("lblFileId");
    //                                    Label lblApprovalStatus = (Label)row.FindControl("lblApprovalStatus");
    //                                    LinkButton FileName = (LinkButton)row.FindControl("lnkView");
    //                                    string[] FileNameID = FileName.CommandArgument.ToString().Split(new Char[] { ',' });
    //                                    string[] filename = FileNameID[1].Split(new Char[] { '.' });
    //                                    string name = filename[0].ToString();
    //                                    DataSet dss = new DataSet();
    //                                    if (System.IO.File.Exists(Server.MapPath("~\\Repository") + "\\" + name + "_" + Session["FolderID"].ToString() + ".enc"))
    //                                    {
    //                                        if (!System.IO.File.Exists(Server.MapPath("~\\Repository") + "\\" + name + "_" + ddlCommitteenew.SelectedValue.ToString() + ".enc"))
    //                                        {
    //                                            System.IO.File.Move(Server.MapPath("~\\Repository") + "\\" + name + "_" + Session["FolderID"].ToString() + ".enc", Server.MapPath("~\\Repository") + "\\" + name + "_" + ddlCommitteenew.SelectedValue.ToString() + ".enc");
    //                                            dss = operation.GetDataSet4Command(string.Format(@"select * from tblFolderIndexMaster where folder_id={0}", Session["FolderID"].ToString()));
    //                                            if (dss.Tables[0].Rows.Count > 0)
    //                                            {
    //                                                DataSet dsMeet = operation.GetDataSet4Command(string.Format(@"select * from {0} where FileId={1}", dss.Tables[0].Rows[0]["TableName"].ToString(), Convert.ToInt32(FileId.Text.ToString())));

    //                                                if (dsMeet.Tables[0].Rows.Count > 0)
    //                                                {

    //                                                    string ImportedBy = dsMeet.Tables[0].Rows[0]["ImportedBy"].ToString();
    //                                                    string DocStatus = dsMeet.Tables[0].Rows[0]["DocStatus"].ToString();
    //                                                    string Column3 = dsMeet.Tables[0].Rows[0]["Column3"].ToString();
    //                                                    string Column0 = dsMeet.Tables[0].Rows[0]["Column0"].ToString();
    //                                                    string Column1 = dsMeet.Tables[0].Rows[0]["Column1"].ToString();
    //                                                    string Column2 = dsMeet.Tables[0].Rows[0]["Column2"].ToString();
    //                                                    string value1 = operation.ExecuteScalar4Command(string.Format(@"insert into {0} (FileId,ImportedBy,DocStatus,FolderId,Column0,Column1,Column2,Column3) values ('" + Convert.ToInt32(FileId.Text.ToString()) + "','" + ImportedBy + "','" + ddlMeetingDate.SelectedValue.ToString() + "', '" + DocStatus + "', '" + Column0 + "','" + Column1 + "','" + Column2 + "','" + Column3 + "')", TableNamenew));
    //                                                    string Deletestatement = string.Format("delete from {0} where Fileid={1}", dss.Tables[0].Rows[0]["TableName"].ToString(), Convert.ToInt32(FileId.Text.ToString()));
    //                                                    operation.Insert4Command(Deletestatement);

    //                                                }
    //                                            }
    //                                            value2 = Convert.ToInt16(operation.Insert4Command(string.Format(@"update tblfile set folderid={0} where fileid={1}", Convert.ToInt32(ddlCommitteenew.SelectedValue.ToString()), Convert.ToInt32(FileId.Text.ToString()))));
    //                                        }
    //                                        else
    //                                        {

    //                                            dss = operation.GetDataSet4Command(string.Format(@"select * from tblFolderIndexMaster where folder_id={0}", Session["FolderID"].ToString()));
    //                                            string Deletestatement = string.Format("delete from {0} where Fileid={1}", dss.Tables[0].Rows[0]["TableName"].ToString(), Convert.ToInt32(FileId.Text.ToString()));
    //                                            operation.Insert4Command(Deletestatement);
    //                                            value2 = Convert.ToInt16(operation.Insert4Command(string.Format(@"delete from tblfile where fileid={0}", Convert.ToInt32(FileId.Text.ToString()))));

    //                                        }
    //                                        bindLvFileView();
    //                                        Panel4.Visible = false;
    //                                    }
    //                                    else
    //                                    {
    //                                        if (lblApprovalStatus.Text != "1")
    //                                        {
    //                                            dss = operation.GetDataSet4Command(string.Format(@"select TableName from tblFolderIndexMaster where folder_id={0}", Session["FolderID"].ToString()));
    //                                            string Deletestatement = string.Format("delete from {0} where Fileid={1}", dss.Tables[0].Rows[0]["TableName"].ToString(), Convert.ToInt32(FileId.Text.ToString()));
    //                                            operation.Insert4Command(Deletestatement);
    //                                            // value2 = Convert.ToInt16(operation.Insert4Command(string.Format(@"delete from tblfile where fileid={0}", Convert.ToInt32(FileId.Text.ToString()))));
    //                                            DataTable dtattachmentnew = operation.GetTable4Command(string.Format(@"select AttachmentID from tblattachment  where Fileid='{0}'", Convert.ToInt32(FileId.Text.ToString())));
    //                                            for (int i = 0; i < dtattachmentnew.Rows.Count; i++)
    //                                            {
    //                                                string str = string.Format("delete from {0} where Fileid={1}", dss.Tables[0].Rows[0]["TableName"].ToString(), Convert.ToInt32(dtattachmentnew.Rows[i]["AttachmentID"].ToString()));
    //                                                operation.Insert4Command(str);
    //                                            }
    //                                        }
    //                                        else
    //                                        {
    //                                            ScriptManager.RegisterStartupScript(Page, this.GetType(), "msg", "alert('File is already authorized.')", true);
    //                                        }


    //                                    }

    //                                }
    //                            }
    //                            bindLvFileView();
    //                            Panel4.Visible = false;


    //                        }
    //                    }
    //                    catch (Exception ex)
    //                    { }

    //                }
    //                else
    //                {
    //                    lblMessage.Text = "Please select atleast one file.";
    //                }
    //            }
    //            else
    //            {
    //                lblMessage.Text = "Please select atleast one file.";
    //            }
    //        }
    //        else
    //        {
    //            ScriptManager.RegisterStartupScript(Page, this.GetType(), "msg", "alert('Please select atleast one Agenda to Delete.')", true);
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        Session["Error"] = ex.StackTrace;
    //        Response.Redirect("../ErrorMessage.aspx", false);
    //    }
    //}
    //#endregion

    //protected void Imgatach_Click(object sender, ImageClickEventArgs e)
    //{
    //    Response.Redirect("../UploadFile/FileUpload_Multiple.aspx?ATR=Attachment" + FileNameID[0].ToString(), false);
    //}




 
    #region [btnDelete_Click]
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        try
        {
            objCommonBAL = new CommonBAL();

            ViewState["Delete"] = "True";

            lblMessage.Text = "";
            string strFileId = "";
            string strFileIddl = "";
            string FolderIDs = "";


            //get chechbox value
            if (Convert.ToInt16(Session["ParentFolderId"]) == 1)
            {
                if (Session["FolderName"].ToString().ToLower() == "meetings\\my briefcase" || Session["FolderName"].ToString().ToLower() == "meetings\\company info" || Session["FolderName"].ToString().ToLower() == "meetings\\shared documents")
                {
                    strFileId = objCommonBAL.GetCheckBoxCheckValue(gvData);

                }
                else
                {
                    strFileId = objCommonBAL.GetCheckBoxCheckValue(gvParent);
                }
            }
            else
                strFileId = objCommonBAL.GetCheckBoxCheckValue(gvData);
            if (strFileId != "")
            {
                //store value for get on get btnyes click.
                ViewState["strFileId"] = strFileId;

                lblRplMessage0.Text = "Are you sure you want to delete selected files ?";
                Panel4.Visible = true;
                ddlCommitteenew.Visible = true;
                OperationClass objOperationClass = new OperationClass();
                string SqlQuery;
                SqlQuery = "SELECT [FolderName], [FolderId] FROM [tblFolder] WHERE ([ParentFolderId] = 1) AND [MEETINGSTATUS]!=1 AND [MEETINGCANCELLED]!=1 and FolderName='Recycle Bin'";
                DataTable dt = objOperationClass.GetTable4Command(SqlQuery);
                if (dt.Rows.Count != 0)
                {
                    ddlCommitteenew.DataSource = dt;
                    ddlCommitteenew.DataTextField = "FolderName";
                    ddlCommitteenew.DataValueField = "FolderId";
                    ddlCommitteenew.DataBind();
                }

                btnNO.Focus();

                objFileUploadBAL = new FileUploadBAL();
                objFileUploadController = new FileUploadController();
                int value2;
                if (ViewState["strFileId"] != null)
                {
                    strFileIddl = (string)ViewState["strFileId"];

                    if (strFileIddl != "")
                    {
                        try
                        {
                            string strrrr = operation.ExecuteScalar4Command(string.Format(@"select ParentFolderId from tblfolder where folderid={0}", Session["FolderID"].ToString()));
                            if (strrrr != "1")
                            {
                                if (chkSelectalll.Checked == true)
                                {
                                    
                                    //string sub = FileNameID[2].ToString();
                                    //string FolderIDs = Convert.ToString(Session["FolderID"]);
                                    string TableName = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Convert.ToInt32(Session["FolderID"])));
                                    //string strItemNo = operation.ExecuteScalar4Command(string.Format(@"select substring(Column0,0,3) as Colun from {0} where  column0='{1}' and folderid={2} order by Column0", TableName, sub, Convert.ToInt32(Session["FolderID"])));
                                    //string strFileID = operation.ExecuteScalar4Command(string.Format(@"select FileID from {0} where  column0='{1}' and folderid={2} order by Column0", TableName, sub, Convert.ToInt32(Session["FolderID"])));
                                    //string stragendades = operation.ExecuteScalar4Command(string.Format(@"select column1 from {0} where fileid='{1}' and column0='{2}'and folderid={3} order by Column0", TableName, strFileID, strItemNo, Convert.ToInt32(Session["FolderID"])));
                                    //DataTable dtfiledetailss = operation.GetTable4Command(string.Format(@"select column0,substring(Column0,0,3) as Colun,FileID from {0} where  column0>'{1}' and column0 like '{2}%' and folderid={3} order by Column0", TableName, sub, sub.Substring(0, 2), Convert.ToInt32(Session["FolderID"])));
                                    //DataTable dtfiledetailssss = operation.GetTable4Command(string.Format(@"select column0,substring(Column0,0,3) as Colun,a.FileID from {0} a inner join tblfile b on a.fileid=b.fileid where  column0 like '{1}%' and b.filename!='' and a.folderid={2} order by Column0", TableName, sub.Substring(0, 2), Convert.ToInt32(Session["FolderID"])));
                                    DataTable dtfiledetailss = operation.GetTable4Command(string.Format(@"select column0,folderid,column1,substring(Column0,0,3) as Colun,FileID from {0} where  folderid={1} order by Column0", TableName, Convert.ToInt32(Session["FolderID"])));
                                    
                                    
                                    if (dtfiledetailss != null & dtfiledetailss.Rows.Count > 0)
                                    {
                                        for (int i = 0; i < dtfiledetailss.Rows.Count; i++)
                                        {

                                            
                                            
                                           
                                            
                                            
                                            
                                            
                                            
                                            
                                            DateTime sfds = DateTime.Now;
                                            string sdasd = sfds.ToString("yyyy-MM-dd HH:mm:ss");
                                            int Valueww = operation.Insert4Command(string.Format(@"insert into tblfileactions (FileId, FileName, ActionName, UserID, ActionDate, FolderId) values ({0},'{1}','{2}',{3},'{4}',{5}) ", Convert.ToInt32(dtfiledetailss.Rows[i]["FileID"].ToString()), Convert.ToString(dtfiledetailss.Rows[i]["column1"].ToString()), "Deleted", Convert.ToInt32(Session["UserID"]), sdasd, Convert.ToInt32(dtfiledetailss.Rows[i]["folderid"].ToString())));
                                            
                                        }
                                    }
                                    string TableNamenew = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Session["FolderID"].ToString()));
                                    string Deletestatement = operation.ExecuteScalar4Command(string.Format("delete from {0} where FolderID='{1}'", TableNamenew, Session["FolderID"].ToString()));
                                    ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('Agenda Deleted Successfully.')", true);
                                    chkSelectalll.Checked = false;
                                }
                                else
                                {
                                    string TableNamenew = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", ddlCommitteenew.SelectedValue));
                                    foreach (GridViewRow row in gvData.Rows)
                                    {
                                        CheckBox chkSelect = (CheckBox)row.FindControl("chkSelect");
                                        if (chkSelect.Checked)
                                        {
                                            Label Id = (Label)row.FindControl("lblId");
                                            Label FileId = (Label)row.FindControl("lblFileId");
                                            Label lblApprovalStatus = (Label)row.FindControl("lblApprovalStatus");
                                            LinkButton FileName = (LinkButton)row.FindControl("lnkOpen");
                                            string[] FileNameID = FileName.CommandArgument.ToString().Split(new Char[] { ',' });
                                            string[] filename = FileNameID[1].Split(new Char[] { '.' });
                                            string name = filename[0].ToString();
                                            DataSet dss = new DataSet();


                                            if (System.IO.File.Exists(Server.MapPath("~\\Repository") + "\\" + name + "_" + Session["FolderID"].ToString() + ".enc"))
                                            {
                                                if (!System.IO.File.Exists(Server.MapPath("~\\Repository") + "\\" + name + "_" + ddlCommitteenew.SelectedValue.ToString() + ".enc"))
                                                {
                                                    System.IO.File.Move(Server.MapPath("~\\Repository") + "\\" + name + "_" + Session["FolderID"].ToString() + ".enc", Server.MapPath("~\\Repository") + "\\" + name + "_" + ddlCommitteenew.SelectedValue.ToString() + ".enc");

                                                    dss = operation.GetDataSet4Command(string.Format(@"select * from tblFolderIndexMaster where folder_id={0}", Session["FolderID"].ToString()));
                                                    if (dss.Tables[0].Rows.Count > 0)
                                                    {
                                                        DataSet dsMeet = operation.GetDataSet4Command(string.Format(@"select * from {0} where FileId={1}", dss.Tables[0].Rows[0]["TableName"].ToString(), Convert.ToInt32(FileId.Text.ToString())));

                                                        if (dsMeet.Tables[0].Rows.Count > 0)
                                                        {

                                                            string ImportedBy = dsMeet.Tables[0].Rows[0]["ImportedBy"].ToString();
                                                            string DocStatus = dsMeet.Tables[0].Rows[0]["DocStatus"].ToString();
                                                            string Column3 = dsMeet.Tables[0].Rows[0]["Column3"].ToString();
                                                            string Column0 = dsMeet.Tables[0].Rows[0]["Column0"].ToString();
                                                            string Column1 = dsMeet.Tables[0].Rows[0]["Column1"].ToString();
                                                            string Column2 = dsMeet.Tables[0].Rows[0]["Column2"].ToString();
                                                            string value1 = operation.ExecuteScalar4Command(string.Format(@"insert into {0} (FileId,ImportedBy,DocStatus,FolderId,Column0,Column1,Column2,Column3) values ('" + Convert.ToInt32(FileId.Text.ToString()) + "','" + ImportedBy + "','" + DocStatus + "', '" + ddlMeetingDate.SelectedValue.ToString() + "', '" + Column0 + "','" + Column1 + "','" + Column2 + "','" + Column3 + "')", TableNamenew));
                                                            string Deletestatement = string.Format("delete from {0} where Fileid={1}", dss.Tables[0].Rows[0]["TableName"].ToString(), Convert.ToInt32(FileId.Text.ToString()));
                                                            operation.Insert4Command(Deletestatement);

                                                        }
                                                    }
                                                    value2 = Convert.ToInt16(operation.Insert4Command(string.Format(@"update tblfile set folderid={0} where fileid={1}", Convert.ToInt32(ddlCommitteenew.SelectedValue.ToString()), Convert.ToInt32(FileId.Text.ToString()))));
                                                }

                                                else
                                                {

                                                    dss = operation.GetDataSet4Command(string.Format(@"select * from tblFolderIndexMaster where folder_id={0}", Session["FolderID"].ToString()));
                                                    string Deletestatement = string.Format("delete from {0} where Fileid={1}", dss.Tables[0].Rows[0]["TableName"].ToString(), Convert.ToInt32(FileId.Text.ToString()));
                                                    operation.Insert4Command(Deletestatement);
                                                    value2 = Convert.ToInt16(operation.Insert4Command(string.Format(@"delete from tblfile where fileid={0}", Convert.ToInt32(FileId.Text.ToString()))));
                                                }
                                                DirectorDataBinding();
                                                Panel4.Visible = false;
                                            }
                                            else
                                            {


                                                R = 0;
                                                string ItemNo;
                                                string strrr = FileNameID[2].ToString();
                                                string TableName = "";
                                                DataTable dtfiledetailsnew = null;
                                                string sub = "";
                                                DataTable dtfiledetailss = null;
                                                DataTable dtfiledetailssss = null;
                                                string strItemNo = "";
                                                string strFileID = "";
                                             
                                                string stragendades = "";
                                                DateTime sfds = new DateTime();
                                                string sdasd = "";
                                                if (Request.QueryString["Value"] != null)
                                                {
                                                    if (Request.QueryString["Value"].ToString().ToLower() == "my briefcase")
                                                    {
                                                        FolderIDs = operation.ExecuteScalar4Command(string.Format(@"select Folderid from tblfolder where foldername='my briefcase' and deletestatus!=1"));
                                                        TableName = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Convert.ToInt32(FolderIDs)));
                                                        dtfiledetailsnew = operation.GetTable4Command(string.Format(@"select  FileID,substring(Column0,0,3) as Colun,Column0 from {0} where Column0>'{1}' and Column0 not like '{2}%' and folderid={3} order by Column0", TableName, strrr, strrr.Substring(0, 2), Convert.ToInt32(FolderIDs)));
                                                        sub = FileNameID[2].ToString();
                                                        dtfiledetailss = operation.GetTable4Command(string.Format(@"select column0,substring(Column0,0,3) as Colun,FileID from {0} where  column0>'{1}' and column0 like '{2}%' and folderid={3} order by Column0", TableName, sub, sub.Substring(0, 2), Convert.ToInt32(FolderIDs)));
                                                        dtfiledetailssss = operation.GetTable4Command(string.Format(@"select column0,substring(Column0,0,3) as Colun,a.FileID from {0} a inner join tblfile b on a.fileid=b.fileid where  column0 like '{1}%' and b.filename!='' and a.folderid={2} order by Column0", TableName, sub.Substring(0, 2), Convert.ToInt32(FolderIDs)));
                                                        strItemNo = operation.ExecuteScalar4Command(string.Format(@"select substring(Column0,0,3) as Colun from {0} where  column0='{1}' and folderid={2} order by Column0", TableName, sub, Convert.ToInt32(FolderIDs)));
                                                        strFileID = operation.ExecuteScalar4Command(string.Format(@"select FileID from {0} where  column0='{1}' and folderid={2} order by Column0", TableName, sub, Convert.ToInt32(FolderIDs)));


                                                    }
                                                    else
                                                        if (Request.QueryString["Value"].ToString().ToLower() == "company info")
                                                        {
                                                            FolderIDs = operation.ExecuteScalar4Command(string.Format(@"select Folderid from tblfolder where foldername='company info' and deletestatus!=1"));
                                                            TableName = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Convert.ToInt32(FolderIDs)));
                                                            dtfiledetailsnew = operation.GetTable4Command(string.Format(@"select  FileID,substring(Column0,0,3) as Colun,Column0 from {0} where Column0>'{1}' and Column0 not like '{2}%' and folderid={3} order by Column0", TableName, strrr, strrr.Substring(0, 2), Convert.ToInt32(FolderIDs)));
                                                            sub = FileNameID[2].ToString();
                                                            dtfiledetailss = operation.GetTable4Command(string.Format(@"select column0,substring(Column0,0,3) as Colun,FileID from {0} where  column0>'{1}' and column0 like '{2}%' and folderid={3} order by Column0", TableName, sub, sub.Substring(0, 2), Convert.ToInt32(FolderIDs)));
                                                            dtfiledetailssss = operation.GetTable4Command(string.Format(@"select column0,substring(Column0,0,3) as Colun,a.FileID from {0} a inner join tblfile b on a.fileid=b.fileid where  column0 like '{1}%' and b.filename!='' and a.folderid={2} order by Column0", TableName, sub.Substring(0, 2), Convert.ToInt32(FolderIDs)));
                                                            strItemNo = operation.ExecuteScalar4Command(string.Format(@"select substring(Column0,0,3) as Colun from {0} where  column0='{1}' and folderid={2} order by Column0", TableName, sub, Convert.ToInt32(FolderIDs)));
                                                            strFileID = operation.ExecuteScalar4Command(string.Format(@"select FileID from {0} where  column0='{1}' and folderid={2} order by Column0", TableName, sub, Convert.ToInt32(FolderIDs)));


                                                        }
                                                        else
                                                        {
                                                            FolderIDs = Convert.ToString(Session["FolderID"]);
                                                            TableName = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Convert.ToInt32(Session["FolderID"])));
                                                            dtfiledetailsnew = operation.GetTable4Command(string.Format(@"select  FileID,substring(Column0,0,3) as Colun,Column0 from {0} where Column0>'{1}' and Column0 not like '{2}%' and folderid={3} order by Column0", TableName, strrr, strrr.Substring(0, 2), Convert.ToInt32(Session["FolderID"])));
                                                            sub = FileNameID[2].ToString();
                                                            dtfiledetailss = operation.GetTable4Command(string.Format(@"select column0,substring(Column0,0,3) as Colun,FileID from {0} where  column0>'{1}' and column0 like '{2}%' and folderid={3} order by Column0", TableName, sub, sub.Substring(0, 2), Convert.ToInt32(Session["FolderID"])));
                                                            dtfiledetailssss = operation.GetTable4Command(string.Format(@"select column0,substring(Column0,0,3) as Colun,a.FileID from {0} a inner join tblfile b on a.fileid=b.fileid where  column0 like '{1}%' and b.filename!='' and a.folderid={2} order by Column0", TableName, sub.Substring(0, 2), Convert.ToInt32(Session["FolderID"])));
                                                            strItemNo = operation.ExecuteScalar4Command(string.Format(@"select substring(Column0,0,3) as Colun from {0} where  column0='{1}' and folderid={2} order by Column0", TableName, sub, Convert.ToInt32(Session["FolderID"])));
                                                            strFileID = operation.ExecuteScalar4Command(string.Format(@"select FileID from {0} where  column0='{1}' and folderid={2} order by Column0", TableName, sub, Convert.ToInt32(Session["FolderID"])));

                                                        }
                                                }
                                                else
                                                {
                                                    FolderIDs = Convert.ToString(Session["FolderID"]);
                                                    TableName = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Convert.ToInt32(Session["FolderID"])));
                                                    dtfiledetailsnew = operation.GetTable4Command(string.Format(@"select  FileID,substring(Column0,0,3) as Colun,Column0 from {0} where Column0>'{1}' and Column0 not like '{2}%' and folderid={3} order by Column0", TableName, strrr, strrr.Substring(0, 2), Convert.ToInt32(Session["FolderID"])));
                                                    sub = FileNameID[2].ToString();
                                                    dtfiledetailss = operation.GetTable4Command(string.Format(@"select column0,substring(Column0,0,3) as Colun,FileID from {0} where  column0>'{1}' and column0 like '{2}%' and folderid={3} order by Column0", TableName, sub, sub.Substring(0, 2), Convert.ToInt32(Session["FolderID"])));
                                                    dtfiledetailssss = operation.GetTable4Command(string.Format(@"select column0,substring(Column0,0,3) as Colun,a.FileID from {0} a inner join tblfile b on a.fileid=b.fileid where  column0 like '{1}%' and b.filename!='' and a.folderid={2} order by Column0", TableName, sub.Substring(0, 2), Convert.ToInt32(Session["FolderID"])));
                                                    strItemNo = operation.ExecuteScalar4Command(string.Format(@"select substring(Column0,0,3) as Colun from {0} where  column0='{1}' and folderid={2} order by Column0", TableName, sub, Convert.ToInt32(Session["FolderID"])));
                                                    strFileID = operation.ExecuteScalar4Command(string.Format(@"select FileID from {0} where  column0='{1}' and folderid={2} order by Column0", TableName, sub, Convert.ToInt32(Session["FolderID"])));
                                                    stragendades = operation.ExecuteScalar4Command(string.Format(@"select column1 from {0} where fileid='{1}' and column0='{2}'and folderid={3} order by Column0", TableName, strFileID, strItemNo, Convert.ToInt32(Session["FolderID"])));
                                                    sfds = DateTime.Now;
                                                    sdasd = sfds.ToString("yyyy-MM-dd HH:mm:ss");
                                                }
                                                //R = 0;
                                                //string ItemNo;
                                                //string strrr = FileNameID[2].ToString();
                                                //string TableName = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Convert.ToInt32(Session["FolderID"])));
                                                //DataTable dtfiledetailsnew = operation.GetTable4Command(string.Format(@"select  FileID,substring(Column0,0,3) as Colun,Column0 from {0} where Column0>'{1}' and Column0 not like '{2}%' and folderid={3} order by Column0", TableName, strrr, strrr.Substring(0, 2), Convert.ToInt32(Session["FolderID"])));

                                                //string sub = FileNameID[2].ToString();
                                                //DataTable dtfiledetailss = operation.GetTable4Command(string.Format(@"select column0,substring(Column0,0,3) as Colun,FileID from {0} where  column0>'{1}' and column0 like '{2}%' and folderid={3} order by Column0", TableName, sub, sub.Substring(0, 2), Convert.ToInt32(Session["FolderID"])));
                                                //DataTable dtfiledetailssss = operation.GetTable4Command(string.Format(@"select column0,substring(Column0,0,3) as Colun,a.FileID from {0} a inner join tblfile b on a.fileid=b.fileid where  column0 like '{1}%' and b.filename!='' and a.folderid={2} order by Column0", TableName, sub.Substring(0, 2), Convert.ToInt32(Session["FolderID"])));
                                                //string strItemNo = operation.ExecuteScalar4Command(string.Format(@"select substring(Column0,0,3) as Colun from {0} where  column0='{1}' and folderid={2} order by Column0", TableName, sub, Convert.ToInt32(Session["FolderID"])));
                                                //string strFileID = operation.ExecuteScalar4Command(string.Format(@"select FileID from {0} where  column0='{1}' and folderid={2} order by Column0", TableName, sub, Convert.ToInt32(Session["FolderID"])));

                                                if (FileNameID[1].ToString() == "")
                                                {

                                                    int Value = operation.Insert4Command(string.Format(@"Delete From {0}  where fileid={1}", TableName, Convert.ToInt32(FileNameID[0].ToString())));
                                                    int Valueww = operation.Insert4Command(string.Format(@"insert into tblfileactions (FileId, FileName, ActionName, UserID, ActionDate, FolderId) values ({0},'{1}','{2}',{3},'{4}',{5}) ", Convert.ToInt32(strFileID), stragendades, "Deleted", Convert.ToInt32(Session["UserID"]), sdasd, Convert.ToInt32(FolderIDs)));
                                                    ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('Agenda Deleted Successfully.')", true);
                                                }
                                                else
                                                {


                                                    if (sub == strItemNo)
                                                    {
                                                        for (int s = 0; s < dtfiledetailssss.Rows.Count; s++)
                                                        {

                                                            // int Value = operation.Insert4Command(string.Format(@"Delete From {0}  where Column0  like '{1}%' and folderid={2}", TableName, dtfiledetailssss.Rows[s]["Colun"].ToString(), Convert.ToInt32(Session["FolderID"])));
                                                            int Value = operation.Insert4Command(string.Format(@"Delete From {0}  where Column0  like '{1}%' and folderid='{2}' and fileid !=ISNULL((select a.fileid from {0} a inner join tblfile b on a.fileid=b.fileid  where a.Column0 like '{1}%' and filename='' and a.folderid='{2}'),'')", TableName, dtfiledetailssss.Rows[s]["Colun"].ToString(), Convert.ToInt32(FolderIDs)));
                                                            int Valueww = operation.Insert4Command(string.Format(@"insert into tblfileactions (FileId, FileName, ActionName, UserID, ActionDate, FolderId) values ({0},'{1}','{2}',{3},'{4}',{5}) ", Convert.ToInt32(strFileID), stragendades, "Deleted", Convert.ToInt32(Session["UserID"]), sdasd, Convert.ToInt32(FolderIDs)));

                                                        }
                                                    }
                                                    else
                                                    {

                                                        int Value = operation.Insert4Command(string.Format(@"Delete From {0}  where Column0  like '{1}%' and folderid={2}", TableName, sub, Convert.ToInt32(FolderIDs)));
                                                        int Valueww = operation.Insert4Command(string.Format(@"insert into tblfileactions (FileId, FileName, ActionName, UserID, ActionDate, FolderId) values ({0},'{1}','{2}',{3},'{4}',{5}) ", Convert.ToInt32(strFileID), stragendades, "Deleted", Convert.ToInt32(Session["UserID"]), sdasd, Convert.ToInt32(FolderIDs)));
                                                        if (dtfiledetailss != null & dtfiledetailss.Rows.Count > 0)
                                                        {
                                                            for (int i = 0; i < dtfiledetailss.Rows.Count; i++)
                                                            {
                                                                string strrrw = dtfiledetailss.Rows[i]["column0"].ToString();
                                                                string subw = strrrw.Substring(2, strrrw.Length - 2);
                                                                byte Letterw = (byte)Convert.ToChar(subw);
                                                                char Outputw = (char)(Letterw - 1);
                                                                subw = strrrw.Substring(0, 2) + "" + Outputw;
                                                                int Values = operation.Insert4Command(string.Format(@"update {0} set column0='{1}' where FileID='{2}'", TableName, subw, dtfiledetailss.Rows[i]["FileID"].ToString()));

                                                            }
                                                        }


                                                    }

                                                    if (sub == strItemNo)
                                                    {
                                                        for (int j = 0; j < dtfiledetailsnew.Rows.Count; j++)
                                                        {
                                                            string stttttttuuut = Convert.ToString(Convert.ToInt32(dtfiledetailsnew.Rows[j]["Colun"].ToString()) - 1);
                                                            string sttttttttw = dtfiledetailsnew.Rows[j]["Column0"].ToString();
                                                            if (Convert.ToInt32(dtfiledetailsnew.Rows[j]["Colun"].ToString()) < 11)
                                                            {
                                                                int Value = operation.Insert4Command(string.Format(@"update {0} set column0='{1}' where FileID='{2}'", TableName, ("0" + Convert.ToInt32(stttttttuuut) + sttttttttw.Substring(2, sttttttttw.Length - 2)), dtfiledetailsnew.Rows[j]["FileID"].ToString()));
                                                            }
                                                            else
                                                            {
                                                                int Value = operation.Insert4Command(string.Format(@"update {0} set column0='{1}' where FileID='{2}'", TableName, (stttttttuuut + sttttttttw.Substring(2, sttttttttw.Length - 2)), dtfiledetailsnew.Rows[j]["FileID"].ToString()));
                                                            }

                                                        }
                                                    }
                                                }


                                                //}
                                                //else
                                                //{
                                                //    ScriptManager.RegisterStartupScript(Page, this.GetType(), "msg", "alert('File is already authorized.')", true);
                                                //}

                                            }

                                        }
                                    }
                                }
                                DirectorDataBinding();
                                Panel4.Visible = false;
                            }
                            else
                            {
                                //string TableNamenew = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", ddlCommitteenew.SelectedValue));
                                //foreach (GridViewRow row in gvParent.Rows)
                                //{
                                //    CheckBox chkSelect = (CheckBox)row.FindControl("chkSelect");
                                //    if (chkSelect.Checked)
                                //    {
                                //        Label Id = (Label)row.FindControl("lblId");
                                //        Label FileId = (Label)row.FindControl("lblFileId");
                                //        Label lblApprovalStatus = (Label)row.FindControl("lblApprovalStatus");
                                //        LinkButton FileName = (LinkButton)row.FindControl("lnkView");
                                //        string[] FileNameID = FileName.CommandArgument.ToString().Split(new Char[] { ',' });
                                //        string[] filename = FileNameID[1].Split(new Char[] { '.' });
                                //        string name = filename[0].ToString();
                                //        DataSet dss = new DataSet();
                                //        if (System.IO.File.Exists(Server.MapPath("~\\Repository") + "\\" + name + "_" + Session["FolderID"].ToString() + ".enc"))
                                //        {
                                //            if (!System.IO.File.Exists(Server.MapPath("~\\Repository") + "\\" + name + "_" + ddlCommitteenew.SelectedValue.ToString() + ".enc"))
                                //            {
                                //                System.IO.File.Move(Server.MapPath("~\\Repository") + "\\" + name + "_" + Session["FolderID"].ToString() + ".enc", Server.MapPath("~\\Repository") + "\\" + name + "_" + ddlCommitteenew.SelectedValue.ToString() + ".enc");
                                //                dss = operation.GetDataSet4Command(string.Format(@"select * from tblFolderIndexMaster where folder_id={0}", Session["FolderID"].ToString()));
                                //                if (dss.Tables[0].Rows.Count > 0)
                                //                {
                                //                    DataSet dsMeet = operation.GetDataSet4Command(string.Format(@"select * from {0} where FileId={1}", dss.Tables[0].Rows[0]["TableName"].ToString(), Convert.ToInt32(FileId.Text.ToString())));

                                //                    if (dsMeet.Tables[0].Rows.Count > 0)
                                //                    {

                                //                        string ImportedBy = dsMeet.Tables[0].Rows[0]["ImportedBy"].ToString();
                                //                        string DocStatus = dsMeet.Tables[0].Rows[0]["DocStatus"].ToString();
                                //                        string Column3 = dsMeet.Tables[0].Rows[0]["Column3"].ToString();
                                //                        string Column0 = dsMeet.Tables[0].Rows[0]["Column0"].ToString();
                                //                        string Column1 = dsMeet.Tables[0].Rows[0]["Column1"].ToString();
                                //                        string Column2 = dsMeet.Tables[0].Rows[0]["Column2"].ToString();
                                //                        string value1 = operation.ExecuteScalar4Command(string.Format(@"insert into {0} (FileId,ImportedBy,DocStatus,FolderId,Column0,Column1,Column2,Column3) values ('" + Convert.ToInt32(FileId.Text.ToString()) + "','" + ImportedBy + "','" + ddlMeetingDate.SelectedValue.ToString() + "', '" + DocStatus + "', '" + Column0 + "','" + Column1 + "','" + Column2 + "','" + Column3 + "')", TableNamenew));
                                //                        string Deletestatement = string.Format("delete from {0} where Fileid={1}", dss.Tables[0].Rows[0]["TableName"].ToString(), Convert.ToInt32(FileId.Text.ToString()));
                                //                        operation.Insert4Command(Deletestatement);

                                //                    }
                                //                }
                                //                value2 = Convert.ToInt16(operation.Insert4Command(string.Format(@"update tblfile set folderid={0} where fileid={1}", Convert.ToInt32(ddlCommitteenew.SelectedValue.ToString()), Convert.ToInt32(FileId.Text.ToString()))));
                                //            }
                                //            else
                                //            {

                                //                dss = operation.GetDataSet4Command(string.Format(@"select * from tblFolderIndexMaster where folder_id={0}", Session["FolderID"].ToString()));
                                //                string Deletestatement = string.Format("delete from {0} where Fileid={1}", dss.Tables[0].Rows[0]["TableName"].ToString(), Convert.ToInt32(FileId.Text.ToString()));
                                //                operation.Insert4Command(Deletestatement);
                                //                value2 = Convert.ToInt16(operation.Insert4Command(string.Format(@"delete from tblfile where fileid={0}", Convert.ToInt32(FileId.Text.ToString()))));

                                //            }
                                //            bindLvFileView();
                                //            Panel4.Visible = false;
                                //        }
                                //        else
                                //        {
                                //            if (lblApprovalStatus.Text != "1")
                                //            {
                                //                dss = operation.GetDataSet4Command(string.Format(@"select TableName from tblFolderIndexMaster where folder_id={0}", Session["FolderID"].ToString()));
                                //                string Deletestatement = string.Format("delete from {0} where Fileid={1}", dss.Tables[0].Rows[0]["TableName"].ToString(), Convert.ToInt32(FileId.Text.ToString()));
                                //                operation.Insert4Command(Deletestatement);
                                //                // value2 = Convert.ToInt16(operation.Insert4Command(string.Format(@"delete from tblfile where fileid={0}", Convert.ToInt32(FileId.Text.ToString()))));
                                //                DataTable dtattachmentnew = operation.GetTable4Command(string.Format(@"select AttachmentID from tblattachment  where Fileid='{0}'", Convert.ToInt32(FileId.Text.ToString())));
                                //                for (int i = 0; i < dtattachmentnew.Rows.Count; i++)
                                //                {
                                //                    string str = string.Format("delete from {0} where Fileid={1}", dss.Tables[0].Rows[0]["TableName"].ToString(), Convert.ToInt32(dtattachmentnew.Rows[i]["AttachmentID"].ToString()));
                                //                    operation.Insert4Command(str);
                                //                }
                                //            }
                                //            else
                                //            {
                                //                ScriptManager.RegisterStartupScript(Page, this.GetType(), "msg", "alert('File is already authorized.')", true);
                                //            }


                                //        }

                                //    }
                                //}
                                //bindLvFileView();
                                //Panel4.Visible = false;
                                string TableNamenew = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", ddlCommitteenew.SelectedValue));
                                foreach (GridViewRow row in gvData.Rows)
                                {
                                    CheckBox chkSelect = (CheckBox)row.FindControl("chkSelect");
                                    if (chkSelect.Checked)
                                    {
                                        Label Id = (Label)row.FindControl("lblId");
                                        Label FileId = (Label)row.FindControl("lblFileId");
                                        Label lblApprovalStatus = (Label)row.FindControl("lblApprovalStatus");
                                        LinkButton FileName = (LinkButton)row.FindControl("lnkOpen");
                                        string[] FileNameID = FileName.CommandArgument.ToString().Split(new Char[] { ',' });
                                        string[] filename = FileNameID[1].Split(new Char[] { '.' });
                                        string name = filename[0].ToString();
                                        DataSet dss = new DataSet();


                                        if (System.IO.File.Exists(Server.MapPath("~\\Repository") + "\\" + name + "_" + Session["FolderID"].ToString() + ".enc"))
                                        {
                                            if (!System.IO.File.Exists(Server.MapPath("~\\Repository") + "\\" + name + "_" + ddlCommitteenew.SelectedValue.ToString() + ".enc"))
                                            {
                                                System.IO.File.Move(Server.MapPath("~\\Repository") + "\\" + name + "_" + Session["FolderID"].ToString() + ".enc", Server.MapPath("~\\Repository") + "\\" + name + "_" + ddlCommitteenew.SelectedValue.ToString() + ".enc");

                                                dss = operation.GetDataSet4Command(string.Format(@"select * from tblFolderIndexMaster where folder_id={0}", Session["FolderID"].ToString()));
                                                if (dss.Tables[0].Rows.Count > 0)
                                                {
                                                    DataSet dsMeet = operation.GetDataSet4Command(string.Format(@"select * from {0} where FileId={1}", dss.Tables[0].Rows[0]["TableName"].ToString(), Convert.ToInt32(FileId.Text.ToString())));

                                                    if (dsMeet.Tables[0].Rows.Count > 0)
                                                    {

                                                        string ImportedBy = dsMeet.Tables[0].Rows[0]["ImportedBy"].ToString();
                                                        string DocStatus = dsMeet.Tables[0].Rows[0]["DocStatus"].ToString();
                                                        string Column3 = dsMeet.Tables[0].Rows[0]["Column3"].ToString();
                                                        string Column0 = dsMeet.Tables[0].Rows[0]["Column0"].ToString();
                                                        string Column1 = dsMeet.Tables[0].Rows[0]["Column1"].ToString();
                                                        string Column2 = dsMeet.Tables[0].Rows[0]["Column2"].ToString();
                                                        string value1 = operation.ExecuteScalar4Command(string.Format(@"insert into {0} (FileId,ImportedBy,DocStatus,FolderId,Column0,Column1,Column2,Column3) values ('" + Convert.ToInt32(FileId.Text.ToString()) + "','" + ImportedBy + "','" + DocStatus + "', '" + ddlMeetingDate.SelectedValue.ToString() + "', '" + Column0 + "','" + Column1 + "','" + Column2 + "','" + Column3 + "')", TableNamenew));
                                                        string Deletestatement = string.Format("delete from {0} where Fileid={1}", dss.Tables[0].Rows[0]["TableName"].ToString(), Convert.ToInt32(FileId.Text.ToString()));
                                                        operation.Insert4Command(Deletestatement);

                                                    }
                                                }
                                                value2 = Convert.ToInt16(operation.Insert4Command(string.Format(@"update tblfile set folderid={0} where fileid={1}", Convert.ToInt32(ddlCommitteenew.SelectedValue.ToString()), Convert.ToInt32(FileId.Text.ToString()))));
                                            }

                                            else
                                            {

                                                dss = operation.GetDataSet4Command(string.Format(@"select * from tblFolderIndexMaster where folder_id={0}", Session["FolderID"].ToString()));
                                                string Deletestatement = string.Format("delete from {0} where Fileid={1}", dss.Tables[0].Rows[0]["TableName"].ToString(), Convert.ToInt32(FileId.Text.ToString()));
                                                operation.Insert4Command(Deletestatement);
                                                value2 = Convert.ToInt16(operation.Insert4Command(string.Format(@"delete from tblfile where fileid={0}", Convert.ToInt32(FileId.Text.ToString()))));
                                            }
                                            DirectorDataBinding();
                                            Panel4.Visible = false;
                                        }
                                        else
                                        {
                                            //if (lblApprovalStatus.Text != "1" && lblApprovalStatus.Text != "2")
                                            //{

                                            R = 0;
                                            string ItemNo;
                                            string strrr = FileNameID[2].ToString();
                                            string TableName = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Convert.ToInt32(Session["FolderID"])));
                                            DataTable dtfiledetailsnew = operation.GetTable4Command(string.Format(@"select  FileID,substring(Column0,0,3) as Colun,Column0 from {0} where Column0>'{1}' and Column0 not like '{2}%' and folderid={3} order by Column0", TableName, strrr, strrr.Substring(0, 2), Convert.ToInt32(Session["FolderID"])));

                                            string sub = FileNameID[2].ToString();
                                            DataTable dtfiledetailss = operation.GetTable4Command(string.Format(@"select column0,substring(Column0,0,3) as Colun,FileID from {0} where  column0>'{1}' and column0 like '{2}%' and folderid={3} order by Column0", TableName, sub, sub.Substring(0, 2), Convert.ToInt32(Session["FolderID"])));
                                            DataTable dtfiledetailssss = operation.GetTable4Command(string.Format(@"select column0,substring(Column0,0,3) as Colun,a.FileID from {0} a inner join tblfile b on a.fileid=b.fileid where  column0 like '{1}%' and b.filename!='' and a.folderid={2} order by Column0", TableName, sub.Substring(0, 2), Convert.ToInt32(Session["FolderID"])));
                                            string strItemNo = operation.ExecuteScalar4Command(string.Format(@"select substring(Column0,0,3) as Colun from {0} where  column0='{1}' and folderid={2} order by Column0", TableName, sub, Convert.ToInt32(Session["FolderID"])));
                                            string strFileID = operation.ExecuteScalar4Command(string.Format(@"select FileID from {0} where  column0='{1}' and folderid={2} order by Column0", TableName, sub, Convert.ToInt32(Session["FolderID"])));

                                            if (FileNameID[1].ToString() == "")
                                            {

                                                int Value = operation.Insert4Command(string.Format(@"Delete From {0}  where fileid={1}", TableName, Convert.ToInt32(FileNameID[0].ToString())));
                                                int Valueww = operation.Insert4Command(string.Format(@"update tblfileactions set ActionName='Deleted' where FileID='{0}'", Convert.ToInt32(FileNameID[0].ToString())));
                                            }
                                            else
                                            {


                                                if (sub == strItemNo)
                                                {
                                                    for (int s = 0; s < dtfiledetailssss.Rows.Count; s++)
                                                    {

                                                        // int Value = operation.Insert4Command(string.Format(@"Delete From {0}  where Column0  like '{1}%' and folderid={2}", TableName, dtfiledetailssss.Rows[s]["Colun"].ToString(), Convert.ToInt32(Session["FolderID"])));
                                                        int Value = operation.Insert4Command(string.Format(@"Delete From {0}  where Column0  like '{1}%' and folderid='{2}' and fileid !=ISNULL((select a.fileid from {0} a inner join tblfile b on a.fileid=b.fileid  where a.Column0 like '{1}%' and filename='' and a.folderid='{2}'),'')", TableName, dtfiledetailssss.Rows[s]["Colun"].ToString(), Convert.ToInt32(Session["FolderID"])));
                                                        int Valueww = operation.Insert4Command(string.Format(@"update tblfileactions set ActionName='Deleted' where FileID='{0}'", dtfiledetailssss.Rows[s]["FileID"].ToString()));

                                                    }
                                                }
                                                else
                                                {

                                                    int Value = operation.Insert4Command(string.Format(@"Delete From {0}  where Column0  like '{1}%' and folderid={2}", TableName, sub, Convert.ToInt32(Session["FolderID"])));
                                                    int Valueww = operation.Insert4Command(string.Format(@"update tblfileactions set ActionName='Deleted' where FileID='{0}'", strFileID));
                                                    if (dtfiledetailss != null & dtfiledetailss.Rows.Count > 0)
                                                    {
                                                        for (int i = 0; i < dtfiledetailss.Rows.Count; i++)
                                                        {
                                                            string strrrw = dtfiledetailss.Rows[i]["column0"].ToString();
                                                            string subw = strrrw.Substring(2, strrrw.Length - 2);
                                                            byte Letterw = (byte)Convert.ToChar(subw);
                                                            char Outputw = (char)(Letterw - 1);
                                                            subw = strrrw.Substring(0, 2) + "" + Outputw;
                                                            int Values = operation.Insert4Command(string.Format(@"update {0} set column0='{1}' where FileID='{2}'", TableName, subw, dtfiledetailss.Rows[i]["FileID"].ToString()));

                                                        }
                                                    }


                                                }

                                                if (sub == strItemNo)
                                                {
                                                    for (int j = 0; j < dtfiledetailsnew.Rows.Count; j++)
                                                    {
                                                        string stttttttuuut = Convert.ToString(Convert.ToInt32(dtfiledetailsnew.Rows[j]["Colun"].ToString()) - 1);
                                                        string sttttttttw = dtfiledetailsnew.Rows[j]["Column0"].ToString();
                                                        if (Convert.ToInt32(dtfiledetailsnew.Rows[j]["Colun"].ToString()) < 11)
                                                        {
                                                            int Value = operation.Insert4Command(string.Format(@"update {0} set column0='{1}' where FileID='{2}'", TableName, ("0" + Convert.ToInt32(stttttttuuut) + sttttttttw.Substring(2, sttttttttw.Length - 2)), dtfiledetailsnew.Rows[j]["FileID"].ToString()));
                                                        }
                                                        else
                                                        {
                                                            int Value = operation.Insert4Command(string.Format(@"update {0} set column0='{1}' where FileID='{2}'", TableName, (stttttttuuut + sttttttttw.Substring(2, sttttttttw.Length - 2)), dtfiledetailsnew.Rows[j]["FileID"].ToString()));
                                                        }

                                                    }
                                                }
                                            }


                                            //}
                                            //else
                                            //{
                                            //    ScriptManager.RegisterStartupScript(Page, this.GetType(), "msg", "alert('File is already authorized.')", true);
                                            //}

                                        }

                                    }
                                }
                                DirectorDataBinding();
                                Panel4.Visible = false;


                            }
                        }
                        catch (Exception ex)
                        { }

                    }
                    else
                    {
                        lblMessage.Text = "Please select atleast one file.";
                    }
                }
                else
                {
                    lblMessage.Text = "Please select atleast one file.";
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(Page, this.GetType(), "msg", "alert('Please select atleast one Agenda to Delete.')", true);
            }
        }
        catch (Exception ex)
        {
            Session["Error"] = ex.StackTrace;
            Response.Redirect("../ErrorMessage.aspx", false);
        }
    }
  
    #endregion

    #region [btnComplete_Click]
    protected void btnComplete_Click(object sender, EventArgs e)
    {
        //pnlComplete.Visible = true;
        //pnlComplete.Visible = true;
        try
        {
            objCommonBAL = new CommonBAL();
            int fId = Convert.ToInt32(Session["FolderID"]);
            OperationClass objOperationClass = new OperationClass();
            //int returnstatus = objOperationClass.ExecuteNonQuery(string.Format(@"update tblfolder set MeetingStatus = 1 where folderid = {0}", fId));
            //if (returnstatus > 0)
            //{
            string TableName = objOperationClass.ExecuteScalar4Command(string.Format(@"select TableName from tblFolderIndexMaster where Folder_Id ={0}", Convert.ToInt32(Session["FolderId"])));
            DataTable dtfoldername = objOperationClass.GetTable4Command(string.Format(@"select FolderName from tblFolder where  folderid='{0}'", Session["FolderID"].ToString()));

            string sdasd = dtfoldername.Rows[0]["FolderName"].ToString();
            DateTime dt2 = DateTime.ParseExact(sdasd, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            string CommeetteeName = objOperationClass.ExecuteScalar4Command(string.Format(@"select FolderName from tblFolder where FolderId in(select ParentFolderId from tblFolder where FolderId={0})", Convert.ToInt32(Session["FolderId"])));


            DataTable dt = objOperationClass.GetTable4Command(string.Format(@"select UserId,Firstname+' '+Lastname 'UserName',EmailID from tbluserdetail where userid in (select distinct userid from tbluseraccesscontrol where  folderid='{0}' and accesssymbol='F')", Session["FolderID"].ToString()));
            DataTable dtAttendance = objOperationClass.GetTable4Command(string.Format(@"select * from tblAttendance where  folderid='{0}'", Session["FolderID"].ToString()));
            if (dtAttendance != null && dtAttendance.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //int returnstatusss = objOperationClass.ExecuteNonQuery(string.Format(@"insert into tblAttendance(folderid,status,userid,UserName,EmailID) values('{0}','Present','{1}','{2}','{3)'", Session["FolderID"].ToString(), dt.Rows[i]["UserId"].ToString(), dt.Rows[i]["UserName"].ToString(), dt.Rows[i]["EmailID"].ToString()));
                    //int returnstatus3 = objOperationClass.ExecuteNonQuery(string.Format(@"insert into tblAttendeesStatus(folderid,status,userid,UserName,EmailID) values('{0}','Present','{1}','{2}','{3}')", Session["FolderID"].ToString(), dt.Rows[i]["UserId"].ToString(), dt.Rows[i]["UserName"].ToString(), dt.Rows[i]["EmailID"].ToString()));
                    //int returnstatusss = objOperationClass.ExecuteNonQuery(string.Format(@"insert into tblAttendance(folderid,status,userid,UserName,EmailID) values('{0}','Present','{1}','{2}','{3)'", Session["FolderID"].ToString(), dt.Rows[i]["UserId"].ToString(), dt.Rows[i]["UserName"].ToString(), dt.Rows[i]["EmailID"].ToString()));
                    //int returnstatus3 = objOperationClass.ExecuteNonQuery(string.Format(@"insert into tblAttendeesStatus(folderid,status,userid,UserName,EmailID) values('{0}','Present','{1}','{2}','{3}')", Session["FolderID"].ToString(), dt.Rows[i]["UserId"].ToString(), dt.Rows[i]["UserName"].ToString(), dt.Rows[i]["EmailID"].ToString()));
                    string UserId = objOperationClass.ExecuteScalar4Command(string.Format(@"select UserId from tblAttendance where folderid='{0}' and userid={1}", Session["FolderID"].ToString(), dt.Rows[i]["UserId"].ToString()));
                    if (UserId != "")
                    {
                        objOperationClass.Insert4Command(string.Format(@"update tblAttendance set folderid={0},status='Present',userid={1},UserName='{2}',EmailID='{3}'", Session["FolderID"].ToString(), dt.Rows[i]["UserId"].ToString(), dt.Rows[i]["UserName"].ToString(), dt.Rows[i]["EmailID"].ToString()));
                        objOperationClass.Insert4Command(string.Format(@"update tblAttendeesStatus set folderid={0},status='Present',userid={1},UserName='{2}',EmailID='{3}'", Session["FolderID"].ToString(), dt.Rows[i]["UserId"].ToString(), dt.Rows[i]["UserName"].ToString(), dt.Rows[i]["EmailID"].ToString()));
                        objOperationClass.Insert4Command(string.Format(@"update tblfileactions set UserID={0},FileName='Present',ActionName='{1}',ActionDate='{2}',Status='2',FolderId='{3}'", dt.Rows[i]["UserId"].ToString(), CommeetteeName, dt2.Date, Session["FolderID"].ToString()));

                        //int returnstatusss = objOperationClass.ExecuteNonQuery(string.Format(@"insert into tblAttendance(folderid,status,userid,UserName,EmailID) values('{0}','Present','{1}','{2}','{3}')", Session["FolderID"].ToString(), dt.Rows[i]["UserId"].ToString(), dt.Rows[i]["UserName"].ToString(), dt.Rows[i]["EmailID"].ToString()));
                        //int returnstatus3 = objOperationClass.ExecuteNonQuery(string.Format(@"insert into tblAttendeesStatus(folderid,status,userid,UserName,EmailID) values('{0}','Present','{1}','{2}','{3}')", Session["FolderID"].ToString(), dt.Rows[i]["UserId"].ToString(), dt.Rows[i]["UserName"].ToString(), dt.Rows[i]["EmailID"].ToString()));
                        //int returnstatussss1 = objOperationClass.ExecuteNonQuery(string.Format(@"insert into tblfileactions(UserID,FileName,ActionName,ActionDate,Status,FolderId) values({0},'{1}','Present','{2}',2,{3})", dt.Rows[i]["UserId"].ToString(), CommeetteeName, dt2.Date, Session["FolderID"].ToString()));
                    }
                    else
                    {
                        int returnstatusss = objOperationClass.ExecuteNonQuery(string.Format(@"insert into tblAttendance(folderid,status,userid,UserName,EmailID) values('{0}','Present','{1}','{2}','{3}')", Session["FolderID"].ToString(), dt.Rows[i]["UserId"].ToString(), dt.Rows[i]["UserName"].ToString(), dt.Rows[i]["EmailID"].ToString()));
                        int returnstatus3 = objOperationClass.ExecuteNonQuery(string.Format(@"insert into tblAttendeesStatus(folderid,status,userid,UserName,EmailID) values('{0}','Present','{1}','{2}','{3}')", Session["FolderID"].ToString(), dt.Rows[i]["UserId"].ToString(), dt.Rows[i]["UserName"].ToString(), dt.Rows[i]["EmailID"].ToString()));
                        int returnstatussss1 = objOperationClass.ExecuteNonQuery(string.Format(@"insert into tblfileactions(UserID,FileName,ActionName,ActionDate,Status,FolderId) values({0},'{1}','Present','{2}',2,{3})", dt.Rows[i]["UserId"].ToString(), CommeetteeName, dt2.Date, Session["FolderID"].ToString()));
                    }
                }
            }
            else
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //int returnstatusss = objOperationClass.ExecuteNonQuery(string.Format(@"insert into tblAttendance(folderid,status,userid,UserName,EmailID) values('{0}','Present','{1}','{2}','{3)'", Session["FolderID"].ToString(), dt.Rows[i]["UserId"].ToString(), dt.Rows[i]["UserName"].ToString(), dt.Rows[i]["EmailID"].ToString()));
                    //int returnstatus3 = objOperationClass.ExecuteNonQuery(string.Format(@"insert into tblAttendeesStatus(folderid,status,userid,UserName,EmailID) values('{0}','Present','{1}','{2}','{3}')", Session["FolderID"].ToString(), dt.Rows[i]["UserId"].ToString(), dt.Rows[i]["UserName"].ToString(), dt.Rows[i]["EmailID"].ToString()));
                    //int returnstatusss = objOperationClass.ExecuteNonQuery(string.Format(@"insert into tblAttendance(folderid,status,userid,UserName,EmailID) values('{0}','Present','{1}','{2}','{3)'", Session["FolderID"].ToString(), dt.Rows[i]["UserId"].ToString(), dt.Rows[i]["UserName"].ToString(), dt.Rows[i]["EmailID"].ToString()));
                    //int returnstatus3 = objOperationClass.ExecuteNonQuery(string.Format(@"insert into tblAttendeesStatus(folderid,status,userid,UserName,EmailID) values('{0}','Present','{1}','{2}','{3}')", Session["FolderID"].ToString(), dt.Rows[i]["UserId"].ToString(), dt.Rows[i]["UserName"].ToString(), dt.Rows[i]["EmailID"].ToString()));

                    int returnstatusss = objOperationClass.ExecuteNonQuery(string.Format(@"insert into tblAttendance(folderid,status,userid,UserName,EmailID) values('{0}','Present','{1}','{2}','{3}')", Session["FolderID"].ToString(), dt.Rows[i]["UserId"].ToString(), dt.Rows[i]["UserName"].ToString(), dt.Rows[i]["EmailID"].ToString()));
                    int returnstatus3 = objOperationClass.ExecuteNonQuery(string.Format(@"insert into tblAttendeesStatus(folderid,status,userid,UserName,EmailID) values('{0}','Present','{1}','{2}','{3}')", Session["FolderID"].ToString(), dt.Rows[i]["UserId"].ToString(), dt.Rows[i]["UserName"].ToString(), dt.Rows[i]["EmailID"].ToString()));
                    int returnstatussss1 = objOperationClass.ExecuteNonQuery(string.Format(@"insert into tblfileactions(UserID,FileName,ActionName,ActionDate,Status,FolderId) values({0},'{1}','Present','{2}',2,{3})", dt.Rows[i]["UserId"].ToString(), CommeetteeName, dt2.Date, Session["FolderID"].ToString()));

                }
            }

            Response.Redirect("~/MOMPreparation/StartMOM.aspx", false);
            ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('Meeting Activated Successfully.')", true);
            //Page p = (Page)System.Web.HttpContext.Current.Handler;
            //ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('Meetimg Status updated successfully.')", true);


            //}
            ////ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('Meetimg Status updated successfully.')", true);
            //else
            //{
            //    Page p = (Page)System.Web.HttpContext.Current.Handler;
            //    ScriptManager.RegisterClientScriptBlock(p, typeof(UpdatePanel), "msg", "alert('Meeting Status not updated.')", true);
            //    //ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('Meeting Status not updated.')", false);
            //}
            pnlComplete.Visible = false;
            //DirectorDataBinding();
        }
        catch (Exception ex)
        {
            //Session["Error"] = ex.StackTrace;
            //Response.Redirect("../ErrorMessage.aspx", false);
        }
    }
    #endregion




    //#region [btnComplete_Click]
    //protected void btnComplete_Click(object sender, EventArgs e)
    //{
    //    //pnlComplete.Visible = true;
    //    //pnlComplete.Visible = true;
    //    try
    //    {
    //        objCommonBAL = new CommonBAL();
    //        int fId = Convert.ToInt32(Session["FolderID"]);
    //        OperationClass objOperationClass = new OperationClass();
    //        //int returnstatus = objOperationClass.ExecuteNonQuery(string.Format(@"update tblfolder set MeetingStatus = 1 where folderid = {0}", fId));
    //        //if (returnstatus > 0)
    //        //{
    //        DataTable dt = objOperationClass.GetTable4Command(string.Format(@"select UserId,Firstname+' '+Lastname 'UserName',EmailID from tbluserdetail where userid in (select distinct userid from tbluseraccesscontrol where  folderid='{0}' and accesssymbol='F')", Session["FolderID"].ToString()));
    //        DataTable dtAttendance = objOperationClass.GetTable4Command(string.Format(@"select * from tblAttendance where  folderid='{0}'", Session["FolderID"].ToString()));
    //        if (dtAttendance != null && dtAttendance.Rows.Count > 0)
    //        {

    //        }
    //        else
    //        {
    //            for (int i = 0; i < dt.Rows.Count; i++)
    //            {
    //                //int returnstatusss = objOperationClass.ExecuteNonQuery(string.Format(@"insert into tblAttendance(folderid,status,userid,UserName,EmailID) values('{0}','Present','{1}','{2}','{3)'", Session["FolderID"].ToString(), dt.Rows[i]["UserId"].ToString(), dt.Rows[i]["UserName"].ToString(), dt.Rows[i]["EmailID"].ToString()));
    //                //int returnstatus3 = objOperationClass.ExecuteNonQuery(string.Format(@"insert into tblAttendeesStatus(folderid,status,userid,UserName,EmailID) values('{0}','Present','{1}','{2}','{3}')", Session["FolderID"].ToString(), dt.Rows[i]["UserId"].ToString(), dt.Rows[i]["UserName"].ToString(), dt.Rows[i]["EmailID"].ToString()));
    //                //int returnstatusss = objOperationClass.ExecuteNonQuery(string.Format(@"insert into tblAttendance(folderid,status,userid,UserName,EmailID) values('{0}','Present','{1}','{2}','{3)'", Session["FolderID"].ToString(), dt.Rows[i]["UserId"].ToString(), dt.Rows[i]["UserName"].ToString(), dt.Rows[i]["EmailID"].ToString()));
    //                //int returnstatus3 = objOperationClass.ExecuteNonQuery(string.Format(@"insert into tblAttendeesStatus(folderid,status,userid,UserName,EmailID) values('{0}','Present','{1}','{2}','{3}')", Session["FolderID"].ToString(), dt.Rows[i]["UserId"].ToString(), dt.Rows[i]["UserName"].ToString(), dt.Rows[i]["EmailID"].ToString()));

    //                int returnstatusss = objOperationClass.ExecuteNonQuery(string.Format(@"insert into tblAttendance(folderid,status,userid,UserName,EmailID) values('{0}','Present','{1}','{2}','{3}')", Session["FolderID"].ToString(), dt.Rows[i]["UserId"].ToString(), dt.Rows[i]["UserName"].ToString(), dt.Rows[i]["EmailID"].ToString()));
    //                int returnstatus3 = objOperationClass.ExecuteNonQuery(string.Format(@"insert into tblAttendeesStatus(folderid,status,userid,UserName,EmailID) values('{0}','Present','{1}','{2}','{3}')", Session["FolderID"].ToString(), dt.Rows[i]["UserId"].ToString(), dt.Rows[i]["UserName"].ToString(), dt.Rows[i]["EmailID"].ToString()));
    //            }
    //        }

    //        Response.Redirect("~/MOMPreparation/StartMOM.aspx", false);
    //        ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('Meeting Activated Successfully.')", true);
    //        //Page p = (Page)System.Web.HttpContext.Current.Handler;
    //        //ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('Meetimg Status updated successfully.')", true);


    //        //}
    //        ////ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('Meetimg Status updated successfully.')", true);
    //        //else
    //        //{
    //        //    Page p = (Page)System.Web.HttpContext.Current.Handler;
    //        //    ScriptManager.RegisterClientScriptBlock(p, typeof(UpdatePanel), "msg", "alert('Meeting Status not updated.')", true);
    //        //    //ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('Meeting Status not updated.')", false);
    //        //}
    //        pnlComplete.Visible = false;
    //        //DirectorDataBinding();
    //    }
    //    catch (Exception ex)
    //    {
    //        //Session["Error"] = ex.StackTrace;
    //        //Response.Redirect("../ErrorMessage.aspx", false);
    //    }
    //}
    //#endregion

    //#region [btnComplete_Click]
    //protected void btnComplete_Click(object sender, EventArgs e)
    //{
    //    //pnlComplete.Visible = true;
    //    //pnlComplete.Visible = true;
    //    try
    //    {
    //        objCommonBAL = new CommonBAL();
    //        int fId = Convert.ToInt32(Session["FolderID"]);
    //        OperationClass objOperationClass = new OperationClass();
    //        //int returnstatus = objOperationClass.ExecuteNonQuery(string.Format(@"update tblfolder set MeetingStatus = 1 where folderid = {0}", fId));
    //        //if (returnstatus > 0)
    //        //{
    //        DataTable dt = objOperationClass.GetTable4Command(string.Format(@"select UserId,Firstname+' '+Lastname 'UserName',EmailID from tbluserdetail where userid in (select distinct userid from tbluseraccesscontrol where  folderid='{0}' and accesssymbol='F')", Session["FolderID"].ToString()));
    //        DataTable dtAttendance = objOperationClass.GetTable4Command(string.Format(@"select * from tblAttendance where  folderid='{0}'", Session["FolderID"].ToString()));
    //        if (dtAttendance != null && dtAttendance.Rows.Count > 0)
    //        {

    //        }
    //        else
    //        {
    //            for (int i = 0; i < dt.Rows.Count; i++)
    //            {
    //                int returnstatusss = objOperationClass.ExecuteNonQuery(string.Format(@"insert into tblAttendance(folderid,status,userid,UserName,EmailID) values('{0}','Present','{1}','{2}','{3)'", Session["FolderID"].ToString(), dt.Rows[i]["UserId"].ToString(), dt.Rows[i]["UserName"].ToString(), dt.Rows[i]["EmailID"].ToString()));
    //                int returnstatus3 = objOperationClass.ExecuteNonQuery(string.Format(@"insert into tblAttendeesStatus(folderid,status,userid,UserName,EmailID) values('{0}','Present','{1}','{2}','{3}')", Session["FolderID"].ToString(), dt.Rows[i]["UserId"].ToString(), dt.Rows[i]["UserName"].ToString(), dt.Rows[i]["EmailID"].ToString()));
    //            }
    //        }

    //        Response.Redirect("~/MOMPreparation/StartMOM.aspx", false);
    //        ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('Meeting Activated Successfully.')", true);
    //        //Page p = (Page)System.Web.HttpContext.Current.Handler;
    //        //ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('Meetimg Status updated successfully.')", true);


    //        //}
    //        ////ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('Meetimg Status updated successfully.')", true);
    //        //else
    //        //{
    //        //    Page p = (Page)System.Web.HttpContext.Current.Handler;
    //        //    ScriptManager.RegisterClientScriptBlock(p, typeof(UpdatePanel), "msg", "alert('Meeting Status not updated.')", true);
    //        //    //ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('Meeting Status not updated.')", false);
    //        //}
    //        pnlComplete.Visible = false;
    //        //DirectorDataBinding();
    //    }
    //    catch (Exception ex)
    //    {
    //        //Session["Error"] = ex.StackTrace;
    //        //Response.Redirect("../ErrorMessage.aspx", false);
    //    }




    //}
    //#endregion

    #region [pnlComplete_Click]
    protected void pnlComplete_Click(object sender, EventArgs e)
    {
        try
        {


            objCommonBAL = new CommonBAL();
            int fId = Convert.ToInt32(Session["FolderID"]);
            int returnstatus = objCommonBAL.UpdateMeetingStatus(fId);
            if (returnstatus > 0)
            {
                Page p = (Page)System.Web.HttpContext.Current.Handler;
                ScriptManager.RegisterClientScriptBlock(p, typeof(UpdatePanel), "msg", "alert('Meeting Completed successfully. Now, this can be moved in Archived Meetings')", true);



            }
            //ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('Meetimg Status updated successfully.')", true);
            else
            {
                Page p = (Page)System.Web.HttpContext.Current.Handler;
                ScriptManager.RegisterClientScriptBlock(p, typeof(UpdatePanel), "msg", "alert('Meeting Status not updated.')", true);
                //ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('Meeting Status not updated.')", false);
            }
            pnlComplete.Visible = false;
            //DirectorDataBinding();
        }
        catch (Exception ex)
        {
            Session["Error"] = ex.StackTrace;
            Response.Redirect("../ErrorMessage.aspx", false);
        }
    }
    #endregion



    #region [pnlCancel_Click]
    protected void pnlCancel_Click(object sender, EventArgs e)
    {
        try
        {
            objCommonBAL = new CommonBAL();
            int fId = Convert.ToInt32(Session["FolderID"]);
            OperationClass objOperationClass = new OperationClass();
            int returnstatus = objOperationClass.ExecuteNonQuery(string.Format(@"update tblfolder set MeetingCancelled = 1 where folderid = {0}", fId));
            if (returnstatus > 0)
            {
                Page p = (Page)System.Web.HttpContext.Current.Handler;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "redirect",
                        "alert('Meeting Cancelled Successfully'); window.location='" +
                        Request.ApplicationPath + "/default/Default.aspx';", true);

                pnlCancel.Visible = false;
            }

            else
            {
                Page p = (Page)System.Web.HttpContext.Current.Handler;
                ScriptManager.RegisterClientScriptBlock(p, typeof(UpdatePanel), "msg", "alert('Meeting not Cancelled.')", true);

            }

        }
        catch (Exception ex)
        {
            Session["Error"] = ex.StackTrace;
            Response.Redirect("../ErrorMessage.aspx", false);
        }
    }
    #endregion

    #region [btnBoxCancel_Click]
    protected void btnBoxCancel_Click(object sender, EventArgs e)
    {
        pnlCancel.Visible = false;
        pnlComplete.Visible = false;
    }
    #endregion

    #region btnYes_Click]
    protected void btnYes_Click(object sender, EventArgs e)
    {

        objFileUploadBAL = new FileUploadBAL();
        objFileUploadController = new FileUploadController();
        int value2;
        if (ViewState["strFileId"] != null)
        {
            string strFileId = (string)ViewState["strFileId"];

            if (strFileId != "")
            {
                try
                {
                    string strrrr = operation.ExecuteScalar4Command(string.Format(@"select ParentFolderId from tblfolder where folderid={0}", Session["FolderID"].ToString()));
                    if (strrrr != "1")
                    {
                        string TableNamenew = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", ddlCommitteenew.SelectedValue));
                        foreach (GridViewRow row in gvData.Rows)
                        {
                            CheckBox chkSelect = (CheckBox)row.FindControl("chkSelect");
                            if (chkSelect.Checked)
                            {
                                Label Id = (Label)row.FindControl("lblId");
                                Label FileId = (Label)row.FindControl("lblFileId");
                                LinkButton FileName = (LinkButton)row.FindControl("lnkOpen");
                                string[] FileNameID = FileName.CommandArgument.ToString().Split(new Char[] { ',' });
                                string[] filename = FileNameID[1].Split(new Char[] { '.' });
                                string name = filename[0].ToString();
                                DataSet dss = new DataSet();


                                if (System.IO.File.Exists(Server.MapPath("~\\Repository") + "\\" + name + "_" + Session["FolderID"].ToString() + ".enc"))
                                {
                                    if (!System.IO.File.Exists(Server.MapPath("~\\Repository") + "\\" + name + "_" + ddlCommitteenew.SelectedValue.ToString() + ".enc"))
                                    {
                                        System.IO.File.Move(Server.MapPath("~\\Repository") + "\\" + name + "_" + Session["FolderID"].ToString() + ".enc", Server.MapPath("~\\Repository") + "\\" + name + "_" + ddlCommitteenew.SelectedValue.ToString() + ".enc");

                                        dss = operation.GetDataSet4Command(string.Format(@"select * from tblFolderIndexMaster where folder_id={0}", Session["FolderID"].ToString()));
                                        if (dss.Tables[0].Rows.Count > 0)
                                        {
                                            DataSet dsMeet = operation.GetDataSet4Command(string.Format(@"select * from {0} where FileId={1}", dss.Tables[0].Rows[0]["TableName"].ToString(), Convert.ToInt32(FileId.Text.ToString())));

                                            if (dsMeet.Tables[0].Rows.Count > 0)
                                            {

                                                string ImportedBy = dsMeet.Tables[0].Rows[0]["ImportedBy"].ToString();
                                                string DocStatus = dsMeet.Tables[0].Rows[0]["DocStatus"].ToString();
                                                string Column3 = dsMeet.Tables[0].Rows[0]["Column3"].ToString();
                                                string Column0 = dsMeet.Tables[0].Rows[0]["Column0"].ToString();
                                                string Column1 = dsMeet.Tables[0].Rows[0]["Column1"].ToString();
                                                string Column2 = dsMeet.Tables[0].Rows[0]["Column2"].ToString();
                                                string value1 = operation.ExecuteScalar4Command(string.Format(@"insert into {0} (FileId,ImportedBy,DocStatus,FolderId,Column0,Column1,Column2,Column3) values ('" + Convert.ToInt32(FileId.Text.ToString()) + "','" + ImportedBy + "','" + DocStatus + "', '" + ddlMeetingDate.SelectedValue.ToString() + "', '" + Column0 + "','" + Column1 + "','" + Column2 + "','" + Column3 + "')", TableNamenew));
                                                string Deletestatement = string.Format("delete from {0} where Fileid={1}", dss.Tables[0].Rows[0]["TableName"].ToString(), Convert.ToInt32(FileId.Text.ToString()));
                                                operation.Insert4Command(Deletestatement);

                                            }
                                        }
                                        value2 = Convert.ToInt16(operation.Insert4Command(string.Format(@"update tblfile set folderid={0} where fileid={1}", Convert.ToInt32(ddlCommitteenew.SelectedValue.ToString()), Convert.ToInt32(FileId.Text.ToString()))));
                                    }

                                    else
                                    {

                                        dss = operation.GetDataSet4Command(string.Format(@"select * from tblFolderIndexMaster where folder_id={0}", Session["FolderID"].ToString()));
                                        string Deletestatement = string.Format("delete from {0} where Fileid={1}", dss.Tables[0].Rows[0]["TableName"].ToString(), Convert.ToInt32(FileId.Text.ToString()));
                                        operation.Insert4Command(Deletestatement);
                                        value2 = Convert.ToInt16(operation.Insert4Command(string.Format(@"delete from tblfile where fileid={0}", Convert.ToInt32(FileId.Text.ToString()))));
                                    }
                                    DirectorDataBinding();
                                    Panel4.Visible = false;
                                }
                                else
                                {
                                    dss = operation.GetDataSet4Command(string.Format(@"select * from tblFolderIndexMaster where folder_id={0}", Session["FolderID"].ToString()));
                                    string Deletestatement = string.Format("delete from {0} where Fileid={1}", dss.Tables[0].Rows[0]["TableName"].ToString(), Convert.ToInt32(FileId.Text.ToString()));
                                    operation.Insert4Command(Deletestatement);
                                    value2 = Convert.ToInt16(operation.Insert4Command(string.Format(@"delete from tblfile where fileid={0}", Convert.ToInt32(FileId.Text.ToString()))));

                                }

                            }
                        }
                        DirectorDataBinding();
                        Panel4.Visible = false;
                    }
                    else
                    {
                        string TableNamenew = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", ddlCommitteenew.SelectedValue));
                        foreach (GridViewRow row in gvParent.Rows)
                        {
                            CheckBox chkSelect = (CheckBox)row.FindControl("chkSelect");
                            if (chkSelect.Checked)
                            {
                                Label Id = (Label)row.FindControl("lblId");
                                Label FileId = (Label)row.FindControl("lblFileId");
                                LinkButton FileName = (LinkButton)row.FindControl("lnkView");
                                string[] FileNameID = FileName.CommandArgument.ToString().Split(new Char[] { ',' });
                                string[] filename = FileNameID[1].Split(new Char[] { '.' });
                                string name = filename[0].ToString();
                                DataSet dss = new DataSet();
                                if (System.IO.File.Exists(Server.MapPath("~\\Repository") + "\\" + name + "_" + Session["FolderID"].ToString() + ".enc"))
                                {
                                    if (!System.IO.File.Exists(Server.MapPath("~\\Repository") + "\\" + name + "_" + ddlCommitteenew.SelectedValue.ToString() + ".enc"))
                                    {
                                        System.IO.File.Move(Server.MapPath("~\\Repository") + "\\" + name + "_" + Session["FolderID"].ToString() + ".enc", Server.MapPath("~\\Repository") + "\\" + name + "_" + ddlCommitteenew.SelectedValue.ToString() + ".enc");
                                        dss = operation.GetDataSet4Command(string.Format(@"select * from tblFolderIndexMaster where folder_id={0}", Session["FolderID"].ToString()));
                                        if (dss.Tables[0].Rows.Count > 0)
                                        {
                                            DataSet dsMeet = operation.GetDataSet4Command(string.Format(@"select * from {0} where FileId={1}", dss.Tables[0].Rows[0]["TableName"].ToString(), Convert.ToInt32(FileId.Text.ToString())));

                                            if (dsMeet.Tables[0].Rows.Count > 0)
                                            {

                                                string ImportedBy = dsMeet.Tables[0].Rows[0]["ImportedBy"].ToString();
                                                string DocStatus = dsMeet.Tables[0].Rows[0]["DocStatus"].ToString();
                                                string Column3 = dsMeet.Tables[0].Rows[0]["Column3"].ToString();
                                                string Column0 = dsMeet.Tables[0].Rows[0]["Column0"].ToString();
                                                string Column1 = dsMeet.Tables[0].Rows[0]["Column1"].ToString();
                                                string Column2 = dsMeet.Tables[0].Rows[0]["Column2"].ToString();
                                                string value1 = operation.ExecuteScalar4Command(string.Format(@"insert into {0} (FileId,ImportedBy,DocStatus,FolderId,Column0,Column1,Column2,Column3) values ('" + Convert.ToInt32(FileId.Text.ToString()) + "','" + ImportedBy + "','" + ddlMeetingDate.SelectedValue.ToString() + "', '" + DocStatus + "', '" + Column0 + "','" + Column1 + "','" + Column2 + "','" + Column3 + "')", TableNamenew));
                                                string Deletestatement = string.Format("delete from {0} where Fileid={1}", dss.Tables[0].Rows[0]["TableName"].ToString(), Convert.ToInt32(FileId.Text.ToString()));
                                                operation.Insert4Command(Deletestatement);

                                            }
                                        }
                                        value2 = Convert.ToInt16(operation.Insert4Command(string.Format(@"update tblfile set folderid={0} where fileid={1}", Convert.ToInt32(ddlCommitteenew.SelectedValue.ToString()), Convert.ToInt32(FileId.Text.ToString()))));
                                    }
                                    else
                                    {

                                        dss = operation.GetDataSet4Command(string.Format(@"select * from tblFolderIndexMaster where folder_id={0}", Session["FolderID"].ToString()));
                                        string Deletestatement = string.Format("delete from {0} where Fileid={1}", dss.Tables[0].Rows[0]["TableName"].ToString(), Convert.ToInt32(FileId.Text.ToString()));
                                        operation.Insert4Command(Deletestatement);
                                        value2 = Convert.ToInt16(operation.Insert4Command(string.Format(@"delete from tblfile where fileid={0}", Convert.ToInt32(FileId.Text.ToString()))));

                                    }
                                    bindLvFileView();
                                    Panel4.Visible = false;
                                }
                                else
                                {

                                    dss = operation.GetDataSet4Command(string.Format(@"select * from tblFolderIndexMaster where folder_id={0}", Session["FolderID"].ToString()));
                                    string Deletestatement = string.Format("delete from {0} where Fileid={1}", dss.Tables[0].Rows[0]["TableName"].ToString(), Convert.ToInt32(FileId.Text.ToString()));
                                    operation.Insert4Command(Deletestatement);
                                    value2 = Convert.ToInt16(operation.Insert4Command(string.Format(@"delete from tblfile where fileid={0}", Convert.ToInt32(FileId.Text.ToString()))));

                                }

                            }
                        }
                        bindLvFileView();
                        Panel4.Visible = false;


                    }
                }
                catch (Exception ex)
                { }

            }
            else
            {
                lblMessage.Text = "Please select atleast one file.";
            }
        }
        else
        {
            lblMessage.Text = "Please select atleast one file.";
        }
    }
    #endregion

    #region [ btnNO_ClickMy]
    protected void btnNO_Click(object sender, EventArgs e)
    {
        Panel4.Visible = false;
        OperationClass operation = new OperationClass();
        if (operation.ExecuteScalar4Command(string.Format(@"select parentfolderid from tblFolder where folderid={0}", Convert.ToInt32(Session["FolderID"]))) == "1")
            bindLvFileView();
        else
            DirectorDataBinding();
        return;
    }
    #endregion

    #region [btnMoveFile_Click]
    protected void btnMoveFile_Click(object sender, EventArgs e)
    {
        DataSet dsTreeViewNode = new DataSet();
        OperationClass objOperationClass = new OperationClass();
        string SqlQuery;

        bool temp = false;
        foreach (GridViewRow row in gvParent.Rows)
        {
            CheckBox chkSelect = (CheckBox)row.FindControl("chkSelect");
            if (chkSelect.Checked)
            {
                temp = true;
                break;
            }
        }
        if (temp)
        {
            if (Convert.ToString(Session["Groupname"]).ToLower() == "admin")
            {
                SqlQuery = "SELECT [FolderName], [FolderId] FROM [tblFolder] WHERE ([ParentFolderId] = 1) AND   [DeleteStatus] !=1 AND [MEETINGSTATUS]!=1 AND [MEETINGCANCELLED]!=1 and ProposedMeetings!=1";
                DataTable dt = objOperationClass.GetTable4Command(SqlQuery);
                if (dt.Rows.Count != 0)
                {
                    ddlCommittee.DataSource = dt;
                    ddlCommittee.DataTextField = "FolderName";
                    ddlCommittee.DataValueField = "FolderId";
                    ddlCommittee.DataBind();
                    ddlCommittee.Items.Insert(0, new System.Web.UI.WebControls.ListItem("---Select---", "0"));
                    ddlMeetingDate.Items.Insert(0, new System.Web.UI.WebControls.ListItem("---Select---", "0"));
                }
            }
            else if (Convert.ToString(Session["Groupname"]).ToLower() != "admin")
            {
                objCommonBAL = new CommonBAL();
                dsTreeViewNode = objCommonBAL.GetTreeViewNode(Convert.ToInt32(Session["GroupID"]), Convert.ToInt32(Session["UserID"]));
                if (dsTreeViewNode.Tables[0].Rows.Count != 0)
                {
                    ddlCommittee.DataSource = dsTreeViewNode;
                    ddlCommittee.DataTextField = "FolderName";
                    ddlCommittee.DataValueField = "FolderId";
                    ddlCommittee.DataBind();
                    ddlCommittee.Items.Insert(0, new System.Web.UI.WebControls.ListItem("---Select---", "0"));
                    ddlMeetingDate.Items.Insert(0, new System.Web.UI.WebControls.ListItem("---Select---", "0"));
                }
            }

            //SqlQuery = "SELECT FolderName,FolderId FROM tblFolder WHERE [MEETINGSTATUS]!=1 AND [MEETINGCANCELLED]!=1 AND ParentFolderId ='" + Session["FolderID"] + "'";
            //DataTable dt1 = objOperationClass.GetTable4Command(SqlQuery);
            //if (dt1.Rows.Count != 0)
            //{
            //    ddlMeetingDate.DataSource = dt1;
            //    ddlMeetingDate.DataTextField = "FolderName";
            //    ddlMeetingDate.DataValueField = "FolderId";
            //    ddlMeetingDate.DataBind();
            //}
            pnlMove.Visible = true;
        }
        else
            ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('Please select the file to Move.')", true);
    }
    #endregion btnMoveFile_Click

    protected void ddlCommittee_SelectedIndexChanged(object sender, EventArgs e)
    {
        pnlMove.Visible = true;
        OperationClass objOperationClass = new OperationClass();
        string SqlQuery;


        bool temp = false;
        foreach (GridViewRow row in gvParent.Rows)
        {
            CheckBox chkSelect = (CheckBox)row.FindControl("chkSelect");
            if (chkSelect.Checked)
            {
                temp = true;
                break;
            }
        }
        if (temp)
        {
            DataTable dt1 = null;
            if (ddlCommittee.SelectedIndex != 0)
            {
                SqlQuery = "SELECT FolderName,FolderId FROM tblFolder WHERE [MEETINGSTATUS]!=1 AND [MEETINGCANCELLED]!=1 AND [DeleteStatus] !=1  AND ParentFolderId ='" + ddlCommittee.SelectedValue + "'";
                dt1 = objOperationClass.GetTable4Command(SqlQuery);
                if (dt1.Rows.Count != 0)
                {
                    ddlMeetingDate.DataSource = dt1;
                    ddlMeetingDate.DataTextField = "FolderName";
                    ddlMeetingDate.DataValueField = "FolderId";
                    ddlMeetingDate.DataBind();

                }
                else
                {
                    ddlMeetingDate.DataSource = dt1;
                    ddlMeetingDate.DataBind();
                }
            }
            else if (ddlCommittee.SelectedIndex == 0)
            {
                ddlMeetingDate.DataSource = dt1;
                ddlMeetingDate.DataBind();
                //rdmeeting.Checked = true;
            }

            pnlMove.Visible = true;
        }
        else
        {
            ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('Please select the file to Move.')", true);
        }
    }


    private void DirectorDataBinding()
    {
        DataSet dsFieldName = new DataSet();
        DataSet dsFolderAccess = new DataSet();
        DataTable dtFolderdt = new DataTable();
        DataTable dtfiledetails = new DataTable();
        try
        {
            objFolderBAL = new FolderBAL();
            objCommonBAL = new CommonBAL();

            //if fodlerid empty then return to the page without doing anythings.
            #region If folderid equal to empty
            if (Convert.ToString(Session["FolderId"]) == "")
            {
                //Get access details on basis of groupid.
                dsFolderAccess = objFolderBAL.GeFolderId(Convert.ToInt32(Session["GroupID"]));

                //if folder has a right to access (Symbol's like 'F','M',"R','L')
                if (dsFolderAccess.Tables[0].Rows.Count > 0)
                {
                    //get folder detail on basis of folderid.
                    dtFolderdt = objCommonBAL.GetFolderDetail(Convert.ToInt64(dsFolderAccess.Tables[0].Rows[0]["FolderID"]));
                    if (dtFolderdt.Rows.Count > 0)
                    {
                        //set session of folder id to use through out application open
                        Session["FolderID"] = dtFolderdt.Rows[0]["FolderID"].ToString();
                    }
                }
                else
                {
                    //btnDelete.Visible = false;
                    btnExport.Visible = false;
                    btnSMS.Visible = false;
                    //btnMoveFile.Visible = false;
                    //lblSelect.Visible = false;
                    //lnkSelectAll.Visible = false;
                    //LinkButton2.Visible = false;
                    //btnCopyFiles.Visible = false;

                    return;
                }
            }
            #endregion
            /*added by nilesh sonpasare on 04/06/2012
             *  ConfigurationManager.AppSettings["AccessType"]
             * if AccessType userwise then pass value 1 and groupwise then pass 0 to 
             * CheckAccessRight,GetParentChildFolderId methodes.
             */

            //Check folder access and then do work as per rights to the selected folder.
            //string Symbol = objCommonBAL.GetSymbolstring(Convert.ToInt32(Session["FolderID"]), Convert.ToInt32(Session["GroupID"]), Convert.ToInt32(Session["UserID"]), ConfigurationManager.AppSettings["AccessType"].ToString().ToLower() == "userwise" ? 1 : 0);
            string Symbol = objCommonBAL.GetSymbolstring(Convert.ToInt32(Session["FolderID"]), Convert.ToInt32(Session["GroupID"]), Convert.ToInt32(Session["UserID"]), ConfigurationManager.AppSettings["AccessType"].ToString().ToLower() == "userwise" ? 1 : 0);
            //if symbol not equal to empty or group have a right to access folder.
            if (Symbol != "" || Symbol.ToUpper() != "N")
            {
                //Make button visible true false as per rights to the folder.
                #region Check symbol & make button true and false .
                switch (Symbol)
                {
                    case "F":
                        if (ConfigurationManager.AppSettings["Application"].ToString() == "hindalco")
                        {
                            if (Session["GroupName"].ToString().ToLower() == "admin")
                            {
                                //btnDelete.Visible = true;
                                btnExport.Visible = true;
                                btnSMS.Visible = true;
                                btnComplete.Visible = true;
                                //btnMoveFile.Visible = true;
                                //lblSelect.Visible = true;
                                //lnkSelectAll.Visible = true;
                                //LinkButton2.Visible = true;
                                //btnCopyFiles.Visible = true;
                            }
                            else
                            {
                                //btnDelete.Visible = false;
                                //btnExport.Visible = true;
                                //btnComplete.Visible = true;
                                //btnMoveFile.Visible = false;
                                //lblSelect.Visible = true;
                                //lnkSelectAll.Visible = true;
                                //LinkButton2.Visible = true;
                                //btnCopyFiles.Visible = false;
                            }
                        }
                        else
                        {
                            if (Session["GroupName"].ToString().ToLower() == "admin")
                            {
                                //btnDelete.Visible = true;
                                btnComplete.Visible = true;
                                btnExport.Visible = true;
                                btnSMS.Visible = true;
                                //btnMoveFile.Visible = true;
                                //lblSelect.Visible = true;
                                //lnkSelectAll.Visible = true;
                                //LinkButton2.Visible = true;
                                //btnCopyFiles.Visible = true;
                            }
                            else
                            {
                                //btnDelete.Visible = false;
                                //btnComplete.Visible = true;
                                btnExport.Visible = true;
                                btnSMS.Visible = true;

                                //btnnotebook.Visible = false;
                                //btnMoveFile.Visible = true;
                                //lblSelect.Visible = true;
                                //lnkSelectAll.Visible = true;
                                //LinkButton2.Visible = true;
                                //btnCopyFiles.Visible = true;
                            }
                        }
                        break;
                    case "M":
                        if (ConfigurationManager.AppSettings["Application"].ToString() == "hindalco")
                        {
                            //btnDelete.Visible = false;
                            btnExport.Visible = true;
                            btnSMS.Visible = true;
                            //btnMoveFile.Visible = false;
                            //lblSelect.Visible = true;
                            //lnkSelectAll.Visible = true;
                            //LinkButton2.Visible = true;
                            //btnCopyFiles.Visible = false;
                        }
                        else
                        {
                            //btnDelete.Visible = false;
                            btnExport.Visible = true;
                            btnSMS.Visible = true;
                            //btnMoveFile.Visible = false;
                            //lblSelect.Visible = true;
                            //lnkSelectAll.Visible = true;
                            //LinkButton2.Visible = true;
                            //btnCopyFiles.Visible = false;
                        }
                        break;
                    case "R":
                        //btnDelete.Visible = false;
                        btnExport.Visible = false;
                        btnSMS.Visible = false;
                        //btnMoveFile.Visible = false;
                        //lblSelect.Visible = false;
                        //lnkSelectAll.Visible = false;
                        //LinkButton2.Visible = false;
                        //btnCopyFiles.Visible = false;

                        break;

                    case "L":
                        //btnDelete.Visible = false;
                        btnExport.Visible = false;
                        btnSMS.Visible = false;
                        //btnMoveFile.Visible = false;
                        //lblSelect.Visible = false;
                        //lnkSelectAll.Visible = false;
                        //LinkButton2.Visible = false;
                        //btnCopyFiles.Visible = false;

                        break;
                    case "N":
                        //btnDelete.Visible = false;
                        btnExport.Visible = false;
                        btnSMS.Visible = false;
                        //btnMoveFile.Visible = false;
                        //btnCopyFiles.Visible = false;

                        break;
                }
                #endregion


                gvData.Visible = true;
                DataTable dtfiledetails1 = null;
                //tblMeetingInformation.Visible = true;
                OperationClass operation = new OperationClass();
                string TableName = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Convert.ToInt32(Session["FolderID"])));
                dtfiledetails = operation.GetTable4Command(
                string.Format(@"select a.fileid,FileName,ApprovalStatus,withdrawcomments,Column0 'Itemno',column1 'Particulars',column2 'Purpose',column3 'GM',b.PageCount from {0} a,tblfile b where a.fileid =b.fileid and b.folderid={1}  and column1 is not null order by column0", TableName, Convert.ToInt32(Session["FolderID"])));

                if (Request.QueryString["Value"] != null && Request.QueryString["Value"] != "meeting")
                {
                    if (Request.QueryString["Value"].ToString().ToLower() == "my briefcase")
                    {
                        string FolderIDs = operation.ExecuteScalar4Command(string.Format(@"select Folderid from tblfolder where foldername='my briefcase' and deletestatus!=1"));
                        string TableNames = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Convert.ToInt32(FolderIDs)));
                        dtfiledetails1 = objCommonBAL.GetDispFileName(TableNames, Convert.ToInt32(FolderIDs), Convert.ToInt32(Session["UserID"]));
                        ((Control)Master.FindControl("ddlPageNumber")).Visible = false;


                    }
                    else
                        if (Request.QueryString["Value"].ToString().ToLower() == "company info")
                        {
                            string FolderIDs = operation.ExecuteScalar4Command(string.Format(@"select Folderid from tblfolder where foldername='company info' and deletestatus!=1"));
                            string TableNames = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Convert.ToInt32(FolderIDs)));
                            dtfiledetails1 = objCommonBAL.GetDispFileName(TableNames, Convert.ToInt32(FolderIDs), Convert.ToInt32(Session["UserID"]));
                            ((Control)Master.FindControl("ddlPageNumber")).Visible = false;
                        }
                }
                else
                {
                    dtfiledetails1 = objCommonBAL.GetDispFileName(TableName, Convert.ToInt32(Session["FolderID"]), Convert.ToInt32(Session["UserID"]));


                }
                if (dtfiledetails1 != null)
                {
                    if (dtfiledetails1.Rows.Count > 0)
                    {
                        if (Session["GroupName"].ToString().ToLower().Trim() == "board secretariat user" || Session["Designation"].ToString().ToLower() == "board secretary" || Session["GroupName"].ToString().ToLower() == "admin" || Session["Designation"].ToString().ToLower() == "board secretary" || Session["GroupName"].ToString().ToLower() == "admin" || Session["GroupName"].ToString().ToLower().Trim() == "president")
                        {
                            if (Symbol == "F")
                            {
                                btnDelete.Visible = true;
                                btnAuthorizeAll.Visible = true;
                                chkSelectalll.Visible = true;
                                btnMoveDown.Visible = true;
                                btnInvitee.Visible = true;
                                btnPublish.Visible = true;
                                btnFirstSeperator.Visible = true;
                                btnLastSeperator.Visible = true;
                                btnnotification.Visible = false;
                                btndownloadselected.Visible = true;
                                btnOCR.Visible = false;
                                tblUploadAgenda.Style.Add("display", "block");
                                tblUploadAgenda.Style.Add("background-color", "#eee9f0 none repeat scroll 0 0");


                                //btnwaterpwd.Visible = true;
                                //btnwaterwoutpwd.Visible = true;
                                //btnpwdwoutwater.Visible = true;
                                //btnwithoutPwdwater.Visible = true;

                            }
                            else
                                if (Symbol == "M")
                                {
                                    btnDelete.Visible = true;
                                    btnAuthorizeAll.Visible = false;
                                    chkSelectalll.Visible = true;
                                    btnExport.Visible = false;
                                    btnSMS.Visible = false;
                                    btnMoveDown.Visible = true;
                                    btnInvitee.Visible = false;
                                    btnPublish.Visible = false;
                                    btnFirstSeperator.Visible = true;
                                    btnLastSeperator.Visible = true;
                                    btnOCR.Visible = false;

                                }
                                else
                                    if (Symbol == "R")
                                    {
                                        btnDelete.Visible = false;
                                        btnAuthorizeAll.Visible = false;
                                        chkSelectalll.Visible = false;
                                        btnMoveDown.Visible = false;
                                        btnInvitee.Visible = false;
                                        btnPublish.Visible = false;
                                        btnFirstSeperator.Visible = false;
                                        btnLastSeperator.Visible = false;
                                        txtColor.Visible = false;
                                        lblRestricted.Visible = false;
                                    }
                        }
                        else
                            if (Session["GroupName"].ToString().ToLower().Trim() == "office secretary")
                            {
                                //btnDelete.Visible = true;
                                //btnAuthorizeAll.Visible = false;
                                //chkSelectalll.Visible = true;
                                //btnMoveDown.Visible = true;
                                //btnInvitee.Visible = true;
                                btnDelete.Visible = true;
                                btnAuthorizeAll.Visible = false;
                                chkSelectalll.Visible = true;
                                btnMoveDown.Visible = true;
                                btnInvitee.Visible = true;
                                btnPublish.Visible = true;
                                btnStartMOM.Visible = true;
                                btnnotebook.Visible = true;
                                btnComplete.Visible = true;
                                btnSubmitSelected.Visible = true;
                                btnPublish.Visible = false;
                                btnFirstSeperator.Visible = true;
                                btnLastSeperator.Visible = true;
                                btnOCR.Visible = false;
                                //btnwaterpwd.Visible = true;
                                //btnwaterwoutpwd.Visible = true;
                                //btnpwdwoutwater.Visible = true;
                                //btnwithoutPwdwater.Visible = true;

                                tblUploadAgenda.Style.Add("display", "block");
                                tblUploadAgenda.Style.Add("background-color", "#eee9f0 none repeat scroll 0 0");

                            }
                            else
                            {
                                string Value1 = operation.ExecuteScalar4Command(string.Format(@"select [Value] from tblConfig where keys='Note Book Selected for Director'", Session["UserID"]));
                                btnDelete.Visible = false;
                                btnAuthorizeAll.Visible = false;
                                if (Value1 == "yes")
                                {
                                    chkSelectalll.Visible = true;
                                }
                                else
                                {
                                    chkSelectalll.Visible = false;
                                }
                                btnMoveDown.Visible = false;
                                btnInvitee.Visible = false;
                                btnPublish.Visible = false;
                                btnFirstSeperator.Visible = false;
                                btnLastSeperator.Visible = false;
                                txtColor.Visible = false;
                                lblRestricted.Visible = false;

                            }
                        gvData.DataSource = null;
                        gvData.DataSource = dtfiledetails1;
                        gvData.DataBind();


                        //Added by Kirti

                        foreach (GridViewRow i in gvData.Rows)
                        {
                            //string TableNameFrom;
                            //DataTable dtattachment;
                            //string ReturnStr = "";
                            //string strPageCount = "";
                            //string strlastCount = "";
                            HtmlTableRow tr = (HtmlTableRow)i.FindControl("listviewRow");
                            Label approv = (Label)i.FindControl("lblApprovalstatus");
                            CheckBox chSelect = (CheckBox)i.FindControl("chkSelect");
                            LinkButton lnkApprove = (LinkButton)i.FindControl("LnkApprove");
                            LinkButton lnkDownload = (LinkButton)i.FindControl("lnkDownload");
                            //LinkButton lnkFileName = (LinkButton)i.FindControl("lnkFileName");
                            Label lnkFileName = (Label)i.FindControl("lnkFileName");
                            //Label FileSize = (Label)i.FindControl("lblFileSize");

                            Label itemno = (Label)i.FindControl("lnkFileName");
                            Label lotno = (Label)i.FindControl("lbllotno");
                            Label subject = (Label)i.FindControl("lblsubject");
                            Label lblwithdrowcomment = (Label)i.FindControl("lblWithdrawComments");
                            Label meetingdate = (Label)i.FindControl("lblmeettingdate");
                            LinkButton lnkEdit = (LinkButton)i.FindControl("lnkEdit");
                            LinkButton lnkOpen = (LinkButton)i.FindControl("lnkOpen");
                            LinkButton lnkOpen1 = (LinkButton)i.FindControl("lnkOpen1");
                            Label lblFileId = (Label)i.FindControl("lblFileId");
                            Label lblviewstaus = (Label)i.FindControl("lblviewstaus");
                            Label lblComment = (Label)i.FindControl("lblComment");
                            Label lblComment1 = (Label)i.FindControl("lblComment1");
                            Label lblComment2 = (Label)i.FindControl("lblComment2");
                            Label lblComment3 = (Label)i.FindControl("lblComment3");
                            Label lblComment4 = (Label)i.FindControl("lblComment4");
                            Label lblComment5 = (Label)i.FindControl("lblComment5");
                            Label lblFolderId = (Label)i.FindControl("lblFolderId");
                            Label lblllItemNo = (Label)i.FindControl("lblllItemNo");
                            Label lblPageNo = (Label)i.FindControl("lblPageNo");
                            Label lblFileName = (Label)i.FindControl("lblFileName");
                            LinkButton LnkReject = (LinkButton)i.FindControl("LnkReject");
                            Label lblstatus = (Label)i.FindControl("lblstatus");
                            ImageButton Imgatach = (ImageButton)i.FindControl("Imgatach");
                            ImageButton ImgReplaceNote = (ImageButton)i.FindControl("ImgReplaceNote");
                            ImageButton imgbtnaccessRes = (ImageButton)i.FindControl("imgbtnaccessRes");
                            // Label lblwithdrowcomment = (Label)i.FindControl("lblwithdrowcomment");
                            foreach (var inputChar in lblllItemNo.Text)
                            {

                                if (Char.IsLetter(inputChar))
                                {
                                    Imgatach.Visible = false;
                                    ImgReplaceNote.Visible = false;
                                    imgbtnaccessRes.Visible = false;
                                }
                            }


                            string withdraw = lblwithdrowcomment.Text;

                            //DropDownList ddlComment = (DropDownList)i.FindControl("ddlComment");

                            //TableNameFrom = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Session["FolderID"]));
                            //strPageCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and b.column0<'{2}'", TableNameFrom, lblFolderId.Text.ToString(), lblllItemNo.Text.ToString()));
                            //if (Convert.ToInt32(lblllItemNo.Text.ToString()) > 008)
                            //{
                            //    strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and b.column0<'{2}'", TableNameFrom, lblFolderId.Text.ToString(), "0" + (Convert.ToInt32(lblllItemNo.Text.ToString()) + 1)));
                            //}
                            //else
                            //{
                            //    strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and b.column0<'{2}'", TableNameFrom, lblFolderId.Text.ToString(), "00" + (Convert.ToInt32(lblllItemNo.Text.ToString()) + 1)));

                            //}
                            //if (strPageCount == "")
                            //{
                            //    strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and b.column0<'{2}'", TableNameFrom, lblFolderId.Text.ToString(), "00" + (Convert.ToInt32(lblllItemNo.Text.ToString()) + 1)));
                            //    lblPageNo.Text = "1" + " - " + strlastCount;
                            //}
                            //else
                            //{
                            //    lblPageNo.Text = (Convert.ToInt32(strPageCount) + 1) + " - " + strlastCount;
                            //}
                            //DataTable pageno1 = operation.GetTable4Command(string.Format(@"select distinct pageno from tblfreedraw where fileid='{0}' and userid='{1}'", lblFileId.Text, Session["UserID"].ToString()));
                            //DataTable pageno = operation.GetTable4Command(string.Format(@"select distinct pageno from tblComments where fileid='{0}' and Commentby_id='{1}'", lblFileId.Text, Session["UserID"].ToString()));
                            //DataTable pageno2 = operation.GetTable4Command(string.Format(@"select distinct pageno from tblhighliter where fileid='{0}' and userid='{1}'", lblFileId.Text, Session["UserID"].ToString()));
                            //if (pageno.Rows.Count > 0)
                            //{
                            //    for (int j = 0; j < pageno.Rows.Count; j++)
                            //    {
                            //        if (strPageCount == "")
                            //        {
                            //            ReturnStr += Convert.ToInt32(0) + Convert.ToInt32(pageno.Rows[j]["pageno"].ToString()) + ",";
                            //        }
                            //        else
                            //        {
                            //            ReturnStr += Convert.ToInt32(strPageCount) + Convert.ToInt32(pageno.Rows[j]["pageno"].ToString()) + ",";
                            //        }
                            //    }
                            //}

                            //dtattachment = operation.GetTable4Command(string.Format(@"select a.fileid,a.Folderid,b.attachmentid,c.Column0 from tblfile a inner join tblattachment b on a.fileid=b.fileid inner join {0} c on b.attachmentid=c.fileid where a.fileid='{1}'", TableNameFrom, lblFileId.Text));
                            //for (int s = 0; s < dtattachment.Rows.Count; s++)
                            //{
                            //    string strCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and b.column0<'{2}'", TableNameFrom, dtattachment.Rows[s]["FolderID"].ToString(), dtattachment.Rows[s]["Column0"].ToString()));
                            //    DataTable dtattachmentpageno = operation.GetTable4Command(string.Format(@"select distinct pageno from tblcomments where fileid='{0}' and Commentby_id='{1}'", dtattachment.Rows[s]["attachmentid"].ToString(), Session["UserID"].ToString()));
                            //    for (int c = 0; c < dtattachmentpageno.Rows.Count; c++)
                            //    {
                            //        ReturnStr += Convert.ToInt32(strCount) + Convert.ToInt32(dtattachmentpageno.Rows[c]["pageno"].ToString()) + ",";
                            //    }

                            //}

                            //if (pageno1.Rows.Count > 0)
                            //{
                            //    for (int j = 0; j < pageno1.Rows.Count; j++)
                            //    {
                            //        if (strPageCount == "")
                            //        {
                            //            ReturnStr += Convert.ToInt32(0) + Convert.ToInt32(pageno1.Rows[j]["pageno"].ToString()) + ",";
                            //        }
                            //        else
                            //        {
                            //            ReturnStr += Convert.ToInt32(strPageCount) + Convert.ToInt32(pageno1.Rows[j]["pageno"].ToString()) + ",";
                            //        }

                            //    }
                            //}

                            //dtattachment = operation.GetTable4Command(string.Format(@"select a.fileid,a.Folderid,b.attachmentid,c.Column0 from tblfile a inner join tblattachment b on a.fileid=b.fileid inner join {0} c on b.attachmentid=c.fileid where a.fileid='{1}'", TableNameFrom, lblFileId.Text));
                            //for (int s = 0; s < dtattachment.Rows.Count; s++)
                            //{
                            //    string strCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and b.column0<'{2}'", TableNameFrom, dtattachment.Rows[s]["FolderID"].ToString(), dtattachment.Rows[s]["Column0"].ToString()));
                            //    DataTable dtattachmentpageno = operation.GetTable4Command(string.Format(@"select distinct pageno from tblfreedraw where fileid='{0}' and userid='{1}'", dtattachment.Rows[s]["attachmentid"].ToString(), Session["UserID"].ToString()));
                            //    for (int c = 0; c < dtattachmentpageno.Rows.Count; c++)
                            //    {
                            //        ReturnStr += Convert.ToInt32(strCount) + Convert.ToInt32(dtattachmentpageno.Rows[c]["pageno"].ToString()) + ",";
                            //    }

                            //}

                            //if (pageno2.Rows.Count > 0)
                            //{
                            //    for (int j = 0; j < pageno2.Rows.Count; j++)
                            //    {
                            //        if (strPageCount == "")
                            //        {
                            //            ReturnStr += Convert.ToInt32(0) + Convert.ToInt32(pageno2.Rows[j]["pageno"].ToString()) + ",";
                            //        }
                            //        else
                            //        {
                            //            ReturnStr += Convert.ToInt32(strPageCount) + Convert.ToInt32(pageno2.Rows[j]["pageno"].ToString()) + ",";
                            //        }
                            //        //ReturnStr += Convert.ToInt32(strPageCount) + pageno2.Rows[j]["pageno"].ToString() + ",";
                            //    }
                            //}
                            ////TableNameFrom = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Session["FolderID"]));
                            //dtattachment = operation.GetTable4Command(string.Format(@"select a.fileid,a.Folderid,b.attachmentid,c.Column0 from tblfile a inner join tblattachment b on a.fileid=b.fileid inner join {0} c on b.attachmentid=c.fileid where a.fileid='{1}'", TableNameFrom, lblFileId.Text));
                            //for (int s = 0; s < dtattachment.Rows.Count; s++)
                            //{
                            //    string strCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and b.column0<'{2}'", TableNameFrom, dtattachment.Rows[s]["FolderID"].ToString(), dtattachment.Rows[s]["Column0"].ToString()));
                            //    DataTable dtattachmentpageno = operation.GetTable4Command(string.Format(@"select distinct pageno from tblhighliter where fileid='{0}' and userid='{1}'", dtattachment.Rows[s]["attachmentid"].ToString(), Session["UserID"].ToString()));
                            //    for (int c = 0; c < dtattachmentpageno.Rows.Count; c++)
                            //    {
                            //        ReturnStr += Convert.ToInt32(strCount) + Convert.ToInt32(dtattachmentpageno.Rows[c]["pageno"].ToString()) + ",";
                            //    }

                            //}

                            //string temp = string.Join(",", ReturnStr.Split(',').Distinct().ToArray());
                            //if (temp != "")
                            //{
                            //    string strarray = "";
                            //    IEnumerable<int> a = StringToIntList(temp);
                            //    int[] b = a.ToArray();
                            //    Array.Sort(b);
                            //    for (int d = 0; d < b.Count(); d++)
                            //    {
                            //        strarray += b[d] + ",";
                            //    }
                            //    if (b.Length != 0)
                            //    {
                            //        ddlComment.DataSource = b;
                            //        ddlComment.DataBind();
                            //        ddlComment.Items.Insert(0, new ListItem("-No-", "0"));
                            //    }

                            //    subject.Text = strarray.Remove(strarray.Length - 1, 1);
                            //}
                            //else
                            //{

                            //    ddlComment.Items.Insert(0, new ListItem("-No-", "0"));
                            //}


                            if (Session["GroupName"].ToString().ToLower().Trim() == "board secretariat user" && Session["Designation"].ToString().ToLower() != "board secretary" || Session["Designation"].ToString().ToLower() == "board secretary" || Session["GroupName"].ToString().ToLower() == "admin" || Session["GroupName"].ToString().ToLower().Trim() == "president")
                            {

                                if (Symbol == "F")
                                {
                                    if (lblstatus.Text.ToLower() == "3" || lblstatus.Text.ToLower() == "2")
                                    {
                                        LnkReject.Visible = false;
                                    }
                                    else
                                        if (lblstatus.Text.ToLower() != "0")
                                        {
                                            LnkReject.Visible = true;
                                        }
                                        else
                                        {
                                            LnkReject.Visible = false;
                                        }



                                    if (lnkOpen.Text.ToLower() == "add document")
                                    {
                                        //lnkOpen1.Enabled = false;
                                        lblviewstaus.Text = "";
                                    }
                                    if (lnkEdit != null)
                                    {

                                        if (approv.Text == "0" && lblFileName.Text != "")
                                        {
                                            lnkEdit.Visible = true;
                                        }
                                        else
                                        {
                                            lnkEdit.Enabled = true;
                                        }
                                        if (approv.Text == "1")
                                        {

                                            lnkApprove.Visible = true;
                                            lnkApprove.Text = "Withdraw";
                                            lnkEdit.Enabled = true;
                                            btnExport.Visible = true;
                                            btnSMS.Visible = true;
                                            btnStartMOM.Visible = false;
                                            if (lblstatus.Text == "10")
                                            {
                                                LnkReject.Visible = true;
                                            }
                                            else
                                            {
                                                LnkReject.Visible = false;
                                            }
                                            //LnkReject.Visible = false;
                                        }

                                        if (approv.Text == "3")
                                        {

                                            lnkApprove.Visible = true;
                                            lnkApprove.Text = "P-Withdraw";
                                            lnkEdit.Enabled = true;
                                            btnExport.Visible = true;
                                            btnSMS.Visible = true;
                                            btnStartMOM.Visible = false;
                                            LnkReject.Visible = false;

                                        }
                                        if (approv.Text == "5" && lblstatus.Text.ToLower() == "10" && lblwithdrowcomment.Text != "")
                                        {

                                            lnkApprove.Text = "rejected";
                                            lnkApprove.Enabled = false;
                                            LnkReject.Visible = false;
                                        }

                                        if (approv.Text == "2")
                                        {
                                            lnkApprove.Enabled = false;
                                            lnkApprove.Text = "Withdrawn";
                                            lnkApprove.Visible = true;
                                            lnkEdit.Enabled = false;
                                            LnkReject.Visible = false;

                                        }
                                        if (approv.Text == "5" && lblstatus.Text.ToLower() == "10" && lblwithdrowcomment.Text == "")
                                        {

                                            i.Visible = false;
                                            lnkApprove.Enabled = false;
                                            LnkReject.Visible = false;
                                        }

                                    }
                                    if (Session["FolderName"].ToString().ToLower() == "meetings\\my briefcase")
                                    {
                                        tblUploadAgenda.Visible = false;
                                        btnAddNew.Visible = true;
                                        btnDelete.Visible = true;
                                        btnSwap.Visible = true;


                                    }
                                    if (Session["FolderName"].ToString().ToLower() == "meetings\\company info" || Session["FolderName"].ToString().ToLower() == "meetings\\shared documents")
                                    {
                                        tblUploadAgenda.Visible = false;
                                        btnAddNew.Visible = true;
                                        btnDelete.Visible = true;
                                        btnSwap.Visible = true;


                                    }


                                }
                                else
                                    if (Symbol == "R")
                                    {
                                        if (lnkOpen.Text.ToLower() == "add document")
                                        {
                                            //lnkOpen.Text = "";
                                            // lnkOpen1.Enabled = false;
                                            lblviewstaus.Text = "";
                                        }
                                        if (approv.Text == "1")
                                        {
                                            i.Visible = true;
                                        }
                                        else if (approv.Text == "0")
                                        {
                                            i.Visible = false;
                                        }
                                    }
                                    else
                                        if (Symbol == "M")
                                        {

                                            if (lnkOpen.Text.ToLower() == "add document")
                                            {
                                                //lnkOpen.Text = "";
                                                // lnkOpen1.Enabled = false;
                                                lblviewstaus.Text = "";
                                            }

                                        }
                            }

                            if (Session["Designation"].ToString().ToLower() == "board secretary")
                            {

                                if (lnkEdit != null)
                                {

                                    if (approv.Text == "0" && lblFileName.Text != "")
                                    {
                                        lnkEdit.Visible = true;
                                    }
                                    else
                                    {
                                        lnkEdit.Enabled = false;
                                    }
                                    if (approv.Text == "1")
                                    {

                                        lnkApprove.Visible = true;
                                        lnkApprove.Text = "Withdraw";
                                        lnkEdit.Enabled = true;
                                        btnExport.Visible = true;
                                        btnSMS.Visible = true;
                                        btnStartMOM.Visible = false;
                                    }
                                    if (approv.Text == "2")
                                    {
                                        lnkApprove.Enabled = false;
                                        lnkApprove.Text = "Withdrawn";
                                        lnkApprove.Visible = true;
                                        lnkEdit.Enabled = false;

                                    }
                                }


                            }
                            else if (Session["GroupName"].ToString().ToLower() == "directors" || Convert.ToString(Session["Groupname"]).ToLower() == "general manager" || Session["GroupName"].ToString().ToLower() == "permanent invitees" || Session["GroupName"].ToString().ToLower() == "cfo" || Convert.ToString(Session["Groupname"]).ToLower() == "senior management" || Convert.ToString(Session["Groupname"]).ToLower() == "functional management" || Convert.ToString(Session["Groupname"]).ToLower() == "others")
                            {
                                
                                //chSelect.Visible = true;
                                //lnkApprove.Visible = false;
                                //lnkOpen.Visible = true;
                                //lnkDownload.Visible = true;
                                string Value = operation.ExecuteScalar4Command(string.Format(@"select [Value] from tblConfig where keys='Note Book for Director'", lblFileId.Text, Session["UserID"]));
                                string Value1 = operation.ExecuteScalar4Command(string.Format(@"select [Value] from tblConfig where keys='Note Book Selected for Director'", lblFileId.Text, Session["UserID"]));
                                if (Symbol == "F")
                                {
                                    if (dtfiledetails1.Rows.Count > 0)
                                    {
                                        if (Session["FolderName"].ToString().ToLower() == "meetings\\my briefcase")
                                        {
                                            btnDelete.Visible = true;
                                            btnAddNew.Visible = true;

                                        }
                                        else if (Session["FolderName"].ToString().ToLower() == "meetings\\company info" || Session["FolderName"].ToString().ToLower() == "meetings\\shared documents")
                                            {
                                                btnDelete.Visible = false;
                                                btnAddNew.Visible = false;
                                            }
                                            else 
                                            if (Value == "yes")
                                            {
                                                btnnotebook.Visible = true;//For Director's Note book
                                                 if (Value1 == "yes")
                                            {
                                                btndownloadselected.Visible = true;

                                            }
                                            else if (Value1 == "no")
                                            {
                                                btndownloadselected.Visible = false;
                                            }
                                            }
                                            else if (Value == "no")
                                            {
                                                btnnotebook.Visible = false;//For Director's No Note book
                                                if (Value1 == "yes")
                                            {
                                                btndownloadselected.Visible = true;

                                            }
                                            else if (Value1 == "no")
                                            {
                                                btndownloadselected.Visible = false;
                                            }
                                            }
                                            

                                    }
                                     


                                }
                                //if (Symbol == "M")
                                //{
                                //    btnnotebook.Visible = false;
                                //}

                                if (lnkOpen.Text.ToLower() == "add document")
                                {
                                    //lnkOpen.Text = "";
                                    // lnkOpen1.Enabled = false;
                                    lblviewstaus.Text = "";
                                }

                                if (approv.Text == "3")
                                {
                                    //string strAccess = operation.ExecuteScalar4Command(string.Format(@"select Access from tblAgendalevelAccessControl where fileid='{0}' and userid='{1}'", lblFileId.Text, Session["UserID"]));
                                    //if (strAccess.ToLower() == "true")
                                    //{
                                    //    i.Visible = true;
                                    //}
                                    //else
                                    //{
                                    //    i.Visible = true;
                                    //}
                                    //i.Visible = true;
                                    string accesskey = operation.ExecuteScalar4Command(string.Format(@"select value from   tblconfig where keys='AgendaAccessRestricted'"));
                                    string accessvalue = operation.ExecuteScalar4Command(string.Format(@"select * from   tblAgendalevelAccessControl where fileid='{0}' and userid='{1}'", lblFileId.Text, Session["UserID"]));
                                    if (accessvalue != "")
                                    {
                                        if (accesskey.ToLower() == "yes")
                                        {
                                            i.Visible = true;

                                        }
                                        else
                                        {
                                            i.Visible = false;
                                        }
                                    }
                                    else
                                    {
                                        i.Visible = true;
                                    }

                                }
                                else if (approv.Text == "2")
                                {
                                    i.Visible = true;
                                }
                                else
                                {
                                    if (Session["FolderName"].ToString().ToLower() == "meetings\\my briefcase")
                                    {
                                        i.Visible = true;
                                    }
                                    else
                                        if (Session["FolderName"].ToString().ToLower() == "meetings\\company info" || Session["FolderName"].ToString().ToLower() == "meetings\\shared documents")
                                        {
                                            i.Visible = true;
                                        }
                                        else
                                            if (Request.QueryString["Value"] != null && Request.QueryString["Value"] != "meeting")
                                            {
                                                if (Request.QueryString["Value"].ToString().ToLower() == "my briefcase")
                                                {
                                                    i.Visible = true;
                                                    btnDelete.Visible = true;
                                                    btnnotebook.Visible = false;
                                                    tblUploadAgenda.Visible = false;
                                                    btnAddNew.Visible = true;
                                                    btnSwap.Visible = true;


                                                }
                                                else
                                                    if (Request.QueryString["Value"].ToString().ToLower() == "company info")
                                                    {
                                                        i.Visible = true;
                                                        btnDelete.Visible = false;
                                                        btnnotebook.Visible = false;
                                                        tblUploadAgenda.Visible = false;



                                                    }
                                                    else
                                                    {
                                                        i.Visible = false;
                                                    }
                                            }
                                            else
                                            {
                                                i.Visible = false;
                                            }

                                }
                                //else
                                //    if (approv.Text == "3")
                                //    {
                                //        string strAccess = operation.ExecuteScalar4Command(string.Format(@"select Access from tblAgendalevelAccessControl where fileid='{0}' and userid='{1}'", lblFileId.Text, Session["UserID"]));
                                //        if (strAccess.ToLower() == "true")
                                //        {
                                //            i.Visible = true;

                                //        }
                                //        else
                                //        {
                                //            i.Visible = true;
                                //        }

                                //    }
                                //    else if (approv.Text == "0")
                                //    {
                                //        if (Session["FolderName"].ToString().ToLower() == "meetings\\my briefcase" || Session["FolderName"].ToString().ToLower() == "meetings\\company info")
                                //        {
                                //            i.Visible = true;
                                //        }
                                //        else
                                //        {
                                //            i.Visible = false;
                                //        }

                                //    }
                                //    else
                                //if (approv.Text == "5")
                                //{
                                //    string strAccess = operation.ExecuteScalar4Command(string.Format(@"select Access from tblAgendalevelAccessControl where fileid='{0}' and userid='{1}'", lblFileId.Text, Session["UserID"]));
                                //    if (strAccess.ToLower() == "true")
                                //    {
                                //        i.Visible = true;

                                //    }
                                //    else
                                //    {
                                //        i.Visible = true;
                                //    }

                                //}


                            }
                            else if (Session["GroupName"].ToString().ToLower() == "office secretary")
                            {

                                if (Session["FolderName"].ToString().ToLower() == "meetings\\my briefcase")
                                {
                                    tblUploadAgenda.Visible = false;
                                    btnAddNew.Visible = true;
                                }
                                if (Session["FolderName"].ToString().ToLower() == "meetings\\company info" || Session["FolderName"].ToString().ToLower() == "meetings\\shared documents")
                                {
                                    tblUploadAgenda.Visible = false;
                                    btnAddNew.Visible = true;
                                }
                                if ((approv.Text == "0" && lblstatus.Text == "10"))
                                {

                                    lnkApprove.Text = "Submitted";


                                    LnkReject.Visible = false;
                                }
                                else
                                    if ((approv.Text == "1" && lblstatus.Text == "3"))
                                    {

                                        lnkApprove.Text = "Authorized";
                                        lnkApprove.Enabled = false;

                                        LnkReject.Visible = false;
                                    }
                                    else
                                    {
                                        if (withdraw != "")
                                        {
                                            lnkApprove.Text = "Resubmit";
                                            LnkReject.Visible = false;
                                        }
                                        else
                                            if ((approv.Text == "0" && lblstatus.Text == "0") && (Session["FolderName"].ToString().ToLower() == "meetings\\my briefcase" || Session["FolderName"].ToString().ToLower() == "meetings\\company info" || Session["FolderName"].ToString().ToLower() == "meetings\\shared documents"))
                                            {
                                                i.Visible = true;
                                            }
                                            else
                                                if ((approv.Text == "0" && lblstatus.Text != "10"))
                                                {
                                                    i.Visible = true;
                                                    LnkReject.Visible = false;
                                                    lnkApprove.Enabled = false;
                                                }
                                                else
                                                    if ((approv.Text == "1" && lblstatus.Text == "0"))
                                                    {
                                                        i.Visible = true;
                                                        LnkReject.Visible = false;
                                                        lnkApprove.Enabled = false;
                                                        lnkApprove.Text = "Authorized";
                                                    }
                                                    else
                                                        if ((approv.Text == "2" && lblstatus.Text == "0"))
                                                        {
                                                            i.Visible = true;
                                                            lnkApprove.Text = "Withdrawn";
                                                            lnkApprove.Enabled = false;
                                                            LnkReject.Visible = false;
                                                            // i.Visible = false;
                                                        }
                                                        else
                                                            if ((approv.Text == "3" && lblstatus.Text == "0"))
                                                            {
                                                                i.Visible = true;
                                                                LnkReject.Visible = false;
                                                                lnkApprove.Enabled = false;
                                                                lnkApprove.Text = "Published";

                                                            }
                                                            else
                                                                if ((approv.Text == "1" && lblstatus.Text == "10"))
                                                                {
                                                                    lnkApprove.Text = "Authorized";
                                                                    LnkReject.Visible = false;
                                                                    lnkApprove.Enabled = false;
                                                                }
                                                                else
                                                                    if ((approv.Text == "2" && lblstatus.Text == "10"))
                                                                    {
                                                                        lnkApprove.Text = "Submitted";
                                                                        LnkReject.Visible = false;
                                                                    }
                                                                    else
                                                                        if ((approv.Text == "3" && lblstatus.Text == "10"))
                                                                        {
                                                                            //lnkApprove.Text = "Submitted";
                                                                            //LnkReject.Visible = false;
                                                                            lnkApprove.Text = "Published";
                                                                            lnkApprove.Enabled = false;
                                                                            LnkReject.Visible = false;
                                                                        }
                                                                        else
                                                                            if ((approv.Text == "3" && lblstatus.Text == "3"))
                                                                            {
                                                                                //lnkApprove.Text = "Submitted";
                                                                                //LnkReject.Visible = false;
                                                                                lnkApprove.Text = "Published";
                                                                                lnkApprove.Enabled = false;
                                                                                LnkReject.Visible = false;
                                                                            }
                                                                            else
                                                                            {
                                                                                lnkApprove.Text = "Submit";
                                                                                LnkReject.Visible = false;
                                                                            }
                                    }



                            }
                            else
                            {


                                if (Session["GroupName"].ToString().ToLower() == "admin")
                                {
                                    lnkEdit.Visible = true;

                                    chSelect.Checked = false;
                                    lnkOpen.Visible = true;
                                    if (approv.Text != "1")
                                    {


                                    }
                                    if (approv.Text == "0")
                                    {
                                        lnkApprove.Text = "Reject";
                                        lnkApprove.Visible = true;
                                        btnExport.Visible = false;
                                        btnSMS.Visible = false;
                                        btnStartMOM.Visible = false;
                                    }
                                    if (approv.Text == "1")
                                    {

                                        lnkApprove.Visible = true;
                                        lnkApprove.Text = "Withdraw";
                                        lnkEdit.Enabled = true;
                                        btnExport.Visible = true;
                                        btnSMS.Visible = true;
                                        btnStartMOM.Visible = false;
                                    }
                                    if (approv.Text == "2")
                                    {
                                        lnkApprove.Enabled = false;
                                        lnkApprove.Text = "Withdrawn";
                                        lnkApprove.Visible = true;
                                        lnkEdit.Enabled = false;

                                    }

                                }

                            }



                        }
                        lblNoMeeting.Visible = false;
                        string Seperator = operation.ExecuteScalar4Command(string.Format(@"select Value from tblConfig where keys='seperator'"));
                        if (Session["GroupName"].ToString().ToLower().Trim() == "board secretariat user" && Session["Designation"].ToString().ToLower() != "board secretary" || Session["Designation"].ToString().ToLower() == "board secretary" || Session["GroupName"].ToString().ToLower() == "admin" || Session["GroupName"].ToString().ToLower().Trim() == "president")
                        {
                            if (Session["FolderName"].ToString().ToLower() == "meetings\\my briefcase" || Session["FolderName"].ToString().ToLower() == "meetings\\company info" || Session["FolderName"].ToString().ToLower() == "meetings\\shared documents")
                            {

                                //gvData.Columns[1].Visible = false;
                                //gvData.Columns[3].Visible = false;
                                //gvData.Columns[5].Visible = false;
                                //gvData.Columns[6].Visible = false;
                                //gvData.Columns[7].Visible = false;
                                //gvData.Columns[8].Visible = false;
                                //gvData.Columns[10].Visible = false;
                                //gvData.Columns[11].Visible = false;
                                //gvData.Columns[13].Visible = false;
                                gvData.Columns[1].Visible = false;
                                //gvData.Columns[3].Visible = false;
                                gvData.Columns[2].Visible = false;
                                gvData.Columns[5].Visible = false;
                                gvData.Columns[6].Visible = true;
                                gvData.Columns[7].Visible = false;
                                gvData.Columns[8].Visible = false;
                                gvData.Columns[9].Visible = false;
                                //gvData.Columns[10].Visible = false;
                                gvData.Columns[12].Visible = false;
                                gvData.Columns[11].Visible = false;
                                //gvData.Columns[13].Visible = false;
                                gvData.Columns[14].Visible = true;
                                gvData.Columns[15].Visible = false;
                                gvData.Columns[16].Visible = false;
                            }
                            else
                            {

                                if (Seperator.ToLower() == "yes")
                                {
                                    gvData.Columns[1].Visible = false;
                                }
                                else
                                {
                                    gvData.Columns[1].Visible = true;
                                }
                                if (Symbol == "R")
                                {


                                    gvData.Columns[0].Visible = false;
                                    gvData.Columns[8].Visible = false;///for View button
                                    gvData.Columns[9].Visible = false;
                                    gvData.Columns[10].Visible = false;
                                    gvData.Columns[11].Visible = false;
                                    gvData.Columns[12].Visible = false;


                                }
                                else
                                    if (Symbol == "M")
                                    {

                                        gvData.Columns[3].Visible = false;
                                        gvData.Columns[7].Visible = false;///for View button
                                        gvData.Columns[5].Visible = false;
                                        gvData.Columns[6].Visible = false;
                                        gvData.Columns[10].Visible = false;
                                        gvData.Columns[11].Visible = false;

                                    }
                            }
                        }
                        if (Convert.ToString(Session["Groupname"]).ToLower() == "office secretary")
                        {
                            if (Session["FolderName"].ToString().ToLower() == "meetings\\my briefcase" || Session["FolderName"].ToString().ToLower() == "meetings\\company info" || Session["FolderName"].ToString().ToLower() == "meetings\\shared documents")
                            {

                                //gvData.Columns[1].Visible = false;
                                //gvData.Columns[3].Visible = false;
                                //gvData.Columns[5].Visible = false;
                                //gvData.Columns[6].Visible = false;
                                //gvData.Columns[7].Visible = false;
                                //gvData.Columns[8].Visible = false;
                                //gvData.Columns[10].Visible = false;
                                //gvData.Columns[11].Visible = false;
                                //gvData.Columns[13].Visible = false;
                                gvData.Columns[1].Visible = false;
                                //gvData.Columns[3].Visible = false;
                                gvData.Columns[2].Visible = false;
                                gvData.Columns[5].Visible = false;
                                gvData.Columns[6].Visible = true;
                                gvData.Columns[7].Visible = false;
                                gvData.Columns[8].Visible = false;
                                gvData.Columns[9].Visible = false;
                                //gvData.Columns[10].Visible = false;
                                gvData.Columns[12].Visible = false;
                                gvData.Columns[11].Visible = false;
                                //gvData.Columns[13].Visible = false;
                                gvData.Columns[14].Visible = false;
                                gvData.Columns[15].Visible = false;
                                gvData.Columns[16].Visible = false;
                            }
                            else
                            {

                                if (Seperator.ToLower() == "yes")
                                {
                                    gvData.Columns[1].Visible = false;
                                }
                                else
                                {
                                    gvData.Columns[1].Visible = true;
                                }

                            }

                        }
                        if (Session["GroupName"].ToString().ToLower() == "directors" || Session["GroupName"].ToString().ToLower() == "permanent invitees" || Session["GroupName"].ToString().ToLower() == "cfo" || Convert.ToString(Session["Groupname"]).ToLower() == "senior management" || Convert.ToString(Session["Groupname"]).ToLower() == "functional management" || Convert.ToString(Session["Groupname"]).ToLower() == "others")
                        {

                            if (Session["FolderName"].ToString().ToLower() == "meetings\\my briefcase" || Session["FolderName"].ToString().ToLower() == "meetings\\company info" || Session["FolderName"].ToString().ToLower() == "meetings\\shared documents")
                            {

                                gvData.Columns[1].Visible = false;
                                //gvData.Columns[3].Visible = false;
                                gvData.Columns[2].Visible = false;
                                gvData.Columns[5].Visible = false;
                                gvData.Columns[6].Visible = true;
                                gvData.Columns[7].Visible = false;
                                gvData.Columns[8].Visible = false;
                                gvData.Columns[9].Visible = false;
                                //gvData.Columns[10].Visible = false;
                                gvData.Columns[12].Visible = false;
                                gvData.Columns[11].Visible = false;
                                gvData.Columns[13].Visible = false;

                                //gvData.Columns[13].Visible = false;
                                gvData.Columns[14].Visible = false;
                                gvData.Columns[15].Visible = false;
                                gvData.Columns[16].Visible = false;
                            }
                            else
                                if (Request.QueryString["Value"] != null && Request.QueryString["Value"] != "meeting")
                                {
                                    if (Request.QueryString["Value"].ToString().ToLower() == "my briefcase")
                                    {

                                        gvData.Columns[1].Visible = false;
                                        gvData.Columns[2].Visible = false;
                                        gvData.Columns[3].Visible = true;
                                        gvData.Columns[4].Visible = false;
                                        //gvData.Columns[7].Visible = false;
                                        gvData.Columns[5].Visible = false;
                                        gvData.Columns[6].Visible = true;
                                        gvData.Columns[7].Visible = false;
                                        gvData.Columns[8].Visible = false;
                                        gvData.Columns[9].Visible = false;
                                        gvData.Columns[10].Visible = true;
                                        gvData.Columns[11].Visible = false;
                                        gvData.Columns[12].Visible = false;
                                        gvData.Columns[13].Visible = true;
                                        gvData.Columns[14].Visible = false;
                                        gvData.Columns[15].Visible = false;
                                        gvData.Columns[16].Visible = false;
                                    }
                                    else
                                        if (Request.QueryString["Value"].ToString().ToLower() == "company info")
                                        {

                                            gvData.Columns[1].Visible = false;
                                            gvData.Columns[2].Visible = false;
                                            gvData.Columns[3].Visible = true;
                                            gvData.Columns[4].Visible = false;
                                            //gvData.Columns[7].Visible = false;
                                            gvData.Columns[5].Visible = false;
                                            gvData.Columns[6].Visible = true;
                                            gvData.Columns[7].Visible = false;
                                            gvData.Columns[8].Visible = false;
                                            gvData.Columns[9].Visible = false;
                                            gvData.Columns[10].Visible = true;
                                            gvData.Columns[11].Visible = false;
                                            gvData.Columns[12].Visible = false;
                                            gvData.Columns[13].Visible = true;
                                            gvData.Columns[14].Visible = false;
                                            gvData.Columns[15].Visible = false;
                                            gvData.Columns[16].Visible = false;
                                        }
                                        else
                                        {

                                            if (Seperator.ToLower() == "yes")
                                            {
                                                gvData.Columns[1].Visible = false;
                                            }
                                            else
                                            {
                                                gvData.Columns[1].Visible = true;
                                            }
                                            gvData.Columns[0].Visible = false;
                                            //gvData.Columns[8].Visible = false;///for View button
                                            //gvData.Columns[9].Visible = false;
                                            // gvData.Columns[10].Visible = false;
                                            gvData.Columns[11].Visible = false;
                                            gvData.Columns[12].Visible = false;
                                            gvData.Columns[13].Visible = false;
                                            gvData.Columns[14].Visible = false;
                                            gvData.Columns[15].Visible = false;

                                        }
                                }
                                else
                                {
                                    string Value1 = operation.ExecuteScalar4Command(string.Format(@"select [Value] from tblConfig where keys='Note Book Selected for Director'", Session["UserID"]));
                                    if (Seperator.ToLower() == "yes")
                                    {
                                        gvData.Columns[1].Visible = false;
                                        if (Value1 == "yes")
                                        {
                                            gvData.Columns[0].Visible = true;
                                        }
                                        else
                                        {
                                            gvData.Columns[0].Visible = false;
                                        }
                                    }
                                    else
                                    {
                                        gvData.Columns[1].Visible = true;
                                        if (Value1 == "yes")
                                    {
                                        gvData.Columns[0].Visible = true;
                                    }
                                    else
                                    {
                                        gvData.Columns[0].Visible = false;
                                    }
                                    }
                                    
                                    //gvData.Columns[8].Visible = false;
                                    gvData.Columns[9].Visible = false;
                                    gvData.Columns[10].Visible = false;
                                    gvData.Columns[11].Visible = false;
                                    gvData.Columns[12].Visible = false;
                                    gvData.Columns[13].Visible = false;
                                    gvData.Columns[14].Visible = false;
                                    gvData.Columns[15].Visible = false;
                                    gvData.Columns[16].Visible = false;
                                }


                        }

                        if (Convert.ToString(Session["Groupname"]).ToLower() == "general manager")
                        {
                            gvData.Columns[0].Visible = false;
                            gvData.Columns[8].Visible = false;
                        }


                    }
                    else
                    {
                        btnExport.Visible = false;
                        btnSMS.Visible = false;
                        if (Session["FolderName"].ToString().ToLower() == "meetings\\my briefcase")
                        {
                            tblUploadAgenda.Visible = false;
                            btnAddNew.Visible = true;
                            gvData.EmptyDataText = "No Documents";
                            ((Control)Master.FindControl("liAddMOM")).Visible = false;
                        }
                        else
                            if (Session["FolderName"].ToString().ToLower() == "meetings\\company info" || Session["FolderName"].ToString().ToLower() == "meetings\\shared documents")
                            {
                                if (Session["GroupName"].ToString().ToLower() == "directors" || Session["GroupName"].ToString().ToLower() == "permanent invitees" || Session["GroupName"].ToString().ToLower() == "cfo" || Convert.ToString(Session["Groupname"]).ToLower() == "senior management" || Convert.ToString(Session["Groupname"]).ToLower() == "functional management" || Convert.ToString(Session["Groupname"]).ToLower() == "others")
                                {
                                    tblUploadAgenda.Visible = false;
                                    btnAddNew.Visible = false;
                                    gvData.EmptyDataText = "No Documents";
                                }
                                else
                                {
                                    tblUploadAgenda.Visible = false;
                                    btnAddNew.Visible = true;
                                    gvData.EmptyDataText = "No Documents";
                                }

                            }
                            else
                                if (Request.QueryString["Value"] != null)
                                {
                                    if (Request.QueryString["Value"].ToString().ToLower() == "my briefcase")
                                    {
                                        btnAddNew.Visible = true;
                                    }
                                }
                                else
                                {
                                    if (Session["GroupName"].ToString().ToLower() == "directors" || Session["GroupName"].ToString().ToLower() == "permanent invitees" || Session["GroupName"].ToString().ToLower() == "cfo" || Convert.ToString(Session["Groupname"]).ToLower() == "senior management" || Convert.ToString(Session["Groupname"]).ToLower() == "functional management" || Convert.ToString(Session["Groupname"]).ToLower() == "others")
                                    {

                                    }
                                    else
                                    {
                                        tblUploadAgenda.Style.Add("display", "block");
                                        tblUploadAgenda.Style.Add("background-color", "#eee9f0 none repeat scroll 0 0");
                                    }
                                }


                        btnDelete.Visible = false;
                        btnAuthorizeAll.Visible = false;
                        chkSelectalll.Visible = false;
                        btnnotebook.Visible = false;
                        btnMoveDown.Visible = false;
                        btnInvitee.Visible = false;
                        btnPublish.Visible = false;
                        btnFirstSeperator.Visible = false;
                        btnLastSeperator.Visible = false;
                        txtColor.Visible = false;
                        lblRestricted.Visible = false;
                        gvData.DataSource = null;
                        gvData.DataBind();
                    }
                }
                else
                {

                    gvData.DataSource = null;
                    gvData.DataBind();

                    btnDelete.Visible = false;
                    btnAuthorizeAll.Visible = false;
                    chkSelectalll.Visible = false;
                    btnExport.Visible = false;
                    btnSMS.Visible = false;
                    btnMoveDown.Visible = false;
                    btnInvitee.Visible = false;
                    btnPublish.Visible = false;
                    btnnotebook.Visible = false;
                    btnComplete.Visible = false;
                    btnFirstSeperator.Visible = false;
                    btnLastSeperator.Visible = false;
                    txtColor.Visible = false;
                    lblRestricted.Visible = false;
                }


            }

            OperationClass objOC = new OperationClass();
            string TableNamenew = objOC.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Convert.ToInt32(Session["FolderID"])));
            //int count = Convert.ToInt16(objOC.ExecuteScalar4Command(string.Format(@"select count(*) from {0} where folderid={1}", TableName,Convert.ToInt16(Session["FolderID"]))));
            if (Session["GroupName"].ToString().ToLower() == "admin")
            {
                int folder = Convert.ToInt32(Session["FolderID"]);

                string meetingcancelled = objOC.ExecuteScalar4Command(string.Format(@"select MeetingCancelled from tblFolder where FolderId = {0}", folder));
                if ((meetingcancelled == "" || meetingcancelled == "0") && Convert.ToInt32(Session["ParentFolderId"]) != 1)
                {
                    //if (dtfiledetails != null)
                    //{
                    if (dtfiledetails.Rows.Count > 0)
                    {
                        //btnCancel.Visible = true;
                    }
                    else if (dtfiledetails.Rows.Count <= 0)
                    {
                        //btnCancel.Visible = false;
                        btnDelete.Visible = false;
                        btnAuthorizeAll.Visible = false;
                        chkSelectalll.Visible = false;
                        btnnotebook.Visible = true;
                        btnMoveDown.Visible = false;
                        btnInvitee.Visible = false;
                        btnPublish.Visible = false;
                        btnFirstSeperator.Visible = false;
                        btnLastSeperator.Visible = false;
                        txtColor.Visible = false;
                        lblRestricted.Visible = false;


                    }
                    //}
                    //else 
                    //{
                    //    btnCancel.Visible = true;
                    //    btnDelete.Visible = false;
                    //    btnComplete.Visible = false;
                    //}
                }
                else
                {
                    if (meetingcancelled == "1")
                    {
                        btnExport.Visible = false;
                        btnSMS.Visible = false;
                        btnDelete.Visible = false;
                        btnComplete.Visible = false;
                        btnActivate.Visible = true;
                        btnAuthorizeAll.Visible = false;
                        chkSelectalll.Visible = false;
                        btnMoveDown.Visible = false;
                        btnInvitee.Visible = false;
                        btnPublish.Visible = false;
                        btnFirstSeperator.Visible = false;
                        btnLastSeperator.Visible = false;
                        txtColor.Visible = false;
                        lblRestricted.Visible = false;
                    }
                }
                if (Session["FolderID"] != null && (Session["FolderID"].ToString()) != "")
                {
                    DataTable dtFolderAccess = objCommonBAL.GetMeetingStatus(Convert.ToInt32(Session["FolderID"]));
                    if (dtFolderAccess.Rows.Count > 0)
                    {
                        int meetingStatus = Convert.ToInt16(dtFolderAccess.Rows[0]["MeetingStatus"]);
                        if (meetingStatus == 0 && Convert.ToInt32(Session["ParentFolderId"]) != 1)// && statusofPhysicalTable > 0)
                        {
                            if (dtfiledetails.Rows.Count > 0)
                            {
                                btnComplete.Visible = true;
                                btnStartMOM.Visible = false;
                            }
                            else
                            {
                                btnComplete.Visible = false;
                            }
                        }
                        else
                        {
                            btnComplete.Visible = false;
                            //btnCancel.Visible = false;
                            btnStartMOM.Visible = false;
                            btnDelete.Visible = false;
                            btnAuthorizeAll.Visible = false; //When Meeting is archived
                            chkSelectalll.Visible = false;
                            btnMoveDown.Visible = false;
                            btnInvitee.Visible = false;
                            btnPublish.Visible = false;
                            btnFirstSeperator.Visible = false;
                            btnLastSeperator.Visible = false;
                            txtColor.Visible = false;
                            lblRestricted.Visible = false;
                            Session["MeetingStatus"] = meetingStatus.ToString();
                        }
                    }
                    else
                        btnComplete.Visible = false;
                }
                else
                {
                    btnComplete.Visible = false;
                }
            }
            else if (Session["Designation"].ToString().ToLower() == "board secretary" || Convert.ToString(Session["Groupname"]).ToLower().Trim() == "board secretariat user" || Session["Designation"].ToString().ToLower() == "board secretary" || Session["GroupName"].ToString().ToLower() == "admin" || Session["GroupName"].ToString().ToLower().Trim() == "president")
            {
                int folder = Convert.ToInt32(Session["FolderID"]);

                string meetingcancelled = objOC.ExecuteScalar4Command(string.Format(@"select MeetingCancelled from tblFolder where FolderId = {0}", folder));
                if ((meetingcancelled == "" || meetingcancelled == "0") && Convert.ToInt32(Session["ParentFolderId"]) != 1)
                {
                    //if (dtfiledetails != null)
                    //{
                    if (dtfiledetails.Rows.Count > 0)
                    {
                        //btnCancel.Visible = true;
                        btnnotebook.Visible = true;
                    }
                    else if (dtfiledetails.Rows.Count <= 0)
                    {
                        //btnCancel.Visible = false;
                        btnDelete.Visible = false;
                        btnAuthorizeAll.Visible = false;
                        chkSelectalll.Visible = false;
                        btnMoveDown.Visible = false;
                        btnInvitee.Visible = false;
                        btnPublish.Visible = false;
                        btnFirstSeperator.Visible = false;
                        btnLastSeperator.Visible = false;
                        txtColor.Visible = false;
                        lblRestricted.Visible = false;
                    }

                }
                else
                {
                    if (meetingcancelled == "1")
                    {
                        btnExport.Visible = false;
                        btnSMS.Visible = false;
                        btnDelete.Visible = false;
                        btnComplete.Visible = false;
                        btnActivate.Visible = true;
                        btnAuthorizeAll.Visible = false;
                        chkSelectalll.Visible = false;
                        btnMoveDown.Visible = false;
                        btnInvitee.Visible = false;
                        btnPublish.Visible = false;
                        btnFirstSeperator.Visible = false;
                        btnLastSeperator.Visible = false;
                        txtColor.Visible = false;
                        lblRestricted.Visible = false;
                    }
                }
                if (Session["FolderID"] != null && (Session["FolderID"]) != "")
                {
                    DataTable dtFolderAccess = objCommonBAL.GetMeetingStatus(Convert.ToInt32(Session["FolderID"]));
                    if (dtFolderAccess.Rows.Count > 0)
                    {
                        int meetingStatus = Convert.ToInt16(dtFolderAccess.Rows[0]["MeetingStatus"]);
                        if (meetingStatus == 0 && Convert.ToInt32(Session["ParentFolderId"]) != 1)// && statusofPhysicalTable > 0)
                        {
                            if (dtfiledetails.Rows.Count > 0)
                            {
                                if (Symbol == "F")
                                {
                                    btnComplete.Visible = true;
                                    btnStartMOM.Visible = true;
                                }
                                else
                                    if (Symbol == "R")
                                    {
                                        btnComplete.Visible = false;
                                        btnStartMOM.Visible = false;
                                    }
                                    else
                                        if (Symbol == "M")
                                        {
                                            btnComplete.Visible = false;
                                            btnStartMOM.Visible = false;
                                        }
                            }
                            else
                            {
                                btnComplete.Visible = false;
                            }
                        }
                        else
                        {

                            btnAuthorizeAll.Visible = false;

                            btnnotebook.Visible = false;
                            btnStartMOM.Visible = false;
                            btnComplete.Visible = false;
                            btnExport.Visible = false;
                            btnSMS.Visible = false;
                            chkSelectalll.Visible = false;
                            btnMoveDown.Visible = false;
                            btnFirstSeperator.Visible = false;
                            btnLastSeperator.Visible = false;
                            txtColor.Visible = false;
                            lblRestricted.Visible = false;
                            if (Session["FolderName"].ToString().ToLower() == "meetings\\my briefcase" || Session["FolderName"].ToString().ToLower() == "meetings\\company info" || Session["FolderName"].ToString().ToLower() == "meetings\\shared documents")
                            {
                                btnPush.Visible = false;

                                btnBackFromArchive.Visible = false;
                                btnBackFromArchive.Visible = false;
                                btnDelete.Visible = true;
                                //btnwaterpwd.Visible = false;
                                //btnwaterwoutpwd.Visible = false;
                                //btnpwdwoutwater.Visible = false;
                                //btnwithoutPwdwater.Visible = false;
                            }
                            else
                            {
                                btnPush.Visible = false;
                                btnBackFromArchive.Visible = true;
                                btnBackFromArchive.Visible = true;
                                btnDelete.Visible = false;
                            }

                            btnInvitee.Visible = false;
                            btnPublish.Visible = false;
                            //;
                            // btnStartMOM.Visible = true;
                            // btnDelete.Visible = true;
                            // btnAuthorizeAll.Visible = true;
                            // chkSelectalll.Visible = false;
                            Session["MeetingStatus"] = meetingStatus.ToString();
                        }
                    }
                    else
                        btnComplete.Visible = false;
                }
                else
                {
                    btnComplete.Visible = false;
                }
                if (meetingcancelled == "1")
                {
                    btnComplete.Visible = false;
                }
            }
            else
                if (Convert.ToString(Session["Groupname"]).ToLower().Trim() == "office secretary")
                {
                    DataTable dtFolderAccess = objCommonBAL.GetMeetingStatus(Convert.ToInt32(Session["FolderID"]));
                    int meetingStatus = Convert.ToInt16(dtFolderAccess.Rows[0]["MeetingStatus"]);
                    //if (dtFolderAccess.Rows.Count > 0)
                    //{
                    //    int meetingStatus = Convert.ToInt16(dtFolderAccess.Rows[0]["MeetingStatus"]);
                    //    if (meetingStatus ==1 && Convert.ToInt32(Session["ParentFolderId"]) != 1)
                    //    {
                    //    }
                    //}

                    if (Session["FolderName"].ToString().ToLower() == "meetings\\my briefcase" || Session["FolderName"].ToString().ToLower() == "meetings\\company info" || Session["FolderName"].ToString().ToLower() == "meetings\\shared documents" || meetingStatus == 1)
                    {
                        btnAuthorizeAll.Visible = false;

                        btnnotebook.Visible = false;
                        btnStartMOM.Visible = false;
                        btnComplete.Visible = false;
                        btnExport.Visible = false;
                        btnSMS.Visible = false;
                        chkSelectalll.Visible = false;
                        btnMoveDown.Visible = false;
                        btnFirstSeperator.Visible = false;
                        btnLastSeperator.Visible = false;
                        btnSubmitSelected.Visible = false;
                        txtColor.Visible = false;
                        lblRestricted.Visible = false;
                        if (Session["FolderName"].ToString().ToLower() == "meetings\\my briefcase" || Session["FolderName"].ToString().ToLower() == "meetings\\company info" || Session["FolderName"].ToString().ToLower() == "meetings\\shared documents")
                        {
                            btnPush.Visible = false;
                            btnBackFromArchive.Visible = false;
                            btnBackFromArchive.Visible = false;
                            btnDelete.Visible = true;
                            //btnwaterpwd.Visible = false;
                            //btnwaterwoutpwd.Visible = false;
                            //btnpwdwoutwater.Visible = false;
                            //btnwithoutPwdwater.Visible = false;
                        }
                        else
                        {
                            btnPush.Visible = false;
                            btnBackFromArchive.Visible = true;
                            btnBackFromArchive.Visible = true;
                            btnDelete.Visible = false;
                        }

                        btnInvitee.Visible = false;
                        btnPublish.Visible = false;
                    }
                }
                else
                {
                    //btnCancel.Visible = false;
                    btnExport.Visible = false;
                    btnSMS.Visible = false;
                    btnComplete.Visible = false;
                }
        }
        catch (Exception ex)
        {
        }
        finally
        {

        }
    }
    //private void FileDownLoad(string filePath)
    //{
    //    if (filePath != "")
    //    {
    //        FileInfo myfile = new FileInfo(filePath);
    //        if (myfile.Exists)
    //        {
    //            HttpContext.Current.Response.ClearContent();
    //            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + myfile.Name);

    //            HttpContext.Current.Response.ContentType = ReturnExtension(myfile.Extension.ToLower());

    //            using (StringWriter sw = new StringWriter())
    //            {
    //                using (HtmlTextWriter hw = new HtmlTextWriter(sw))
    //                {
    //                    //To Export all pages
    //                    //GridView1.AllowPaging = false;
    //                    //this.BindGrid();

    //                    //GridView1.RenderControl(hw);

    //                    StringReader sr = new StringReader(sw.ToString());
    //                    Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
    //                    HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
    //                    //using (MemoryStream memoryStream = new MemoryStream())
    //                    //{
    //                       // PdfWriter.GetInstance(pdfDoc, memoryStream);
    //                        pdfDoc.Open();
    //                        htmlparser.Parse(sr);
    //                        pdfDoc.Close();
    //                        //byte[] bytes = memoryStream.ToArray();
    //                        //memoryStream.Close();
    //                        using (MemoryStream input = new MemoryStream())
    //                        {
    //                            using (MemoryStream output = new MemoryStream())
    //                            {
    //                                string password = "pass@123";
    //                                PdfReader reader = new PdfReader(input);
    //                                PdfEncryptor.Encrypt(reader, output, true, password, password, PdfWriter.ALLOW_SCREENREADERS);
    //                                HttpContext.Current.Response.ContentType = ReturnExtension(myfile.Extension.ToLower());
    //                                HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + myfile.Name);
    //                                Response.Cache.SetCacheability(HttpCacheability.NoCache);
    //                                //Response.BinaryWrite(bytes);
    //                                Response.End();
    //                            }
    //                        }
    //                    //}
    //                }
    //            }
    //            HttpContext.Current.Response.TransmitFile(myfile.FullName);
    //            HttpContext.Current.Response.Flush();
    //            HttpContext.Current.Response.Close();
    //            HttpContext.Current.ApplicationInstance.CompleteRequest();
    //            //HttpContext.Current.Response.End();
    //        }
    //    }
    //}


    //protected void FileDownLoad(string filePath)
    //{
    //    using (StringWriter sw = new StringWriter())
    //    {
    //        using (HtmlTextWriter hw = new HtmlTextWriter(sw))
    //        {
    //            //To Export all pages
    //            // GridView1.AllowPaging = false;
    //            // this.BindGrid();

    //            //GridView1.RenderControl(hw);

    //            if (filePath != "")
    //            {
    //                FileInfo myfile = new FileInfo(filePath);

    //                if (myfile.Exists)
    //                {
    //                    //HttpContext.Current.Response.ClearContent();
    //                    //HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + myfile.Name);

    //                    //HttpContext.Current.Response.ContentType = ReturnExtension(myfile.Extension.ToLower());

    //                    StringReader sr = new StringReader(sw.ToString());
    //                    //Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
    //                    string originalFile = filePath;
    //                    int startPage = 1;
    //                    using (FileStream fs = new FileStream(originalFile, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
    //                    using (Document doc = new Document(PageSize.A4))
    //                    {
    //                        using (PdfWriter writer = PdfWriter.GetInstance(doc, fs))
    //                        {
    //                            //HTMLWorker htmlparser = new HTMLWorker(myfile.Name);
    //                            using (MemoryStream memoryStream = new MemoryStream())
    //                            {
    //                               //PdfWriter.GetInstance(pdfDoc, memoryStream);


    //                                //pdfDoc.Open();
    //                                //myfile.Open();
    //                                //htmlparser.Parse(sr);
    //                                //myfile.Close();
    //                                byte[] bytes = memoryStream.ToArray();
    //                                memoryStream.Close();
    //                                using (MemoryStream input = new MemoryStream(bytes))
    //                                {
    //                                    using (MemoryStream output = new MemoryStream())
    //                                    {
    //                                        string password = "pass@123";
    //                                        doc.Open();
    //                                        PdfReader reader = new PdfReader(originalFile);

    //                                        PdfEncryptor.Encrypt(reader, output, true, password, password, PdfWriter.ALLOW_SCREENREADERS);
    //                                        bytes = output.ToArray();
    //                                        Response.ContentType = "application/pdf";
    //                                        Response.AddHeader("content-disposition", "attachment;filename=" + myfile.Name);
    //                                        Response.Cache.SetCacheability(HttpCacheability.NoCache);
    //                                        Response.BinaryWrite(bytes);

    //                                       //HttpContext.Current.Response.TransmitFile(myfile.FullName);
    //                                       //HttpContext.Current.Response.Flush();
    //                                       //HttpContext.Current.Response.Close();
    //                                       //HttpContext.Current.ApplicationInstance.CompleteRequest();
    //                                    }
    //                                }
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}
    //protected void FileDownLoad(string filePath)
    //{
    //    using (StringWriter sw = new StringWriter())
    //    {
    //        using (HtmlTextWriter hw = new HtmlTextWriter(sw))
    //        {
    //            //To Export all pages
    //            //GridView1.AllowPaging = false;
    //            //this.BindGrid();

    //            //GridView1.RenderControl(hw);

    //            if (filePath != "")
    //            {
    //                FileInfo myfile = new FileInfo(filePath);

    //                if (myfile.Exists)
    //                {
    //                    HttpContext.Current.Response.ClearContent();
    //                    //HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + myfile.Name);

    //                    //HttpContext.Current.Response.ContentType = ReturnExtension(myfile.Extension.ToLower());

    //                    StringReader sr = new StringReader(sw.ToString());

    //                    //Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
    //                    //                      Document pdfDoc = new Document(PageSize.A4);
    //                    string originalFile = filePath;

    //                    using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
    //                    using (Document doc = new Document(PageSize.A4))
    //                    {
    //                        using (PdfWriter writer = PdfWriter.GetInstance(doc, fs))
    //                        {

    //                          //  HTMLWorker htmlparser = new HTMLWorker(doc);
    //                            using (MemoryStream memoryStream = new MemoryStream())
    //                            {
    //                               // PdfWriter.GetInstance(pdfDoc, memoryStream);
    //                                //pdfDoc.Open();
    //                                // doc.Open();
    //                                // htmlparser.Parse(sr);
    //                                //pdfDoc.Close();
    //                              //  doc.Close();
    //                                 byte[] bytes = new byte[fs.Length];
    //                                //byte[] bytes = memoryStream.ToArray();
    //                                memoryStream.Close();
    //                                using (MemoryStream input = new MemoryStream(bytes))
    //                                {
    //                                    using (MemoryStream output = new MemoryStream())
    //                                    {
    //                                        string password = "pass@123";
    //                                        PdfReader reader = new PdfReader(input);
    //                                        PdfEncryptor.Encrypt(reader, output, true, password, password, PdfWriter.ALLOW_SCREENREADERS);
    //                                        bytes = output.ToArray();
    //                                        Response.ContentType = "application/pdf";
    //                                        Response.AddHeader("content-disposition", "attachment; filename=" + myfile.Name);
    //                                        Response.Cache.SetCacheability(HttpCacheability.NoCache);
    //                                        Response.BinaryWrite(bytes);
    //                                        Response.End();
    //                                    }
    //                                }
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }

    //}

    //public override void VerifyRenderingInServerForm(Control control)
    //{
    //    /* Verifies that the control is rendered */
    //}


    //latest code


    //protected void FileDownLoad(string filePath)
    //{
    //    if (filePath != "")
    //    {
    //        FileInfo myfile = new FileInfo(filePath);

    //        if (myfile.Exists)
    //        {
    //            Response.ContentType = "application/pdf";
    //            Response.AddHeader("content-disposition", "attachment;filename=" + myfile.Name);
    //            Response.Cache.SetCacheability(HttpCacheability.NoCache);
    //            StringWriter sw = new StringWriter();
    //            HtmlTextWriter hw = new HtmlTextWriter(sw);
    //           //this.Page.RenderControl(hw);
    //            StringReader sr = new StringReader(sw.ToString());
    //            Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 100f, 0.0f);
    //            HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
    //            PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
    //            pdfDoc.Open();
    //            htmlparser.Parse(sr);
    //            pdfDoc.Close();
    //            Response.Write(pdfDoc);
    //            Response.End();
    //        }
    //    }
    //}
    //public override void VerifyRenderingInServerForm(Control control)
    //{
    //    /* Verifies that the control is rendered */
    //}


    //protected void chkSelectEmailId_CheckedChanged(object sender, EventArgs e)
    //{

    //    if (Session["FolderName"].ToString().ToLower() != "board & its committees\\recycle bin")
    //    {
    //        if (chkSelectEmailId.Checked)
    //        {
    //            for (int i = 0; i < gvData.Rows.Count; i++)
    //            {
    //                System.Web.UI.WebControls.CheckBox m_ChkSelectInvitees = (System.Web.UI.WebControls.CheckBox)gvData.Rows[i].FindControl("chkSelect");
    //                m_ChkSelectInvitees.Checked = true;

    //            }
    //        }
    //        else
    //        {
    //            for (int i = 0; i < gvData.Rows.Count; i++)
    //            {
    //                System.Web.UI.WebControls.CheckBox m_ChkSelectInvitees = (System.Web.UI.WebControls.CheckBox)gvData.Rows[i].FindControl("chkSelect");
    //                m_ChkSelectInvitees.Checked = false;

    //            }
    //        }
    //    }
    //    else
    //    {
    //        if (chkSelectEmailId.Checked)
    //        {
    //            for (int i = 0; i < gvParent.Rows.Count; i++)
    //            {
    //                System.Web.UI.WebControls.CheckBox m_ChkSelectInvitees = (System.Web.UI.WebControls.CheckBox)gvParent.Rows[i].FindControl("chkSelect");
    //                m_ChkSelectInvitees.Checked = true;

    //            }
    //        }
    //        else
    //        {
    //            for (int i = 0; i < gvParent.Rows.Count; i++)
    //            {
    //                System.Web.UI.WebControls.CheckBox m_ChkSelectInvitees = (System.Web.UI.WebControls.CheckBox)gvParent.Rows[i].FindControl("chkSelect");
    //                m_ChkSelectInvitees.Checked = false;

    //            }
    //        }

    //    }
    //}

    //protected void ddlComment_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    DropDownList ddl = new DropDownList();
    //    foreach (GridViewRow row in gvData.Rows)
    //    {
    //        DropDownList ddlComment = (DropDownList)row.FindControl("ddlComment");
    //        if (ddlComment.SelectedIndex != 0)
    //        {

    //            string str = ddlComment.SelectedItem.Text;
    //            Response.Redirect("../Viewer/Thumbnailsearch.aspx?id=" + str);
    //        }

    //    }

    //}

    protected void FileDownLoadWithoutPassword(string filePath)
    {
        if (filePath != "")
        {
            FileInfo myfile = new FileInfo(filePath);

            if (myfile.Exists)
            {
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=" + myfile.Name);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                StringWriter sw = new StringWriter();
                HtmlTextWriter hw = new HtmlTextWriter(sw);
                //this.Page.RenderControl(hw);
                StringReader sr = new StringReader(sw.ToString());
                Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 100f, 0.0f);
                HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                pdfDoc.Open();
                htmlparser.Parse(sr);
                pdfDoc.Close();
                Response.Write(pdfDoc);
                Response.End();
            }
        }
    }

    public static IEnumerable<int> StringToIntList(string str)
    {
        if (String.IsNullOrEmpty(str))
            yield break;

        foreach (var s in str.Split(','))
        {
            int num;
            if (int.TryParse(s, out num))
                yield return num;
        }
    }

    protected void FileDownLoad(string filePath)
    {
        if (Session["txtboxPassword"].ToString() != "")
        {
            if (filePath != "")
            {
                FileInfo myfile = new FileInfo(filePath);

                if (myfile.Exists)
                {
                    string path = Server.MapPath("~/");
                    //string fileName =txtPDFName.Text + ".pdf";
                    string fileName = "dot.pdf";

                    string ModifiedFileName = string.Empty;
                    object TargetFile = filePath;
                    //string Extension = ReturnExtension(filePath.Extension.ToLower());
                    //string Extensionbefore = filePath.Substring(0, filePath.LastIndexOf("."));
                    //object TargetFile = Extensionbefore + "_" + Session["FolderId"].ToString() + ".pdf";
                    //Bind PDF
                    //BindData(path, fileName);

                    iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(TargetFile.ToString());
                    ModifiedFileName = TargetFile.ToString();
                    ModifiedFileName = ModifiedFileName.Insert(ModifiedFileName.Length - 4, "pdf");

                    iTextSharp.text.pdf.PdfEncryptor.Encrypt(reader, new FileStream(ModifiedFileName, FileMode.Append), iTextSharp.text.pdf.PdfWriter.STRENGTH128BITS, Session["txtboxPassword"].ToString(), Session["txtboxPassword"].ToString(), iTextSharp.text.pdf.PdfWriter.AllowPrinting);

                    if (File.Exists(TargetFile.ToString()))
                        File.Delete(TargetFile.ToString());
                    //Send PDF
                    //SendMail(ModifiedFileName);
                    PdfFileDownLoad(ModifiedFileName);

                }
            }
        }
    }

    protected void FileDownLoadwithWaterMark(string filePath)
    {
        //string watermarkText = "Naresh Srinivas Singu";

        try
        {
            if (Session["Name"] != null)
            {
                string NootBookPasswrd = operation.ExecuteScalar4Command(string.Format(@"select NoteBookPassword from tblNoteBookPassword where UserId={0}", Convert.ToInt64(Session["UserID"])));

                string watermarkText = Session["Name"].ToString();
                string ModifiedFileName = string.Empty;
                object TargetFile = filePath;
                iTextSharp.text.pdf.PdfReader reader1 = new iTextSharp.text.pdf.PdfReader(TargetFile.ToString());
                ModifiedFileName = TargetFile.ToString();
                ModifiedFileName = ModifiedFileName.Insert(ModifiedFileName.Length - 4, "pdf");
                //PdfReader reader1 = new PdfReader(filePath);
                //using (FileStream fs = new FileStream(OutLocation, FileMode.Create, FileAccess.Write, FileShare.None))
                using (FileStream fs = new FileStream(ModifiedFileName, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    //iTextSharp.text.pdf.PdfEncryptor.Encrypt(reader1, fs, iTextSharp.text.pdf.PdfWriter.STRENGTH128BITS, Session["txtboxPassword"].ToString(), Session["txtboxPassword"].ToString(), iTextSharp.text.pdf.PdfWriter.AllowPrinting);

                    using (PdfStamper stamper = new PdfStamper(reader1, fs))
                    {
                        if (NootBookPasswrd == "")
                        {
                            stamper.SetEncryption(iTextSharp.text.pdf.PdfWriter.STRENGTH128BITS, Session["txtboxPassword"].ToString(), Session["txtboxPassword"].ToString(), iTextSharp.text.pdf.PdfWriter.AllowPrinting);
                        }
                        else
                        {
                            stamper.SetEncryption(iTextSharp.text.pdf.PdfWriter.STRENGTH128BITS, NootBookPasswrd, NootBookPasswrd, iTextSharp.text.pdf.PdfWriter.AllowPrinting);

                        }
                        int pageCount1 = reader1.NumberOfPages;
                        //Create a new layer
                        PdfLayer layer = new PdfLayer("WatermarkLayer", stamper.Writer);
                        for (int i = 1; i <= pageCount1; i++)
                        {
                            iTextSharp.text.Rectangle rect = reader1.GetPageSize(i);
                            //Get the ContentByte object
                            PdfContentByte cb = stamper.GetOverContent(i);
                            //Tell the CB that the next commands should be "bound" to this new layer
                            cb.BeginLayer(layer);
                            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED), 50);
                            PdfGState gState = new PdfGState();
                            gState.FillOpacity = 0.50f;
                            cb.SetGState(gState);
                            cb.SetColorFill(BaseColor.GRAY);
                            cb.BeginText();
                            cb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, watermarkText, rect.Width / 2, rect.Height / 2, 45f);
                            //string TableNameFrom = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Convert.ToInt32(Session["FolderID"])));
                            //DataTable dtfiledetails1 = objCommonBAL.GetDispFileName(TableNameFrom, Convert.ToInt32(Session["FolderID"]), Convert.ToInt32(Session["UserID"]));
                            //if (i != 1)
                            //{
                            //    if (dtfiledetails1.Rows.Count > 0)
                            //    {
                            //        foreach (DataRow dr in dtfiledetails1.Rows)
                            //        {
                            //            string Status = dr["ApprovalStatus"].ToString();
                            //            if (Status == "2")
                            //            {
                            //                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Withdrawn", 200f, 15f, 2f);
                            //                //cb.ShowTextAligned(stamper.GetUnderContent(i), Element.ALIGN_RIGHT, new Phrase("Index -" + i.ToString(), blackFont), 568f, 15f, 0);
                            //            }
                            //        }

                            //    }
                            //}
                            cb.EndText();
                            //"Close" the layer
                            cb.EndLayer();
                        }

                    }
                }


                PdfFileDownLoad(ModifiedFileName);
                //if (File.Exists(TargetFile.ToString()))
                //    File.Delete(TargetFile.ToString());
            }
        }
        catch (Exception es)
        {
        }
    }


    protected void FileDownLoadwithoutWaterMark(string filePath)
    {
        //string watermarkText = "Naresh Srinivas Singu";

        try
        {
            if (Session["Name"] != null)
            {
                //string watermarkText = Session["Name"].ToString();
                string watermarkText = "";

                string ModifiedFileName = string.Empty;
                object TargetFile = filePath;
                iTextSharp.text.pdf.PdfReader reader1 = new iTextSharp.text.pdf.PdfReader(TargetFile.ToString());
                ModifiedFileName = TargetFile.ToString();
                ModifiedFileName = ModifiedFileName.Insert(ModifiedFileName.Length - 4, "pdf");
                //PdfReader reader1 = new PdfReader(filePath);
                //using (FileStream fs = new FileStream(OutLocation, FileMode.Create, FileAccess.Write, FileShare.None))
                using (FileStream fs = new FileStream(ModifiedFileName, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    //iTextSharp.text.pdf.PdfEncryptor.Encrypt(reader1, fs, iTextSharp.text.pdf.PdfWriter.STRENGTH128BITS, Session["txtboxPassword"].ToString(), Session["txtboxPassword"].ToString(), iTextSharp.text.pdf.PdfWriter.AllowPrinting);

                    using (PdfStamper stamper = new PdfStamper(reader1, fs))
                    {
                        stamper.SetEncryption(iTextSharp.text.pdf.PdfWriter.STRENGTH128BITS, Session["txtboxPassword"].ToString(), Session["txtboxPassword"].ToString(), iTextSharp.text.pdf.PdfWriter.AllowPrinting);
                        int pageCount1 = reader1.NumberOfPages;
                        //Create a new layer
                        PdfLayer layer = new PdfLayer("WatermarkLayer", stamper.Writer);
                        for (int i = 1; i <= pageCount1; i++)
                        {
                            iTextSharp.text.Rectangle rect = reader1.GetPageSize(i);
                            //Get the ContentByte object
                            PdfContentByte cb = stamper.GetOverContent(i);
                            //Tell the CB that the next commands should be "bound" to this new layer
                            cb.BeginLayer(layer);
                            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED), 50);
                            PdfGState gState = new PdfGState();
                            gState.FillOpacity = 0.50f;
                            cb.SetGState(gState);
                            cb.SetColorFill(BaseColor.RED);
                            cb.BeginText();
                            cb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, watermarkText, rect.Width / 2, rect.Height / 2, 45f);
                            cb.EndText();
                            //"Close" the layer
                            cb.EndLayer();
                        }

                    }
                }


                PdfFileDownLoad(ModifiedFileName);
                //if (File.Exists(TargetFile.ToString()))
                //    File.Delete(TargetFile.ToString());
            }
        }
        catch (Exception es)
        {
        }
    }



    private void PdfFileDownLoad(string filePath)
    {
        if (filePath != "")
        {

            //string Dmeetingdt1 = Session["DMeetingdate"].ToString();
            //string pattern1 = "/";
            //Regex reg1 = new Regex(pattern1);
            //string two1 = "-";
            //string Dmeetingdate = reg1.Replace(Dmeetingdt1, two1);

            //string Dmeetingdt = Dmeetingdt1.Replace("\\","-");
            //string filename = Session["DCommiteename"].ToString() + "_" + Dmeetingdate;
            //string filename1 = filename + "_"+Session["Itemno"].ToString();
            FileInfo myfile = new FileInfo(filePath);
            if (myfile.Exists)
            {

                //HttpContext.Current.Response.ClearContent();
                //HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + myfile.Name);
                //HttpContext.Current.Response.ContentType = ReturnExtension(myfile.Extension.ToLower());

                //HttpContext.Current.Response.TransmitFile(myfile.FullName);
                //HttpContext.Current.Response.Flush();
                //HttpContext.Current.Response.Close();
                //HttpContext.Current.ApplicationInstance.CompleteRequest();

                //var file = new System.IO.FileInfo(sFilePath);

                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + myfile.Name);
                HttpContext.Current.Response.AddHeader("Content-Length", myfile.Length.ToString(CultureInfo.InvariantCulture));
                HttpContext.Current.Response.ContentType = "application/octet-stream";
                HttpContext.Current.Response.WriteFile(myfile.FullName);
                HttpContext.Current.Response.End();
            }
        }
    }

    #region ReturnExtension
    private string ReturnExtension(string fileExtension)
    {
        switch (fileExtension)
        {
            case ".htm":
            case ".html":
            case ".log":
                return "text/HTML";
            case ".txt":
                return "text/plain";
            case ".doc":
                return "application/ms-word";
            case ".tiff":
            case ".tif":
                return "image/tiff";
            case ".asf":
                return "video/x-ms-asf";
            case ".avi":
                return "video/avi";
            case ".zip":
                return "application/zip";
            case ".xls":
            case ".csv":
                return "application/vnd.ms-excel";
            case ".gif":
                return "image/gif";
            case ".jpg":
            case "jpeg":
                return "image/jpeg";
            case ".bmp":
                return "image/bmp";
            case ".png":
                return "image/png";
            case ".wav":
                return "audio/wav";
            case ".mp3":
                return "audio/mpeg3";
            case ".mpg":
            case "mpeg":
                return "video/mpeg";
            case ".rtf":
                return "application/rtf";
            case ".asp":
                return "text/asp";
            case ".pdf":
                return "application/pdf";
            case ".fdf":
                return "application/vnd.fdf";
            case ".ppt":
                return "application/mspowerpoint";
            case ".dwg":
                return "image/vnd.dwg";
            case ".msg":
                return "application/msoutlook";
            case ".xml":
            case ".sdxl":
                return "application/xml";
            case ".xdp":
                return "application/vnd.adobe.xdp+xml";
            default:
                return "application/octet-stream";
        }
    }
    #endregion ReturnExtension
    //Added by Kirti on 04/06/13 
    [WebMethod]
    public static string WithdrawWithComments(string fileid, string comments)
    {
        GenericDAL objDAL = new GenericDAL();
        int success = 0;
        success = (int)objDAL.ExecuteNonQuery("Update tblFile set ApprovalStatus='2' , withdrawcomments='" + comments + "'where FileId=" + Convert.ToInt32(fileid) + "");
        if (success == 1)
        {
            //  ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('" + filename + " is withdrawn.')", true);
            return "true";

        }
        else
            return "false";
    }

    protected void gvData_DataBound(object sender, EventArgs e)
    {
        try
        {
            if (gvData.Rows.Count > 0)
            {

                for (int i = 0; i < gvData.Rows.Count; i++)
                {
                    GridViewRow row = gvData.Rows[i];
                    Label viewStatus = (Label)row.FindControl("lblviewstaus");
                    LinkButton lnkDownload = (LinkButton)row.FindControl("lnkDownload");
                    LinkButton lnkOpen1 = (LinkButton)row.FindControl("lnkOpen1");
                    LinkButton lnkOpen = (LinkButton)row.FindControl("lnkOpen");
                    Label lblFileName = (Label)row.FindControl("lblFileName");
                    Label lblstatus = (Label)row.FindControl("lblstatus");
                    Label lblSRno = (Label)row.FindControl("LinkButton1");
                    Label lblPageNo = (Label)row.FindControl("lblPageNo");
                    Label lblApprovalStatus = (Label)row.FindControl("lblApprovalStatus");
                    Label lblwithdrowcomment = (Label)row.FindControl("lblWithdrawComments");
                    Label lblFileId = (Label)row.FindControl("lblFileId");
                    ImageButton Imgatach = (ImageButton)row.FindControl("Imgatach");
                    ImageButton ImgReplaceNote = (ImageButton)row.FindControl("ImgReplaceNote");
                    ImageButton imgbtnaccessRes = (ImageButton)row.FindControl("imgbtnaccessRes");




                    LinkButton lnkEdit = (LinkButton)row.FindControl("lnkEdit");
                    string accesskey = operation.ExecuteScalar4Command(string.Format(@"select value from   tblconfig where keys='AgendaAccessRestricted'"));
                    string strAccess = operation.ExecuteScalar4Command(string.Format(@"select Access from tblAgendalevelAccessControl where fileid='{0}' and userid='{1}'", lblFileId.Text, Session["UserID"]));
                    if (strAccess.ToLower() == "true")
                    {
                        if (accesskey.ToLower() == "yes")
                        {

                            row.BackColor = System.Drawing.Color.LightPink;
                            lblRestricted.Visible = true;
                            txtColor.Visible = true;
                        }
                        else
                        {
                            row.Visible = false;
                            //row.BackColor = System.Drawing.Color.LightPink;
                            // lblRestricted.Visible = true;
                            //txtColor.Visible = true;
                        }


                        //row.BackColor = System.Drawing.Color.LightPink;
                        // lblRestricted.Visible = true;
                        //txtColor.Visible = true;
                    }
                    if (o != 0)
                    {

                        row.Cells[6].Visible = true;
                    }

                    //if (lnkOpen1.Text.ToLower() == "atr" || lnkOpen1.Text.ToLower() == "any other business" || lnkOpen1.Text.ToLower() == "mom")
                    //{
                    //    lnkDownload.Visible = false;

                    //}
                    if (lblApprovalStatus.Text == "2")
                    {

                        row.BackColor = System.Drawing.Color.Wheat;
                    }
                    if ((lblApprovalStatus.Text == "3" || lblApprovalStatus.Text == "1" || lblApprovalStatus.Text == "0") && lblstatus.Text == "0" && Convert.ToString(Session["Groupname"]).ToLower().Trim() == "office secretary")
                    {
                        row.Cells[11].BackColor = System.Drawing.Color.LightYellow;
                    }
                    //if (lblApprovalStatus.Text == "5" && Convert.ToString(Session["Groupname"]).ToLower().Trim() != "office secretary")
                    //{

                    //    row.Visible = false;
                    //}
                    if (lblstatus.Text == "10" && lblApprovalStatus.Text != "2" && lblApprovalStatus.Text != "3" && lblApprovalStatus.Text != "1" && Convert.ToString(Session["Groupname"]).ToLower().Trim() == "board secretariat user")
                    {
                        row.Cells[11].BackColor = System.Drawing.Color.LightGreen;
                    }
                    //if ((lblApprovalStatus.Text == "0" ) && Convert.ToString(Session["Groupname"]).ToLower().Trim() == "office secretary" && strAccess.ToLower() != "true")
                    //{

                    //    row.Visible = true;
                    //    row.Cells[11].BackColor = System.Drawing.Color.LightGreen;
                    //}


                    if (lblwithdrowcomment.Text != "")
                    {
                        row.Cells[11].BackColor = System.Drawing.Color.LightSkyBlue;
                    }

                    if (lblFileName.Text == "")
                    {
                        Imgatach.Visible = false;
                        ImgReplaceNote.Visible = false;
                        imgbtnaccessRes.Visible = false;
                        lnkOpen.Text = "Add Document";
                        lnkDownload.Visible = false;
                        lblSRno.Visible = false;
                        lblPageNo.Visible = false;
                        lnkOpen.Visible = false;
                        lnkDownload.Visible = false;
                        //lnkEdit.Visible = false;
                        lnkOpen1.Enabled = false;

                        row.Cells[2].Font.Bold = true;
                        row.Cells[4].Font.Bold = true;
                        row.BackColor = System.Drawing.Color.SkyBlue;


                    }
                    else
                    {
                        row.Cells[2].Attributes.Add("style", "padding-left:0px;");
                    }
                    if (viewStatus.Text == "1")
                    {
                        //gvData.Rows[i].Cells[5].BorderColor = Color.Yellow;
                        //gvData.Rows[i].Cells[5].ForeColor = Color.Black;
                        //gvData.Rows[i].Cells[5].BorderColor = Color.Black;
                        //gvData.Rows[i].BackColor = Color.Yellow;

                        if (lblstatus.Text == "2")
                        {
                            lnkOpen.Text = "Add Document";
                        }

                        else
                        {
                            lnkOpen.Text = "Viewed";
                        }
                        //lnkOpen.ForeColor = Color.Green;

                    }
                    else
                    {
                        //gvData.Rows[i].Cells[5].ForeColor = Color.Black;
                        //gvData.Rows[i].Cells[5].BorderColor = Color.Black;
                        //gvData.Rows[i].BackColor = Color.Yellow;
                        if (lblstatus.Text == "2")
                        {
                            lnkOpen.Text = "Add Document";
                        }

                        else
                        {
                            lnkOpen.Text = "View";
                        }
                    }
                }
                //for (int rowIndex = gvData.Rows.Count - 2; rowIndex >= 0; rowIndex--)
                //{
                //    GridViewRow row = gvData.Rows[rowIndex];
                //    GridViewRow previousRow = gvData.Rows[rowIndex + 1];
                //    Label fieldrow = (Label)row.FindControl("LinkButton1");
                //    if (fieldrow.Text != null)
                //    {
                //        string tempCurrent = fieldrow.Text;
                //        Label fieldpreviousrow = (Label)previousRow.FindControl("LinkButton1");
                //        string tempprevious = fieldpreviousrow.Text;

                //        if (tempCurrent == tempprevious)
                //        {
                //            row.Cells[1].RowSpan = previousRow.Cells[1].RowSpan < 2 ? 2 : previousRow.Cells[1].RowSpan + 1;
                //            previousRow.Cells[1].Visible = false;
                //        }
                //    }
                //}
            }
        }
        catch (Exception ex) { }
    }



    //protected void gvData_RowDataBound(object sender, GridViewRowEventArgs e)
    //{
    //    if (e.Row.RowType == DataControlRowType.DataRow)
    //    {
    //        string TableNameFrom;
    //        DataTable dtattachment;
    //        string ReturnStr = "";
    //        string strPageCount = "";
    //        string strlastCount = "";
    //        HtmlTableRow tr = (HtmlTableRow)e.Row.FindControl("listviewRow");
    //        Label approv = (Label)e.Row.FindControl("lblApprovalstatus");
    //        CheckBox chSelect = (CheckBox)e.Row.FindControl("chkSelect");
    //        LinkButton lnkApprove = (LinkButton)e.Row.FindControl("LnkApprove");
    //        LinkButton lnkDownload = (LinkButton)e.Row.FindControl("lnkDownload");

    //        Label lblFileName = (Label)e.Row.FindControl("lblFileName");
    //        //LinkButton lnkFileName = (LinkButton)i.FindControl("lnkFileName");
    //        Label lnkFileName = (Label)e.Row.FindControl("lnkFileName");
    //        //Label FileSize = (Label)i.FindControl("lblFileSize");
    //        Label itemno = (Label)e.Row.FindControl("lnkFileName");
    //        Label lotno = (Label)e.Row.FindControl("lbllotno");
    //        Label subject = (Label)e.Row.FindControl("lblsubject");
    //        Label withdrawcomments = (Label)e.Row.FindControl("lblWithdrawComments");
    //        Label meetingdate = (Label)e.Row.FindControl("lblmeettingdate");
    //        LinkButton lnkEdit = (LinkButton)e.Row.FindControl("lnkEdit");
    //        LinkButton lnkOpen = (LinkButton)e.Row.FindControl("lnkOpen");

    //        LinkButton lnkOpen1 = (LinkButton)e.Row.FindControl("lnkOpen1");
    //        Label lblFileId = (Label)e.Row.FindControl("lblFileId");
    //        Label lblviewstaus = (Label)e.Row.FindControl("lblviewstaus");
    //        Label lblComment = (Label)e.Row.FindControl("lblComment");
    //        Label lblComment1 = (Label)e.Row.FindControl("lblComment1");
    //        Label lblComment2 = (Label)e.Row.FindControl("lblComment2");
    //        Label lblComment3 = (Label)e.Row.FindControl("lblComment3");
    //        Label lblComment4 = (Label)e.Row.FindControl("lblComment4");
    //        Label lblComment5 = (Label)e.Row.FindControl("lblComment5");
    //        Label lblFolderId = (Label)e.Row.FindControl("lblFolderId");
    //        Label lblllItemNo = (Label)e.Row.FindControl("lblllItemNo");
    //        Label lblPageNo = (Label)e.Row.FindControl("lblPageNo");
    //        DropDownList ddlComment = (DropDownList)e.Row.FindControl("ddlComment");
    //        ListView GridViewnew = e.Row.FindControl("GridViewnew2") as ListView;
    //        if (lblFileName.Text == "")
    //        {
    //            lnkOpen.Visible = false;
    //            lnkDownload.Visible = false;
    //            lnkOpen1.Enabled = false;
    //        }
    //        string strFolderID = Session["FolderID"].ToString();
    //        TableNameFrom = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Session["FolderID"]));
    //        if (lblFolderId.Text.ToString() == "")
    //        {
    //            strPageCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and b.column0<'{2}'", TableNameFrom, strFolderID, lblllItemNo.Text.ToString()));
    //        }
    //        else
    //        {
    //            strPageCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and b.column0<'{2}'", TableNameFrom, lblFolderId.Text.ToString(), lblllItemNo.Text.ToString()));
    //        }
    //        if (Convert.ToInt32(lblllItemNo.Text.ToString()) > 08)
    //        {
    //            if (lblFolderId.Text.ToString() == "")
    //            {
    //                strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and b.column0<'{2}'", TableNameFrom, strFolderID, (Convert.ToInt32(lblllItemNo.Text.ToString()) + 1)));
    //            }
    //            else
    //            {
    //                strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and b.column0<'{2}'", TableNameFrom, lblFolderId.Text.ToString(), (Convert.ToInt32(lblllItemNo.Text.ToString()) + 1)));
    //            }
    //        }
    //        else
    //        {
    //            if (lblFolderId.Text.ToString() == "")
    //            {
    //                strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and b.column0<'{2}'", TableNameFrom, strFolderID, "0" + (Convert.ToInt32(lblllItemNo.Text.ToString()) + 1)));
    //            }
    //            else
    //            {
    //                strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and b.column0<'{2}'", TableNameFrom, lblFolderId.Text.ToString(), "0" + (Convert.ToInt32(lblllItemNo.Text.ToString()) + 1)));
    //            }

    //        }
    //        if (lblFileName.Text == "")
    //        {
    //            if (strPageCount == "")
    //            {
    //                if (lblFolderId.Text.ToString() == "")
    //                {
    //                    strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and b.column0<'{2}'", TableNameFrom, strFolderID, "0" + (Convert.ToInt32(lblllItemNo.Text.ToString()) + 1)));
    //                }
    //                else
    //                {
    //                    strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and b.column0<'{2}'", TableNameFrom, lblFolderId.Text.ToString(), "0" + (Convert.ToInt32(lblllItemNo.Text.ToString()) + 1)));
    //                }
    //                lblPageNo.Text = "0" + " - " + strlastCount;
    //            }
    //            else
    //            {
    //                lblPageNo.Text = (Convert.ToInt32(strPageCount)) + " - " + strlastCount;
    //            }
    //        }
    //        else
    //        {

    //            if (strPageCount == "")
    //            {
    //                if (lblFolderId.Text.ToString() == "")
    //                {
    //                    strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and b.column0<'{2}'", TableNameFrom, strFolderID, "0" + (Convert.ToInt32(lblllItemNo.Text.ToString()) + 1)));
    //                }
    //                else
    //                {
    //                    strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and b.column0<'{2}'", TableNameFrom, lblFolderId.Text.ToString(), "0" + (Convert.ToInt32(lblllItemNo.Text.ToString()) + 1)));
    //                }
    //                lblPageNo.Text = "1" + " - " + strlastCount;
    //            }
    //            else
    //            {
    //                lblPageNo.Text = (Convert.ToInt32(strPageCount) + 1) + " - " + strlastCount;
    //            }

    //        }
    //        DataTable pageno1 = operation.GetTable4Command(string.Format(@"select distinct pageno from tblfreedraw where fileid='{0}' and userid='{1}'", lblFileId.Text, Session["UserID"].ToString()));
    //        DataTable pageno = operation.GetTable4Command(string.Format(@"select distinct pageno from tblComments where fileid='{0}' and Commentby_id='{1}'", lblFileId.Text, Session["UserID"].ToString()));
    //        DataTable pageno2 = operation.GetTable4Command(string.Format(@"select distinct pageno from tblhighliter where fileid='{0}' and userid='{1}'", lblFileId.Text, Session["UserID"].ToString()));
    //        if (pageno.Rows.Count > 0)
    //        {
    //            for (int j = 0; j < pageno.Rows.Count; j++)
    //            {
    //                if (strPageCount == "")
    //                {
    //                    ReturnStr += Convert.ToInt32(0) + Convert.ToInt32(pageno.Rows[j]["pageno"].ToString()) + ",";
    //                }
    //                else
    //                {
    //                    ReturnStr += Convert.ToInt32(strPageCount) + Convert.ToInt32(pageno.Rows[j]["pageno"].ToString()) + ",";
    //                }
    //            }
    //        }

    //        dtattachment = operation.GetTable4Command(string.Format(@"select a.fileid,a.Folderid,b.attachmentid,c.Column0 from tblfile a inner join tblattachment b on a.fileid=b.fileid inner join {0} c on b.attachmentid=c.fileid where a.fileid='{1}'", TableNameFrom, lblFileId.Text));
    //        for (int s = 0; s < dtattachment.Rows.Count; s++)
    //        {
    //            string strCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and b.column0<'{2}'", TableNameFrom, dtattachment.Rows[s]["FolderID"].ToString(), dtattachment.Rows[s]["Column0"].ToString()));
    //            DataTable dtattachmentpageno = operation.GetTable4Command(string.Format(@"select distinct pageno from tblcomments where fileid='{0}' and Commentby_id='{1}'", dtattachment.Rows[s]["attachmentid"].ToString(), Session["UserID"].ToString()));
    //            for (int c = 0; c < dtattachmentpageno.Rows.Count; c++)
    //            {
    //                ReturnStr += Convert.ToInt32(strCount) + Convert.ToInt32(dtattachmentpageno.Rows[c]["pageno"].ToString()) + ",";
    //            }

    //        }
    //        //string strrrItemID = operation.ExecuteScalar4Command(string.Format(@"select Column0 from {0} where fileid={1}", TableNameFrom, strfileId));

    //        if (pageno1.Rows.Count > 0)
    //        {
    //            for (int j = 0; j < pageno1.Rows.Count; j++)
    //            {
    //                if (strPageCount == "")
    //                {
    //                    ReturnStr += Convert.ToInt32(0) + Convert.ToInt32(pageno1.Rows[j]["pageno"].ToString()) + ",";
    //                }
    //                else
    //                {
    //                    ReturnStr += Convert.ToInt32(strPageCount) + Convert.ToInt32(pageno1.Rows[j]["pageno"].ToString()) + ",";
    //                }
    //                //ReturnStr += Convert.ToInt32(strPageCount) + pageno1.Rows[j]["pageno"].ToString() + ",";
    //            }
    //        }
    //        //TableNameFrom = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Session["FolderID"]));
    //        dtattachment = operation.GetTable4Command(string.Format(@"select a.fileid,a.Folderid,b.attachmentid,c.Column0 from tblfile a inner join tblattachment b on a.fileid=b.fileid inner join {0} c on b.attachmentid=c.fileid where a.fileid='{1}'", TableNameFrom, lblFileId.Text));
    //        for (int s = 0; s < dtattachment.Rows.Count; s++)
    //        {
    //            string strCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and b.column0<'{2}'", TableNameFrom, dtattachment.Rows[s]["FolderID"].ToString(), dtattachment.Rows[s]["Column0"].ToString()));
    //            DataTable dtattachmentpageno = operation.GetTable4Command(string.Format(@"select distinct pageno from tblfreedraw where fileid='{0}' and userid='{1}'", dtattachment.Rows[s]["attachmentid"].ToString(), Session["UserID"].ToString()));
    //            for (int c = 0; c < dtattachmentpageno.Rows.Count; c++)
    //            {
    //                ReturnStr += Convert.ToInt32(strCount) + Convert.ToInt32(dtattachmentpageno.Rows[c]["pageno"].ToString()) + ",";
    //            }

    //        }

    //        if (pageno2.Rows.Count > 0)
    //        {
    //            for (int j = 0; j < pageno2.Rows.Count; j++)
    //            {
    //                if (strPageCount == "")
    //                {
    //                    ReturnStr += Convert.ToInt32(0) + Convert.ToInt32(pageno2.Rows[j]["pageno"].ToString()) + ",";
    //                }
    //                else
    //                {
    //                    ReturnStr += Convert.ToInt32(strPageCount) + Convert.ToInt32(pageno2.Rows[j]["pageno"].ToString()) + ",";
    //                }
    //                //ReturnStr += Convert.ToInt32(strPageCount) + pageno2.Rows[j]["pageno"].ToString() + ",";
    //            }
    //        }
    //        //TableNameFrom = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Session["FolderID"]));
    //        dtattachment = operation.GetTable4Command(string.Format(@"select a.fileid,a.Folderid,b.attachmentid,c.Column0 from tblfile a inner join tblattachment b on a.fileid=b.fileid inner join {0} c on b.attachmentid=c.fileid where a.fileid='{1}'", TableNameFrom, lblFileId.Text));
    //        for (int s = 0; s < dtattachment.Rows.Count; s++)
    //        {
    //            string strCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and b.column0<'{2}'", TableNameFrom, dtattachment.Rows[s]["FolderID"].ToString(), dtattachment.Rows[s]["Column0"].ToString()));
    //            DataTable dtattachmentpageno = operation.GetTable4Command(string.Format(@"select distinct pageno from tblhighliter where fileid='{0}' and userid='{1}'", dtattachment.Rows[s]["attachmentid"].ToString(), Session["UserID"].ToString()));
    //            for (int c = 0; c < dtattachmentpageno.Rows.Count; c++)
    //            {
    //                ReturnStr += Convert.ToInt32(strCount) + Convert.ToInt32(dtattachmentpageno.Rows[c]["pageno"].ToString()) + ",";
    //            }

    //        }

    //        string temp = string.Join(",", ReturnStr.Split(',').Distinct().ToArray());
    //        if (temp != "")
    //        {
    //            string strarray = "";
    //            IEnumerable<int> a = StringToIntList(temp);
    //            int[] b = a.ToArray();
    //            Array.Sort(b);
    //            for (int d = 0; d < b.Count(); d++)
    //            {
    //                strarray += b[d] + ",";
    //            }
    //            DataTable dt = new DataTable();
    //            dt.Columns.Add("Name");
    //            for (int i = 0; i < b.Length; i++)
    //            {
    //                dt.Rows.Add();
    //                dt.Rows[i]["Name"] = b[i].ToString();
    //            }
    //            if (b.Length != 0)
    //            {
    //                GridViewnew.DataSource = dt;
    //                GridViewnew.DataBind();
    //            }

    //            //subject.Text = strarray.Remove(strarray.Length - 1, 1);
    //        }
    //        else
    //        {
    //            GridViewnew.DataSource = null;
    //            GridViewnew.DataBind();
    //        }

    //    }
    //}

    //with indexing( attachment at gvData)
    static int o = 0;
    protected void gvData_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Header && (Session["FolderName"].ToString().ToLower() == "meetings\\my briefcase" || Session["FolderName"].ToString().ToLower() == "meetings\\company info" || Session["FolderName"].ToString().ToLower() == "meetings\\shared documents"))
        {
            e.Row.Cells[5].Text = "Date";

        }
        if (e.Row.RowType == DataControlRowType.Header && Session["GroupName"].ToString().ToLower() == "directors")
        {
            e.Row.Cells[6].Visible = false;
        }

        if (e.Row.RowType == DataControlRowType.DataRow)
        {

            string TableNameFrom = "";
            DataTable dtattachment;
            string ReturnStr = "";
            string strPageCount = "";
            string strlastCount = "";
            if ((e.Row.RowState & DataControlRowState.Edit) == DataControlRowState.Edit && (Session["FolderName"].ToString().ToLower() == "meetings\\my briefcase" || Session["FolderName"].ToString().ToLower() == "meetings\\company info" || Session["FolderName"].ToString().ToLower() == "meetings\\shared documents"))
            {
                TextBox txt = (TextBox)e.Row.FindControl("txtRemark");
                txt.Enabled = false;

            }

            if ((e.Row.RowState & DataControlRowState.Edit) == DataControlRowState.Edit && Request.QueryString["Value"] != null && Request.QueryString["Value"] != "meeting")
            {
                if (Request.QueryString["Value"].ToString().ToLower() == "my briefcase")
                {
                    TextBox txt = (TextBox)e.Row.FindControl("txtRemark");
                    txt.Enabled = false;


                }
                else
                    if (Request.QueryString["Value"].ToString().ToLower() == "company info")
                    {
                        TextBox txt = (TextBox)e.Row.FindControl("txtRemark");
                        txt.Enabled = false;
                    }
            }

            HtmlTableRow tr = (HtmlTableRow)e.Row.FindControl("listviewRow");
            Label approv = (Label)e.Row.FindControl("lblApprovalstatus");
            CheckBox chSelect = (CheckBox)e.Row.FindControl("chkSelect");
            LinkButton lnkApprove = (LinkButton)e.Row.FindControl("LnkApprove");
            LinkButton lnkDownload = (LinkButton)e.Row.FindControl("lnkDownload");
            //LinkButton lnkFileName = (LinkButton)i.FindControl("lnkFileName");
            Label lnkFileName = (Label)e.Row.FindControl("lnkFileName");
            //Label FileSize = (Label)i.FindControl("lblFileSize");
            Label itemno = (Label)e.Row.FindControl("lnkFileName");
            Label lotno = (Label)e.Row.FindControl("lbllotno");
            Label subject = (Label)e.Row.FindControl("lblsubject");
            Label withdrawcomments = (Label)e.Row.FindControl("lblWithdrawComments");
            Label meetingdate = (Label)e.Row.FindControl("lblmeettingdate");
            LinkButton lnkEdit = (LinkButton)e.Row.FindControl("lnkEdit");
            LinkButton lnkOpen = (LinkButton)e.Row.FindControl("lnkOpen");
            LinkButton lnkOpen1 = (LinkButton)e.Row.FindControl("lnkOpen1");
            Label lblFileId = (Label)e.Row.FindControl("lblFileId");
            Label lblviewstaus = (Label)e.Row.FindControl("lblviewstaus");
            Label lblComment = (Label)e.Row.FindControl("lblComment");
            Label lblComment1 = (Label)e.Row.FindControl("lblComment1");
            Label lblComment2 = (Label)e.Row.FindControl("lblComment2");
            Label lblComment3 = (Label)e.Row.FindControl("lblComment3");
            Label lblComment4 = (Label)e.Row.FindControl("lblComment4");
            Label lblComment5 = (Label)e.Row.FindControl("lblComment5");
            Label lblFolderId = (Label)e.Row.FindControl("lblFolderId");
            Label lblllItemNo = (Label)e.Row.FindControl("lblllItemNo");
            Label lblPageNo = (Label)e.Row.FindControl("lblPageNo");
            DropDownList ddlComment = (DropDownList)e.Row.FindControl("ddlComment");
            Label lblRemarks = (Label)e.Row.FindControl("lblRemark");
            ListView GridViewnew = e.Row.FindControl("GridViewnew2") as ListView;
            string strFolderID = Session["FolderID"].ToString();
            string subItemNo = "";
            DataTable dtGetNoteAttachmentDetails = new DataTable();

            if (Session["GroupName"].ToString().ToLower() == "directors")
            {
                if (lblRemarks.Text != "")
                {
                    o++;
                }
                else
                {
                    e.Row.Cells[6].Visible = false;
                }
            }
            if (lblllItemNo.Text != "")
            {
                subItemNo = lblllItemNo.Text.Substring(0, 2);


                if (Request.QueryString["Value"] != null && Request.QueryString["Value"] != "meeting")
                {
                    if (Request.QueryString["Value"].ToString().ToLower() == "my briefcase")
                    {
                        string FolderIDs = operation.ExecuteScalar4Command(string.Format(@"select Folderid from tblfolder where foldername='my briefcase' and deletestatus!=1"));
                        TableNameFrom = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Convert.ToInt32(FolderIDs)));
                        strFolderID = FolderIDs;
                    }
                    else
                        if (Request.QueryString["Value"].ToString().ToLower() == "company info")
                        {
                            string FolderIDs = operation.ExecuteScalar4Command(string.Format(@"select Folderid from tblfolder where foldername='company info' and deletestatus!=1"));
                            TableNameFrom = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Convert.ToInt32(FolderIDs)));
                            strFolderID = FolderIDs;
                        }
                }
                else
                {
                    TableNameFrom = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Session["FolderID"]));
                }
                string strQuery = string.Format(@"select a.fileid,a.column0,isnull(column1,column2) as column1 ,b.PageCount"
                               + ",(select isnull(sum(PageCount),0) + 1 from  {0} XX inner join tblfile YY on XX.fileid=YY.fileid where XX.FolderID={1} and XX.column0 <=a.column0) as 'StartPageNo'"
                               + " ,CASE   WHEN Len(Column1) > 2 THEN 'N'   ELSE 'A' END as Type "
                               + " from {0} as a inner join tblfile b on a.fileid=b.fileid where a.FolderID={1} and column0='{2}' order by column0", TableNameFrom, strFolderID, lblllItemNo.Text.ToString());


                dtGetNoteAttachmentDetails = operation.GetTable4Command(strQuery);
            }
            string ShowAttachmentGridviewNotebook = operation.ExecuteScalar4Command(string.Format(@"select Value from tblConfig where keys='Show Attachment'"));
            if (lblllItemNo.Text != "")
            {
                if (lblllItemNo.Text.Length >= 3)
                {
                    e.Row.Cells[1].Text = "";
                    e.Row.Cells[2].Text = "";
                }
            }



            //TableNameFrom = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Session["FolderID"]));
            //if (Session["GroupName"].ToString().ToLower().Trim() == "board secretariat user" || Session["Designation"].ToString().ToLower() == "board secretary" || Session["GroupName"].ToString().ToLower() == "admin" || Session["Designation"].ToString().ToLower() == "board secretary" || Session["GroupName"].ToString().ToLower() == "admin" || Session["GroupName"].ToString().ToLower().Trim() == "president")
            //{
            if (lblFolderId.Text.ToString() == "")
            {
                strPageCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}  and b.column0<'{2}'", TableNameFrom, strFolderID, lblllItemNo.Text.ToString()));
            }
            else
            {
                strPageCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}  and b.column0<'{2}'", TableNameFrom, lblFolderId.Text.ToString(), lblllItemNo.Text.ToString()));
            }
            //strPageCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}  and b.column0<'{2}'", TableNameFrom, lblFolderId.Text.ToString(), lblllItemNo.Text.ToString()));
            if (Convert.ToInt32(subItemNo) > 08)
            {
                if (lblFolderId.Text.ToString() == "")
                {
                    strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and b.column0<'{2}'", TableNameFrom, strFolderID, (Convert.ToInt32(subItemNo) + 1)));
                }
                else
                {

                    strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and b.column0<'{2}'", TableNameFrom, lblFolderId.Text.ToString(), (Convert.ToInt32(subItemNo) + 1)));
                }
            }
            else
            {
                if (lblFolderId.Text.ToString() == "")
                {
                    strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}and b.column0<'{2}'", TableNameFrom, strFolderID, "0" + (Convert.ToInt32(subItemNo) + 1)));
                }
                else
                {
                    strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}and b.column0<'{2}'", TableNameFrom, lblFolderId.Text.ToString(), "0" + (Convert.ToInt32(subItemNo) + 1)));
                }

            }
            if (strPageCount == "")
            {
                if (lblFolderId.Text.ToString() == "")
                {
                    strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}  and b.column0<'{2}'", TableNameFrom, strFolderID, "0" + (Convert.ToInt32(subItemNo) + 1)));
                }
                else
                {
                    strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}  and b.column0<'{2}'", TableNameFrom, lblFolderId.Text.ToString(), "0" + (Convert.ToInt32(subItemNo) + 1)));

                }
                //lblPageNo.Text = "1" + " - " + strlastCount;
                if (ShowAttachmentGridviewNotebook.ToLower() == "yes")
                {
                    lblPageNo.Text = "1" + " - " + dtGetNoteAttachmentDetails.Rows[0]["PageCount"].ToString();
                }
                else
                {
                    lblPageNo.Text = "1" + " - " + strlastCount;
                }
                // lblPageNo.Text = "1" + " - " + dtGetNoteAttachmentDetails.Rows[0]["PageCount"].ToString();
            }
            else
            {
                //lblPageNo.Text = (Convert.ToInt32(strPageCount) + 1) + " - " + strlastCount;
                // lblPageNo.Text = (Convert.ToInt32(strPageCount) + 1) + " - " + (Convert.ToInt32(dtGetNoteAttachmentDetails.Rows[0]["StartPageNo"].ToString()) - 1);

                if (ShowAttachmentGridviewNotebook.ToLower() == "yes")
                {
                    lblPageNo.Text = (Convert.ToInt32(strPageCount) + 1) + " - " + (Convert.ToInt32(dtGetNoteAttachmentDetails.Rows[0]["StartPageNo"].ToString()) - 1);
                }
                else
                {
                    lblPageNo.Text = (Convert.ToInt32(strPageCount) + 1) + " - " + strlastCount;
                }
            }
            DataTable pageno1 = operation.GetTable4Command(string.Format(@"select distinct pageno from tblfreedraw where fileid='{0}' and Drawingfor_id='{1}'", lblFileId.Text, Session["UserID"].ToString()));
            DataTable pageno = operation.GetTable4Command(string.Format(@"select distinct pageno from tblComments where fileid='{0}' and Commentfor_id='{1}'", lblFileId.Text, Session["UserID"].ToString()));
            DataTable pageno2 = operation.GetTable4Command(string.Format(@"select distinct pageno from tblhighliter where fileid='{0}' and Highlitefor_id='{1}'", lblFileId.Text, Session["UserID"].ToString()));
            if (pageno.Rows.Count > 0)
            {
                for (int j = 0; j < pageno.Rows.Count; j++)
                {
                    if (strPageCount == "")
                    {
                        ReturnStr += Convert.ToInt32(0) + Convert.ToInt32(pageno.Rows[j]["pageno"].ToString()) + ",";
                    }
                    else
                    {
                        ReturnStr += Convert.ToInt32(strPageCount) + Convert.ToInt32(pageno.Rows[j]["pageno"].ToString()) + ",";
                    }
                }
            }
            if (ShowAttachmentGridviewNotebook.ToLower() != "yes")
            {
                dtattachment = operation.GetTable4Command(string.Format(@"select a.fileid,a.Folderid,b.attachmentid,c.Column0 from tblfile a inner join tblattachment b on a.fileid=b.fileid inner join {0} c on b.attachmentid=c.fileid where a.fileid='{1}'", TableNameFrom, lblFileId.Text));
                for (int s = 0; s < dtattachment.Rows.Count; s++)
                {
                    string strCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and b.column0<'{2}'", TableNameFrom, dtattachment.Rows[s]["FolderID"].ToString(), dtattachment.Rows[s]["Column0"].ToString()));
                    DataTable dtattachmentpageno = operation.GetTable4Command(string.Format(@"select distinct pageno from tblcomments where fileid='{0}' and Commentby_id='{1}'", dtattachment.Rows[s]["attachmentid"].ToString(), Session["UserID"].ToString()));
                    for (int c = 0; c < dtattachmentpageno.Rows.Count; c++)
                    {
                        ReturnStr += Convert.ToInt32(strCount) + Convert.ToInt32(dtattachmentpageno.Rows[c]["pageno"].ToString()) + ",";
                    }

                }
            }


            if (pageno1.Rows.Count > 0)
            {
                for (int j = 0; j < pageno1.Rows.Count; j++)
                {
                    if (strPageCount == "")
                    {
                        ReturnStr += Convert.ToInt32(0) + Convert.ToInt32(pageno1.Rows[j]["pageno"].ToString()) + ",";
                    }
                    else
                    {
                        ReturnStr += Convert.ToInt32(strPageCount) + Convert.ToInt32(pageno1.Rows[j]["pageno"].ToString()) + ",";
                    }

                }
            }
            if (ShowAttachmentGridviewNotebook.ToLower() != "yes")
            {
                dtattachment = operation.GetTable4Command(string.Format(@"select a.fileid,a.Folderid,b.attachmentid,c.Column0 from tblfile a inner join tblattachment b on a.fileid=b.fileid inner join {0} c on b.attachmentid=c.fileid where a.fileid='{1}' ", TableNameFrom, lblFileId.Text));
                for (int s = 0; s < dtattachment.Rows.Count; s++)
                {
                    string strCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and b.column0<'{2}'", TableNameFrom, dtattachment.Rows[s]["FolderID"].ToString(), dtattachment.Rows[s]["Column0"].ToString()));
                    DataTable dtattachmentpageno = operation.GetTable4Command(string.Format(@"select distinct pageno from tblfreedraw where fileid='{0}' and userid='{1}'", dtattachment.Rows[s]["attachmentid"].ToString(), Session["UserID"].ToString()));
                    for (int c = 0; c < dtattachmentpageno.Rows.Count; c++)
                    {
                        ReturnStr += Convert.ToInt32(strCount) + Convert.ToInt32(dtattachmentpageno.Rows[c]["pageno"].ToString()) + ",";
                    }

                }
            }

            if (pageno2.Rows.Count > 0)
            {
                for (int j = 0; j < pageno2.Rows.Count; j++)
                {
                    if (strPageCount == "")
                    {
                        ReturnStr += Convert.ToInt32(0) + Convert.ToInt32(pageno2.Rows[j]["pageno"].ToString()) + ",";
                    }
                    else
                    {
                        ReturnStr += Convert.ToInt32(strPageCount) + Convert.ToInt32(pageno2.Rows[j]["pageno"].ToString()) + ",";
                    }

                }
            }
            if (ShowAttachmentGridviewNotebook.ToLower() != "yes")
            {
                dtattachment = operation.GetTable4Command(string.Format(@"select a.fileid,a.Folderid,b.attachmentid,c.Column0 from tblfile a inner join tblattachment b on a.fileid=b.fileid inner join {0} c on b.attachmentid=c.fileid where a.fileid='{1}' ", TableNameFrom, lblFileId.Text));
                for (int s = 0; s < dtattachment.Rows.Count; s++)
                {
                    string strCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}  and b.column0<'{2}'", TableNameFrom, dtattachment.Rows[s]["FolderID"].ToString(), dtattachment.Rows[s]["Column0"].ToString()));
                    DataTable dtattachmentpageno = operation.GetTable4Command(string.Format(@"select distinct pageno from tblhighliter where fileid='{0}' and userid='{1}'", dtattachment.Rows[s]["attachmentid"].ToString(), Session["UserID"].ToString()));
                    for (int c = 0; c < dtattachmentpageno.Rows.Count; c++)
                    {
                        ReturnStr += Convert.ToInt32(strCount) + Convert.ToInt32(dtattachmentpageno.Rows[c]["pageno"].ToString()) + ",";
                    }

                }
            }
            //}
            //else
            //{
            //    if (lblFolderId.Text.ToString() == "")
            //    {

            //        strPageCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and (a.approvalstatus= 1 or a.approvalstatus= 2) and b.column0<'{2}'", TableNameFrom, strFolderID, lblllItemNo.Text.ToString()));
            //    }
            //    else
            //    {
            //        strPageCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and (a.approvalstatus= 1 or a.approvalstatus= 2) and b.column0<'{2}'", TableNameFrom, lblFolderId.Text.ToString(), lblllItemNo.Text.ToString()));
            //    }
            //    if (Convert.ToInt32(subItemNo) > 08)
            //    {
            //        if (lblFolderId.Text.ToString() == "")
            //        {

            //            strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and (a.approvalstatus= 1 or a.approvalstatus= 2) and b.column0<'{2}'", TableNameFrom, strFolderID, "0" + (Convert.ToInt32(subItemNo) + 1)));
            //        }
            //        else
            //        {
            //            strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and (a.approvalstatus= 1 or a.approvalstatus= 2) and b.column0<'{2}'", TableNameFrom, lblFolderId.Text.ToString(), "0" + (Convert.ToInt32(subItemNo) + 1)));
            //        }
            //    }
            //    else
            //    {
            //        if (lblFolderId.Text.ToString() == "")
            //        {
            //            strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and (a.approvalstatus= 1 or a.approvalstatus= 2) and b.column0<'{2}'", TableNameFrom, strFolderID, "0" + (Convert.ToInt32(subItemNo) + 1)));
            //        }
            //        else
            //        {
            //            strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and (a.approvalstatus= 1 or a.approvalstatus= 2) and b.column0<'{2}'", TableNameFrom, lblFolderId.Text.ToString(), "0" + (Convert.ToInt32(subItemNo) + 1)));
            //        }

            //    }
            //    if (strPageCount == "")
            //    {
            //        if (lblFolderId.Text.ToString() == "")
            //        {
            //            strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and (a.approvalstatus= 1 or a.approvalstatus= 2) and b.column0<'{2}'", TableNameFrom, strFolderID, "0" + (Convert.ToInt32(lblllItemNo.Text.ToString()) + 1)));
            //        }
            //        else
            //        {
            //            strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and (a.approvalstatus= 1 or a.approvalstatus= 2) and b.column0<'{2}'", TableNameFrom, lblFolderId.Text.ToString(), "0" + (Convert.ToInt32(lblllItemNo.Text.ToString()) + 1)));
            //        }
            //        lblPageNo.Text = "1" + " - " + dtGetNoteAttachmentDetails.Rows[0]["PageCount"].ToString();

            //    }
            //    else
            //    {
            //        lblPageNo.Text = (Convert.ToInt32(strPageCount) + 1) + " - " + (Convert.ToInt32(dtGetNoteAttachmentDetails.Rows[0]["StartPageNo"].ToString()) - 1);

            //    }
            //    DataTable pageno1 = operation.GetTable4Command(string.Format(@"select distinct pageno from tblfreedraw where fileid='{0}' and userid='{1}'", lblFileId.Text, Session["UserID"].ToString()));
            //    DataTable pageno = operation.GetTable4Command(string.Format(@"select distinct pageno from tblComments where fileid='{0}' and Commentby_id='{1}'", lblFileId.Text, Session["UserID"].ToString()));
            //    DataTable pageno2 = operation.GetTable4Command(string.Format(@"select distinct pageno from tblhighliter where fileid='{0}' and userid='{1}'", lblFileId.Text, Session["UserID"].ToString()));
            //    if (pageno.Rows.Count > 0)
            //    {
            //        for (int j = 0; j < pageno.Rows.Count; j++)
            //        {
            //            if (strPageCount == "")
            //            {
            //                ReturnStr += Convert.ToInt32(0) + Convert.ToInt32(pageno.Rows[j]["pageno"].ToString()) + ",";
            //            }
            //            else
            //            {
            //                ReturnStr += Convert.ToInt32(strPageCount) + Convert.ToInt32(pageno.Rows[j]["pageno"].ToString()) + ",";
            //            }
            //        }
            //    }



            //    if (pageno1.Rows.Count > 0)
            //    {
            //        for (int j = 0; j < pageno1.Rows.Count; j++)
            //        {
            //            if (strPageCount == "")
            //            {
            //                ReturnStr += Convert.ToInt32(0) + Convert.ToInt32(pageno1.Rows[j]["pageno"].ToString()) + ",";
            //            }
            //            else
            //            {
            //                ReturnStr += Convert.ToInt32(strPageCount) + Convert.ToInt32(pageno1.Rows[j]["pageno"].ToString()) + ",";
            //            }

            //        }
            //    }


            //    if (pageno2.Rows.Count > 0)
            //    {
            //        for (int j = 0; j < pageno2.Rows.Count; j++)
            //        {
            //            if (strPageCount == "")
            //            {
            //                ReturnStr += Convert.ToInt32(0) + Convert.ToInt32(pageno2.Rows[j]["pageno"].ToString()) + ",";
            //            }
            //            else
            //            {
            //                ReturnStr += Convert.ToInt32(strPageCount) + Convert.ToInt32(pageno2.Rows[j]["pageno"].ToString()) + ",";
            //            }

            //        }
            //    }
            //}

            string temp = string.Join(",", ReturnStr.Split(',').Distinct().ToArray());
            if (temp != "")
            {
                string strarray = "";
                IEnumerable<int> a = StringToIntList(temp);
                int[] b = a.ToArray();
                Array.Sort(b);
                for (int d = 0; d < b.Count(); d++)
                {
                    strarray += b[d] + ",";
                }
                DataTable dt = new DataTable();
                dt.Columns.Add("Name");
                for (int i = 0; i < b.Length; i++)
                {
                    dt.Rows.Add();
                    dt.Rows[i]["Name"] = b[i].ToString();
                }
                if (b.Length != 0)
                {
                    GridViewnew.DataSource = dt;
                    GridViewnew.DataBind();
                }

                //subject.Text = strarray.Remove(strarray.Length - 1, 1);
            }
            else
            {
                GridViewnew.DataSource = null;
                GridViewnew.DataBind();
            }


        }
        if (e.Row.RowType == DataControlRowType.Header && Session["GroupName"].ToString().ToLower() == "directors")
        {
            if (o != 0)
            {
                e.Row.Cells[6].Visible = true;
            }
        }
    }

    protected void FileGenerateWithWithdrawn(string filePath)
    {
        try
        {
            string watermarkText = "Withdrawn";

            byte[] bytes = File.ReadAllBytes(filePath);
            Font blackFont = FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.BLACK);
            using (MemoryStream stream = new MemoryStream())
            {
                PdfReader reader = new PdfReader(bytes);
                using (PdfStamper stamper = new PdfStamper(reader, stream))
                {
                    PdfLayer layer = new PdfLayer("WatermarkLayer", stamper.Writer);
                    int j = 1;
                    int pages = reader.NumberOfPages;
                    ////int TotalPages = pages - indexPages;
                    for (int k = 1; k <= pages; k++)
                    {
                        PdfContentByte cb = stamper.GetOverContent(k);
                        cb.BeginLayer(layer);
                        cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED), 50);
                        PdfGState gState = new PdfGState();
                        gState.FillOpacity = 0.50f;
                        cb.SetGState(gState);
                        cb.SetColorFill(BaseColor.GRAY);
                        cb.BeginText();
                        cb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, watermarkText, 298, 421, 45f);
                        //ColumnText.ShowTextAligned(stamper.GetOverContent(k), Element.ALIGN_RIGHT, new Phrase("Withdrawn ", blackFont), 200f, 15f, 0);
                        cb.EndText();
                        //"Close" the layer
                        cb.EndLayer();
                    }
                }
                bytes = stream.ToArray();
            }
            File.WriteAllBytes(filePath, bytes);

        }
        catch (Exception es)
        {
        }
    }


    //for without indexing(No attachment at gvData)
    //protected void gvData_RowDataBound(object sender, GridViewRowEventArgs e)
    //{
    //    if (e.Row.RowType == DataControlRowType.DataRow)
    //    {
    //        string TableNameFrom;
    //        DataTable dtattachment;
    //        string ReturnStr = "";
    //        string strPageCount = "";
    //        string strlastCount = "";
    //        HtmlTableRow tr = (HtmlTableRow)e.Row.FindControl("listviewRow");
    //        Label approv = (Label)e.Row.FindControl("lblApprovalstatus");
    //        CheckBox chSelect = (CheckBox)e.Row.FindControl("chkSelect");
    //        LinkButton lnkApprove = (LinkButton)e.Row.FindControl("LnkApprove");
    //        LinkButton lnkDownload = (LinkButton)e.Row.FindControl("lnkDownload");
    //        //LinkButton lnkFileName = (LinkButton)i.FindControl("lnkFileName");
    //        Label lnkFileName = (Label)e.Row.FindControl("lnkFileName");
    //        //Label FileSize = (Label)i.FindControl("lblFileSize");
    //        Label itemno = (Label)e.Row.FindControl("lnkFileName");
    //        Label lotno = (Label)e.Row.FindControl("lbllotno");
    //        Label subject = (Label)e.Row.FindControl("lblsubject");
    //        Label withdrawcomments = (Label)e.Row.FindControl("lblWithdrawComments");
    //        Label meetingdate = (Label)e.Row.FindControl("lblmeettingdate");
    //        LinkButton lnkEdit = (LinkButton)e.Row.FindControl("lnkEdit");
    //        LinkButton lnkOpen = (LinkButton)e.Row.FindControl("lnkOpen");
    //        LinkButton lnkOpen1 = (LinkButton)e.Row.FindControl("lnkOpen1");
    //        Label lblFileId = (Label)e.Row.FindControl("lblFileId");
    //        Label lblviewstaus = (Label)e.Row.FindControl("lblviewstaus");
    //        Label lblComment = (Label)e.Row.FindControl("lblComment");
    //        Label lblComment1 = (Label)e.Row.FindControl("lblComment1");
    //        Label lblComment2 = (Label)e.Row.FindControl("lblComment2");
    //        Label lblComment3 = (Label)e.Row.FindControl("lblComment3");
    //        Label lblComment4 = (Label)e.Row.FindControl("lblComment4");
    //        Label lblComment5 = (Label)e.Row.FindControl("lblComment5");
    //        Label lblFolderId = (Label)e.Row.FindControl("lblFolderId");
    //        Label lblllItemNo = (Label)e.Row.FindControl("lblllItemNo");
    //        Label lblPageNo = (Label)e.Row.FindControl("lblPageNo");
    //        DropDownList ddlComment = (DropDownList)e.Row.FindControl("ddlComment");
    //        ListView GridViewnew = e.Row.FindControl("GridViewnew2") as ListView;
    //        string strFolderID = Session["FolderID"].ToString();
    //        TableNameFrom = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Session["FolderID"]));
    //        if (Session["GroupName"].ToString().ToLower().Trim() == "board secretariat user" || Session["Designation"].ToString().ToLower() == "board secretary" || Session["GroupName"].ToString().ToLower() == "admin" || Session["Designation"].ToString().ToLower() == "board secretary" || Session["GroupName"].ToString().ToLower() == "admin" || Session["GroupName"].ToString().ToLower().Trim() == "president")
    //        {
    //            if (lblFolderId.Text.ToString() == "")
    //            {
    //                strPageCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}  and b.column0<'{2}'", TableNameFrom, strFolderID, lblllItemNo.Text.ToString()));
    //            }
    //            else
    //            {
    //                strPageCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}  and b.column0<'{2}'", TableNameFrom, lblFolderId.Text.ToString(), lblllItemNo.Text.ToString()));
    //            }
    //            //strPageCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}  and b.column0<'{2}'", TableNameFrom, lblFolderId.Text.ToString(), lblllItemNo.Text.ToString()));
    //            if (Convert.ToInt32(lblllItemNo.Text.ToString()) > 08)
    //            {
    //                if (lblFolderId.Text.ToString() == "")
    //                {
    //                    strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and b.column0<'{2}'", TableNameFrom, strFolderID, (Convert.ToInt32(lblllItemNo.Text.ToString()) + 1)));
    //                }
    //                else
    //                {

    //                    strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and b.column0<'{2}'", TableNameFrom, lblFolderId.Text.ToString(), (Convert.ToInt32(lblllItemNo.Text.ToString()) + 1)));
    //                }
    //            }
    //            else
    //            {
    //                if (lblFolderId.Text.ToString() == "")
    //                {
    //                    strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}and b.column0<'{2}'", TableNameFrom, strFolderID, "0" + (Convert.ToInt32(lblllItemNo.Text.ToString()) + 1)));
    //                }
    //                else
    //                {
    //                    strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}and b.column0<'{2}'", TableNameFrom, lblFolderId.Text.ToString(), "0" + (Convert.ToInt32(lblllItemNo.Text.ToString()) + 1)));
    //                }

    //            }
    //            if (strPageCount == "")
    //            {
    //                if (lblFolderId.Text.ToString() == "")
    //                {
    //                    strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}  and b.column0<'{2}'", TableNameFrom, strFolderID, "0" + (Convert.ToInt32(lblllItemNo.Text.ToString()) + 1)));
    //                }
    //                else
    //                {
    //                    strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}  and b.column0<'{2}'", TableNameFrom, lblFolderId.Text.ToString(), "0" + (Convert.ToInt32(lblllItemNo.Text.ToString()) + 1)));

    //                }
    //                lblPageNo.Text = "1" + " - " + strlastCount;
    //            }
    //            else
    //            {
    //                lblPageNo.Text = (Convert.ToInt32(strPageCount) + 1) + " - " + strlastCount;
    //            }
    //            DataTable pageno1 = operation.GetTable4Command(string.Format(@"select distinct pageno from tblfreedraw where fileid='{0}' and userid='{1}'", lblFileId.Text, Session["UserID"].ToString()));
    //            DataTable pageno = operation.GetTable4Command(string.Format(@"select distinct pageno from tblComments where fileid='{0}' and Commentby_id='{1}'", lblFileId.Text, Session["UserID"].ToString()));
    //            DataTable pageno2 = operation.GetTable4Command(string.Format(@"select distinct pageno from tblhighliter where fileid='{0}' and userid='{1}'", lblFileId.Text, Session["UserID"].ToString()));
    //            if (pageno.Rows.Count > 0)
    //            {
    //                for (int j = 0; j < pageno.Rows.Count; j++)
    //                {
    //                    if (strPageCount == "")
    //                    {
    //                        ReturnStr += Convert.ToInt32(0) + Convert.ToInt32(pageno.Rows[j]["pageno"].ToString()) + ",";
    //                    }
    //                    else
    //                    {
    //                        ReturnStr += Convert.ToInt32(strPageCount) + Convert.ToInt32(pageno.Rows[j]["pageno"].ToString()) + ",";
    //                    }
    //                }
    //            }

    //            dtattachment = operation.GetTable4Command(string.Format(@"select a.fileid,a.Folderid,b.attachmentid,c.Column0 from tblfile a inner join tblattachment b on a.fileid=b.fileid inner join {0} c on b.attachmentid=c.fileid where a.fileid='{1}'", TableNameFrom, lblFileId.Text));
    //            for (int s = 0; s < dtattachment.Rows.Count; s++)
    //            {
    //                string strCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and b.column0<'{2}'", TableNameFrom, dtattachment.Rows[s]["FolderID"].ToString(), dtattachment.Rows[s]["Column0"].ToString()));
    //                DataTable dtattachmentpageno = operation.GetTable4Command(string.Format(@"select distinct pageno from tblcomments where fileid='{0}' and Commentby_id='{1}'", dtattachment.Rows[s]["attachmentid"].ToString(), Session["UserID"].ToString()));
    //                for (int c = 0; c < dtattachmentpageno.Rows.Count; c++)
    //                {
    //                    ReturnStr += Convert.ToInt32(strCount) + Convert.ToInt32(dtattachmentpageno.Rows[c]["pageno"].ToString()) + ",";
    //                }

    //            }
    //            //string strrrItemID = operation.ExecuteScalar4Command(string.Format(@"select Column0 from {0} where fileid={1}", TableNameFrom, strfileId));

    //            if (pageno1.Rows.Count > 0)
    //            {
    //                for (int j = 0; j < pageno1.Rows.Count; j++)
    //                {
    //                    if (strPageCount == "")
    //                    {
    //                        ReturnStr += Convert.ToInt32(0) + Convert.ToInt32(pageno1.Rows[j]["pageno"].ToString()) + ",";
    //                    }
    //                    else
    //                    {
    //                        ReturnStr += Convert.ToInt32(strPageCount) + Convert.ToInt32(pageno1.Rows[j]["pageno"].ToString()) + ",";
    //                    }
    //                    //ReturnStr += Convert.ToInt32(strPageCount) + pageno1.Rows[j]["pageno"].ToString() + ",";
    //                }
    //            }
    //            //TableNameFrom = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Session["FolderID"]));
    //            dtattachment = operation.GetTable4Command(string.Format(@"select a.fileid,a.Folderid,b.attachmentid,c.Column0 from tblfile a inner join tblattachment b on a.fileid=b.fileid inner join {0} c on b.attachmentid=c.fileid where a.fileid='{1}' ", TableNameFrom, lblFileId.Text));
    //            for (int s = 0; s < dtattachment.Rows.Count; s++)
    //            {
    //                string strCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and b.column0<'{2}'", TableNameFrom, dtattachment.Rows[s]["FolderID"].ToString(), dtattachment.Rows[s]["Column0"].ToString()));
    //                DataTable dtattachmentpageno = operation.GetTable4Command(string.Format(@"select distinct pageno from tblfreedraw where fileid='{0}' and userid='{1}'", dtattachment.Rows[s]["attachmentid"].ToString(), Session["UserID"].ToString()));
    //                for (int c = 0; c < dtattachmentpageno.Rows.Count; c++)
    //                {
    //                    ReturnStr += Convert.ToInt32(strCount) + Convert.ToInt32(dtattachmentpageno.Rows[c]["pageno"].ToString()) + ",";
    //                }

    //            }

    //            if (pageno2.Rows.Count > 0)
    //            {
    //                for (int j = 0; j < pageno2.Rows.Count; j++)
    //                {
    //                    if (strPageCount == "")
    //                    {
    //                        ReturnStr += Convert.ToInt32(0) + Convert.ToInt32(pageno2.Rows[j]["pageno"].ToString()) + ",";
    //                    }
    //                    else
    //                    {
    //                        ReturnStr += Convert.ToInt32(strPageCount) + Convert.ToInt32(pageno2.Rows[j]["pageno"].ToString()) + ",";
    //                    }
    //                    //ReturnStr += Convert.ToInt32(strPageCount) + pageno2.Rows[j]["pageno"].ToString() + ",";
    //                }
    //            }
    //            //TableNameFrom = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Session["FolderID"]));
    //            dtattachment = operation.GetTable4Command(string.Format(@"select a.fileid,a.Folderid,b.attachmentid,c.Column0 from tblfile a inner join tblattachment b on a.fileid=b.fileid inner join {0} c on b.attachmentid=c.fileid where a.fileid='{1}' ", TableNameFrom, lblFileId.Text));
    //            for (int s = 0; s < dtattachment.Rows.Count; s++)
    //            {
    //                string strCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}  and b.column0<'{2}'", TableNameFrom, dtattachment.Rows[s]["FolderID"].ToString(), dtattachment.Rows[s]["Column0"].ToString()));
    //                DataTable dtattachmentpageno = operation.GetTable4Command(string.Format(@"select distinct pageno from tblhighliter where fileid='{0}' and userid='{1}'", dtattachment.Rows[s]["attachmentid"].ToString(), Session["UserID"].ToString()));
    //                for (int c = 0; c < dtattachmentpageno.Rows.Count; c++)
    //                {
    //                    ReturnStr += Convert.ToInt32(strCount) + Convert.ToInt32(dtattachmentpageno.Rows[c]["pageno"].ToString()) + ",";
    //                }

    //            }
    //        }
    //        else
    //        {
    //            if (lblFolderId.Text.ToString() == "")
    //            {

    //                strPageCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and a.approvalstatus= 1 and b.column0<'{2}'", TableNameFrom, strFolderID, lblllItemNo.Text.ToString()));
    //            }
    //            else
    //            {
    //                strPageCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and a.approvalstatus= 1 and b.column0<'{2}'", TableNameFrom, lblFolderId.Text.ToString(), lblllItemNo.Text.ToString()));
    //            }
    //            if (Convert.ToInt32(lblllItemNo.Text.ToString()) > 08)
    //            {
    //                if (lblFolderId.Text.ToString() == "")
    //                {

    //                    strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and a.approvalstatus= 1 and b.column0<'{2}'", TableNameFrom, strFolderID, (Convert.ToInt32(lblllItemNo.Text.ToString()) + 1)));
    //                }
    //                else
    //                {
    //                    strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and a.approvalstatus= 1 and b.column0<'{2}'", TableNameFrom, lblFolderId.Text.ToString(), (Convert.ToInt32(lblllItemNo.Text.ToString()) + 1)));
    //                }
    //            }
    //            else
    //            {
    //                if (lblFolderId.Text.ToString() == "")
    //                {
    //                    strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}and a.approvalstatus= 1 and b.column0<'{2}'", TableNameFrom, strFolderID, "0" + (Convert.ToInt32(lblllItemNo.Text.ToString()) + 1)));
    //                }
    //                else
    //                {
    //                    strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}and a.approvalstatus= 1 and b.column0<'{2}'", TableNameFrom, lblFolderId.Text.ToString(), "0" + (Convert.ToInt32(lblllItemNo.Text.ToString()) + 1)));
    //                }

    //            }
    //            if (strPageCount == "")
    //            {
    //                if (lblFolderId.Text.ToString() == "")
    //                {
    //                    strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and a.approvalstatus= 1 and b.column0<'{2}'", TableNameFrom, strFolderID, "0" + (Convert.ToInt32(lblllItemNo.Text.ToString()) + 1)));
    //                }
    //                else
    //                {
    //                    strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and a.approvalstatus= 1 and b.column0<'{2}'", TableNameFrom, lblFolderId.Text.ToString(), "0" + (Convert.ToInt32(lblllItemNo.Text.ToString()) + 1)));
    //                }
    //                lblPageNo.Text = "1" + " - " + strlastCount;
    //            }
    //            else
    //            {
    //                lblPageNo.Text = (Convert.ToInt32(strPageCount) + 1) + " - " + strlastCount;
    //            }
    //            DataTable pageno1 = operation.GetTable4Command(string.Format(@"select distinct pageno from tblfreedraw where fileid='{0}' and userid='{1}'", lblFileId.Text, Session["UserID"].ToString()));
    //            DataTable pageno = operation.GetTable4Command(string.Format(@"select distinct pageno from tblComments where fileid='{0}' and Commentby_id='{1}'", lblFileId.Text, Session["UserID"].ToString()));
    //            DataTable pageno2 = operation.GetTable4Command(string.Format(@"select distinct pageno from tblhighliter where fileid='{0}' and userid='{1}'", lblFileId.Text, Session["UserID"].ToString()));
    //            if (pageno.Rows.Count > 0)
    //            {
    //                for (int j = 0; j < pageno.Rows.Count; j++)
    //                {
    //                    if (strPageCount == "")
    //                    {
    //                        ReturnStr += Convert.ToInt32(0) + Convert.ToInt32(pageno.Rows[j]["pageno"].ToString()) + ",";
    //                    }
    //                    else
    //                    {
    //                        ReturnStr += Convert.ToInt32(strPageCount) + Convert.ToInt32(pageno.Rows[j]["pageno"].ToString()) + ",";
    //                    }
    //                }
    //            }

    //            dtattachment = operation.GetTable4Command(string.Format(@"select a.fileid,a.Folderid,b.attachmentid,c.Column0 from tblfile a inner join tblattachment b on a.fileid=b.fileid inner join {0} c on b.attachmentid=c.fileid where a.fileid='{1}'and a.approvalstatus=1", TableNameFrom, lblFileId.Text));
    //            for (int s = 0; s < dtattachment.Rows.Count; s++)
    //            {
    //                string strCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}and a.approvalstatus= 1 and b.column0<'{2}'", TableNameFrom, dtattachment.Rows[s]["FolderID"].ToString(), dtattachment.Rows[s]["Column0"].ToString()));
    //                DataTable dtattachmentpageno = operation.GetTable4Command(string.Format(@"select distinct pageno from tblcomments where fileid='{0}' and Commentby_id='{1}'", dtattachment.Rows[s]["attachmentid"].ToString(), Session["UserID"].ToString()));
    //                for (int c = 0; c < dtattachmentpageno.Rows.Count; c++)
    //                {
    //                    ReturnStr += Convert.ToInt32(strCount) + Convert.ToInt32(dtattachmentpageno.Rows[c]["pageno"].ToString()) + ",";
    //                }

    //            }
    //            //string strrrItemID = operation.ExecuteScalar4Command(string.Format(@"select Column0 from {0} where fileid={1}", TableNameFrom, strfileId));

    //            if (pageno1.Rows.Count > 0)
    //            {
    //                for (int j = 0; j < pageno1.Rows.Count; j++)
    //                {
    //                    if (strPageCount == "")
    //                    {
    //                        ReturnStr += Convert.ToInt32(0) + Convert.ToInt32(pageno1.Rows[j]["pageno"].ToString()) + ",";
    //                    }
    //                    else
    //                    {
    //                        ReturnStr += Convert.ToInt32(strPageCount) + Convert.ToInt32(pageno1.Rows[j]["pageno"].ToString()) + ",";
    //                    }
    //                    //ReturnStr += Convert.ToInt32(strPageCount) + pageno1.Rows[j]["pageno"].ToString() + ",";
    //                }
    //            }
    //            //TableNameFrom = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Session["FolderID"]));
    //            dtattachment = operation.GetTable4Command(string.Format(@"select a.fileid,a.Folderid,b.attachmentid,c.Column0 from tblfile a inner join tblattachment b on a.fileid=b.fileid inner join {0} c on b.attachmentid=c.fileid where a.fileid='{1}' and a.approvalstatus=1", TableNameFrom, lblFileId.Text));
    //            for (int s = 0; s < dtattachment.Rows.Count; s++)
    //            {
    //                string strCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}and a.approvalstatus= 1 and b.column0<'{2}'", TableNameFrom, dtattachment.Rows[s]["FolderID"].ToString(), dtattachment.Rows[s]["Column0"].ToString()));
    //                DataTable dtattachmentpageno = operation.GetTable4Command(string.Format(@"select distinct pageno from tblfreedraw where fileid='{0}' and userid='{1}'", dtattachment.Rows[s]["attachmentid"].ToString(), Session["UserID"].ToString()));
    //                for (int c = 0; c < dtattachmentpageno.Rows.Count; c++)
    //                {
    //                    ReturnStr += Convert.ToInt32(strCount) + Convert.ToInt32(dtattachmentpageno.Rows[c]["pageno"].ToString()) + ",";
    //                }

    //            }

    //            if (pageno2.Rows.Count > 0)
    //            {
    //                for (int j = 0; j < pageno2.Rows.Count; j++)
    //                {
    //                    if (strPageCount == "")
    //                    {
    //                        ReturnStr += Convert.ToInt32(0) + Convert.ToInt32(pageno2.Rows[j]["pageno"].ToString()) + ",";
    //                    }
    //                    else
    //                    {
    //                        ReturnStr += Convert.ToInt32(strPageCount) + Convert.ToInt32(pageno2.Rows[j]["pageno"].ToString()) + ",";
    //                    }
    //                    //ReturnStr += Convert.ToInt32(strPageCount) + pageno2.Rows[j]["pageno"].ToString() + ",";
    //                }
    //            }
    //            //TableNameFrom = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Session["FolderID"]));
    //            dtattachment = operation.GetTable4Command(string.Format(@"select a.fileid,a.Folderid,b.attachmentid,c.Column0 from tblfile a inner join tblattachment b on a.fileid=b.fileid inner join {0} c on b.attachmentid=c.fileid where a.fileid='{1}' and a.approvalstatus=1", TableNameFrom, lblFileId.Text));
    //            for (int s = 0; s < dtattachment.Rows.Count; s++)
    //            {
    //                string strCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and a.approvalstatus= 1 and b.column0<'{2}'", TableNameFrom, dtattachment.Rows[s]["FolderID"].ToString(), dtattachment.Rows[s]["Column0"].ToString()));
    //                DataTable dtattachmentpageno = operation.GetTable4Command(string.Format(@"select distinct pageno from tblhighliter where fileid='{0}' and userid='{1}'", dtattachment.Rows[s]["attachmentid"].ToString(), Session["UserID"].ToString()));
    //                for (int c = 0; c < dtattachmentpageno.Rows.Count; c++)
    //                {
    //                    ReturnStr += Convert.ToInt32(strCount) + Convert.ToInt32(dtattachmentpageno.Rows[c]["pageno"].ToString()) + ",";
    //                }

    //            }

    //        }

    //        string temp = string.Join(",", ReturnStr.Split(',').Distinct().ToArray());
    //        if (temp != "")
    //        {
    //            string strarray = "";
    //            IEnumerable<int> a = StringToIntList(temp);
    //            int[] b = a.ToArray();
    //            Array.Sort(b);
    //            for (int d = 0; d < b.Count(); d++)
    //            {
    //                strarray += b[d] + ",";
    //            }
    //            DataTable dt = new DataTable();
    //            dt.Columns.Add("Name");
    //            for (int i = 0; i < b.Length; i++)
    //            {
    //                dt.Rows.Add();
    //                dt.Rows[i]["Name"] = b[i].ToString();
    //            }
    //            if (b.Length != 0)
    //            {
    //                GridViewnew.DataSource = dt;
    //                GridViewnew.DataBind();
    //            }

    //            //subject.Text = strarray.Remove(strarray.Length - 1, 1);
    //        }
    //        else
    //        {
    //            GridViewnew.DataSource = null;
    //            GridViewnew.DataBind();
    //        }

    //    }
    //}



    //protected void gvData_RowDataBound(object sender, GridViewRowEventArgs e)
    //{
    //    if (e.Row.RowType == DataControlRowType.DataRow)
    //    {
    //        string TableNameFrom;
    //        DataTable dtattachment;
    //        string ReturnStr = "";
    //        string strPageCount = "";
    //        string strlastCount = "";
    //        HtmlTableRow tr = (HtmlTableRow)e.Row.FindControl("listviewRow");
    //        Label approv = (Label)e.Row.FindControl("lblApprovalstatus");
    //        CheckBox chSelect = (CheckBox)e.Row.FindControl("chkSelect");
    //        LinkButton lnkApprove = (LinkButton)e.Row.FindControl("LnkApprove");
    //        LinkButton lnkDownload = (LinkButton)e.Row.FindControl("lnkDownload");
    //        //LinkButton lnkFileName = (LinkButton)i.FindControl("lnkFileName");
    //        Label lnkFileName = (Label)e.Row.FindControl("lnkFileName");
    //        //Label FileSize = (Label)i.FindControl("lblFileSize");
    //        Label itemno = (Label)e.Row.FindControl("lnkFileName");
    //        Label lotno = (Label)e.Row.FindControl("lbllotno");
    //        Label subject = (Label)e.Row.FindControl("lblsubject");
    //        Label withdrawcomments = (Label)e.Row.FindControl("lblWithdrawComments");
    //        Label meetingdate = (Label)e.Row.FindControl("lblmeettingdate");
    //        LinkButton lnkEdit = (LinkButton)e.Row.FindControl("lnkEdit");
    //        LinkButton lnkOpen = (LinkButton)e.Row.FindControl("lnkOpen");
    //        LinkButton lnkOpen1 = (LinkButton)e.Row.FindControl("lnkOpen1");
    //        Label lblFileId = (Label)e.Row.FindControl("lblFileId");
    //        Label lblviewstaus = (Label)e.Row.FindControl("lblviewstaus");
    //        Label lblComment = (Label)e.Row.FindControl("lblComment");
    //        Label lblComment1 = (Label)e.Row.FindControl("lblComment1");
    //        Label lblComment2 = (Label)e.Row.FindControl("lblComment2");
    //        Label lblComment3 = (Label)e.Row.FindControl("lblComment3");
    //        Label lblComment4 = (Label)e.Row.FindControl("lblComment4");
    //        Label lblComment5 = (Label)e.Row.FindControl("lblComment5");
    //        Label lblFolderId = (Label)e.Row.FindControl("lblFolderId");
    //        Label lblllItemNo = (Label)e.Row.FindControl("lblllItemNo");
    //        Label lblPageNo = (Label)e.Row.FindControl("lblPageNo");
    //        DropDownList ddlComment = (DropDownList)e.Row.FindControl("ddlComment");
    //        ListView GridViewnew = e.Row.FindControl("GridViewnew2") as ListView;
    //        string strFolderID = Session["FolderID"].ToString();
    //        string subItemNo = lblllItemNo.Text.Substring(0, 2);
    //        TableNameFrom = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Session["FolderID"]));
    //        string strQuery = string.Format(@"select a.fileid,a.column0,isnull(column1,column2) as column1 ,b.PageCount"
    //                           + ",(select isnull(sum(PageCount),0) + 1 from  {0} XX inner join tblfile YY on XX.fileid=YY.fileid where XX.FolderID={1} and XX.column0 <=a.column0) as 'StartPageNo'"
    //                           + " ,CASE   WHEN Len(Column1) > 2 THEN 'N'   ELSE 'A' END as Type "
    //                           + " from {0} as a inner join tblfile b on a.fileid=b.fileid where a.FolderID={1} and column0='{2}' order by column0", TableNameFrom, lblFolderId.Text.ToString(), lblllItemNo.Text.ToString());


    //        DataTable dtGetNoteAttachmentDetails = operation.GetTable4Command(strQuery);


    //        //TableNameFrom = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Session["FolderID"]));
    //        if (Session["GroupName"].ToString().ToLower().Trim() == "board secretariat user" || Session["Designation"].ToString().ToLower() == "board secretary" || Session["GroupName"].ToString().ToLower() == "admin" || Session["Designation"].ToString().ToLower() == "board secretary" || Session["GroupName"].ToString().ToLower() == "admin" || Session["GroupName"].ToString().ToLower().Trim() == "president")
    //        {
    //            if (lblFolderId.Text.ToString() == "")
    //            {
    //                strPageCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}  and b.column0<'{2}'", TableNameFrom, strFolderID, lblllItemNo.Text.ToString()));
    //            }
    //            else
    //            {
    //                strPageCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}  and b.column0<'{2}'", TableNameFrom, lblFolderId.Text.ToString(), lblllItemNo.Text.ToString()));
    //            }
    //            //strPageCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}  and b.column0<'{2}'", TableNameFrom, lblFolderId.Text.ToString(), lblllItemNo.Text.ToString()));
    //            if (Convert.ToInt32(subItemNo) > 08)
    //            {
    //                if (lblFolderId.Text.ToString() == "")
    //                {
    //                    strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and b.column0<'{2}'", TableNameFrom, strFolderID, (Convert.ToInt32(lblllItemNo.Text.ToString()) + 1)));
    //                }
    //                else
    //                {

    //                    strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and b.column0<'{2}'", TableNameFrom, lblFolderId.Text.ToString(), (Convert.ToInt32(subItemNo) + 1)));
    //                }
    //            }
    //            else
    //            {
    //                if (lblFolderId.Text.ToString() == "")
    //                {
    //                    strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}and b.column0<'{2}'", TableNameFrom, strFolderID, "0" + (Convert.ToInt32(lblllItemNo.Text.ToString()) + 1)));
    //                }
    //                else
    //                {
    //                    strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}and b.column0<'{2}'", TableNameFrom, lblFolderId.Text.ToString(), "0" + (Convert.ToInt32(subItemNo) + 1)));
    //                }

    //            }
    //            if (strPageCount == "")
    //            {
    //                if (lblFolderId.Text.ToString() == "")
    //                {
    //                    strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}  and b.column0<'{2}'", TableNameFrom, strFolderID, "0" + (Convert.ToInt32(lblllItemNo.Text.ToString()) + 1)));
    //                }
    //                else
    //                {
    //                    strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}  and b.column0<'{2}'", TableNameFrom, lblFolderId.Text.ToString(), "0" + (Convert.ToInt32(lblllItemNo.Text.ToString()) + 1)));

    //                }
    //                //lblPageNo.Text = "1" + " - " + strlastCount;
    //                lblPageNo.Text = "1" + " - " + dtGetNoteAttachmentDetails.Rows[0]["PageCount"].ToString();
    //            }
    //            else
    //            {
    //                //lblPageNo.Text = (Convert.ToInt32(strPageCount) + 1) + " - " + strlastCount;
    //                lblPageNo.Text = (Convert.ToInt32(strPageCount) + 1) + " - " + (Convert.ToInt32(dtGetNoteAttachmentDetails.Rows[0]["StartPageNo"].ToString()) - 1);
    //            }
    //            DataTable pageno1 = operation.GetTable4Command(string.Format(@"select distinct pageno from tblfreedraw where fileid='{0}' and userid='{1}'", lblFileId.Text, Session["UserID"].ToString()));
    //            DataTable pageno = operation.GetTable4Command(string.Format(@"select distinct pageno from tblComments where fileid='{0}' and Commentby_id='{1}'", lblFileId.Text, Session["UserID"].ToString()));
    //            DataTable pageno2 = operation.GetTable4Command(string.Format(@"select distinct pageno from tblhighliter where fileid='{0}' and userid='{1}'", lblFileId.Text, Session["UserID"].ToString()));
    //            if (pageno.Rows.Count > 0)
    //            {
    //                for (int j = 0; j < pageno.Rows.Count; j++)
    //                {
    //                    if (strPageCount == "")
    //                    {
    //                        ReturnStr += Convert.ToInt32(0) + Convert.ToInt32(pageno.Rows[j]["pageno"].ToString()) + ",";
    //                    }
    //                    else
    //                    {
    //                        ReturnStr += Convert.ToInt32(strPageCount) + Convert.ToInt32(pageno.Rows[j]["pageno"].ToString()) + ",";
    //                    }
    //                }
    //            }

    //            dtattachment = operation.GetTable4Command(string.Format(@"select a.fileid,a.Folderid,b.attachmentid,c.Column0 from tblfile a inner join tblattachment b on a.fileid=b.fileid inner join {0} c on b.attachmentid=c.fileid where a.fileid='{1}'", TableNameFrom, lblFileId.Text));
    //            for (int s = 0; s < dtattachment.Rows.Count; s++)
    //            {
    //                string strCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and b.column0<'{2}'", TableNameFrom, dtattachment.Rows[s]["FolderID"].ToString(), dtattachment.Rows[s]["Column0"].ToString()));
    //                DataTable dtattachmentpageno = operation.GetTable4Command(string.Format(@"select distinct pageno from tblcomments where fileid='{0}' and Commentby_id='{1}'", dtattachment.Rows[s]["attachmentid"].ToString(), Session["UserID"].ToString()));
    //                for (int c = 0; c < dtattachmentpageno.Rows.Count; c++)
    //                {
    //                    ReturnStr += Convert.ToInt32(strCount) + Convert.ToInt32(dtattachmentpageno.Rows[c]["pageno"].ToString()) + ",";
    //                }

    //            }
    //            //string strrrItemID = operation.ExecuteScalar4Command(string.Format(@"select Column0 from {0} where fileid={1}", TableNameFrom, strfileId));

    //            if (pageno1.Rows.Count > 0)
    //            {
    //                for (int j = 0; j < pageno1.Rows.Count; j++)
    //                {
    //                    if (strPageCount == "")
    //                    {
    //                        ReturnStr += Convert.ToInt32(0) + Convert.ToInt32(pageno1.Rows[j]["pageno"].ToString()) + ",";
    //                    }
    //                    else
    //                    {
    //                        ReturnStr += Convert.ToInt32(strPageCount) + Convert.ToInt32(pageno1.Rows[j]["pageno"].ToString()) + ",";
    //                    }
    //                    //ReturnStr += Convert.ToInt32(strPageCount) + pageno1.Rows[j]["pageno"].ToString() + ",";
    //                }
    //            }
    //            //TableNameFrom = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Session["FolderID"]));
    //            dtattachment = operation.GetTable4Command(string.Format(@"select a.fileid,a.Folderid,b.attachmentid,c.Column0 from tblfile a inner join tblattachment b on a.fileid=b.fileid inner join {0} c on b.attachmentid=c.fileid where a.fileid='{1}' ", TableNameFrom, lblFileId.Text));
    //            for (int s = 0; s < dtattachment.Rows.Count; s++)
    //            {
    //                string strCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and b.column0<'{2}'", TableNameFrom, dtattachment.Rows[s]["FolderID"].ToString(), dtattachment.Rows[s]["Column0"].ToString()));
    //                DataTable dtattachmentpageno = operation.GetTable4Command(string.Format(@"select distinct pageno from tblfreedraw where fileid='{0}' and userid='{1}'", dtattachment.Rows[s]["attachmentid"].ToString(), Session["UserID"].ToString()));
    //                for (int c = 0; c < dtattachmentpageno.Rows.Count; c++)
    //                {
    //                    ReturnStr += Convert.ToInt32(strCount) + Convert.ToInt32(dtattachmentpageno.Rows[c]["pageno"].ToString()) + ",";
    //                }

    //            }

    //            if (pageno2.Rows.Count > 0)
    //            {
    //                for (int j = 0; j < pageno2.Rows.Count; j++)
    //                {
    //                    if (strPageCount == "")
    //                    {
    //                        ReturnStr += Convert.ToInt32(0) + Convert.ToInt32(pageno2.Rows[j]["pageno"].ToString()) + ",";
    //                    }
    //                    else
    //                    {
    //                        ReturnStr += Convert.ToInt32(strPageCount) + Convert.ToInt32(pageno2.Rows[j]["pageno"].ToString()) + ",";
    //                    }
    //                    //ReturnStr += Convert.ToInt32(strPageCount) + pageno2.Rows[j]["pageno"].ToString() + ",";
    //                }
    //            }
    //            //TableNameFrom = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Session["FolderID"]));
    //            dtattachment = operation.GetTable4Command(string.Format(@"select a.fileid,a.Folderid,b.attachmentid,c.Column0 from tblfile a inner join tblattachment b on a.fileid=b.fileid inner join {0} c on b.attachmentid=c.fileid where a.fileid='{1}' ", TableNameFrom, lblFileId.Text));
    //            for (int s = 0; s < dtattachment.Rows.Count; s++)
    //            {
    //                string strCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}  and b.column0<'{2}'", TableNameFrom, dtattachment.Rows[s]["FolderID"].ToString(), dtattachment.Rows[s]["Column0"].ToString()));
    //                DataTable dtattachmentpageno = operation.GetTable4Command(string.Format(@"select distinct pageno from tblhighliter where fileid='{0}' and userid='{1}'", dtattachment.Rows[s]["attachmentid"].ToString(), Session["UserID"].ToString()));
    //                for (int c = 0; c < dtattachmentpageno.Rows.Count; c++)
    //                {
    //                    ReturnStr += Convert.ToInt32(strCount) + Convert.ToInt32(dtattachmentpageno.Rows[c]["pageno"].ToString()) + ",";
    //                }

    //            }
    //        }
    //        else
    //        {
    //            if (lblFolderId.Text.ToString() == "")
    //            {

    //                strPageCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and a.approvalstatus= 1 and b.column0<'{2}'", TableNameFrom, strFolderID, lblllItemNo.Text.ToString()));
    //            }
    //            else
    //            {
    //                strPageCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and a.approvalstatus= 1 and b.column0<'{2}'", TableNameFrom, lblFolderId.Text.ToString(), lblllItemNo.Text.ToString()));
    //            }
    //            if (Convert.ToInt32(subItemNo) > 08)
    //            {
    //                if (lblFolderId.Text.ToString() == "")
    //                {

    //                    strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and a.approvalstatus= 1 and b.column0<'{2}'", TableNameFrom, strFolderID, (Convert.ToInt32(lblllItemNo.Text.ToString()) + 1)));
    //                }
    //                else
    //                {
    //                    strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and a.approvalstatus= 1 and b.column0<'{2}'", TableNameFrom, lblFolderId.Text.ToString(), (Convert.ToInt32(subItemNo) + 1)));
    //                }
    //            }
    //            else
    //            {
    //                if (lblFolderId.Text.ToString() == "")
    //                {
    //                    strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}and a.approvalstatus= 1 and b.column0<'{2}'", TableNameFrom, strFolderID, "0" + (Convert.ToInt32(lblllItemNo.Text.ToString()) + 1)));
    //                }
    //                else
    //                {
    //                    strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}and a.approvalstatus= 1 and b.column0<'{2}'", TableNameFrom, lblFolderId.Text.ToString(), "0" + (Convert.ToInt32(subItemNo) + 1)));
    //                }

    //            }
    //            if (strPageCount == "")
    //            {
    //                if (lblFolderId.Text.ToString() == "")
    //                {
    //                    strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and a.approvalstatus= 1 and b.column0<'{2}'", TableNameFrom, strFolderID, "0" + (Convert.ToInt32(lblllItemNo.Text.ToString()) + 1)));
    //                }
    //                else
    //                {
    //                    strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and a.approvalstatus= 1 and b.column0<'{2}'", TableNameFrom, lblFolderId.Text.ToString(), "0" + (Convert.ToInt32(lblllItemNo.Text.ToString()) + 1)));
    //                }
    //                lblPageNo.Text = "1" + " - " + dtGetNoteAttachmentDetails.Rows[0]["PageCount"].ToString();
    //                //lblPageNo.Text = "1" + " - " + strlastCount;
    //            }
    //            else
    //            {
    //                lblPageNo.Text = (Convert.ToInt32(strPageCount) + 1) + " - " + (Convert.ToInt32(dtGetNoteAttachmentDetails.Rows[0]["StartPageNo"].ToString()) - 1);
    //                //lblPageNo.Text = (Convert.ToInt32(strPageCount) + 1) + " - " + strlastCount;
    //            }
    //            DataTable pageno1 = operation.GetTable4Command(string.Format(@"select distinct pageno from tblfreedraw where fileid='{0}' and userid='{1}'", lblFileId.Text, Session["UserID"].ToString()));
    //            DataTable pageno = operation.GetTable4Command(string.Format(@"select distinct pageno from tblComments where fileid='{0}' and Commentby_id='{1}'", lblFileId.Text, Session["UserID"].ToString()));
    //            DataTable pageno2 = operation.GetTable4Command(string.Format(@"select distinct pageno from tblhighliter where fileid='{0}' and userid='{1}'", lblFileId.Text, Session["UserID"].ToString()));
    //            if (pageno.Rows.Count > 0)
    //            {
    //                for (int j = 0; j < pageno.Rows.Count; j++)
    //                {
    //                    if (strPageCount == "")
    //                    {
    //                        ReturnStr += Convert.ToInt32(0) + Convert.ToInt32(pageno.Rows[j]["pageno"].ToString()) + ",";
    //                    }
    //                    else
    //                    {
    //                        ReturnStr += Convert.ToInt32(strPageCount) + Convert.ToInt32(pageno.Rows[j]["pageno"].ToString()) + ",";
    //                    }
    //                }
    //            }

    //            dtattachment = operation.GetTable4Command(string.Format(@"select a.fileid,a.Folderid,b.attachmentid,c.Column0 from tblfile a inner join tblattachment b on a.fileid=b.fileid inner join {0} c on b.attachmentid=c.fileid where a.fileid='{1}'and a.approvalstatus=1", TableNameFrom, lblFileId.Text));
    //            for (int s = 0; s < dtattachment.Rows.Count; s++)
    //            {
    //                string strCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}and a.approvalstatus= 1 and b.column0<'{2}'", TableNameFrom, dtattachment.Rows[s]["FolderID"].ToString(), dtattachment.Rows[s]["Column0"].ToString()));
    //                DataTable dtattachmentpageno = operation.GetTable4Command(string.Format(@"select distinct pageno from tblcomments where fileid='{0}' and Commentby_id='{1}'", dtattachment.Rows[s]["attachmentid"].ToString(), Session["UserID"].ToString()));
    //                for (int c = 0; c < dtattachmentpageno.Rows.Count; c++)
    //                {
    //                    ReturnStr += Convert.ToInt32(strCount) + Convert.ToInt32(dtattachmentpageno.Rows[c]["pageno"].ToString()) + ",";
    //                }

    //            }
    //            //string strrrItemID = operation.ExecuteScalar4Command(string.Format(@"select Column0 from {0} where fileid={1}", TableNameFrom, strfileId));

    //            if (pageno1.Rows.Count > 0)
    //            {
    //                for (int j = 0; j < pageno1.Rows.Count; j++)
    //                {
    //                    if (strPageCount == "")
    //                    {
    //                        ReturnStr += Convert.ToInt32(0) + Convert.ToInt32(pageno1.Rows[j]["pageno"].ToString()) + ",";
    //                    }
    //                    else
    //                    {
    //                        ReturnStr += Convert.ToInt32(strPageCount) + Convert.ToInt32(pageno1.Rows[j]["pageno"].ToString()) + ",";
    //                    }
    //                    //ReturnStr += Convert.ToInt32(strPageCount) + pageno1.Rows[j]["pageno"].ToString() + ",";
    //                }
    //            }
    //            //TableNameFrom = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Session["FolderID"]));
    //            dtattachment = operation.GetTable4Command(string.Format(@"select a.fileid,a.Folderid,b.attachmentid,c.Column0 from tblfile a inner join tblattachment b on a.fileid=b.fileid inner join {0} c on b.attachmentid=c.fileid where a.fileid='{1}' and a.approvalstatus=1", TableNameFrom, lblFileId.Text));
    //            for (int s = 0; s < dtattachment.Rows.Count; s++)
    //            {
    //                string strCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}and a.approvalstatus= 1 and b.column0<'{2}'", TableNameFrom, dtattachment.Rows[s]["FolderID"].ToString(), dtattachment.Rows[s]["Column0"].ToString()));
    //                DataTable dtattachmentpageno = operation.GetTable4Command(string.Format(@"select distinct pageno from tblfreedraw where fileid='{0}' and userid='{1}'", dtattachment.Rows[s]["attachmentid"].ToString(), Session["UserID"].ToString()));
    //                for (int c = 0; c < dtattachmentpageno.Rows.Count; c++)
    //                {
    //                    ReturnStr += Convert.ToInt32(strCount) + Convert.ToInt32(dtattachmentpageno.Rows[c]["pageno"].ToString()) + ",";
    //                }

    //            }

    //            if (pageno2.Rows.Count > 0)
    //            {
    //                for (int j = 0; j < pageno2.Rows.Count; j++)
    //                {
    //                    if (strPageCount == "")
    //                    {
    //                        ReturnStr += Convert.ToInt32(0) + Convert.ToInt32(pageno2.Rows[j]["pageno"].ToString()) + ",";
    //                    }
    //                    else
    //                    {
    //                        ReturnStr += Convert.ToInt32(strPageCount) + Convert.ToInt32(pageno2.Rows[j]["pageno"].ToString()) + ",";
    //                    }
    //                    //ReturnStr += Convert.ToInt32(strPageCount) + pageno2.Rows[j]["pageno"].ToString() + ",";
    //                }
    //            }
    //            //TableNameFrom = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Session["FolderID"]));
    //            dtattachment = operation.GetTable4Command(string.Format(@"select a.fileid,a.Folderid,b.attachmentid,c.Column0 from tblfile a inner join tblattachment b on a.fileid=b.fileid inner join {0} c on b.attachmentid=c.fileid where a.fileid='{1}' and a.approvalstatus=1", TableNameFrom, lblFileId.Text));
    //            for (int s = 0; s < dtattachment.Rows.Count; s++)
    //            {
    //                string strCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and a.approvalstatus= 1 and b.column0<'{2}'", TableNameFrom, dtattachment.Rows[s]["FolderID"].ToString(), dtattachment.Rows[s]["Column0"].ToString()));
    //                DataTable dtattachmentpageno = operation.GetTable4Command(string.Format(@"select distinct pageno from tblhighliter where fileid='{0}' and userid='{1}'", dtattachment.Rows[s]["attachmentid"].ToString(), Session["UserID"].ToString()));
    //                for (int c = 0; c < dtattachmentpageno.Rows.Count; c++)
    //                {
    //                    ReturnStr += Convert.ToInt32(strCount) + Convert.ToInt32(dtattachmentpageno.Rows[c]["pageno"].ToString()) + ",";
    //                }

    //            }

    //        }

    //        string temp = string.Join(",", ReturnStr.Split(',').Distinct().ToArray());
    //        if (temp != "")
    //        {
    //            string strarray = "";
    //            IEnumerable<int> a = StringToIntList(temp);
    //            int[] b = a.ToArray();
    //            Array.Sort(b);
    //            for (int d = 0; d < b.Count(); d++)
    //            {
    //                strarray += b[d] + ",";
    //            }
    //            DataTable dt = new DataTable();
    //            dt.Columns.Add("Name");
    //            for (int i = 0; i < b.Length; i++)
    //            {
    //                dt.Rows.Add();
    //                dt.Rows[i]["Name"] = b[i].ToString();
    //            }
    //            if (b.Length != 0)
    //            {
    //                GridViewnew.DataSource = dt;
    //                GridViewnew.DataBind();
    //            }

    //            //subject.Text = strarray.Remove(strarray.Length - 1, 1);
    //        }
    //        else
    //        {
    //            GridViewnew.DataSource = null;
    //            GridViewnew.DataBind();
    //        }

    //    }
    //}


    protected void GridViewnew2_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            LinkButton lnkEdit = (LinkButton)e.Item.FindControl("LinkButton1");
            lnkEdit.Text = lnkEdit.Text + ",";
        }
    }

    protected void GridViewnew2_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        if (e.CommandName.ToString() == "deletecomment")
        {
            LinkButton lb = (LinkButton)e.CommandSource;
            ListViewDataItem gvr = (ListViewDataItem)lb.NamingContainer;
            ListView gridview = gvr.NamingContainer as ListView;
            int rowIndex = int.Parse(e.CommandArgument.ToString());
            Response.Redirect("../Viewer/Thumbnailsearch.aspx?id=" + rowIndex);
        }

    }

    //protected void GridViewnew2_RowCommand(object sender, GridViewCommandEventArgs e)
    //{
    //    if (e.CommandName.ToString() == "deletecomment")
    //    {
    //        LinkButton lb = (LinkButton)e.CommandSource;
    //        GridViewRow gvr = (GridViewRow)lb.NamingContainer;
    //        GridView gridview = gvr.NamingContainer as GridView;
    //        int rowIndex = int.Parse(e.CommandArgument.ToString());
    //        //string stuID = gridview.DataKeys[rowIndex].Value.ToString();
    //    }
    //}


    protected void gvData_RowEditing(object sender, GridViewEditEventArgs e)
    {
        gvData.EditIndex = e.NewEditIndex;

        DirectorDataBinding();
    }
    protected void gvData_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        GenericDAL objDAL = new GenericDAL();
        TextBox particulars = (TextBox)gvData.Rows[e.RowIndex].FindControl("txtParticular");
        TextBox txtRemark = (TextBox)gvData.Rows[e.RowIndex].FindControl("txtRemark");
        TextBox purpose = (TextBox)gvData.Rows[e.RowIndex].FindControl("txtComments");
        DropDownList gm = (DropDownList)gvData.Rows[e.RowIndex].FindControl("ddlGM");
        TextBox itemno = (TextBox)gvData.Rows[e.RowIndex].FindControl("txtItemNo555");
        HiddenField hditem = (HiddenField)gvData.Rows[e.RowIndex].FindControl("hditem");
        TextBox txtItemNos = (TextBox)gvData.Rows[e.RowIndex].FindControl("txtItemNos");

        string sttrrrr = itemno.Text;
        //TextBox txtMeetingDate = (TextBox)ListView1.Items[e.ItemIndex].FindControl("txtMeetingDate");
        LinkButton lnkUpdate = (LinkButton)gvData.Rows[e.RowIndex].FindControl("LnkApprove");
        string fileId;
        fileId = lnkUpdate.CommandArgument.ToString();
        string filename = "";
        filename = fileId.Substring(fileId.IndexOf(',') + 1);
        //lnkUpdate.Visible = false;
        int fid = Convert.ToInt32(fileId.Substring(0, fileId.IndexOf(',')));
        string IndexTable = string.Empty;
        string FolderIDs = string.Empty;

        if (Request.QueryString["Value"] != null && Request.QueryString["Value"] != "meeting")
        {
            if (Request.QueryString["Value"].ToString().ToLower() == "my briefcase")
            {
                FolderIDs = operation.ExecuteScalar4Command(string.Format(@"select Folderid from tblfolder where foldername='my briefcase' and deletestatus!=1"));
                Object[] objparam = { Convert.ToInt32(FolderIDs) };
                IndexTable = (string)objDAL.ExecuteScalarString("SPGetIndexTableNameOfFolder", objparam);
            }
            else
                if (Request.QueryString["Value"].ToString().ToLower() == "company info")
                {
                    FolderIDs = operation.ExecuteScalar4Command(string.Format(@"select Folderid from tblfolder where foldername='company info' and deletestatus!=1"));
                    Object[] objparam = { Convert.ToInt32(FolderIDs) };
                    IndexTable = (string)objDAL.ExecuteScalarString("SPGetIndexTableNameOfFolder", objparam);
                }
        }
        else
        {
            Object[] objparam = { Convert.ToInt32(Session["FolderId"]) };
            IndexTable = (string)objDAL.ExecuteScalarString("SPGetIndexTableNameOfFolder", objparam);
        }

        // string query = "Select TableName From tblFolderIndexMaster where Folder_Id="+folderid+"";


        if (itemno.Text.ToString() != "" || hditem.Value.ToString() != "")
        {
            if (hditem.Value.Length == 3)
            {
                string stritem = "";
                string updatequery = "";
                //string accesskey = operation.ExecuteScalar4Command(string.Format(@"select value from   tblconfig where keys='AutoNumber'"));
                //if (accesskey.ToLower() == "yes")
                //{
                //     stritem = particulars.Text.ToString();
                //     updatequery = "UPDATE " + IndexTable + " SET  Column2='" + stritem + "',Column3='" + txtRemark.Text.ToString().Replace("'", "''").Trim() + "',Column5='" + txtItemNos.Text.ToString().Replace("'", "''").Trim() + "' WHERE FILEID=" + fid;
                //}
                //else
                //{
                //     stritem = particulars.Text.ToString();
                //     updatequery = "UPDATE " + IndexTable + " SET  Column2='" + stritem + "',Column3='" + txtRemark.Text.ToString().Replace("'", "''").Trim() + "',Column5='" + txtItemNos.Text.ToString().Replace("'", "''").Trim() + "' WHERE FILEID=" + fid;
                //}
                stritem = particulars.Text.ToString();
                updatequery = "UPDATE " + IndexTable + " SET  Column2='" + stritem + "',Column3='" + txtRemark.Text.ToString().Replace("'", "''").Trim() + "',Column5='" + txtItemNos.Text.ToString().Replace("'", "''").Trim() + "' WHERE FILEID=" + fid;
                int success = objDAL.ExecuteNonQuery(updatequery);
                if (success > 0)
                {

                    ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('Index Updation of " + filename + " is done successfully.')", true);

                }
                else
                {
                    ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('Index Updation of " + filename + " is not done')", true);
                }
            }
            else
            {
                OperationClass objOperationClass = new OperationClass();
                string updatequery = "UPDATE " + IndexTable + " SET  Column0='" + itemno.Text.ToString() + "',Column1='" + particulars.Text.ToString().Replace("'", "''").Trim() + "',Column3='" + txtRemark.Text.ToString().Replace("'", "''").Trim() + "',Column5='" + txtItemNos.Text.ToString().Replace("'", "''").Trim() + "' WHERE FILEID=" + fid;
                int success = objDAL.ExecuteNonQuery(updatequery);
                string strTableName = objOperationClass.ExecuteScalar4Command(string.Format(@"select tablename from tblfolderindexmaster where folder_id='{0}'", Session["FolderID"]));
                DataTable dt = objOperationClass.GetTable4Command(string.Format(@"select substring(Column0,1,2),* from {0} where Column0 like '{1}%' and folderid='{2}'  and column0!='{1}'", strTableName, itemno.Text.ToString(), Session["FolderID"]));
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int j = 0; j < dt.Rows.Count; j++)
                    {
                        string updatequerys = "UPDATE " + IndexTable + " SET  Column5='" + txtItemNos.Text.ToString().Replace("'", "''").Trim() + "' where FileID='" + dt.Rows[j]["FileId"].ToString() + "'";
                        int successs = objDAL.ExecuteNonQuery(updatequerys);
                    }
                }



                if (success > 0)
                {

                    ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('Index Updation of " + filename + " is done successfully.')", true);

                }
                else
                {
                    ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('Index Updation of " + filename + " is not done')", true);
                }

            }

            e.Cancel = true;
            gvData.EditIndex = -1;
            DirectorDataBinding();
        }
        else
        {
            ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('Please Enter Agenda No.')", true);
            itemno.Focus();
        }
    }
    protected void gvData_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        e.Cancel = true;
        gvData.EditIndex = -1;
        DirectorDataBinding();
    }


    #region [btnAddActionItem_Click]
    protected void btnAddActionItem_Click(object sender, EventArgs e)
    {
        try
        {
            //gvData.EditIndex = e.NewEditIndex;
        }
        catch (System.Exception ex)
        {
            Session["Error"] = ex.StackTrace;
        }
        finally
        {
            objCommonBAL = null;
            objFileUploadBAL = null;
        }
    }
    #endregion btnAddActionItem_Click

    #region [lnkView_Click]
    protected void lnkView_Click(object sender, EventArgs e)
    {
        try
        {
            int i = Convert.ToInt16(gvData.SelectedIndex);
            GridViewRow row = gvData.Rows[i];

            LinkButton lnkView = (LinkButton)row.FindControl("lnkOpen");
            //if user does not have file view permission then a message will be displayed. You don't have access to view these files.
            string[] FileNameID = lnkView.CommandArgument.ToString().Split(new Char[] { ',' });

            Session["FileID"] = FileNameID[0].ToString();
            Session["FileName"] = FileNameID[1].ToString();

            Session["Redirect"] = "View";
            if (Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".tif" || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".tiff"
                || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".gif" || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".bmp"
                || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".jpg" || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".jpeg" || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".png")
            {
                Response.Redirect("../Viewer/Thumbnail.aspx", false);
            }
            else if (Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".wmv")
            {
                StringBuilder sbwindow = new StringBuilder();
                sbwindow.Append("window.showModalDialog('../Viewer/WMVViwer.aspx',null,'status:no;dialogTop:300;dialogWidth:1014px;dialogHeight:700px;dialogHide:true;help:no;scroll:no;center:yes');");
                ScriptManager.RegisterClientScriptBlock(this.Page, typeof(UpdatePanel), "VideoViewer", sbwindow.ToString(), true);
                sbwindow = null;
            }
            else if (Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".pdf")
            {
                Response.Redirect("../Viewer/Thumbnail.aspx", false);

            }
            else if (Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".zip" || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".rar" || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".zipx")
            {
                ScriptManager.RegisterStartupScript(this.Page, typeof(UpdatePanel), "msg", "alert('Please download file, then view on your local')", true);
                return;
            }

            else
            {
                StringBuilder sbwindow = new StringBuilder();
                sbwindow.Append("window.open('../Viewer/OfficerViewer.aspx',null,'scrollbars:no;width=1024px, height=800px;help:no;center:yes; resizable=yes');");
                // sbwindow.Append("window.showModelessDialog('Viewer/OfficerViewer.aspx',null,'status:no;dialogTop:300;dialogWidth:1024px;dialogHeight:800px;dialogHide:true;help:no;scroll:yes;center:yes');");
                ScriptManager.RegisterClientScriptBlock(this.Page, typeof(UpdatePanel), "DocumentViewer", sbwindow.ToString(), true);
                sbwindow = null;
            }
        }
        catch (System.Exception ex)
        {
            Session["Error"] = ex.StackTrace;
        }
        finally
        {
            objCommonBAL = null;
            objFileUploadBAL = null;
        }
    }
    #endregion

    #region [lnkDownload_Click]
    protected void lnkDownload_Click(object sender, EventArgs e)
    {
        try
        {
            int i = Convert.ToInt16(gvData.SelectedIndex);
            GridViewRow row = gvData.Rows[i];

            LinkButton lnkDownload = (LinkButton)row.FindControl("lnkDownload");
            string[] FileNameID = lnkDownload.CommandArgument.ToString().Split(new Char[] { ',' });

            Session["FileID"] = FileNameID[0].ToString();
            Session["FileName"] = FileNameID[1].ToString();

            // create objects of class
            objCommonBAL = new CommonBAL();

            //visible lblmessage
            lblMessage.Visible = false;
            lblMessage.Text = "";

            //visible panel first
            Panel4.Visible = false;

            //set folder for save decrypt file.
            string ImageSavingFilePath = Convert.ToString(HttpContext.Current.Server.MapPath("~\\Repository\\Decrypt\\" + HttpContext.Current.Session["UserName"].ToString()));

            //set folder path exported file.
            string ImagesavedFilePath = Convert.ToString(HttpContext.Current.Server.MapPath("~\\ExportFile\\" + Convert.ToString(HttpContext.Current.Session["UserName"]).Trim()));

            //Set zip file name and path
            string ZipFilePath = Convert.ToString(Server.MapPath("~\\ExportFile\\".Trim() + Session["UserName"].ToString().Trim() + "\\" + string.Format("ExportedFile{0:MMM-dd-yyyy_hh-mm-ss}", System.DateTime.Now) + ".zip"));

            //set directory path for delete tepory file.
            string ZipDirectoryPath = Convert.ToString(Server.MapPath("~\\ExportFile\\".Trim() + Session["UserName"].ToString().Trim()));

            //Download();
            // Call methode for export file from database or folder
            string strMessage = objCommonBAL.ExportFileOnButtonClick1(Convert.ToString(Session["FileID"]), ImageSavingFilePath, ImagesavedFilePath, ZipFilePath, ZipDirectoryPath, "");
            if (strMessage.Contains("alert"))
            {
                ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "myScr", strMessage, true);
            }
            else
            {
                FileDownLoad(strMessage);
            }
        }
        catch (System.Exception ex)
        {
            Session["Error"] = ex.StackTrace;
        }
        finally
        {
            objCommonBAL = null;
            objFileUploadBAL = null;
        }
    }
    #endregion

    #region [lnkApprove_Click]
    protected void lnkApprove_Click(object sender, EventArgs e)
    {
        try
        {
            int i = Convert.ToInt16(gvData.SelectedIndex);
            GridViewRow row = gvData.Rows[i];

            LinkButton lnkApprove = (LinkButton)row.FindControl("LnkApprove");

            if (lnkApprove.Text == "Authorize")
            {
                //aprove
                string arg = lnkApprove.CommandArgument.ToString();
                CheckBox ch = (CheckBox)row.FindControl("chkSelect");
                int success = 0;

                int fileId = Convert.ToInt32(arg.Substring(0, arg.IndexOf(',')));
                string filename = arg.Substring(arg.IndexOf(',') + 1);
                if (ch.Checked == true)
                {
                    success = objFileUploadBAL.UpdateFileStatus(fileId, 1);

                }
                if (success == 1)
                {
                    ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('" + filename + " is authorized.')", true);
                    //((LinkButton)e.Item.FindControl("LnkApprove")).EnableViewState = false;
                    ((LinkButton)row.FindControl("LnkApprove")).Text = "Withdraw"; //on 3/6/13
                }

            }
            else if (lnkApprove.Text == "reject")
            {
                //reject
                objCommonBAL = new CommonBAL();

                ViewState["Delete"] = "True";

                lblMessage.Text = "";
                string strFileId = "";

                //get chechbox value
                string arg = "";
                arg = lnkApprove.CommandArgument.ToString();
                strFileId = arg.Substring(0, arg.IndexOf(','));
                if (strFileId != "")
                {
                    //store value for get on get btnyes click.
                    ViewState["strFileId"] = strFileId;

                    lblRplMessage0.Text = "Are you sure you want to reject the file ?";
                    Panel4.Visible = true;
                    btnNO.Focus();
                }
            }
            else if (lnkApprove.Text == "withdraw")
            {
                //withdraw
                int success = 0;
                string arg = lnkApprove.CommandArgument.ToString();
                string filename = arg.Substring(arg.IndexOf(',') + 1);
                int fileId = Convert.ToInt32(arg.Substring(0, arg.IndexOf(',')));
                // Page.RegisterStartupScript("asd", "javascript:WithdrawComments()");
                ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "javascript:WithdrawComments('" + fileId.ToString() + "');", true);
            }



        }
        catch (System.Exception ex)
        {
            Session["Error"] = ex.StackTrace;
        }
        finally
        {
            objCommonBAL = null;
            objFileUploadBAL = null;
        }
    }
    #endregion


    protected void gvData_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        lblMessage.Text = "";
        int intCount = 0;
        try
        {

            objCommonBAL = new CommonBAL();
            objFileUploadBAL = new FileUploadBAL();
            Session["IDS"] = Convert.ToInt32(Session["FolderID"]);
            /*
             *  ConfigurationManager.AppSettings["AccessType"]
             * if AccessType userwise then pass value 1 and groupwise then pass 0 to 
             * CheckAccessRight,GetParentChildFolderId methodes.
             */

            //Get access symbol on basis of folderid of  selected folder of treeview.
            //string Symbol = objCommonBAL.GetSymbolstring(Convert.ToInt32(Session["FolderID"]), Convert.ToInt32(Session["GroupID"]), Convert.ToInt32(Session["UserID"]), ConfigurationManager.AppSettings["AccessType"].ToString().ToLower() == "userwise" ? 1 : 0);
            string Symbol = objCommonBAL.GetSymbolstring(Convert.ToInt32(Session["FolderID"]), Convert.ToInt32(Session["GroupID"]), Convert.ToInt32(Session["UserID"]), ConfigurationManager.AppSettings["AccessType"].ToString().ToLower() == "userwise" ? 1 : 0);

            //if symbol equal to 'L' or 'N' then retrun on page.
            if (Symbol == "L" || Symbol == "N")
            {
                lblMessage.Visible = true;
                lblMessage.Text = "You don't have access to view these files.";
                return;
            }
            GridViewRow editrow = null;
            if (e.CommandName != "Page")
                editrow = (GridViewRow)(((Control)e.CommandSource).NamingContainer);
            //int index = Convert.ToInt32(e.CommandArgument);
            //GridViewRow editrow = (GridViewRow)gvData.Rows[index];
            //Added by Kirti s Loke on 13/5/14
            //To approve files for viewing
            if (e.CommandName == "Approval")
            {

                if (((LinkButton)(e.CommandSource)).Text.ToLower() == "authorize")
                {
                    string arg = e.CommandArgument.ToString();

                    CheckBox ch = (CheckBox)(editrow.FindControl("chkSelect"));
                    int success = 0;
                    Label txtItemNo = (Label)(editrow.FindControl("LinkButton1"));
                    Label lblsubject = (Label)(editrow.FindControl("lblsubject"));
                    HiddenField hditem = (HiddenField)(editrow.FindControl("hditem"));
                    int fileId = Convert.ToInt32(arg.Substring(0, arg.IndexOf(',')));
                    string filename = arg.Substring(arg.IndexOf(',') + 1);
                    //if (ch.Checked == true)
                    //{
                    if (!String.IsNullOrEmpty(hditem.Value))
                    {
                        success = objFileUploadBAL.UpdateFileStatus(fileId, 1);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('Please Enter Agenda No.')", true);

                    }
                    //}
                    if (success >= 1)
                    {
                        ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('" + filename + " is authorized.')", true);
                        //((LinkButton)e.Item.FindControl("LnkApprove")).EnableViewState = false;
                        ((LinkButton)((LinkButton)e.CommandSource).FindControl("LnkApprove")).Text = "Withdraw"; //on 3/6/13
                        ((LinkButton)((LinkButton)e.CommandSource).FindControl("LnkReject")).Visible = true;
                        DirectorDataBinding();
                    }


                }
                else
                    if (((LinkButton)(e.CommandSource)).Text.ToLower() == "| reject")
                    {
                        //int success = 0;
                        string arg = e.CommandArgument.ToString();
                        //string filename = arg.Substring(arg.IndexOf(',') + 1);
                        int fileId = Convert.ToInt32(arg.Substring(0, arg.IndexOf(',')));
                        Session["FileId"] = fileId;

                        //GenericDAL objDAL = new GenericDAL();
                        //success = (int)objDAL.ExecuteNonQuery("Update tblFile set ApprovalStatus='2' where FileId=" + Convert.ToInt32(fileId) + "");
                        //if (success == 1)
                        //{
                        //   // return "true";
                        //  ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('" + filename + " is withdrawn.')", true);


                        //}
                        //else
                        //{
                        //   // return "false";
                        //}
                        //return "false";
                        pncomment.Visible = true;
                        txtcomments.Focus();
                    }
                    else
                        if (((LinkButton)(e.CommandSource)).Text.ToLower() == "submit" || ((LinkButton)(e.CommandSource)).Text.ToLower() == "resubmit")
                        {
                            string arg = e.CommandArgument.ToString();

                            CheckBox ch = (CheckBox)(editrow.FindControl("chkSelect"));
                            int success = 0;
                            Label txtItemNo = (Label)(editrow.FindControl("LinkButton1"));
                            Label lblsubject = (Label)(editrow.FindControl("lblsubject"));
                            HiddenField hditem = (HiddenField)(editrow.FindControl("hditem"));
                            int fileId = Convert.ToInt32(arg.Substring(0, arg.IndexOf(',')));
                            string filename = arg.Substring(arg.IndexOf(',') + 1);
                            //if (ch.Checked == true)
                            //{
                            if (!String.IsNullOrEmpty(hditem.Value))
                            {
                                success = objFileUploadBAL.UpdateFileStatus(fileId, 5);
                            }
                            else
                            {
                                ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('Please Enter Agenda No.')", true);

                            }
                            //}
                            if (success >= 1)
                            {
                                ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('" + filename + " is authorized.')", true);
                                //((LinkButton)e.Item.FindControl("LnkApprove")).EnableViewState = false;
                                ((LinkButton)((LinkButton)e.CommandSource).FindControl("LnkApprove")).Text = "submitted"; //on 3/6/13
                            }
                            DirectorDataBinding();


                        }
                        else if (((LinkButton)(e.CommandSource)).Text.ToLower() == "reject")
                        {
                            objCommonBAL = new CommonBAL();

                            ViewState["Delete"] = "True";

                            lblMessage.Text = "";
                            string strFileId = "";

                            //get chechbox value
                            string arg = "";
                            arg = e.CommandArgument.ToString();
                            strFileId = arg.Substring(0, arg.IndexOf(','));
                            if (strFileId != "")
                            {
                                //store value for get on get btnyes click.
                                ViewState["strFileId"] = strFileId;

                                lblRplMessage0.Text = "Are you sure you want to reject the file ?";
                                Panel4.Visible = true;
                                btnNO.Focus();
                            }
                            //  objCommonBAL = null;
                        }
                        else if (((LinkButton)(e.CommandSource)).Text.ToLower() == "p-withdraw")
                        {

                            int success = 0;
                            string arg = e.CommandArgument.ToString();
                            string filename = arg.Substring(arg.IndexOf(',') + 1);
                            int fileId = Convert.ToInt32(arg.Substring(0, arg.IndexOf(',')));
                            string ItemNo = arg.Substring(arg.LastIndexOf(',') + 1);
                            // Session["FileId"] = fileId;
                            string FolderId = Session["FolderId"].ToString();


                            string strdt = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblfolderindexmaster where folder_id={0}", FolderId));


                            DataTable dt2 = operation.GetTable4Command(string.Format(@"select tblfile.FileID ,tblfile.[FileName],{0}.Column0 from tblfile inner join {0} on tblfile.FileID = {0}.FileId where {0}.FolderId ={1} and  {0}.Column0 like '{2}%' order by {0}.Column0", strdt, FolderId, ItemNo));
                            if (dt2.Rows.Count > 0)
                            {
                                //string[] stin = new string[dt2.Rows.Count + 1];
                                //Session["FileID"] = dt2.Rows[0]["FileID"];
                                //if (Path.GetExtension(dt2.Rows[0]["FileName"].ToString()).ToLower() == ".docx" || Path.GetExtension(dt2.Rows[0]["FileName"].ToString()).ToLower() == ".xlsx" || Path.GetExtension(dt2.Rows[0]["FileName"].ToString()).ToLower() == ".pptx")
                                //{

                                //    Session["FileName"] = Path.GetFileNameWithoutExtension(dt2.Rows[0]["FileName"].ToString()) + ".pdf";
                                //}
                                //else
                                //{
                                //    Session["FileName"] = dt2.Rows[0]["FileName"];
                                //}

                                //int count = dt2.Rows.Count;
                                //string[] strfilename = new string[count];
                                //string[] strFileId = new string[count];
                                //string DecryptFilePath;
                                //DecryptFilePath = Convert.ToString(Server.MapPath("~/Repository//"));

                                for (int i = 0; i <= dt2.Rows.Count - 1; i++)
                                {

                                    GenericDAL objDAL = new GenericDAL();
                                    success = (int)objDAL.ExecuteNonQuery("Update tblFile set ApprovalStatus='2' where FileId=" + Convert.ToInt32(dt2.Rows[i]["FileId"].ToString()) + "");


                                    string[] stin = new string[dt2.Rows.Count + 1];
                                    Session["FileID"] = dt2.Rows[i]["FileID"];
                                    if (Path.GetExtension(dt2.Rows[i]["FileName"].ToString()).ToLower() == ".docx" || Path.GetExtension(dt2.Rows[i]["FileName"].ToString()).ToLower() == ".xlsx" || Path.GetExtension(dt2.Rows[i]["FileName"].ToString()).ToLower() == ".pptx")
                                    {

                                        Session["FileName"] = Path.GetFileNameWithoutExtension(dt2.Rows[i]["FileName"].ToString()) + ".pdf";
                                    }
                                    else
                                    {
                                        Session["FileName"] = dt2.Rows[i]["FileName"];
                                    }

                                    int count = dt2.Rows.Count;
                                    string[] strfilename = new string[count];
                                    string[] strFileId = new string[count];
                                    string DecryptFilePath;
                                    DecryptFilePath = Convert.ToString(Server.MapPath("~/Repository//"));
                                    if (Path.GetExtension(dt2.Rows[i]["FileName"].ToString()).ToLower() == ".docx" || Path.GetExtension(dt2.Rows[i]["FileName"].ToString()).ToLower() == ".xlsx" || Path.GetExtension(dt2.Rows[i]["FileName"].ToString()).ToLower() == ".pptx")
                                    {
                                        strfilename[i] = Path.GetFileNameWithoutExtension(dt2.Rows[i]["FileName"].ToString()) + ".pdf";
                                    }
                                    else
                                    {
                                        strfilename[i] = dt2.Rows[i]["FileName"].ToString();
                                    }



                                    strFileId[i] = dt2.Rows[i]["FileID"].ToString();
                                    Session["FileID"] = strFileId[i];
                                    if (Path.GetExtension(strfilename[i]).ToLower() == ".docx" || Path.GetExtension(strfilename[i]).ToLower() == ".xlsx" || Path.GetExtension(strfilename[i]).ToLower() == ".pptx")
                                    {

                                        Session["FileName"] = Path.GetFileNameWithoutExtension(strfilename[i]) + ".pdf";
                                    }
                                    else
                                    {
                                        Session["FileName"] = strfilename[i];
                                    }

                                    string sourceFileName = null;

                                    if (Path.GetExtension(strfilename[i].ToLower()) == ".pdf")
                                    {
                                        string strfileId = Session["FileID"].ToString();
                                        strFileName = Session["FileName"].ToString();
                                        string[] _strFileName = new string[count];
                                        _strFileName[i] = strfilename[i];
                                        sourceFileName = (DecryptFilePath + Path.GetFileNameWithoutExtension(_strFileName[i]) + "_" + (strFileId[i]) + Path.GetExtension(_strFileName[i]));
                                        FileGenerateWithWithdrawn(sourceFileName);


                                    }
                                    string OutPutTiffFileName = "";
                                    OutPutTiffFileName = Path.GetFileNameWithoutExtension(strFileName) + "_" + (strFileId[i]) + ".pdf";
                                    //sourceFileName = (DecryptFilePath + Path.GetFileNameWithoutExtension(_strFileName[i]) + "_" + (strFileId[i]) + Path.GetExtension(_strFileName[i]));

                                    convertPDF_TIFF(sourceFileName, OutPutTiffFileName);

                                }
                                DirectorDataBinding();
                            }


                        }

            }
            if (e.CommandName == "DownLoad")
            {
                string[] FileNameID = e.CommandArgument.ToString().Split(new Char[] { ',' });

                Session["FileID"] = FileNameID[0].ToString();
                Session["FileName"] = FileNameID[1].ToString();
                Session["Itemno"] = FileNameID[2].ToString();

                // create objects of class
                objCommonBAL = new CommonBAL();

                //visible lblmessage
                lblMessage.Visible = false;
                lblMessage.Text = "";

                //visible panel first
                Panel4.Visible = false;

                //set folder for save decrypt file.
                string ImageSavingFilePath = Convert.ToString(HttpContext.Current.Server.MapPath("~\\Repository\\Decrypt\\" + HttpContext.Current.Session["UserName"].ToString()));

                //set folder path exported file.
                string ImagesavedFilePath = Convert.ToString(HttpContext.Current.Server.MapPath("~\\ExportFile\\" + Convert.ToString(HttpContext.Current.Session["UserName"]).Trim()));

                //Set zip file name and path
                string ZipFilePath = Convert.ToString(Server.MapPath("~\\ExportFile\\".Trim() + Session["UserName"].ToString().Trim() + "\\" + string.Format("ExportedFile{0:MMM-dd-yyyy_hh-mm-ss}", System.DateTime.Now) + ".zip"));

                //set directory path for delete tepory file.
                string ZipDirectoryPath = Convert.ToString(Server.MapPath("~\\ExportFile\\".Trim() + Session["UserName"].ToString().Trim()));

                //Download();
                // Call methode for export file from database or folder
                string strMessage = objCommonBAL.ExportFileOnButtonClick1(Convert.ToString(Session["FileID"]), ImageSavingFilePath, ImagesavedFilePath, ZipFilePath, ZipDirectoryPath, "");
                if (strMessage.Contains("alert"))
                {
                    ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "myScr", strMessage, true);
                }
                else
                {
                    PdfFileDownLoad(strMessage);
                    //FileDownLoad(strMessage);
                    //FileDownLoadwithWaterMark(strMessage);

                }
            }
            if (e.CommandName == "View")
            {
                //if user does not have file view permission then a message will be displayed. You don't have access to view these files.
                string[] FileNameID = e.CommandArgument.ToString().Split(new Char[] { ',' });
                Session["WorkFlow"] = null;
                //if (FileNameID[3].ToString().ToLower() == "atr")
                //{
                //    Response.Redirect("../mompreparation/ATR.aspx", false);
                //}
                //else
                //    if (FileNameID[3].ToString().ToLower() == "any other business")
                //    {
                //        Response.Redirect("../mompreparation/Business.aspx", false);
                //    }
                //    else
                //        if (FileNameID[3].ToString().ToLower() == "mom")
                //        {
                //            Response.Redirect("../mompreparation/MOM.aspx", false);
                //        }
                //        else
                //        {
                if (FileNameID[1].ToString().ToLower() != "")
                {
                    Session["FileID"] = FileNameID[0].ToString();
                    Session["FileName"] = FileNameID[1].ToString();
                    Session["Itemno"] = FileNameID[2].ToString();
                    //Session["Particulars"] = FileNameID[3].ToString();
                    Session["PageCount"] = FileNameID[3].ToString();

                    Session["Redirect"] = "View";
                    if (Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".tif" || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".tiff"
                        || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".gif" || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".bmp"
                        || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".jpg" || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".jpeg" || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".png")
                    {
                        Response.Redirect("../Viewer/Thumbnail.aspx", false);
                    }
                    else if (Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".wmv")
                    {
                        StringBuilder sbwindow = new StringBuilder();
                        sbwindow.Append("window.showModalDialog('../Viewer/WMVViwer.aspx',null,'status:no;dialogTop:300;dialogWidth:1014px;dialogHeight:700px;dialogHide:true;help:no;scroll:no;center:yes');");
                        ScriptManager.RegisterClientScriptBlock(this.Page, typeof(UpdatePanel), "VideoViewer", sbwindow.ToString(), true);
                        sbwindow = null;
                    }
                    else if (Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".pdf" || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".docx" || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".pptx" || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".xlsx")
                    {
                        string strPageCount = "";
                        //Response.Redirect("../Viewer/Thumbnail.aspx", false);
                        int sCustCode = 0;
                        string TableNameFrom = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Session["FolderID"]));
                        //string strPageCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and b.column0<'{2}'", TableNameFrom, Session["FolderID"].ToString(), FileNameID[2].ToString()));
                        if (Session["GroupName"].ToString().ToLower().Trim() == "board secretariat user" || Session["Designation"].ToString().ToLower() == "board secretary" || Session["GroupName"].ToString().ToLower() == "admin" || Session["Designation"].ToString().ToLower() == "board secretary" || Session["GroupName"].ToString().ToLower() == "admin" || Session["GroupName"].ToString().ToLower().Trim() == "president" || Session["GroupName"].ToString().ToLower().Trim() == "office secretary")
                        {
                            strPageCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}  and b.column0<'{2}'", TableNameFrom, Session["FolderID"].ToString(), FileNameID[2].ToString()));

                        }
                        else
                        {
                            strPageCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and (a.approvalstatus=1  or a.approvalstatus=2   or a.approvalstatus=3) and b.column0<'{2}'", TableNameFrom, Session["FolderID"].ToString(), FileNameID[2].ToString()));

                        }
                        if (strPageCount == "")
                        {
                            sCustCode = 1;
                        }
                        else
                        {
                            sCustCode = (Convert.ToInt32(strPageCount) + 1);
                        }

                        if (Convert.ToString(Session["Groupname"]).ToLower() == "directors" || Session["GroupName"].ToString().ToLower() == "permanent invitees" || Session["GroupName"].ToString().ToLower() == "cfo" || Convert.ToString(Session["Groupname"]).ToLower() == "senior management" || Convert.ToString(Session["Groupname"]).ToLower() == "functional management" || Convert.ToString(Session["Groupname"]).ToLower() == "others")
                        {

                            if (Session["FolderName"].ToString().ToLower() == "meetings\\my briefcase" || Session["FolderName"].ToString().ToLower() == "meetings\\company info" || Session["FolderName"].ToString().ToLower() == "meetings\\shared documents")
                            {
                                Response.Redirect("../AddRenameFolder/ThumbnailMOMM.aspx?value=6", false);
                            }
                            else
                                if (Request.QueryString["Value"] != null && Request.QueryString["Value"] != "meeting")
                                {
                                    if (Request.QueryString["Value"].ToString().ToLower() == "my briefcase")
                                    {
                                        Response.Redirect("../Viewer/ThumbnailSearch.aspx?id=" + sCustCode + "&value=my briefcase", false);
                                    }
                                    else
                                        if (Request.QueryString["Value"].ToString().ToLower() == "company info")
                                        {
                                            Response.Redirect("../Viewer/ThumbnailSearch.aspx?id=" + sCustCode + "&value=company info", false);
                                        }
                                }
                                else
                                {
                                    Response.Redirect("../Viewer/ThumbnailSearch.aspx?id=" + sCustCode, false);
                                }
                        }
                        else
                        {
                            //int value = 0;
                            //DataTable dtfiledetails1 = objCommonBAL.GetDispFileNameBoardSec(TableNameFrom, Convert.ToInt32(Session["FolderID"]), Convert.ToInt32(Session["UserID"]));
                            //for (int i = 0; i < dtfiledetails1.Rows.Count; i++)
                            //{
                            //    if (Convert.ToInt32(dtfiledetails1.Rows[i]["ApprovalStatus"].ToString()) == 2)
                            //    {
                            //        value += 1;
                            //    }
                            //    else
                            //    {
                            //        value += Convert.ToInt32(dtfiledetails1.Rows[i]["ApprovalStatus"].ToString());
                            //    }
                            //}
                            //if (value == dtfiledetails1.Rows.Count)
                            //{

                            Response.Redirect("../Viewer/ThumbnailSearch.aspx?id=" + sCustCode, false);
                            //}
                            //else
                            //{
                            //    if (Session["FolderName"].ToString().ToLower() == "meetings\\my briefcase" || Session["FolderName"].ToString().ToLower() == "meetings\\company info")
                            //    {
                            //        Response.Redirect("../AddRenameFolder/ThumbnailMOMM.aspx?value=6", false);
                            //    }
                            //    else
                            //    {
                            //        Response.Redirect("../Viewer/Thumbnail.aspx", false);
                            //    }
                            //}
                        }


                    }
                    else if (Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".zip" || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".rar" || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".zipx")
                    {
                        ScriptManager.RegisterStartupScript(this.Page, typeof(UpdatePanel), "msg", "alert('Please download file, then view on your local')", true);
                        return;
                    }

                    else
                    {

                        StringBuilder sbwindow = new StringBuilder();
                        //sbwindow.Append("window.open('../Viewer/OfficerViewer.aspx',null,'scrollbars:no;width=1024px, height=800px;help:no;center:yes; resizable=yes');");
                        sbwindow.Append("window.open('../Viewer/HTML_OfficerViewer.aspx',null,'scrollbars:no;width=1024px, height=800px;help:no;center:yes; resizable=yes');");
                        // sbwindow.Append("window.showModelessDialog('Viewer/OfficerViewer.aspx',null,'status:no;dialogTop:300;dialogWidth:1024px;dialogHeight:800px;dialogHide:true;help:no;scroll:yes;center:yes');");
                        ScriptManager.RegisterClientScriptBlock(this.Page, typeof(UpdatePanel), "DocumentViewer", sbwindow.ToString(), true);
                        sbwindow = null;
                    }
                }
                //else
                //{
                //    Response.Redirect("../UploadFile/FileUpload_New.aspx?ATR=" + FileNameID[0].ToString(), false);
                //}


                //}
            }
            if (e.CommandName == "Attachment")
            {
                string[] FileNameID = e.CommandArgument.ToString().Split(new Char[] { ',' });
                if (FileNameID[1].ToString().ToLower() != "")
                {

                    Response.Redirect("../UploadFile/FileUpload_Multiple.aspx?ATR=Attachment&FileID=" + FileNameID[0].ToString(), false);
                }
            }
            if (e.CommandName == "AccessRestrict")
            {
                string[] FileNameID = e.CommandArgument.ToString().Split(new Char[] { ',' });
                if (FileNameID[1].ToString().ToLower() != "")
                {

                    Response.Redirect("../UploadFile/AgendaLevelAccessControl.aspx?FileID=" + FileNameID[0].ToString(), false);
                }
            }
            if (e.CommandName == "BoardView")
            {
                //if user does not have file view permission then a message will be displayed. You don't have access to view these files.
                string[] FileNameID = e.CommandArgument.ToString().Split(new Char[] { ',' });
                Session["WorkFlow"] = null;

                if (FileNameID[1].ToString().ToLower() != "")
                {
                    Session["FileID"] = FileNameID[0].ToString();
                    Session["FileName"] = FileNameID[1].ToString();
                    Session["Itemno"] = FileNameID[2].ToString();
                    //Session["Particulars"] = FileNameID[3].ToString();
                    Session["PageCount"] = FileNameID[3].ToString();

                    Session["Redirect"] = "View";
                    if (Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".tif" || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".tiff"
                        || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".gif" || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".bmp"
                        || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".jpg" || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".jpeg" || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".png")
                    {
                        Response.Redirect("../Viewer/Thumbnail.aspx", false);
                    }

                    else if (Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".pdf" || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".docx" || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".pptx" || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".xlsx")
                    {
                        bool strCheckDigits = IsAllDigits(FileNameID[2].ToString());
                        if (FileNameID[4].ToString() == "2" && strCheckDigits == true)
                        {
                            Response.Redirect("../UploadFile/FileUpload_New.aspx?ATR=" + FileNameID[0].ToString() + "&ID=" + FileNameID[2].ToString(), false);
                            Session["TempFileID"] = FileNameID[0].ToString();
                        }
                        else
                            if (FileNameID[4].ToString() == "2" && strCheckDigits == false)
                            {
                                Response.Redirect("../UploadFile/FileUpload_New.aspx?ATR=" + FileNameID[0].ToString() + "&ID=" + FileNameID[2].ToString(), false);
                                Session["TempFileID"] = FileNameID[0].ToString();
                            }
                            else
                            {
                                Response.Redirect("../Viewer/Thumbnail.aspx", false);
                            }

                    }
                    else
                    {


                    }
                }

            }

            if (e.CommandName == "ReplaceNote")
            {
                //if user does not have file view permission then a message will be displayed. You don't have access to view these files.
                string[] FileNameID = e.CommandArgument.ToString().Split(new Char[] { ',' });
                Session["WorkFlow"] = null;

                if (FileNameID[1].ToString().ToLower() != "")
                {
                    Session["FileID"] = FileNameID[0].ToString();
                    Session["FileName"] = FileNameID[1].ToString();
                    Session["Itemno"] = FileNameID[2].ToString();
                    //Session["Particulars"] = FileNameID[3].ToString();
                    Session["PageCount"] = FileNameID[3].ToString();
                    Response.Redirect("../UploadFile/FileUpload_New.aspx?ATR=" + FileNameID[0].ToString() + "&ID=" + FileNameID[2].ToString(), false);
                    Session["TempFileID"] = FileNameID[0].ToString();
                }
            }


            if (e.CommandName == "Version")
            {
                //we check if a file does not have version This file has no versions
                string[] FileNameID = e.CommandArgument.ToString().Split(new Char[] { ',' });

                Session["FileId"] = FileNameID[0].ToString();
                Session["FileName"] = FileNameID[1].ToString();
                Session["versionNo"] = FileNameID[2].ToString();

                Session["Redirect"] = "Version";
                if (Path.GetExtension(Session["FileName"].ToString().ToLower()) != ".tif" || Path.GetExtension(Session["FileName"].ToString().ToLower()) != ".tiff"
                    || Path.GetExtension(Session["FileName"].ToString().ToLower()) != ".gif" || Path.GetExtension(Session["FileName"].ToString().ToLower()) != ".bmp"
                    || Path.GetExtension(Session["FileName"].ToString().ToLower()) != ".jpg" || Path.GetExtension(Session["FileName"].ToString().ToLower()) != ".jpeg"
                    || Path.GetExtension(Session["FileName"].ToString().ToLower()) != ".png" || Path.GetExtension(Session["FileName"].ToString().ToLower()) != ".wmv")
                {
                    intCount = objCommonBAL.CheckVersionExistance(Convert.ToInt32(Session["FolderID"]), Convert.ToInt64(Session["FileId"]));
                    if (intCount > 0)
                    {
                        StringBuilder sbwindow = new StringBuilder();
                        sbwindow.Append("window.open('../Viewer/VersionViewer.aspx?page',null,'scrollbars=yes;width=1024, height=800;help:no;center:yes; resizable=yes');");
                        //sbwindow.Append("window.showModelessDialog ('Viewer/VersionViewer.aspx',null,'status:no;dialogTop:300;dialogWidth:1014px;dialogHeight:700px;dialogHide:true;help:no;scroll:no;center:yes');");

                        ScriptManager.RegisterClientScriptBlock(this.Page, typeof(UpdatePanel), "DocumentViewer", sbwindow.ToString(), true);
                        sbwindow = null;
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('This file has no versions.')", true);
                        return;
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('This file has no versions.')", true);
                    return;
                }

            }
            if (e.CommandName == "Edit")
            {
                gvData.EditIndex = 1;
                //((LinkButton)(e.CommandSource)).FindControl("UpdateButton").Visible = true;
                //((LinkButton)(e.CommandSource)).FindControl("CancelButton").Visible = true;
                //((LinkButton)(e.CommandSource)).FindControl("lnkEdit").Visible = false;
            }
            //DirectorDataBinding();
        }
        catch (Exception ex)
        {
            Session["Error"] = ex.ToString();
            //Response.Redirect("../ErrorMessage.aspx", false);
        }
        finally
        {
            objCommonBAL = null;
        }
    }

    public static bool convertPDF_TIFF(string filename, string OutPutFileName)
    {

        //This is the object that perform the real conversion!
        PDFConvert converter = new PDFConvert();

        bool Converted = false;
        //Setup the converter
        // if (numericThreads.Value > 0)
        converter.RenderingThreads = -1;// (int)numericThreads.Value;

        //if (((int)numericTextSampling.Value > 0) && ((int)numericTextSampling.Value != 3))
        converter.TextAlphaBit = -1;// (int)numericTextSampling.Value;

        // if (((int)numericGraphSampling.Value > 0) && ((int)numericGraphSampling.Value != 3))
        converter.TextAlphaBit = -1;//(int)numericGraphSampling.Value;


        converter.OutputToMultipleFile = true;//checkSingleFile.Checked;

        converter.FirstPageToConvert = -1;// (int)numericFirstPage.Value;
        converter.LastPageToConvert = -1;// (int)numericLastPage.Value;
        converter.FitPage = false;//checkFitTopage.Checked;
        converter.JPEGQuality = 50;// (int)numQuality.Value;
        converter.ResolutionX = 300;
        converter.ResolutionY = 300;
        // converter.OutputFormat = "tifflzw";//comboFormat.Text;
        converter.OutputFormat = "jpeg";//comboFormat.Text;
        System.IO.FileInfo input = new FileInfo(filename);
        string OutputFilePath = HttpContext.Current.Server.MapPath("~\\Repository\\PageTiffFiles");
        if (!Directory.Exists(OutputFilePath))
        {
            Directory.CreateDirectory(OutputFilePath);
        }
        string output = string.Format("{0}\\{1}{2}", OutputFilePath, OutPutFileName, ".tif");
        //If the output file exist alrady be sure to add a random name at the end until is unique!
        //while (System.IO.File.Exists(output))
        //{
        //    output = output.Replace(".tif", string.Format("{1}{0}", ".tif", DateTime.Now.Ticks));
        //}

        Converted = converter.Convert(input.FullName, output);

        return Converted;
    }

    public bool IsAllDigits(string s)
    {
        return s.All(Char.IsDigit);
    }

    #region [lnkEdit_Click]
    protected void lnkEdit_Click(object sender, EventArgs e)
    {
        try
        {
            //int i = Convert.ToInt16(gvData.SelectedIndex);
            //GridViewRow row = gvData.Rows[i];
            gvData.EditIndex = 1;
            //row.FindControl("UpdateButton").Visible = true;
            //row.FindControl("CancelButton").Visible = true;
            //row.FindControl("lnkEdit").Visible = false;
        }
        catch (System.Exception ex)
        {
            Session["Error"] = ex.StackTrace;
        }
        finally
        {
            objCommonBAL = null;
            objFileUploadBAL = null;
        }
    }
    #endregion

    #region [lnkUpdate_Click]
    protected void lnkUpdate_Click(object sender, EventArgs e)
    {
        try
        {
            int i = Convert.ToInt16(gvData.SelectedIndex);
            GridViewRow row = gvData.Rows[i];

            //LinkButton lnkUpdate = (LinkButton)row.FindControl("UpdateButton");
            GenericDAL objDAL = new GenericDAL();
            Label subject = (Label)row.FindControl("lblsubject");
            Label txtlotno = (Label)row.FindControl("txtLotNo");
            //TextBox txtMeetingDate = (TextBox)ListView1.Items[e.ItemIndex].FindControl("txtMeetingDate");
            LinkButton lnkUpdate = (LinkButton)row.FindControl("LnkApprove");
            string fileId;
            fileId = lnkUpdate.CommandArgument.ToString();
            string filename = "";
            filename = fileId.Substring(fileId.IndexOf(',') + 1);
            //lnkUpdate.Visible = false;
            int fid = Convert.ToInt32(fileId.Substring(0, fileId.IndexOf(',')));
            Object[] objparam = { Convert.ToInt32(Session["FolderId"]) };

            // string query = "Select TableName From tblFolderIndexMaster where Folder_Id="+folderid+"";

            string IndexTable = (string)objDAL.ExecuteScalarString("SPGetIndexTableNameOfFolder", objparam);
            string updatequery = "UPDATE " + IndexTable + " SET Column2='" + subject.Text.ToString() + "',Column3='" + txtlotno.Text.ToString() + "' WHERE FILEID=" + fid;
            int success = objDAL.ExecuteNonQuery(updatequery);
            if (success > 0)
            {

                ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('Index Updation of " + filename + " is done successfully.')", true);

            }
            else
            {
                ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('Index Updation of " + filename + " is not done')", true);
            }
            gvData.EditIndex = -1;
            row.FindControl("UpdateButton").Visible = false;
            row.FindControl("CancelButton").Visible = false;
            row.FindControl("lnkEdit").Visible = true;
        }
        catch (System.Exception ex)
        {
            Session["Error"] = ex.StackTrace;
        }
        finally
        {
            objCommonBAL = null;
            objFileUploadBAL = null;
        }
    }
    #endregion

    #region [lnkCancel_Click]
    protected void lnkCancel_Click(object sender, EventArgs e)
    {

    }
    #endregion lnkCancel_Click

    //protected void btnStartMOM_Click(object sender, EventArgs e)
    //{
    //    OperationClass objOperationClass = new OperationClass();
    //    int returnstatus = objOperationClass.ExecuteNonQuery(string.Format(@"update tblfolder set MeetingStatus = 1 where folderid = {0}", Session["FolderID"].ToString()));
    //    if (returnstatus > 0)
    //    {
    //        ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('Meetimg Status updated successfully.')", true);
    //    }

    //}

    protected void btnStartMOM_Click(object sender, EventArgs e)
    {
        OperationClass objOperationClass = new OperationClass();
        int returnstatus = objOperationClass.ExecuteNonQuery(string.Format(@"update tblfolder set MeetingStatus = 1 where folderid = {0}", Session["FolderID"].ToString()));
        if (returnstatus > 0)
        {
            string ParentFolderName = operation.ExecuteScalar4Command(string.Format(@"select FolderName  from tblFolder where folderId in (select ParentFolderId from tblFolder where folderId={0})", Convert.ToInt32(Session["FolderID"])));
            string CommitteFname = ParentFolderName + "//" + Session["DMeetingdate"].ToString();
            DateTime sfds = DateTime.Now;
            string sdasd = sfds.ToString("yyyy-MM-dd HH:mm:ss");
            string value1 = operation.ExecuteScalar4Command(string.Format(@"insert into tblfileactions (FileName,ActionName,UserID,ActionDate,Status,FolderId) values ('{0}','{1}',{2},'{3}','{4}',{5})", CommitteFname, "Moved to Archive", Session["UserId"], sdasd, 1, Session["FolderId"].ToString()));

            ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('Meetimg Status updated successfully.')", true);


        }
    }

    #region Parent Grid View
    protected void gvParent_DataBound(object sender, EventArgs e)
    {
        try
        {
            if (gvParent.Rows.Count > 0)
            {
                for (int rowIndex = gvParent.Rows.Count - 2; rowIndex >= 0; rowIndex--)
                {
                    GridViewRow row = gvParent.Rows[rowIndex];
                    GridViewRow previousRow = gvParent.Rows[rowIndex + 1];
                    Label fieldrow = (Label)row.FindControl("lblItemNo");
                    string tempCurrent = fieldrow.Text;
                    Label fieldpreviousrow = (Label)previousRow.FindControl("lblItemNo");
                    string tempprevious = fieldpreviousrow.Text;
                    //for (int i = 0; i < row.Cells.Count; i++)
                    //{
                    if (tempCurrent == tempprevious)
                    {
                        row.Cells[1].RowSpan = previousRow.Cells[1].RowSpan < 2 ? 2 : previousRow.Cells[1].RowSpan + 1;
                        previousRow.Cells[1].Visible = false;
                    }
                }
            }
        }
        catch (Exception ex) { }
    }
    protected void gvParent_RowEditing(object sender, GridViewEditEventArgs e)
    {
        gvParent.EditIndex = e.NewEditIndex;
        bindLvFileView();
    }
    protected void gvParent_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        GenericDAL objDAL = new GenericDAL();
        TextBox particulars = (TextBox)gvParent.Rows[e.RowIndex].FindControl("txtParticular");
        TextBox purpose = (TextBox)gvParent.Rows[e.RowIndex].FindControl("txtComments");
        DropDownList gm = (DropDownList)gvParent.Rows[e.RowIndex].FindControl("ddlGM");
        TextBox itemno = (TextBox)gvParent.Rows[e.RowIndex].FindControl("txtItemNo");
        //TextBox txtMeetingDate = (TextBox)ListView1.Items[e.ItemIndex].FindControl("txtMeetingDate");
        LinkButton lnkUpdate = (LinkButton)gvParent.Rows[e.RowIndex].FindControl("lnkDownload");
        string fileId;
        fileId = lnkUpdate.CommandArgument.ToString();
        string filename = "";
        filename = fileId.Substring(fileId.IndexOf(',') + 1);
        //lnkUpdate.Visible = false;
        int fid = Convert.ToInt32(fileId.Substring(0, fileId.IndexOf(',')));
        Object[] objparam = { Convert.ToInt32(Session["FolderId"]) };

        // string query = "Select TableName From tblFolderIndexMaster where Folder_Id="+folderid+"";

        string IndexTable = (string)objDAL.ExecuteScalarString("SPGetIndexTableNameOfFolder", objparam);
        string updatequery = "UPDATE " + IndexTable + " SET  Column0='" + itemno.Text.ToString() + "',Column1='" + particulars.Text.ToString() + "',Column2='" + purpose.Text.ToString() + "',Column3='" + gm.SelectedItem.Text.ToString() + "' WHERE FILEID=" + fid;
        int success = objDAL.ExecuteNonQuery(updatequery);
        if (success == 1)
        {

            ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('Index Updation of " + filename + " is done successfully.')", true);

        }
        else
        {
            ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('Index Updation of " + filename + " is not done')", true);
        }
        e.Cancel = true;
        gvParent.EditIndex = -1;
        bindLvFileView();
    }
    protected void gvParent_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        e.Cancel = true;
        gvParent.EditIndex = -1;
        bindLvFileView();
    }
    protected void gvParent_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        lblMessage.Text = "";
        int intCount = 0;
        try
        {

            objCommonBAL = new CommonBAL();
            objFileUploadBAL = new FileUploadBAL();
            Session["IDS"] = Convert.ToInt32(Session["FolderID"]);
            /*
             *  ConfigurationManager.AppSettings["AccessType"]
             * if AccessType userwise then pass value 1 and groupwise then pass 0 to 
             * CheckAccessRight,GetParentChildFolderId methodes.
             */

            //Get access symbol on basis of folderid of  selected folder of treeview.
            //string Symbol = objCommonBAL.GetSymbolstring(Convert.ToInt32(Session["FolderID"]), Convert.ToInt32(Session["GroupID"]), Convert.ToInt32(Session["UserID"]), ConfigurationManager.AppSettings["AccessType"].ToString().ToLower() == "userwise" ? 1 : 0);
            string Symbol = objCommonBAL.GetSymbolstring(Convert.ToInt32(Session["FolderID"]), Convert.ToInt32(Session["GroupID"]), Convert.ToInt32(Session["UserID"]), ConfigurationManager.AppSettings["AccessType"].ToString().ToLower() == "userwise" ? 1 : 0);

            //if symbol equal to 'L' or 'N' then retrun on page.
            if (Symbol == "L" || Symbol == "N")
            {
                lblMessage.Visible = true;
                lblMessage.Text = "You don't have access to view these files.";
                return;
            }
            GridViewRow editrow = null;

            if (e.CommandName != "Page")
                editrow = (GridViewRow)(((Control)e.CommandSource).NamingContainer);


            if (e.CommandName == "DownLoad")
            {
                string[] FileNameID = e.CommandArgument.ToString().Split(new Char[] { ',' });

                Session["FileID"] = FileNameID[0].ToString();
                Session["FileName"] = FileNameID[1].ToString();

                // create objects of class
                objCommonBAL = new CommonBAL();

                //visible lblmessage
                lblMessage.Visible = false;
                lblMessage.Text = "";

                //visible panel first
                Panel4.Visible = false;

                //set folder for save decrypt file.
                string ImageSavingFilePath = Convert.ToString(HttpContext.Current.Server.MapPath("~\\Repository\\Decrypt\\" + HttpContext.Current.Session["UserName"].ToString()));

                //set folder path exported file.
                string ImagesavedFilePath = Convert.ToString(HttpContext.Current.Server.MapPath("~\\ExportFile\\" + Convert.ToString(HttpContext.Current.Session["UserName"]).Trim()));

                //Set zip file name and path
                string ZipFilePath = Convert.ToString(Server.MapPath("~\\ExportFile\\".Trim() + Session["UserName"].ToString().Trim() + "\\" + string.Format("ExportedFile{0:MMM-dd-yyyy_hh-mm-ss}", System.DateTime.Now) + ".zip"));

                //set directory path for delete tepory file.
                string ZipDirectoryPath = Convert.ToString(Server.MapPath("~\\ExportFile\\".Trim() + Session["UserName"].ToString().Trim()));

                //Download();
                // Call methode for export file from database or folder
                string strMessage = objCommonBAL.ExportFileOnButtonClick1(Convert.ToString(Session["FileID"]), ImageSavingFilePath, ImagesavedFilePath, ZipFilePath, ZipDirectoryPath, "");
                if (strMessage.Contains("alert"))
                {
                    ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "myScr", strMessage, true);
                }
                else
                {
                    FileDownLoad(strMessage);
                }
            }
            if (e.CommandName == "View")
            {
                //if user does not have file view permission then a message will be displayed. You don't have access to view these files.
                string[] FileNameID = e.CommandArgument.ToString().Split(new Char[] { ',' });

                Session["FileID"] = FileNameID[0].ToString();
                Session["FileName"] = FileNameID[1].ToString();

                Session["Redirect"] = "View";
                if (Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".tif" || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".tiff"
                    || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".gif" || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".bmp"
                    || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".jpg" || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".jpeg" || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".png")
                {
                    Response.Redirect("../Viewer/Thumbnail.aspx", false);
                }
                else if (Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".wmv")
                {
                    StringBuilder sbwindow = new StringBuilder();
                    sbwindow.Append("window.showModalDialog('../Viewer/WMVViwer.aspx',null,'status:no;dialogTop:300;dialogWidth:1014px;dialogHeight:700px;dialogHide:true;help:no;scroll:no;center:yes');");
                    ScriptManager.RegisterClientScriptBlock(this.Page, typeof(UpdatePanel), "VideoViewer", sbwindow.ToString(), true);
                    sbwindow = null;
                }
                else if (Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".pdf")
                {
                    Response.Redirect("../Viewer/Thumbnail.aspx", false);

                }
                else if (Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".zip" || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".rar" || Path.GetExtension(Session["FileName"].ToString().ToLower()) == ".zipx")
                {
                    ScriptManager.RegisterStartupScript(this.Page, typeof(UpdatePanel), "msg", "alert('Please download file, then view on your local')", true);
                    return;
                }

                else
                {
                    StringBuilder sbwindow = new StringBuilder();
                    sbwindow.Append("window.open('../Viewer/OfficerViewer.aspx',null,'scrollbars:no;width=1024px, height=800px;help:no;center:yes; resizable=yes');");
                    // sbwindow.Append("window.showModelessDialog('Viewer/OfficerViewer.aspx',null,'status:no;dialogTop:300;dialogWidth:1024px;dialogHeight:800px;dialogHide:true;help:no;scroll:yes;center:yes');");
                    ScriptManager.RegisterClientScriptBlock(this.Page, typeof(UpdatePanel), "DocumentViewer", sbwindow.ToString(), true);
                    sbwindow = null;
                }
            }

            if (e.CommandName == "Edit")
            {
                gvParent.EditIndex = 1;

            }

        }
        catch (Exception ex)
        {
            Session["Error"] = ex.ToString();
            Response.Redirect("../ErrorMessage.aspx", false);
        }
        finally
        {
            objCommonBAL = null;
        }
    }
    #endregion

    //protected void btnAuthorizeAll_Click(object sender, EventArgs e)
    //{
    //    foreach (GridViewRow row in gvData.Rows)
    //    {
    //        CheckBox chkSelect = (CheckBox)row.FindControl("chkSelect");
    //        if (chkSelect.Checked)
    //        {
    //            Label FileId = (Label)row.FindControl("lblFileId");
    //            int value2 = Convert.ToInt16(operation.Insert4Command(string.Format(@"Update tblfile set ApprovalStatus=1 where fileid={0}", Convert.ToInt32(FileId.Text.ToString()))));
    //        }
    //    }
    //    DirectorDataBinding();
    //}
    //protected void btnAuthorizeAll_Click(object sender, EventArgs e)
    //{
    //    OperationClass objOperationClass = new OperationClass();
    //    foreach (GridViewRow row in gvData.Rows)
    //    {
    //        CheckBox chkSelect = (CheckBox)row.FindControl("chkSelect");
    //        if (chkSelect.Checked)
    //        {
    //            Label FileId = (Label)row.FindControl("lblFileId");
    //            DataTable dtAttachmentIds = objOperationClass.GetTable4Command(string.Format(@"select attachmentid  from tblattachment where fileid = {0}", Convert.ToInt32(FileId.Text.ToString())));
    //            string fileids = FileId.Text.ToString();

    //            fileids = fileids + ",";
    //            if (dtAttachmentIds != null && dtAttachmentIds.Rows.Count > 0)
    //            {
    //                for (int i = 0; i < dtAttachmentIds.Rows.Count; i++)
    //                {
    //                    if (i == dtAttachmentIds.Rows.Count - 1)
    //                    {
    //                        fileids = fileids + dtAttachmentIds.Rows[i]["attachmentid"].ToString();
    //                    }
    //                    else
    //                    {
    //                        fileids = fileids + dtAttachmentIds.Rows[i]["attachmentid"].ToString() + ",";
    //                    }
    //                }
    //            }
    //            else
    //            {
    //                fileids = fileids.Substring(0, fileids.LastIndexOf(","));
    //            }
    //            //int value2 = Convert.ToInt16(operation.Insert4Command(string.Format(@"Update tblfile set ApprovalStatus=1 where fileid={0}", Convert.ToInt32(FileId.Text.ToString()))));
    //            int value2 = Convert.ToInt16(operation.Insert4Command(string.Format(@"Update tblfile set ApprovalStatus=1 where fileid in ({0})", fileids)));
    //        }
    //    }
    //    DirectorDataBinding();
    //}

    protected void btnAuthorizeAll_Click(object sender, EventArgs e)
    {
        if (chkSelectalll.Checked == true)
        {

            R = 0;
            OperationClass objOperationClass = new OperationClass();

            string TableName = objOperationClass.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Convert.ToInt32(Session["FolderID"])));
            DataTable dtfiledetails = objOperationClass.GetTable4Command(
            string.Format(@"select b.FileiD,approvalstatus,b.folderid from tblfile a inner join {0} b on a.fileid=b.fileid where b.folderid='{1}' order by column0", TableName, Convert.ToInt32(Session["FolderID"])));
            foreach (DataRow row in dtfiledetails.Rows)
            {

                string strapprovalstatus = row["approvalstatus"].ToString();
                string strFileId = row["FileiD"].ToString();
                DataTable dtAttachmentIds = objOperationClass.GetTable4Command(string.Format(@"select attachmentid  from tblattachment where fileid = {0}", Convert.ToInt32(strFileId.ToString())));
                string fileids = strFileId.ToString();

                fileids = fileids + ",";
                if (dtAttachmentIds != null && dtAttachmentIds.Rows.Count > 0)
                {
                    for (int i = 0; i < dtAttachmentIds.Rows.Count; i++)
                    {
                        if (i == dtAttachmentIds.Rows.Count - 1)
                        {
                            fileids = fileids + dtAttachmentIds.Rows[i]["attachmentid"].ToString();
                        }
                        else
                        {
                            fileids = fileids + dtAttachmentIds.Rows[i]["attachmentid"].ToString() + ",";
                        }
                    }
                }
                else
                {
                    fileids = fileids.Substring(0, fileids.LastIndexOf(","));
                }

                if (Convert.ToString(row["approvalstatus"].ToString()) != "2" && Convert.ToString(row["approvalstatus"].ToString()) != "5")
                {
                    int value2 = Convert.ToInt16(operation.Insert4Command(string.Format(@"Update tblfile set ApprovalStatus=1 where fileid in ({0})", fileids)));
                }

            }
            DirectorDataBinding();
            chkSelectalll.Checked = false;

        }
        else
        {
            R = 0;
            OperationClass objOperationClass = new OperationClass();
            foreach (GridViewRow row in gvData.Rows)
            {
                CheckBox chkSelect = (CheckBox)row.FindControl("chkSelect");
                Label lblApprovalStatus = (Label)row.FindControl("lblApprovalStatus");
                if (chkSelect.Checked)
                {
                    Label FileId = (Label)row.FindControl("lblFileId");
                    DataTable dtAttachmentIds = objOperationClass.GetTable4Command(string.Format(@"select attachmentid  from tblattachment where fileid = {0}", Convert.ToInt32(FileId.Text.ToString())));
                    string fileids = FileId.Text.ToString();

                    fileids = fileids + ",";
                    if (dtAttachmentIds != null && dtAttachmentIds.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtAttachmentIds.Rows.Count; i++)
                        {
                            if (i == dtAttachmentIds.Rows.Count - 1)
                            {
                                fileids = fileids + dtAttachmentIds.Rows[i]["attachmentid"].ToString();
                            }
                            else
                            {
                                fileids = fileids + dtAttachmentIds.Rows[i]["attachmentid"].ToString() + ",";
                            }
                        }
                    }
                    else
                    {
                        fileids = fileids.Substring(0, fileids.LastIndexOf(","));
                    }
                    //int value2 = Convert.ToInt16(operation.Insert4Command(string.Format(@"Update tblfile set ApprovalStatus=1 where fileid={0}", Convert.ToInt32(FileId.Text.ToString()))));
                    if (lblApprovalStatus.Text != "2" && lblApprovalStatus.Text != "5")
                    {
                        int value2 = Convert.ToInt16(operation.Insert4Command(string.Format(@"Update tblfile set ApprovalStatus=1 where fileid in ({0})", fileids)));
                    }
                }
            }
            DirectorDataBinding();
            chkSelectalll.Checked = false;

        }
    }


    protected void btnAuthorize_Click(object sender, EventArgs e)
    {
        int strStepStatus = 0;
        string strAgendatablename = operation.ExecuteScalar4Command(string.Format(@"select foldername from tblfolder where FolderID={0}", Convert.ToInt32(Session["FolderID"])));
        if (strAgendatablename.ToLower() == "binani cement")
        {
            foreach (GridViewRow row in GridView1.Rows)
            {
                CheckBox chkSelect = (CheckBox)row.FindControl("chkSelect");
                Label lblReason = (Label)row.FindControl("lblReason");

                if (chkSelect.Checked && lblReason.Text == "")
                {
                    HiddenField hdStepStatus = (HiddenField)row.FindControl("hdStepStatus");
                    Label lblId = (Label)row.FindControl("lblId");

                    if (hdStepStatus.Value.ToString() == "2")
                    {
                        strStepStatus = Convert.ToInt32(operation.ExecuteNonQuery(string.Format(@"update tblAgendaApprovalDetail set StepStatus={0},Status='{1}' where Id={2}", (Convert.ToInt32(hdStepStatus.Value.ToString()) + 1), "For Approval", Convert.ToInt32(lblId.Text.ToString()))));
                    }
                    else
                        if (hdStepStatus.Value.ToString() == "3")
                        {
                            strStepStatus = Convert.ToInt32(operation.ExecuteNonQuery(string.Format(@"update tblAgendaApprovalDetail set StepStatus={0},Status='{1}' where Id={2}", (Convert.ToInt32(hdStepStatus.Value.ToString()) + 1), "For Approval", Convert.ToInt32(lblId.Text.ToString()))));
                        }
                        else
                            if (hdStepStatus.Value.ToString() == "4")
                            {
                                strStepStatus = Convert.ToInt32(operation.ExecuteNonQuery(string.Format(@"update tblAgendaApprovalDetail set StepStatus={0},Status='{1}' where Id={2}", (Convert.ToInt32(hdStepStatus.Value.ToString()) + 1), "Approved", Convert.ToInt32(lblId.Text.ToString()))));
                            }
                            else
                                if (hdStepStatus.Value.ToString() == "5")
                                {
                                    strStepStatus = Convert.ToInt32(operation.ExecuteNonQuery(string.Format(@"update tblAgendaApprovalDetail set StepStatus={0},FinalAgendaApprovalStatus={1} where Id={2}", (Convert.ToInt32(hdStepStatus.Value.ToString()) + 1), 1, Convert.ToInt32(lblId.Text.ToString()))));
                                }
                                else
                                {

                                    strStepStatus = Convert.ToInt32(operation.ExecuteNonQuery(string.Format(@"update tblAgendaApprovalDetail set StepStatus={0} where Id={1}", (Convert.ToInt32(hdStepStatus.Value.ToString()) + 1), Convert.ToInt32(lblId.Text.ToString()))));
                                }


                    if (strStepStatus > 0)
                    {
                        if (hdStepStatus.Value.ToString() == "2")
                        {
                            ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('Agenda is Reviewed.')", true);
                        }
                        else
                            if (hdStepStatus.Value.ToString() == "3")
                            {
                                ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('Agenda is authorized.')", true);
                            }
                            else
                                if (hdStepStatus.Value.ToString() == "4")
                                {
                                    ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('Agenda is Forwarded.')", true);
                                }
                                else
                                    if (hdStepStatus.Value.ToString() == "5")
                                    {
                                        ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('Agenda is Forwarded.')", true);
                                    }


                    }
                }
            }

            GetAgendaDetails();
        }
        else
        {
            foreach (GridViewRow row in GridView1.Rows)
            {
                CheckBox chkSelect = (CheckBox)row.FindControl("chkSelect");
                Label lblReason = (Label)row.FindControl("lblReason");
                if (chkSelect.Checked && lblReason.Text == "")
                {
                    HiddenField hdStepStatus = (HiddenField)row.FindControl("hdStepStatus");
                    Label lblId = (Label)row.FindControl("lblId");

                    if (hdStepStatus.Value.ToString() == "2")
                    {
                        strStepStatus = Convert.ToInt32(operation.ExecuteNonQuery(string.Format(@"update tblAgendaApprovalDetail set StepStatus={0},Status='{1}' where Id={2}", (Convert.ToInt32(hdStepStatus.Value.ToString()) + 1), "For Approval", Convert.ToInt32(lblId.Text.ToString()))));
                    }
                    else
                        if (hdStepStatus.Value.ToString() == "3")
                        {
                            strStepStatus = Convert.ToInt32(operation.ExecuteNonQuery(string.Format(@"update tblAgendaApprovalDetail set StepStatus={0},FinalAgendaApprovalStatus={1} where Id={2}", (Convert.ToInt32(hdStepStatus.Value.ToString()) + 1), 1, Convert.ToInt32(lblId.Text.ToString()))));
                        }



                    if (strStepStatus > 0)
                    {
                        if (hdStepStatus.Value.ToString() == "2")
                        {
                            ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('Agenda is Reviewed.')", true);
                        }
                        else
                            if (hdStepStatus.Value.ToString() == "3")
                            {
                                ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('Agenda is Forwarded.')", true);
                            }
                    }
                }
            }
            GetAgendaDetailsforBoard();

        }
        Response.Redirect("../Default/Default.aspx");

    }

    protected void btnDistributeAll_Click(object sender, EventArgs e)
    {
        Panel7.Visible = true;
    }

    #region btnYes_Click]
    protected void btnYesSms_Click(object sender, EventArgs e)
    {
        string Response = string.Empty;
        if (Session["UserName"] != null)
        {
            string no1 = "";
            try
            {
                OperationClass objOperationClass = new OperationClass();
                string parentfoldername = objOperationClass.ExecuteScalar4Command(string.Format(@"select foldername from tblfolder where folderid=(select parentfolderid from tblfolder where folderid={0})", Convert.ToInt16(Session["FolderID"])));
                string foldername = objOperationClass.ExecuteScalar4Command(string.Format(@"select foldername from tblfolder where folderid={0}", Convert.ToInt16(Session["FolderID"])));
                string TableName = objOperationClass.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Convert.ToInt32(Session["FolderID"])));
                DataTable dtmin = objOperationClass.GetTable4Command(string.Format(@"select min(column0) 'Min',max(column0) 'Max' from {0}", TableName));
                string min = "", max = "";
                if (dtmin != null && dtmin.Rows.Count > 0)
                {
                    min = dtmin.Rows[0]["Min"].ToString();
                    max = dtmin.Rows[0]["Max"].ToString();
                }
                string msgText = "";
                string userName = ConfigurationSettings.AppSettings["UserName"].ToString();
                string password = ConfigurationSettings.AppSettings["Password"].ToString();
                string From = ConfigurationSettings.AppSettings["From"].ToString();
                //DataTable dt = objOperationClass.GetTable4Command(string.Format(@"Select distinct b.MobileNo,b.Id from tblUserAccesscontrol a inner join tbluserdetail b on a.userid=b.userid and a.FOLDERID IN(" + Convert.ToString(Session["FolderID"]) + ") and AccessSymbol not in('N') and b.MobileNo is not null"));
                DataTable dt = objOperationClass.GetTable4Command(string.Format(@"Select distinct b.MobileNo,b.UID from tblUserAccesscontrol a inner join tblAddressBook b on a.userid=b.userid inner join tbluserdetail c on a.userid=c.userid and a.FOLDERID IN(" + Convert.ToString(Session["FolderID"]) + ") and AccessSymbol not in('N') and b.MobileNo is not null  and bcl=1"));

                //msgText = "Dear Sir/Madam, " + parentfoldername + " Agenda No. " + min + " to " + max + " updated for the Meeting dated " + foldername + " Regards , Board Secretariat.";
                //msgText = "Dear Sir/Madam, " + parentfoldername + " Agenda are updated for the Meeting dated " + foldername + " Regards , Board Secretariat.";
                //int length = msgText.Length;
                msgText = txtsmscomment.Text;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //string no1 = arrno[i].ToString();
                    no1 = dt.Rows[i]["MobileNo"].ToString();

                    //Response = serr.SEND_SMS(no1, msgText, ConfigurationSettings.AppSettings["SMSUserName"].ToString(), ConfigurationSettings.AppSettings["SMSPassword"].ToString());
                    string URL = "http://api.myvaluefirst.com/psms/servlet/psms.Eservice2?data=<?xml%20version=\"1.0\"%20encoding=\"ISO-8859-1\"?><!DOCTYPE%20MESSAGE%20SYSTEM%20\"http://127.0.0.1:80/psms/dtd/messagev12.dtd\"%20><MESSAGE%20VER=\"1.2\"><USER%20USERNAME=\"" + userName + "\"%20PASSWORD=\"" + password + "\"/><SMS%20UDH=\"0\"%20CODING=\"1\"%20TEXT=\"" + msgText + "\"%20PROPERTY=\"0\"%20ID=\"1\"><ADDRESS%20FROM=\"" + From + "\"%20TO=\"" + no1 + "\"%20SEQ=\"1\"%20TAG=\"some%20clientside%20random%20data\"%20/></SMS></MESSAGE>&action=send";
                    //string URL = "https://paypoint.selcommobile.com/bulksms/dispatch56.php?msisdn=" + no1 + "&" + "user=" + userName + "&" + " password= " + password + "&message= " + msgText + "";
                    WebRequest myWebRequest = WebRequest.Create(URL);
                    WebResponse myWebResponse = myWebRequest.GetResponse();
                    Stream ReceiveStream = myWebResponse.GetResponseStream();
                    Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                    StreamReader readStream = new StreamReader(ReceiveStream, encode);
                    string strResponse = readStream.ReadToEnd();

                    string DeliveryStatus1 = "SMS sent to " + no1 + " with Message " + msgText;
                    string ParentFolderNamess = operation.ExecuteScalar4Command(string.Format(@"select FolderName  from tblFolder where folderId in (select ParentFolderId from tblFolder where folderId={0})", Convert.ToInt32(Session["FolderID"])));
                    string CommitteFname = ParentFolderNamess + "//" + Session["DMeetingdate"].ToString();
                    DateTime sfds = DateTime.Now;
                    string sdasd = sfds.ToString("yyyy-MM-dd HH:mm:ss");
                    string value1 = operation.ExecuteScalar4Command(string.Format(@"insert into tblfileactions (FileName,ActionName,UserID,ActionDate,Status,FolderId) values ('{0}','{1}',{2},'{3}','{4}',{5})", CommitteFname, DeliveryStatus1, Session["UserId"], sdasd, 1, Session["FolderId"].ToString()));
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Message Sent Successfully')", true);

                    //SMSServiceClient.SMSServiceService objUserDetails = new SMSServiceClient.SMSServiceService();
                    //int output = objUserDetails.sendSMS(ConfigurationSettings.AppSettings["ApplicationID"].ToString(), no1, msgText);
                    //if (output == 0)
                    //{
                    //    string DeliveryStatus1 = "SMS sent to " + no1 + "with Message" + msgText;
                    //    string ParentFolderNamess = operation.ExecuteScalar4Command(string.Format(@"select FolderName  from tblFolder where folderId in (select ParentFolderId from tblFolder where folderId={0})", Convert.ToInt32(Session["FolderID"])));
                    //    string CommitteFname = ParentFolderNamess + "//" + Session["DMeetingdate"].ToString();
                    //    DateTime sfds = DateTime.Now;
                    //    string sdasd = sfds.ToString("yyyy-MM-dd HH:mm:ss");
                    //    string value1 = operation.ExecuteScalar4Command(string.Format(@"insert into tblfileactions (FileName,ActionName,UserID,ActionDate,Status,FolderId) values ('{0}','{1}',{2},'{3}','{4}',{5})", CommitteFname, DeliveryStatus1, Session["UserId"], sdasd, 1, Session["FolderId"].ToString()));
                    //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Message Sent Successfully')", true);
                    //}
                    //else
                    //{

                    //    string DeliveryStatus1 = "SMS not sent to " + no1 + " Error:" + output;
                    //    string ParentFolderNamess = operation.ExecuteScalar4Command(string.Format(@"select FolderName  from tblFolder where folderId in (select ParentFolderId from tblFolder where folderId={0})", Convert.ToInt32(Session["FolderID"])));
                    //    string CommitteFname = ParentFolderNamess + "//" + Session["DMeetingdate"].ToString();
                    //    DateTime sfds = DateTime.Now;
                    //    string sdasd = sfds.ToString("yyyy-MM-dd HH:mm:ss");
                    //    string value1 = operation.ExecuteScalar4Command(string.Format(@"insert into tblfileactions (FileName,ActionName,UserID,ActionDate,Status,FolderId) values ('{0}','{1}',{2},'{3}','{4}',{5})", CommitteFname, DeliveryStatus1, Session["UserId"], sdasd, 1, Session["FolderId"].ToString()));
                    //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Message not sent')", true);
                    //}
                    //createlogsms("Response :" + strResponse);

                }
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Message Sent Successfully')", true);
                pnlSms.Visible = false;

            }
            catch (Exception ex)
            {
                createlog("mail :" + Response + "," + ex.Message);
                string DeliveryStatus1 = "SMS not sent to " + no1 + " Error:" + ex.Message;
                string ParentFolderNamess = operation.ExecuteScalar4Command(string.Format(@"select FolderName  from tblFolder where folderId in (select ParentFolderId from tblFolder where folderId={0})", Convert.ToInt32(Session["FolderID"])));
                string CommitteFname = ParentFolderNamess + "//" + Session["DMeetingdate"].ToString();
                DateTime sfds = DateTime.Now;
                string sdasd = sfds.ToString("yyyy-MM-dd HH:mm:ss");
                string value1 = operation.ExecuteScalar4Command(string.Format(@"insert into tblfileactions (FileName,ActionName,UserID,ActionDate,Status,FolderId) values ('{0}','{1}',{2},'{3}','{4}',{5})", CommitteFname, DeliveryStatus1, Session["UserId"], sdasd, 1, Session["FolderId"].ToString()));
            }
        }
    }
    #endregion


    #region [ btnNO_Click]
    protected void btnNOSms_Click(object sender, EventArgs e)
    {
        pnlSms.Visible = false;
    }
    #endregion

    protected void gvData_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gvData.PageIndex = e.NewPageIndex;
        Session["myGVPageId"] = e.NewPageIndex;
        DirectorDataBinding();
    }
    protected void gvParent_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gvParent.PageIndex = e.NewPageIndex;
        bindLvFileView();
    }

    protected void rdcommittee_CheckedChanged(object sender, EventArgs e)
    {
        ddlCommittee.Enabled = true;
        ddlMeetingDate.Enabled = false;
    }


    protected void rdmeeting_CheckedChanged(object sender, EventArgs e)
    {
        ddlMeetingDate.Enabled = true;
        ddlCommittee.Enabled = false;
    }

    //protected void pnlSubmit_Click(object sender, EventArgs e)
    //{
    //    try
    //    {
    //        string FolderName = "";
    //        FolderName = operation.ExecuteScalar4Command(string.Format(@"select FolderName from tblFolder where FolderId={0}", Convert.ToInt32(Session["FolderID"])));
    //        if (FolderName.ToLower() != "recycle bin" || FolderName.ToLower() != "archived meetings")
    //        {
    //            if (rdmeeting.Checked == true && FolderName == ddlCommittee.SelectedItem.ToString())
    //            {
    //                string TableName = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Convert.ToInt32(Session["FolderID"])));
    //                foreach (GridViewRow row in gvParent.Rows)
    //                {
    //                    CheckBox chkSelect = (CheckBox)row.FindControl("chkSelect");
    //                    if (chkSelect.Checked)
    //                    {
    //                        Label Id = (Label)row.FindControl("lblId");
    //                        Label FileId = (Label)row.FindControl("lblFileId");
    //                        LinkButton FileName = (LinkButton)row.FindControl("lnkView");
    //                        string[] FileNameID = FileName.CommandArgument.ToString().Split(new Char[] { ',' });
    //                        string[] filename = FileNameID[1].Split(new Char[] { '.' });
    //                        string name = filename[0].ToString();
    //                        System.IO.File.Move(Server.MapPath("~\\Repository") + "\\" + name + "_" + Session["FolderID"].ToString() + ".enc", Server.MapPath("~\\Repository") + "\\" + name + "_" + ddlMeetingDate.SelectedValue.ToString() + ".enc");
    //                        string[] meetingdate = ddlMeetingDate.SelectedItem.ToString().Split('/');
    //                        string date = meetingdate[0];
    //                        string month = meetingdate[1];
    //                        string year = meetingdate[2];
    //                        string meetingdateinmmddyyy = month + '/' + date + '/' + year;

    //                        int value1 = Convert.ToInt16(operation.Insert4Command(string.Format(@"update {0} set folderid={1},column4='{2}' where id={3} and fileid={4}", TableName, Convert.ToInt32(ddlMeetingDate.SelectedValue.ToString()), meetingdateinmmddyyy, Convert.ToInt32(Id.Text.ToString()), Convert.ToInt32(FileId.Text.ToString()))));
    //                        int value2 = Convert.ToInt16(operation.Insert4Command(string.Format(@"update tblfile set folderid={0} where fileid={1}", Convert.ToInt32(ddlMeetingDate.SelectedValue.ToString()), Convert.ToInt32(FileId.Text.ToString()))));

    //                        if (value1 > 0)
    //                        {
    //                            ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('File Moved Successfully.')", true);
    //                        }
    //                    }
    //                }
    //            }
    //            else
    //            {
    //                string TableName = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", ddlCommittee.SelectedValue));
    //                foreach (GridViewRow row in gvParent.Rows)
    //                {
    //                    CheckBox chkSelect = (CheckBox)row.FindControl("chkSelect");
    //                    if (chkSelect.Checked)
    //                    {
    //                        Label Id = (Label)row.FindControl("lblId");
    //                        Label FileId = (Label)row.FindControl("lblFileId");
    //                        LinkButton FileName = (LinkButton)row.FindControl("lnkView");
    //                        string[] FileNameID = FileName.CommandArgument.ToString().Split(new Char[] { ',' });
    //                        string[] filename = FileNameID[1].Split(new Char[] { '.' });
    //                        string name = filename[0].ToString();
    //                        System.IO.File.Move(Server.MapPath("~\\Repository") + "\\" + name + "_" + Session["FolderID"].ToString() + ".enc", Server.MapPath("~\\Repository") + "\\" + name + "_" + ddlCommittee.SelectedValue.ToString() + ".enc");

    //                        DataSet dss = operation.GetDataSet4Command(string.Format(@"select * from tblFolderIndexMaster where folder_id={0}", Session["FolderID"].ToString()));
    //                        if (dss.Tables[0].Rows.Count > 0)
    //                        {
    //                            DataSet dsMeet = operation.GetDataSet4Command(string.Format(@"select * from {0} where FileId={1}", dss.Tables[0].Rows[0]["TableName"].ToString(), Convert.ToInt32(FileId.Text.ToString())));

    //                            if (dsMeet.Tables[0].Rows.Count > 0)
    //                            {

    //                                string ImportedBy = dsMeet.Tables[0].Rows[0]["ImportedBy"].ToString();
    //                                string DocStatus = dsMeet.Tables[0].Rows[0]["DocStatus"].ToString();
    //                                string Column3 = dsMeet.Tables[0].Rows[0]["Column3"].ToString();
    //                                string Column0 = dsMeet.Tables[0].Rows[0]["Column0"].ToString();
    //                                string Column1 = dsMeet.Tables[0].Rows[0]["Column1"].ToString();
    //                                string Column2 = dsMeet.Tables[0].Rows[0]["Column2"].ToString();
    //                                string value1 = operation.ExecuteScalar4Command(string.Format(@"insert into {0} (FileId,ImportedBy,DocStatus,FolderId,Column0,Column1,Column2,Column3) values ('" + Convert.ToInt32(FileId.Text.ToString()) + "','" + ImportedBy + "','" + DocStatus + "','" + ddlMeetingDate.SelectedValue.ToString() + "',  '" + Column0 + "','" + Column1 + "','" + Column2 + "','" + Column3 + "')", TableName));
    //                                string Deletestatement = string.Format("delete from {0} where Fileid={1}", dss.Tables[0].Rows[0]["TableName"].ToString(), Convert.ToInt32(FileId.Text.ToString()));
    //                                operation.Insert4Command(Deletestatement);

    //                            }
    //                        }
    //                        int value2 = Convert.ToInt16(operation.Insert4Command(string.Format(@"update tblfile set folderid={0} where fileid={1}", Convert.ToInt32(ddlMeetingDate.SelectedValue.ToString()), Convert.ToInt32(FileId.Text.ToString()))));

    //                    }
    //                }
    //            }


    //        }
    //        else
    //        {
    //            if (FolderName.ToLower() == "recycle bin" || FolderName.ToLower() == "archived meetings")
    //            {
    //                string TableName = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Convert.ToInt32(Session["FolderID"])));
    //                foreach (GridViewRow row in gvParent.Rows)
    //                {
    //                    CheckBox chkSelect = (CheckBox)row.FindControl("chkSelect");
    //                    if (chkSelect.Checked)
    //                    {
    //                        Label Id = (Label)row.FindControl("lblId");
    //                        Label FileId = (Label)row.FindControl("lblFileId");
    //                        LinkButton FileName = (LinkButton)row.FindControl("lnkView");
    //                        string[] FileNameID = FileName.CommandArgument.ToString().Split(new Char[] { ',' });
    //                        string[] filename = FileNameID[1].Split(new Char[] { '.' });
    //                        string name = filename[0].ToString();
    //                        System.IO.File.Move(Server.MapPath("~\\Repository") + "\\" + name + "_" + Session["FolderID"].ToString() + ".enc", Server.MapPath("~\\Repository") + "\\" + name + "_" + ddlMeetingDate.SelectedValue.ToString() + ".enc");
    //                        string[] meetingdate = ddlMeetingDate.SelectedItem.ToString().Split('/');
    //                        string date = meetingdate[0];
    //                        string month = meetingdate[1];
    //                        string year = meetingdate[2];
    //                        string meetingdateinmmddyyy = month + '/' + date + '/' + year;

    //                        int value1 = Convert.ToInt16(operation.Insert4Command(string.Format(@"update {0} set folderid={1},column4='{2}' where id={3} and fileid={4}", TableName, Convert.ToInt32(ddlMeetingDate.SelectedValue.ToString()), meetingdateinmmddyyy, Convert.ToInt32(Id.Text.ToString()), Convert.ToInt32(FileId.Text.ToString()))));
    //                        int value2 = Convert.ToInt16(operation.Insert4Command(string.Format(@"update tblfile set folderid={0} where fileid={1}", Convert.ToInt32(ddlMeetingDate.SelectedValue.ToString()), Convert.ToInt32(FileId.Text.ToString()))));

    //                        if (value1 > 0)
    //                        {
    //                            ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('File Moved Successfully.')", true);
    //                        }
    //                    }
    //                }
    //            }
    //            else
    //            {
    //                string TableName = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", ddlCommittee.SelectedValue));
    //                foreach (GridViewRow row in gvParent.Rows)
    //                {
    //                    CheckBox chkSelect = (CheckBox)row.FindControl("chkSelect");
    //                    if (chkSelect.Checked)
    //                    {
    //                        Label Id = (Label)row.FindControl("lblId");
    //                        Label FileId = (Label)row.FindControl("lblFileId");
    //                        LinkButton FileName = (LinkButton)row.FindControl("lnkView");
    //                        string[] FileNameID = FileName.CommandArgument.ToString().Split(new Char[] { ',' });
    //                        string[] filename = FileNameID[1].Split(new Char[] { '.' });
    //                        string name = filename[0].ToString();
    //                        System.IO.File.Move(Server.MapPath("~\\Repository") + "\\" + name + "_" + Session["FolderID"].ToString() + ".enc", Server.MapPath("~\\Repository") + "\\" + name + "_" + ddlCommittee.SelectedValue.ToString() + ".enc");

    //                        DataSet dss = operation.GetDataSet4Command(string.Format(@"select * from tblFolderIndexMaster where folder_id={0}", Session["FolderID"].ToString()));
    //                        if (dss.Tables[0].Rows.Count > 0)
    //                        {
    //                            DataSet dsMeet = operation.GetDataSet4Command(string.Format(@"select * from {0} where FileId={1}", dss.Tables[0].Rows[0]["TableName"].ToString(), Convert.ToInt32(FileId.Text.ToString())));

    //                            if (dsMeet.Tables[0].Rows.Count > 0)
    //                            {

    //                                string ImportedBy = dsMeet.Tables[0].Rows[0]["ImportedBy"].ToString();
    //                                string DocStatus = dsMeet.Tables[0].Rows[0]["DocStatus"].ToString();
    //                                string Column3 = dsMeet.Tables[0].Rows[0]["Column3"].ToString();
    //                                string Column0 = dsMeet.Tables[0].Rows[0]["Column0"].ToString();
    //                                string Column1 = dsMeet.Tables[0].Rows[0]["Column1"].ToString();
    //                                string Column2 = dsMeet.Tables[0].Rows[0]["Column2"].ToString();
    //                                string value1 = operation.ExecuteScalar4Command(string.Format(@"insert into {0} (FileId,ImportedBy,DocStatus,Column0,Column1,Column2,Column3) values ('" + Convert.ToInt32(FileId.Text.ToString()) + "','" + ImportedBy + "','" + DocStatus + "',  '" + Column0 + "','" + Column1 + "','" + Column2 + "','" + Column3 + "')", TableName));
    //                                string Deletestatement = string.Format("delete from {0} where Fileid={1}", dss.Tables[0].Rows[0]["TableName"].ToString(), Convert.ToInt32(FileId.Text.ToString()));
    //                                operation.Insert4Command(Deletestatement);

    //                            }
    //                        }
    //                        int value2 = Convert.ToInt16(operation.Insert4Command(string.Format(@"update tblfile set folderid={0} where fileid={1}", Convert.ToInt32(ddlCommittee.SelectedValue.ToString()), Convert.ToInt32(FileId.Text.ToString()))));

    //                    }
    //                }
    //            }
    //        }
    //        bindLvFileView();
    //    }
    //    catch (System.Exception ex)
    //    {
    //        Session["Error"] = ex.StackTrace;
    //    }
    //    finally
    //    {
    //        objCommonBAL = null;
    //        objFileUploadBAL = null;
    //    }
    //    pnlMove.Visible = false;
    //}

    protected void btnActivate_Click(object sender, EventArgs e)
    {
        int stractivate = Convert.ToInt32(operation.Insert4Command(string.Format(@"update tblfolder set MeetingCancelled=0 where FolderID='{0}'", Session["FolderID"].ToString())));
        if (stractivate > 0)
        {
            ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('Meeting Activated Successfully.')", true);
            Response.Redirect("../Default/Default.aspx");
            gvData.Visible = false;
        }

    }
    //protected void pnlCancelMove_Click(object sender, EventArgs e)
    //{
    //    pnlMove.Visible = false;
    //    bindLvFileView();
    //}
    protected void ddlCommittee_SelectedIndexChanged1(object sender, EventArgs e)
    {

    }

    protected void GridView2_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GridView2.PageIndex = e.NewPageIndex;
        GetAgendaDetails();
    }

    protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GridView1.PageIndex = e.NewPageIndex;
        GetAgendaDetails();
    }


    protected void btnYesnew1_Click(object sender, EventArgs e)
    {
        string strAgendatablename = operation.ExecuteScalar4Command(string.Format(@"select foldername from tblfolder where FolderID={0}", Convert.ToInt32(Session["FolderID"])));

        string str = "";
        if (strAgendatablename.ToLower() == "binani cement")
        {
            foreach (GridViewRow row in GridView2.Rows)
            {
                CheckBox chkSelected = (CheckBox)row.FindControl("chkSelected");
                Label lblFileId = (Label)row.FindControl("lblFileIded");
                Label lblFileName = (Label)row.FindControl("lblItemNoed");
                if (chkSelected.Checked)
                {
                    if (ddlCommitteName.SelectedIndex != 0)
                    {
                        str = ddlMeetingDateNew.SelectedValue.ToString();
                        Session["FileID"] = lblFileId.Text;
                        Session["FileName"] = lblFileName.Text;
                        string[] filename = lblFileName.Text.Split(new Char[] { '.' });
                        string name = filename[0].ToString();
                        System.IO.File.Move(Server.MapPath("~\\Repository") + "\\" + name + "_" + Session["FolderID"].ToString() + ".enc", Server.MapPath("~\\Repository") + "\\" + name + "_" + str + ".enc");
                        string TableName = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Convert.ToInt32(str)));
                        string TableNameFrom = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Session["FolderID"]));
                        string value1 = operation.ExecuteScalar4Command(string.Format(@"insert into {0}(FileId,ImportedBy,ImportedOn,DocStatus,FolderId,Column1)(select FileId,ImportedBy,ImportedOn,DocStatus,FolderId,Column0 from {1} where FileId='{2}')", TableName, TableNameFrom, lblFileId.Text.ToString()));
                        string Deletestatement = string.Format("delete from {0} where Fileid={1}", TableNameFrom, Convert.ToInt32(lblFileId.Text.ToString()));
                        operation.Insert4Command(Deletestatement);
                        int value2 = Convert.ToInt16(operation.Insert4Command(string.Format(@"update tblfile set folderid={0} where fileid={1}", Convert.ToInt32(str), Convert.ToInt32(lblFileId.Text.ToString()))));
                        string DeleteAgenda = string.Format("Update tblAgendaApprovalDetail Set FinalAgendaApprovalStatus=2 where Fileid={0}", Convert.ToInt32(lblFileId.Text.ToString()));
                        operation.Insert4Command(DeleteAgenda);
                        if (value2 > 0)
                        {
                            ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('File Moved Successfully.')", true);
                        }
                    }
                }
            }
            GetAgendaDetails();
        }
        else
        {
            foreach (GridViewRow row in GridView2.Rows)
            {
                CheckBox chkSelected = (CheckBox)row.FindControl("chkSelected");
                Label lblFileId = (Label)row.FindControl("lblFileIded");
                Label lblFileName = (Label)row.FindControl("lblItemNoed");
                if (chkSelected.Checked)
                {
                    if (ddlCommitteName.SelectedIndex != 0)
                    {
                        str = ddlMeetingDateNew.SelectedValue.ToString();
                        Session["FileID"] = lblFileId.Text;
                        Session["FileName"] = lblFileName.Text;
                        string[] filename = lblFileName.Text.Split(new Char[] { '.' });
                        string name = filename[0].ToString();
                        System.IO.File.Move(Server.MapPath("~\\Repository") + "\\" + name + "_" + Session["FolderID"].ToString() + ".enc", Server.MapPath("~\\Repository") + "\\" + name + "_" + str + ".enc");
                        string TableName = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Convert.ToInt32(str)));
                        string TableNameFrom = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Session["FolderID"]));
                        string value1 = operation.ExecuteScalar4Command(string.Format(@"insert into {0}(FileId,ImportedBy,ImportedOn,DocStatus,FolderId,Column1)(select FileId,ImportedBy,ImportedOn,DocStatus,FolderId,Column0 from {1} where FileId='{2}')", TableName, TableNameFrom, lblFileId.Text.ToString()));
                        string Deletestatement = string.Format("delete from {0} where Fileid={1}", TableNameFrom, Convert.ToInt32(lblFileId.Text.ToString()));
                        operation.Insert4Command(Deletestatement);
                        int value2 = Convert.ToInt16(operation.Insert4Command(string.Format(@"update tblfile set folderid={0} where fileid={1}", Convert.ToInt32(str), Convert.ToInt32(lblFileId.Text.ToString()))));
                        string DeleteAgenda = string.Format("Update tblAgendaApprovalDetail Set FinalAgendaApprovalStatus=2 where Fileid={0}", Convert.ToInt32(lblFileId.Text.ToString()));
                        operation.Insert4Command(DeleteAgenda);
                        if (value2 > 0)
                        {
                            ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('File Moved Successfully.')", true);
                        }
                    }
                }
            }
            GetAgendaDetailsforBoard();

        }
        Panel7.Visible = false;
    }

    protected void btnNOnew1_Click(object sender, EventArgs e)
    {
        Panel7.Visible = false;
    }

    //protected void btnnotebook_Click(object sender, EventArgs e)
    //{
    //    try
    //    {
    //        DateTime sfds = DateTime.Now;
    //        string sdasd = sfds.ToString("yyyy-MM-dd HH:mm:ss");
    //        OperationClass operation = new OperationClass();
    //        GenerateIndexPage();
    //        MergePdfFiles();
    //        string value1 = operation.ExecuteScalar4Command(string.Format(@"insert into tblfileactions (FileName,ActionName,UserID,ActionDate,FolderId) values ('{0}','{1}',{2},'{3}','{4}')", Session["DMeetingdate"].ToString(), "Note Book Downloaded", Session["UserId"], sdasd, Session["FolderId"].ToString()));

    //    }
    //    catch(Exception ex)
    //    {
    //    }

    //}
    protected void btnnotebook_Click(object sender, EventArgs e)
    {
        OperationClass operation = new OperationClass();
        try
        {
            string ParentFolderName = operation.ExecuteScalar4Command(string.Format(@"select FolderName  from tblFolder where folderId in (select ParentFolderId from tblFolder where folderId={0})", Convert.ToInt32(Session["FolderID"])));
            string CommitteFname = ParentFolderName + "//" + Session["DMeetingdate"].ToString();
            DateTime sfds = DateTime.Now;
            string sdasd = sfds.ToString("yyyy-MM-dd HH:mm:ss");
            GenerateIndexPage();
            MergePdfFiles();
            //MergePdfFilesWithoutPwdforDirect();
            string value1 = operation.ExecuteScalar4Command(string.Format(@"insert into tblfileactions (FileName,ActionName,UserID,ActionDate,Status,FolderId) values ('{0}','{1}',{2},'{3}','{4}',{5})", CommitteFname, "Note Book Downloaded", Session["UserId"], sdasd, 1, Session["FolderId"].ToString()));

        }
        catch (Exception ex)
        {
        }

    }
    protected void btnwaterpwd_Click(object sender, EventArgs e)
    {
        OperationClass operation = new OperationClass();
        try
        {
            //string ParentFolderName = operation.ExecuteScalar4Command(string.Format(@"select FolderName  from tblFolder where folderId in (select ParentFolderId from tblFolder where folderId={0})", Convert.ToInt32(Session["FolderID"])));
            //string CommitteFname = ParentFolderName + "//" + Session["DMeetingdate"].ToString();
            //DateTime sfds = DateTime.Now;
            //string sdasd = sfds.ToString("yyyy-MM-dd HH:mm:ss");
            GenerateIndexPage();
            MergePdfFiles();
            //string value1 = operation.ExecuteScalar4Command(string.Format(@"insert into tblfileactions (FileName,ActionName,UserID,ActionDate,Status,FolderId) values ('{0}','{1}',{2},'{3}','{4}',{5})", CommitteFname, "Note Book Downloaded", Session["UserId"], sdasd, 1, Session["FolderId"].ToString()));
        }
        catch (Exception ex)
        {
        }

    }
    protected void btnwaterwoutpwd_Click(object sender, EventArgs e)
    {
        OperationClass operation = new OperationClass();
        try
        {
            //string ParentFolderName = operation.ExecuteScalar4Command(string.Format(@"select FolderName  from tblFolder where folderId in (select ParentFolderId from tblFolder where folderId={0})", Convert.ToInt32(Session["FolderID"])));
            //string CommitteFname = ParentFolderName + "//" + Session["DMeetingdate"].ToString();
            //DateTime sfds = DateTime.Now;
            //string sdasd = sfds.ToString("yyyy-MM-dd HH:mm:ss");
            GenerateIndexPage();
            MergePdfFilesWithoutPwd();

            //string value1 = operation.ExecuteScalar4Command(string.Format(@"insert into tblfileactions (FileName,ActionName,UserID,ActionDate,Status,FolderId) values ('{0}','{1}',{2},'{3}','{4}',{5})", CommitteFname, "Note Book Downloaded", Session["UserId"], sdasd, 1, Session["FolderId"].ToString()));
        }
        catch (Exception ex)
        {
        }

    }

    protected void btnpwdwoutwater_Click(object sender, EventArgs e)
    {
        OperationClass operation = new OperationClass();
        try
        {
            //string ParentFolderName = operation.ExecuteScalar4Command(string.Format(@"select FolderName  from tblFolder where folderId in (select ParentFolderId from tblFolder where folderId={0})", Convert.ToInt32(Session["FolderID"])));
            //string CommitteFname = ParentFolderName + "//" + Session["DMeetingdate"].ToString();
            //DateTime sfds = DateTime.Now;
            //string sdasd = sfds.ToString("yyyy-MM-dd HH:mm:ss");btnwithoutPwdwater_Click
            GenerateIndexPage();
            MergePdfFilesWithPwdWithoutWaterMark();
            //string value1 = operation.ExecuteScalar4Command(string.Format(@"insert into tblfileactions (FileName,ActionName,UserID,ActionDate,Status,FolderId) values ('{0}','{1}',{2},'{3}','{4}',{5})", CommitteFname, "Note Book Downloaded", Session["UserId"], sdasd, 1, Session["FolderId"].ToString()));
        }
        catch (Exception ex)
        {
        }

    }
    protected void btnwithoutPwdwater_Click(object sender, EventArgs e)
    {
        OperationClass operation = new OperationClass();
        try
        {
            //string ParentFolderName = operation.ExecuteScalar4Command(string.Format(@"select FolderName  from tblFolder where folderId in (select ParentFolderId from tblFolder where folderId={0})", Convert.ToInt32(Session["FolderID"])));
            //string CommitteFname = ParentFolderName + "//" + Session["DMeetingdate"].ToString();
            //DateTime sfds = DateTime.Now;
            //string sdasd = sfds.ToString("yyyy-MM-dd HH:mm:ss");btnwithoutPwdwater_Click
            GenerateIndexPage();
            MergePdfFilesWithoutPwdandWaterMark();
            //string value1 = operation.ExecuteScalar4Command(string.Format(@"insert into tblfileactions (FileName,ActionName,UserID,ActionDate,Status,FolderId) values ('{0}','{1}',{2},'{3}','{4}',{5})", CommitteFname, "Note Book Downloaded", Session["UserId"], sdasd, 1, Session["FolderId"].ToString()));
        }
        catch (Exception ex)
        {
        }

    }
    private void MergePdfFilesWithoutPwdforDirect()
    {
        try
        {
            CommonBAL objCommonBAL = new CommonBAL();
            OperationClass operation = new OperationClass();
            int loopcount = 0;
            //Session["UserSelected"] = ddlNote.SelectedValue.ToString();
            string DestinationPath1 = null;
            string DestinationPath = null;
            string sourceFileName = null;
            string inderFileName = null;
            string FolderId = Session["FolderId"].ToString();
            string Foldername = Session["DMeetingdate"].ToString();
            Session["FolderId"] = FolderId;
            Session["Foldername"] = Foldername;
            string trg = "Agenda_" + Session["Foldername"] + "";
            string trgt = trg.Replace("/", "-");
            string ImagesavedFilePath = Convert.ToString(HttpContext.Current.Server.MapPath("~\\ExportFile\\" + Convert.ToString(HttpContext.Current.Session["UserName"]).Trim()));

            string sdfsdf = ImagesavedFilePath + "\\" + trgt;

            if (!Directory.Exists(ImagesavedFilePath))
            {
                Directory.CreateDirectory(ImagesavedFilePath);
            }
            else
            {
                objCommonBAL.DeleteTempFilesFromExportFolder(ImagesavedFilePath);
            }
            string IFilePath = Convert.ToString(HttpContext.Current.Server.MapPath("~//Report//PdfReports//AccessDenie//" + Convert.ToString(HttpContext.Current.Session["UserName"]).Trim()));
            if (!Directory.Exists(ImagesavedFilePath))
            {
                Directory.CreateDirectory(IFilePath);
            }
            else
            {
                objCommonBAL.DeleteTempFilesFromExportFolder(IFilePath);
            }
            //string stout = sdfsdf + trgt + ".pdf";
            string stout = sdfsdf + ".pdf";


            string strdt = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblfolderindexmaster where folder_id={0}", FolderId));
            //  DataTable dt2 = operation.GetTable4Command(string.Format(@"select * from tblfile where FileId in (select FileId from {0} where FolderId = {1})", strdt, FolderId));
            DataTable dt2 = operation.GetTable4Command(string.Format(@"select tblfile.FileID ,tblfile.[FileName],{0}.Column0 from tblfile inner join {0} on tblfile.FileID = {0}.FileId where {0}.FolderId ={1} order by {0}.Column0", strdt, FolderId));
            if (dt2.Rows.Count > 0)
            {
                string[] stin = new string[dt2.Rows.Count + 1];
                Session["FileID"] = dt2.Rows[0]["FileID"];
                Session["FileName"] = dt2.Rows[0]["FileName"];
                int count = dt2.Rows.Count;
                string[] strfilename = new string[count];
                string[] strFileId = new string[count];
                string DecryptFilePath;
                DecryptFilePath = Convert.ToString(Server.MapPath("~/Repository//"));
                DestinationPath1 = Convert.ToString(Server.MapPath("~/Repository//Decrypt//" + Session["UserName"].ToString()));
                objCommonBAL.DeleteTempFilesFromExportFolder(DestinationPath1);

                //objCommonBAL.DeleteTempFilesFromExportFolder(DecryptFilePath);

                for (int i = 0; i <= dt2.Rows.Count - 1; i++)
                {
                    if (Path.GetExtension(dt2.Rows[i]["FileName"].ToString()).ToLower() == ".docx" || Path.GetExtension(dt2.Rows[i]["FileName"].ToString()).ToLower() == ".xlsx" || Path.GetExtension(dt2.Rows[i]["FileName"].ToString()).ToLower() == ".pptx")
                    {
                        strfilename[i] = Path.GetFileNameWithoutExtension(dt2.Rows[i]["FileName"].ToString()) + ".pdf";
                    }
                    else
                    {
                        strfilename[i] = dt2.Rows[i]["FileName"].ToString();
                    }
                    strFileId[i] = dt2.Rows[i]["FileID"].ToString();
                    Session["FileID"] = strFileId[i];
                    Session["FileName"] = strfilename[i];

                    if (Path.GetExtension(strfilename[i].ToLower()) == ".pdf")
                    {
                        string strfileId = Session["FileID"].ToString();
                        //strFileName = Session["FileName"].ToString();


                        //List<string> lstReturnCodePath = objCommonBAL.CheckImageStorageEncryptDecryptPDFImage(DecryptFilePath, Convert.ToString(strfilename[i]), strFileId[i], "1", "", "");
                        //List<string> lstReturnCodePath = objCommonBAL.CheckImageStorageEncryptDecryptImage(DecryptFilePath, Convert.ToString(strfilename[i]), strFileId[i], "1", "", "");



                        string[] _strFileName = new string[count];
                        _strFileName[i] = strfilename[i];
                        sourceFileName = (DecryptFilePath + Path.GetFileNameWithoutExtension(_strFileName[i]) + "_" + (strFileId[i]) + Path.GetExtension(_strFileName[i]));


                        inderFileName = Server.MapPath("~/Report//PdfReports//AgendaIndex.pdf");

                        //DestinationPath = Convert.ToString(Server.MapPath(DestinationPath1 + "//" + Path.GetFileNameWithoutExtension(_strFileName[i]) + "_" + (HttpContext.Current.Session["FileID"]) + Path.GetExtension(_strFileName[i])));
                        //DestinationPath = Convert.ToString(Server.MapPath(DestinationPath1 + "//" + Path.GetFileNameWithoutExtension(_strFileName[i]) + "_" + (HttpContext.Current.Session["FileID"]) + Path.GetExtension(_strFileName[i])));
                        if (loopcount == 0)
                        {
                            if (File.Exists(inderFileName))
                            {

                                string indexfiledelete = Server.MapPath("~/Report//PdfReports");
                                loopcount = loopcount + 1;
                                stin[loopcount - 1] = inderFileName;
                                //objCommonBAL.DeleteTempFilesFromExportFolder(indexfiledelete);

                            }
                        }
                        //else
                        //{
                        if (File.Exists(sourceFileName))
                        {

                            //File.Copy(sourceFileName, DestinationPath, true);
                            loopcount = loopcount + 1;
                            string attach = dt2.Rows[i]["Column0"].ToString();
                            Session["Attach"] = "";

                            dtDeniePath = operation.GetTable4Command(string.Format(@"select tblAgendalevelAccessControl.Access,tblfile.PageCount from tblAgendalevelAccessControl inner join tblfile on tblAgendalevelAccessControl.FileID=tblfile.FileID where tblAgendalevelAccessControl.fileid='{0}' and tblAgendalevelAccessControl.userid='{1}'", dt2.Rows[i]["fileid"].ToString(), Session["UserID"]));
                            if (attach.Length >= 3)
                            {
                                string strSub = attach.Substring(0, 2);
                                string fileid = operation.ExecuteScalar4Command(string.Format(@"select TOP 1 FILEID from {0}  WHERE COLUMN0 LIKE '{1}%' AND FOLDERID={2}", strdt, strSub, Session["FolderID"]));

                                dtDeniePath = operation.GetTable4Command(string.Format(@"SELECT * FROM TBLAGENDALEVELACCESSCONTROL WHERE FILEID='{0}' AND USERID='{1}'", fileid, Session["UserID"]));
                            }

                            if (dtDeniePath.Rows.Count > 0)
                            {

                                //if (dtDeniePath.Rows[0]["Access"].ToString() != "true" || attach.Length >= 3)
                                //{

                                //string AccessPath = Convert.ToString(Server.MapPath("~/Report//PdfReports//AccessDenie//sukumal"));

                                //if (!Directory.Exists(AccessPath))
                                //{
                                //    Directory.CreateDirectory(AccessPath);
                                //}
                                //string AccessRestrict = Convert.ToString(Server.MapPath("~/Report//PdfReports//AccessDenie//sukumal//AccessDenie.pdf"));


                                string AccessRestrict = "";
                                FileGenerateWithAccessDenied(AccessRestrict, dt2.Rows[i]["FileID"].ToString());
                                FileDownLoadwithAccessDenied(AccessRestrict, dt2.Rows[i]["FileID"].ToString());
                                AccessDeniePath = Convert.ToString(Server.MapPath("~//Report//PdfReports//AccessDenie//" + Session["UserSelected"].ToString() + "//" + "AccessDenie" + Session["FileID"] + "pdf.pdf"));
                                stin[loopcount - 1] = AccessDeniePath;
                                sourceFileName = "";
                                //}
                                //}
                                Session["Attach"] = dt2.Rows[i]["FileID"].ToString();
                            }
                            else
                            {
                                stin[loopcount - 1] = sourceFileName;
                            }

                        }
                        //}

                    }

                }
                loopcount = 0;
                int value;
                value = MergePDFDocuments(stin, stout, 1);
                if (value > 0)
                {

                    AddPageNumber();

                    //FileDownLoadwithWaterMark(stout);
                    FileDownLoadwithWaterMarkWithoutPwd(stout);

                }

            }
        }
        catch (Exception ex)
        {

        }
    }

    protected void FileGenerateWithAccessDenied(string filePath, string fileid)
    {
        try
        {

            //Create document
            Document doc = new Document();
            //Create PDF Table
            PdfPTable tableLayout = new PdfPTable(2);
            PdfPTable tableLayout2 = new PdfPTable(2);

            PdfPTable tableLayout1 = new PdfPTable(3);

            PdfPTable tableLayout4 = new PdfPTable(2);
            PdfPTable tableLayout3 = new PdfPTable(1);
            DataCon dc = new DataCon();
            string DestinationPath = Server.MapPath("~//Report//PdfReports//AccessDenie//" + Session["UserName"].ToString() + "//" + "AccessDenie" + ".pdf");

            string ssss = Server.MapPath("~/Report//PdfReports//AccessDenie//") + Session["UserName"].ToString();
            if (!Directory.Exists(ssss))
            {
                Directory.CreateDirectory(ssss);
            }

            //string AccessPath = "~/Report//PdfReports//AccessDenie//" + Session["UserName"].ToString();
            //if (!Directory.Exists(AccessPath))
            //{
            //    Directory.CreateDirectory(AccessPath);
            //}
            string Fillpath = ssss + "//" + "AccessDenie.pdf";

            //Create a PDF file in specific path Session["UserName"]
            PdfWriter.GetInstance(doc, new FileStream(HttpContext.Current.Server.MapPath("~//Report//PdfReports//AccessDenie//" + Session["UserName"].ToString() + "//" + "AccessDenie" + fileid + ".pdf"), FileMode.Create));


            //Open the PDF document
            doc.Open();
            string FolderId = Session["FolderId"].ToString();
            string strdt = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblfolderindexmaster where folder_id={0}", FolderId));
            //  DataTable dt2 = operation.GetTable4Command(string.Format(@"select * from tblfile where FileId in (select FileId from {0} where FolderId = {1})", strdt, FolderId));
            DataTable dt2 = operation.GetTable4Command(string.Format(@"select tblfile.FileID ,tblfile.[FileName],{0}.Column0 from tblfile inner join {0} on tblfile.FileID = {0}.FileId where {0}.FolderId ={1} order by {0}.Column0", strdt, FolderId));


            //string strAccess = obj.ExecuteScalar4Command(string.Format(@"select Access from tblAgendalevelAccessControl where fileid='{0}' and userid='{1}'", dt2.Rows[k]["fileid"].ToString(), Session["UserID"]));Session["FileID"]
            //string strAttachmentAccess = "";
            //if (strAccess.ToLower() != "true")
            //{

            //    string sstttttt = dtttrrr.Rows[k]["Column0"].ToString().Substring(0, 2);
            //    string strrrrrFileID = obj.ExecuteScalar4Command(string.Format(@"select top 1 Fileid  as column0 from {0} where column0 like '{1}' and Folderid='{2}'", TableNameFrom, sstttttt, Session["FolderID"]));
            //    strAttachmentAccess = obj.ExecuteScalar4Command(string.Format(@"select Access from tblAgendalevelAccessControl where fileid='{0}' and userid='{1}'", strrrrrFileID, Session["UserID"]));
            //}
            //if (strAccess.ToLower() == "true")
            //{
            //    sourceFileName = Server.MapPath("~/Repository/PageTiffFiles") + "//" + "AccessDenied.tif";

            //}
            //else
            //    if (strAttachmentAccess.ToLower() == "true")
            //    {
            //        sourceFileName = Server.MapPath("~/Repository/PageTiffFiles") + "//" + "AccessDenied.tif";
            //    }
            //    else
            //    {
            //        sourceFileName = Server.MapPath("~/Repository/PageTiffFiles") + "//" + Path.GetFileNameWithoutExtension(dtttrrr.Rows[k]["filename"].ToString()) + "_" + dtttrrr.Rows[k]["fileid"].ToString() + ".pdf" + (i + 1).ToString() + ".tif";
            //    }

            DataTable dtFroattach = operation.GetTable4Command(string.Format(@"select PageCount from tblFile where FileID={0}", fileid));


            DataTable dtDeniePath = operation.GetTable4Command(string.Format(@"select tblAgendalevelAccessControl.Access,tblfile.PageCount from tblAgendalevelAccessControl inner join tblfile on tblAgendalevelAccessControl.FileID=tblfile.FileID where tblAgendalevelAccessControl.fileid='{0}' and tblAgendalevelAccessControl.userid='{1}'", fileid, Session["UserID"]));
            if (dtDeniePath.Rows.Count > 0)
            {
                if (dtDeniePath.Rows[0]["Access"].ToString() != "true" && dtDeniePath.Rows[0]["PageCount"].ToString() != "")
                {
                    string count = dtDeniePath.Rows[0]["PageCount"].ToString();
                    int count1 = Convert.ToInt32(count);
                    for (int i = 1; i <= count1; i++)
                    {
                        Font blackFont = FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.BLACK);
                        //doc.Add(Add_Content_To_PDF31(tableLayout3));
                        doc.NewPage();
                        //doc.Add(new Paragraph(string.Format("{0}", i)));
                        doc.Add(new Paragraph("."));
                    }
                }
            }
            else if (dtFroattach.Rows.Count > 0)
            {
                if (dtFroattach.Rows[0]["PageCount"].ToString() != "")
                {
                    string count = dtFroattach.Rows[0]["PageCount"].ToString();

                    int count1 = Convert.ToInt32(count);
                    //double acount = count1 % 2;
                    if (count1 > 1)
                    {
                        for (int i = 1; i <= Convert.ToInt32(count1); i++)
                        {
                            Font blackFont = FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.BLACK);
                            //doc.Add(Add_Content_To_PDFattachment(tableLayout3));
                            doc.NewPage();
                            doc.Add(new Paragraph("."));
                            //ColumnText.ShowTextAligned(stamper.GetOverContent(i), Element.TITLE, new Phrase("Note:" + "hi" + ":" + "hkkkkk", blackFont), 270, 750, 1f);
                        }
                    }
                    else
                    {
                        for (int i = 1; i <= Convert.ToInt32(count1); i++)
                        {

                            //doc.Add(Add_Content_To_PDF31(tableLayout3));
                            doc.NewPage();
                            doc.Add(new Paragraph("."));
                        }
                    }
                }
            }
            // Closing the document
            doc.Close();

        }
        catch (Exception es)
        {
        }
    }

    protected void FileDownLoadwithAccessDenied(string filePath, string FileId)
    {
        //string watermarkText = "Naresh Srinivas Singu";
        OperationClass operation = new OperationClass();
        string strdt = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblfolderindexmaster where folder_id={0}", Session["FolderId"].ToString()));

        DataTable dt2 = operation.GetTable4Command(string.Format(@"select * from {0} where FileId={1}", strdt, FileId));
        string Destinationfile = Server.MapPath("~//Report//PdfReports//AccessDenie//" + Session["UserName"].ToString() + "//" + "AccessDenie" + FileId + ".pdf");

        try
        {
            if (Session["Name"] != null)
            {

                string watermarkText = "Access Restricted";
                string ModifiedFileName = string.Empty;
                object TargetFile = Destinationfile;
                iTextSharp.text.pdf.PdfReader reader1 = new iTextSharp.text.pdf.PdfReader(TargetFile.ToString());
                ModifiedFileName = TargetFile.ToString();
                ModifiedFileName = ModifiedFileName.Insert(ModifiedFileName.Length - 4, "pdf");
                //PdfReader reader1 = new PdfReader(filePath);
                //using (FileStream fs = new FileStream(OutLocation, FileMode.Create, FileAccess.Write, FileShare.None))
                using (FileStream fs = new FileStream(ModifiedFileName, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    //iTextSharp.text.pdf.PdfEncryptor.Encrypt(reader1, fs, iTextSharp.text.pdf.PdfWriter.STRENGTH128BITS, Session["txtboxPassword"].ToString(), Session["txtboxPassword"].ToString(), iTextSharp.text.pdf.PdfWriter.AllowPrinting);

                    using (PdfStamper stamper = new PdfStamper(reader1, fs))
                    {

                        int pageCount1 = reader1.NumberOfPages;
                        //Create a new layer
                        PdfLayer layer = new PdfLayer("WatermarkLayer", stamper.Writer);
                        //PdfLayer layer1 = new PdfLayer("WatermarkLayer", stamper.Writer);
                        for (int i = 1; i <= pageCount1; i++)
                        {
                            iTextSharp.text.Rectangle rect = reader1.GetPageSize(i);
                            //Get the ContentByte object
                            PdfContentByte cb = stamper.GetOverContent(i);
                            //Tell the CB that the next commands should be "bound" to this new layer
                            cb.BeginLayer(layer);
                            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED), 30);
                            PdfGState gState = new PdfGState();
                            gState.FillOpacity = 0.50f;
                            cb.SetGState(gState);
                            cb.SetColorFill(BaseColor.BLACK);
                            cb.BeginText();
                            cb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, watermarkText, 270, 600, 0f);
                            cb.EndText();
                            ////"Close" the layer
                            ////cb1.EndLayer();
                            //cb.EndLayer();
                            PdfContentByte cb1 = stamper.GetOverContent(i);
                            ////Tell the CB that the next commands should be "bound" to this new layer
                            cb1.BeginLayer(layer);
                            cb1.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED), 13);
                            PdfGState gState1 = new PdfGState();
                            gState1.FillOpacity = 0.50f;
                            cb1.SetGState(gState);
                            cb1.SetColorFill(BaseColor.BLACK);
                            cb1.BeginText();

                            if (dt2.Rows[0]["Column1"].ToString() != "")
                            {
                                //cb1.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "vani", 270, 800, 1f);
                                cb1.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "Note:" + dt2.Rows[0]["Column0"] + ":" + dt2.Rows[0]["Column1"], 270, 800, 0f);//currect
                                //string Pdffont = dt2.Rows[0]["Column1"].ToString();
                                //BaseFont baseFont = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, "", false);
                                //cb1.SetFontAndSize(baseFont, 13); // 40 point font
                                //cb1.SetTextMatrix(25,788);
                                //cb1.SetColorFill(BaseColor.BLACK);
                                //cb1.ShowText(dt2.Rows[0]["Column5"] + " " + dt2.Rows[0]["Column1"]);

                                ////cb1.ShowTextAligned(PdfContentByte.ALIGN_CENTER, dt2.Rows[0]["Column5"] + " " + dt2.Rows[0]["Column1"], 270, 800, 0f);
                                ////Font blackFont = FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.BLACK);
                                cb1.EndText();
                                cb1.EndLayer();

                                //Font blackFont = FontFactory.GetFont("georgia", 13, Font.NORMAL, BaseColor.BLACK);
                                //ColumnText.ShowTextAligned(stamper.GetOverContent(i), Element.ALIGN_RIGHT, new Phrase(dt2.Rows[0]["Column5"] + " " + dt2.Rows[0]["Column1"], blackFont), 75f, 790f, 0);

                            }
                            else
                            {
                                //BaseFont baseFont = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, "", false);
                                //cb1.SetFontAndSize(baseFont, 13); // 40 point font
                                //cb1.SetTextMatrix(30, 790);
                                //cb1.SetColorFill(BaseColor.BLACK);
                                //cb1.ShowText(dt2.Rows[0]["Column5"] + " " + dt2.Rows[0]["Column1"]);
                                //cb1.EndText();
                                //cb1.EndLayer();
                                cb1.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "Note:" + dt2.Rows[0]["Column0"] + ":" + dt2.Rows[0]["Column2"], 270, 800, 0f);//currect
                                cb1.EndText();
                                cb1.EndLayer();

                                //Font blackFont = FontFactory.GetFont("georgia", 13, Font.NORMAL, BaseColor.BLACK);
                                //ColumnText.ShowTextAligned(stamper.GetOverContent(i), Element.ALIGN_RIGHT, new Phrase(dt2.Rows[0]["Column5"] + " " + dt2.Rows[0]["Column1"], blackFont), 40f, 800f, 0);
                            }



                        }

                    }
                }


                //PdfFileDownLoad(ModifiedFileName);

            }
        }
        catch (Exception es)
        {
        }
    }

    private void MergePdfFilesWithoutPwd()
    {
        try
        {
            CommonBAL objCommonBAL = new CommonBAL();
            OperationClass operation = new OperationClass();
            int loopcount = 0;

            string DestinationPath1 = null;
            string DestinationPath = null;
            string sourceFileName = null;
            string inderFileName = null;
            string FolderId = Session["FolderId"].ToString();
            string Foldername = Session["DMeetingdate"].ToString();
            Session["FolderId"] = FolderId;
            Session["Foldername"] = Foldername;
            string trg = "Agenda_" + Session["Foldername"] + "";
            string trgt = trg.Replace("/", "-");
            string ImagesavedFilePath = Convert.ToString(HttpContext.Current.Server.MapPath("~\\ExportFile\\" + Convert.ToString(HttpContext.Current.Session["UserName"]).Trim()));

            string sdfsdf = ImagesavedFilePath + "\\" + trgt;

            if (!Directory.Exists(ImagesavedFilePath))
            {
                Directory.CreateDirectory(ImagesavedFilePath);
            }
            else
            {
                objCommonBAL.DeleteTempFilesFromExportFolder(ImagesavedFilePath);
            }

            //string stout = sdfsdf + trgt + ".pdf";
            string stout = sdfsdf + ".pdf";


            string strdt = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblfolderindexmaster where folder_id={0}", FolderId));
            //  DataTable dt2 = operation.GetTable4Command(string.Format(@"select * from tblfile where FileId in (select FileId from {0} where FolderId = {1})", strdt, FolderId));
            DataTable dt2 = operation.GetTable4Command(string.Format(@"select tblfile.FileID ,tblfile.[FileName],{0}.Column0 from tblfile inner join {0} on tblfile.FileID = {0}.FileId where {0}.FolderId ={1} order by {0}.Column0", strdt, FolderId));
            if (dt2.Rows.Count > 0)
            {
                string[] stin = new string[dt2.Rows.Count + 1];
                Session["FileID"] = dt2.Rows[0]["FileID"];
                Session["FileName"] = dt2.Rows[0]["FileName"];
                int count = dt2.Rows.Count;
                string[] strfilename = new string[count];
                string[] strFileId = new string[count];
                string DecryptFilePath;
                DecryptFilePath = Convert.ToString(Server.MapPath("~/Repository//"));
                DestinationPath1 = Convert.ToString(Server.MapPath("~/Repository//Decrypt//" + Session["UserName"].ToString()));
                objCommonBAL.DeleteTempFilesFromExportFolder(DestinationPath1);

                //objCommonBAL.DeleteTempFilesFromExportFolder(DecryptFilePath);

                for (int i = 0; i <= dt2.Rows.Count - 1; i++)
                {
                    if (Path.GetExtension(dt2.Rows[i]["FileName"].ToString()).ToLower() == ".docx" || Path.GetExtension(dt2.Rows[i]["FileName"].ToString()).ToLower() == ".xlsx" || Path.GetExtension(dt2.Rows[i]["FileName"].ToString()).ToLower() == ".pptx")
                    {
                        strfilename[i] = Path.GetFileNameWithoutExtension(dt2.Rows[i]["FileName"].ToString()) + ".pdf";
                    }
                    else
                    {
                        strfilename[i] = dt2.Rows[i]["FileName"].ToString();
                    }
                    strFileId[i] = dt2.Rows[i]["FileID"].ToString();
                    Session["FileID"] = strFileId[i];
                    Session["FileName"] = strfilename[i];

                    if (Path.GetExtension(strfilename[i].ToLower()) == ".pdf")
                    {
                        string strfileId = Session["FileID"].ToString();
                        strFileName = Session["FileName"].ToString();


                        //List<string> lstReturnCodePath = objCommonBAL.CheckImageStorageEncryptDecryptPDFImage(DecryptFilePath, Convert.ToString(strfilename[i]), strFileId[i], "1", "", "");
                        //List<string> lstReturnCodePath = objCommonBAL.CheckImageStorageEncryptDecryptImage(DecryptFilePath, Convert.ToString(strfilename[i]), strFileId[i], "1", "", "");



                        string[] _strFileName = new string[count];
                        _strFileName[i] = strfilename[i];
                        sourceFileName = (DecryptFilePath + Path.GetFileNameWithoutExtension(_strFileName[i]) + "_" + (strFileId[i]) + Path.GetExtension(_strFileName[i]));


                        inderFileName = Server.MapPath("~/Report//PdfReports//AgendaIndex.pdf");

                        //DestinationPath = Convert.ToString(Server.MapPath(DestinationPath1 + "//" + Path.GetFileNameWithoutExtension(_strFileName[i]) + "_" + (HttpContext.Current.Session["FileID"]) + Path.GetExtension(_strFileName[i])));
                        //DestinationPath = Convert.ToString(Server.MapPath(DestinationPath1 + "//" + Path.GetFileNameWithoutExtension(_strFileName[i]) + "_" + (HttpContext.Current.Session["FileID"]) + Path.GetExtension(_strFileName[i])));
                        if (loopcount == 0)
                        {
                            if (File.Exists(inderFileName))
                            {

                                string indexfiledelete = Server.MapPath("~/Report//PdfReports");
                                loopcount = loopcount + 1;
                                stin[loopcount - 1] = inderFileName;
                                //objCommonBAL.DeleteTempFilesFromExportFolder(indexfiledelete);

                            }
                        }
                        //else
                        //{
                        if (File.Exists(sourceFileName))
                        {

                            //File.Copy(sourceFileName, DestinationPath, true);
                            loopcount = loopcount + 1;
                            stin[loopcount - 1] = sourceFileName;
                        }
                        //}

                    }

                }
                loopcount = 0;
                int value;
                value = MergePDFDocuments(stin, stout, 1);
                if (value > 0)
                {

                    AddPageNumber();

                    //FileDownLoadwithWaterMark(stout);
                    FileDownLoadwithWaterMarkWithoutPwd(stout);

                }

            }
        }
        catch (Exception ex)
        {

        }
    }

    private void MergePdfFilesWithPwdWithoutWaterMark()
    {
        try
        {
            CommonBAL objCommonBAL = new CommonBAL();
            OperationClass operation = new OperationClass();
            int loopcount = 0;

            string DestinationPath1 = null;
            string DestinationPath = null;
            string sourceFileName = null;
            string inderFileName = null;
            string FolderId = Session["FolderId"].ToString();
            string Foldername = Session["DMeetingdate"].ToString();
            Session["FolderId"] = FolderId;
            Session["Foldername"] = Foldername;
            string trg = "Agenda_" + Session["Foldername"] + "";
            string trgt = trg.Replace("/", "-");
            string ImagesavedFilePath = Convert.ToString(HttpContext.Current.Server.MapPath("~\\ExportFile\\" + Convert.ToString(HttpContext.Current.Session["UserName"]).Trim()));

            string sdfsdf = ImagesavedFilePath + "\\" + trgt;

            if (!Directory.Exists(ImagesavedFilePath))
            {
                Directory.CreateDirectory(ImagesavedFilePath);
            }
            else
            {
                objCommonBAL.DeleteTempFilesFromExportFolder(ImagesavedFilePath);
            }

            //string stout = sdfsdf + trgt + ".pdf";
            string stout = sdfsdf + ".pdf";


            string strdt = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblfolderindexmaster where folder_id={0}", FolderId));
            //  DataTable dt2 = operation.GetTable4Command(string.Format(@"select * from tblfile where FileId in (select FileId from {0} where FolderId = {1})", strdt, FolderId));
            DataTable dt2 = operation.GetTable4Command(string.Format(@"select tblfile.FileID ,tblfile.[FileName],{0}.Column0 from tblfile inner join {0} on tblfile.FileID = {0}.FileId where {0}.FolderId ={1} order by {0}.Column0", strdt, FolderId));
            if (dt2.Rows.Count > 0)
            {
                string[] stin = new string[dt2.Rows.Count + 1];
                Session["FileID"] = dt2.Rows[0]["FileID"];
                Session["FileName"] = dt2.Rows[0]["FileName"];
                int count = dt2.Rows.Count;
                string[] strfilename = new string[count];
                string[] strFileId = new string[count];
                string DecryptFilePath;
                DecryptFilePath = Convert.ToString(Server.MapPath("~/Repository//"));
                DestinationPath1 = Convert.ToString(Server.MapPath("~/Repository//Decrypt//" + Session["UserName"].ToString()));
                objCommonBAL.DeleteTempFilesFromExportFolder(DestinationPath1);

                //objCommonBAL.DeleteTempFilesFromExportFolder(DecryptFilePath);

                for (int i = 0; i <= dt2.Rows.Count - 1; i++)
                {
                    if (Path.GetExtension(dt2.Rows[i]["FileName"].ToString()).ToLower() == ".docx" || Path.GetExtension(dt2.Rows[i]["FileName"].ToString()).ToLower() == ".xlsx" || Path.GetExtension(dt2.Rows[i]["FileName"].ToString()).ToLower() == ".pptx")
                    {
                        strfilename[i] = Path.GetFileNameWithoutExtension(dt2.Rows[i]["FileName"].ToString()) + ".pdf";
                    }
                    else
                    {
                        strfilename[i] = dt2.Rows[i]["FileName"].ToString();
                    }
                    strFileId[i] = dt2.Rows[i]["FileID"].ToString();
                    Session["FileID"] = strFileId[i];
                    Session["FileName"] = strfilename[i];

                    if (Path.GetExtension(strfilename[i].ToLower()) == ".pdf")
                    {
                        string strfileId = Session["FileID"].ToString();
                        strFileName = Session["FileName"].ToString();


                        //List<string> lstReturnCodePath = objCommonBAL.CheckImageStorageEncryptDecryptPDFImage(DecryptFilePath, Convert.ToString(strfilename[i]), strFileId[i], "1", "", "");
                        //List<string> lstReturnCodePath = objCommonBAL.CheckImageStorageEncryptDecryptImage(DecryptFilePath, Convert.ToString(strfilename[i]), strFileId[i], "1", "", "");



                        string[] _strFileName = new string[count];
                        _strFileName[i] = strfilename[i];
                        sourceFileName = (DecryptFilePath + Path.GetFileNameWithoutExtension(_strFileName[i]) + "_" + (strFileId[i]) + Path.GetExtension(_strFileName[i]));


                        inderFileName = Server.MapPath("~/Report//PdfReports//AgendaIndex.pdf");

                        //DestinationPath = Convert.ToString(Server.MapPath(DestinationPath1 + "//" + Path.GetFileNameWithoutExtension(_strFileName[i]) + "_" + (HttpContext.Current.Session["FileID"]) + Path.GetExtension(_strFileName[i])));
                        //DestinationPath = Convert.ToString(Server.MapPath(DestinationPath1 + "//" + Path.GetFileNameWithoutExtension(_strFileName[i]) + "_" + (HttpContext.Current.Session["FileID"]) + Path.GetExtension(_strFileName[i])));
                        if (loopcount == 0)
                        {
                            if (File.Exists(inderFileName))
                            {

                                string indexfiledelete = Server.MapPath("~/Report//PdfReports");
                                loopcount = loopcount + 1;
                                stin[loopcount - 1] = inderFileName;
                                //objCommonBAL.DeleteTempFilesFromExportFolder(indexfiledelete);

                            }
                        }
                        //else
                        //{
                        if (File.Exists(sourceFileName))
                        {

                            //File.Copy(sourceFileName, DestinationPath, true);
                            loopcount = loopcount + 1;
                            stin[loopcount - 1] = sourceFileName;
                        }
                        //}

                    }

                }
                loopcount = 0;
                int value;
                value = MergePDFDocuments(stin, stout, 1);
                if (value > 0)
                {
                    //AddPageNumber(stin);
                    AddPageNumber();

                    FileDownLoadwithoutWaterMark(stout);

                }

            }
        }
        catch (Exception ex)
        {

        }
    }


    private void MergePdfFilesWithoutPwdandWaterMark()
    {
        try
        {
            CommonBAL objCommonBAL = new CommonBAL();
            OperationClass operation = new OperationClass();
            int loopcount = 0;

            string DestinationPath1 = null;
            string DestinationPath = null;
            string sourceFileName = null;
            string inderFileName = null;
            string FolderId = Session["FolderId"].ToString();
            string Foldername = Session["DMeetingdate"].ToString();
            Session["FolderId"] = FolderId;
            Session["Foldername"] = Foldername;
            string trg = "Agenda_" + Session["Foldername"] + "";
            string trgt = trg.Replace("/", "-");
            string ImagesavedFilePath = Convert.ToString(HttpContext.Current.Server.MapPath("~\\ExportFile\\" + Convert.ToString(HttpContext.Current.Session["UserName"]).Trim()));

            string sdfsdf = ImagesavedFilePath + "\\" + trgt;

            if (!Directory.Exists(ImagesavedFilePath))
            {
                Directory.CreateDirectory(ImagesavedFilePath);
            }
            else
            {
                objCommonBAL.DeleteTempFilesFromExportFolder(ImagesavedFilePath);
            }

            //string stout = sdfsdf + trgt + ".pdf";
            string stout = sdfsdf + ".pdf";


            string strdt = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblfolderindexmaster where folder_id={0}", FolderId));
            //  DataTable dt2 = operation.GetTable4Command(string.Format(@"select * from tblfile where FileId in (select FileId from {0} where FolderId = {1})", strdt, FolderId));
            DataTable dt2 = operation.GetTable4Command(string.Format(@"select tblfile.FileID ,tblfile.[FileName],{0}.Column0 from tblfile inner join {0} on tblfile.FileID = {0}.FileId where {0}.FolderId ={1} order by {0}.Column0", strdt, FolderId));
            if (dt2.Rows.Count > 0)
            {
                string[] stin = new string[dt2.Rows.Count + 1];
                Session["FileID"] = dt2.Rows[0]["FileID"];
                Session["FileName"] = dt2.Rows[0]["FileName"];
                int count = dt2.Rows.Count;
                string[] strfilename = new string[count];
                string[] strFileId = new string[count];
                string DecryptFilePath;
                DecryptFilePath = Convert.ToString(Server.MapPath("~/Repository//"));
                DestinationPath1 = Convert.ToString(Server.MapPath("~/Repository//Decrypt//" + Session["UserName"].ToString()));
                objCommonBAL.DeleteTempFilesFromExportFolder(DestinationPath1);

                //objCommonBAL.DeleteTempFilesFromExportFolder(DecryptFilePath);

                for (int i = 0; i <= dt2.Rows.Count - 1; i++)
                {
                    if (Path.GetExtension(dt2.Rows[i]["FileName"].ToString()).ToLower() == ".docx" || Path.GetExtension(dt2.Rows[i]["FileName"].ToString()).ToLower() == ".xlsx" || Path.GetExtension(dt2.Rows[i]["FileName"].ToString()).ToLower() == ".pptx")
                    {
                        strfilename[i] = Path.GetFileNameWithoutExtension(dt2.Rows[i]["FileName"].ToString()) + ".pdf";
                    }
                    else
                    {
                        strfilename[i] = dt2.Rows[i]["FileName"].ToString();
                    }
                    strFileId[i] = dt2.Rows[i]["FileID"].ToString();
                    Session["FileID"] = strFileId[i];
                    Session["FileName"] = strfilename[i];

                    if (Path.GetExtension(strfilename[i].ToLower()) == ".pdf")
                    {
                        string strfileId = Session["FileID"].ToString();
                        strFileName = Session["FileName"].ToString();


                        //List<string> lstReturnCodePath = objCommonBAL.CheckImageStorageEncryptDecryptPDFImage(DecryptFilePath, Convert.ToString(strfilename[i]), strFileId[i], "1", "", "");
                        //List<string> lstReturnCodePath = objCommonBAL.CheckImageStorageEncryptDecryptImage(DecryptFilePath, Convert.ToString(strfilename[i]), strFileId[i], "1", "", "");



                        string[] _strFileName = new string[count];
                        _strFileName[i] = strfilename[i];
                        sourceFileName = (DecryptFilePath + Path.GetFileNameWithoutExtension(_strFileName[i]) + "_" + (strFileId[i]) + Path.GetExtension(_strFileName[i]));


                        inderFileName = Server.MapPath("~/Report//PdfReports//AgendaIndex.pdf");

                        //DestinationPath = Convert.ToString(Server.MapPath(DestinationPath1 + "//" + Path.GetFileNameWithoutExtension(_strFileName[i]) + "_" + (HttpContext.Current.Session["FileID"]) + Path.GetExtension(_strFileName[i])));
                        //DestinationPath = Convert.ToString(Server.MapPath(DestinationPath1 + "//" + Path.GetFileNameWithoutExtension(_strFileName[i]) + "_" + (HttpContext.Current.Session["FileID"]) + Path.GetExtension(_strFileName[i])));
                        if (loopcount == 0)
                        {
                            if (File.Exists(inderFileName))
                            {

                                string indexfiledelete = Server.MapPath("~/Report//PdfReports");
                                loopcount = loopcount + 1;
                                stin[loopcount - 1] = inderFileName;
                                //objCommonBAL.DeleteTempFilesFromExportFolder(indexfiledelete);

                            }
                        }
                        //else
                        //{
                        if (File.Exists(sourceFileName))
                        {

                            //File.Copy(sourceFileName, DestinationPath, true);
                            loopcount = loopcount + 1;
                            stin[loopcount - 1] = sourceFileName;
                        }
                        //}

                    }

                }
                loopcount = 0;
                int value;
                value = MergePDFDocuments(stin, stout, 1);
                if (value > 0)
                {
                    //AddPageNumber(stin);
                    AddPageNumber();
                    PdfFileDownLoad(stout);

                }

            }
        }
        catch (Exception ex)
        {

        }
    }

    protected void FileDownLoadwithWaterMarkWithoutPwd(string filePath)
    {
        //string watermarkText = "Naresh Srinivas Singu";

        try
        {
            if (Session["Name"] != null)
            {
                string NootBookPasswrd = operation.ExecuteScalar4Command(string.Format(@"select NoteBookPassword from tblNoteBookPassword where UserId={0}", Convert.ToInt64(Session["UserID"])));

                string watermarkText = Session["Name"].ToString();
                string ModifiedFileName = string.Empty;
                object TargetFile = filePath;
                iTextSharp.text.pdf.PdfReader reader1 = new iTextSharp.text.pdf.PdfReader(TargetFile.ToString());
                ModifiedFileName = TargetFile.ToString();
                ModifiedFileName = ModifiedFileName.Insert(ModifiedFileName.Length - 4, "pdf");
                //PdfReader reader1 = new PdfReader(filePath);
                //using (FileStream fs = new FileStream(OutLocation, FileMode.Create, FileAccess.Write, FileShare.None))
                using (FileStream fs = new FileStream(ModifiedFileName, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    //iTextSharp.text.pdf.PdfEncryptor.Encrypt(reader1, fs, iTextSharp.text.pdf.PdfWriter.STRENGTH128BITS, Session["txtboxPassword"].ToString(), Session["txtboxPassword"].ToString(), iTextSharp.text.pdf.PdfWriter.AllowPrinting);

                    using (PdfStamper stamper = new PdfStamper(reader1, fs))
                    {
                        //if (NootBookPasswrd == "")
                        //{
                        stamper.SetEncryption(iTextSharp.text.pdf.PdfWriter.STRENGTH128BITS, "", "", iTextSharp.text.pdf.PdfWriter.AllowPrinting);
                        //}
                        //else
                        //{
                        //stamper.SetEncryption(iTextSharp.text.pdf.PdfWriter.STRENGTH128BITS, NootBookPasswrd, NootBookPasswrd, iTextSharp.text.pdf.PdfWriter.AllowPrinting);

                        //}
                        int pageCount1 = reader1.NumberOfPages;
                        //Create a new layer
                        PdfLayer layer = new PdfLayer("WatermarkLayer", stamper.Writer);
                        for (int i = 1; i <= pageCount1; i++)
                        {
                            iTextSharp.text.Rectangle rect = reader1.GetPageSize(i);
                            //Get the ContentByte object
                            PdfContentByte cb = stamper.GetOverContent(i);
                            //Tell the CB that the next commands should be "bound" to this new layer
                            cb.BeginLayer(layer);
                            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED), 50);
                            PdfGState gState = new PdfGState();
                            gState.FillOpacity = 0.50f;
                            cb.SetGState(gState);
                            cb.SetColorFill(BaseColor.GRAY);
                            cb.BeginText();
                            cb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, watermarkText, rect.Width / 2, rect.Height / 2, 45f);
                            //string TableNameFrom = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Convert.ToInt32(Session["FolderID"])));
                            //DataTable dtfiledetails1 = objCommonBAL.GetDispFileName(TableNameFrom, Convert.ToInt32(Session["FolderID"]), Convert.ToInt32(Session["UserID"]));
                            //if (i != 1)
                            //{
                            //    if (dtfiledetails1.Rows.Count > 0)
                            //    {
                            //        foreach (DataRow dr in dtfiledetails1.Rows)
                            //        {
                            //            string Status = dr["ApprovalStatus"].ToString();
                            //            if (Status == "2")
                            //            {
                            //                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Withdrawn", 200f, 15f, 2f);
                            //                //cb.ShowTextAligned(stamper.GetUnderContent(i), Element.ALIGN_RIGHT, new Phrase("Index -" + i.ToString(), blackFont), 568f, 15f, 0);
                            //            }
                            //        }

                            //    }
                            //}
                            cb.EndText();
                            //"Close" the layer
                            cb.EndLayer();
                        }

                    }
                }


                PdfFileDownLoad(ModifiedFileName);
                //if (File.Exists(TargetFile.ToString()))
                //    File.Delete(TargetFile.ToString());
            }
        }
        catch (Exception es)
        {
        }
    }





    public void GenerateIndexPage()
    {
        try
        {
            //Create document
            Document doc = new Document();
            //Create PDF Table
            PdfPTable tableLayout = new PdfPTable(2);
            PdfPTable tableLayout2 = new PdfPTable(2);

            PdfPTable tableLayout1 = new PdfPTable(4);

            PdfPTable tableLayout4 = new PdfPTable(2);
            PdfPTable tableLayout3 = new PdfPTable(1);
            PdfPTable tableLayout6 = new PdfPTable(2);

            //Create a PDF file in specific path
            PdfWriter.GetInstance(doc, new FileStream(HttpContext.Current.Server.MapPath("~/Report//PdfReports//AgendaIndex.pdf"), FileMode.Create));

            //Open the PDF document
            doc.Open();

            //Add Content to PDF
            //doc.Add(Add_Content_To_PDF(tableLayout));

            //doc.Add(Add_Content_To_PDF2(tableLayout2));
            //doc.Add(Add_Content_To_PDF4(tableLayout4));

            doc.Add(Add_Content_To_PDF3(tableLayout3));



            doc.Add(Add_Content_To_PDF2(tableLayout1));
            doc.Add(Add_Content_To_PDF6(tableLayout6));


            // Closing the document
            doc.Close();
        }
        catch (Exception ex)
        {
        }

    }
    private PdfPTable Add_Content_To_PDF6(PdfPTable tableLayout6)
    {
        PdfPCell cell = null;
        float[] headers = { 0.4f, 10 };  //Header Widths
        //float[] headers = { 2, 2, 2, 2,2 };
        tableLayout6.SetWidths(headers);        //Set the pdf headers
        tableLayout6.WidthPercentage = 80;       //Set the PDF File witdh percentage



        //tableLayout6.AddCell(new PdfPCell(new Phrase("", new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
        tableLayout6.AddCell(new PdfPCell(new Phrase("", new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });


        //Add header
        AddCellToHeadertital1244(tableLayout6, "");
        AddCellToHeadertital124(tableLayout6, "   Access Restricted");

        //AddCellToHeader1(tableLayout1, "Parents");

        return tableLayout6;
    }
    private static void AddCellToHeadertital1244(PdfPTable tableLayout1, string cellText)
    {
        //tableLayout1.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.NORMAL, 8, 1, iTextSharp.text.BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 5, BackgroundColor = iTextSharp.text.BaseColor.WHITE });
        //ColumnText.ShowTextAligned(stamper.GetUnderContent(i), Element.ALIGN_RIGHT, new Phrase("Page " + j.ToString() + " of " + TotalPages, blackFont), 568f, 15f, 0);

        //tableLayout1.AddCell(new PdfPCell(new Phrase(cellText, FontFactory.GetFont("Arial", 10, Font.NORMAL))) { HorizontalAlignment = Element.ALIGN_LEFT, Padding = 5, Border = 1, PaddingLeft = -2f });
        tableLayout1.AddCell(new PdfPCell(new Phrase(cellText, FontFactory.GetFont("Arial", 1, Font.NORMAL))) { HorizontalAlignment = Element.ALIGN_LEFT, Padding = 5, BackgroundColor = new iTextSharp.text.BaseColor(255, 182, 193), PaddingLeft = 4f });

    }
    private static void AddCellToHeadertital124(PdfPTable tableLayout1, string cellText)
    {
        tableLayout1.AddCell(new PdfPCell(new Phrase(cellText, FontFactory.GetFont("Arial", 10, Font.NORMAL))) { HorizontalAlignment = Element.ALIGN_LEFT, Padding = 5, Border = 0, PaddingLeft = -2f });

        //tableLayout1.AddCell(new PdfPCell(new Phrase(cellText, FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 5, BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY });

    }
    private PdfPTable Add_Content_To_PDF4(PdfPTable tableLayout4)
    {
        float[] headers = { 40, 30 };  //Header Widths
        float[] headers1 = { 40, 30 };  //Header Height
        tableLayout4.SetWidths(headers);        //Set the pdf headers      
        tableLayout4.WidthPercentage = 80;       //Set the PDF File witdh percentage


        OperationClass objOC = new OperationClass();
        DataTable dt = objOC.GetTable4Command(string.Format(@"Select case when convert(nvarchar,a.initialstatus)='1' then convert(nvarchar,a.initialstatus)+ 'st ' 
              when convert(nvarchar,a.initialstatus)='2' then convert(nvarchar,a.initialstatus)+ 'nd '
              when convert(nvarchar,a.initialstatus) ='3' then convert(nvarchar,a.initialstatus)+ 'rd '    
              when convert(nvarchar,a.initialstatus) like'1%' then convert(nvarchar,a.initialstatus)+ 'th ' 
              when convert(nvarchar,a.initialstatus) like'2%' then convert(nvarchar,a.initialstatus)+ 'nd '
              when convert(nvarchar,a.initialstatus) like'3%' then convert(nvarchar,a.initialstatus)+ 'rd '
              when convert(nvarchar,a.initialstatus) like'4%' then convert(nvarchar,a.initialstatus)+ 'th '
              when convert(nvarchar,a.initialstatus) like'5%' then convert(nvarchar,a.initialstatus)+ 'th '
              when convert(nvarchar,a.initialstatus) like'6%' then convert(nvarchar,a.initialstatus)+ 'th '
              when convert(nvarchar,a.initialstatus) like'7%' then convert(nvarchar,a.initialstatus)+ 'th '
              when convert(nvarchar,a.initialstatus) like'8%' then convert(nvarchar,a.initialstatus)+ 'th '
              when convert(nvarchar,a.initialstatus) like'9%' then convert(nvarchar,a.initialstatus)+ 'th '
              end as Initialstatus,isnull(b.time,'--')Time from tblfolder as a inner join tblfolderindexmaster as b on a.folderid=b.folder_id where a.FolderId={0}", Convert.ToInt32(HttpContext.Current.Session["FolderID"].ToString())));

        DateTime MyDateTime = new DateTime();
        MyDateTime = DateTime.ParseExact(Session["DMeetingdate"].ToString(), "dd/MM/yyyy", null);
        tableLayout4.AddCell(new PdfPCell(new Phrase("", new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
        //tableLayout4.AddCell(new PdfPCell(new Phrase("TITAN COMPANY LIMITED", FontFactory.GetFont("Arial", 18, Font.NORMAL, BaseColor.BLACK))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
        tableLayout4.AddCell(new PdfPCell(new Phrase(ConfigurationManager.AppSettings["CompanyName"].ToString(), FontFactory.GetFont("Arial", 18, Font.NORMAL, BaseColor.BLACK))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });

        //string SendUC_DateOfMeeting = String.Format(" {0:MMMM d, yyyy(dddd)}", MyDateTime);

        //string Send_DateOfMeeting = String.Format(" {0:MMMM d, yyyy }", MyDateTime);
        string SendF_DateOfMeeting = String.Format("{0:D}", MyDateTime);

        if (SendF_DateOfMeeting != "")
        {
            AddCellToHeader5(tableLayout4, SendF_DateOfMeeting);
            AddCellToHeader5(tableLayout4, dt.Rows[0]["Time"].ToString());
        }

        return tableLayout4;

    }

    private PdfPTable Add_Content_To_PDF2(PdfPTable tableLayout1)
    {
        PdfPCell cell = null;
        float[] headers = { 5, 25, 5, 5 };  //Header Widths
        //float[] headers = { 3, 30, 5 };  //Header Widths
        //float[] headers = { 2, 2, 2, 2,2 };
        tableLayout1.SetWidths(headers);        //Set the pdf headers
        tableLayout1.WidthPercentage = 100;       //Set the PDF File witdh percentage
        OperationClass objOC = new OperationClass();
        DataTable dt = objOC.GetTable4Command(string.Format(@"Select case when convert(nvarchar,a.initialstatus)='1' then convert(nvarchar,a.initialstatus)+ 'st ' 
              when convert(nvarchar,a.initialstatus)='2' then convert(nvarchar,a.initialstatus)+ 'nd '
              when convert(nvarchar,a.initialstatus) ='3' then convert(nvarchar,a.initialstatus)+ 'rd '    
              when convert(nvarchar,a.initialstatus) like'1%' then convert(nvarchar,a.initialstatus)+ 'th ' 
              when convert(nvarchar,a.initialstatus) like'2%' then convert(nvarchar,a.initialstatus)+ 'nd '
              when convert(nvarchar,a.initialstatus) like'3%' then convert(nvarchar,a.initialstatus)+ 'rd '
              when convert(nvarchar,a.initialstatus) like'4%' then convert(nvarchar,a.initialstatus)+ 'th '
              when convert(nvarchar,a.initialstatus) like'5%' then convert(nvarchar,a.initialstatus)+ 'th '
              when convert(nvarchar,a.initialstatus) like'6%' then convert(nvarchar,a.initialstatus)+ 'th '
              when convert(nvarchar,a.initialstatus) like'7%' then convert(nvarchar,a.initialstatus)+ 'th '
              when convert(nvarchar,a.initialstatus) like'8%' then convert(nvarchar,a.initialstatus)+ 'th '
              when convert(nvarchar,a.initialstatus) like'9%' then convert(nvarchar,a.initialstatus)+ 'th '
              end as Initialstatus,isnull(b.time,'--')Time from tblfolder as a inner join tblfolderindexmaster as b on a.folderid=b.folder_id where a.FolderId={0}", Convert.ToInt32(HttpContext.Current.Session["FolderID"].ToString())));

        DateTime MyDateTime = new DateTime();
        MyDateTime = DateTime.ParseExact(Session["DMeetingdate"].ToString(), "dd/MM/yyyy", null);

        //string SendUC_DateOfMeeting = String.Format(" {0:MMMM d, yyyy(dddd)}", MyDateTime);

        //string Send_DateOfMeeting = String.Format(" {0:MMMM d, yyyy }", MyDateTime);
        string SendF_DateOfMeeting = String.Format("{0:D}", MyDateTime);


        //Company Logo
        //cell = ImageCell("~/images/logo2.png", 40f, PdfPCell.ALIGN_CENTER);
        //tableLayout1.AddCell(cell);

        //Add Title to the PDF file at the top
        //tableLayout1.AddCell(new PdfPCell(new Phrase("", new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
        tableLayout1.AddCell(new PdfPCell(new Phrase("", new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
        tableLayout1.AddCell(new PdfPCell(new Phrase("", new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
        //tableLayout1.AddCell(new PdfPCell(new Phrase("", new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
        //if (SendF_DateOfMeeting != "")
        //{
        //    tableLayout1.AddCell(new PdfPCell(new Phrase(SendF_DateOfMeeting, new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_LEFT });
        //}
        //tableLayout1.AddCell(new PdfPCell(new Phrase(dt.Rows[0]["Time"].ToString(), new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_RIGHT });

        //tableLayout1.AddCell(new PdfPCell(new Phrase("TITAN COMPANY LIMITED", FontFactory.GetFont("Arial", 18, Font.NORMAL))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = iTextSharp.text.BaseColor.WHITE });
        //tableLayout1.AddCell(new PdfPCell(new Phrase("", new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
        //tableLayout1.AddCell(new PdfPCell(new Phrase("", new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
        //tableLayout1.AddCell(new PdfPCell(new Phrase("", new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
        //tableLayout1.AddCell(new PdfPCell(new Phrase("", new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
        //tableLayout1.AddCell(new PdfPCell(new Phrase("", new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
        //tableLayout1.AddCell(new PdfPCell(new Phrase("", new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });

        //OperationClass operation = new OperationClass();
        //string TableName = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Convert.ToInt32(HttpContext.Current.Session["FolderID"])));
        //CommonBAL objcomm = new CommonBAL();
        //DataTable dtfiledetails1 = objcomm.GetDispFileName(TableName, Convert.ToInt32(HttpContext.Current.Session["FolderID"]), Convert.ToInt32(HttpContext.Current.Session["UserID"]));

        //Add header
        AddCellToHeadertital(tableLayout1, "Sl.No.");
        AddCellToHeadertital(tableLayout1, "Description");
        AddCellToHeadertital(tableLayout1, "Page Number");
        AddCellToHeadertital(tableLayout1, "Time");
        //AddCellToHeader1(tableLayout1, "Parents");
        string TableNameFrom = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Convert.ToInt32(Session["FolderID"])));
        DataTable dtfiledetails1 = objCommonBAL.GetDispFileName(TableNameFrom, Convert.ToInt32(Session["FolderID"]), Convert.ToInt32(Session["UserID"]));
        //foreach (GridViewRow i in gvData.Rows)
        //{
        //    Label lblllItemNo = (Label)i.FindControl("lblllItemNo");
        //    LinkButton lnkOpen1 = (LinkButton)i.FindControl("lnkOpen1");
        //    Label lblPageNo = (Label)i.FindControl("lblPageNo");
        //    AddCellToBody1(tableLayout1, lblllItemNo.Text);
        //    AddCellToBody1(tableLayout1, lnkOpen1.Text);
        //    AddCellToBody1(tableLayout1, lblPageNo.Text);

        //}
        string lblPageNo = "";
        string strPageCount = "";
        string strlastCount = "";
        foreach (DataRow dr in dtfiledetails1.Rows)
        {

            TableNameFrom = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Session["FolderID"]));
            string strQuery = string.Format(@"select a.fileid,b.filename,a.column0,a.column5,isnull(column1,column2) as column1 ,b.PageCount"
                               + ",(select isnull(sum(PageCount),0) + 1 from  {0} XX inner join tblfile YY on XX.fileid=YY.fileid where XX.FolderID={1} and XX.column0 <=a.column0) as 'StartPageNo'"
                               + " ,CASE   WHEN Len(Column1) > 2 THEN 'N'   ELSE 'A' END as Type "
                               + " from {0} as a inner join tblfile b on a.fileid=b.fileid where a.FolderID={1} and column0='{2}' order by column0", TableNameFrom, Session["FolderID"].ToString(), dr["ItemNo"].ToString());
            string accesskey = operation.ExecuteScalar4Command(string.Format(@"select value from   tblconfig where keys='AgendaAccessRestricted'"));

            string accessvalue = operation.ExecuteScalar4Command(string.Format(@"select * from tblAgendalevelAccessControl where fileid='{0}' and userid='{1}'", dr["fileid"].ToString(), Session["UserID"]));

            DataTable dtGetNoteAttachmentDetails = operation.GetTable4Command(strQuery);
            if (accesskey == "no")
            {
                if (accessvalue == "")
                {
                    string lbpdfItemNo = dr["ItemNo"].ToString();
                    string lblllItemNo = dr["ItemNo"].ToString().Substring(0, 2);
                    string lbpdfItemNos = dr["Itemnos"].ToString();
                    // string lblllItemNo = dr["ItemNo"].ToString();
                    string lnkOpen1 = dr["Particulars"].ToString();
                    string Time = dr["Remark"].ToString();

                    if (dr["FolderID"].ToString() == "")
                    {
                        strPageCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}  and b.column0<'{2}'", TableNameFrom, Session["FolderID"].ToString(), dr["ItemNo"].ToString()));
                    }
                    else
                    {
                        strPageCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}  and b.column0<'{2}'", TableNameFrom, Session["FolderID"].ToString(), dr["ItemNo"].ToString()));
                    }
                    //strPageCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}  and b.column0<'{2}'", TableNameFrom, lblFolderId.Text.ToString(), lblllItemNo.Text.ToString()));
                    if (Convert.ToInt32(lblllItemNo) > 08)
                    {
                        if (dr["FolderID"].ToString() == "")
                        {
                            strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and b.column0<'{2}'", TableNameFrom, Session["FolderID"].ToString(), (Convert.ToInt32(lblllItemNo) + 1)));
                        }
                        else
                        {

                            strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and b.column0<'{2}'", TableNameFrom, Session["FolderID"].ToString(), (Convert.ToInt32(lblllItemNo) + 1)));
                        }
                    }
                    else
                    {
                        if (dr["FolderID"].ToString() == "")
                        {
                            strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}and b.column0<'{2}'", TableNameFrom, Session["FolderID"].ToString(), "0" + (Convert.ToInt32(lblllItemNo) + 1)));
                        }
                        else
                        {
                            strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}and b.column0<'{2}'", TableNameFrom, Session["FolderID"].ToString(), "0" + (Convert.ToInt32(lblllItemNo) + 1)));
                        }

                    }
                    string ShowAttachmentGridviewNotebook = operation.ExecuteScalar4Command(string.Format(@"select Value from tblConfig where keys='ShowAttachmentGridviewNotebook'"));

                    if (strPageCount == "")
                    {
                        if (dr["FolderID"].ToString() == "")
                        {
                            strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}  and b.column0<'{2}'", TableNameFrom, Session["FolderID"].ToString(), "0" + (Convert.ToInt32(lblllItemNo) + 1)));
                        }
                        else
                        {
                            strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}  and b.column0<'{2}'", TableNameFrom, Session["FolderID"].ToString(), "0" + (Convert.ToInt32(lblllItemNo) + 1)));

                        }
                        if (ShowAttachmentGridviewNotebook.ToLower() == "yes")
                        {
                            lblPageNo = "1" + " - " + dtGetNoteAttachmentDetails.Rows[0]["PageCount"].ToString();
                        }
                        else
                        {
                            lblPageNo = "1" + " - " + strlastCount;
                        }

                        //lblPageNo = "1" + " - " + strlastCount;
                    }
                    else
                    {
                        if (ShowAttachmentGridviewNotebook.ToLower() == "yes")
                        {
                            lblPageNo = (Convert.ToInt32(strPageCount) + 1) + " - " + (Convert.ToInt32(dtGetNoteAttachmentDetails.Rows[0]["StartPageNo"].ToString()) - 1);
                        }
                        else
                        {
                            lblPageNo = (Convert.ToInt32(strPageCount) + 1) + " - " + strlastCount;
                        }

                        //lblPageNo = (Convert.ToInt32(strPageCount) + 1) + " - " + strlastCount;
                    }
                    //  Label lblPageNo = (Label)i.FindControl("lblPageNo");
                    string Seperator = operation.ExecuteScalar4Command(string.Format(@"select Value from tblConfig where keys='seperator'"));

                    if (dr["filename"].ToString() == "")
                    {
                        AddCellToBodySeprator(tableLayout1, lbpdfItemNos);
                        AddCellToBodySeprator(tableLayout1, lnkOpen1);
                        AddCellToBodySeprator(tableLayout1, "");
                    }
                    else
                    {
                        if (Seperator.ToLower() == "yes")
                        {
                            AddCellToBody1(tableLayout1, lbpdfItemNos);
                        }
                        else
                        {
                            AddCellToBody1(tableLayout1, lbpdfItemNo);
                        }
                        //dtDeniePath = operation.GetTable4Command(string.Format(@"select tblAgendalevelAccessControl.Access,tblfile.PageCount from tblAgendalevelAccessControl inner join tblfile on tblAgendalevelAccessControl.FileID=tblfile.FileID where tblAgendalevelAccessControl.fileid='{0}' and tblAgendalevelAccessControl.userid='{1}'", dr["fileid"].ToString(), ddlNote.SelectedValue.ToString()));
                        //if (dtDeniePath.Rows.Count > 0)
                        //{
                        //    if (dtDeniePath.Rows[0]["Access"].ToString() != "true")
                        //    {

                        //        AddCellToBody4(tableLayout1, lnkOpen1);
                        //    }
                        //}
                        //else
                        //{
                        AddCellToBody3(tableLayout1, lnkOpen1);
                        //}

                        AddCellToBody1(tableLayout1, lblPageNo);
                    }

                    if (Time == "")
                    {

                        AddCellToBody1(tableLayout1, "");
                    }
                    else
                    {
                        AddCellToBody1(tableLayout1, Time);

                    }

                }
            }
            else if (accesskey == "yes")
            {
                string lbpdfItemNo = dr["ItemNo"].ToString();
                string lblllItemNo = dr["ItemNo"].ToString().Substring(0, 2);
                string lbpdfItemNos = dr["Itemnos"].ToString();
                // string lblllItemNo = dr["ItemNo"].ToString();
                string lnkOpen1 = dr["Particulars"].ToString();
                string Time = dr["Remark"].ToString();

                if (dr["FolderID"].ToString() == "")
                {
                    strPageCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}  and b.column0<'{2}'", TableNameFrom, Session["FolderID"].ToString(), dr["ItemNo"].ToString()));
                }
                else
                {
                    strPageCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}  and b.column0<'{2}'", TableNameFrom, Session["FolderID"].ToString(), dr["ItemNo"].ToString()));
                }
                //strPageCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}  and b.column0<'{2}'", TableNameFrom, lblFolderId.Text.ToString(), lblllItemNo.Text.ToString()));
                if (Convert.ToInt32(lblllItemNo) > 08)
                {
                    if (dr["FolderID"].ToString() == "")
                    {
                        strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and b.column0<'{2}'", TableNameFrom, Session["FolderID"].ToString(), (Convert.ToInt32(lblllItemNo) + 1)));
                    }
                    else
                    {

                        strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and b.column0<'{2}'", TableNameFrom, Session["FolderID"].ToString(), (Convert.ToInt32(lblllItemNo) + 1)));
                    }
                }
                else
                {
                    if (dr["FolderID"].ToString() == "")
                    {
                        strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}and b.column0<'{2}'", TableNameFrom, Session["FolderID"].ToString(), "0" + (Convert.ToInt32(lblllItemNo) + 1)));
                    }
                    else
                    {
                        strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}and b.column0<'{2}'", TableNameFrom, Session["FolderID"].ToString(), "0" + (Convert.ToInt32(lblllItemNo) + 1)));
                    }

                }
                string ShowAttachmentGridviewNotebook = operation.ExecuteScalar4Command(string.Format(@"select Value from tblConfig where keys='ShowAttachmentGridviewNotebook'"));

                if (strPageCount == "")
                {
                    if (dr["FolderID"].ToString() == "")
                    {
                        strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}  and b.column0<'{2}'", TableNameFrom, Session["FolderID"].ToString(), "0" + (Convert.ToInt32(lblllItemNo) + 1)));
                    }
                    else
                    {
                        strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}  and b.column0<'{2}'", TableNameFrom, Session["FolderID"].ToString(), "0" + (Convert.ToInt32(lblllItemNo) + 1)));

                    }
                    if (ShowAttachmentGridviewNotebook.ToLower() == "yes")
                    {
                        lblPageNo = "1" + " - " + dtGetNoteAttachmentDetails.Rows[0]["PageCount"].ToString();
                    }
                    else
                    {
                        lblPageNo = "1" + " - " + strlastCount;
                    }

                    //lblPageNo = "1" + " - " + strlastCount;
                }
                else
                {
                    if (ShowAttachmentGridviewNotebook.ToLower() == "yes")
                    {
                        lblPageNo = (Convert.ToInt32(strPageCount) + 1) + " - " + (Convert.ToInt32(dtGetNoteAttachmentDetails.Rows[0]["StartPageNo"].ToString()) - 1);
                    }
                    else
                    {
                        lblPageNo = (Convert.ToInt32(strPageCount) + 1) + " - " + strlastCount;
                    }

                    //lblPageNo = (Convert.ToInt32(strPageCount) + 1) + " - " + strlastCount;
                }
                //  Label lblPageNo = (Label)i.FindControl("lblPageNo");
                string Seperator = operation.ExecuteScalar4Command(string.Format(@"select Value from tblConfig where keys='seperator'"));

                if (dr["filename"].ToString() == "")
                {
                    AddCellToBodySeprator(tableLayout1, lbpdfItemNos);
                    AddCellToBodySeprator(tableLayout1, lnkOpen1);
                    AddCellToBodySeprator(tableLayout1, "");
                }
                else
                {
                    if (Seperator.ToLower() == "yes")
                    {
                        AddCellToBody1(tableLayout1, lbpdfItemNos);
                    }
                    else
                    {
                        AddCellToBody1(tableLayout1, lbpdfItemNo);
                    }
                    dtDeniePath = operation.GetTable4Command(string.Format(@"select tblAgendalevelAccessControl.Access,tblfile.PageCount from tblAgendalevelAccessControl inner join tblfile on tblAgendalevelAccessControl.FileID=tblfile.FileID where tblAgendalevelAccessControl.fileid='{0}' and tblAgendalevelAccessControl.userid='{1}'", dr["fileid"].ToString(), Session["UserID"]));
                    if (dtDeniePath.Rows.Count > 0)
                    {
                        if (dtDeniePath.Rows[0]["Access"].ToString() != "true")
                        {

                            AddCellToBody4(tableLayout1, lnkOpen1);
                        }
                    }
                    else
                    {
                        AddCellToBody3(tableLayout1, lnkOpen1);
                    }

                    AddCellToBody1(tableLayout1, lblPageNo);
                }

                if (Time == "")
                {

                    AddCellToBody1(tableLayout1, "");
                }
                else
                {
                    AddCellToBody1(tableLayout1, Time);

                }
            }
        }


        return tableLayout1;
    }

    //private PdfPTable Add_Content_To_PDF2(PdfPTable tableLayout2)
    //{
    //    OperationClass objOC = new OperationClass();
    //    float[] headers = { 40, 30 };  //Header Widths
    //    float[] headers1 = { 40, 30 };  //Header Height
    //    tableLayout2.SetWidths(headers);        //Set the pdf headers      
    //    tableLayout2.WidthPercentage = 80;       //Set the PDF File witdh percentage
    //    string Committee = objOC.ExecuteScalar4Command(string.Format(@"select FolderName from dbo.tblFolder where FolderId={0}", Convert.ToInt32(Session["ParentFolderID"])));


    //    tableLayout2.AddCell(new PdfPCell(new Phrase("", new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
    //    tableLayout2.AddCell(new PdfPCell(new Phrase("", new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
    //    tableLayout2.AddCell(new PdfPCell(new Phrase("", new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
    //    tableLayout2.AddCell(new PdfPCell(new Phrase("", new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
    //    tableLayout2.AddCell(new PdfPCell(new Phrase("", new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
    //    tableLayout2.AddCell(new PdfPCell(new Phrase("", new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
    //    tableLayout2.AddCell(new PdfPCell(new Phrase("TITAN COMPANY LIMITED", FontFactory.GetFont("Arial", 18, Font.NORMAL))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = iTextSharp.text.BaseColor.WHITE });
    //    tableLayout2.AddCell(new PdfPCell(new Phrase("", new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
    //    tableLayout2.AddCell(new PdfPCell(new Phrase("", new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
    //    tableLayout2.AddCell(new PdfPCell(new Phrase("", new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
    //    tableLayout2.AddCell(new PdfPCell(new Phrase("", new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
    //    tableLayout2.AddCell(new PdfPCell(new Phrase("", new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
    //    tableLayout2.AddCell(new PdfPCell(new Phrase("", new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
    //    AddCellToHeader(tableLayout2, Committee);
    //    AddCellToHeader(tableLayout2, "Mr.A R Rajaram");

    //    return tableLayout2;

    //}

    //private PdfPTable Add_Content_To_PDF3(PdfPTable tableLayout3)
    //{
    //    OperationClass objOC = new OperationClass();
    //    string InitialStatus = operation.ExecuteScalar4Command(string.Format(@"select InitialStatus from tblfolder where folderId={0}", Convert.ToInt32(Session["FolderID"])));
    //    string Committee = objOC.ExecuteScalar4Command(string.Format(@"select FolderName from dbo.tblFolder where FolderId={0}", Convert.ToInt32(Session["ParentFolderID"])));
    //    string Address = objOC.ExecuteScalar4Command(string.Format(@"select a.FullAddress+','+a.City as Address from tblLocationDetails a left outer join tblFolderIndexMaster b on a.LocationId=b.LocationId where b.Folder_Id={0}", Convert.ToInt32(HttpContext.Current.Session["FolderID"].ToString())));
    //    DataTable dt = objOC.GetTable4Command(string.Format(@"Select MeetingNo,isnull(b.time,'--')Time from tblfolder as a inner join tblfolderindexmaster as b on a.folderid=b.folder_id where a.FolderId={0}", Convert.ToInt32(HttpContext.Current.Session["FolderID"].ToString())));
    //    float[] headers = { 40 };  //Header Widths
    //    float[] headers1 = { 40 };  //Header Height
    //    tableLayout3.SetWidths(headers);        //Set the pdf headers      
    //    tableLayout3.WidthPercentage = 80;       //Set the PDF File witdh percentage

    //    DateTime MyDateTime = new DateTime();
    //    MyDateTime = DateTime.ParseExact(Session["DMeetingdate"].ToString(), "dd/MM/yyyy", null);
    //    //tableLayout4.AddCell(new PdfPCell(new Phrase("", new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
    //    //tableLayout4.AddCell(new PdfPCell(new Phrase("TITAN COMPANY LIMITED", FontFactory.GetFont("Arial", 18, Font.NORMAL, BaseColor.BLACK))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });

    //    //string SendUC_DateOfMeeting = String.Format(" {0:MMMM d, yyyy(dddd)}", MyDateTime);

    //    //string Send_DateOfMeeting = String.Format(" {0:MMMM d, yyyy }", MyDateTime);
    //    string SendF_DateOfMeeting = String.Format("{0:D}", MyDateTime);

    //    //if (SendF_DateOfMeeting != "")
    //    //{
    //    //    AddCellToHeader5(tableLayout4, SendF_DateOfMeeting);
    //    //    AddCellToHeader5(tableLayout4, dt.Rows[0]["Time"].ToString());
    //    //}

    //    //tableLayout3.AddCell(new PdfPCell(new Phrase("", new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
    //    //tableLayout3.AddCell(new PdfPCell(new Phrase("", new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
    //    //tableLayout3.AddCell(new PdfPCell(new Phrase("", new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
    //    //tableLayout3.AddCell(new PdfPCell(new Phrase("", new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
    //    //tableLayout3.AddCell(new PdfPCell(new Phrase("", new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
    //    //tableLayout3.AddCell(new PdfPCell(new Phrase("", new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
    //    //tableLayout3.AddCell(new PdfPCell(new Phrase("TITAN COMPANY LIMITED", FontFactory.GetFont("Arial", 18, Font.NORMAL,BaseColor.BLACK))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
    //    //tableLayout3.AddCell(new PdfPCell(new Phrase(Committee, FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.BLACK))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
    //    tableLayout3.AddCell(new PdfPCell(new Phrase("", new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
    //    tableLayout3.AddCell(new PdfPCell(new Phrase("Titan", FontFactory.GetFont("Arial", 18, Font.NORMAL, BaseColor.BLACK))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
    //    //tableLayout3.AddCell(new PdfPCell(new Phrase("TITAN COMPANY LIMITED", FontFactory.GetFont("Arial", 18, Font.NORMAL, BaseColor.BLACK))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
    //    //tableLayout3.AddCell(new PdfPCell(new Phrase("Exide Industries", FontFactory.GetFont("Arial", 18, Font.NORMAL, BaseColor.BLACK))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });

    //    if (dt.Rows[0]["MeetingNo"].ToString() != "")
    //    {
    //        tableLayout3.AddCell(new PdfPCell(new Phrase(dt.Rows[0]["MeetingNo"].ToString() + " " + Committee, FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.BLACK))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });

    //    }
    //    else
    //    {
    //        tableLayout3.AddCell(new PdfPCell(new Phrase(Committee, FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.BLACK))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
    //    }

    //    AddCellToHeader4(tableLayout3, SendF_DateOfMeeting + "," + dt.Rows[0]["Time"].ToString());
    //    ///AddCellToHeader4(tableLayout3, SendF_DateOfMeeting );
    //    AddCellToHeader4(tableLayout3, "Venue:" + Address + "");
    //    //AddCellToHeader(tableLayout2, "Mr.A R Rajaram");

    //    return tableLayout3;

    //}
    private PdfPTable Add_Content_To_PDF3(PdfPTable tableLayout3)
    {
        OperationClass objOC = new OperationClass();
        OperationClass operation = new OperationClass();
        string[] Cmeeting = new string[1];


        string InitialStatus = operation.ExecuteScalar4Command(string.Format(@"select InitialStatus from tblfolder where folderId={0}", Convert.ToInt32(Session["FolderID"])));
        string Committee = objOC.ExecuteScalar4Command(string.Format(@"select FolderName from dbo.tblFolder where FolderId={0}", Convert.ToInt32(Session["ParentFolderID"])));
        string Company = objOC.ExecuteScalar4Command(string.Format(@"select Company from tblCompanyMaster where LCId in (select CompanyId from tblFolder where FolderId={0})", Convert.ToInt32(Session["ParentFolderID"])));
        string Address = objOC.ExecuteScalar4Command(string.Format(@"select a.FullAddress from tblLocationDetails a left outer join tblFolderIndexMaster b on a.LocationId=b.LocationId where b.Folder_Id={0}", Convert.ToInt32(HttpContext.Current.Session["FolderID"].ToString())));
        DataTable dt = objOC.GetTable4Command(string.Format(@"Select MeetingNo,isnull(b.time,'--')Time,a.MeetingDateTo,b.TimeTo from tblfolder as a inner join tblfolderindexmaster as b on a.folderid=b.folder_id where a.FolderId={0}", Convert.ToInt32(HttpContext.Current.Session["FolderID"].ToString())));

        float[] headers = { 40 };  //Header Widths
        float[] headers1 = { 40 };  //Header Height
        tableLayout3.SetWidths(headers);        //Set the pdf headers      
        tableLayout3.WidthPercentage = 80;       //Set the PDF File witdh percentage

        DateTime MyDateTime = new DateTime();
        MyDateTime = DateTime.ParseExact(Session["DMeetingdate"].ToString(), "dd/MM/yyyy", null);
        //tableLayout4.AddCell(new PdfPCell(new Phrase("", new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
        //tableLayout4.AddCell(new PdfPCell(new Phrase("TITAN COMPANY LIMITED", FontFactory.GetFont("Arial", 18, Font.NORMAL, BaseColor.BLACK))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
        if (Committee != "")
        {
            Cmeeting = Committee.Split('-');
        }
        string SendF_DateOfMeeting = String.Format("{0:D}", MyDateTime);
        string stringDate;
        switch (dt.Rows[0]["MeetingNo"].ToString())
        {
            case ("1"):
                stringDate = "st";

                break;

            case ("2"):

                stringDate = "nd";
                break;

            case ("3"):

                stringDate = "rd";
                break;
            default:

                stringDate = "th";
                break;
        }

        //tableLayout3.AddCell(new PdfPCell(new Phrase("", new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
        //tableLayout3.AddCell(new PdfPCell(new Phrase("", new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
        //tableLayout3.AddCell(new PdfPCell(new Phrase("", new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
        //tableLayout3.AddCell(new PdfPCell(new Phrase("", new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
        //tableLayout3.AddCell(new PdfPCell(new Phrase("", new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
        //tableLayout3.AddCell(new PdfPCell(new Phrase("", new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
        //tableLayout3.AddCell(new PdfPCell(new Phrase("TITAN COMPANY LIMITED", FontFactory.GetFont("Arial", 18, Font.NORMAL,BaseColor.BLACK))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
        //tableLayout3.AddCell(new PdfPCell(new Phrase(Committee, FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.BLACK))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
        tableLayout3.AddCell(new PdfPCell(new Phrase("", new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
        if (Cmeeting[0] == "TTPL")
        {
            tableLayout3.AddCell(new PdfPCell(new Phrase("TITAN TIMEPRODUCTS LIMITED", FontFactory.GetFont("Arial", 22, Font.NORMAL, BaseColor.BLACK))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
            //tableLayout3.AddCell(new PdfPCell(new Phrase(ConfigurationManager.AppSettings["CompanyName"].ToString(), FontFactory.GetFont("Arial", 22, Font.NORMAL, BaseColor.BLACK))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });

        }
        if (Cmeeting[0].ToLower().ToString() == "titan")
        {
            tableLayout3.AddCell(new PdfPCell(new Phrase("TITAN COMPANY LIMITED", FontFactory.GetFont("Arial", 22, Font.NORMAL, BaseColor.BLACK))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
            //tableLayout3.AddCell(new PdfPCell(new Phrase(ConfigurationManager.AppSettings["CompanyName"].ToString(), FontFactory.GetFont("Arial", 22, Font.NORMAL, BaseColor.BLACK))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });

        }
        if (Company != "")
        {
            tableLayout3.AddCell(new PdfPCell(new Phrase(Company, FontFactory.GetFont("Arial", 22, Font.NORMAL, BaseColor.BLACK))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
            //tableLayout3.AddCell(new PdfPCell(new Phrase(ConfigurationManager.AppSettings["CompanyName"].ToString(), FontFactory.GetFont("Arial", 22, Font.NORMAL, BaseColor.BLACK))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });

        }
        else
        {
            tableLayout3.AddCell(new PdfPCell(new Phrase("", FontFactory.GetFont("Arial", 22, Font.NORMAL, BaseColor.BLACK))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
        }
        //tableLayout3.AddCell(new PdfPCell(new Phrase("TITAN COMPANY LIMITED", FontFactory.GetFont("Arial", 18, Font.NORMAL, BaseColor.BLACK))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
        //tableLayout3.AddCell(new PdfPCell(new Phrase("Titan Industries", FontFactory.GetFont("Arial", 18, Font.NORMAL, BaseColor.BLACK))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });

        if (dt.Rows[0]["MeetingNo"].ToString() != "")
        {
            if (Cmeeting.Length == 2)
            {
                tableLayout3.AddCell(new PdfPCell(new Phrase(dt.Rows[0]["MeetingNo"].ToString() + " " + "Meeting of " + Cmeeting[1].ToString(), FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.BLACK))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });

            }
            else
            {
                tableLayout3.AddCell(new PdfPCell(new Phrase(dt.Rows[0]["MeetingNo"].ToString() + " " + "Meeting of " + Committee, FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.BLACK))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });

            }
        }
        else
        {
            tableLayout3.AddCell(new PdfPCell(new Phrase(Committee, FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.BLACK))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
        }

        AddCellToHeader4(tableLayout3, "From:" + SendF_DateOfMeeting + "," + dt.Rows[0]["Time"].ToString());
        if (dt.Rows[0]["MeetingDateTo"].ToString() != "")
        {
            DateTime MyToDateTime = new DateTime();
            MyToDateTime = DateTime.ParseExact(dt.Rows[0]["MeetingDateTo"].ToString(), "dd/MM/yyyy", null);
            string SendT_DateOfMeeting = String.Format("{0:D}", MyToDateTime);
            if (dt.Rows[0]["TimeTo"].ToString() != "")
            {
                AddCellToHeader4(tableLayout3, "To:" + SendT_DateOfMeeting + "," + dt.Rows[0]["TimeTo"].ToString());
            }
            else
            {
                AddCellToHeader4(tableLayout3, "To:" + SendT_DateOfMeeting + "," + "");
            }

        }
        else
        {
            //AddCellToHeader4(tableLayout3, "To:" + "" + "," + "");
        }
        ///AddCellToHeader4(tableLayout3, SendF_DateOfMeeting );
        AddCellToHeader4(tableLayout3, "Venue:" + Address + "");
        string In_Attendance = operation.ExecuteScalar4Command(string.Format(@"select Value from tblConfig where keys='In Attendance'"));
        if (In_Attendance.ToLower() == "yes")
        {
            string InAttendance = operation.ExecuteScalar4Command(string.Format(@"select InAttendance from tblFolder where FolderId={0}", Session["FolderId"].ToString()));
            string Invitees = operation.ExecuteScalar4Command(string.Format(@"select Invitees from tblFolder where FolderId={0}", Session["FolderId"].ToString()));
            AddCellToHeader4(tableLayout3, "In Attendance:" + InAttendance + "");
            AddCellToHeader4(tableLayout3, "Invitees:" + Invitees + "");
        }
        //AddCellToHeader(tableLayout2, "Mr.A R Rajaram");

        return tableLayout3;

    }

    private PdfPTable Add_Content_To_PDF(PdfPTable tableLayout)
    {
        OperationClass objOC = new OperationClass();

        string Address = objOC.ExecuteScalar4Command(string.Format(@"select a.FullAddress+','+a.City as Address from tblLocationDetails a left outer join tblFolderIndexMaster b on a.LocationId=b.LocationId where b.Folder_Id={0}", Convert.ToInt32(HttpContext.Current.Session["FolderID"].ToString())));
        DataTable dt = objOC.GetTable4Command(string.Format(@"Select case when convert(nvarchar,a.initialstatus)='1' then convert(nvarchar,a.initialstatus)+ 'st ' 
              when convert(nvarchar,a.initialstatus)='2' then convert(nvarchar,a.initialstatus)+ 'nd '
              when convert(nvarchar,a.initialstatus) ='3' then convert(nvarchar,a.initialstatus)+ 'rd '    
              when convert(nvarchar,a.initialstatus) like'1%' then convert(nvarchar,a.initialstatus)+ 'th ' 
              when convert(nvarchar,a.initialstatus) like'2%' then convert(nvarchar,a.initialstatus)+ 'nd '
              when convert(nvarchar,a.initialstatus) like'3%' then convert(nvarchar,a.initialstatus)+ 'rd '
              when convert(nvarchar,a.initialstatus) like'4%' then convert(nvarchar,a.initialstatus)+ 'th '
              when convert(nvarchar,a.initialstatus) like'5%' then convert(nvarchar,a.initialstatus)+ 'th '
              when convert(nvarchar,a.initialstatus) like'6%' then convert(nvarchar,a.initialstatus)+ 'th '
              when convert(nvarchar,a.initialstatus) like'7%' then convert(nvarchar,a.initialstatus)+ 'th '
              when convert(nvarchar,a.initialstatus) like'8%' then convert(nvarchar,a.initialstatus)+ 'th '
              when convert(nvarchar,a.initialstatus) like'9%' then convert(nvarchar,a.initialstatus)+ 'th '
              end as Initialstatus,isnull(b.time,'--')Time from tblfolder as a inner join tblfolderindexmaster as b on a.folderid=b.folder_id where a.FolderId={0}", Convert.ToInt32(HttpContext.Current.Session["FolderID"].ToString())));
        float[] headers = { 20, 30 };  //Header Widths
        tableLayout.SetWidths(headers);        //Set the pdf headers
        tableLayout.WidthPercentage = 80;       //Set the PDF File witdh percentage

        string Committee = objOC.ExecuteScalar4Command(string.Format(@"select FolderName from dbo.tblFolder where FolderId={0}", Convert.ToInt32(Session["ParentFolderID"])));


        //cell = ImageCell("~/images/logo2.png", 40f, PdfPCell.ALIGN_CENTER);

        //tableLayout.AddCell(cell);

        //Add Title to the PDF file at the top


        //tableLayout.AddCell(new PdfPCell(new Phrase("", new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
        //tableLayout.AddCell(new PdfPCell(cell));

        tableLayout.AddCell(new PdfPCell(new Phrase("", new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });

        //Add header ddlfoldername
        AddCellToHeader(tableLayout, "Meeting");
        AddCellToHeader(tableLayout, "Details");

        //Add body
        AddCellToBody(tableLayout, "Meeting Type");
        AddCellToBody(tableLayout, Committee);

        AddCellToBody(tableLayout, "Meeting Date");
        AddCellToBody(tableLayout, Session["DMeetingdate"].ToString());

        AddCellToBody(tableLayout, "Venue");
        AddCellToBody(tableLayout, Address);


        AddCellToBody(tableLayout, "Meeting Time");
        AddCellToBody(tableLayout, dt.Rows[0]["Time"].ToString());

        //AddCellToBody(tableLayout, "Meeting Number");
        //AddCellToBody(tableLayout, dt.Rows[0]["Initialstatus"].ToString());

        return tableLayout;
    }

    private static void AddCellToHeader5(PdfPTable tableLayout, string cellText)
    {
        //tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.NORMAL, 8, 1, iTextSharp.text.BaseColor.WHITE))) { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 5, BackgroundColor = new iTextSharp.text.BaseColor(0, 51, 102) });
        tableLayout.AddCell(new PdfPCell(new Phrase(cellText, FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 5, BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY, Border = 0 });
    }
    // Method to add single cell to the header
    private static void AddCellToHeader(PdfPTable tableLayout, string cellText)
    {
        //tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.NORMAL, 8, 1, iTextSharp.text.BaseColor.WHITE))) { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 5, BackgroundColor = new iTextSharp.text.BaseColor(0, 51, 102) });
        tableLayout.AddCell(new PdfPCell(new Phrase(cellText, FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.WHITE))) { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 5, BackgroundColor = iTextSharp.text.BaseColor.GRAY });
    }
    private static void AddCellToHeader4(PdfPTable tableLayout, string cellText)
    {
        tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.NORMAL, 10, 1, iTextSharp.text.BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 5, BackgroundColor = new iTextSharp.text.BaseColor(224, 225, 178) });
        //tableLayout.AddCell(new PdfPCell(new Phrase(cellText, FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 5, BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY });


    }

    // Method to add single cell to the body
    private static void AddCellToBody(PdfPTable tableLayout, string cellText)
    {
        tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.NORMAL, 8, 1, iTextSharp.text.BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 5, BackgroundColor = iTextSharp.text.BaseColor.WHITE });
    }


    //private PdfPTable Add_Content_To_PDF1(PdfPTable tableLayout1)
    //{
    //    PdfPCell cell = null;
    //    float[] headers = { 5, 30, 5 };  //Header Widths
    //    tableLayout1.SetWidths(headers);        //Set the pdf headers
    //    tableLayout1.WidthPercentage = 80;       //Set the PDF File witdh percentage

    //    //Company Logo
    //    //cell = ImageCell("~/images/logo2.png", 40f, PdfPCell.ALIGN_CENTER);
    //    //tableLayout1.AddCell(cell);

    //    //Add Title to the PDF file at the top
    //    tableLayout1.AddCell(new PdfPCell(new Phrase("", new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });

    //    //OperationClass operation = new OperationClass();
    //    //string TableName = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Convert.ToInt32(HttpContext.Current.Session["FolderID"])));
    //    //CommonBAL objcomm = new CommonBAL();
    //    //DataTable dtfiledetails1 = objcomm.GetDispFileName(TableName, Convert.ToInt32(HttpContext.Current.Session["FolderID"]), Convert.ToInt32(HttpContext.Current.Session["UserID"]));

    //    //Add header
    //    AddCellToHeader1(tableLayout1, "Sr.No");
    //    AddCellToHeader1(tableLayout1, "Description");
    //    AddCellToHeader1(tableLayout1, "Page Number");
    //    //AddCellToHeader1(tableLayout1, "Parents");
    //    string TableNameFrom = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Convert.ToInt32(Session["FolderID"])));
    //    DataTable dtfiledetails1 = objCommonBAL.GetDispFileName(TableNameFrom, Convert.ToInt32(Session["FolderID"]), Convert.ToInt32(Session["UserID"]));
    //    //foreach (GridViewRow i in gvData.Rows)
    //    //{
    //    //    Label lblllItemNo = (Label)i.FindControl("lblllItemNo");
    //    //    LinkButton lnkOpen1 = (LinkButton)i.FindControl("lnkOpen1");
    //    //    Label lblPageNo = (Label)i.FindControl("lblPageNo");
    //    //    AddCellToBody1(tableLayout1, lblllItemNo.Text);
    //    //    AddCellToBody1(tableLayout1, lnkOpen1.Text);
    //    //    AddCellToBody1(tableLayout1, lblPageNo.Text);

    //    //}
    //    string lblPageNo = "";
    //    string strPageCount = "";
    //    string strlastCount = "";
    //    foreach (DataRow dr in dtfiledetails1.Rows)
    //    {
    //        string lblllItemNo = dr["ItemNo"].ToString();
    //        string lnkOpen1 = dr["Particulars"].ToString();

    //        if (dr["FolderID"].ToString() == "")
    //        {
    //            strPageCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}  and b.column0<'{2}'", TableNameFrom, dr["FolderID"].ToString(), dr["ItemNo"].ToString()));
    //        }
    //        else
    //        {
    //            strPageCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}  and b.column0<'{2}'", TableNameFrom, dr["FolderID"].ToString(), dr["ItemNo"].ToString()));
    //        }
    //        //strPageCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}  and b.column0<'{2}'", TableNameFrom, lblFolderId.Text.ToString(), lblllItemNo.Text.ToString()));
    //        if (Convert.ToInt32(dr["ItemNo"].ToString()) > 08)
    //        {
    //            if (dr["FolderID"].ToString() == "")
    //            {
    //                strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and b.column0<'{2}'", TableNameFrom, dr["FolderID"].ToString(), (Convert.ToInt32(dr["ItemNo"].ToString()) + 1)));
    //            }
    //            else
    //            {

    //                strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and b.column0<'{2}'", TableNameFrom, dr["FolderID"].ToString(), (Convert.ToInt32(dr["ItemNo"].ToString()) + 1)));
    //            }
    //        }
    //        else
    //        {
    //            if (dr["FolderID"].ToString() == "")
    //            {
    //                strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}and b.column0<'{2}'", TableNameFrom, dr["FolderID"].ToString(), "0" + (Convert.ToInt32(dr["ItemNo"].ToString()) + 1)));
    //            }
    //            else
    //            {
    //                strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}and b.column0<'{2}'", TableNameFrom, dr["FolderID"].ToString(), "0" + (Convert.ToInt32(dr["ItemNo"].ToString()) + 1)));
    //            }

    //        }
    //        if (strPageCount == "")
    //        {
    //            if (dr["FolderID"].ToString() == "")
    //            {
    //                strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}  and b.column0<'{2}'", TableNameFrom, dr["FolderID"].ToString(), "0" + (Convert.ToInt32(dr["ItemNo"].ToString()) + 1)));
    //            }
    //            else
    //            {
    //                strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}  and b.column0<'{2}'", TableNameFrom, dr["FolderID"].ToString(), "0" + (Convert.ToInt32(dr["ItemNo"].ToString()) + 1)));

    //            }
    //            lblPageNo = "1" + " - " + strlastCount;
    //        }
    //        else
    //        {
    //            lblPageNo = (Convert.ToInt32(strPageCount) + 1) + " - " + strlastCount;
    //        }
    //        //  Label lblPageNo = (Label)i.FindControl("lblPageNo");

    //        AddCellToBody1(tableLayout1, lblllItemNo);
    //        AddCellToBody1(tableLayout1, lnkOpen1);
    //        AddCellToBody1(tableLayout1, lblPageNo);

    //    }


    //    return tableLayout1;
    //}

    //attachment at the index page

    private PdfPTable Add_Content_To_PDF1(PdfPTable tableLayout1)
    {
        PdfPCell cell = null;
        float[] headers = { 8, 30, 5 };  //Header Widths
        tableLayout1.SetWidths(headers);        //Set the pdf headers
        tableLayout1.WidthPercentage = 90;       //Set the PDF File witdh percentage
        OperationClass objOC = new OperationClass();
        DataTable dt = objOC.GetTable4Command(string.Format(@"Select case when convert(nvarchar,a.initialstatus)='1' then convert(nvarchar,a.initialstatus)+ 'st ' 
              when convert(nvarchar,a.initialstatus)='2' then convert(nvarchar,a.initialstatus)+ 'nd '
              when convert(nvarchar,a.initialstatus) ='3' then convert(nvarchar,a.initialstatus)+ 'rd '    
              when convert(nvarchar,a.initialstatus) like'1%' then convert(nvarchar,a.initialstatus)+ 'th ' 
              when convert(nvarchar,a.initialstatus) like'2%' then convert(nvarchar,a.initialstatus)+ 'nd '
              when convert(nvarchar,a.initialstatus) like'3%' then convert(nvarchar,a.initialstatus)+ 'rd '
              when convert(nvarchar,a.initialstatus) like'4%' then convert(nvarchar,a.initialstatus)+ 'th '
              when convert(nvarchar,a.initialstatus) like'5%' then convert(nvarchar,a.initialstatus)+ 'th '
              when convert(nvarchar,a.initialstatus) like'6%' then convert(nvarchar,a.initialstatus)+ 'th '
              when convert(nvarchar,a.initialstatus) like'7%' then convert(nvarchar,a.initialstatus)+ 'th '
              when convert(nvarchar,a.initialstatus) like'8%' then convert(nvarchar,a.initialstatus)+ 'th '
              when convert(nvarchar,a.initialstatus) like'9%' then convert(nvarchar,a.initialstatus)+ 'th '
              end as Initialstatus,isnull(b.time,'--')Time from tblfolder as a inner join tblfolderindexmaster as b on a.folderid=b.folder_id where a.FolderId={0}", Convert.ToInt32(HttpContext.Current.Session["FolderID"].ToString())));

        DateTime MyDateTime = new DateTime();
        MyDateTime = DateTime.ParseExact(Session["DMeetingdate"].ToString(), "dd/MM/yyyy", null);

        //string SendUC_DateOfMeeting = String.Format(" {0:MMMM d, yyyy(dddd)}", MyDateTime);

        //string Send_DateOfMeeting = String.Format(" {0:MMMM d, yyyy }", MyDateTime);
        string SendF_DateOfMeeting = String.Format("{0:D}", MyDateTime);


        //Company Logo
        //cell = ImageCell("~/images/logo2.png", 40f, PdfPCell.ALIGN_CENTER);
        //tableLayout1.AddCell(cell);

        //Add Title to the PDF file at the top
        //tableLayout1.AddCell(new PdfPCell(new Phrase("", new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
        tableLayout1.AddCell(new PdfPCell(new Phrase("", new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
        tableLayout1.AddCell(new PdfPCell(new Phrase("", new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
        //tableLayout1.AddCell(new PdfPCell(new Phrase("", new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
        //if (SendF_DateOfMeeting != "")
        //{
        //    tableLayout1.AddCell(new PdfPCell(new Phrase(SendF_DateOfMeeting, new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_LEFT });
        //}
        //tableLayout1.AddCell(new PdfPCell(new Phrase(dt.Rows[0]["Time"].ToString(), new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_RIGHT });

        //tableLayout1.AddCell(new PdfPCell(new Phrase("TITAN COMPANY LIMITED", FontFactory.GetFont("Arial", 18, Font.NORMAL))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = iTextSharp.text.BaseColor.WHITE });
        //tableLayout1.AddCell(new PdfPCell(new Phrase("", new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
        //tableLayout1.AddCell(new PdfPCell(new Phrase("", new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
        //tableLayout1.AddCell(new PdfPCell(new Phrase("", new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
        //tableLayout1.AddCell(new PdfPCell(new Phrase("", new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
        //tableLayout1.AddCell(new PdfPCell(new Phrase("", new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });
        //tableLayout1.AddCell(new PdfPCell(new Phrase("", new Font(Font.NORMAL, 13, 1, new iTextSharp.text.BaseColor(153, 51, 0)))) { Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER });

        //OperationClass operation = new OperationClass();
        //string TableName = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Convert.ToInt32(HttpContext.Current.Session["FolderID"])));
        //CommonBAL objcomm = new CommonBAL();
        //DataTable dtfiledetails1 = objcomm.GetDispFileName(TableName, Convert.ToInt32(HttpContext.Current.Session["FolderID"]), Convert.ToInt32(HttpContext.Current.Session["UserID"]));

        //Add header
        AddCellToHeadertital(tableLayout1, "Sl.No.");
        AddCellToHeadertital(tableLayout1, "Description");
        AddCellToHeadertital(tableLayout1, "Page Number");
        //AddCellToHeader1(tableLayout1, "Parents");
        string TableNameFrom = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Convert.ToInt32(Session["FolderID"])));
        DataTable dtfiledetails1 = objCommonBAL.GetDispFileName(TableNameFrom, Convert.ToInt32(Session["FolderID"]), Convert.ToInt32(Session["UserID"]));
        //foreach (GridViewRow i in gvData.Rows)
        //{
        //    Label lblllItemNo = (Label)i.FindControl("lblllItemNo");
        //    LinkButton lnkOpen1 = (LinkButton)i.FindControl("lnkOpen1");
        //    Label lblPageNo = (Label)i.FindControl("lblPageNo");
        //    AddCellToBody1(tableLayout1, lblllItemNo.Text);
        //    AddCellToBody1(tableLayout1, lnkOpen1.Text);
        //    AddCellToBody1(tableLayout1, lblPageNo.Text);

        //}
        string lblPageNo = "";
        string strPageCount = "";
        string strlastCount = "";
        foreach (DataRow dr in dtfiledetails1.Rows)
        {

            TableNameFrom = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Session["FolderID"]));
            string strQuery = string.Format(@"select a.fileid,b.filename,a.column0,a.column5,isnull(column1,column2) as column1 ,b.PageCount"
                               + ",(select isnull(sum(PageCount),0) + 1 from  {0} XX inner join tblfile YY on XX.fileid=YY.fileid where XX.FolderID={1} and XX.column0 <=a.column0) as 'StartPageNo'"
                               + " ,CASE   WHEN Len(Column1) > 2 THEN 'N'   ELSE 'A' END as Type "
                               + " from {0} as a inner join tblfile b on a.fileid=b.fileid where a.FolderID={1} and column0='{2}' order by column0", TableNameFrom, Session["FolderID"].ToString(), dr["ItemNo"].ToString());


            DataTable dtGetNoteAttachmentDetails = operation.GetTable4Command(strQuery);


            string lbpdfItemNo = dr["ItemNo"].ToString();
            string lblllItemNo = dr["ItemNo"].ToString().Substring(0, 2);
            string lbpdfItemNos = dr["Itemnos"].ToString();
            // string lblllItemNo = dr["ItemNo"].ToString();
            string lnkOpen1 = dr["Particulars"].ToString();

            if (dr["FolderID"].ToString() == "")
            {
                strPageCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}  and b.column0<'{2}'", TableNameFrom, Session["FolderID"].ToString(), dr["ItemNo"].ToString()));
            }
            else
            {
                strPageCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}  and b.column0<'{2}'", TableNameFrom, Session["FolderID"].ToString(), dr["ItemNo"].ToString()));
            }
            //strPageCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}  and b.column0<'{2}'", TableNameFrom, lblFolderId.Text.ToString(), lblllItemNo.Text.ToString()));
            if (Convert.ToInt32(lblllItemNo) > 08)
            {
                if (dr["FolderID"].ToString() == "")
                {
                    strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and b.column0<'{2}'", TableNameFrom, Session["FolderID"].ToString(), (Convert.ToInt32(lblllItemNo) + 1)));
                }
                else
                {

                    strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1} and b.column0<'{2}'", TableNameFrom, Session["FolderID"].ToString(), (Convert.ToInt32(lblllItemNo) + 1)));
                }
            }
            else
            {
                if (dr["FolderID"].ToString() == "")
                {
                    strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}and b.column0<'{2}'", TableNameFrom, Session["FolderID"].ToString(), "0" + (Convert.ToInt32(lblllItemNo) + 1)));
                }
                else
                {
                    strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}and b.column0<'{2}'", TableNameFrom, Session["FolderID"].ToString(), "0" + (Convert.ToInt32(lblllItemNo) + 1)));
                }

            }
            string ShowAttachmentGridviewNotebook = operation.ExecuteScalar4Command(string.Format(@"select Value from tblConfig where keys='ShowAttachmentGridviewNotebook'"));

            if (strPageCount == "")
            {
                if (dr["FolderID"].ToString() == "")
                {
                    strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}  and b.column0<'{2}'", TableNameFrom, Session["FolderID"].ToString(), "0" + (Convert.ToInt32(lblllItemNo) + 1)));
                }
                else
                {
                    strlastCount = operation.ExecuteScalar4Command(string.Format(@"select sum(a.pagecount) from tblfile a left outer join {0} b on a.fileid=b.fileid where a.folderid={1}  and b.column0<'{2}'", TableNameFrom, Session["FolderID"].ToString(), "0" + (Convert.ToInt32(lblllItemNo) + 1)));

                }
                if (ShowAttachmentGridviewNotebook.ToLower() == "yes")
                {
                    lblPageNo = "1" + " - " + dtGetNoteAttachmentDetails.Rows[0]["PageCount"].ToString();
                }
                else
                {
                    lblPageNo = "1" + " - " + strlastCount;
                }

                //lblPageNo = "1" + " - " + strlastCount;
            }
            else
            {
                if (ShowAttachmentGridviewNotebook.ToLower() == "yes")
                {
                    lblPageNo = (Convert.ToInt32(strPageCount) + 1) + " - " + (Convert.ToInt32(dtGetNoteAttachmentDetails.Rows[0]["StartPageNo"].ToString()) - 1);
                }
                else
                {
                    lblPageNo = (Convert.ToInt32(strPageCount) + 1) + " - " + strlastCount;
                }

                //lblPageNo = (Convert.ToInt32(strPageCount) + 1) + " - " + strlastCount;
            }
            //  Label lblPageNo = (Label)i.FindControl("lblPageNo");
            string Seperator = operation.ExecuteScalar4Command(string.Format(@"select Value from tblConfig where keys='seperator'"));

            if (dr["filename"].ToString() == "")
            {
                AddCellToBodySeprator(tableLayout1, lbpdfItemNos);
                AddCellToBodySeprator(tableLayout1, lnkOpen1);
                AddCellToBodySeprator(tableLayout1, "");
            }
            else
            {
                if (Seperator.ToLower() == "yes")
                {
                    AddCellToBody1(tableLayout1, lbpdfItemNos);
                }
                else
                {
                    AddCellToBody1(tableLayout1, lbpdfItemNo);
                }

                AddCellToBody3(tableLayout1, lnkOpen1);
                AddCellToBody1(tableLayout1, lblPageNo);
            }

        }


        return tableLayout1;
    }

    private static void AddCellToHeader1(PdfPTable tableLayout1, string cellText)
    {
        tableLayout1.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.NORMAL, 8, 1, iTextSharp.text.BaseColor.WHITE))) { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 5, BackgroundColor = new iTextSharp.text.BaseColor(0, 51, 102) });
    }
    private static void AddCellToHeadertital(PdfPTable tableLayout1, string cellText)
    {
        tableLayout1.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.NORMAL, 12, 1, iTextSharp.text.BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 5, BackgroundColor = new iTextSharp.text.BaseColor(238, 233, 233) });
        //tableLayout1.AddCell(new PdfPCell(new Phrase(cellText, FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 5, BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY });

    }
    private static void AddCellToBody3(PdfPTable tableLayout1, string cellText)
    {
        //tableLayout1.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.NORMAL, 8, 1, iTextSharp.text.BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 5, BackgroundColor = iTextSharp.text.BaseColor.WHITE });
        //ColumnText.ShowTextAligned(stamper.GetUnderContent(i), Element.ALIGN_RIGHT, new Phrase("Page " + j.ToString() + " of " + TotalPages, blackFont), 568f, 15f, 0);

        //tableLayout1.AddCell(new PdfPCell(new Phrase(cellText, FontFactory.GetFont("Arial", 10, Font.NORMAL))) { HorizontalAlignment = Element.ALIGN_LEFT, Padding = 5, Border = 1, PaddingLeft = -2f });
        tableLayout1.AddCell(new PdfPCell(new Phrase(cellText, FontFactory.GetFont("Arial", 9, Font.NORMAL))) { HorizontalAlignment = Element.ALIGN_LEFT, Padding = 5 });

    }
    private static void AddCellToBody4(PdfPTable tableLayout1, string cellText)
    {
        //tableLayout1.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.NORMAL, 8, 1, iTextSharp.text.BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 5, BackgroundColor = iTextSharp.text.BaseColor.WHITE });
        //ColumnText.ShowTextAligned(stamper.GetUnderContent(i), Element.ALIGN_RIGHT, new Phrase("Page " + j.ToString() + " of " + TotalPages, blackFont), 568f, 15f, 0);

        //tableLayout1.AddCell(new PdfPCell(new Phrase(cellText, FontFactory.GetFont("Arial", 10, Font.NORMAL))) { HorizontalAlignment = Element.ALIGN_LEFT, Padding = 5, Border = 1, PaddingLeft = -2f });
        tableLayout1.AddCell(new PdfPCell(new Phrase(cellText, FontFactory.GetFont("Arial", 9, Font.NORMAL))) { HorizontalAlignment = Element.ALIGN_LEFT, Padding = 5, BackgroundColor = new iTextSharp.text.BaseColor(255, 182, 193) });

    }
    private static void AddCellToBodySeprator(PdfPTable tableLayout1, string cellText)
    {
        //tableLayout1.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.NORMAL, 8, 1, iTextSharp.text.BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 5, BackgroundColor = iTextSharp.text.BaseColor.WHITE });
        //tableLayout1.AddCell(new PdfPCell(new Phrase(cellText, FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_LEFT, Padding = 5, BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY, Border = 0 });
        tableLayout1.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.NORMAL, 12, 1, iTextSharp.text.BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_LEFT, Padding = 5, BackgroundColor = new iTextSharp.text.BaseColor(141, 192, 210), PaddingLeft = 10f });
    }
    private static void AddCellToHeader6(PdfPTable tableLayout1, string cellText)
    {
        tableLayout1.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.NORMAL, 8, 1, iTextSharp.text.BaseColor.WHITE))) { HorizontalAlignment = Element.ALIGN_LEFT, Padding = 5, BackgroundColor = new iTextSharp.text.BaseColor(0, 51, 102) });
    }

    // Method to add single cell to the body
    private static void AddCellToBody1(PdfPTable tableLayout1, string cellText)
    {
        //tableLayout1.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.NORMAL, 8, 1, iTextSharp.text.BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 5, BackgroundColor = iTextSharp.text.BaseColor.WHITE });
        tableLayout1.AddCell(new PdfPCell(new Phrase(cellText, FontFactory.GetFont("Arial", 9, Font.NORMAL))) { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 5 });

    }


    private void MergePdfFiles()
    {
        try
        {
            CommonBAL objCommonBAL = new CommonBAL();
            OperationClass operation = new OperationClass();
            int loopcount = 0;

            string DestinationPath1 = null;
            string DestinationPath = null;
            string sourceFileName = null;
            string inderFileName = null;
            string FolderId = Session["FolderId"].ToString();
            string Foldername = Session["DMeetingdate"].ToString();
            Session["FolderId"] = FolderId;
            Session["Foldername"] = Foldername;
            string trg = "Agenda_" + Session["Foldername"] + "";
            string trgt = trg.Replace("/", "-");
            string ImagesavedFilePath = Convert.ToString(HttpContext.Current.Server.MapPath("~\\ExportFile\\" + Convert.ToString(HttpContext.Current.Session["UserName"]).Trim()));

            string sdfsdf = ImagesavedFilePath + "\\" + trgt;

            if (!Directory.Exists(ImagesavedFilePath))
            {
                Directory.CreateDirectory(ImagesavedFilePath);
            }
            else
            {
                objCommonBAL.DeleteTempFilesFromExportFolder(ImagesavedFilePath);
            }

            //string stout = sdfsdf + trgt + ".pdf";
            string stout = sdfsdf + ".pdf";


            string strdt = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblfolderindexmaster where folder_id={0}", FolderId));
            //  DataTable dt2 = operation.GetTable4Command(string.Format(@"select * from tblfile where FileId in (select FileId from {0} where FolderId = {1})", strdt, FolderId));
            string accesskey = operation.ExecuteScalar4Command(string.Format(@"select value from tblconfig where keys='AgendaAccessRestricted'"));
            DataTable dt2 = new DataTable();
            if (accesskey == "no")
            {
                dt2 = operation.GetTable4Command(string.Format(@"select tblfile.FileID ,tblfile.[FileName],{0}.Column0 from tblfile inner join {0} on tblfile.FileID = {0}.FileId where {0}.FolderId ={1} and {0}.FileId not in (select FileID from  tblAgendalevelAccessControl where userid='{2}' and {0}.FolderId={1}) order by {0}.Column0", strdt, FolderId, Session["UserID"]));
            }
            else if (accesskey == "yes")
            {
                dt2 = operation.GetTable4Command(string.Format(@"select tblfile.FileID ,tblfile.[FileName],{0}.Column0 from tblfile inner join {0} on tblfile.FileID = {0}.FileId where {0}.FolderId ={1} order by {0}.Column0", strdt, FolderId));
            }
            if (dt2.Rows.Count > 0)
            {
                if (accesskey == "no")
                {
                    string[] stin = new string[dt2.Rows.Count + 1];
                    Session["FileID"] = dt2.Rows[0]["FileID"];
                    Session["FileName"] = dt2.Rows[0]["FileName"];
                    int count = dt2.Rows.Count;
                    string[] strfilename = new string[count];
                    string[] strFileId = new string[count];
                    string DecryptFilePath;
                    DecryptFilePath = Convert.ToString(Server.MapPath("~/Repository//"));
                    DestinationPath1 = Convert.ToString(Server.MapPath("~/Repository//Decrypt//" + Session["UserName"].ToString()));
                    objCommonBAL.DeleteTempFilesFromExportFolder(DestinationPath1);


                    //objCommonBAL.DeleteTempFilesFromExportFolder(DecryptFilePath);

                    for (int i = 0; i <= dt2.Rows.Count - 1; i++)
                    {
                        string accessvalue = operation.ExecuteScalar4Command(string.Format(@"select * from   tblAgendalevelAccessControl where fileid='{0}' and userid='{1}'", dt2.Rows[i]["FileID"].ToString(), Session["UserID"]));

                        if (Path.GetExtension(dt2.Rows[i]["FileName"].ToString()).ToLower() == ".docx" || Path.GetExtension(dt2.Rows[i]["FileName"].ToString()).ToLower() == ".xlsx" || Path.GetExtension(dt2.Rows[i]["FileName"].ToString()).ToLower() == ".pptx")
                        {
                            strfilename[i] = Path.GetFileNameWithoutExtension(dt2.Rows[i]["FileName"].ToString()) + ".pdf";
                        }
                        else
                        {
                            strfilename[i] = dt2.Rows[i]["FileName"].ToString();
                        }
                        strFileId[i] = dt2.Rows[i]["FileID"].ToString();
                        Session["FileID"] = strFileId[i];
                        Session["FileName"] = strfilename[i];

                        if (Path.GetExtension(strfilename[i].ToLower()) == ".pdf")
                        {
                            string strfileId = Session["FileID"].ToString();
                            strFileName = Session["FileName"].ToString();


                            //List<string> lstReturnCodePath = objCommonBAL.CheckImageStorageEncryptDecryptPDFImage(DecryptFilePath, Convert.ToString(strfilename[i]), strFileId[i], "1", "", "");
                            //List<string> lstReturnCodePath = objCommonBAL.CheckImageStorageEncryptDecryptImage(DecryptFilePath, Convert.ToString(strfilename[i]), strFileId[i], "1", "", "");



                            string[] _strFileName = new string[count];
                            _strFileName[i] = strfilename[i];
                            sourceFileName = (DecryptFilePath + Path.GetFileNameWithoutExtension(_strFileName[i]) + "_" + (strFileId[i]) + Path.GetExtension(_strFileName[i]));


                            inderFileName = Server.MapPath("~/Report//PdfReports//AgendaIndex.pdf");

                            //DestinationPath = Convert.ToString(Server.MapPath(DestinationPath1 + "//" + Path.GetFileNameWithoutExtension(_strFileName[i]) + "_" + (HttpContext.Current.Session["FileID"]) + Path.GetExtension(_strFileName[i])));
                            //DestinationPath = Convert.ToString(Server.MapPath(DestinationPath1 + "//" + Path.GetFileNameWithoutExtension(_strFileName[i]) + "_" + (HttpContext.Current.Session["FileID"]) + Path.GetExtension(_strFileName[i])));
                            if (loopcount == 0)
                            {
                                if (File.Exists(inderFileName))
                                {

                                    string indexfiledelete = Server.MapPath("~/Report//PdfReports");
                                    loopcount = loopcount + 1;
                                    stin[loopcount - 1] = inderFileName;
                                    //objCommonBAL.DeleteTempFilesFromExportFolder(indexfiledelete);

                                }
                            }
                            //else
                            //{
                            if (File.Exists(sourceFileName))
                            {

                                //File.Copy(sourceFileName, DestinationPath, true);
                                loopcount = loopcount + 1;
                                stin[loopcount - 1] = sourceFileName;
                            }
                            //}

                        }


                    }
                    loopcount = 0;
                    int value;
                    value = MergePDFDocuments(stin, stout, 1);
                    if (value > 0)
                    {
                        AddPageNumber(stin);
                        //AddPageNumber();
                        //string NoteBookWithWaterMark= Convert.ToString(ConfigurationManager.AppSettings["NoteBookWithWaterMark"]);
                        //if (NoteBookWithWaterMark == "yes")
                        //{
                        //    FileDownLoadwithWaterMark(stout);

                        //}
                        //else
                        //{
                        //    //AddPageNumber();
                        //    //FileDownLoadwithoutWaterMark(stout);
                        //    //FileDownLoadwithWaterMark(stout);
                        //PdfFileDownLoad(stout);
                        //}
                        if (Convert.ToString(Session["Groupname"]).ToLower() == "directors")
                        {
                            FileDownLoadwithWaterMarkWithoutPwd(stout);
                        }
                        else
                        {
                            FileDownLoadwithWaterMark(stout);
                        }
                        //FileDownLoadwithoutWaterMark(stout);

                    }

                }
                else if (accesskey == "yes")
                {
                    if (dt2.Rows.Count > 0)
                    {
                        string[] stin = new string[dt2.Rows.Count + 1];
                        Session["FileID"] = dt2.Rows[0]["FileID"];
                        Session["FileName"] = dt2.Rows[0]["FileName"];
                        Session["UserSelected"] = Session["UserID"];
                        int count = dt2.Rows.Count;
                        string[] strfilename = new string[count];
                        string[] strFileId = new string[count];
                        string DecryptFilePath;
                        DecryptFilePath = Convert.ToString(Server.MapPath("~/Repository//"));
                        DestinationPath1 = Convert.ToString(Server.MapPath("~/Repository//Decrypt//" + Session["UserName"].ToString()));
                        objCommonBAL.DeleteTempFilesFromExportFolder(DestinationPath1);

                        for (int i = 0; i <= dt2.Rows.Count - 1; i++)
                        {
                            if (Path.GetExtension(dt2.Rows[i]["FileName"].ToString()).ToLower() == ".docx" || Path.GetExtension(dt2.Rows[i]["FileName"].ToString()).ToLower() == ".xlsx" || Path.GetExtension(dt2.Rows[i]["FileName"].ToString()).ToLower() == ".pptx")
                            {
                                strfilename[i] = Path.GetFileNameWithoutExtension(dt2.Rows[i]["FileName"].ToString()) + ".pdf";
                            }
                            else
                            {
                                strfilename[i] = dt2.Rows[i]["FileName"].ToString();
                            }
                            strFileId[i] = dt2.Rows[i]["FileID"].ToString();
                            Session["FileID"] = strFileId[i];
                            Session["FileName"] = strfilename[i];

                            if (Path.GetExtension(strfilename[i].ToLower()) == ".pdf")
                            {
                                string strfileId = Session["FileID"].ToString();


                                string[] _strFileName = new string[count];
                                _strFileName[i] = strfilename[i];
                                sourceFileName = (DecryptFilePath + Path.GetFileNameWithoutExtension(_strFileName[i]) + "_" + (strFileId[i]) + Path.GetExtension(_strFileName[i]));


                                inderFileName = Server.MapPath("~/Report//PdfReports//AgendaIndex.pdf");


                                if (loopcount == 0)
                                {
                                    if (File.Exists(inderFileName))
                                    {

                                        string indexfiledelete = Server.MapPath("~/Report//PdfReports");
                                        loopcount = loopcount + 1;
                                        stin[loopcount - 1] = inderFileName;


                                    }
                                }

                                if (File.Exists(sourceFileName))
                                {


                                    loopcount = loopcount + 1;
                                    string attach = dt2.Rows[i]["Column0"].ToString();
                                    Session["Attach"] = "";

                                    dtDeniePath = operation.GetTable4Command(string.Format(@"select tblAgendalevelAccessControl.Access,tblfile.PageCount from tblAgendalevelAccessControl inner join tblfile on tblAgendalevelAccessControl.FileID=tblfile.FileID where tblAgendalevelAccessControl.fileid='{0}' and tblAgendalevelAccessControl.userid='{1}'", dt2.Rows[i]["fileid"].ToString(), Session["UserID"]));
                                    if (attach.Length >= 3)
                                    {
                                        string strSub = attach.Substring(0, 2);
                                        string fileid = operation.ExecuteScalar4Command(string.Format(@"select TOP 1 FILEID from {0}  WHERE COLUMN0 LIKE '{1}%' AND FOLDERID={2}", strdt, strSub, Session["FolderID"]));

                                        dtDeniePath = operation.GetTable4Command(string.Format(@"SELECT * FROM TBLAGENDALEVELACCESSCONTROL WHERE FILEID='{0}' AND USERID='{1}'", fileid, Session["UserID"]));
                                    }

                                    if (dtDeniePath.Rows.Count > 0)
                                    {


                                        string AccessRestrict = "";
                                        FileGenerateWithAccessDenied(AccessRestrict, dt2.Rows[i]["FileID"].ToString());
                                        FileDownLoadwithAccessDenied(AccessRestrict, dt2.Rows[i]["FileID"].ToString());
                                        AccessDeniePath = Convert.ToString(Server.MapPath("~//Report//PdfReports//AccessDenie//" + Session["UserName"].ToString() + "//" + "AccessDenie" + Session["FileID"] + "pdf.pdf"));
                                        stin[loopcount - 1] = AccessDeniePath;
                                        sourceFileName = "";

                                        Session["Attach"] = dt2.Rows[i]["FileID"].ToString();
                                    }
                                    else
                                    {
                                        stin[loopcount - 1] = sourceFileName;
                                    }

                                }


                            }

                        }
                        loopcount = 0;
                        int value;
                        value = MergePDFDocuments(stin, stout, 1);
                        if (value > 0)
                        {

                            AddPageNumber();

                            //FileDownLoadwithWaterMark(stout);
                            FileDownLoadwithWaterMarkWithoutPwd(stout);

                        }

                    }
                }

            }
        }
        catch (Exception ex)
        {

        }
    }

    protected void AddPageNumber()
    {
        string indexpath = HttpContext.Current.Server.MapPath("~/Report//PdfReports//AgendaIndex.pdf");

        byte[] bytes1 = File.ReadAllBytes(indexpath);
        PdfReader reader1 = new PdfReader(bytes1);
        int indexPages = reader1.NumberOfPages;

        string FolderId = Session["FolderId"].ToString();
        string Foldername = Session["DMeetingdate"].ToString();
        Session["FolderId"] = FolderId;
        Session["Foldername"] = Foldername;
        string trg = "Agenda_" + Session["Foldername"] + "";
        string trgt = trg.Replace("/", "-");
        string ImagesavedFilePath = Convert.ToString(HttpContext.Current.Server.MapPath("~\\ExportFile\\" + Convert.ToString(HttpContext.Current.Session["UserName"]).Trim()));
        string sdfsdf = ImagesavedFilePath + "\\" + trgt;
        string stout = sdfsdf + ".pdf";

        byte[] bytes = File.ReadAllBytes(stout);
        Font blackFont = FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.BLACK);
        using (MemoryStream stream = new MemoryStream())
        {
            PdfReader reader = new PdfReader(bytes);
            using (PdfStamper stamper = new PdfStamper(reader, stream))
            {
                int j = 1;
                int pages = reader.NumberOfPages;
                int TotalPages = pages - indexPages;
                for (int i = 1; i <= pages; i++)
                {
                    if (i > indexPages)
                    {
                        ColumnText.ShowTextAligned(stamper.GetOverContent(i), Element.ALIGN_RIGHT, new Phrase("Page " + j.ToString() + " of " + TotalPages, blackFont), 568f, 15f, 0);
                        j++;
                    }
                    else
                    {
                        ColumnText.ShowTextAligned(stamper.GetOverContent(i), Element.ALIGN_RIGHT, new Phrase("Index -" + i.ToString(), blackFont), 568f, 15f, 0);

                    }
                }
            }
            bytes = stream.ToArray();
        }
        File.WriteAllBytes(stout, bytes);
    }

    protected void AddPageNumber(string[] sourcePdfPath)
    {
        string TableNameFrom = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Convert.ToInt32(Session["FolderID"])));
        DataTable dtfiledetails1 = objCommonBAL.GetDispFileName(TableNameFrom, Convert.ToInt32(Session["FolderID"]), Convert.ToInt32(Session["UserID"]));
        string indexpath = HttpContext.Current.Server.MapPath("~/Report//PdfReports//AgendaIndex.pdf");

        byte[] bytes1 = File.ReadAllBytes(indexpath);
        PdfReader reader1 = new PdfReader(bytes1);
        int indexPages = reader1.NumberOfPages;

        string FolderId = Session["FolderId"].ToString();
        string Foldername = Session["DMeetingdate"].ToString();
        Session["FolderId"] = FolderId;
        Session["Foldername"] = Foldername;
        string trg = "Agenda_" + Session["Foldername"] + "";
        string trgt = trg.Replace("/", "-");
        string ImagesavedFilePath = Convert.ToString(HttpContext.Current.Server.MapPath("~\\ExportFile\\" + Convert.ToString(HttpContext.Current.Session["UserName"]).Trim()));
        string sdfsdf = ImagesavedFilePath + "\\" + trgt;
        string stout = sdfsdf + ".pdf";

        byte[] bytes = File.ReadAllBytes(stout);
        Font blackFont = FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.BLACK);
        Font blackFont1 = FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK);
        using (MemoryStream stream = new MemoryStream())
        {
            PdfReader reader = new PdfReader(bytes);
            using (PdfStamper stamper = new PdfStamper(reader, stream))
            {
                int n = indexPages;
                int j = 1;
                int pages = reader.NumberOfPages;
                int TotalPages = pages - indexPages;

                for (int i = 1; i <= pages; i++)
                {
                    if (i > indexPages)
                    {
                        ColumnText.ShowTextAligned(stamper.GetOverContent(i), Element.ALIGN_RIGHT, new Phrase("Page " + j.ToString() + " of " + TotalPages, blackFont), 568f, 15f, 0);
                        j++;
                    }
                    else
                    {
                        ColumnText.ShowTextAligned(stamper.GetOverContent(i), Element.ALIGN_RIGHT, new Phrase("Index -" + i.ToString(), blackFont), 568f, 15f, 0);
                        //tableLayout1.AddCell(new PdfPCell(new Phrase(cellText, FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 5, BackgroundColor = new iTextSharp.text.BaseColor(238, 233, 233) });

                    }
                }
                for (int m = 1; m < sourcePdfPath.Length; m++)
                {

                    if (m == 1)
                    {
                        n = n + 1;
                    }
                    byte[] bytes2 = File.ReadAllBytes(sourcePdfPath[m]);
                    PdfReader reader2 = new PdfReader(bytes2);
                    int agendaPages = reader2.NumberOfPages;
                    if (agendaPages > 0)
                    {
                        string Particulars = "";
                        string lblitemno = dtfiledetails1.Rows[m - 1]["ItemNo"].ToString();
                        string lblparticulars = dtfiledetails1.Rows[m - 1]["particulars"].ToString();
                        if (lblparticulars.Length >= 100)
                        {
                            Particulars = lblparticulars.Substring(0, 100) + "..........";
                            //Particulars = lblparticulars;
                        }
                        else
                        {
                            Particulars = lblparticulars;
                        }
                        for (int k = 0; k < agendaPages; k++)
                        {
                            if (lblitemno.Length <= 2)
                            {
                                //ColumnText.ShowTextAligned(stamper.GetUnderContent(n), Element.ALIGN_RIGHT, new Phrase("Note:" + lblitemno + ":" + lblparticulars, blackFont), rect.getRight(), rect.getTop(), 0);new Phrase(text, new iTextSharp.text.Font() { Size = 10 })
                                //ColumnText.ShowTextAligned(stamper.GetOverContent(n), Element.TITLE, new Phrase("Note:" + lblitemno + ":" + Particulars, blackFont), 208f, 15f, 0);
                                ColumnText.ShowTextAligned(stamper.GetOverContent(n), Element.ALIGN_MIDDLE, new Phrase("Note:" + lblitemno + ":" + Particulars, blackFont1), 80f, 50f, 0);

                            }
                            else
                            {
                                ColumnText.ShowTextAligned(stamper.GetOverContent(n), Element.TITLE, new Phrase("Attachment:" + lblitemno + ":" + Particulars, blackFont), 208f, 15f, 0);

                            }
                            n++;
                        }

                    }
                }
            }
            bytes = stream.ToArray();
        }
        File.WriteAllBytes(stout, bytes);
    }

    //protected void AddPageNumber(string[] sourcePdfPath)
    //{
    //    string TableNameFrom = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Convert.ToInt32(Session["FolderID"])));
    //    DataTable dtfiledetails1 = objCommonBAL.GetDispFileName(TableNameFrom, Convert.ToInt32(Session["FolderID"]), Convert.ToInt32(Session["UserID"]));
    //    string indexpath = HttpContext.Current.Server.MapPath("~/Report//PdfReports//AgendaIndex.pdf");

    //    byte[] bytes1 = File.ReadAllBytes(indexpath);
    //    PdfReader reader1 = new PdfReader(bytes1);
    //    int indexPages = reader1.NumberOfPages;

    //    string FolderId = Session["FolderId"].ToString();
    //    string Foldername = Session["DMeetingdate"].ToString();
    //    Session["FolderId"] = FolderId;
    //    Session["Foldername"] = Foldername;
    //    string trg = "Agenda_" + Session["Foldername"] + "";
    //    string trgt = trg.Replace("/", "-");
    //    string ImagesavedFilePath = Convert.ToString(HttpContext.Current.Server.MapPath("~\\ExportFile\\" + Convert.ToString(HttpContext.Current.Session["UserName"]).Trim()));
    //    string sdfsdf = ImagesavedFilePath + "\\" + trgt;
    //    string stout = sdfsdf + ".pdf";

    //    byte[] bytes = File.ReadAllBytes(stout);
    //    Font blackFont = FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.BLACK);
    //    using (MemoryStream stream = new MemoryStream())
    //    {
    //        PdfReader reader = new PdfReader(bytes);
    //        using (PdfStamper stamper = new PdfStamper(reader, stream))
    //        {
    //            int n = indexPages;
    //            int j = 1;
    //            int pages = reader.NumberOfPages;
    //            int TotalPages = pages - indexPages;

    //            for (int i = 1; i <= pages; i++)
    //            {
    //                if (i > indexPages)
    //                {
    //                    ColumnText.ShowTextAligned(stamper.GetOverContent(i), Element.ALIGN_RIGHT, new Phrase("Page " + j.ToString() + " of " + TotalPages, blackFont), 568f, 15f, 0);
    //                    j++;
    //                }
    //                else
    //                {
    //                    ColumnText.ShowTextAligned(stamper.GetOverContent(i), Element.ALIGN_RIGHT, new Phrase("Index -" + i.ToString(), blackFont), 568f, 15f, 0);
    //                }
    //            }
    //            for (int m = 1; m < sourcePdfPath.Length; m++)
    //            {

    //                if (m == 1)
    //                {
    //                    n = n + 1;
    //                }
    //                byte[] bytes2 = File.ReadAllBytes(sourcePdfPath[m]);
    //                PdfReader reader2 = new PdfReader(bytes2);
    //                int agendaPages = reader2.NumberOfPages;
    //                if (agendaPages > 0)
    //                {
    //                    string lblitemno = dtfiledetails1.Rows[m - 1]["ItemNo"].ToString();
    //                    string lblparticulars = dtfiledetails1.Rows[m - 1]["particulars"].ToString();
    //                    for (int k = 0; k < agendaPages; k++)
    //                    {
    //                        if (lblitemno.Length <= 2)
    //                        {
    //                            //ColumnText.ShowTextAligned(stamper.GetUnderContent(n), Element.ALIGN_RIGHT, new Phrase("Note:" + lblitemno + ":" + lblparticulars, blackFont), rect.getRight(), rect.getTop(), 0);
    //                            ColumnText.ShowTextAligned(stamper.GetOverContent(n), Element.TITLE, new Phrase("Note:" + lblitemno + ":" + lblparticulars, blackFont), 208f, 15f, 0);
    //                        }
    //                        else
    //                        {
    //                            ColumnText.ShowTextAligned(stamper.GetOverContent(n), Element.TITLE, new Phrase("Attachment:" + lblitemno + ":" + lblparticulars, blackFont), 208f, 15f, 0);

    //                        }
    //                        n++;
    //                    }

    //                }
    //            }
    //        }
    //        bytes = stream.ToArray();
    //    }
    //    File.WriteAllBytes(stout, bytes);
    //}

    private int MergePDFDocuments(string[] sourcePdfPath, string outputPdfPath, int startPage)
    {
        int value;
        PdfReader reader1 = null;
        PdfReader reader2 = null;
        Document sourceDocument = null;
        PdfCopy pdfCopyProvider = null;
        PdfImportedPage importedPage = null;
        Document inputDocument = null;
        try
        {


            PdfReader[] reader = new PdfReader[sourcePdfPath.Length];
            int[] endpage = new int[sourcePdfPath.Length];

            for (int i = 0; i <= sourcePdfPath.Length - 1; i++)
            {
                if (sourcePdfPath[i] != null)
                {
                    reader[i] = new PdfReader(sourcePdfPath[i]);

                    endpage[i] = reader[i].NumberOfPages;

                    sourceDocument = new Document(reader[i].GetPageSizeWithRotation(startPage));
                }
            }
            pdfCopyProvider = new PdfCopy(sourceDocument,
                   new System.IO.FileStream(outputPdfPath, System.IO.FileMode.CreateNew));

            sourceDocument.Open();
            // For simplicity, I am assuming all the pages share the same size
            // and rotation as the first page:
            for (int i = 0; i <= sourcePdfPath.Length - 1; i++)
            {
                for (int j = startPage; j <= endpage[i]; j++)
                {
                    importedPage = pdfCopyProvider.GetImportedPage(reader[i], j);
                    pdfCopyProvider.AddPage(importedPage);
                }
            }

            sourceDocument.Close();

            value = 1;
        }
        catch (Exception ex)
        {
            value = 1;
        }
        return value;
    }

    protected void btnBackFromArchive_Click(object sender, EventArgs e)
    {
        OperationClass obj = new OperationClass();
        string SqlQuery = "update tblfolder set meetingstatus=0 where folderid='" + Session["FolderId"] + "'";
        int value = obj.ExecuteNonQuery(SqlQuery);
        if (value > 0)
        {
            ScriptManager.RegisterClientScriptBlock(Page, typeof(UpdatePanel), "msg", "alert('Status updated successfully.')", true);

        }
    }

    protected void btnPush_Click(object sender, EventArgs e)
    {
        OperationClass objOperationClass = new OperationClass();
        if (Convert.ToString(Session["Groupname"]).ToLower() == "admin")
        {

            string SqlQuery = "SELECT [FolderName], [FolderId] FROM [tblFolder] WHERE  ([ParentFolderId] = 1) AND   [DeleteStatus] !=1 AND  [MEETINGSTATUS]!=1 AND [MEETINGCANCELLED]!=1 and lower(FolderName)!='archived meetings' and Foldername!='Repository' and lower(FolderName)!='cancelled Meetings'";
            DataTable dt = objOperationClass.GetTable4Command(SqlQuery);
            if (dt.Rows.Count != 0)
            {
                ddlCommittee.DataSource = dt;
                ddlCommittee.DataTextField = "FolderName";
                ddlCommittee.DataValueField = "FolderId";
                ddlCommittee.DataBind();
                ddlCommittee.Items.Insert(0, new System.Web.UI.WebControls.ListItem("---Select---", "0"));
                ddlMeetingDate.Items.Insert(0, new System.Web.UI.WebControls.ListItem("---Select---", "0"));
            }
        }
        else if (Convert.ToString(Session["Groupname"]).ToLower() != "admin")
        {
            objCommonBAL = new CommonBAL();
            string SqlQuery = "SELECT [FolderName], [FolderId] FROM [tblFolder] WHERE  ([ParentFolderId] = 1) AND   [DeleteStatus] !=1 AND  [MEETINGSTATUS]!=1 AND [MEETINGCANCELLED]!=1 and lower(FolderName)!='archived meetings' and Foldername!='Repository'  and lower(FolderName)!='cancelled Meetings'";
            DataTable dt = objOperationClass.GetTable4Command(SqlQuery);
            if (dt.Rows.Count != 0)
            {
                ddlCommittee.DataSource = dt;
                ddlCommittee.DataTextField = "FolderName";
                ddlCommittee.DataValueField = "FolderId";
                ddlCommittee.DataBind();
                ddlCommittee.Items.Insert(0, new System.Web.UI.WebControls.ListItem("---Select---", "0"));
                ddlMeetingDate.Items.Insert(0, new System.Web.UI.WebControls.ListItem("---Select---", "0"));
            }
        }


        pnlMove.Visible = true;

    }

    protected void chkSelect_CheckedChanged(object sender, EventArgs e)
    {

        if (chkSelectalll.Checked == false)
        {
            if (R == 0)
            {
                foreach (GridViewRow j in gvData.Rows)
                {
                    CheckBox chk = (CheckBox)j.FindControl("chkSelect");
                    Label lblItemNo = (Label)j.FindControl("LinkButton1");
                    Label lblFileId = (Label)j.FindControl("lblFileId");

                    if (chk.Checked == true)
                    {
                        ViewState["FirstSelected"] = lblItemNo.Text;
                        ViewState["FileId"] = lblFileId.Text;
                        R++;
                    }
                }
            }
            else
                if (R == 1)
                {
                    int w = 0;

                    foreach (GridViewRow j in gvData.Rows)
                    {
                        w++;
                        CheckBox chk = (CheckBox)j.FindControl("chkSelect");
                        Label lblItemNo = (Label)j.FindControl("LinkButton1");
                        if (chk.Checked == true)
                        {
                            ViewState["FirstSelected"] = ViewState["FirstSelected"];
                            break;
                        }
                        if (gvData.Rows.Count == w)
                        {
                            ViewState["FirstSelected"] = null;
                            ViewState["SecondSelected"] = null;
                        }
                    }
                    if (ViewState["FirstSelected"] != null || Convert.ToString(ViewState["FirstSelected"]) != "")
                    {
                        int o = 0;
                        foreach (GridViewRow j in gvData.Rows)
                        {
                            CheckBox chk = (CheckBox)j.FindControl("chkSelect");
                            Label lblItemNo = (Label)j.FindControl("LinkButton1");


                            if (chk.Checked == true)
                            {
                                o++;
                                if (ViewState["FirstSelected"].ToString() != lblItemNo.Text)
                                {
                                    ViewState["SecondSelected"] = lblItemNo.Text;

                                    R++;
                                }
                            }
                            else
                                if (ViewState["SecondSelected"] == null && 0 > 1)
                                {
                                    R = 0;
                                }


                        }
                    }
                    else
                    {
                        R = 0;
                        return;
                    }

                }
                else
                {
                    R = 0;
                    int h = 0;
                    foreach (GridViewRow u in gvData.Rows)
                    {
                        CheckBox chk = (CheckBox)u.FindControl("chkSelect");
                        Label lblItemNo = (Label)u.FindControl("LinkButton1");
                        Label lblFileId = (Label)u.FindControl("lblFileId");

                        if (chk.Checked == true)
                        {
                            h++;
                        }
                        if (h > 2)
                        {
                            ViewState["FirstSelected"] = null;
                            ViewState["SecondSelected"] = null;
                            return;
                        }
                    }
                    foreach (GridViewRow j in gvData.Rows)
                    {
                        CheckBox chk = (CheckBox)j.FindControl("chkSelect");
                        Label lblItemNo = (Label)j.FindControl("LinkButton1");
                        Label lblFileId = (Label)j.FindControl("lblFileId");

                        if (chk.Checked == true)
                        {
                            ViewState["FirstSelected"] = lblItemNo.Text;
                            ViewState["FileId"] = lblFileId.Text;
                            R++;
                            if (h == 1)
                            {
                                ViewState["SecondSelected"] = null;
                            }
                            return;
                        }
                    }
                    if (chkSelectalll.Checked == false)
                    {
                        //DirectorDataBinding();
                        ViewState["FirstSelected"] = null;
                        ViewState["SecondSelected"] = null;
                        // ScriptManager.RegisterClientScriptBlock(Page, typeof(UpdatePanel), "msg", "alert('Select only two items.')", true);
                    }
                }
        }

    }

    protected void btnSwap_Click(object sender, EventArgs e)
    {



        if (ViewState["FirstSelected"] != null && ViewState["SecondSelected"] != null)
        {
            string strFirstAgendaNo = ViewState["FirstSelected"].ToString();
            string strSecondAgendaNo = ViewState["SecondSelected"].ToString();
            string strFirstAgenda = "";
            string strSecondAgenda = "";
            string TableName = "";
            string FolderIDs = "";
            OperationClass operation = new OperationClass();
            if (Request.QueryString["Value"] != null)
            {
                if (Request.QueryString["Value"].ToString().ToLower() == "my briefcase")
                {
                    FolderIDs = operation.ExecuteScalar4Command(string.Format(@"select Folderid from tblfolder where foldername='my briefcase' and deletestatus!=1"));
                    TableName = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Convert.ToInt32(FolderIDs)));
                }
                else
                    if (Request.QueryString["Value"].ToString().ToLower() == "company info")
                    {
                        FolderIDs = operation.ExecuteScalar4Command(string.Format(@"select Folderid from tblfolder where foldername='company info' and deletestatus!=1"));
                        TableName = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Convert.ToInt32(FolderIDs)));
                    }
                    else
                    {
                        return;
                    }
            }
            else
            {


                TableName = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Convert.ToInt32(HttpContext.Current.Session["FolderID"])));
            }
            string strFirstAgendaNos = strFirstAgendaNo;
            string strSecondAgendaNos = strSecondAgendaNo;
            strFirstAgenda = strFirstAgendaNos;
            strSecondAgenda = strSecondAgendaNos;
            DataTable dtFirst = null;
            DataTable dtSecond = null;
            if ((strFirstAgenda != null && strSecondAgenda != null) || (strFirstAgenda != "" && strSecondAgenda != ""))
            {
                string strFileID = "";
                if (Request.QueryString["Value"] != null)
                {
                    if (Request.QueryString["Value"].ToString().ToLower() == "my briefcase")
                    {
                        dtFirst = operation.GetTable4Command(string.Format(@"select Column0,fileid from {0} where folderid={1} and Column0  like '{2}%'", TableName, Convert.ToInt32(FolderIDs), strFirstAgenda));
                        dtSecond = operation.GetTable4Command(string.Format(@"select Column0,fileid from {0} where folderid={1} and Column0  like '{2}%'", TableName, Convert.ToInt32(FolderIDs), strSecondAgenda));
                    }
                    else
                        if (Request.QueryString["Value"].ToString().ToLower() == "company info")
                        {
                            dtFirst = operation.GetTable4Command(string.Format(@"select Column0,fileid from {0} where folderid={1} and Column0  like '{2}%'", TableName, Convert.ToInt32(FolderIDs), strFirstAgenda));
                            dtSecond = operation.GetTable4Command(string.Format(@"select Column0,fileid from {0} where folderid={1} and Column0  like '{2}%'", TableName, Convert.ToInt32(FolderIDs), strSecondAgenda));
                        }
                        else
                        {
                            return;
                        }

                }
                else
                {
                    dtFirst = operation.GetTable4Command(string.Format(@"select Column0,fileid from {0} where folderid={1} and Column0  like '{2}%'", TableName, Convert.ToInt32(HttpContext.Current.Session["FolderID"]), strFirstAgenda));
                    dtSecond = operation.GetTable4Command(string.Format(@"select Column0,fileid from {0} where folderid={1} and Column0  like '{2}%'", TableName, Convert.ToInt32(HttpContext.Current.Session["FolderID"]), strSecondAgenda));
                }

                int Value1;
                foreach (DataRow dr in dtSecond.Rows)
                {
                    if (dr["Column0"].ToString().Length > 2)
                    {

                        string sss = string.Concat(strFirstAgenda.ToString().Substring(0, 2), dr["Column0"].ToString().Substring(2, (dr["Column0"].ToString().Length - 2)));
                        Value1 = operation.Insert4Command(string.Format(@"update {0} set column0='{1}' where fileid='{2}'", TableName, sss, dr["fileid"].ToString()));
                    }
                    else
                    {
                        Value1 = operation.Insert4Command(string.Format(@"update {0} set column0='{1}' where fileid='{2}'", TableName, strFirstAgenda, dr["fileid"].ToString()));
                    }
                }
                foreach (DataRow dr in dtFirst.Rows)
                {
                    if (dr["Column0"].ToString().Length > 2)
                    {

                        string sss = string.Concat(strSecondAgenda.ToString().Substring(0, 2), dr["Column0"].ToString().Substring(2, (dr["Column0"].ToString().Length - 2)));
                        Value1 = operation.Insert4Command(string.Format(@"update {0} set column0='{1}' where fileid='{2}'", TableName, sss, dr["fileid"].ToString()));
                    }
                    else
                    {
                        Value1 = operation.Insert4Command(string.Format(@"update {0} set column0='{1}' where fileid='{2}'", TableName, strSecondAgenda, dr["fileid"].ToString()));
                    }
                }


                //strFileID = operation.ExecuteScalar4Command(string.Format(@"select FileID from {0} where folderid={1} and Column0='{2}'", TableName, Convert.ToInt32(HttpContext.Current.Session["FolderID"]), strSecondAgenda));
                //int Value = operation.Insert4Command(string.Format(@"update {0} set column0='{1}' where column0='{2}'", TableName, strSecondAgenda, strFirstAgenda));
                //int Value1 = operation.Insert4Command(string.Format(@"update {0} set column0='{1}' where fileid='{2}'", TableName, strFirstAgenda, strFileID));

            }
            ViewState["FirstSelected"] = null;
            ViewState["SecondSelected"] = null;

        }
        else
        {

            ViewState["FirstSelected"] = null;
            ViewState["SecondSelected"] = null;
            ScriptManager.RegisterClientScriptBlock(Page, typeof(UpdatePanel), "msg", "alert('Select only two items.')", true);
        }
        DirectorDataBinding();
        R = 0;

    }
    protected void btnMoveDown_Click(object sender, EventArgs e)
    {


        if (ViewState["FirstSelected"] != null && ViewState["SecondSelected"] != null)
        {
            string strFirst = ViewState["FirstSelected"].ToString();
            string strSecond = ViewState["SecondSelected"].ToString();
            if (Convert.ToInt32(ViewState["FirstSelected"]) > Convert.ToInt32(ViewState["SecondSelected"]))
            {
                string TableName = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Convert.ToInt32(Session["FolderID"])));
                DataTable dtfiledetailsnew = operation.GetTable4Command(string.Format(@"select  FileID,substring(Column0,0,3) as Colun,Column0 from {0} where Column0>='{1}'  and Column0<'{2}' and Column0 not like '{2}%' and folderid={3} order by Column0", TableName, strSecond, strFirst, Convert.ToInt32(Session["FolderID"])));
                DataTable dtfiledetails = operation.GetTable4Command(string.Format(@"select  FileID,substring(Column0,0,3) as Colun,Column0 from {0} where Column0  like '{1}%' and Column0 !='{1}' and folderid={2} order by Column0", TableName, strFirst, Convert.ToInt32(Session["FolderID"])));
                for (int j = 0; j < dtfiledetailsnew.Rows.Count; j++)
                {
                    string stttttttt = Convert.ToString(Convert.ToInt32(dtfiledetailsnew.Rows[j]["Colun"].ToString()) + 1);
                    string sttttttttw = dtfiledetailsnew.Rows[j]["Column0"].ToString();
                    if (Convert.ToInt32(dtfiledetailsnew.Rows[j]["Colun"].ToString()) < 9)
                    {
                        int Value = operation.Insert4Command(string.Format(@"update {0} set column0='{1}' where FileID='{2}'", TableName, ("0" + Convert.ToInt32(stttttttt) + sttttttttw.Substring(2, sttttttttw.Length - 2)), dtfiledetailsnew.Rows[j]["FileID"].ToString()));
                    }
                    else
                    {
                        int Value = operation.Insert4Command(string.Format(@"update {0} set column0='{1}' where FileID='{2}'", TableName, (stttttttt + sttttttttw.Substring(2, sttttttttw.Length - 2)), dtfiledetailsnew.Rows[j]["FileID"].ToString()));
                    }

                }

                int FirsttoSeconds = operation.Insert4Command(string.Format(@"update {0} set column0='{1}' where FileID='{2}'", TableName, ViewState["SecondSelected"], ViewState["FileId"]));
                if (dtfiledetails != null && dtfiledetails.Rows.Count > 0)
                {
                    for (int k = 0; k < dtfiledetails.Rows.Count; k++)
                    {
                        string strrrw = dtfiledetails.Rows[k]["column0"].ToString();
                        string subw = strrrw.Substring(2, strrrw.Length - 2);
                        if (Convert.ToInt32(ViewState["SecondSelected"]) < 10)
                        {
                            subw = "0" + Convert.ToInt32(ViewState["SecondSelected"]) + "" + subw;
                        }
                        else
                        {
                            subw = Convert.ToInt32(ViewState["SecondSelected"]) + "" + subw;
                        }

                        int FirsttoSecond = operation.Insert4Command(string.Format(@"update {0} set column0='{1}' where FileID='{2}'", TableName, subw, dtfiledetails.Rows[k]["FileID"].ToString()));
                    }
                }

            }
            else
            {
                string TableName = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Convert.ToInt32(Session["FolderID"])));
                DataTable dtfiledetailsnew = operation.GetTable4Command(string.Format(@"select  FileID,substring(Column0,0,3) as Colun,Column0 from {0} where (Column0<='{1}'  or Column0 like '{1}%')  and Column0>'{2}' and Column0 not like '{2}%' and folderid={3} order by Column0", TableName, strSecond, strFirst, Convert.ToInt32(Session["FolderID"])));
                DataTable dtfiledetails = operation.GetTable4Command(string.Format(@"select  FileID,substring(Column0,0,3) as Colun,Column0 from {0} where Column0  like '{1}%' and Column0 !='{1}' and folderid={2} order by Column0", TableName, strFirst, Convert.ToInt32(Session["FolderID"])));
                for (int j = 0; j < dtfiledetailsnew.Rows.Count; j++)
                {
                    string stttttttt = Convert.ToString(Convert.ToInt32(dtfiledetailsnew.Rows[j]["Colun"].ToString()) - 1);
                    string sttttttttw = dtfiledetailsnew.Rows[j]["Column0"].ToString();
                    if (Convert.ToInt32(dtfiledetailsnew.Rows[j]["Colun"].ToString()) < 11)
                    {
                        int Value = operation.Insert4Command(string.Format(@"update {0} set column0='{1}' where FileID='{2}'", TableName, ("0" + Convert.ToInt32(stttttttt) + sttttttttw.Substring(2, sttttttttw.Length - 2)), dtfiledetailsnew.Rows[j]["FileID"].ToString()));
                    }
                    else
                    {
                        int Value = operation.Insert4Command(string.Format(@"update {0} set column0='{1}' where FileID='{2}'", TableName, (stttttttt + sttttttttw.Substring(2, sttttttttw.Length - 2)), dtfiledetailsnew.Rows[j]["FileID"].ToString()));
                    }

                }

                int FirsttoSeconds = operation.Insert4Command(string.Format(@"update {0} set column0='{1}' where FileID='{2}'", TableName, ViewState["SecondSelected"], ViewState["FileId"]));
                if (dtfiledetails != null && dtfiledetails.Rows.Count > 0)
                {
                    for (int k = 0; k < dtfiledetails.Rows.Count; k++)
                    {
                        string strrrw = dtfiledetails.Rows[k]["column0"].ToString();
                        string subw = strrrw.Substring(2, strrrw.Length - 2);
                        if (Convert.ToInt32(ViewState["SecondSelected"]) < 10)
                        {
                            subw = "0" + Convert.ToInt32(ViewState["SecondSelected"]) + "" + subw;
                        }
                        else
                        {
                            subw = Convert.ToInt32(ViewState["SecondSelected"]) + "" + subw;
                        }

                        int FirsttoSecond = operation.Insert4Command(string.Format(@"update {0} set column0='{1}' where FileID='{2}'", TableName, subw, dtfiledetails.Rows[k]["FileID"].ToString()));
                    }
                }


            }
            ViewState["FirstSelected"] = null;
            ViewState["SecondSelected"] = null;
        }
        else
        {

            ViewState["FirstSelected"] = null;
            ViewState["SecondSelected"] = null;
            ScriptManager.RegisterClientScriptBlock(Page, typeof(UpdatePanel), "msg", "alert('Select only two items.')", true);
        }
        DirectorDataBinding();
        R = 0;


    }

    protected void pnlSubmit_Click(object sender, EventArgs e)
    {

    }

    protected void pnlCancelMove_Click(object sender, EventArgs e)
    {

    }

    [WebMethod]
    public static int Getusers(string ee, string rr)
    {
        return 1;
    }

    protected void btnInvitee_Click(object sender, EventArgs e)
    {
        bindGvUserDetail();

        btnMeeting.Visible = true;
        btnCancel.Visible = true;
    }

    protected void btnPublish_Click(object sender, EventArgs e)
    {
        OperationClass objOperationClass = new OperationClass();
        if (chkSelectalll.Checked == true)
        {

            R = 0;
            

            string TableName = objOperationClass.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Convert.ToInt32(Session["FolderID"])));
            DataTable dtfiledetails = objOperationClass.GetTable4Command(
            string.Format(@"select b.FileiD,approvalstatus,b.folderid from tblfile a inner join {0} b on a.fileid=b.fileid where b.folderid='{1}' order by column0", TableName, Convert.ToInt32(Session["FolderID"])));
            foreach (DataRow row in dtfiledetails.Rows)
            {

                string strapprovalstatus = row["approvalstatus"].ToString();
                string strFileId = row["FileiD"].ToString();
                DataTable dtAttachmentIds = objOperationClass.GetTable4Command(string.Format(@"select attachmentid  from tblattachment where fileid = {0}", Convert.ToInt32(strFileId.ToString())));
                string fileids = strFileId.ToString();

                fileids = fileids + ",";
                if (dtAttachmentIds != null && dtAttachmentIds.Rows.Count > 0)
                {
                    for (int i = 0; i < dtAttachmentIds.Rows.Count; i++)
                    {
                        if (i == dtAttachmentIds.Rows.Count - 1)
                        {
                            fileids = fileids + dtAttachmentIds.Rows[i]["attachmentid"].ToString();
                        }
                        else
                        {
                            fileids = fileids + dtAttachmentIds.Rows[i]["attachmentid"].ToString() + ",";
                        }
                    }
                }
                else
                {
                    fileids = fileids.Substring(0, fileids.LastIndexOf(","));
                }

                if (Convert.ToString(row["approvalstatus"].ToString()) != "2" && Convert.ToString(row["approvalstatus"].ToString()) != "5")
                {
                    int value2 = Convert.ToInt16(operation.Insert4Command(string.Format(@"Update tblfile set ApprovalStatus=3 where fileid in ({0})", fileids)));
                }

            }

         
         
            string strUserIds = "select FirstName+' '+LastName 'UserName' from tbluseraccesscontrol a inner join tblworkgroupmaster b "
    + " on a.groupid=b.groupid inner join tbluserdetail c on a.userid=c.userid "
    + " and groupname<>'admin' where folderid='" + Convert.ToInt32(Session["FolderID"].ToString()) + "' and  accesssymbol<>'n'";
            DataTable dtUserIds = objOperationClass.GetTable4Command(strUserIds);

            for (int i = 0; i < dtUserIds.Rows.Count; i++)
            {
                if (chkSelectalll.Checked == true)
                {

                    R = 0;
                    

                    string TableName1 = objOperationClass.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Convert.ToInt32(Session["FolderID"])));
                    DataTable dtfiledetails1 = objOperationClass.GetTable4Command(
                    string.Format(@"select b.FileiD,approvalstatus,column1,b.folderid from tblfile a inner join {0} b on a.fileid=b.fileid where b.folderid='{1}' order by column0", TableName1, Convert.ToInt32(Session["FolderID"])));
                    foreach (DataRow row in dtfiledetails1.Rows)
                    {

                        string strapprovalstatus = row["approvalstatus"].ToString();
                        string strFileId = row["FileiD"].ToString();
                        string strDescription = row["column1"].ToString();
                        DataTable dtAttachmentIds = objOperationClass.GetTable4Command(string.Format(@"select attachmentid  from tblattachment where fileid = {0}", Convert.ToInt32(strFileId.ToString())));
                        string fileids = strFileId.ToString();

                        string ParentFolderName = operation.ExecuteScalar4Command(string.Format(@"select FolderName  from tblFolder where folderId in (select ParentFolderId from tblFolder where folderId={0})", Convert.ToInt32(Session["FolderID"])));
                        string CommitteFname = strDescription;
                        string DeliveryStatus1 = "Sent and delivered to" + dtUserIds.Rows[i]["UserName"].ToString();
                        DateTime sfds = DateTime.Now;
                        string sdasd = sfds.ToString("yyyy-MM-dd HH:mm:ss");
                        string value1 = operation.ExecuteScalar4Command(string.Format(@"insert into tblfileactions (FileName,ActionName,UserID,ActionDate,Status,FolderId) values ('{0}','{1}',{2},'{3}','{4}',{5})", CommitteFname, DeliveryStatus1, Session["UserId"], sdasd, 1, Session["FolderId"].ToString()));



                    }
                   
                }
              
                DirectorDataBinding();
                chkSelectalll.Checked = false;
                ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('Agenda Published.')", true);
            }

        }
        else
        {
            R = 0;
          
            foreach (GridViewRow row in gvData.Rows)
            {
                CheckBox chkSelect = (CheckBox)row.FindControl("chkSelect");
                Label lblApprovalStatus = (Label)row.FindControl("lblApprovalStatus");
                if (chkSelect.Checked)
                {
                    Label FileId = (Label)row.FindControl("lblFileId");
                    DataTable dtAttachmentIds = objOperationClass.GetTable4Command(string.Format(@"select attachmentid  from tblattachment where fileid = {0}", Convert.ToInt32(FileId.Text.ToString())));
                    string fileids = FileId.Text.ToString();

                    fileids = fileids + ",";
                    if (dtAttachmentIds != null && dtAttachmentIds.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtAttachmentIds.Rows.Count; i++)
                        {
                            if (i == dtAttachmentIds.Rows.Count - 1)
                            {
                                fileids = fileids + dtAttachmentIds.Rows[i]["attachmentid"].ToString();
                            }
                            else
                            {
                                fileids = fileids + dtAttachmentIds.Rows[i]["attachmentid"].ToString() + ",";
                            }
                        }
                    }
                    else
                    {
                        fileids = fileids.Substring(0, fileids.LastIndexOf(","));
                    }
                    //int value2 = Convert.ToInt16(operation.Insert4Command(string.Format(@"Update tblfile set ApprovalStatus=1 where fileid={0}", Convert.ToInt32(FileId.Text.ToString()))));
                    if (lblApprovalStatus.Text != "2" && lblApprovalStatus.Text != "5")
                    {
                        int value2 = Convert.ToInt16(operation.Insert4Command(string.Format(@"Update tblfile set ApprovalStatus=3 where fileid in ({0})", fileids)));
                    }
                }
            }
           
            string strUserIds = "select FirstName+' '+LastName 'UserName' from tbluseraccesscontrol a inner join tblworkgroupmaster b "
    + " on a.groupid=b.groupid inner join tbluserdetail c on a.userid=c.userid "
    + " and groupname<>'admin' where folderid='" + Convert.ToInt32(Session["FolderID"].ToString()) + "' and  accesssymbol<>'n'";
            DataTable dtUserIds = objOperationClass.GetTable4Command(strUserIds);

            for (int i = 0; i < dtUserIds.Rows.Count; i++)
            {
                foreach (GridViewRow row in gvData.Rows)
                {
                    CheckBox chkSelect = (CheckBox)row.FindControl("chkSelect");
                    Label lblApprovalStatus = (Label)row.FindControl("lblApprovalStatus");
                    if (chkSelect.Checked)
                    {
                        Label FileId = (Label)row.FindControl("lblFileId");
                        LinkButton lnkParticular = (LinkButton)row.FindControl("lnkOpen1");

                        string ParentFolderName = operation.ExecuteScalar4Command(string.Format(@"select FolderName  from tblFolder where folderId in (select ParentFolderId from tblFolder where folderId={0})", Convert.ToInt32(Session["FolderID"])));
                        string CommitteFname = lnkParticular.Text;
                        string DeliveryStatus1 = "Sent and delivered to" + dtUserIds.Rows[i]["UserName"].ToString();
                        DateTime sfds = DateTime.Now;
                        string sdasd = sfds.ToString("yyyy-MM-dd HH:mm:ss");
                        string value1 = operation.ExecuteScalar4Command(string.Format(@"insert into tblfileactions (FileName,ActionName,UserID,ActionDate,Status,FolderId) values ('{0}','{1}',{2},'{3}','{4}',{5})", CommitteFname, DeliveryStatus1, Session["UserId"], sdasd, 1, Session["FolderId"].ToString()));
                    }
                }
            }
            DirectorDataBinding();
            chkSelectalll.Checked = false;
            ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('Agenda Published.')", true);
        }
    }
            

    protected void btnAddAttendee_Click(object sender, EventArgs e)
    {
        tblPanel.Visible = true;
    }

    protected void btnAddDetail_Click(object sender, EventArgs e)
    {
        OperationClass objOperationClass = new OperationClass();
        if (txtUserName.Text != "" || txtEmailIDs.Text != "" || txtMobileNo.Text != "")
        {
            string strUID = objOperationClass.ExecuteScalar4Command("select max(UID) from tblAddressbook");
            int valll = objOperationClass.Insert4Command(string.Format(@"insert into tblAddressbook(UserID,UserName,EmailID,MobileNo) values('" + (Convert.ToInt32(strUID) + 1) + "','" + txtUserName.Text.Replace("'", "''").ToString().Trim() + "','" + txtEmailIDs.Text.Replace("'", "''").ToString().Trim() + "','" + txtMobileNo.Text.Replace("'", "''").ToString().Trim() + "')"));
            //int val = obj.Insert4Command(string.Format(@"insert into tblAttendeesStatus(UserID,FolderID,UserName,EmailID,Status) values('" + strUID + "','" + ViewState["FolderrrTD"].ToString() + "','" + txtUserName.Text.Replace("'", "''").ToString().Trim() + "','" + txtEmailIDs.Text.Replace("'", "''").ToString().Trim() + "','" + txtEmailIDs.Text.Replace("'", "''").ToString().Trim() + "','Present')"));
            //int strrr2 = objOperationClass.Insert4Command(@"insert into tblAttendance(FolderID,Status,UserID) values('" + ViewState["FolderrrTD"].ToString() + "','Present','" + strUID + "')");
        }
        else
        {

            ScriptManager.RegisterClientScriptBlock(Page, typeof(UpdatePanel), "msg", "Please enter the required details.", true);
        }
        bindGvUserDetail();
        txtUserName.Text = "";
        txtEmailIDs.Text = "";
        txtMobileNo.Text = "";
        tblPanel.Visible = false;
    }

    protected void btnCancelDetail_Click(object sender, EventArgs e)
    {
        tblPanel.Visible = false;
    }

    public void bindGvUserDetail()
    {
        try
        {
            DataTable dt;
            OperationClass objOperationClass = new OperationClass();
            dt = new DataTable();

            //dt = objOperationClass.GetTable4Command(string.Format(@"Select * from tblAttendeesStatus where FolderID='{0}'", ViewState["FolderrrTD"].ToString()));
            dt = objOperationClass.GetTable4Command(string.Format(@"select * from tbladdressbook where userid not in (select userid from tblusermaster) order by username"));
            if (dt != null && dt.Rows.Count > 0)
            {
                tblAddAttendees.Visible = true;
                gvUserDetail.DataSource = dt;
                gvUserDetail.DataBind();
                btnAddAttendee.Visible = true;
            }
            else
            {
                gvUserDetail.DataSource = null;
                btnAddAttendee.Visible = false;
            }
        }
        catch (System.Exception ex)
        {
            Session["SPath"] = Server.MapPath(".");
        }

    }

    public void btnMeeting_Click(object sender, EventArgs e)
    {
        R = 0;
        if (Session["UserName"] != null)
        {
            bool sendmail = false;


            OperationClass operation = new OperationClass();


            //  DataTable dtEmailIDs = operation.GetTable4Command("select EmailID from tbladdressbook where userid not in (select userid from tblusermaster)");
            string emailIds = "";

            string ParentFolderName = operation.ExecuteScalar4Command(string.Format(@"select FolderName  from tblFolder where folderId in (select ParentFolderId from tblFolder where folderId={0})", Convert.ToInt32(Session["FolderID"])));
            string FolderName = operation.ExecuteScalar4Command(string.Format(@"select FolderName from tblFolder where folderId={0}", Convert.ToInt32(Session["FolderID"])));
            string FullFolderName = Session["FolderName"].ToString();
            //int len = FullFolderName.Length - (FullFolderName.IndexOf('\\') + 1) - (FullFolderName.Substring(FullFolderName.LastIndexOf('\\'))).Length;
            //Session["FolderName"].ToString().Substring((Session["FolderName"].ToString().IndexOf('\')+1)));
            foreach (GridViewRow j in gvUserDetail.Rows)
            {
                CheckBox chSelectt = null;
                Label lblApprovee = null;
                chSelectt = (CheckBox)j.FindControl("chkSelect");
                lblApprovee = (Label)j.FindControl("lblEmails");
                if (chSelectt.Checked)
                {
                    emailIds = lblApprovee.Text.ToString().Trim() + ",";

                    emailIds = emailIds.Substring(0, emailIds.LastIndexOf(','));
                    StringBuilder sb = new StringBuilder();
                    HttpContext context = HttpContext.Current;
                    sb.Append("<table>");
                    sb.Append("<tr>");
                    sb.Append("<td align='left'>");
                    sb.Append("<b> Respected Sir/Madam  </b> <br />");

                    //sb.Append("<p>Agenda have been uploaded on Director's Portal by <B>Secretarial Team</B> for " +
                    //  " the <B> " + ParentFolderName + "</B> scheduled on <B>" + FolderName + "</B>");
                    sb.Append("<p>Your proposal for the <B>" + ParentFolderName + "</B> scheduled on <B>" + FolderName + "</B>  is as follows:");


                    sb.Append("</td>");
                    sb.Append("</tr><tr><td style='height:25px;'></td></tr>");


                    sb.Append("<tr>");
                    sb.Append("<td>");
                    sb.Append("<table border=1 >");
                    sb.Append("<tr align='center'>");

                    //sb.Append("<td style='width=15%;' align='center'>");
                    //sb.Append("<b>Agenda No.</b>");
                    //sb.Append("</td>");

                    sb.Append("<td style='width=15%;' align='center'>");
                    sb.Append("<b>Agenda Particulars</b>");
                    sb.Append("</td>");

                    sb.Append("<td style='width=15%;' align='center'>");
                    sb.Append("<b>Time</b>");
                    sb.Append("</td>");
                    sb.Append("</tr>");



                    foreach (GridViewRow i in gvData.Rows)
                    {
                        CheckBox chSelect = null;
                        Label lblApprove = null;
                        chSelect = (CheckBox)i.FindControl("chkSelect");
                        lblApprove = (Label)i.FindControl("lblApprovalStatus");
                        if (chSelect.Checked)
                        {
                            sendmail = true;
                            Label AgendaNo = (Label)i.FindControl("LinkButton1");
                            Label LotNo = (Label)i.FindControl("lnkFileName");
                            LinkButton AgendaParticular = (LinkButton)i.FindControl("lnkOpen1");
                            Label subject = (Label)i.FindControl("lblsubject");
                            Label Gm = (Label)i.FindControl("lbllotno");
                            Label MeetingDate = (Label)i.FindControl("lblmeettingdate");
                            Label lblRemark = (Label)i.FindControl("lblRemark");
                            //LinkButton NoteNo = (LinkButton)i.FindControl("lnkFileName");
                            //Label NoteNo = (Label)i.FindControl("lnkFileName");
                            sb.Append("<tr>");



                            //sb.Append("<td style='width=15%;' align='center'>");
                            //sb.Append(AgendaNo.Text);
                            //sb.Append("</td>");

                            sb.Append("<td style='width=15%;' align='left'>");
                            sb.Append(AgendaParticular.Text);
                            sb.Append("</td>");
                            sb.Append("<td style='width=15%;' align='left'>");
                            sb.Append(lblRemark.Text);
                            sb.Append("</td>");
                        }


                    }
                    sb.Append("</table>");
                    sb.Append("</td>");
                    sb.Append("</tr>");

                    sb.Append("<tr>");
                    sb.Append("<td align='left'>");
                    sb.Append(Environment.NewLine + "<br />");
                    sb.Append("<tr/>");
                    sb.Append("<tr><td><b>Regards," + "</b><br />");
                    //sb.Append("Board Sercretariat,<br/>Wockhardt</td></tr>");
                    //sb.Append("</td></tr></table>");
                    sb.Append("Company Secretary.</td></tr>");
                    sb.Append("</td></tr></table>");
                    //sb.Append("Company Secretary,<br/>DCB Bank</td></tr>");
                    //sb.Append("</td></tr></table>");
                    Boolean DeliveryStatus = sendMail(emailIds, sb.ToString(), "RE: Mail Format for Informing Functional Heads");
                    if (DeliveryStatus == true)
                    {
                        string ParentFolderNames = operation.ExecuteScalar4Command(string.Format(@"select FolderName  from tblFolder where folderId in (select ParentFolderId from tblFolder where folderId={0})", Convert.ToInt32(Session["FolderID"])));
                        string CommitteFname = ParentFolderNames + "//" + Session["DMeetingdate"].ToString();
                        string delivery = "Agenda Email sent to " + emailIds + "with Subject RE: Mail Format for Informing Functional Heads";
                        DateTime sfds = DateTime.Now;
                        string sdasd = sfds.ToString("yyyy-MM-dd HH:mm:ss");
                        string value1 = operation.ExecuteScalar4Command(string.Format(@"insert into tblfileactions (FileName,ActionName,UserID,ActionDate,Status,FolderId) values ('{0}','{1}',{2},'{3}','{4}',{5})", CommitteFname, delivery, Session["UserId"], sdasd, 1, Session["FolderId"].ToString()));
                    }
                    else
                    {

                        //string ParentFolderName = operation.ExecuteScalar4Command(string.Format(@"select FolderName  from tblFolder where folderId in (select ParentFolderId from tblFolder where folderId={0})", Convert.ToInt32(Session["FolderID"])));
                        //string CommitteFname = ParentFolderName + "//" + Session["DMeetingdate"].ToString();
                        //DateTime sfds = DateTime.Now;
                        //string sdasd = sfds.ToString("yyyy-MM-dd HH:mm:ss");
                        //string value1 = operation.ExecuteScalar4Command(string.Format(@"insert into tblfileactions (FileName,ActionName,UserID,ActionDate,Status,FolderId) values ('{0}','{1}',{2},'{3}','{4}',{5})", CommitteFname, delivery, Session["UserId"], sdasd, 1, Session["FolderId"].ToString()));
                    }
                    ScriptManager.RegisterStartupScript(Page, this.GetType(), "msg", "alert('Email has been sent.')", true);


                }
            }



        }
        if (Session["UserName"] != null)
        {
            try
            {
                OperationClass objOperationClass = new OperationClass();
                string parentfoldername = objOperationClass.ExecuteScalar4Command(string.Format(@"select foldername from tblfolder where folderid=(select parentfolderid from tblfolder where folderid={0})", Convert.ToInt16(Session["FolderID"])));
                string foldername = objOperationClass.ExecuteScalar4Command(string.Format(@"select foldername from tblfolder where folderid={0}", Convert.ToInt16(Session["FolderID"])));
                string TableName = objOperationClass.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Convert.ToInt32(Session["FolderID"])));
                DataTable dtmin = objOperationClass.GetTable4Command(string.Format(@"select min(column0) 'Min',max(column0) 'Max' from {0}", TableName));
                string min = "", max = "";
                if (dtmin != null && dtmin.Rows.Count > 0)
                {
                    min = dtmin.Rows[0]["Min"].ToString();
                    max = dtmin.Rows[0]["Max"].ToString();
                }
                string msgText = "";
                string userName = ConfigurationSettings.AppSettings["UserName"].ToString();
                string password = ConfigurationSettings.AppSettings["Password"].ToString();
                string From = ConfigurationSettings.AppSettings["From"].ToString();
                //DataTable dt = objOperationClass.GetTable4Command(string.Format(@"Select distinct b.MobileNo,b.Id from tblUserAccesscontrol a inner join tbluserdetail b on a.userid=b.userid and a.FOLDERID IN(" + Convert.ToString(Session["FolderID"]) + ") and AccessSymbol not in('N') and b.MobileNo is not null"));
                DataTable dt = objOperationClass.GetTable4Command(string.Format(@"select MobileNo from tbladdressbook where userid not in (select userid from tblusermaster)"));

                //msgText = "Dear Sir/Madam, " + parentfoldername + " Agenda No. " + min + " to " + max + " updated for the Meeting dated " + foldername + " Regards , Board Secretariat.";
                msgText = "Dear Sir/Madam, " + parentfoldername + " Agenda are updated for the Meeting dated " + foldername + " Regards , Board Secretariat.";
                int length = msgText.Length;
                //string[] arrno = { "9867087012", "9867087012", "9867087012" };
                //DataTable dt = objOperationClass.GetTable4Command(string.Format(@"Select b.ContactNo from tblUserAccesscontrol a inner join tbluserdetail b on a.userid=b.userid and a.FOLDERID IN(" + Convert.ToString(Session["FolderID"]) + ") and AccessSymbol not in('N') and b.GroupId in (select GroupID from dbo.tblWorkGroupMaster where lower(groupname) !='admin')"));
                //string[] arrno = { ConfigurationSettings.AppSettings["Number1"].ToString(),ConfigurationSettings.AppSettings["Number2"].ToString()};
                //for (int i = 0; i < arrno.Length; i++)
                //{
                string no1 = "";

                //for (int i = 0; i < dt.Rows.Count; i++)
                //{
                foreach (GridViewRow i in gvUserDetail.Rows)
                {
                    CheckBox chSelect = null;
                    Label lblApprove = null;
                    chSelect = (CheckBox)i.FindControl("chkSelect");
                    lblApprove = (Label)i.FindControl("lblmobileno");
                    if (chSelect.Checked)
                    {
                        no1 = lblApprove.Text.ToString().Trim();
                        // no1 = dt.Rows[i]["MobileNo"].ToString();

                        string URL = "http://api.myvaluefirst.com/psms/servlet/psms.Eservice2?data=<?xml%20version=\"1.0\"%20encoding=\"ISO-8859-1\"?><!DOCTYPE%20MESSAGE%20SYSTEM%20\"http://127.0.0.1:80/psms/dtd/messagev12.dtd\"%20><MESSAGE%20VER=\"1.2\"><USER%20USERNAME=\"" + userName + "\"%20PASSWORD=\"" + password + "\"/><SMS%20UDH=\"0\"%20CODING=\"1\"%20TEXT=\"" + msgText + "\"%20PROPERTY=\"0\"%20ID=\"1\"><ADDRESS%20FROM=\"" + From + "\"%20TO=\"" + no1 + "\"%20SEQ=\"1\"%20TAG=\"some%20clientside%20random%20data\"%20/></SMS></MESSAGE>&action=send";

                        WebRequest myWebRequest = WebRequest.Create(URL);
                        WebResponse myWebResponse = myWebRequest.GetResponse();
                        Stream ReceiveStream = myWebResponse.GetResponseStream();
                        Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                        StreamReader readStream = new StreamReader(ReceiveStream, encode);
                        string strResponse = readStream.ReadToEnd();

                        string DeliveryStatus1 = "SMS sent to " + no1 + " with Message " + msgText;
                        string ParentFolderNamess = operation.ExecuteScalar4Command(string.Format(@"select FolderName  from tblFolder where folderId in (select ParentFolderId from tblFolder where folderId={0})", Convert.ToInt32(Session["FolderID"])));
                        string CommitteFname = ParentFolderNamess + "//" + Session["DMeetingdate"].ToString();
                        DateTime sfds = DateTime.Now;
                        string sdasd = sfds.ToString("yyyy-MM-dd HH:mm:ss");
                        string value1 = operation.ExecuteScalar4Command(string.Format(@"insert into tblfileactions (FileName,ActionName,UserID,ActionDate,Status,FolderId) values ('{0}','{1}',{2},'{3}','{4}',{5})", CommitteFname, DeliveryStatus1, Session["UserId"], sdasd, 1, Session["FolderId"].ToString()));
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Message Sent Successfully')", true);



                        //SMSServiceClient.SMSServiceService objUserDetails = new SMSServiceClient.SMSServiceService();
                        //int output = objUserDetails.sendSMS(ConfigurationSettings.AppSettings["ApplicationID"].ToString(), no1, msgText);
                        //if (output == 0)
                        //{
                        //    string DeliveryStatus1 = "SMS sent to " + no1 + "with Message" + msgText;
                        //    string ParentFolderNamess = operation.ExecuteScalar4Command(string.Format(@"select FolderName  from tblFolder where folderId in (select ParentFolderId from tblFolder where folderId={0})", Convert.ToInt32(Session["FolderID"])));
                        //    string CommitteFname = ParentFolderNamess + "//" + Session["DMeetingdate"].ToString();
                        //    DateTime sfds = DateTime.Now;
                        //    string sdasd = sfds.ToString("yyyy-MM-dd HH:mm:ss");
                        //    string value1 = operation.ExecuteScalar4Command(string.Format(@"insert into tblfileactions (FileName,ActionName,UserID,ActionDate,Status,FolderId) values ('{0}','{1}',{2},'{3}','{4}',{5})", CommitteFname, DeliveryStatus1, Session["UserId"], sdasd, 1, Session["FolderId"].ToString()));
                        //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Message Sent Successfully')", true);
                        //}
                        //else
                        //{

                        //    string DeliveryStatus1 = "SMS not sent to " + no1 + " Error:" + output;
                        //    string ParentFolderNamess = operation.ExecuteScalar4Command(string.Format(@"select FolderName  from tblFolder where folderId in (select ParentFolderId from tblFolder where folderId={0})", Convert.ToInt32(Session["FolderID"])));
                        //    string CommitteFname = ParentFolderNamess + "//" + Session["DMeetingdate"].ToString();
                        //    DateTime sfds = DateTime.Now;
                        //    string sdasd = sfds.ToString("yyyy-MM-dd HH:mm:ss");
                        //    string value1 = operation.ExecuteScalar4Command(string.Format(@"insert into tblfileactions (FileName,ActionName,UserID,ActionDate,Status,FolderId) values ('{0}','{1}',{2},'{3}','{4}',{5})", CommitteFname, DeliveryStatus1, Session["UserId"], sdasd, 1, Session["FolderId"].ToString()));
                        //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Message not sent')", true);
                        //}
                    }


                }
                // ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Message Sent Successfully')", true);
                pnlSms.Visible = false;
            }
            catch (Exception ex)
            {
                string DeliveryStatus1 = "SMS not sent Error:" + ex.Message;
                string ParentFolderNamess = operation.ExecuteScalar4Command(string.Format(@"select FolderName  from tblFolder where folderId in (select ParentFolderId from tblFolder where folderId={0})", Convert.ToInt32(Session["FolderID"])));
                string CommitteFname = ParentFolderNamess + "//" + Session["DMeetingdate"].ToString();
                DateTime sfds = DateTime.Now;
                string sdasd = sfds.ToString("yyyy-MM-dd HH:mm:ss");
                string value1 = operation.ExecuteScalar4Command(string.Format(@"insert into tblfileactions (FileName,ActionName,UserID,ActionDate,Status,FolderId) values ('{0}','{1}',{2},'{3}','{4}',{5})", CommitteFname, DeliveryStatus1, Session["UserId"], sdasd, 1, Session["FolderId"].ToString()));
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Message not sent')", true);
            }
        }
        lblMeetingDate.Visible = false;

        btnMeeting.Visible = false;
        btnCancel.Visible = false;
        tblAddAttendees.Visible = false;
        DirectorDataBinding();
    }

    #region [btnCancel_Click]
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        lblMeetingDate.Visible = false;

        btnMeeting.Visible = false;
        btnCancel.Visible = false;
        tblAddAttendees.Visible = false;
        DirectorDataBinding();

    }
    #endregion

    public void btnFirstSeperator_Click(object sender, EventArgs e)
    {
        if (chkSelectalll.Checked == false)
        {

            foreach (GridViewRow j in gvData.Rows)
            {
                CheckBox chk = (CheckBox)j.FindControl("chkSelect");
                Label lblItemNo = (Label)j.FindControl("LinkButton1");
                Label lblFileId = (Label)j.FindControl("lblFileId");

                if (chk.Checked == true)
                {
                    ViewState["FirstSelectedd"] = lblItemNo.Text;
                    ViewState["FileIds"] = lblFileId.Text;
                    W = 0;
                    pncomment.Visible = true;

                }
            }


        }



    }

    public void btnLastSeperator_Click(object sender, EventArgs e)
    {
        if (chkSelectalll.Checked == false)
        {

            foreach (GridViewRow j in gvData.Rows)
            {
                CheckBox chk = (CheckBox)j.FindControl("chkSelect");
                Label lblItemNo = (Label)j.FindControl("LinkButton1");
                Label lblFileId = (Label)j.FindControl("lblFileId");

                if (chk.Checked == true)
                {
                    ViewState["LastSelectedd"] = lblItemNo.Text;
                    ViewState["FileIds"] = lblFileId.Text;
                    W = 0;
                    pncomment.Visible = true;
                }
            }

        }
    }

    public void btnAddNew_Click(object sender, EventArgs e)
    {
        if (Request.QueryString["Value"] != null && Request.QueryString["Value"] != "meeting")
        {
            if (Request.QueryString["Value"].ToString().ToLower() == "my briefcase")
            {
                Response.Redirect("../UploadFile/FileUpload_New.aspx?ATR=NotExists&Value=my briefcase");
            }
            else
            {
                Response.Redirect("../UploadFile/FileUpload_New.aspx?ATR=NotExists&Value=company info");
            }
        }
        else
        {
            Response.Redirect("../UploadFile/FileUpload_New.aspx?ATR=NotExists");
        }
    }

    #region [SetBussinessentityies]
    private DataTable SetBussinessentityies(string strFileName, string strFilePath, byte[] imgFileImage, string strFileSize,
                                     string strInitialPath, string strIpAddress, string strFPath, string SaveImage, Int32 PageCount,
                                     string ReplaceAnswer, string TableName
                                     )
    {
        try
        {
            objFileUploadController = new FileUploadController();

            Hashtable hstFileDetail = new Hashtable();

            hstFileDetail.Add("FileID", Convert.ToInt64(ViewState["FileIds"]));

            hstFileDetail.Add("FileName", strFileName);
            hstFileDetail.Add("FilePath", strFilePath);
            hstFileDetail.Add("FileImage", imgFileImage);
            hstFileDetail.Add("FolderId", Convert.ToInt32(Session["FolderID"]));
            hstFileDetail.Add("ImportedBy", Convert.ToInt32(Session["UserID"]));
            hstFileDetail.Add("FileSize", strFileSize);
            hstFileDetail.Add("InitialPath", strInitialPath);
            hstFileDetail.Add("IPAddress", strIpAddress);
            if (ConfigurationManager.AppSettings["Application"].ToString().Trim().ToLower() != "hindalco")
            {
                hstFileDetail.Add("Keyword", "");
                hstFileDetail.Add("Subject", "");
                hstFileDetail.Add("MeetingDate", "");
                hstFileDetail.Add("Theme", "");

                hstFileDetail.Add("FieldNames", Convert.ToString(ViewState["FieldNames"]));
                hstFileDetail.Add("TableName", Convert.ToString(TableName));
                hstFileDetail.Add("FieldValues", Convert.ToString(ViewState["FieldValues"]));
            }
            else
            {
                //main index
                //hstFileDetail.Add("Keyword", ddlMainIndex.SelectedItem.ToString().Trim() == "Please select" ? "" : ddlMainIndex.SelectedItem.ToString().Trim());
                ////sub index 
                //hstFileDetail.Add("Subject", ddlSubIndex.SelectedItem.ToString().Trim() == "Please select" ? "" : ddlSubIndex.SelectedItem.ToString().Trim());
                ////location
                //hstFileDetail.Add("MeetingDate", ddlLocation.SelectedItem.ToString().Trim() == "Please select" ? "" : ddlLocation.SelectedItem.ToString().Trim());
                ////discription
                //hstFileDetail.Add("Theme", txtDescription.Text.Trim());

                hstFileDetail.Add("Keyword", "");
                //sub index 
                hstFileDetail.Add("Subject", "");
                //location
                hstFileDetail.Add("MeetingDate", "");
                //discription
                hstFileDetail.Add("Theme", "");

                hstFileDetail.Add("FieldNames", Convert.ToString(ViewState["FieldNames"]));
                hstFileDetail.Add("TableName", Convert.ToString(ViewState["TableName"]));
                hstFileDetail.Add("FieldValues", Convert.ToString(ViewState["FieldValues"]));
            }

            hstFileDetail.Add("strFilePath", strFPath);
            hstFileDetail.Add("StoragePlace", SaveImage);
            hstFileDetail.Add("PageCount", PageCount);
            hstFileDetail.Add("Company", ConfigurationManager.AppSettings["Application"].ToString().Trim().ToLower());
            string strMessage = "";
            DataTable DtImageUploaded = null;
            if (ReplaceAnswer == "yes")
            {
                strMessage = "alert('File replaced successfully.');";
                DtImageUploaded = objFileUploadController.UpdateFileDetail(hstFileDetail);
            }
            else
            {
                strMessage = "alert('File uploaded successfully.');";
                DtImageUploaded = objFileUploadController.InserttblFile(hstFileDetail);
            }
            return DtImageUploaded;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            objFileUploadController = null;
        }
    }
    #endregion

    protected void btnOCR_Click(object sender, EventArgs e)
    {
        string locn = ConfigurationManager.AppSettings["EXELOCs"];

        try
        {
            OperationClass objOperationClass = new OperationClass();
            if (chkSelectalll.Checked == true)
            {

                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('please check on inividual item and do OCR.')", true);
                //string TableName = objOperationClass.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Convert.ToInt32(Session["FolderID"])));
                //DataTable dtfiledetails = objOperationClass.GetTable4Command(
                //string.Format(@"select b.FileiD from tblfile a inner join {0} b on a.fileid=b.fileid where b.folderid='{1}' order by column0", TableName, Convert.ToInt32(Session["FolderID"])));
                //foreach (DataRow row in dtfiledetails.Rows)
                //{
                //    Process myProcess = new Process();
                //    myProcess.StartInfo.UseShellExecute = false;
                //    myProcess.StartInfo.FileName = locn + "OfficeToPdfConsole.exe";
                //    myProcess.StartInfo.Arguments = row["FileiD"].ToString();
                //    myProcess.StartInfo.CreateNoWindow = true;
                //    myProcess.Start();
                //}
                chkSelectalll.Checked = false;
            }
            else
            {


                foreach (GridViewRow row in gvData.Rows)
                {
                    CheckBox chkSelect = (CheckBox)row.FindControl("chkSelect");
                    Label FileId = (Label)row.FindControl("lblFileId");
                    if (chkSelect.Checked)
                    {
                        string Count = objOperationClass.ExecuteScalar4Command(string.Format(@"select count(*) from tblcontentsearch where fileid='{0}'", Convert.ToInt32(FileId.Text.ToString())));
                        if (Convert.ToInt32(Count) > 0)
                        {

                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('OCR has been already done for this file')", true);
                        }
                        else
                        {

                            Process myProcess = new Process();
                            myProcess.StartInfo.UseShellExecute = false;
                            myProcess.StartInfo.FileName = locn + "OfficeToPdfConsole.exe";
                            myProcess.StartInfo.Arguments = FileId.Text.ToString();
                            myProcess.StartInfo.CreateNoWindow = true;
                            myProcess.Start();
                        }


                    }
                }




            }

            DirectorDataBinding();


        }
        catch (Exception ex)
        {
            Response.Write(ex.Message);
        }

    }

    protected void btndownloadselected_Click(object sender, EventArgs e)
    {
        try
        {
            //List<String> FileNameID = new List<String>();
            int count = 0;
            string FileNameID = "";
            foreach (GridViewRow row in gvData.Rows)
            {
                CheckBox chkSelect = (CheckBox)row.FindControl("chkSelect");
                Label lblFileId = (Label)row.FindControl("lblFileId");
                if (chkSelect.Checked)
                {
                    string Value = operation.ExecuteScalar4Command(string.Format(@"select Value from tblConfig where Keys='AgendaAccessRestricted'"));
                    if (Value == "yes")
                    {
                        count++;
                        FileNameID += lblFileId.Text + ",";
                        string AId = operation.ExecuteScalar4Command(string.Format(@"select AId from tblAgendalevelAccessControl where UserID={0} and FolderId={1} and FileID={2}", Convert.ToInt32(Session["UserID"]), Convert.ToInt32(Session["FolderID"]), Convert.ToInt32(lblFileId.Text)));

                        string TableName = operation.ExecuteScalar4Command(string.Format(@"select tablename from tblFolderIndexMaster where folder_id={0}", Convert.ToInt32(Session["FolderID"])));
                        string Column0 = operation.ExecuteScalar4Command(string.Format(@"select Column0 from " + TableName + " where FolderId={0} and FileId={1}", Convert.ToInt32(Session["FolderID"]), Convert.ToInt32(lblFileId.Text)));
                        string sstttttt = Column0.Substring(0, 2);
                        string FileId = operation.ExecuteScalar4Command(string.Format(@"select FileId from " + TableName + " where Column0='{0}'", sstttttt));
                        string FileIdATT = operation.ExecuteScalar4Command(string.Format(@"select FileId from tblAgendalevelAccessControl where FileID={0}", FileId));
                        if (FileIdATT != "" || AId != "")
                        {
                            string AccessRestrict = "";
                            FileGenerateWithAccessDenied(AccessRestrict, lblFileId.Text);
                            FileDownLoadwithAccessDenied(AccessRestrict, lblFileId.Text);
                            AccessDeniePath = Convert.ToString(Server.MapPath("~//Report//PdfReports//AccessDenie//" + Session["UserName"].ToString() + "//" + "AccessDenie" + lblFileId.Text + "pdf.pdf"));
                        }

                        //string AId = operation.ExecuteScalar4Command(string.Format(@"select Column0 from table873 where FolderId=1018 and FileId=5291", Convert.ToInt32(Session["UserID"]), Convert.ToInt32(Session["FolderID"]), Convert.ToInt32(lblFileId.Text)));

                        // if()
                        // {

                        // }
                        // else
                        // {
                        // }
                    }
                    else if (Value == "no")
                    {
                        string AId = operation.ExecuteScalar4Command(string.Format(@"select AId from tblAgendalevelAccessControl where UserID={0} and FolderId={1} and FileID={2}", Convert.ToInt32(Session["UserID"]), Convert.ToInt32(Session["FolderID"]), Convert.ToInt32(lblFileId.Text)));
                        if (AId == "")
                        {
                            count++;
                            FileNameID += lblFileId.Text + ",";
                        }

                    }


                }
            }
            if (count > 0)
            {
                FileNameID = FileNameID.Substring(0, FileNameID.LastIndexOf(','));
                //}
                //else if (count == 0)
                //{
                //    ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "myScr", strMessage, true);
                //}


                Session["FileID"] = FileNameID;
                //Session["FileName"] = FileNameID[1].ToString();
                //Session["Itemno"] = FileNameID[2].ToString();

                // create objects of class
                objCommonBAL = new CommonBAL();

                //visible lblmessage
                lblMessage.Visible = false;
                lblMessage.Text = "";

                //visible panel first
                Panel4.Visible = false;

                //set folder for save decrypt file.
                string ImageSavingFilePath = Convert.ToString(HttpContext.Current.Server.MapPath("~\\Repository\\Decrypt\\" + HttpContext.Current.Session["UserName"].ToString()));

                //set folder path exported file.
                string ImagesavedFilePath = Convert.ToString(HttpContext.Current.Server.MapPath("~\\ExportFile\\" + Convert.ToString(HttpContext.Current.Session["UserName"]).Trim()));

                //Set zip file name and path
                string ZipFilePath = Convert.ToString(Server.MapPath("~\\ExportFile\\".Trim() + Session["UserName"].ToString().Trim() + "\\" + string.Format("ExportedFile{0:MMM-dd-yyyy_hh-mm-ss}", System.DateTime.Now) + ".zip"));

                //set directory path for delete tepory file.
                string ZipDirectoryPath = Convert.ToString(Server.MapPath("~\\ExportFile\\".Trim() + Session["UserName"].ToString().Trim()));

                //Download();
                // Call methode for export file from database or folder
                string strMessage = objCommonBAL.ExportFileOnButtonClick1(Convert.ToString(Session["FileID"]), ImageSavingFilePath, ImagesavedFilePath, ZipFilePath, ZipDirectoryPath, "");
                if (strMessage.Contains("alert"))
                {
                    ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "myScr", strMessage, true);
                }
                else
                {
                    PdfFileDownLoad(strMessage);
                    //FileDownLoad(strMessage);
                    //FileDownLoadwithWaterMark(strMessage);

                }
            }
            else if (count == 0)
            {
                //ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "Please select the file for download.", strMessage, true);
                ScriptManager.RegisterStartupScript(Page, typeof(UpdatePanel), "mgs", "alert('Please select the file for download.')", true);
            }

        }
        catch (Exception exx)
        {
        }
    }
}



