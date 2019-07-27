using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using MicroSoft.EnterpriseLibrary.Data;
public partial class Login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void btnLogin_Click(object sender, EventArgs e)
    {
        string username = this.txtName.Value;
        string userpwd = this.txtPwd.Value;
        try
        {
            string sqlstr = "select * from Member where loginname=@loginname and pwd=@pwd";
            DataSet ds = SqlDataProvider.GetResultBySql(sqlstr,
                SqlDataProvider.CreateSqlParameter("@loginname", SqlDbType.VarChar, username),
                SqlDataProvider.CreateSqlParameter("@pwd", SqlDbType.VarChar, userpwd));
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                Member member = new Member();
                member.id = ds.Tables[0].Rows[0]["id"].ToString();
                member.loginname = ds.Tables[0].Rows[0]["loginname"].ToString();
                member.pwd = ds.Tables[0].Rows[0]["pwd"].ToString();
                member.face = ds.Tables[0].Rows[0]["face"].ToString();
                member.truename = ds.Tables[0].Rows[0]["truename"].ToString();
                Session["Member"] = member;

                Response.Write("<script>alert('Login Successfully');location.href='Index.aspx'</script>");
            }
            else
            {
                Response.Write("<script>alert('Login Error')'</script>");
            }
        }
        catch
        {
            Response.Write("<script>alert('Login Error')'</script>");
        }
    }
}