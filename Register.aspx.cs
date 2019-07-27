using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using MicroSoft.EnterpriseLibrary.Data;
public partial class Register : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void btnUp_Click(object sender, EventArgs e)
    {
        try
        {
            string filename = this.FileUp.FileName;
            string extend = filename.Substring(filename.LastIndexOf(".") + 1);
            if (extend.ToUpper() != "JPG" && extend.ToUpper() != "PNG")
            {
                Response.Write("<script>alert('Picture format error(need .jpg or .png)!')</script>");
            }
            else
            {
                this.FileUp.SaveAs(Server.MapPath(@"assets\images\profile\") + filename);
                this.HidFace.Value = filename;
                Response.Write("<script>alert('Upload Successfully!')</script>");
            }
        }
        catch
        { }
    }

    protected void btnRegister_Click(object sender, EventArgs e)
    {
        string username = this.txtName.Value;
        string userpwd = this.txtPwd.Value;
        string truename = this.txtTrueName.Value;
        string face = this.HidFace.Value;
        try
        {
            string sqlstr = "select count(1) from Member where loginname=@loginname";
            Object obj = SqlDataProvider.GetScalarBySql(sqlstr,
                SqlDataProvider.CreateSqlParameter("@loginname",SqlDbType.VarChar,username));
            if (Convert.ToInt32(obj) > 0)
            {
                Response.Write("<script>alert('This loginName already exists!')</script>");
                return;
            }
            else
            {
                sqlstr = "insert into Member(loginname, pwd, face, truename) values(@loginname, @pwd, @face, @truename)";
                int res = SqlDataProvider.ExecuteBySql(sqlstr,
                    SqlDataProvider.CreateSqlParameter("@loginname", SqlDbType.VarChar, username),
                    SqlDataProvider.CreateSqlParameter("@pwd", SqlDbType.VarChar, userpwd),
                    SqlDataProvider.CreateSqlParameter("@face", SqlDbType.VarChar, face),
                    SqlDataProvider.CreateSqlParameter("@truename", SqlDbType.VarChar, truename));
                if (res >0)
                {
                    sqlstr = "select * from Member where loginname=@loginname";
                    DataSet ds = SqlDataProvider.GetResultBySql(sqlstr,
                        SqlDataProvider.CreateSqlParameter("@loginname", SqlDbType.VarChar, username));
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        Member member = new Member();
                        member.id = ds.Tables[0].Rows[0]["id"].ToString();
                        member.loginname = ds.Tables[0].Rows[0]["loginname"].ToString();
                        member.pwd = ds.Tables[0].Rows[0]["pwd"].ToString();
                        member.face = ds.Tables[0].Rows[0]["face"].ToString();
                        member.truename = ds.Tables[0].Rows[0]["truename"].ToString();
                        Session["Member"] = member;
                    }

                    Response.Write("<script>alert('Register Successfully!');location.href='Index.aspx'</script>");
                }
                else
                {
                    Response.Write("<script>alert('Login Error')</script>");
                }
            }
        }
        catch
        {
            Response.Write("<script>alert('Login Error')</script>");
        }
    }
}