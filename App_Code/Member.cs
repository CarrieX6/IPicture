using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Member 的摘要说明
/// </summary>
public class Member
{
	public Member()
	{
		//
		// TODO: 在此处添加构造函数逻辑
		//
	}
    public string id { get; set; }
    public string loginname { get; set; }
    public string pwd { get; set; }
    public string truename { get; set; }
    public string face { get; set; }
}