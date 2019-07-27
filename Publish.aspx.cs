using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using MicroSoft.EnterpriseLibrary.Data;
public partial class Publish : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            string sqlstr = "select * from Category";
            DataSet ds= SqlDataProvider.GetResultBySql(sqlstr);
            this.chkCartegory.DataSource = ds;
            this.chkCartegory.DataTextField = "name";
            this.chkCartegory.DataValueField = "code";
            this.chkCartegory.DataBind();
        }
    }
    protected void btnUp_Click(object sender, EventArgs e)
    {
        try
        {
            string filename = this.FileUp.FileName;
            string extend = filename.Substring(filename.LastIndexOf(".") + 1);
            /*if (extend.ToUpper() != "JPG" && extend.ToUpper() != "PNG")
            {
                Response.Write("<script>alert('Picture format error(need .jpg or .png)!')</script>");
            }
            */
            //else
            //{
                this.FileUp.SaveAs(Server.MapPath(@"assets/images/blog/") + filename);
                this.HidPic.Value = filename;
                Response.Write("<script>alert('Upload Successfully!');</script>");
            //}
        }
        catch
        { }
    }

    protected void btnPublish_Click(object sender, EventArgs e)
    {
        string title = this.txtTitle.Value;
        string pic = this.HidPic.Value;
        string describe = this.txtdescribe.Value;
        Member member = (Member)Session["Member"];
        string category = string.Empty;
        for (int i = 0; i < chkCartegory.Items.Count; i++)
        {
            if (chkCartegory.Items[i].Selected)
            {
                if (string.IsNullOrEmpty(category))
                {
                    category = chkCartegory.Items[i].Value;
                }
                else
                {
                    category = category + " " + chkCartegory.Items[i].Value;
                }
            }
        }


        try
        {
            string sqlstr = "insert into Article(title, pic, opdate, opperson,describe,category) values(@title, @pic, getdate(), @opperson,@describe,@category)";
            int res = SqlDataProvider.ExecuteBySql(sqlstr,
                SqlDataProvider.CreateSqlParameter("@title", SqlDbType.VarChar, title),
                SqlDataProvider.CreateSqlParameter("@pic", SqlDbType.VarChar, pic),
                SqlDataProvider.CreateSqlParameter("@opperson", SqlDbType.VarChar, member.id.ToString()),
                SqlDataProvider.CreateSqlParameter("@describe", SqlDbType.VarChar, describe),
                SqlDataProvider.CreateSqlParameter("@category", SqlDbType.VarChar, category));
            if (res > 0)
            {
                Response.Write("<script>alert('Publis Successfully!');location.href='Index.aspx'</script>");
            }
            else
            {
                Response.Write("<script>alert('Publis Error!')</script>");
            }
        }
        catch
        {
            Response.Write("<script>alert('Publis Error!')</script>");
        }
    }
}