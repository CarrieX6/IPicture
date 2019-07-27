using MicroSoft.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class Fans : System.Web.UI.Page
{
    public int seq = 1;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            string sqlstr = string.Empty;
            try
            {
                sqlstr = "select top 7 t1.*,t2.truename from Article t1 left join Member t2 on t1.opperson = t2.id where opperson=@opperson";
                DataSet ds = SqlDataProvider.GetResultBySql(sqlstr,
                    SqlDataProvider.CreateSqlParameter("@opperson", SqlDbType.VarChar, Request.QueryString["memberid"].ToString()));
                this.rpt_list.DataSource = ds;
                this.rpt_list.DataBind();
            }
            catch
            { }

            try
            {
                sqlstr = "select * from Member where id=@id";
                DataSet mds = SqlDataProvider.GetResultBySql(sqlstr,
                    SqlDataProvider.CreateSqlParameter("@id", SqlDbType.VarChar, Request.QueryString["memberid"].ToString()));
                rpt_member.DataSource = mds;
                rpt_member.DataBind();
            }
            catch
            { }
        }
    }
    protected void btnNoAttention_Click(object sender, EventArgs e)
    {
        Member member = (Member)Session["Member"];
        if (member != null)
        {
            string sqlstr = "delete from Fans where fansid=@fansid and owerid=@owerid";
            int res = SqlDataProvider.ExecuteBySql(sqlstr,
                SqlDataProvider.CreateSqlParameter("@fansid", SqlDbType.VarChar, Request.QueryString["memberid"].ToString()),
                SqlDataProvider.CreateSqlParameter("@owerid", SqlDbType.VarChar, member.id.ToString()));

            if (res > 0)
            {
                Response.Write("<script>alert('Unfollow Successfully!');location.href='Index.aspx'</script>");
            }
            else
            {
                Response.Write("<script>alert('Unfollow Error!')</script>");
            }
        }
        else
        {
            Response.Write("<script>alert('Login First!')</script>");
        }
    }
}