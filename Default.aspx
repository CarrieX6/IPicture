<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

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
	</head>
	<body>
		<form id="form1" runat="server">
		<!-- header area start here -->
		<uc1:HeadControl ID="HeadControl1" runat="server" />
		<!-- header area start here -->
		<!-- breadcrumb area start here	 -->
		    
		<div class="breadcrumb-area" style="height:30px; padding-top:80px">
		</div>
		<!-- breadcrumb area end here	 -->	
		<!-- portfolio area start here	 -->
		<div class="portfolio-area section">
			<div class="container ">
				<div class="portfolio">
					<div class="row acurate">
						<div class="col-sm-12 acurate">
							<div class="filtering-button">
								<a class="active" data-filter="*"> All Categories</a>
								<a data-filter=".cat1">Landscape</a>
								<a data-filter=".cat2">Pets</a>
								<a data-filter=".cat3">Portrait</a>
								<a data-filter=".cat4">Life</a>
								<a data-filter=".cat5">Plant</a>
								<a data-filter=".cat6">Celebrity</a>
								<a data-filter=".cat7">Makeup</a>								
							</div>
						</div>
					</div>
					<div class="grid row acurate">
                        <asp:Repeater ID="rpt_list" runat="server">
                        <ItemTemplate>
						<div class="grid-item col-sm-6 col-md-4 col-xs-12 <%#Eval("category") %> acurate">
							<div class="single-portfolio">
								<div class="images">
									<a href="Pic_details.aspx?id=<%#Eval("id") %>"><img src="assets/images/blog/<%#Eval("pic") %>" alt="portfolio"></a>
								</div>
							</div>
						</div>
                        </ItemTemplate>
                        </asp:Repeater>
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