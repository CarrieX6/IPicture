using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class HeadControl : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Member member = (Member)Session["Member"];
        if (member != null)
        {
            li_0.InnerText = "Wellcome " + member.truename;
            this.li_1.Visible = false;
            this.li_2.Visible = false;
            this.li_3.Visible = true;
            this.li_4.Visible = true;
            this.li_5.Visible = true;
        }
        else
        {
            li_0.InnerText = "Wellcome ";
            this.li_1.Visible = true;
            this.li_2.Visible = true;
            this.li_3.Visible = false;
            this.li_4.Visible = false;
            this.li_5.Visible = false;
        }

    }
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        Response.Redirect("Default.aspx?keyword=" + this.txtKeyWord.Value);
    }
}