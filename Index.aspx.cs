using MicroSoft.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class Index : System.Web.UI.Page
{
    public static Member member = new Member();
    public int seq = 1;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            member = (Member)Session["Member"];
        }
        catch
        {
        }

        if (!Page.IsPostBack && member != null)
        {
            string sqlstr = string.Empty;
            try
            {
                sqlstr = "select t1.*,t2.truename from Article t1 left join Member t2 on t1.opperson = t2.id where opperson=@opperson order by opdate desc";
                DataSet ds = SqlDataProvider.GetResultBySql(sqlstr,
                    SqlDataProvider.CreateSqlParameter("@opperson", SqlDbType.VarChar, member.id.ToString()));
                this.rpt_list.DataSource = ds;
                this.rpt_list.DataBind();
            }
            catch
            { }

            try
            {
                //粉丝图片
                sqlstr = "select top 7 * from dbo.Article where opperson in(select fansid from fans where owerid=@owerid order by opdate desc)";
                DataSet dsfanspic = SqlDataProvider.GetResultBySql(sqlstr,
                        SqlDataProvider.CreateSqlParameter("@owerid", SqlDbType.VarChar, member.id.ToString()));
                rpt_fansPic.DataSource = dsfanspic;
                rpt_fansPic.DataBind();
            }
            catch
            { }

            try
            {
                sqlstr = "select * from dbo.Member where id in(select fansid from fans where owerid=@owerid)";
                DataSet dsfans = SqlDataProvider.GetResultBySql(sqlstr,
                        SqlDataProvider.CreateSqlParameter("@owerid", SqlDbType.VarChar, member.id.ToString()));
                rpt_fans.DataSource = dsfans;
                rpt_fans.DataBind();
            }
            catch
            { }

        }
    }

    protected void rpt_list_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "Delete")
        {
            int id = int.Parse(e.CommandArgument.ToString());

            string sqlstr = "delete from dbo.Article where id=@id";
            SqlDataProvider.ExecuteBySql(sqlstr, SqlDataProvider.CreateSqlParameter("@id", SqlDbType.VarChar, id));

            sqlstr = "delete from dbo.Comment where aid=@id";
            SqlDataProvider.ExecuteBySql(sqlstr, SqlDataProvider.CreateSqlParameter("@id", SqlDbType.VarChar, id));

            Response.Redirect(Request.Url.ToString());
        }
    }
}