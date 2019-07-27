<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Pic_details.aspx.cs" Inherits="Pic_details" %>

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
		<link rel="stylesheet" href="assets/css/jquery.barfiller.css">							
		<!--Theme custom css -->
		<link rel="stylesheet" href="assets/css/style.css">
		<!--Theme Responsive css-->
		<link rel="stylesheet" href="assets/css/responsive.css" />
		<script src="assets/js/vendor/modernizr-2.8.3-respond-1.4.2.min.js"></script>
        <script type="text/javascript">
            function DataCheck()
            {
                var mid = $("#HidID").val();
                var message = $("#txtMessage").val();
               
                if (mid == undefined || mid == "")
                {
                    alert("Please Login first!");
                    return false;
                }

                if (message == undefined || message == "") {
                    alert("Comment must be filled!");
                    return false;
                }
                return true;
            }
        </script>
	</head>
	<body>
		<form id="form1" runat="server">
		<!-- header area start here -->
		<uc1:HeadControl ID="HeadControl1" runat="server" />
		<!-- header area start here -->
		<!-- breadcrumb area start here	 -->
		    
		<div class="breadcrumb-area" style="padding-top:60px">
			<div class="container">
				<div class="row">
					<div class="col-sm-12">
						<div class="breadcrumb-title">
							<h2>View Page</h2>
						</div>
						<div class="breadcrumb-sibtitle">
							<h4>Comment Details</h4>
						</div>
					</div>
				</div>
			</div>
		</div>
		<!-- breadcrumb area end here	 -->	
		<!-- portfolio area start here	 -->
		<div class="blog-details-area section">
			<div class="container">
				<div class="row">
					<div class="col-lg-8">

                        <asp:Repeater ID="rpt_details" runat="server">
                        <ItemTemplate>
						<div class="single-post box-shadow">
							<div class="post-thumnile">
								<div class="slide-thumbnile">
									<div class="single-image">
										<img src="assets/images/blog/<%#Eval("pic") %>" alt="blog">
										<div class="post-date">
											<span>01</span>
										</div>
									</div>
								</div>
								
							</div>
							<div class="post-title">
								<h3><a href="Pic_details.aspx?id=<%#Eval("id") %>"><%#Eval("title") %></a></h3>
							</div>
							<div class="blog-meta">
								<ul>
									<li> <span class="flaticon-man-user user"></span> <p>By <a href="#"><%#Eval("truename") %></a> </p></li>
									<li> <span class="flaticon-calendar clendar"></span> <p><%#Eval("opdate","{0:d}") %></p></li>																												
								</ul>
							</div>
							<div class="blog-content">
								<%#Eval("describe") %>
							</div>
							
						</div>	
                        </ItemTemplate>
						</asp:Repeater>
						
						<div class="comment-area box-shadow mt-70">
							<div class="section-title">
								<h3>Comments</h3>
							</div>
							<asp:Repeater ID="rpt_comment" runat="server">
                            <ItemTemplate>
                            <div class="comment-list">
								<div class="single-comment">
									<div class="member-image">
										<a href="#"><img src="assets/images/profile/<%#Eval("face") %>" alt="comment img"></a>
									</div>

                                    
									<div class="comment-info">
										<div class="comment-title">
											<h4><a href="#"><%#Eval("truename") %></a> <span><%#Eval("commentDate","{0:d}") %></span></h4>
										</div>
										<div class="comment-content">
											<p><%#Eval("comment")%></p>
										</div>
									</div>
                                    
								</div>	
							</div>
                            </ItemTemplate>
                            </asp:Repeater>
							<div class="comment-from mt-63">
								<div class="section-title">
									<h3>Leave A Comment</h3>
								</div>
									<div class="row">
										<div class="col-lg-12">
											<div class="form-group box-shadow">
												<input type="text" class="form-control" id="txtTrueName" name="name" placeholder="True name" runat="server">
                                                <asp:HiddenField ID="HidID" runat="server" />
											</div>
										</div>
										
										<div class="col-lg-12">
											<div class="form-group box-shadow">
												<textarea name="txtMessage" id="txtMessage" cols="30" rows="10" style="width:100%" placeholder="Message" runat="server"></textarea>
											</div>
										</div>
										<div class="col-lg-12">
											<div class="from-btn">
                                                <asp:Button ID="btnSend" CssClass="primary-btn" runat="server" Text="Send" OnClientClick="return DataCheck()" OnClick="btnSend_Click" />
											</div>
										</div>
									</div>
							</div>
						</div>				
					</div>
					<div class="col-lg-4">
						<div class="sidebar-widget">
                            <asp:Repeater ID="rpt_member" runat="server">
                            <ItemTemplate>
							<div class="single-widget profile-widget box-shadow">
								<div class="widget-inner text-center">
									<div class="profile-img">
										<a href="#"><img src="assets/images/profile/<%#Eval("face") %>" alt="profile"></a>
									</div>
									<div class="profile-name">
										<h3><a href="#"><%#Eval("truename") %></a></h3>
                                        </ItemTemplate>
							</asp:Repeater>
										<p>
                                            <asp:HiddenField ID="HidSendMemberID" runat="server" />
                                            <asp:LinkButton ID="btnAttention" runat="server" OnClick="btnAttention_Click">Follow
                                            </asp:LinkButton>
										</p>
									</div>
								</div>
							</div>
                            
							<div class="single-widget recent-post-widget box-shadow">
								<div class="widget-inner">
									<div class="widget-title">
										<h3>Recent pictures</h3>
									</div>
									<div class="recent-post-list">
                                        <asp:Repeater ID="rpt_rec" runat="server">
									    <ItemTemplate>
                                        <div class="single-post">
											<div class="posty-img">
												<a href="Pic_details.aspx?id=<%#Eval("id") %>"><img src='assets/images/blog/<%#Eval("pic") %>' alt="post"></a>
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
		<!-- portfolio area start here	 -->		
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