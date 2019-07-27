using MicroSoft.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            string keyword = string.Empty;
            try
            {
                keyword = Request.QueryString["keyword"].ToString();
            }
            catch
            { }

            string sqlstr = string.Empty;
            if (!string.IsNullOrEmpty(keyword))
            {
                sqlstr = "select t1.* from Article t1 where title like '%" + keyword + "%' order by opdate desc";
            }
            else
            {
                sqlstr = "select t1.* from Article t1 order by opdate desc";
            }
            DataSet ds = SqlDataProvider.GetResultBySql(sqlstr);
            this.rpt_list.DataSource = ds;
            this.rpt_list.DataBind();
        }
    }
}