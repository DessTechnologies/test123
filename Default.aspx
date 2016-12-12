<%@ Page Language="C#" MasterPageFile="~/MasterPage/MasterPage.master" AutoEventWireup="true"
    CodeFile="Default.aspx.cs" Inherits="default_copy" Title="Dess Digital Meetings" %>

<%-- Given by Mukesh on 06/07/2015
--%>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMaster" runat="Server">

    <script type="text/javascript" src="../JavaScript/jquery-1.7.1.min.js"></script>

    <script type="text/javascript" src="../JavaScript/jquery.alertsNew.js"></script>

    <link rel="stylesheet" type="text/css" href="../App_Themes/jquery.alertsNew.css" />

    <script type="text/javascript" src="../JavaScript/jquery-1.7.1.min.js"></script>

    <script type="text/javascript" src="../JavaScript/jquery.alertsNew.js"></script>

    <script type="text/javascript" src="../JavaScript/jquery.touchSwipe.min.js"></script>

    <script type="text/javascript" src="../JavaScript/jquery.min.js"></script>

    <link rel="stylesheet" type="text/css" href="../App_Themes/jquery.alertsNew.css" />
    <link href="../Content/cleditor/jquery.cleditor.css" rel="stylesheet" type="text/css" />
    <link href="../Content/Site.css" rel="stylesheet" type="text/css" />

    <script src="../Scripts/jquery-1.6.3.js" type="text/javascript"></script>

    <script src="../Scripts/jquery.cleditor.js" type="text/javascript"></script>

     <script language="javascript" type="text/javascript">
 
    $(document).ready(function () {

        var options = {
            width: 400,
            height: 200,
            controls: "bold italic underline strikethrough subscript superscript | font size " +
                    "style | color highlight removeformat | bullets numbering | outdent " +
                    "indent | alignleft center alignright justify | undo redo | " +
                    "rule link image unlink | cut copy paste pastetext | print source"
        };

        var editor = $("#editor").cleditor(options)[0];

        $("#btnClear").click(function (e) {
            e.preventDefault();
            editor.focus();
            editor.clear();
        });

        $("#btnAddImage").click(function () {
            editor.execCommand("insertimage", "http://images.free-extras.com/pics/s/smile-1620.JPG", null, null)
            editor.focus();
        });
    
     });
    
    
    
    
    
     function ShowPopup(rrr) {
$("#editor").html(rrr);
}
    
       function  Cancels()
   {
 
     $.ajax({
 
type: "POST",
 
url: "Default.aspx/InsertMeetingData",
 
 data: "{MeetingData:'"+$("#editor").val()+"'}",
 
contentType: "application/json; charset=utf-8",
 async:false,
  jsonp:true,
dataType: "json",
 
success: function (response) {

  $("#editor").val(response.d);
  

},
 
failure: function (response) {
 
 $("#editor").val(response.d);
 
}
 
});
  
   
   }

    
    </script>

    <div>
        <asp:UpdatePanel ID="upFileView" runat="server">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="btnDelete" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="btnYes" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="btnNO" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="btnSubmit" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="pnlCancelMove" EventName="Click" />
                <asp:PostBackTrigger ControlID="btnExport" />
                <asp:PostBackTrigger ControlID="gvData" />
                <asp:PostBackTrigger ControlID="GridView1" />
                <asp:PostBackTrigger ControlID="GridView2" />
                <asp:PostBackTrigger ControlID="gvParent" />
                <asp:PostBackTrigger ControlID="btnnotebook" />
                <asp:PostBackTrigger ControlID="btndownloadselected" />
                <asp:AsyncPostBackTrigger ControlID="btnActivate" EventName="Click" />
            </Triggers>
            <ContentTemplate>
                <table style="width: 100%; display: none;">
                    <tr id="tblMeetingInformation" runat="server">
                        <td class="HeaderStyle" align="left">
                            <font style="font-family: Calibri; font-size: 15px; font-weight: bold;" class="WebsiteColor">
                                Meetings :</font>
                            <asp:Label ID="lblhdcommitee" runat="server" Font-Names="Calibri" Font-Size="12pt"
                                Font-Bold="true" class="WebsiteColor"></asp:Label>
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                            <font>
                                Meeting Date :</font>
                            <asp:Label ID="lblhdmeetingdate" runat="server" Font-Names="Calibri" Font-Size="15px"
                                Font-Bold="true"  class="WebsiteColor"></asp:Label>
                        </td>
                    </tr>
                </table>
                <table style="width: 100%;">
                   <tr id="tblinattendance" runat="server">
                                    <td  class="HeaderStyle">
                                            <asp:Label ID="lblinattendancelbl"  runat="server" Visible="false"
                                                Text="In Attendance :"  Font-Bold="True" Font-Names="Calibri"
                                                Font-Size="15px" class="WebsiteColor"></asp:Label>
                                                  <asp:Label ID="lblinattendancetext"  runat="server" 
                                                 Font-Bold="True" Font-Names="Calibri" Visible="false"
                                                Font-Size="15px" class="WebsiteColor"></asp:Label>                                            
                                    </td>
                                     
                                    </tr>
                </table>
                <table style="width: 100%;">
                   <tr id="tblinvitees" runat="server">
                                    <td  class="HeaderStyle">
                                            <asp:Label ID="lblinviteeslbl"  runat="server" Visible="false"
                                                Text="Invitees :"  Font-Bold="True" Font-Names="Calibri"
                                                Font-Size="15px" class="WebsiteColor"></asp:Label>
                                                  <asp:Label ID="lblinviteestext"  runat="server" 
                                                 Font-Bold="True" Font-Names="Calibri" Visible="false"
                                                Font-Size="15px" class="WebsiteColor"></asp:Label>                                            
                                    </td>
                                     
                                    </tr>
                </table>
                <table id="tblUploadAgenda" class="HeaderStyle" align="right" runat="server" style="width: 100%;
                    display: none;">
                    <tr>
                        <td style="width: 40%;">
                        </td>
                        <td>
                            <a style="font-family: Calibri; font-size: 12pt;" href="../UploadFile/FileUpload_New.aspx?ATR=NotExists">
                                Note to Agenda</a> &nbsp;&nbsp;&nbsp; <a style="font-family: Calibri; font-size: 12pt;"
                                    href="../UploadFile/FileUpload_Bulk.aspx?ATR=NotExists">Bulk Upload</a>
                            &nbsp;&nbsp;&nbsp; <a style="font-family: Calibri; font-size: 12pt;" href="../AccessControl/AgendaLevelNoteBook.aspx">
                                Agenda Level Note Book</a>
                        </td>
                    </tr>
                </table>
                <table width="100%">
                    <table cellpadding="1" cellspacing="1" width="100%">
                        <tr>
                            <td align="center" style="width: 100%;">
                                <asp:GridView ID="gvParent" runat="server" Width="100%" AllowPaging="True" CellPadding="2"
                                    CellSpacing="2" CssClass="GridView" AllowSorting="True" PageSize="20" AutoGenerateColumns="False"
                                    EmptyDataText="Agenda Will Follow" EmptyDataRowStyle-ForeColor="Red" OnDataBound="gvParent_DataBound"
                                    OnRowCancelingEdit="gvParent_RowCancelingEdit" OnRowCommand="gvParent_RowCommand"
                                    OnRowUpdating="gvParent_RowUpdating" OnRowEditing="gvParent_RowEditing" OnPageIndexChanging="gvParent_PageIndexChanging">
                                    <HeaderStyle CssClass="HeaderStyle"></HeaderStyle>
                                    <AlternatingRowStyle CssClass="AlternatingRowStyle" />
                                    <RowStyle CssClass="RowStyle" />
                                    <EmptyDataRowStyle ForeColor="Red" />
                                    <Columns>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Left">
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chkSelectee" runat="server"></asp:CheckBox>
                                                <asp:Label ID="lblFileId" runat="server" Text='<%#Eval("FileID")%>' Visible="false"></asp:Label>
                                                <asp:Label ID="lblId" runat="server" Text='<%#Eval("ID")%>' Visible="false"></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:CheckBox ID="chkSelectee" runat="server"></asp:CheckBox>
                                                <asp:Label ID="lblFileId" runat="server" Text='<%#Eval("FileID")%>' Visible="false"></asp:Label>
                                                <asp:Label ID="lblId" runat="server" Text='<%#Eval("ID")%>' Visible="false"></asp:Label>
                                            </EditItemTemplate>
                                            <HeaderStyle HorizontalAlign="Center" />
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="Item No">
                                            <ItemTemplate>
                                                <asp:Label ID="lblItemNo" runat="server" Text='<%#Eval("itemno")%>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtItemNo" runat="server" Text='<%#Eval("itemno")%>'></asp:TextBox>
                                                <%--<asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
                                            ControlToValidate="txtItemNo" ErrorMessage="Enter only Numbers" 
                                            ValidationExpression="0*[1-9]\d*"></asp:RegularExpressionValidator>--%>
                                                <%-- <asp:Label ID="LinkButton1" runat="server" Text='<%#Eval("itemno")%>'></asp:Label>--%>
                                            </EditItemTemplate>
                                            <HeaderStyle HorizontalAlign="Center" />
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="Particulars">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkOpen1" runat="server" CommandArgument='<%# Eval("FileId")+","+Eval("FileName")%>'
                                                    Text='<%#Eval("Particulars")%>' CommandName="View">View</asp:LinkButton>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtParticular" runat="server" Text='<%#Eval("Particulars")%>' TextMode="MultiLine"></asp:TextBox>
                                            </EditItemTemplate>
                                            <HeaderStyle HorizontalAlign="Center" />
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="Purpose">
                                            <ItemTemplate>
                                                <asp:Label ID="lblsubject" runat="server" Text='<%#Eval("Purpose")%>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:DropDownList ID="ddlPurpose" runat="server" DataSourceID="sqlPurpose" DataTextField="PurposeName">
                                                </asp:DropDownList>
                                                <asp:SqlDataSource ID="sqlPurpose" runat="server" ConnectionString="<%$ ConnectionStrings:CorporationBankConnectionString %>"
                                                    SelectCommand="SELECT [PurposeName] FROM [tblPurposeMaster] ORDER BY [PurposeId]">
                                                </asp:SqlDataSource>
                                            </EditItemTemplate>
                                            <HeaderStyle HorizontalAlign="Center" />
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="G M">
                                            <ItemTemplate>
                                                <asp:Label ID="lbllotno" runat="server" Text='<%#Eval("GM")%>' Visible="false"></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:DropDownList ID="ddlGM" runat="server" Width="150px" DataSourceID="SqlGM1" Visible="false"
                                                    DataTextField="ShortName" DataValueField="UserId">
                                                </asp:DropDownList>
                                                <asp:SqlDataSource ID="SqlGM1" runat="server" ConnectionString="<%$ ConnectionStrings:CorporationBankConnectionString %>"
                                                    SelectCommand="SELECT [ShortName], [UserId] FROM [tblUserDetail] WHERE ([GroupId] = @GroupId)">
                                                    <SelectParameters>
                                                        <asp:Parameter DefaultValue="16" Name="GroupId" Type="Int64" />
                                                    </SelectParameters>
                                                </asp:SqlDataSource>
                                            </EditItemTemplate>
                                            <HeaderStyle HorizontalAlign="Center" />
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:CheckBoxField Visible="False" />
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Left">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkView" runat="server" CommandArgument='<%# Eval("FileId")+","+Eval("FileName")%>'
                                                    CommandName="View">View</asp:LinkButton>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:LinkButton ID="lnkOpen" runat="server" CommandArgument='<%# Eval("FileId")+","+Eval("FileName")%>'
                                                    CommandName="View">View</asp:LinkButton>
                                            </EditItemTemplate>
                                            <HeaderStyle HorizontalAlign="Center" />
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Left">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkDownload" runat="server" CommandArgument='<%# Eval("FileId")+","+Eval("FileName")%>'
                                                    CommandName="DownLoad">Download</asp:LinkButton>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:LinkButton ID="lnkDownload" runat="server" CommandArgument='<%# Eval("FileId")+","+Eval("FileName")%>'
                                                    CommandName="DownLoad">Download</asp:LinkButton>
                                            </EditItemTemplate>
                                            <HeaderStyle HorizontalAlign="Center" />
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Left">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkEdit" runat="server" OnClick="lnkEdit_Click" CommandArgument='<%# Eval("FileId")+","+Eval("FileName")%>'
                                                    CommandName="Edit">Edit</asp:LinkButton>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:LinkButton ID="UpdateButton" runat="server" CommandName="Update" Text="Update" />
                                                <br />
                                                <asp:LinkButton ID="CancelButton" runat="server" CommandName="Cancel" CausesValidation="false"
                                                    Text="Cancel" />
                                            </EditItemTemplate>
                                            <HeaderStyle HorizontalAlign="Center" />
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </td>
                        </tr>
                        <tr style="overflow: scroll">
                            <td align="center" style="width: 100%; overflow: scroll">
                                <asp:GridView ID="gvData" runat="server" Width="100%" AllowPaging="True" CellPadding="2"
                                    CellSpacing="2" CssClass="GridView" AllowSorting="True" AutoGenerateColumns="False"
                                    PageSize="20" EmptyDataText="Agenda Will Follow" 
                                    OnDataBound="gvData_DataBound" OnRowCancelingEdit="gvData_RowCancelingEdit" OnPageIndexChanging="gvData_PageIndexChanging"
                                    OnRowCommand="gvData_RowCommand" OnRowDataBound="gvData_RowDataBound" OnRowUpdating="gvData_RowUpdating"
                                    OnRowEditing="gvData_RowEditing">
                                    <HeaderStyle CssClass="HeaderStyle"></HeaderStyle>
                                    <AlternatingRowStyle CssClass="AlternatingRowStyle" />
                                    <%--     <PagerSettings FirstPageText="First" LastPageText="Last"
                                             Mode="NextPrevious"  />--%>
                                    <RowStyle CssClass="RowStyle" />
                                    <EmptyDataRowStyle CssClass="WebsiteColor"  />
                                    <Columns>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Left">
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chkSelect" runat="server" OnCheckedChanged="chkSelect_CheckedChanged"
                                                    AutoPostBack="true"></asp:CheckBox>
                                                <asp:Label ID="lblFileId" runat="server" Text='<%#Eval("FileID")%>' Visible="false"></asp:Label>
                                                <asp:Label ID="lblApprovalStatus" runat="server" Text='<%#Eval("ApprovalStatus")%>'
                                                    Visible="false"></asp:Label>
                                                <asp:Label ID="lblWithdrawComments" runat="server" Text='<%#Eval("withdrawcomments")%>'
                                                    Visible="false"></asp:Label>
                                                <asp:Label ID="lblFileName" runat="server" Text='<%#Eval("FileName")%>' Visible="false"></asp:Label>
                                                <asp:Label ID="lblFolderId" runat="server" Text='<%#Eval("FolderID")%>' Visible="false"></asp:Label>
                                                <asp:Label ID="lblllItemNo" runat="server" Text='<%#Eval("ItemNo")%>' Visible="false"></asp:Label>
                                                <asp:Label ID="lblstatus" runat="server" Text='<%#Eval("statuss")%>' Visible="false"></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:CheckBox ID="chkSelect" runat="server"></asp:CheckBox>
                                                <asp:Label ID="lblFileId" runat="server" Text='<%#Eval("FileID")%>' Visible="false"></asp:Label>
                                                <asp:Label ID="lblApprovalStatus" runat="server" Text='<%#Eval("ApprovalStatus")%>'
                                                    Visible="false"></asp:Label>
                                                <asp:Label ID="lblWithdrawComments" runat="server" Text='<%#Eval("withdrawcomments")%>'
                                                    Visible="false"></asp:Label>
                                                <asp:Label ID="lblFileName" runat="server" Text='<%#Eval("FileName")%>' Visible="false"></asp:Label>
                                                <asp:Label ID="lblFolderId" runat="server" Text='<%#Eval("FolderID")%>' Visible="false"></asp:Label>
                                                <asp:Label ID="lblllItemNo" runat="server" Text='<%#Eval("ItemNo")%>' Visible="false"></asp:Label>
                                                <asp:Label ID="lblstatus" runat="server" Text='<%#Eval("statuss")%>' Visible="false"></asp:Label>
                                            </EditItemTemplate>
                                            <HeaderStyle HorizontalAlign="Center" />
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="Sr No.">
                                            <ItemTemplate>
                                                <asp:Label ID="LinkButton1" Style="font-size: 16px;" runat="server" Text='<%#Eval("itemno")%>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtItemNo555" Style="font-size: 16px;" Enabled="false" runat="server"
                                                    Width="40px" Text='<%#Eval("itemno")%>'></asp:TextBox>
                                            </EditItemTemplate>
                                            <HeaderStyle HorizontalAlign="Center" />
                                            <ItemStyle HorizontalAlign="Center" Width="7%" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="Sl.No.">
                                            <ItemTemplate>
                                                <asp:Label ID="lblItemNos" Style="font-size: 16px; text-align: left;" runat="server"
                                                    Text='<%#Eval("itemnos")%>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtItemNos" Style="font-size: 16px;" runat="server" Width="40px"
                                                    Text='<%#Eval("itemnos")%>'></asp:TextBox>
                                            </EditItemTemplate>
                                            <HeaderStyle HorizontalAlign="Center" />
                                            <ItemStyle HorizontalAlign="left" Width="7%" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="Description">
                                            <ItemTemplate>
                                                <asp:HiddenField ID="hditem" runat="server" Value='<%#Eval("itemnoo")%>' />
                                                <asp:LinkButton ID="lnkOpen1" Style="font-size: 16px; font-family: Cambria;" runat="server"
                                                    CommandArgument='<%# Eval("FileId")+","+Eval("FileName")+","+Eval("itemno")+","+Eval("PageCount")%>'
                                                    Text='<%#Eval("Particulars")%>' CommandName="View">View</asp:LinkButton>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:HiddenField ID="hditem" runat="server" Value='<%#Eval("itemnoo")%>' />
                                                <asp:TextBox ID="txtParticular" Style="font-size: 16px;" Width="250px" runat="server"
                                                    Text='<%#Eval("Particulars")%>' TextMode="MultiLine"></asp:TextBox>
                                            </EditItemTemplate>
                                            <HeaderStyle HorizontalAlign="Center" />
                                            <ItemStyle HorizontalAlign="Left" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Left" Visible="false" HeaderText="G M">
                                            <ItemTemplate>
                                                <asp:Label ID="lbllotno" Style="font-size: 14px;" runat="server" Visible="false"
                                                    Text='<%#Eval("GM")%>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:Label ID="txtGM" Style="font-size: 14px;" runat="server" Visible="false" Text='<%#Eval("GM")%>'></asp:Label>
                                            </EditItemTemplate>
                                            <HeaderStyle HorizontalAlign="Center" />
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="Page No">
                                            <ItemTemplate>
                                                <asp:Label ID="lblPageNo" runat="server" Style="font-size: 16px; font-family: Cambria;"
                                                    Text='<%#Eval("GM")%>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:Label ID="lblPageNo" runat="server" Style="font-size: 16px;" Text='<%#Eval("GM")%>'></asp:Label>
                                            </EditItemTemplate>
                                            <HeaderStyle HorizontalAlign="Center" />
                                            <ItemStyle HorizontalAlign="Center" Width="11%" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="Time(Hrs)">
                                            <ItemTemplate>
                                                <asp:Label ID="lblRemark" runat="server" Style="font-size: 14px; font-family: Cambria;"
                                                    Text='<%#Eval("Remark")%>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtRemark" runat="server" Width="150px" TextMode="MultiLine" Text='<%#Eval("Remark")%>'></asp:TextBox>
                                            </EditItemTemplate>
                                            <HeaderStyle HorizontalAlign="Center" />
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:CheckBoxField Visible="False" />
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="Comments">
                                            <ItemTemplate>
                                                <asp:Label ID="lblComment" runat="server" Visible="false" Text='<%#Eval("Comment")%>'></asp:Label>
                                                <asp:Label ID="lblComment1" runat="server" Visible="false" Text='<%#Eval("Comment1")%>'></asp:Label>
                                                <asp:Label ID="lblComment2" runat="server" Visible="false" Text='<%#Eval("Comment2")%>'></asp:Label>
                                                <asp:Label ID="lblComment3" runat="server" Visible="false" Text='<%#Eval("Comment3")%>'></asp:Label>
                                                <asp:Label ID="lblComment4" runat="server" Visible="false" Text='<%#Eval("Comment4")%>'></asp:Label>
                                                <asp:Label ID="lblComment5" runat="server" Visible="false" Text='<%#Eval("Comment5")%>'></asp:Label>
                                                <%--             <asp:Label ID="lblsubject" Style="font-size: 14px;" runat="server" Text='<%#Eval("Purpose")%>'></asp:Label>--%>
                                                <asp:ListView ID="GridViewnew2" runat="server" OnItemDataBound="GridViewnew2_ItemDataBound"
                                                    OnItemCommand="GridViewnew2_ItemCommand">
                                                    <LayoutTemplate>
                                                        <table>
                                                            <tr>
                                                                <td runat="server" id="itemPlaceholder">
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </LayoutTemplate>
                                                    <ItemTemplate>
                                                        <td>
                                                            <asp:LinkButton ID="LinkButton1" Style="font-size: 16px;" CommandName="deletecomment"
                                                                runat="server" Text='<%# Eval("Name") %>' CommandArgument='<%# Eval("Name") %>'></asp:LinkButton>
                                                        </td>
                                                    </ItemTemplate>
                                                </asp:ListView>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:Label ID="lblComment" runat="server" Visible="false" Text='<%#Eval("Comment")%>'></asp:Label>
                                                <asp:Label ID="lblComment1" runat="server" Visible="false" Text='<%#Eval("Comment1")%>'></asp:Label>
                                                <asp:Label ID="lblComment2" runat="server" Visible="false" Text='<%#Eval("Comment2")%>'></asp:Label>
                                                <asp:Label ID="lblComment3" runat="server" Visible="false" Text='<%#Eval("Comment3")%>'></asp:Label>
                                                <asp:Label ID="lblComment4" runat="server" Visible="false" Text='<%#Eval("Comment4")%>'></asp:Label>
                                                <asp:Label ID="lblComment5" runat="server" Visible="false" Text='<%#Eval("Comment5")%>'></asp:Label>
                                                <%--             <asp:Label ID="lblsubject" Style="font-size: 14px;" runat="server" Text='<%#Eval("Purpose")%>'></asp:Label>--%>
                                                <asp:ListView ID="GridViewnew2" runat="server" OnItemDataBound="GridViewnew2_ItemDataBound"
                                                    OnItemCommand="GridViewnew2_ItemCommand">
                                                    <LayoutTemplate>
                                                        <table>
                                                            <tr>
                                                                <td runat="server" id="itemPlaceholder">
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </LayoutTemplate>
                                                    <ItemTemplate>
                                                        <td>
                                                            <asp:LinkButton ID="LinkButton1" Style="font-size: 16px;" CommandName="deletecomment"
                                                                runat="server" Text='<%# Eval("Name") %>' CommandArgument='<%# Eval("Name") %>'></asp:LinkButton>
                                                        </td>
                                                    </ItemTemplate>
                                                </asp:ListView>
                                                <%--<asp:Label ID="lblsubject" Style="font-size: 14px;" runat="server" Text='<%#Eval("Purpose")%>'></asp:Label>--%>
                                            </EditItemTemplate>
                                            <HeaderStyle HorizontalAlign="Center" />
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="View">
                                            <ItemTemplate>
                                                <asp:Label ID="lblviewstaus" runat="server" Visible="false" Text='<%#Eval("status")%>'></asp:Label>
                                                <asp:LinkButton ID="lnkOpen" Style="font-size: 14px; font-family: Cambria;" runat="server"
                                                    CommandArgument='<%# Eval("FileId")+","+Eval("FileName")+","+Eval("itemno")+","+Eval("PageCount")+","+Eval("statuss")%>'
                                                    CommandName="BoardView">View</asp:LinkButton>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:Label ID="lblviewstaus" runat="server" Visible="false" Text='<%#Eval("status")%>'></asp:Label>
                                                <asp:LinkButton ID="lnkOpen" Style="font-size: 14px;" runat="server" CommandArgument='<%# Eval("FileId")+","+Eval("FileName")+","+Eval("itemno")+","+Eval("PageCount")+","+Eval("statuss")%>'
                                                    CommandName="BoardView">View</asp:LinkButton>
                                            </EditItemTemplate>
                                            <HeaderStyle HorizontalAlign="Center" />
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="Download">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkDownload" runat="server" CommandArgument='<%# Eval("FileId")+","+Eval("FileName")+","+Eval("itemno")%>'
                                                    CommandName="DownLoad">Download</asp:LinkButton>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:LinkButton ID="lnkDownload" runat="server" CommandArgument='<%# Eval("FileId")+","+Eval("FileName")+","+Eval("itemno")%>'
                                                    CommandName="DownLoad">Download</asp:LinkButton>
                                            </EditItemTemplate>
                                            <HeaderStyle HorizontalAlign="Center" />
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="Authorize">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="LnkApprove" runat="server" CommandArgument='<%# Eval("FileId")+","+Eval("FileName")+","+Eval("itemno")%>'
                                                    CommandName="Approval">Authorize</asp:LinkButton>
                                                <asp:LinkButton ID="LnkReject" runat="server" CommandArgument='<%# Eval("FileId")+","+Eval("FileName")%>'
                                                    CommandName="Approval">| Reject</asp:LinkButton>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:LinkButton ID="LnkApprove" runat="server" CommandArgument='<%# Eval("FileId")+","+Eval("FileName")+","+Eval("itemno")%>'
                                                    CommandName="Approval">Authorize</asp:LinkButton>
                                                      <asp:LinkButton ID="LnkReject" runat="server" CommandArgument='<%# Eval("FileId")+","+Eval("FileName")%>'
                                                    CommandName="Approval">| Reject</asp:LinkButton>
                                            </EditItemTemplate>
                                            <HeaderStyle HorizontalAlign="Center" />
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="Comments">
                                            <ItemTemplate>
                                                <asp:Label ID="lblwithdrowcomment" runat="server" Style="font-size: 14px; font-family: Cambria;"
                                                    Text='<%#Eval("withdrawcomments")%>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtwithdrowcomment" runat="server" Width="100px" TextMode="MultiLine"
                                                    Text='<%#Eval("withdrawcomments")%>'></asp:TextBox>
                                            </EditItemTemplate>
                                            <HeaderStyle HorizontalAlign="Center" />
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="Edit">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkEdit" runat="server" OnClick="lnkEdit_Click" CommandArgument='<%# Eval("FileId")+","+Eval("FileName")%>'
                                                    CommandName="Edit">Edit</asp:LinkButton>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:LinkButton ID="UpdateButton" runat="server" CommandName="Update" Text="Update" />
                                                <br />
                                                <asp:LinkButton ID="CancelButton" runat="server" CommandName="Cancel" CausesValidation="false"
                                                    Text="Cancel" />
                                            </EditItemTemplate>
                                            <HeaderStyle HorizontalAlign="Center" />
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Left">
                                            <ItemTemplate>
                                                <asp:ImageButton ID="ImgReplaceNote" ToolTip="Replace Agenda" runat="Server" Width="15px" ImageUrl="~/img/Note.jpg"
                                                    CommandName="ReplaceNote" CommandArgument='<%# Eval("FileId")+","+Eval("FileName")+","+Eval("itemno")+","+Eval("PageCount")+","+Eval("statuss")%>' />
                                            </ItemTemplate>
                                            <HeaderStyle HorizontalAlign="Center" />
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Left">
                                            <ItemTemplate>
                                                <asp:ImageButton ID="Imgatach" ToolTip="Upload Multiple Attachment" runat="Server" Width="15px" ImageUrl="~/img/atach.png"
                                                    CommandName="Attachment" CommandArgument='<%# Eval("FileId")+","+Eval("FileName")%>' />
                                            </ItemTemplate>
                                            <HeaderStyle HorizontalAlign="Center" />
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Left">
                                            <ItemTemplate>
                                                <asp:ImageButton ID="imgbtnaccessRes" ToolTip="Agenda Level Access Control" runat="Server" Width="15px" ImageUrl="~/Images/AccessRestricted.png"
                                                    CommandName="AccessRestrict" CommandArgument='<%# Eval("FileId")+","+Eval("FileName")+","+Eval("FolderID")%>' />
                                            </ItemTemplate>
                                            <HeaderStyle HorizontalAlign="Center" />
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <%-- <asp:TemplateField HeaderStyle-HorizontalAlign="Left">
                                  <ItemTemplate>
                                      <asp:Button ID="btnStartMOM" runat="server" CommandName="MOM" 
                                          OnClick="btnStartMOM_Click" Text="Start MOM" />
                                  </ItemTemplate>
                                  <EditItemTemplate>
                                  <asp:Button ID="btnStartMOM" runat="server" CommandName="MOM" 
                                          OnClick="btnStartMOM_Click" Text="Start MOM" />
                                  
                                  </EditItemTemplate>
                                  <HeaderStyle HorizontalAlign="Center" />
                                  <ItemStyle HorizontalAlign="Center" />
                              </asp:TemplateField>--%>
                                    </Columns>
                                    <PagerStyle CssClass="cssPager" />
                                </asp:GridView>
                                <%--  <asp:Panel ID="pnlGridView" runat="server" ScrollBars="Horizontal"  Width="70%" Height="150px"> 
                               </asp:Panel> --%>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <%-- <asp:CheckBox runat="server" ID="chkSelectEmailId" AutoPostBack="true" OnCheckedChanged="chkSelectEmailId_CheckedChanged" />
                                <asp:Label ID="lblChkEmailId" runat="server" Text="Check All"></asp:Label>--%>
                            </td>
                        </tr>
                        <tr>
                            <td align="center">
                                <asp:Label runat="server" ID="lblblankmessage" Font-Bold="true" Text=" * Please choose the meeting date to view/update * "
                                    Visible="false"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td style="padding-left: 5px;">
                                <asp:CheckBox ID="chkSelectalll" Text="Check All" runat="server" Visible="true" AutoPostBack="true"
                                    OnCheckedChanged="chkSelectalll_OnCheckedChanged" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                        </tr>
                    </table>
                    <tr align="center">
                        <td>
                            <%-- <asp:Button ID="btnDownLoad" runat="server" Text="Mail &amp; SMS" CssClass="Buttons"/>--%>
                            <asp:Button ID="btnExport" runat="server" Text="Mail" CssClass="Buttons" OnClick="btnExport_Click"
                                Visible="false" />
                            <asp:Button ID="btnSMS" runat="server" Text="SMS" CssClass="Buttons" OnClick="btnSMS_Click"
                                Visible="false" />
                            <asp:Button ID="btnAuthorizeAll" runat="server" Width="20%" Text="Authorize Selected"
                                CssClass="Buttons" OnClick="btnAuthorizeAll_Click" Visible="false" />
                            <asp:Button ID="btnSubmitSelected" runat="server" Width="20%" Text="Submit Selected"
                                CssClass="Buttons" OnClick="btnSubmitSelected_Click" Visible="false" />
                            <asp:Button ID="btnDelete" runat="server" Width="20%" Text="Delete Selected" ToolTip="Delete Selected Agenda"
                                CssClass="Buttons" OnClick="btnDelete_Click" Visible="false" />
                            <asp:Button ID="btnComplete" runat="server" Width="28%" Text="Proceedings of the Meeting"
                                CssClass="Buttons" OnClick="btnComplete_Click" Visible="False" />
                            <%--  <asp:Button ID="btnCancel" runat="server" Width="20%" ToolTip="Cancel Meeting" Text="Meeting Cancel" CssClass="Buttons" OnClick="btnCancel_Click"
                                Visible="False" />--%>
                            <asp:Button ID="btnStartMOM" runat="server" Text="Repository" Width="12%" CssClass="Buttons"
                                OnClick="btnStartMOM_Click" Visible="False" />
                            <asp:Button ID="btnMoveFile" runat="server" Visible="false" Text="Move File" CssClass="Buttons"
                                OnClick="btnMoveFile_Click" />
                            <asp:Button ID="btnnotebook" runat="server" Width="15%" Text="Note Book" CssClass="Buttons"
                                Visible="false" OnClick="btnnotebook_Click" />
                            <asp:Button ID="btnActivate" runat="server" Width="25%" Text="Activate" CssClass="Buttons"
                                OnClick="btnActivate_Click" Visible="false" />
                            <asp:Button ID="btnAddNew" runat="server" Width="25%" Text="AddNew" CssClass="Buttons"
                                OnClick="btnAddNew_Click" Visible="false" />
                            <%-- <asp:Button ID="btnCopyFiles" runat="server" Text="Copy File" CssClass="Buttons"
                                OnClick="btnCopyFiles_Click" />--%>
                            <%--       <asp:Button ID="btnwaterpwd" runat="server" Height="25px" Text="WaterMark With Pwd"
                                Width="200px" CssClass="Buttons" OnClick="btnwaterpwd_Click" Visible="false" />
                            <asp:Button ID="btnwaterwoutpwd" OnClick="btnwaterwoutpwd_Click" runat="server" Height="25px"
                                Text="WaterMark Without Pwd" Width="200px" CssClass="Buttons" Visible="false" />
                            <asp:Button ID="btnpwdwoutwater" OnClick="btnpwdwoutwater_Click" runat="server" Height="25px"
                                Text="Pwd Without WaterMark" Width="200px" CssClass="Buttons" Visible="false" />
                            <asp:Button ID="btnwithoutPwdwater" OnClick="btnwithoutPwdwater_Click" runat="server"
                                Height="25px" Text="Without Password and without WaterMark" Width="200px" CssClass="Buttons"
                                Visible="false" />--%>
                        </td>
                    </tr>
                    <tr>
                        <asp:Button ID="btnMoveDown" runat="server" Width="20%" Text="Move UpDown" CssClass="Buttons"
                            OnClick="btnMoveDown_Click" Visible="false" />
                        <asp:Button ID="btnPush" runat="server" Width="20%" Text="Push" CssClass="Buttons"
                            OnClick="btnPush_Click" Visible="false" />
                        <asp:Button ID="btnBackFromArchive" runat="server" Width="20%" Text="Activate" CssClass="Buttons"
                            OnClick="btnBackFromArchive_Click" Visible="false" />
                        <asp:Button ID="btnInvitee" runat="server" Width="15%" Text="Invitee" CssClass="Buttons"
                            OnClick="btnInvitee_Click" Visible="false" />
                        <asp:Button ID="btnPublish" runat="server" Width="20%" Text="Publish selected" CssClass="Buttons"
                            OnClick="btnPublish_Click" Visible="false" />
                        <asp:Button ID="btnFirstSeperator" runat="server" Width="25%" Text="Separator Above Selected"
                            CssClass="Buttons" OnClick="btnFirstSeperator_Click" Visible="false" />
                        <asp:Button ID="btnLastSeperator" runat="server" Width="25%" Text="Separator Below selected"
                            CssClass="Buttons" OnClick="btnLastSeperator_Click" Visible="false" />
                            <asp:Button ID="btndownloadselected" runat="server" Width="25%" Text="Download selected"
                            Visible="false" CssClass="Buttons" OnClick="btndownloadselected_Click" />
                             <asp:Button ID="btnnotification" runat="server" Width="25%" Text="Notification"
                            CssClass="Buttons" OnClick="btnnotification_Click"  Visible="false" />
                            <asp:Button ID="btnSwap" runat="server" Width="25%" Text="Swap"
                            CssClass="Buttons" OnClick="btnSwap_Click"  Visible="false"/>
                              <asp:Button ID="btnOCR" runat="server" Width="25%" Text="OCR"
                            CssClass="Buttons" OnClick="btnOCR_Click" Visible="false"/>
                    </tr>
                    <table>
                        <tr>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="txtColor" runat="server" Width="15px" Visible="false" Height="15px"
                                    Style="background-color: LightPink;" ></asp:TextBox>
                                <asp:Label ID="lblRestricted" Text="Access Restricted" runat="server" Visible="false"
                                    Font-Bold="True" CssClass="Label"></asp:Label>
                            </td>
                        </tr>
                        <%--     <tr>
                    <td>
                     <asp:Button ID="btnAgendalevelNotebook" runat="server" Width="200px" Text="Agenda Level Note Book"
                            CssClass="Buttons" OnClick="btnAgendalevelNotebook_Click"/>
                    </td>
                    </tr>--%>
                    </table>
                    <tr>
                        <td align="center">
                            <asp:Label ID="lblMessage" runat="server" Font-Bold="True" CssClass="Label" ForeColor="#FF3300"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:UpdatePanel ID="upMessage" runat="server" UpdateMode="Conditional">
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="btnDelete" EventName="Click" />
                                    <asp:AsyncPostBackTrigger ControlID="btnYes" EventName="Click" />
                                </Triggers>
                                <ContentTemplate>
                                    <asp:Panel ID="Panel4" Width="370px" runat="server" BorderColor="Black" BorderStyle="Ridge"
                                        Visible="false">
                                        <table style="width: 100%" border="Confirm Files Delete" frame="box" bgcolor="lightgrey"
                                            title="Confirm File Delete">
                                            <tr>
                                                <td bgcolor="#3366CC">
                                                    <asp:Label ID="Label1" Text="Confirm File Delete" runat="server" CssClass="Label"
                                                        ForeColor="White" BackColor="#3366CC"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblRplMessage0" runat="server" ForeColor="Black" CssClass="Label"
                                                        Width="350px"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:DropDownList ID="ddlCommitteenew" runat="server" Enabled="false">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right">
                                                    <asp:Button ID="btnYes" runat="server" Height="25px" Text="Yes" Width="60px" CssClass="Buttons"
                                                        OnClick="btnYes_Click" />
                                                    <asp:Button ID="btnNO" runat="server" Height="25px" Text="No" Width="60px" CssClass="Buttons"
                                                        OnClick="btnNO_Click" />
                                                    <%--   btnYes--%>
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <div id="divExport" runat="server">
                                <table>
                                    <tr>
                                        <%#Eval("FileSize") %>
                                    </tr>
                                </table>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:TextBox ID="txtOder" runat="server" Visible="false"></asp:TextBox>
                            <asp:TextBox ID="txtSort" runat="server" Visible="false"></asp:TextBox>
                            <asp:DropDownList ID="ddlIDList" runat="server" Visible="False">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:Label ID="lblNoMeeting" runat="server" Visible="false" Text="No Meeting Scheduled&nbsp;&nbsp;&nbsp;&nbsp;"
                                CssClass="PageHeader" Width="100%"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" align="center">
                            <asp:Panel ID="pnlComplete" Width="370px" runat="server" BorderColor="Black" BorderStyle="Ridge"
                                Visible="false">
                                <table style="width: 100%" border="Meeting Complete" frame="box" bgcolor="lightgrey"
                                    title="Complete the Meeting">
                                    <tr>
                                        <td bgcolor="#3366CC">
                                            <asp:Label ID="Label2" Text="Do you want to Complete the Meeting?" runat="server"
                                                CssClass="Label" ForeColor="White" BackColor="#3366CC"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label6" runat="server" ForeColor="Black" CssClass="Label" Width="350px">Meeting is Completed</asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right">
                                            <asp:Button ID="btnReplace" runat="server" Height="25px" Text="Yes" Width="60px"
                                                CssClass="Buttons" OnClick="pnlComplete_Click" />
                                            <asp:Button ID="Button2" runat="server" Height="25px" Text="No" Width="60px" CssClass="Buttons"
                                                OnClick="btnBoxCancel_Click" />
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:Panel ID="pnlSms" Width="370px" runat="server" BorderColor="Black" BorderStyle="Ridge"
                                Visible="false">
                                <table style="width: 100%" border="Confirm Files Delete" frame="box" bgcolor="lightgrey"
                                    title="Confirm SMS to be Sent">
                                    <tr>
                                        <td bgcolor="#3366CC">
                                            <asp:Label ID="Label3" Text="Confirm SMS to be Sent" runat="server" CssClass="Label"
                                                ForeColor="White" BackColor="#3366CC"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label4" runat="server" ForeColor="Black" CssClass="Label" Width="350px">Are you sure you want to sent message?</asp:Label>
                                        </td>
                                                                                   

                                    </tr>
                                    <tr>
                                        <td>
                                     <asp:TextBox ID="txtsmscomment" TextMode="MultiLine" Width="300px" Height="60px" runat="server"></asp:TextBox>
                                     
                                        </td>
                                       </tr>
                                    <tr>
                                        <td align="right">
                                            <asp:Button ID="btnYesSms" runat="server" Height="25px" Text="Yes" Width="60px" CssClass="Buttons"
                                                OnClick="btnYesSms_Click" />
                                            <asp:Button ID="btnNoSms" runat="server" Height="25px" Text="No" Width="60px" CssClass="Buttons"
                                                OnClick="btnNOSms_Click" />
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" align="center">
                            <asp:Panel ID="pnlCancel" Width="370px" runat="server" BorderColor="Black" BorderStyle="Ridge"
                                Visible="false">
                                <table style="width: 100%" border="Cancel Meeting" frame="box" bgcolor="lightgrey"
                                    title="Cancel the Meeting">
                                    <tr>
                                        <td bgcolor="#3366CC">
                                            <asp:Label ID="Label7" Text="Do you want to Cancel the Meeting?" runat="server" CssClass="Label"
                                                ForeColor="White" BackColor="#3366CC"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label8" runat="server" ForeColor="Black" CssClass="Label" Width="350px">Cancel the Scheduled Meeting?</asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right">
                                            <asp:Button ID="Button3" runat="server" Height="25px" Text="Yes" Width="60px" CssClass="Buttons"
                                                OnClick="pnlCancel_Click" />
                                            <asp:Button ID="Button4" runat="server" Height="25px" Text="No" Width="60px" CssClass="Buttons"
                                                OnClick="btnBoxCancel_Click" />
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" align="center">
                            <asp:Panel ID="pnlMove" Width="500px" runat="server" BorderColor="Black" BorderStyle="Ridge"
                                Visible="false">
                                <table>
                                    <%--    <tr>
                                        <td>
                                            <asp:Label ID="lblcommittee" runat="server" ForeColor="Black" CssClass="Label" Width="150px">Select committee/Meeting Date</asp:Label>
                                        </td>
                                        <td>
                                            <asp:RadioButton ID="rdcommittee" AutoPostBack="true" runat="server" GroupName="committee"
                                                OnCheckedChanged="rdcommittee_CheckedChanged" />&nbsp;committee
                                        </td>
                                        <td>
                                            <asp:RadioButton ID="rdmeeting" AutoPostBack="true" runat="server" GroupName="committee"
                                                OnCheckedChanged="rdmeeting_CheckedChanged" />&nbsp;Meeting Date
                                        </td>
                                    </tr>--%>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblMeetingDate" runat="server" ForeColor="Black" CssClass="Label"
                                                Width="150px">Select Meeting Date</asp:Label>
                                        </td>
                                        <td style="width: 140px;">
                                            <asp:DropDownList ID="ddlCommittee" runat="server" OnSelectedIndexChanged="ddlCommittee_SelectedIndexChanged"
                                                AutoPostBack="true">
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="ddlMeetingDate" runat="server">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Button ID="btnSubmit" runat="server" Height="25px" Text="Submit" Width="60px"
                                                CssClass="Buttons" OnClick="pnlSubmit_Click" />
                                            <asp:Button ID="pnlCancelMove" runat="server" Height="25px" Text="Cancel" Width="60px"
                                                CssClass="Buttons" OnClick="pnlCancelMove_Click" />
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
                <table cellpadding="1" cellspacing="1" width="100%">
                    <tr>
                        <td align="center" style="width: 100%">
                            <asp:Label ID="lblApproval" runat="server" Font-Size="16px" Visible="false" ForeColor="Black"
                                CssClass="Label" Width="350px">Agenda For Approval</asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td align="center" style="width: 100%">
                            <asp:GridView ID="GridView1" runat="server" Width="100%" AllowPaging="True" CellPadding="2"
                                CellSpacing="2" CssClass="GridView" AllowSorting="True" PageSize="20" OnPageIndexChanging="GridView1_PageIndexChanging"
                                AutoGenerateColumns="False" EmptyDataText="No Notes For Approval" OnRowDataBound="GridView1_RowDataBound"
                                OnRowCommand="GridView1_RowCommand" EmptyDataRowStyle-ForeColor="Red">
                                <HeaderStyle CssClass="HeaderStyle"></HeaderStyle>
                                <AlternatingRowStyle CssClass="AlternatingRowStyle" />
                                <RowStyle CssClass="RowStyle" />
                                <EmptyDataRowStyle ForeColor="Red" />
                                <Columns>
                                    <asp:TemplateField HeaderStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chkSelectr" runat="server"></asp:CheckBox>
                                            <asp:Label ID="lblFileId" runat="server" Text='<%#Eval("FileID")%>' Visible="false"></asp:Label>
                                            <asp:Label ID="lblWorkFlowAgenda_Id" runat="server" Text='<%#Eval("WorkFlowAgenda_Id")%>'
                                                Visible="false"></asp:Label>
                                            <asp:Label ID="lblId" runat="server" Text='<%#Eval("ID")%>' Visible="false"></asp:Label>
                                            <asp:HiddenField ID="hdstepsno" runat="server" Value='<%#Eval("stepsno")%>' Visible="false" />
                                            <asp:HiddenField ID="hdStepStatus" runat="server" Value='<%#Eval("StepStatus")%>'
                                                Visible="false" />
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="File Name">
                                        <ItemTemplate>
                                            <asp:Label ID="lblItemNo" runat="server" Text='<%#Eval("filename")%>'></asp:Label>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="Subject">
                                        <ItemTemplate>
                                            <asp:Label ID="lblSubject" runat="server" Text='<%#Eval("Subject")%>'></asp:Label>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="Reason">
                                        <ItemTemplate>
                                            <asp:Label ID="lblReason" runat="server" Text='<%#Eval("Reason")%>'></asp:Label>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="Status">
                                        <ItemTemplate>
                                            <asp:Label ID="lblStatus" runat="server" Text='<%#Eval("Status")%>'></asp:Label>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkView" runat="server" CommandArgument='<%# Eval("FileId")+","+Eval("FileName")%>'
                                                CommandName="View">View</asp:LinkButton>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkDownloadAgenda" runat="server" CommandArgument='<%# Eval("FileId")+","+Eval("FileName")%>'
                                                CommandName="DownLoad">Download</asp:LinkButton>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkApproveAgenda" runat="server" CommandArgument='<%# Eval("ID")+","+Eval("StepStatus")+","+Eval("FileId")+","+Eval("WorkFlowAgenda_Id")%>'
                                                CommandName="Approve">Approve</asp:LinkButton>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkDisApproveAgenda" runat="server" CommandName="DisApprove"
                                                CommandArgument='<%# Eval("ID")+","+Eval("StepStatus")+","+Eval("FileId")+","+Eval("WorkFlowAgenda_Id")%>'>DisApprove</asp:LinkButton>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkReject" runat="server" CommandArgument='<%# Eval("FileId")+","+Eval("FileName")+","+Eval("StepStatus")%>'
                                                CommandName="Reject">Reject</asp:LinkButton>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </td>
                    </tr>
                    <tr>
                        <td align="left" style="width: 100%; height: 15px;">
                            <asp:Button ID="btnAuthorize" runat="server" Width="20%" Text="Authorize All" CssClass="Buttons"
                                OnClick="btnAuthorize_Click" Visible="False" />
                        </td>
                    </tr>
                    <tr>
                        <td align="center" style="width: 100%; height: 15px;">
                        </td>
                    </tr>
                    <tr>
                        <td align="center" style="width: 100%">
                            <asp:Label ID="lblItems" runat="server" Font-Size="16px" Visible="false" ForeColor="Black"
                                CssClass="Label" Width="350px">Agenda Items</asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td align="center" style="width: 100%">
                            <asp:GridView ID="GridView2" runat="server" Width="100%" AllowPaging="True" CellPadding="2"
                                CellSpacing="2" CssClass="GridView" AllowSorting="True" PageSize="10" AutoGenerateColumns="False"
                                EmptyDataText="No Agenda Items" OnPageIndexChanging="GridView2_PageIndexChanging"
                                OnRowDataBound="GridView2_RowDataBound" OnRowCommand="GridView2_RowCommand" EmptyDataRowStyle-ForeColor="Red">
                                <HeaderStyle CssClass="HeaderStyle"></HeaderStyle>
                                <AlternatingRowStyle CssClass="AlternatingRowStyle" />
                                <RowStyle CssClass="RowStyle" />
                                <EmptyDataRowStyle ForeColor="Red" />
                                <Columns>
                                    <asp:TemplateField HeaderStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chkSelected" runat="server"></asp:CheckBox>
                                            <asp:Label ID="lblFileIded" runat="server" Text='<%#Eval("FileID")%>' Visible="false"></asp:Label>
                                            <asp:Label ID="lblIded" runat="server" Text='<%#Eval("ID")%>' Visible="false"></asp:Label>
                                            <asp:HiddenField ID="hdstepsnoed" runat="server" Value='<%#Eval("stepsno")%>' Visible="false" />
                                            <asp:HiddenField ID="hdStepStatused" runat="server" Value='<%#Eval("StepStatus")%>'
                                                Visible="false" />
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="File Name">
                                        <ItemTemplate>
                                            <asp:Label ID="lblItemNoed" runat="server" Text='<%#Eval("filename")%>'></asp:Label>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="Subject">
                                        <ItemTemplate>
                                            <asp:Label ID="lblSubjected" runat="server" Text='<%#Eval("Subject")%>'></asp:Label>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="Committe">
                                        <ItemTemplate>
                                            <asp:DropDownList ID="ddlCommitte" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlCommitte_SelectedIndexChanged">
                                            </asp:DropDownList>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="Meeting">
                                        <ItemTemplate>
                                            <asp:DropDownList ID="ddlMeeting" runat="server">
                                                <asp:ListItem Value="0" Text="Select" />
                                            </asp:DropDownList>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkViewed" runat="server" CommandArgument='<%# Eval("FileId")+","+Eval("FileName")%>'
                                                CommandName="View">View</asp:LinkButton>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkDownloadAgendaed" runat="server" CommandArgument='<%# Eval("FileId")+","+Eval("FileName")%>'
                                                CommandName="DownLoad">Download</asp:LinkButton>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkMove" runat="server" CommandArgument='<%# Eval("FileId")+","+Eval("FileName")%>'
                                                CommandName="Move">Distribute</asp:LinkButton>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </td>
                    </tr>
                    <tr>
                        <td align="left" style="width: 100%; height: 15px;">
                            <asp:Button ID="btnDistributeAll" runat="server" Width="20%" Text="Distribute All"
                                CssClass="Buttons" OnClick="btnDistributeAll_Click" Visible="False" />
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:Panel ID="Panel5" Width="370px" runat="server" BorderColor="Black" BorderStyle="Ridge"
                                Visible="false">
                                <table style="width: 100%" border="Confirm Files Delete" frame="box" bgcolor="lightgrey"
                                    title="Enter The Reason">
                                    <tr>
                                        <td bgcolor="#3366CC">
                                            <asp:Label ID="lblReason" Text="Enter The Reason" runat="server" CssClass="Label"
                                                ForeColor="White" BackColor="#3366CC"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="txtReason" runat="server" TextMode="MultiLine" Width="350px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right">
                                            <asp:Button ID="btnYesnew" runat="server" Height="25px" Text="Yes" Width="60px" CssClass="Buttons"
                                                OnClick="btnYesnew_Click" />
                                            <asp:Button ID="btnNOnew" runat="server" Height="25px" Text="No" Width="60px" CssClass="Buttons"
                                                OnClick="btnNOnew_Click" />
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:Panel ID="Panel7" Width="370px" runat="server" BorderColor="Black" BorderStyle="Ridge"
                                Visible="false">
                                <table style="width: 100%" border="Confirm Files Delete" frame="box" bgcolor="lightgrey"
                                    title="Enter The Reason">
                                    <tr>
                                        <td bgcolor="#3366CC">
                                            <asp:Label ID="Label5" Text="Select Committee" runat="server" CssClass="Label" ForeColor="White"
                                                BackColor="#3366CC"></asp:Label>
                                        </td>
                                        <td bgcolor="#3366CC">
                                            <asp:DropDownList ID="ddlCommitteName" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlCommitteName_SelectedIndexChanged">
                                            </asp:DropDownList>
                                        </td>
                                        <td bgcolor="#3366CC">
                                            <asp:DropDownList ID="ddlMeetingDateNew" runat="server">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="3" align="center">
                                            <asp:Button ID="btnYesnew1" runat="server" Height="25px" Text="Yes" Width="60px"
                                                CssClass="Buttons" OnClick="btnYesnew1_Click" />
                                            <asp:Button ID="btnNOnew1" runat="server" Height="25px" Text="No" Width="60px" CssClass="Buttons"
                                                OnClick="btnNOnew1_Click" />
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:Panel ID="pncomment" Width="370px" runat="server" BorderColor="Black" BorderStyle="Ridge"
                                Visible="false">
                                <table style="width: 100%" border="Confirm Files Delete" frame="box" bgcolor="lightgrey"
                                    title="Enter The Reason">
                                    <tr>
                                        <td bgcolor="#3366CC">
                                            <asp:Label ID="Label9" Text="Comment" runat="server" CssClass="Label" ForeColor="White"
                                                BackColor="#3366CC"></asp:Label>
                                        </td>
                                        <td bgcolor="#3366CC">
                                            <%-- <asp:DropDownList ID="DropDownList1" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlCommitteName_SelectedIndexChanged">
                                            </asp:DropDownList>--%>
                                            <asp:TextBox ID="txtcomments" TextMode="MultiLine" Width="300px" runat="server"></asp:TextBox>
                                        </td>
                                        <%-- <td bgcolor="#3366CC">
                                            <asp:DropDownList ID="DropDownList2" runat="server">
                                            </asp:DropDownList>
                                        </td>--%>
                                    </tr>
                                    <tr>
                                        <td colspan="3" align="center">
                                            <asp:Button ID="btncommentsave" runat="server" Height="25px" Text="Save" Width="60px"
                                                CssClass="Buttons" OnClick="btncommentsave_Click" />
                                            <asp:Button ID="btncommentcancel" runat="server" Height="25px" Text="Cancel" Width="60px"
                                                CssClass="Buttons" OnClick="btncommentcancel_Click" />
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td colspan="2" align="center">
                            <table border="1" width="500px" id="tblAddAttendees" visible="false" runat="server">
                                <tr>
                                    <td id="Td1" runat="server">
                                        <div id="divGvuserDetail" runat="server" style="overflow: auto; width: 520px; height: 150px;">
                                            <asp:GridView Caption="Members" ID="gvUserDetail" CellPadding="1" Width="500px" CellSpacing="1"
                                                runat="server" CssClass="Font" AllowSorting="true" AutoGenerateColumns="false"
                                                EmptyDataText="No Members to add to attendees list!!">
                                                <HeaderStyle BackColor="ActiveBorder" CssClass="Gv"></HeaderStyle>
                                                <AlternatingRowStyle BorderColor="ActiveBorder" BackColor="#EEEEEE" />
                                                <PagerStyle HorizontalAlign="Right" BackColor="ActiveBorder" CssClass="Gv" />
                                                <Columns>
                                                    <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <asp:CheckBox runat="server" ID="chkSelect" />
                                                            <%-- <asp:HiddenField ID="lblId" runat="server" Value=' <%#Eval("ID")%>' />--%>
                                                            <asp:Label ID="lblEmailId" runat="server" Text=' <%#Eval("EmailId")%>' Visible="false"></asp:Label>
                                                            <asp:Label ID="lblUserIDNew" runat="server" Text=' <%#Eval("UserId")%>' Visible="false"></asp:Label>
                                                            <asp:Label ID="lblusernamee" runat="server" Text=' <%#Eval("UserName")%>' Visible="false"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Name">
                                                        <ItemTemplate>
                                                            <%#Eval("UserName")%>
                                                        </ItemTemplate>
                                                        <HeaderStyle HorizontalAlign="Center" />
                                                        <ItemStyle HorizontalAlign="Center" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Email ID's">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblEmails" runat="server" Text=' <%#Eval("EmailId")%>'></asp:Label>
                                                        </ItemTemplate>
                                                        <HeaderStyle HorizontalAlign="Center" />
                                                        <ItemStyle HorizontalAlign="Center" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Mobile no.">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblmobileno" runat="server" Text=' <%#Eval("MobileNo")%>'></asp:Label>
                                                        </ItemTemplate>
                                                        <HeaderStyle HorizontalAlign="Center" />
                                                        <ItemStyle HorizontalAlign="Center" />
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        &nbsp;&nbsp; &nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp;&nbsp; &nbsp;
                                        &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp;&nbsp; &nbsp; &nbsp;&nbsp; &nbsp;&nbsp;&nbsp;
                                        &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp;&nbsp; &nbsp; &nbsp;&nbsp; &nbsp;&nbsp;&nbsp;
                                        &nbsp;
                                        <asp:Button ID="btnAddAttendee" runat="server" Text="Add Invitees" OnClick="btnAddAttendee_Click" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <table id="tblPanel" visible="false" runat="server" style="width: 690px;">
                    <tr>
                        <td>
                            <asp:textbox runat="server" id="txtUserName" onfocus="if (this.value == 'Name') this.value = '';"
                                onblur="if (this.value == '') this.value = 'Name';" value="Name" height="20px"
                                width="180px" xmlns:asp="#unknown">
                            </asp:textbox>&nbsp;
                        </td>
                        <td>
                            <asp:textbox runat="server" id="txtEmailIDs" onfocus="if (this.value == 'Email ID') this.value = '';"
                                onblur="if (this.value == '') this.value = 'Email ID';" value="Email ID" height="20px"
                                width="180px" xmlns:asp="#unknown">
                            </asp:textbox>&nbsp;
                        </td>
                        <td>
                            <asp:textbox runat="server" id="txtMobileNo" onfocus="if (this.value == 'Mobile No') this.value = '';"
                                onblur="if (this.value == '') this.value = 'Mobile No';" value="Mobile No" height="20px"
                                width="180px" xmlns:asp="#unknown">
                            </asp:textbox>&nbsp;
                        </td>
                        <td>
                            <asp:Button ID="btnAddDetail" runat="server" Text="Add" OnClick="btnAddDetail_Click"
                                Width="57px" />
                        </td>
                        <td>
                            <asp:Button ID="btnCancelDetail" runat="server" Text="Cancel" OnClick="btnCancelDetail_Click"
                                Width="57px" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td style="height: 10px;">
                        </td>
                    </tr>
                    <tr>
                        <td align="center" style="width: 520px;">
                            <asp:Button ID="btnMeeting" runat="server" class="Buttons" OnClick="btnMeeting_Click"
                                Text="Meeting Notice" value="Meeting Notice" Visible="false" OnClientClick="Open();"
                                Width="150px" />
                            <%--</td>
                        <td align="left">--%>
                            <asp:Button ID="btnCancel" Width="150px" value="Cancel" Text="Cancel" Visible="false"
                                runat="server" class="Buttons" OnClick="btnCancel_Click" />
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
        <br />
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
            <Triggers>
                <asp:PostBackTrigger ControlID="btncommentsave" />
            </Triggers>
            <ContentTemplate>
                <center>
                    <table width="100%" id="tblGrid" runat="server" visible="true">
                        <tr>
                            <td align="center" style="width: 100%">
                                <asp:GridView ID="gvFielSearch" runat="server" Width="100%" AllowPaging="True" CellPadding="2"
                                    CellSpacing="2" CssClass="GridView" AllowSorting="True" PageSize="5" AutoGenerateColumns="False"
                                    OnRowCommand="gvFielSearch_RowCommand" OnPageIndexChanging="gvFielSearch_PageIndexChanging"
                                    EmptyDataText="Records not found" OnRowDataBound="gvFielSearch_RowDataBound"
                                    EmptyDataRowStyle-ForeColor="Red">
                                    <HeaderStyle CssClass="HeaderStyle"></HeaderStyle>
                                    <AlternatingRowStyle CssClass="AlternatingRowStyle" />
                                    <RowStyle CssClass="RowStyle" />
                                    <Columns>
                                        <asp:TemplateField >
                                            <ItemTemplate>
                                                <input id="chkSelectnew" type="checkbox" runat="server" />
                                            </ItemTemplate>
                                            <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="SL.No.">
                                            <ItemTemplate>
                                                <asp:Label ID="lblFileIdnew" runat="server" Text='<%# Eval("FileId") %>' Visible="false"></asp:Label>
                                                <asp:Label ID="lblDataItemIndex" runat="server" Style="font-size: 16px;" Text=' <%#Container.DataItemIndex+1%>'></asp:Label>
                                            </ItemTemplate>
                                            <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Meeting Date" HeaderStyle-HorizontalAlign="Left">
                                            <ItemTemplate>
                                                <%--  <asp:HiddenField ID="hdDocstatus" runat="server" Value='<%# Eval("DocStatus") %>' />--%>
                                                <asp:Label ID="lblMeeting" Style="font-size: 16px;" runat="server" Text='<%#Eval("Meeting Name")%>'></asp:Label>
                                            </ItemTemplate>
                                            <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <%--    <asp:TemplateField HeaderText="Subject">
                                            <ItemTemplate>
                                                <%#Eval("Noteno")%>
                                            </ItemTemplate>
                                            <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>--%>
                                        <asp:TemplateField HeaderText="View" HeaderStyle-Font-Bold="true">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkView" runat="server" Style="font-size: 16px;" CommandName="Viewmom"
                                                    CommandArgument='<%# Eval("FileId")+","+Eval("FileName")%>'>View</asp:LinkButton>
                                                <headerstyle horizontalalign="Center"></headerstyle>
                                                <itemstyle horizontalalign="Center" />
                                            </ItemTemplate>
                                            <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Delete" HeaderStyle-Font-Bold="true">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkDelete" runat="server" Style="font-size: 16px;" CommandName="Delete"
                                                    CommandArgument='<%# Eval("FileId")+","+Eval("FileName")%>'>Delete</asp:LinkButton>
                                                <headerstyle horizontalalign="Center"></headerstyle>
                                                <itemstyle horizontalalign="Center" />
                                            </ItemTemplate>
                                            <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <%--  <asp:TemplateField HeaderText="Completed" HeaderStyle-Font-Bold="true">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkCompleted" runat="server" Style="font-size: 16px;"  CommandName="Completedmom" CommandArgument='<%# Eval("FileId")+","+Eval("FileName")%>'>Complete</asp:LinkButton>
                                            </ItemTemplate>
                                            <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>--%>
                                    </Columns>
                                </asp:GridView>
                            </td>
                        </tr>
                    </table>
                </center>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
      
    <div id="pnlpopup" runat="server" visible="false">
        <div>
            <asp:TextBox ID="lblsubject" TextMode="MultiLine" Width="400px" placeholder="Subject"
                runat="server"></asp:TextBox>
        </div>
        <div>
            <textarea id="editor" rows="0" cols="0"></textarea>
        </div>
        <div class="normaldiv" style="float: center">
            <asp:Button ID="btnMeetingnotice" Width="150px" value="Cancel" Text="Send Mail"
                runat="server" class="Buttons" OnClick="btnMeetingnotice_Click" OnClientClick="return Cancels();" />
            <asp:Button ID="btnCancelling" Width="150px" value="btnCancelling" Text="Cancel"
                runat="server" class="Buttons" OnClick="btnCancelling_Click" />
        gdfgdfgdfg
        </div>
    </div>
</asp:Content>
