using MicroSoft.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Pic_details : System.Web.UI.Page
{
    public Member member = new Member();
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            member = (Member)Session["Member"];
        }
        catch
        {
        }

        if (!Page.IsPostBack)
        {
            string id = Request.QueryString["id"].ToString();
            string sqlstr = "select t1.*,t2.truename from Article t1 left join Member t2 on t1.opperson = t2.id where t1.id=@id";
            DataSet ds = SqlDataProvider.GetResultBySql(sqlstr,
                SqlDataProvider.CreateSqlParameter("@id", SqlDbType.VarChar, id));

            string memberid = ds.Tables[0].Rows[0]["opperson"].ToString();

            rpt_details.DataSource = ds;
            rpt_details.DataBind();

            //取发布图片的会员

            sqlstr = "select * from Member where id = @id";
            ds = SqlDataProvider.GetResultBySql(sqlstr,
                SqlDataProvider.CreateSqlParameter("@id", SqlDbType.VarChar, memberid));
            rpt_member.DataSource = ds;
            rpt_member.DataBind();
            HidSendMemberID.Value = ds.Tables[0].Rows[0]["id"].ToString();

            sqlstr = "select top 7 t1.*,t2.truename from Article t1 left join Member t2 on t1.opperson = t2.id where t1.opperson=@opperson order by opdate desc";
            ds = SqlDataProvider.GetResultBySql(sqlstr,
                SqlDataProvider.CreateSqlParameter("@opperson", SqlDbType.VarChar, memberid));
            rpt_rec.DataSource = ds;
            rpt_rec.DataBind();

            //当前会员
            if (member != null)
            {
                this.txtTrueName.Value = member.truename;
                this.HidID.Value = member.id.ToString();
            }


            #region 绑定评论
            sqlstr = "select t1.*,t2.truename,t2.face from dbo.Comment t1 left join Member t2 on t1.commperson = t2.id where aid=@aid";
            ds = SqlDataProvider.GetResultBySql(sqlstr,
                SqlDataProvider.CreateSqlParameter("@aid", SqlDbType.VarChar, Request.QueryString["id"].ToString()));
            rpt_comment.DataSource = ds;
            rpt_comment.DataBind();
            #endregion
        }
    }
    protected void btnSend_Click(object sender, EventArgs e)
    {
        string sqlstr = "insert into Comment(comment, aid, commentDate, commperson) values(@comment, @aid, getdate(), @commperson)";
        int res = SqlDataProvider.ExecuteBySql(sqlstr,
            SqlDataProvider.CreateSqlParameter("@comment", SqlDbType.VarChar, this.txtMessage.Value),
            SqlDataProvider.CreateSqlParameter("@aid", SqlDbType.VarChar, Request.QueryString["id"].ToString()),
            SqlDataProvider.CreateSqlParameter("@commperson", SqlDbType.VarChar, this.HidID.Value));
        if (res > 0)
        {
            Response.Write("<script>alert('Send Successfully!');location.href='Pic_details.aspx?id=" + Request.QueryString["id"].ToString() + "'</script>");
        }
        else
        {
            Response.Write("<script>alert('Send Error')</script>");
        }
    }
    protected void btnAttention_Click(object sender, EventArgs e)
    {
        member = (Member)Session["Member"];
        if (member != null)
        {
            if (this.HidSendMemberID.Value == member.id.ToString())
            {
                Response.Write("<script>alert('Cannot Follow Yourself!')</script>");
            }
            else
            {
                string sqlstr = "select count(1) from Fans where fansid=@fansid and owerid=@owerid";
                Object obj = SqlDataProvider.GetScalarBySql(sqlstr,
                    SqlDataProvider.CreateSqlParameter("@fansid",SqlDbType.VarChar,this.HidSendMemberID.Value),
                    SqlDataProvider.CreateSqlParameter("@owerid", SqlDbType.VarChar, this.member.id.ToString()));
                if (Convert.ToInt32(obj) > 0)
                {
                    Response.Write("<script>alert('Already Followed!')</script>");
                }
                else
                {
                    sqlstr = "insert into Fans(fansid, owerid) values(@fansid, @owerid)";
                    int res = SqlDataProvider.ExecuteBySql(sqlstr,
                        SqlDataProvider.CreateSqlParameter("@fansid", SqlDbType.VarChar, this.HidSendMemberID.Value),
                        SqlDataProvider.CreateSqlParameter("@owerid", SqlDbType.VarChar, member.id.ToString()));
                    if (res > 0)
                    {
                        Response.Write("<script>alert('Follow Successfully!');</script>");
                    }
                    else
                    {
                        Response.Write("<script>alert('Follow Error!')</script>");
                    }
                }
            }
        }
        else
        {
            Response.Write("<script>alert('Login First!')</script>");
        }
    }
}