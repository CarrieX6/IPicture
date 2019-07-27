﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Index.aspx.cs" Inherits="Index" %>

<%@ Register src="HeadControl.ascx" tagname="HeadControl" tagprefix="uc1" %>

<%@ Register src="FootControl.ascx" tagname="FootControl" tagprefix="uc2" %>

<!DOCTYPE HTML>
<html class="no-js" lang="en">
	<head>
		<meta charset="utf-8">
		<meta http-equiv="x-ua-compatible" content="ie=edge">
		<meta name="author" content="smartit-source">
		<meta name="description" content="">
		<meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">	
		<!-- title here -->
		<title>IPicture</title>		
        <!-- Favicon and Touch Icons -->
        <link rel="shortcut icon" href="assets/images/fav.png">
		<!-- Place favicon.ico in the root directory -->
		<link rel="apple-touch-icon" href="apple-touch-icon.png">
		<!-- Fonts -->
		<link href="https://fonts.googleapis.com/css?family=Poppins:300,400,500,600,700,800,900" rel="stylesheet">
		<link rel="stylesheet" href="assets/css/font-awesome.min.css">
		<link rel="stylesheet" href="assets/css/flaticon.css">
		<!-- Plugin CSS -->
		<link rel="stylesheet" href="assets/css/bootstrap.min.css">
		<link rel="stylesheet" href="assets/css/animate.css">
		<link rel="stylesheet" href="assets/css/magnific-popup.css">
		<link rel="stylesheet" href="assets/css/owl.carousel.css">
		<link rel="stylesheet" href="assets/css/owl.theme.css">
		<link rel="stylesheet" href="assets/css/owl.transitions.css">	
		<link rel="stylesheet" href="assets/css/jquery.barfiller.css" />					
		<!--Theme custom css -->
		<link rel="stylesheet" href="assets/css/style.css">
		<!--Theme Responsive css-->
		<link rel="stylesheet" href="assets/css/responsive.css" />
		<script src="assets/js/vendor/modernizr-2.8.3-respond-1.4.2.min.js"></script>
	</head>
	<body>
		<form id="form1" runat="server">
		<!-- header area start here -->
		
        <uc1:HeadControl ID="HeadControl1" runat="server" />

		<!-- header area start here -->

		<!-- banner area start here -->
		<div class="banner-area banner-three" style="padding-top:10px">
			<div class="container">
				<div class="row">
					<div class="col-md-6">
						<div class="banner-content">
                            <br /><br />
							<div class="banner-title white">
								<h1><%=member != null ? member.truename : "" %></h1>
							</div>
							
						</div>
					</div>
				</div>
			</div>
		    
		</div>
		<!-- banner area start here -->
		<!-- post area start here	 -->
		<div class="post-two-area">
			<div class="container">
				<div class="row">
					<div class="col-lg-8">
						<div class="post-list">

                            <asp:Repeater ID="rpt_list" runat="server" OnItemCommand="rpt_list_ItemCommand">
                            <ItemTemplate>
							<div class="single-post">
								<div class="inner-post">
									<div class="post-img">
										<a href="#"><img src="assets/images/blog/<%#Eval("pic") %>" alt="blog"></a>
									</div>
									<div class="post-info">
										<div class="post-title">
											<h3><a href="Pic_details.aspx?id=<%#Eval("id") %>"><%#Eval("title") %></a></h3>
										</div>
										<div class="post-content">
											<p><%#Eval("describe") %></p>
										</div>
										<div class="blog-meta fix" >
											<div class="meta-left pull-left">
												<ul>
													<li> <span class="flaticon-man-user user"></span> <p>By <a href="#"><%#Eval("truename") %></a> </p></li>
													<li> <span class="flaticon-calendar clendar"></span> <p><%#Eval("opdate","{0:d}") %></p></li>											
												</ul>
											</div>
											<div class="post-readmore pull-right">
												<a href="Pic_details.aspx?id=<%#Eval("id") %>" class="readmore-btn">View Comment</a>
                                                <br />
                                                <asp:ImageButton ID="Delete" runat="server" ImageUrl="~/assets/images/delete.png" Height="30px" Width="30px" style="position: absolute; border: 0; right: 15px; bottom: 15px;" CommandName="Delete" CommandArgument='<%#Eval("id") %>'/>
											</div>
										</div>
									</div>
								</div>
								<div class="post-date one">
									<span>0<%=seq++ %></span></div>
							</div>
                            </ItemTemplate>
							</asp:Repeater>

						</div>
					</div>
					<div class="col-lg-4">
						<div class="sidebar-widget">
							<div class="single-widget profile-widget box-shadow">
								<div class="widget-inner text-center">
									<div class="profile-img">
										<a href="#"><img src="assets/images/profile/<% = member != null ? member.face : "no.png" %>" alt="pro" />
									</div>
									<div class="profile-name">
										<h3><a href="#"><%=member != null ? member.truename : "No Login" %></a></h3>
									</div>
								</div>
							</div>
							
							<div class="single-widget recent-post-widget box-shadow">
								<div class="widget-inner">
									<div class="widget-title">
										<h3>Following Pictures</h3>
									</div>
									<div class="recent-post-list">
                                        <asp:Repeater ID="rpt_fansPic" runat="server">
                                        <ItemTemplate>
										<div class="single-post">
											<div class="posty-img">
												<a href="Pic_details.aspx?id=<%#Eval("id") %>"><img src="assets/images/blog/<%#Eval("pic") %>" alt="post"></a>
											</div>
											<div class="post-title">
												<h4><a href="Pic_details.aspx?id=<%#Eval("id") %>"><%#Eval("title") %></a></h4>
											</div>
										</div>
                                        </ItemTemplate>
                                        </asp:Repeater>
									</div>
								</div>
							</div>
							
							<div class="single-widget instagram-widget box-shadow">
								<div class="widget-inner">
									<div class="widget-title">
										<h3>Following Users</h3>
									</div>
									<div class="instagram-list">
										<ul>
                                            <asp:Repeater ID="rpt_fans" runat="server">
                                            <ItemTemplate>
											<li><a href="Fans.aspx?memberid=<%#Eval("id") %>"><img src="assets/images/profile/<%#Eval("face") %>" alt="instagram"></a></li>
                                            </ItemTemplate>
											</asp:Repeater>							
										</ul>
									</div>
								</div>
							</div>
						</div>
					</div>
				</div>
			</div>
			<!-- leaf left area start here	 -->
			<div class="leaf-left">
				<img src="assets/images/leaf-left.png" alt="leaf-right">
			</div>
			<!-- leaf left area end here	 -->
			<!-- leaf right area start here	 -->
			<div class="leaf-right">
				<img src="assets/images/leaf-right.png" alt="leaf-right">
			</div>
			<!-- leaf right area end here	 -->
		</div>
		<!-- post area end here	 -->	
		<!-- footer area start here -->
            <uc2:FootControl ID="FootControl1" runat="server" />
		<!-- footer area end here -->		
		<!-- js file start -->
		<script src="assets/js/vendor/jquery-1.12.0.min.js"></script>
		<script src="assets/js/plugins.js"></script>
		<script src="assets/js/Popper.js"></script>
		<script src="assets/js/bootstrap.min.js"></script>
		<script src="assets/js/jquery.magnific-popup.min.js"></script>
		<script src="assets/js/owl.carousel.min.js"></script>
		<script src="assets/js/isotope.pkgd.min.js"></script>
		<script src="assets/js/imagesloaded.pkgd.min.js"></script>	
		<script src="assets/js/scrollup.js"></script>	
		<script src="assets/js/jquery.counterup.min.js"></script>	
		<script src="assets/js/waypoints.min.js"></script>	
		<script src="assets/js/jquery.appear.js"></script>	
		<script src="assets/js/jquery.barfiller.js"></script>																																		
		<script src="assets/js/main.js"></script>
		<!-- End js file -->
	        
	    </form>
	</body>
</html>
