<%@ Control Language="C#" AutoEventWireup="true" CodeFile="HeadControl.ascx.cs" Inherits="HeadControl" %>
<!-- header area start here -->
		<header class="header-area" id="sticky">
			<div class="container">
				<div class="row">
					<div class="col-md-2 col-xs-6">
						<div class="log-area">
							<a href="index.aspx"><img src="assets/images/logo1.png" alt=""></a>
						</div>
					</div>
					<div class="col-md-9">
						<div class="menu-area white">
							<nav>
								<ul>
                                    <li>
                                        <input type="text" class="form-control" style="width:100%; height:24px" id="txtKeyWord" name="txtKeyWord" placeholder="search" runat="server">
                                    </li>
                                    <li><asp:ImageButton ID="btnQuery" runat="server" OnClick="btnQuery_Click" Height="24px" ImageUrl="~/assets/images/search.png" Width="24px" /></li>
									<li>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</li>
                                    <li>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</li>
                                    <li><a href="Default.aspx">Home</a></li>
									<li class="active sub"><a href="#">Member Center<span class="fa fa-angle-down"></span></a>
                                        <ul class="sub-menu">
											<li id="li_0" runat="server"></li>
                                            <li id="li_1" runat="server"><a href="Login.aspx">Login</a></li>
											<li id="li_2" runat="server"><a href="Register.aspx">Register</a></li>
                                            <li id="li_3" runat="server"><a href="Index.aspx">My Pictures</a></li>
                                            <li id="li_4" runat="server"><a href="Publish.aspx">Publish Picture</a></li>
                                            <li id="li_5" runat="server"><a href="LoginOut.aspx">Log Out</a></li>
										</ul>
									</li>					
								</ul>
							</nav>
						</div>
					</div>
					
				</div>
			</div>
		</header>
		<!-- header area start here -->