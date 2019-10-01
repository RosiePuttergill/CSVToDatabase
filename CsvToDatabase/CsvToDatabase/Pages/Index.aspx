<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="CsvToDatabase.Pages.Index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>CSV to Database</title>
    <style type="text/css">
        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h3>
                Upload a CSV file matching the following schema to upload data to the database for display:
            </h3>
            <p>
                Name: NVARCHAR(50), NON NULL<br />
                DOB: Date, NON NULL<br />
                CreditLimit: DECIMAL, 2DP, NON NULL<br />
                YearsAtCurrentAddress: INT32, NON NULL<br />
                PolicyId: GUID, NON NULL
            </p>
            <hr />
            <asp:FileUpload ID="FileUpload" runat="server" />
            <asp:Button ID="UploadButton" Text="Upload" runat="server"  OnClick="UploadButton_Click" />
            <br />
            <br />
            <asp:Label ID="Info" ForeColor="Black" runat="server"></asp:Label>
            <br />
            <br />
            <asp:Label ID="ErrorTitle" ForeColor="Black" runat="server"></asp:Label>
            <br />
            <br />
            <asp:Label ID="ErrorMessage" ForeColor="Black" runat="server"></asp:Label>
            <br />
            <br />
            <asp:Button ID="ClearButton" Text="Clear Table" runat="server" OnClick="ClearButton_Click"/>
            <asp:GridView ID="DataGrid" runat="server" AutoGenerateColumns="false" CellPadding="2">
                <Columns>
                    <asp:BoundField DataField="Id" HeaderText="ID" />
                    <asp:BoundField DataField="Name" HeaderText="Name"/>
                    <asp:BoundField DataFormatString="{0:dd MMM yyyy}" DataField="DOB" HeaderText="Date of Birth"  />
                    <asp:BoundField DataFormatString="{0:C2}" DataField="CreditLimit" HeaderText="Credit Limit" />
                    <asp:BoundField DataField="YearsAtCurrentAddress" HeaderText="Years at Current Address" />
                    <asp:BoundField DataField="PolicyId" HeaderText="Policy ID" />
                </Columns>
            </asp:GridView>
        </div>
    </form>
</body>
</html>
